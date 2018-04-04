
#Region "Imports"

Imports System.IO
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesFile
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.Geometry.esriGeometryType
Imports ESRI.ArcGIS.DataSourcesRaster
'
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' PGB - 8 Jun 2012. Do not import any modules or classes that are within the GISCode 
' namespace. The parent project for this code file (GCD, RBT etc) should import the
' "GISCode" namespace, making all the child libraries visible without needing to specify
' the child namespace here. For example "GCD.GISCode.ArcMap" or' "RBT.GISCode.ArcMap".
' This makes the code portable between different products.
'
' In the methods below, reference any code stored in the GISCode folder simply using
' the GISCode namespace. e.g. GISCode.ArcMap.FeatureClas...
'
#End Region

Namespace GISCode.GISDataStructures

    ''' <summary>
    ''' Basic GIS data geometry types
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum BasicGISTypes
        Point
        Line
        Polygon
        Raster
    End Enum

    Public Enum GISDataStorageTypes
        ShapeFile
        FileGeodatase
        RasterFile
        PersonalGeodatabase
        CAD
        TIN
    End Enum

    Public Enum BrowseGISTypes
        Point
        Line
        Polygon
        Raster
        TIN
        Any
    End Enum

    Public Enum BrowseVectorTypes
        Point
        Line
        Polygon
        CrossSections
    End Enum

    Public MustInherit Class GISDataSource

        Private Const m_nMaxPathLength As Integer = 255

        Private m_sDatasetName As String
        Private m_sDisplayName As String
        Private m_sFullPath As String
        Private m_sWorkspace As String
        Private m_pGeoDataset As IGeoDataset

        Public Sub New(sFullPath As String)

            If String.IsNullOrEmpty(sFullPath) Then
                Throw New ArgumentNullException("Full Path", "Empty or null string passed.")
            End If

            If sFullPath.Length > m_nMaxPathLength Then
                Throw New GISException(GISException.ErrorTypes.CriticalError, "The data source path length (" & sFullPath.Length.ToString & ") exceeds the maximum allowed (" & m_nMaxPathLength & ")", "Move the data source to a short path location higher up your file directory structure")
            End If
            '
            ' Set the basic path string properties
            '
            m_sFullPath = sFullPath
            m_sWorkspace = GetWorkspacePath(sFullPath)

            If GISDataStorageType = GISDataStorageTypes.FileGeodatase Then
                '
                ' Check for a slash **after** the file geodatabase name. This is
                ' needed if there is a feature dataset within the file geodatabase
                ' that contains the feature classes.
                '
                Dim nSlash As Integer = sFullPath.LastIndexOf(IO.Path.DirectorySeparatorChar)
                If nSlash > 0 Then
                    m_sDatasetName = sFullPath.Substring(nSlash + 1)
                    m_sDisplayName = m_sDatasetName
                Else
                    Dim ex As New GISException(GISException.ErrorTypes.CriticalError, "Unable to find dataset name in file geodatabase. Cannot find last index of folder slash character.")
                    ex.Data.Add("Full Path", sFullPath)
                    Throw ex
                End If
            Else
                '
                ' File data source. Could be shapefile, file based raster or folder based raster
                'New Code to check if the file exists - James Hensleigh 3/25
                If File.Exists(sFullPath) Then
                    m_sDatasetName = IO.Path.GetFileNameWithoutExtension(sFullPath)
                    m_sDisplayName = IO.Path.GetFileNameWithoutExtension(sFullPath)
                Else
                    'Throw New Exception("This file does not exist!!")
                End If
            End If

            'Dim pWS As IWorkspace = Workspace
            'Dim pDSEnum As IEnumDataset = pWS.Datasets(esriDatasetType.esriDTFeatureClass)
            'Dim pDS As IDataset = pDSEnum.Next
            'Do While Not pDS Is Nothing
            '    If TypeOf pDS Is IGeoDataset Then
            '        Dim pGDS As IGeoDataset = pDS
            '        If String.Compare(pDS.FullName.NameString, DatasetName, True) = 0 Then
            '            m_pGeoDataset = pDS
            '            Exit Do
            '        End If
            '    End If
            '    pDS = pDSEnum.Next
            'Loop
        End Sub

        Public Sub New(fiFullPath As FileInfo)
            Me.New(fiFullPath.FullName)
        End Sub

        Public Sub New(sFolder As String, sFileName As String)
            Me.New(IO.Path.Combine(sFolder, sFileName))
        End Sub

        Protected Overrides Sub Finalize()
            'If TypeOf m_pGeoDataset Is IGeoDataset Then
            '    Runtime.InteropServices.Marshal.ReleaseComObject(m_pGeoDataset)
            '    m_pGeoDataset = Nothing
            'End If
            MyBase.Finalize()
        End Sub

