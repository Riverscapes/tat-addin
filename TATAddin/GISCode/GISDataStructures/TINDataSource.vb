Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.DataSourcesFile
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Carto
Imports GCDAddIn.GISCode.ErrorHandling.ExceptionUI

Namespace GISCode.GISDataStructures

    Public Class TINDataSource
        'Inherits GISDataStructures.GISDataSource

        Private m_pTIN As ITin
        Private Const m_nMaxTINPathLength As Integer = 116

#Region "Properties"
        Public ReadOnly Property ExtentOriginal As ESRI.ArcGIS.Geometry.IEnvelope
            Get
                Return Geodataset.Extent()
            End Get
        End Property

        Public ReadOnly Property FullPath As String
            Get
                Dim pDS As IDataset = Geodataset
                Return IO.Path.Combine(pDS.Workspace.PathName, pDS.Name)
            End Get
        End Property

        Public ReadOnly Property ExtentOriginalAsString As String
            Get
                Return ExtentOriginal.LowerLeft.X & " " & ExtentOriginal.LowerLeft.Y & " " & ExtentOriginal.LowerRight.X & " " & ExtentOriginal.UpperLeft.Y
            End Get
        End Property

        Public ReadOnly Property Geodataset As IGeoDataset
            Get
                Return DirectCast(m_pTIN, IGeoDataset)
            End Get
        End Property

        Public ReadOnly Property SpatialReference As ISpatialReference
            Get
                Return Geodataset.SpatialReference
            End Get
        End Property

