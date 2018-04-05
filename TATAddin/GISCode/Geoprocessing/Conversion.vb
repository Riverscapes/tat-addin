#Region "Imports"
Imports System.IO
Imports ESRI.ArcGIS.ConversionTools
Imports ESRI.ArcGIS.Geoprocessor
#End Region

Namespace GISCode.GP.Conversion

    Public Module Conversion

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001200000030000000
        ''' License Levels: ArcInfo without extensions. ArcView with 3D or Spatial Analyst</remarks>
        Public Sub PolygonToRaster_conversion(ByVal gPolygon As GISDataStructures.PolygonDataSource,
                                              ByVal sValueField As String,
                                              ByVal sOutputRasterPath As String,
                                              Optional ByVal gReferenceRaster As GISDataStructures.Raster = Nothing)

            If gPolygon.FindField(sValueField) < 0 Then
                Dim ex As New Exception("The field cannot be found in the feature class.")
                ex.Data("Feature Class") = gPolygon.FullPath
                ex.Data("Field Name") = sValueField
                Throw ex
            End If

            If GISDataStructures.Raster.Exists(sOutputRasterPath) Then
                Dim ex As New Exception("The output raster path already exists")
                ex.Data("Raster Path") = sOutputRasterPath
                Throw ex
            End If

            Dim GP As New Geoprocessor
            Dim PolyToRasterTool As New PolygonToRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            PolyToRasterTool.in_features = gPolygon.FeatureClass
            PolyToRasterTool.value_field = sValueField
            PolyToRasterTool.out_rasterdataset = sOutputRasterPath

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            If TypeOf gReferenceRaster Is GISDataStructures.Raster Then
                SetConcurrentEnvironment(GP, gReferenceRaster.FullPath)
            End If

            GP.ClearMessages()

            Try
                GP.Execute(PolyToRasterTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = gPolygon.FullPath
                ex.Data("valueField") = sValueField
                ex.Data("outRaster") = sOutputRasterPath
                If TypeOf gReferenceRaster Is GISDataStructures.Raster Then
                    ex.Data("concurrencyRaster") = gReferenceRaster.FullPath
                End If
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#/Polyline_to_Raster/001200000031000000/
        ''' License Levels: ArcInfo without extensions. ArcView with 3D or Spatial Analyst</remarks>
        Public Sub PolylineToRaster_conversion(ByVal gPolyline As GISDataStructures.PolylineDataSource,
                                              ByVal sValueField As String,
                                              ByVal sOutputRasterPath As String,
                                              Optional ByVal gReferenceRaster As GISDataStructures.Raster = Nothing)

            If gPolyline.FindField(sValueField) < 0 Then
                Dim ex As New Exception("The field cannot be found in the feature class.")
                ex.Data("Feature Class") = gPolyline.FullPath
                ex.Data("Field Name") = sValueField
                Throw ex
            End If

            If GISDataStructures.Raster.Exists(sOutputRasterPath) Then
                Dim ex As New Exception("The output raster path already exists")
                ex.Data("Raster Path") = sOutputRasterPath
                Throw ex
            End If

            Dim GP As New Geoprocessor
            Dim PolyToRasterTool As New PolylineToRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            PolyToRasterTool.in_features = gPolyline.FeatureClass
            PolyToRasterTool.value_field = sValueField
            PolyToRasterTool.out_rasterdataset = sOutputRasterPath

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            If TypeOf gReferenceRaster Is GISDataStructures.Raster Then
                SetConcurrentEnvironment(GP, gReferenceRaster.FullPath)
            End If

            GP.ClearMessages()

            Try
                GP.Execute(PolyToRasterTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = gPolyline.FullPath
                ex.Data("valueField") = sValueField
                ex.Data("outRaster") = sOutputRasterPath
                If TypeOf gReferenceRaster Is GISDataStructures.Raster Then
                    ex.Data("concurrencyRaster") = gReferenceRaster.FullPath
                End If
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00120000002v000000
        ''' License Level: All Levels
        ''' Extension: None</remarks>
        Public Sub FeatureToRaster()

            Dim pDissolve As New ESRI.ArcGIS.ConversionTools.FeatureToRaster
            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(pDissolve, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inRaster"></param>
        ''' <param name="outPolyline"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001200000009000000
        ''' License Level: All Levels
        ''' Extension: None</remarks>
        Public Sub RasterToPolyline(ByVal inRaster As FileInfo,
                                    ByVal outPolyline As FileInfo)

            If TypeOf inRaster Is FileInfo Then
                If Not inRaster.Exists Then
                    Throw New Exception("Input raster does not exist.")
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outPolyline Is FileInfo Then
                Throw New Exception("Invalid output polyline.")
            End If

            Dim GP As Geoprocessor = New Geoprocessor()
            Dim RasterToPolylineTool As New RasterToPolyline
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            RasterToPolylineTool.in_raster = inRaster.FullName
            RasterToPolylineTool.out_polyline_features = outPolyline.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(RasterToPolylineTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("outPolyline") = outPolyline.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inRaster"></param>
        ''' <param name="outPolygon"></param>
        ''' <param name="simplify"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001200000008000000
        ''' License Level: All Levels
        ''' Extension: None</remarks>
        Public Sub RasterToPolygon_conversion(ByVal inRaster As FileInfo,
                                              ByVal outPolygon As FileInfo,
                                              Optional ByVal simplify As String = "NO_SIMPLIFY")

            If TypeOf inRaster Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inRaster.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inRaster.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outPolygon Is FileInfo Then
                Throw New Exception("Invalid output polygon.")
            End If

            If String.IsNullOrEmpty(simplify) Then
                Throw New Exception("simplify is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim ConvertTool As New RasterToPolygon
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ConvertTool.in_raster = inRaster.FullName
            ConvertTool.out_polygon_features = outPolygon.FullName
            ConvertTool.simplify = simplify

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(ConvertTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("outPolygon") = outPolygon.FullName
                ex.Data("simplify") = simplify
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inRaster"></param>
        ''' <param name="outPolygon"></param>
        ''' <param name="simplify"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001200000008000000
        ''' License Level: All Levels
        ''' Extension: None</remarks>
        Public Function RasterToPolygon_conversion(ByVal inRaster As String,
                                              ByVal outPolygon As String,
                                              Optional ByVal simplify As String = "NO_SIMPLIFY") As GISDataStructures.PolygonDataSource

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("inRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outPolygon) Then
                Throw New Exception("outPolygon is null or empty string.")
            End If

            If String.IsNullOrEmpty(simplify) Then
                Throw New Exception("simplify is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim ConvertTool As New RasterToPolygon
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ConvertTool.in_raster = inRaster
            ConvertTool.out_polygon_features = outPolygon
            ConvertTool.simplify = simplify

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(ConvertTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("outPolygon") = outPolygon
                ex.Data("simplify") = simplify
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Dim gResult As GISDataStructures.PolygonDataSource = Nothing
            If GISDataStructures.PolygonDataSource.Exists(outPolygon) Then
                gResult = New GISDataStructures.PolygonDataSource(outPolygon)
            End If

            Return gResult

        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inRaster"></param>
        ''' <param name="outPoints"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001200000007000000
        ''' License Level: All Levels
        ''' Extension: None</remarks>
        Public Sub RasterToPoint_conversion(ByVal inRaster As FileInfo,
                                            ByVal outPoints As FileInfo)

            If Not TypeOf inRaster Is FileInfo Then
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outPoints Is FileInfo Then
                Throw New Exception("Invalid output points.")
            End If

            Dim GP As Geoprocessor = New Geoprocessor
            Dim RasterToPoints As New RasterToPoint
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            RasterToPoints.in_raster = inRaster.FullName
            RasterToPoints.out_point_features = outPoints.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(RasterToPoints, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("outPoints") = outPoints.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inPoints"></param>
        ''' <param name="valueField"></param>
        ''' <param name="outRaster"></param>
        ''' <param name="extentRaster"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00120000002z000000
        ''' License Levels: ArcInfo without extensions. ArcView with 3D or Spatial Analyst</remarks>
        Public Sub PointToRaster(ByVal inPoints As String,
                                 ByVal valueField As String,
                                 ByVal outRaster As String,
                                 ByVal extentRaster As String)

            If String.IsNullOrEmpty(inPoints) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(valueField) Then
                Throw New Exception("value_field is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("out_raster is null or empty string.")
            End If

            If String.IsNullOrEmpty(extentRaster) Then
                Throw New Exception("strExtentRaster is null or empty string.")
            End If

            Dim GP As Geoprocessor = New Geoprocessor()
            Dim PointToRaster As New PointToRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            PointToRaster.in_features = inPoints
            PointToRaster.value_field = valueField
            PointToRaster.out_rasterdataset = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            SetConcurrentEnvironment(GP, extentRaster)
            GP.ClearMessages()

            Try
                GP.Execute(PointToRaster, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inPoints") = inPoints
                ex.Data("valueField") = valueField
                ex.Data("outRaster") = outRaster
                ex.Data("extentRaster") = extentRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        Public Sub FeatureToRaster(ByVal inFeature As String,
                         ByVal valueField As String,
                         ByVal outRaster As String)

            If String.IsNullOrEmpty(inFeature) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(valueField) Then
                Throw New Exception("value_field is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("out_raster is null or empty string.")
            End If

            Dim GP As Geoprocessor = New Geoprocessor()
            Dim FeatureToRaster As New FeatureToRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FeatureToRaster.in_features = inFeature
            FeatureToRaster.field = valueField
            FeatureToRaster.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            'SetConcurrentEnvironment(GP, extentRaster)
            GP.ClearMessages()

            Try
                GP.Execute(FeatureToRaster, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inPoints") = inFeature
                ex.Data("valueField") = valueField
                ex.Data("outRaster") = outRaster
                'ex.Data("extentRaster", extentRaster)
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        Public Sub FeatureToRaster(ByVal inFeature As String,
                 ByVal valueField As String,
                 ByVal outRaster As String,
                 ByVal dCellsize As Double)

            If String.IsNullOrEmpty(inFeature) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(valueField) Then
                Throw New Exception("value_field is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("out_raster is null or empty string.")
            End If

            Dim GP As Geoprocessor = New Geoprocessor()
            Dim FeatureToRaster As New FeatureToRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FeatureToRaster.in_features = inFeature
            FeatureToRaster.field = valueField
            FeatureToRaster.out_raster = outRaster
            FeatureToRaster.cell_size = dCellsize

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            'SetConcurrentEnvironment(GP, extentRaster)
            GP.ClearMessages()

            Try
                GP.Execute(FeatureToRaster, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inPoints") = inFeature
                ex.Data("valueField") = valueField
                ex.Data("outRaster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Configure the geoprocessing environment to ensure concurrent outputs
        ''' </summary>
        ''' <param name="GP">Geoprocessor</param>
        ''' <param name="extentRaster">The path to the raster whose properties will be used for concurrency</param>
        ''' <remarks>PGB 8 Jul 2011
        ''' There are three properties for concurrency: Extent, cells aligning and cell size.</remarks>
        Private Sub SetConcurrentEnvironment(ByRef GP As Geoprocessor,
                                             ByVal extentRaster As String)

            If String.IsNullOrEmpty(extentRaster) Then
                Throw New Exception("sExtentRaster is null or empty string.")
            End If

            Debug.Assert(TypeOf GP Is Geoprocessor)
            If String.IsNullOrEmpty(extentRaster) Then
                Exit Sub
            End If
            '
            ' Set the processing extent to the input raster
            '
            Dim gRaster As New GISDataStructures.Raster(extentRaster)
            GP.SetEnvironmentValue("extent", gRaster.ExtentAsString)
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap
            '
            ' Set the snap raster to the input raster
            '
            GP.SetEnvironmentValue("snapRaster", extentRaster)
            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            '
            ' Set the cell size to the input raster
            '
            GP.SetEnvironmentValue("cellSize", gRaster.CellSize)
        End Sub

        ''' <summary>
        ''' Convert a list of vector feature classes to ShapeFiles in the output folder
        ''' </summary>
        ''' <param name="inFeatures">List of vector feature classes</param>
        ''' <param name="outFolder">Folder for output shapefiles</param>
        ''' <remarks>PGB 4 Mar 2012. Needed for converting geodatabase feature classes to 
        ''' the file system for use in the GCD.
        ''' 
        ''' PGB 13 Nov 2013. Note that this method will choose
        ''' a unique for the destination shapefile by adding "_#" at the end. New
        ''' code added to try and get the output feature class path</remarks>
        Public Function FeatureClassToShapeFile(ByVal inFeatures As String, ByVal outFolder As String) As String

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("inFeatures is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFolder) Then
                Throw New Exception("sOutputFolder is null or empty string.")
            End If

            Dim dir As New IO.DirectoryInfo(outFolder)
            If Not dir.Exists Then
                Dim ex As New Exception("Output directory needs to already exist.")
                ex.Data("outFolder") = outFolder
            End If

            Dim GP As Geoprocessor = New Geoprocessor()
            Dim FCToShapeFile As New FeatureClassToShapefile
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FCToShapeFile.Input_Features = inFeatures
            FCToShapeFile.Output_Folder = outFolder

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(FCToShapeFile, Nothing)

                Dim sResult As String = String.Empty
                Dim sMessageLookup As String = "Successfully converted:"
                For nMessage As Integer = 0 To GP.MessageCount - 1
                    Dim nIndex As Integer = GP.GetMessage(nMessage).IndexOf(sMessageLookup)
                    If nIndex > 0 Then
                        sResult = GP.GetMessage(nMessage).Substring(nIndex + sMessageLookup.Length + 1).Trim
                        Exit For
                    End If
                Next

                If String.IsNullOrEmpty(sResult) Then
                    Throw New Exception("No output feature class detected from Feature Clas To ShapeFile")
                Else
                    If GISDataStructures.VectorDataSource.Exists(sResult) Then
                        FeatureClassToShapeFile = sResult
                    End If
                End If

                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFolder") = outFolder
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return FeatureClassToShapeFile

        End Function

    End Module

End Namespace