#Region "Properties"

        Public Overrides Function ToString() As String
            Return m_sDisplayName
        End Function

        Public ReadOnly Property FullPath As String
            Get
                Return m_sFullPath
            End Get
        End Property

        Public ReadOnly Property DatasetName As String
            Get
                Return m_sDatasetName
            End Get
        End Property


        Public ReadOnly Property WorkspacePath As String
            Get
                Return Workspace.PathName
            End Get
        End Property

        Public ReadOnly Property WorkspaceType As esriWorkspaceType
            Get
                Dim pWS As IWorkspace = Workspace
                Return pWS.Type
            End Get
        End Property

        Public ReadOnly Property GISDataStorageType As GISDataStorageTypes
            Get
                Return GetWorkspaceType(m_sFullPath)
            End Get
        End Property

        ''' <summary>
        ''' Get the workspace object for this data source
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Now just calls GISDataSource method in base class.</remarks>
        Public ReadOnly Property Workspace As IWorkspace
            Get
                Return GetWorkspace(m_sFullPath, GISDataStorageType)
            End Get
        End Property

        Protected Property GeoDataset As IGeoDataset
            Get
                Return m_pGeoDataset
            End Get
            Set(value As IGeoDataset)
                m_pGeoDataset = value
            End Set
        End Property

        Public ReadOnly Property SpatialReference As ISpatialReference
            Get
                Return m_pGeoDataset.SpatialReference
            End Get
        End Property

        Public ReadOnly Property Extent As IEnvelope
            Get
                Return m_pGeoDataset.Extent
            End Get
        End Property

        Public ReadOnly Property ExtentAsString As String
            Get
                Dim pExtent As IEnvelope = Extent
                Dim sResult As String = pExtent.XMin & " " & pExtent.YMin & " " & pExtent.XMax & " " & pExtent.YMax
                Return sResult
            End Get
        End Property

        Public ReadOnly Property IsMetres As Boolean
            Get
                Dim bIsMetres As Boolean = False
                Dim pLinearUnit As ILinearUnit = LinearUnits
                If TypeOf pLinearUnit Is ILinearUnit Then
                    bIsMetres = (String.Compare(pLinearUnit.Name, "In meters.", True) = 0)
                End If
                Return bIsMetres
            End Get
        End Property

        Public ReadOnly Property LinearUnits As ILinearUnit
            Get
                Dim pLinearUnit As ILinearUnit = Nothing
                If TypeOf SpatialReference Is IProjectedCoordinateSystem Then
                    Dim pProjCoordSys As IProjectedCoordinateSystem = SpatialReference
                    pLinearUnit = pProjCoordSys.CoordinateUnit
                End If
                Return pLinearUnit
            End Get
        End Property