#End Region

        Public Sub New(dFullPath As IO.DirectoryInfo)
            Me.new(dFullPath.FullName)
        End Sub

        Public Sub New(sFullPath As String)

            If String.IsNullOrEmpty(sFullPath) Then
                Throw New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.CriticalError, "The TIN path is an empty string")
            Else
                Try
                    If sFullPath.Length > m_nMaxTINPathLength Then
                        Throw New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.CriticalError, "The TIN path exceeds the maximum allowing length", "Move the TIN to a folder further up the directory hierarchy")
                    Else
                        If GISDataStructures.TINDataSource.Exists(sFullPath) Then
                            Dim pWSF As IWorkspaceFactory = GISCode.ArcMap.GetWorkspaceFactory(GISDataStorageTypes.TIN)
                            Dim pWS As IWorkspace = pWSF.OpenFromFile(IO.Path.GetDirectoryName(sFullPath), Nothing)
                            Dim pTWS As ITinWorkspace = pWS
                            m_pTIN = pTWS.OpenTin(IO.Path.GetFileName(sFullPath))
                        Else
                            Dim ex As New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.CriticalError, "The TIN path does not appear to be a valid directory")
                            ex.Data("TIN path") = sFullPath
                            Throw ex
                        End If
                    End If
                Catch ex As Exception
                    ex.Data("Full Path") = sFullPath
                    ex.Data("Actual path length") = sFullPath.Length.ToString
                    ex.Data("Max path length") = m_nMaxTINPathLength.ToString
                    Throw
                End Try
            End If
        End Sub

        Public Function Validate(ByRef lErrors As List(Of ErrorHandling.GISException), pReferenceSR As ISpatialReference) As Boolean

            Try
                If TypeOf pReferenceSR Is ISpatialReference Then
                    If Not CheckSpatialReferenceMatches(pReferenceSR) Then
                        Dim ex As New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.CriticalError, "Spatial reference does not match", "Reproject the TIN to the same spatial reference")
                        ex.Data("Violating Dataset") = FullPath
                        ex.Data("Violating dataset SR") = SpatialReference.Name
                        ex.Data("Reference dataset SR") = pReferenceSR.Name
                        lErrors.Add(ex)
                    End If
                End If

            Catch ex As GISException
                lErrors.Add(ex)
            End Try

            Return lErrors.Count = 0

        End Function

        Public Function CheckSpatialReferenceMatches(gReferenceSR As ISpatialReference) As Boolean

            Dim bResult As Boolean = False
            If TypeOf gReferenceSR Is ISpatialReference Then
                Dim pThisDataset As IClone = SpatialReference
                Dim pReference As IClone = gReferenceSR
                bResult = pThisDataset.IsEqual(pReference)
            End If
            Return bResult

        End Function

        Public Function ExtentOrthogonal(fCellSize As Double, Optional fPrecision As Double = -1, Optional fBuffer As Double = 0) As String

            Dim fOrthogonalityFactor As Double
            If fPrecision < 0 Then
                fOrthogonalityFactor = fCellSize
            Else
                fOrthogonalityFactor = Math.Max(fCellSize, fPrecision)
            End If

            Dim fLeft As Double = ExtentOriginal.LowerLeft.X
            If fCellSize >= 0 Then
                fLeft = fLeft / fOrthogonalityFactor
                fLeft = Math.Floor(fLeft)
                fLeft = fLeft * fOrthogonalityFactor
            End If
            fLeft -= fBuffer

            Dim fRight As Double = ExtentOriginal.LowerRight.X
            If fCellSize >= 0 Then
                fRight = fRight / fOrthogonalityFactor
                fRight = Math.Ceiling(fRight)
                fRight = fRight * fOrthogonalityFactor
            End If
            fRight += fBuffer

            Dim fTop As Double = ExtentOriginal.UpperLeft.Y
            If fCellSize >= 0 Then
                fTop = fTop / fOrthogonalityFactor
                fTop = Math.Ceiling(fTop)
                fTop = fTop * fOrthogonalityFactor
            End If
            fTop += fBuffer

            Dim fBottom As Double = ExtentOriginal.LowerLeft.Y
            If fCellSize >= 0 Then
                fBottom = fBottom / fOrthogonalityFactor
                fBottom = Math.Floor(fBottom)
                fBottom = fBottom * fOrthogonalityFactor
            End If
            fBottom -= fBuffer

            Return fLeft & " " & fBottom & " " & fRight & " " & fTop

        End Function

        ''' <summary>
        ''' Open an ArcCatalog browser window and navigate to a feature class
        ''' </summary>
        ''' <param name="sTitle">Browser window title</param>
        ''' <param name="sFolder">Output folder of selected feature class</param>
        ''' <param name="sFCName">Output feature class name</param>
        ''' <returns>The fully qualified (geoprocessing compatible) path to the selected feature class</returns>
        ''' <remarks>This method now works with both ShapeFile sources and geodatabase sources.</remarks>
        Public Shared Function BrowseOpen(ByVal sTitle As String, ByRef sFolder As String, ByRef sFCName As String) As String

            Dim pGxDialog As ESRI.ArcGIS.CatalogUI.IGxDialog = New ESRI.ArcGIS.CatalogUI.GxDialog
            Dim pFilterCol As ESRI.ArcGIS.Catalog.IGxObjectFilterCollection = pGxDialog
            pFilterCol.AddFilter(New ESRI.ArcGIS.Catalog.GxFilterTINDatasets, True)

            sFolder = ""
            sFCName = ""

            Dim pEnumGx As ESRI.ArcGIS.Catalog.IEnumGxObject = Nothing
            Dim pGxObject As ESRI.ArcGIS.Catalog.IGxObject = Nothing

            pGxDialog.RememberLocation = True
            pGxDialog.AllowMultiSelect = False
            pGxDialog.Title = sTitle

            Try
                If pGxDialog.DoModalOpen(0, pEnumGx) Then
                    pGxObject = pEnumGx.Next
                    Return pGxObject.FullName
                End If

            Catch ex As Exception
                Dim ex2 As New Exception("Error browsing for TIN", ex)
                ex2.Data.Add("sTitle", sTitle)
                ex2.Data.Add("sFolder", sFolder)
                ex2.Data.Add("sFCName", sFCName)
                Throw ex2
            End Try

            Return String.Empty

        End Function

        Public Shared Function BrowseOpen(ByRef txt As System.Windows.Forms.TextBox,
                                          sFormTitle As String,
                                          sErrorMessage As String) As String

            Dim sFolder As String = String.Empty
            Dim sName As String = String.Empty
            Dim sResult As String = String.Empty
            Try
                If String.IsNullOrEmpty(txt.Text) Then
                    If IO.Directory.Exists(txt.Text) Then
                        sFolder = IO.Path.GetDirectoryName(txt.Text)
                        sFolder = IO.Path.GetFileName(txt.Text)
                    End If
                End If

                sResult = BrowseOpen(sFormTitle, sFolder, sName)
                If Not String.IsNullOrEmpty(sResult) Then
                    txt.Text = sResult
                End If

            Catch ex As Exception
                ExceptionUI.HandleException(ex, sErrorMessage)
            End Try

            Return sResult

        End Function

        Public Shared Function BrowseSave(ByVal sFormTitle As String, Optional ByRef sWorkspace As String = Nothing, Optional ByRef sName As String = Nothing) As String

            Dim pGxDialog As ESRI.ArcGIS.CatalogUI.IGxDialog = New ESRI.ArcGIS.CatalogUI.GxDialog
            Dim pFilterCol As ESRI.ArcGIS.Catalog.IGxObjectFilterCollection = pGxDialog
            pFilterCol.AddFilter(New ESRI.ArcGIS.Catalog.GxFilterTINDatasets(), True)
            pGxDialog.RememberLocation = True
            pGxDialog.AllowMultiSelect = False
            pGxDialog.Title = sFormTitle

            If Not String.IsNullOrEmpty(sWorkspace) Then
                pGxDialog.StartingLocation = sWorkspace
            End If

            If TypeOf sWorkspace Is String Then
                sWorkspace = String.Empty
            End If

            If TypeOf sName Is String Then
                sName = String.Empty
            End If

            Dim sResult As String = String.Empty

            Try
                If pGxDialog.DoModalSave(0) Then
                    sWorkspace = pGxDialog.FinalLocation.FullName
                    sName = pGxDialog.Name
                    sResult = IO.Path.Combine(sWorkspace, sName) '  rResult = New Raster(IO.Path.Combine(sWorkspace, sName))
                End If

            Catch ex As Exception
                ex.Data("Title") = sFormTitle
                ex.Data("Folder") = sWorkspace
                ex.Data("Name") = sName
                Throw
            End Try

            Return sResult

        End Function


        ''' <summary>
        ''' Checks if a TIN surface exists
        ''' </summary>
        ''' <param name="sPath"></param>
        ''' <returns></returns>
        ''' <remarks>PGB 12 Jun 2014. Enhanced to not just check if the folder exists
        ''' but actually check using ArcObjects. Note that you can't do the simple
        ''' IWorkspace2.NameExists() and pass the TIN data type. Apparently TIN workspaces
        ''' don't exhibit the IWorkspace2 interface.</remarks>
        Public Shared Function Exists(sPath As String) As Boolean

            Dim bResult As Boolean = False
            If Not String.IsNullOrEmpty(sPath) Then
                If IO.Directory.Exists(sPath) Then
                    Try

                        Dim pWFS As IWorkspaceFactory = GISCode.ArcMap.GetWorkspaceFactory(GISDataStorageTypes.TIN)
                        If pWFS.IsWorkspace(IO.Path.GetDirectoryName(sPath)) Then
                            Dim pWS As IWorkspace = pWFS.OpenFromFile(IO.Path.GetDirectoryName(sPath), 0)
                            If TypeOf pWS Is ITinWorkspace Then
                                Dim pTWS As ITinWorkspace = pWS
                                bResult = pTWS.IsTin(IO.Path.GetFileNameWithoutExtension(sPath))
                            End If
                        End If
                    Catch ex As Exception
                        Dim ex2 As New Exception("Error checking if TIN workspace exists.", ex)
                        ex2.Data("TIN Path") = sPath
                        Throw ex2
                    End Try
                End If
            End If

            Return bResult

        End Function

        ''' <summary>
        ''' Get the elevation of a TIN at the specified point location
        ''' </summary>
        ''' <param name="gPoint">Point on the TINs surface</param>
        ''' <returns>Elevation of the point on the TINs</returns>
        ''' <remarks></remarks>
        Public Function GetElevation(gPoint As IPoint) As Double

            Dim fResult As Double = 0
            If TypeOf m_pTIN Is ITinSurface Then
                Dim pSurface As ITinSurface = m_pTIN
                fResult = pSurface.GetElevation(gPoint)
                If pSurface.IsVoidZ(fResult) Then
                    fResult = 0
                End If
            End If
            Return fResult
        End Function

        Public Sub Delete()

            If String.IsNullOrEmpty(Me.FullPath) Then
                Throw New ArgumentException("Provided path cannot be empty.", "sUserTINPath")
            End If

            Dim sTrueIO_Name As String = GetTrueTINPath(Me.FullPath)

            If System.IO.Directory.Exists(sTrueIO_Name) Then
                IO.Directory.Delete(sTrueIO_Name, True)
            End If


        End Sub

        Private Function GetTrueTINPath(ByVal sUserTINPath As String) As String

            If String.IsNullOrEmpty(sUserTINPath) Then
                Throw New ArgumentException("Provided path cannot be empty.", "sUserTINPath")
            End If

            'Create TIN path with attention to all lowercase in 'filename'
            Dim sTINDirectory As String = IO.Path.GetDirectoryName(sUserTINPath)
            Dim sTINFileName As String = IO.Path.GetFileName(sUserTINPath).ToLower
            Dim sTINFullPath As String = IO.Path.Combine(sTINDirectory, sTINFileName)

            If Not System.IO.Directory.Exists(sTINFullPath) Then
                Throw New Exception(String.Format("{0} does not exists.", sTINFullPath))
            End If

            Return sTINFullPath

        End Function

    End Class


End Namespace