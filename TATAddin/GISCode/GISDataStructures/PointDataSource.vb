Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry

Namespace GISCode.GISDataStructures

    Public Class PointDataSource
        Inherits VectorDataSource

        Public Sub New(sFilePath As String)
            MyBase.New(sFilePath)

            If GeometryType <> GeometryTypes.Point Then
                Dim ex As New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.CriticalError, "Invalid geometry type.")
                ex.Data("Feature Class") = FullPath
                ex.Data("Actual geometry type") = GeometryType
                ex.Data("Expected geometry type") = GeometryTypes.Point
                Throw ex
            End If

        End Sub

        'Public Shadows Function Validate(lErrors As List(Of GISException), pReferenceSR As ISpatialReference) As Boolean
        '    Return MyBase.Validate(lErrors, pReferenceSR)
        'End Function

        Protected Function AllPointsExistOnRaster(gRaster As Raster, Optional pQry As IQueryFilter = Nothing) As Boolean

            Dim bResult As Boolean = True
            Dim pFCursor As IFeatureCursor = FeatureClass.Search(Nothing, True)
            Dim pFeature As IFeature = pFCursor.NextFeature
            Dim fRasterValue As Double = 0
            Do While TypeOf pFeature Is IFeature
                If Not gRaster.GetValueAtPoint(pFeature.Shape, fRasterValue) Then
                    bResult = False
                    Exit Do
                End If
                pFeature = pFCursor.NextFeature
            Loop
            Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)
            pFCursor = Nothing
            Return bResult

        End Function

        Protected Function AllPointsExistOnRaster(gRaster As Raster, sFieldName As String, sFieldValue As String) As Boolean

            If FindField(sFieldName) < 0 Then
                Dim ex As New ArgumentException("unable to find field")
                ex.Data("sFieldName") = sFieldName
                ex.Data("Feature Class") = FullPath
                Throw ex
            End If

            Dim pQry As IQueryFilter = New QueryFilter
            pQry.WhereClause = "(""" & sFieldName & """ = '" & sFieldValue.ToLower & "') OR " & _
                    "(""" & sFieldName & """ = '" & sFieldValue.ToUpper & "') OR " & _
                    "(""" & sFieldName & """ = '" & sFieldValue & "') "

            Return AllPointsExistOnRaster(gRaster, pQry)

        End Function

        ''' <summary>
        ''' Gets the points with a specified code in the specified field
        ''' </summary>
        ''' <param name="sFieldName">The name of the field in which to obtain values</param>
        ''' <param name="sCode">The field value that will be part of the WHERE clause</param>
        ''' <param name="lGeometries">List of geometries in the feature class with the argument code</param>
        ''' <returns>The total number of features with the argument code</returns>
        ''' <remarks></remarks>
        Public Function GetPointsWithCode(sFieldName As String, sCode As String, ByRef lGeometries As List(Of IGeometry)) As Integer

            If String.IsNullOrEmpty(sFieldName) Then
                Throw New Exception("The field name cannot be null or empty")
            End If

            Dim nField As Integer = FeatureClass.FindField(sFieldName)
            If nField < 0 Then
                Dim ex As New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.CriticalError, "The field name does not exist in the feature class")
                ex.Data("Field Name") = sFieldName
                ex.Data("Feature Class,") = FullPath
                Throw ex
            Else
                If FeatureClass.Fields.Field(nField).Type <> esriFieldType.esriFieldTypeString Then
                    Dim ex As New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.CriticalError, "The field exists but is not of type string")
                    ex.Data("Field Name") = sFieldName
                    ex.Data("Feature Class,") = FullPath
                    ex.Data("Actual Field Type") = FeatureClass.Fields.Field(nField).Type.ToString
                    Throw ex
                End If
            End If

            Dim sWhereClause As String = ""
            If Not String.IsNullOrEmpty(sCode) Then
                sWhereClause = "(""" & sFieldName & """ = '" & sCode.ToLower & "') OR (""" & sFieldName & """ = '" & sCode.ToUpper & "')"
            End If
            Return GetGeometries(sWhereClause, lGeometries)
        End Function

        Public Overrides Function Exists() As Boolean
            Return VectorDataSource.Exists(FullPath, ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
        End Function

        ''' <summary>
        ''' Get the horizonal euclidean distance between two points
        ''' </summary>
        ''' <param name="pA"></param>
        ''' <param name="pB"></param>
        ''' <returns>Horizonal (2D) euclidean distance between two points</returns>
        ''' <remarks>i.e. ignores Z dimension</remarks>
        Public Shared Function GetHorizDistance(ByVal pA As IPoint, ByVal pB As IPoint) As Double

            Debug.Assert(TypeOf pA Is IPoint)
            Debug.Assert(TypeOf pB Is IPoint)

            Return Math.Sqrt((pA.X - pB.X) ^ 2 + (pA.Y - pB.Y) ^ 2)

        End Function

        ''' <summary>
        ''' Open an ArcCatalog browser window and navigate to a feature class
        ''' </summary>
        ''' <param name="sTitle">Browser window title</param>
        ''' <param name="sFolder">Output folder of selected feature class</param>
        ''' <param name="sFCName">Output feature class name</param>
        ''' <returns>The fully qualified (geoprocessing compatible) path to the selected feature class</returns>
        ''' <remarks>This method now works with both ShapeFile sources and geodatabase sources.</remarks>
        Public Shared Shadows Function BrowseOpen(ByVal sTitle As String, ByRef sFolder As String, ByRef sFCName As String, hParentWindowHandle As System.IntPtr) As GISDataStructures.PointDataSource
            Return GISDataStructures.VectorDataSource.BrowseOpen(sTitle, sFolder, sFCName, GISCode.GISDataStructures.GeometryTypes.Point, hParentWindowHandle)
        End Function

        Public Function GetPointValuesFromRaster(gRaster As GISDataStructures.Raster, Optional bIgnoreNoData As Boolean = True) As List(Of Double)

            Dim lResults As New List(Of Double)
            '
            ' Extract raster values at points
            '
            Dim strOutExtract As String = WorkspaceManager.GetTempShapeFile("extract")
            GP.SpatialAnalyst.ExtractValuesToPoints(Me, gRaster, strOutExtract, True)
            '
            ' Remove points with values of -9999 (no data)
            '
            Dim sLayerName As String = WorkspaceManager.GetRandomString
            GP.DataManagement.MakeFeatureLayer(strOutExtract, sLayerName)

            If bIgnoreNoData Then
                GP.DataManagement.SelectLayerByAttribute(sLayerName, """RASTERVALU"" <> -9999", "NEW_SELECTION")
            End If

            Dim sPointsCopy As String = WorkspaceManager.GetTempShapeFile("ptscopy")
            GP.DataManagement.CopyFeatures(sLayerName, sPointsCopy)
            Dim gTemp As New PointDataSource(sPointsCopy)
            '
            ' PGB 26 Oct 2012. Seems to be a lock on this layer because of the selection?
            '
            'GP.DataManagement.Delete(sLayerName)

            'Dim pFCPoints As IFeatureClass = GISCode.FeatureClass.GetFeatureClass(sPointsCopy)
            Dim nField As Integer = gTemp.FindField("RASTERVALU")
            If nField < 0 Then
                Dim ex As New Exception("Raster value field not found in feature class")
                ex.Data("Feature class") = sPointsCopy
                ex.Data("Raster value field") = "RASTERVALU"
                Throw ex
            End If

            Dim pCursor As IFeatureCursor
            pCursor = gTemp.FeatureClass.Search(Nothing, False)
            Dim pFeature As IFeature = pCursor.NextFeature
            Do While TypeOf pFeature Is IFeature
                lResults.Add(pFeature.Value(nField))
                pFeature = pCursor.NextFeature
            Loop
            Runtime.InteropServices.Marshal.ReleaseComObject(pCursor)
            pCursor = Nothing

            Return lResults

        End Function

        ''' <summary>
        ''' Loop through all points and get the value of the point on the raster
        ''' </summary>
        ''' <param name="gRaster">Raster from which to get values. e.g. DEM</param>
        ''' <param name="fMin">Minimum value of all points on the raster.</param>
        ''' <param name="fMax">Maximum value of all points on the raster.</param>
        ''' <returns>True when the min and max were determined successfully from at least one point. Else false.</returns>
        ''' <remarks>Loops through all the points in the feature class. gets the point values individually, so could be
        ''' slowere than using Geoprocessing. Only use for small feature classes. Ignores no data values</remarks>
        Public Function GetMinMaxRasterValues(gRaster As GISDataStructures.Raster, ByRef fMin As Double, ByRef fMax As Double) As Boolean

            Dim bResult As Boolean = False
            Dim fMinLocal As System.Nullable(Of Double)
            Dim fMaxLocal As System.Nullable(Of Double)

            Dim pCursor As IFeatureCursor
            pCursor = FeatureClass.Search(Nothing, False)
            Dim pFeature As IFeature = pCursor.NextFeature
            Dim fValue As Double = 0
            Do While TypeOf pFeature Is IFeature
                gRaster.GetValueAtPoint(pFeature.Shape, fValue)
                If fValue <> GISDataStructures.Raster.NullRasterValue Then
                    If Not fMinLocal.HasValue OrElse fValue < fMin Then
                        fMinLocal = fValue
                    End If

                    If Not fMaxLocal.HasValue OrElse fValue > fMax Then
                        fMaxLocal = fValue
                    End If
                End If
                pFeature = pCursor.NextFeature
            Loop
            Runtime.InteropServices.Marshal.ReleaseComObject(pCursor)
            pCursor = Nothing

            If fMinLocal.HasValue Then
                fMin = fMinLocal
            End If

            If fMaxLocal.HasValue Then
                fMax = fMaxLocal
            End If

            Return fMinLocal.HasValue AndAlso fMaxLocal.HasValue

        End Function

    End Class

End Namespace