#End Region

        Public Overridable Function Validate(ByRef lErrors As List(Of GISException), gReferenceSR As ISpatialReference) As Boolean

            Dim nInitialErrors As Integer = lErrors.Count
            Try
                If TypeOf SpatialReference Is ISpatialReference Then
                    If TypeOf gReferenceSR Is ISpatialReference Then
                        If Not CheckSpatialReferenceMatches(gReferenceSR) Then
                            Dim ex As New GISException(GISException.ErrorTypes.CriticalError, "The spatial reference does not match.", "Reproject the " & DatasetName & " dataset to the same spatial reference.")
                            ex.Data.Add("Violating dataset SR", SpatialReference.Name)
                            ex.Data.Add("Reference dataset SR", gReferenceSR.Name)
                            Throw ex
                        End If
                    End If
                Else
                    Throw New GISException(GISException.ErrorTypes.CriticalError, "The dataset is missing a spatial reference")
                End If

            Catch ex As GISException
                ex.Data.Add("GIS Data Source Path", FullPath)
                lErrors.Add(ex)
            End Try

            Return lErrors.Count <= nInitialErrors

        End Function

        Public Function CheckSpatialReferenceMatches(gReferenceSR As ISpatialReference) As Boolean

            Dim bResult As Boolean = False
            If TypeOf gReferenceSR Is ISpatialReference Then
                If TypeOf m_pGeoDataset Is IGeoDataset Then
                    If TypeOf m_pGeoDataset.SpatialReference Is ISpatialReference Then
                        Dim pThisDataset As IClone = SpatialReference
                        Dim pReference As IClone = gReferenceSR
                        bResult = pThisDataset.IsEqual(pReference)
                    Else
                        If gReferenceSR.Name.Contains("Unknown") Then
                            Return True
                        End If
                    End If
                End If
            End If
            Return bResult

        End Function

        ''' <summary>
        ''' Safely add this vector GIS data source to a user interface combox if it doesn't already exist in the combobox
        ''' </summary>
        ''' <param name="cbo">Dropdown combobox</param>
        ''' <remarks>Checks to ensure that the vector GIS data source doesn't already exist in the combobox</remarks>
        Public Sub AddToCombo(ByRef cbo As System.Windows.Forms.ComboBox, Optional bSelect As Boolean = True)

            Dim bAlreadyInList As Boolean = False
            Dim nIndex As Integer = -1
            For nIndex = 0 To cbo.Items.Count - 1
                If TypeOf cbo.Items(nIndex) Is VectorDataSource Then
                    If String.Compare(DirectCast(cbo.Items(nIndex), GISDataSource).FullPath, FullPath, True) = 0 Then
                        bAlreadyInList = True
                        Exit For
                    End If
                End If
            Next

            If Not bAlreadyInList Then
                nIndex = cbo.Items.Add(Me)
            End If

            If bSelect AndAlso nIndex >= 0 AndAlso nIndex <= cbo.Items.Count - 1 Then
                cbo.SelectedIndex = nIndex
            End If

        End Sub

        Public Function AddToMap(pArcMap As IApplication, _
                                 Optional sLayerName As String = "",
                                 Optional sGroupLayer As String = "",
                                 Optional sSymbologyLayerFile As String = "",
                                 Optional bAddToMapIfPresent As Boolean = False) As ILayer

            If pArcMap Is Nothing Then
                Return Nothing
            End If
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Only add if it doesn't exist already
            '
            Dim pResultLayer As ILayer = GISCode.ArcMap.GetLayerBySource(pArcMap, FullPath)
            If TypeOf pResultLayer Is ILayer Then
                If Not bAddToMapIfPresent Then
                    Return pResultLayer
                End If
            End If
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Confirm that the symbology layer file exists
            '
            If Not String.IsNullOrEmpty(sSymbologyLayerFile) Then
                If Not IO.File.Exists(sSymbologyLayerFile) Then
                    Dim ex As New Exception("A symbology layer file was provided, but the file does not exist")
                    ex.Data.Add("Data Source", FullPath)
                    ex.Data.Add("Layer file", sSymbologyLayerFile)
                    Throw ex
                End If
            End If
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Get or create the group layer if one is specified.
            '
            Dim pGrpLayer As IGroupLayer = Nothing
            If Not String.IsNullOrEmpty(sGroupLayer) Then
                pGrpLayer = GISCode.ArcMap.GetGroupLayer(pArcMap, sGroupLayer)
            End If

            Dim pMXDoc As ESRI.ArcGIS.ArcMapUI.IMxDocument = pArcMap.Document
            Dim pMap As ESRI.ArcGIS.Carto.IMap = pMXDoc.FocusMap

            If TypeOf Me Is GISDataStructures.Raster Then
                Dim gRaster = DirectCast(Me, GISDataStructures.Raster)
                gRaster.CalculateStatistics()
                If Not String.IsNullOrEmpty(sSymbologyLayerFile) Then
                    pResultLayer = gRaster.ApplySymbology(sSymbologyLayerFile)
                Else
                    pResultLayer = gRaster.RasterLayer
                End If
            Else
                Dim pFL = New FeatureLayer
                pFL.FeatureClass = DirectCast(Me, VectorDataSource).FeatureClass
                pResultLayer = pFL
            End If
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Finally, add the layer to the map
            '
            If TypeOf pResultLayer Is ILayer Then
                If String.IsNullOrEmpty(sLayerName) Then
                    pResultLayer.Name = Me.DatasetName
                Else
                    pResultLayer.Name = sLayerName
                End If

                Dim pMapLayers As IMapLayers = pMap
                If pGrpLayer Is Nothing Then
                    pMapLayers.InsertLayer(pResultLayer, True, 0)
                Else
                    pMapLayers.InsertLayerInGroup(pGrpLayer, pResultLayer, True, 0)
                End If

                'Release lock on raster layer - FP Jan 10 2012
                If pMapLayers IsNot Nothing Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pMapLayers)
                    pMapLayers = Nothing
                End If

                pMXDoc.UpdateContents()
                pMXDoc.ActiveView.Refresh()
                pMXDoc.CurrentContentsView.Refresh(Nothing)
            End If

            Return pResultLayer

        End Function

    End Class

    Public Module GISDataSourceSharedMethods

        Public Function GetLinearUnitsAsString(pLinearUnit As ILinearUnit, Optional bParentheses As Boolean = False, Optional nPower As Short = 0) As String

            If pLinearUnit Is Nothing Then
                Return String.Empty
            End If

            Dim sResult As String = ""
            Select Case pLinearUnit.Name
                Case "Meter"
                    sResult = "m"
                Case "Feet", "Foot", "Foot_US"
                    sResult = "ft"
                Case Else
                    Dim ex As New GISException(GISException.ErrorTypes.CriticalError, "Unhandled linear units")
                    ex.Data.Add("Units", pLinearUnit.Name)
                    Throw ex
            End Select

            If Not String.IsNullOrEmpty(sResult) Then
                Select Case nPower
                    Case 2
                        sResult &= "²"
                    Case 3
                        sResult &= "³"
                End Select

                If bParentheses Then
                    sResult = "(" & sResult & ")"
                End If
            End If

            Return sResult

        End Function

        Public Function GetUnitsAsString(eUnit As esriUnits, Optional bParentheses As Boolean = False, Optional nPower As Short = 0) As String

            If eUnit = Nothing Then
                Return String.Empty
            End If

            If nPower < 0 OrElse nPower > 3 Then
                Dim ex As New Exception("Invalid power number for units display")
                ex.Data.Add("Power", nPower)
                Throw ex
            End If

            Dim sResult As String = ""
            Select Case eUnit
                Case esriUnits.esriCentimeters
                    sResult = "cm"
                Case esriUnits.esriDecimalDegrees
                    sResult = "degrees"
                Case esriUnits.esriDecimeters
                    sResult = "dm"
                Case esriUnits.esriFeet
                    sResult = "ft"
                Case esriUnits.esriInches
                    sResult = "in"
                Case esriUnits.esriKilometers
                    sResult = "km"
                Case esriUnits.esriMeters
                    sResult = "m"
                Case esriUnits.esriMiles
                    sResult = "mi"
                Case esriUnits.esriMillimeters
                    sResult = "mm"
                Case esriUnits.esriNauticalMiles
                    sResult = "nm"
                Case esriUnits.esriPoints
                    sResult = "pt"
                Case esriUnits.esriYards
                    sResult = "yd"
                Case esriUnits.esriUnknownUnits
                    sResult = String.Empty '"unknown"
                Case Else
                    sResult = String.Empty
            End Select

            If Not String.IsNullOrEmpty(sResult) Then

                Select Case nPower
                    Case 2
                        sResult &= "²"

                    Case 3
                        sResult &= "³"
                End Select

                If bParentheses Then
                    sResult = "(" & sResult & ")"
                End If
            End If

            Return sResult

        End Function

        Public Function IsFileGeodatabase(sFullPath As String) As Boolean

            If String.IsNullOrEmpty(sFullPath) Then
                Return False
            Else
                Return sFullPath.ToLower.Contains(".gdb")
            End If

        End Function

        ''' <summary>
        ''' Derives the file system path of a workspace given any path
        ''' </summary>
        ''' <param name="sPath">Any path. Can be a folder (e.g. file geodatabase) or absolute path to a file.</param>
        ''' <returns>The workspace path (ending with .gdb for file geodatabases) or the folder for file based data.</returns>
        ''' <remarks>PGB 9 Sep 2011.</remarks>
        Public Function GetWorkspacePath(sPath As String) As String

            If String.IsNullOrEmpty(sPath) Then
                Throw New ArgumentNullException("sPath", "The path cannot be null or empty")
            End If

            Dim sWorkspacePath As String = String.Empty
            Select Case GetWorkspaceType(sPath)
                Case GISDataStorageTypes.FileGeodatase
                    Dim index As Integer = sPath.ToLower.LastIndexOf(".gdb")
                    sWorkspacePath = sPath.Substring(0, index + 4)
                Case GISDataStorageTypes.CAD
                    Dim index As Integer = sPath.ToLower.LastIndexOf(".dxf")
                    sWorkspacePath = IO.Path.GetDirectoryName(sPath.Substring(0, index))
                Case Else
                    sWorkspacePath = IO.Path.GetDirectoryName(sPath)
            End Select
            Return sWorkspacePath

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sPath"></param>
        ''' <returns></returns>
        ''' <remarks>Note that the path that comes in may or may not have a dataset name on the end. So it
        ''' may be the path to a directory, or end with .gdb if a file geodatabase or may have a slash and
        ''' then the dataset name on the end.</remarks>
        Public Function GetWorkspaceType(sPath As String) As GISDataStorageTypes

            If String.IsNullOrEmpty(sPath) Then
                Throw New ArgumentNullException("sPath", "The path cannot be null or empty")
            End If

            If sPath.ToLower.Contains(".gdb") Then
                Return GISDataStorageTypes.FileGeodatase
            Else
                If IO.Directory.Exists(sPath) Then
                    ' ESRI GRID (folder)
                    Return GISDataStorageTypes.RasterFile
                Else
                    If sPath.ToLower.Contains(".dxf") Then
                        Return GISDataStorageTypes.CAD
                    ElseIf sPath.ToLower.Contains(".tif") Then
                        Return GISDataStorageTypes.RasterFile
                    ElseIf sPath.ToLower.Contains(".img") Then
                        Return GISDataStorageTypes.RasterFile
                    Else
                        Return GISDataStorageTypes.ShapeFile
                    End If
                End If
            End If

        End Function

        Public Function GetWorkspace(sFullPath As String, eType As GISDataStorageTypes) As IWorkspace

            Dim pWorkspace As IWorkspace = Nothing
            Dim pWSFact As IWorkspaceFactory = GISCode.ArcMap.GetWorkspaceFactory(eType)
            Dim sWorkspace As String = GetWorkspacePath(sFullPath)
            If eType = GISDataStorageTypes.FileGeodatase Then
                If pWSFact.IsWorkspace(sWorkspace) Then
                    pWorkspace = pWSFact.OpenFromFile(sWorkspace, 0)
                End If
            Else
                If IO.Directory.Exists(sWorkspace) Then
                    pWorkspace = pWSFact.OpenFromFile(sWorkspace, 0)
                End If
            End If


            Return pWorkspace

        End Function

        Public Function NameExistsInWorkspace(ByVal pWorkspace As IWorkspace, ByVal eType As esriDatasetType, ByVal sDatasetName As String) As Boolean

            Dim NameWithoutExtension As String = IO.Path.GetFileNameWithoutExtension(sDatasetName)
            Dim pEnumDSName As IEnumDatasetName = pWorkspace.DatasetNames(eType)
            Dim pName As IDatasetName = pEnumDSName.Next()
            While pName IsNot Nothing
                If pName.Name.ToLower = NameWithoutExtension.ToLower Then
                    Return True
                End If
                pName = pEnumDSName.Next
            End While

            Return False
        End Function

    End Module
End Namespace