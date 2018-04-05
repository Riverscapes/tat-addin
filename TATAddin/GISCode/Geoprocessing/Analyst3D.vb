#Region "Imports"
Imports ESRI.ArcGIS.Analyst3DTools
Imports ESRI.ArcGIS.Geoprocessor
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry

#End Region

Namespace GISCode.GP.Analyst3D

    Public Module Analyst3D

        Public Enum RasterDataTypes
            IntegerValues
            FloatValues
        End Enum

        Public Enum SamplingMetods
            Linear
            NaturalNeighbours
        End Enum

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFDS"></param>
        ''' <param name="outName"></param>
        ''' <param name="spacing"></param>
        ''' <param name="inPoints"></param>
        ''' <param name="inClip"></param>
        ''' <param name="inHardbreaks"></param>
        ''' <param name="inSoftbreaks"></param>
        ''' <param name="outRaster"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00q90000001n000000
        ''' License: All Levels
        ''' Extension: 3D Analyst</remarks>
        Public Sub CreateTerrainAndPyramidLevels(ByVal inFDS As IFeatureDataset,
                                                 ByVal outName As String,
                                                 ByVal spacing As Double,
                                                 ByVal inPoints As String,
                                                 ByVal inClip As String,
                                                 ByVal inHardbreaks As String,
                                                 ByVal inSoftbreaks As String,
                                                 ByVal outRaster As String)

            If Not TypeOf inFDS Is IFeatureDataset Then
                Throw New Exception("Invalid feature dataset")
            End If

            If String.IsNullOrEmpty(outName) Then
                Throw New Exception("out_name is null or empty string.")
            End If

            If spacing < 0 Then
                Throw New Exception("Spacing is less than zero.")
            End If

            If String.IsNullOrEmpty(inPoints) Then
                Throw New Exception("in_points is null or empty string.")
            End If

            If String.IsNullOrEmpty(inClip) Then
                Throw New Exception("in_clip is null or empty string.")
            End If

            If String.IsNullOrEmpty(inHardbreaks) Then
                Throw New Exception("in_hardbreaks is null or empty string.")
            End If

            If String.IsNullOrEmpty(inSoftbreaks) Then
                Throw New Exception("in_softbreaks is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outDEM is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim CreateTerrain As New CreateTerrain
            Dim test_terrain As Object
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CreateTerrain.in_feature_dataset = inFDS
            CreateTerrain.out_terrain_name = outName
            CreateTerrain.average_point_spacing = spacing

            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()
            Try

                GP.Execute(CreateTerrain, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

            Catch ex As Exception
                ex.Data("out_name") = outName
                ex.Data("spacing") = spacing.ToString
                ex.Data("in_points") = inPoints
                ex.Data("in_clip") = inClip
                ex.Data("in_hardbreaks") = inHardbreaks
                ex.Data("in_softbreaks") = inSoftbreaks
                ex.Data("outDEM") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            test_terrain = CreateTerrain.derived_out_terrain

            Dim PyramidLevels As New AddTerrainPyramidLevel
            PyramidLevels.in_terrain = test_terrain
            PyramidLevels.pyramid_level_definition = "'10 50000'"

            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(PyramidLevels, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("out_name") = outName
                ex.Data("spacing") = spacing.ToString
                ex.Data("in_points") = inPoints
                ex.Data("in_clip") = inClip
                ex.Data("in_hardbreaks") = inHardbreaks
                ex.Data("in_softbreaks") = inSoftbreaks
                ex.Data("outDEM") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Dim AddFeatures As New AddFeatureClassToTerrain

            AddFeatures.in_terrain = test_terrain
            AddFeatures.in_features = inPoints & inClip & inHardbreaks & inSoftbreaks

            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()
            Try
                GP.Execute(AddFeatures, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("out_name") = outName
                ex.Data("spacing") = spacing.ToString
                ex.Data("in_points") = inPoints
                ex.Data("in_clip") = inClip
                ex.Data("in_hardbreaks") = inHardbreaks
                ex.Data("in_softbreaks") = inSoftbreaks
                ex.Data("outDEM") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Dim BuildTerrain As New BuildTerrain

            BuildTerrain.in_terrain = test_terrain

            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()
            Try
                GP.Execute(BuildTerrain, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("out_name") = outName
                ex.Data("spacing") = spacing.ToString
                ex.Data("in_points") = inPoints
                ex.Data("in_clip") = inClip
                ex.Data("in_hardbreaks") = inHardbreaks
                ex.Data("in_softbreaks") = inSoftbreaks
                ex.Data("outDEM") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Dim TerrainToRaster As New TerrainToRaster

            TerrainToRaster.in_terrain = test_terrain
            TerrainToRaster.out_raster = outRaster
            TerrainToRaster.data_type = "FLOAT"
            TerrainToRaster.method = "LINEAR"
            TerrainToRaster.sample_distance = "CELLSIZE " & spacing
            TerrainToRaster.pyramid_level_resolution = "0"

            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()
            Try
                GP.Execute(TerrainToRaster, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("out_name") = outName
                ex.Data("spacing") = spacing.ToString
                ex.Data("in_points") = inPoints
                ex.Data("in_clip") = inClip
                ex.Data("in_hardbreaks") = inHardbreaks
                ex.Data("in_softbreaks") = inSoftbreaks
                ex.Data("outDEM") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' Converts a TIN to a raster.
        ''' </summary>
        ''' <param name="gTIN">The TIN to be converted.</param>
        ''' <param name="outRaster">The output raster.</param>
        ''' <param name="dataType">Choose whether the output raster will be floating point or integer (FLOAR or INT).</param>
        ''' <param name="method">Choose an interpolation method (LINEAR or NATURAL_NEIGHBORS).</param>
        ''' <param name="sampleDistance">Choose the sampling distance (OBSERVATIONS or CELLSIZE). Must specify a cellsize (eg. CELLSIZE 10).</param>
        ''' <param name="zFactor">The factor by which the heights of the resultant raster will be multiplied from those of the input TIN; used to convert z-units to x and y-units.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00q900000077000000.htm
        ''' License: All levels
        ''' Extensions: 3D Analyst</remarks>
        Public Sub TINtoRaster_3D(ByVal gTIN As GISDataStructures.TINDataSource,
                                  ByVal outRaster As String,
                                  Optional ByVal dataType As String = "FLOAT",
                                  Optional ByVal method As String = "LINEAR",
                                  Optional ByVal sampleDistance As String = "OBSERVATIONS",
                                  Optional ByVal zFactor As Double = 0)

            If gTIN Is Nothing Then
                Throw New Exception("gTIN is null or empty")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(dataType) Then
                Throw New Exception("dataType is null or empty string.")
            End If

            If String.IsNullOrEmpty(method) Then
                Throw New Exception("method is null or empty string.")
            End If

            If String.IsNullOrEmpty(sampleDistance) Then
                Throw New Exception("sampleDistance is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim TinToRaster As New TinRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            TinToRaster.in_tin = gTIN.Geodataset
            TinToRaster.out_raster = outRaster
            TinToRaster.data_type = dataType
            TinToRaster.method = method
            TinToRaster.sample_distance = sampleDistance
            TinToRaster.z_factor = zFactor

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(TinToRaster, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTIN") = gTIN.FullPath
                ex.Data("outRaster") = outRaster
                ex.Data("dataType") = dataType
                ex.Data("method") = method
                ex.Data("sampleDistance") = sampleDistance
                ex.Data("Z Factor") = zFactor
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="gTIN"></param>
        ''' <param name="outRaster"></param>
        ''' <param name="fSampleDistance"></param>
        ''' <remarks>Overloaded to handle the most common case of generating a floating point DEM</remarks>
        Public Sub TINtoRaster_3D(ByVal gTIN As GISDataStructures.TINDataSource, ByVal outRaster As String, ByVal fSampleDistance As Double)

            If fSampleDistance <= 0 Then
                Dim ex As New Exception("Invalid Sample distance for Tin to Raster")
                ex.Data("inTIN") = gTIN.FullPath
                ex.Data("outraster") = outRaster
                ex.Data("SampleDistance") = fSampleDistance
                Throw ex
            End If

            Dim sCellSize As String = "CELLSIZE " & fSampleDistance.ToString
            TINtoRaster_3D(gTIN, outRaster, "FLOAT", "NATURAL_NEIGHBORS", sCellSize, 1)

        End Sub

        Public Sub TINtoRaster_Orthogonal_Concurrent(ByVal gTIN As GISDataStructures.TINDataSource,
                                                     ByVal outRaster As String,
                                                     ByVal eDataType As RasterDataTypes,
                                                     ByVal eMethod As SamplingMetods,
                                                     ByVal fPrecision As Double,
                                                     ByVal fBuffer As Double,
                                                     ByVal gReferenceRaster As GISDataStructures.Raster,
                                                     Optional ByVal zFactor As Double = 1)


            Dim fSampleDistance As Double = gReferenceRaster.CellSize
            Dim sOrthogonalExtent As String = gReferenceRaster.ExtentAsString
            If gTIN Is Nothing Then
                Throw New Exception("inTIN is null.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            Else
                If GISDataStructures.Raster.Exists(outRaster) Then
                    Dim ex As New Exception("The output raster already exists.")
                    ex.Data("TIN") = gTIN.FullPath
                    ex.Data("Raster") = outRaster
                    Throw ex
                End If
            End If

            If fSampleDistance <= 0 Then
                Dim ex As New Exception("Invalid sample distance.")
                ex.Data("Sample Distance") = fSampleDistance
                Throw ex
            End If

            Dim GP As New Geoprocessor
            Dim TinToRaster As New TinRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Get bounding rectangle of the TIN and make the coordinates orthogonal. Then use these
            ' as the raster extent for the geoprocessing tool.

            'Dim theTin As New Concurrency.TINInfoClass(inTIN, fSampleDistance, fPrecision, fBuffer)

            TinToRaster.in_tin = gTIN.Geodataset
            TinToRaster.out_raster = outRaster

            Select Case eDataType
                Case RasterDataTypes.IntegerValues : TinToRaster.data_type = "INT"
                Case RasterDataTypes.FloatValues : TinToRaster.data_type = "FLOAT"
            End Select

            Select Case eMethod
                Case SamplingMetods.Linear : TinToRaster.method = "LINEAR"
                Case SamplingMetods.NaturalNeighbours : TinToRaster.method = "NATURAL_NEIGHBORS"
            End Select

            TinToRaster.sample_distance = "CELLSIZE " & fSampleDistance.ToString
            TinToRaster.z_factor = zFactor.ToString

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.SetEnvironmentValue("extent", sOrthogonalExtent)
            'GP.SetEnvironmentValue("snapRaster", sRectangle)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(TinToRaster, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTIN") = gTIN.FullPath
                ex.Data("outRaster") = outRaster
                ex.Data("dataType") = eDataType.ToString
                ex.Data("method") = eMethod.ToString
                ex.Data("Sample Distance") = fSampleDistance.ToString
                ex.Data("Z Factor") = zFactor
                ex.Data("Extent original") = gTIN.ExtentOriginalAsString
                ex.Data("Extent orthogonal") = sOrthogonalExtent
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        Public Sub TINtoRaster_Orthogonal(ByVal gTIN As GISDataStructures.TINDataSource, ByVal outRaster As String, ByVal fSampleDistance As Double,
                                        ByVal eDataType As RasterDataTypes,
                                        ByVal eMethod As SamplingMetods,
                                        ByVal fPrecision As Double,
                                        ByVal fBuffer As Double,
                                        Optional ByVal zFactor As Double = 1)

            If gTIN Is Nothing Then
                Throw New Exception("inTIN is null.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            If fSampleDistance <= 0 Then
                Dim ex As New Exception("Invalid sample distance.")
                ex.Data("Sample Distance") = fSampleDistance
                Throw ex
            End If

            Dim GP As New Geoprocessor
            Dim TinToRaster As New TinRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Get bounding rectangle of the TIN and make the coordinates orthogonal. Then use these
            ' as the raster extent for the geoprocessing tool.

            'Dim theTin As New Concurrency.TINInfoClass(inTIN, fSampleDistance, fPrecision, fBuffer)

            TinToRaster.in_tin = gTIN.Geodataset
            TinToRaster.out_raster = outRaster

            Select Case eDataType
                Case RasterDataTypes.IntegerValues : TinToRaster.data_type = "INT"
                Case RasterDataTypes.FloatValues : TinToRaster.data_type = "FLOAT"
            End Select

            Select Case eMethod
                Case SamplingMetods.Linear : TinToRaster.method = "LINEAR"
                Case SamplingMetods.NaturalNeighbours : TinToRaster.method = "NATURAL_NEIGHBORS"
            End Select

            TinToRaster.sample_distance = "CELLSIZE " & fSampleDistance.ToString
            TinToRaster.z_factor = zFactor.ToString

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            Dim sOrthogonalExtent As String = gTIN.ExtentOrthogonal(fSampleDistance, fPrecision, fBuffer)
            GP.SetEnvironmentValue("extent", sOrthogonalExtent)
            'GP.SetEnvironmentValue("snapRaster", sRectangle)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(TinToRaster, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTIN") = gTIN.FullPath
                ex.Data("outRaster") = outRaster
                ex.Data("dataType") = eDataType.ToString
                ex.Data("method") = eMethod.ToString
                ex.Data("Sample Distance") = fSampleDistance.ToString
                ex.Data("Z Factor") = zFactor
                ex.Data("Extent original") = gTIN.ExtentOriginalAsString
                ex.Data("Extent orthogonal") = sOrthogonalExtent
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        Public Sub TINtoRaster(ByVal gTin As GISDataStructures.TINDataSource, ByVal outRaster As String, ByVal fSampleDistance As Double,
                                 ByVal eDataType As RasterDataTypes,
                                 ByVal eMethod As SamplingMetods,
                                 ByVal fPrecision As Double,
                                 ByVal theExtent As GISCode.GISDataStructures.ExtentRectangle,
                                 Optional ByVal zFactor As Double = 1)

            If gTin Is Nothing Then
                Throw New Exception("inTIN is null")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            If fSampleDistance <= 0 Then
                Dim ex As New Exception("Invalid sample distance.")
                ex.Data("Sample Distance") = fSampleDistance
                Throw ex
            End If

            Dim GP As New Geoprocessor
            Dim TinToRaster As New TinRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            TinToRaster.in_tin = gTin.Geodataset
            TinToRaster.out_raster = outRaster

            Select Case eDataType
                Case RasterDataTypes.IntegerValues : TinToRaster.data_type = "INT"
                Case RasterDataTypes.FloatValues : TinToRaster.data_type = "FLOAT"
            End Select

            Select Case eMethod
                Case SamplingMetods.Linear : TinToRaster.method = "LINEAR"
                Case SamplingMetods.NaturalNeighbours : TinToRaster.method = "NATURAL_NEIGHBORS"
            End Select

            TinToRaster.sample_distance = "CELLSIZE " & fSampleDistance.ToString
            TinToRaster.z_factor = zFactor.ToString

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.SetEnvironmentValue("extent", theExtent.Rectangle)
            'Debug.WriteLine(String.Format("TIN to raster extent: {0}", theExtent.Rectangle))
            'GP.SetEnvironmentValue("snapRaster", sRectangle)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(TinToRaster, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTIN") = gTin.FullPath
                ex.Data("outRaster") = outRaster
                ex.Data("dataType") = eDataType.ToString
                ex.Data("method") = eMethod.ToString
                ex.Data("Sample Distance") = fSampleDistance.ToString
                ex.Data("Z Factor") = zFactor
                ex.Data("Extent Rectangle") = theExtent.Rectangle
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Exports the nodes of a TIN dataset to a point feature class
        ''' </summary>
        ''' <remarks>License: All levels
        ''' Extensions: 3D Analyst</remarks>
        Public Sub TINNodesToPoints_3D(ByVal gTIN As GISDataStructures.TINDataSource,
                                       ByVal sOutFC As String)

            If gTIN Is Nothing Then
                Throw New Exception("gTIN is null or empty")
            End If

            If String.IsNullOrEmpty(sOutFC) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim TinToPointFC As New ESRI.ArcGIS.Analyst3DTools.TinNode

            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            TinToPointFC.in_tin = gTIN.Geodataset
            TinToPointFC.out_feature_class = sOutFC

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(TinToPointFC, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTIN") = gTIN.FullPath
                ex.Data("outPointFC") = sOutFC
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Exports the nodes of a TIN dataset to a point feature class
        ''' </summary>
        ''' <param name="bObtainExtension">Console applications cannot use the ExtensionManager that the ObtainExtension method uses. Instead they use the
        ''' AOInit.CheckoutExtension method to obtain spatial analyst. Therefore this method should only do the extension check in UI products.</param>
        ''' <remarks>License: All levels
        ''' Extensions: 3D Analyst</remarks>
        Public Sub TINNodesToPoints_3D(ByVal gTIN As GISDataStructures.TINDataSource,
                                       ByVal sOutFC As String,
                                       ByVal sZField As String,
                                       Optional bObtainExtension As Boolean = True)

            If bObtainExtension Then
                If Not GISCode.Extensions.ObtainExtension(Extensions.ESRI_Extensions.Analyst3D, False) = ESRI.ArcGIS.esriSystem.esriExtensionState.esriESEnabled Then
                    Throw New Exception("The 3D Analyst Extension is Unavailable.")
                End If
            End If

            If gTIN Is Nothing Then
                Throw New Exception("gTIN is null or empty")
            End If

            If String.IsNullOrEmpty(sOutFC) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim TinToPointFC As New ESRI.ArcGIS.Analyst3DTools.TinNode

            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            TinToPointFC.in_tin = gTIN.Geodataset
            TinToPointFC.spot_field = sZField
            TinToPointFC.out_feature_class = sOutFC

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(TinToPointFC, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTIN") = gTIN.FullPath
                ex.Data("outPointFC") = sOutFC
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Exports the nodes of a TIN dataset to a point feature class
        ''' </summary>
        ''' <remarks>
        ''' License: All levels
        ''' Extensions: 3D Analyst</remarks>
        Public Sub TINNodesToPoints_3D(ByVal sTIN As String,
                                       ByVal sOutFC As String,
                                       ByVal sZFieldName As String)

            If sTIN Is Nothing Then
                Throw New Exception("gTIN is null or empty")
            End If

            If String.IsNullOrEmpty(sOutFC) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim TinToPointFC As New ESRI.ArcGIS.Analyst3DTools.TinNode

            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            TinToPointFC.in_tin = sTIN
            TinToPointFC.spot_field = sZFieldName
            TinToPointFC.out_feature_class = sOutFC

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(TinToPointFC, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTIN") = sTIN
                ex.Data("outPointFC") = sOutFC
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Exports the nodes of a TIN dataset to a point feature class
        ''' </summary>
        ''' <remarks>
        ''' License: All levels
        ''' Extensions: 3D Analyst</remarks>
        Public Sub TINNodesToPoints_3D(ByVal sTIN As String,
                                       ByVal sOutFC As String)

            If sTIN Is Nothing Then
                Throw New Exception("gTIN is null or empty")
            End If

            If String.IsNullOrEmpty(sOutFC) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim TinToPointFC As New ESRI.ArcGIS.Analyst3DTools.TinNode

            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            TinToPointFC.in_tin = sTIN
            TinToPointFC.out_feature_class = sOutFC

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(TinToPointFC, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTIN") = sTIN
                ex.Data("outPointFC") = sOutFC
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Enumeration for the types of feature classes that can be used as input to the 3D Analyst CreateTIN tool
        ''' </summary>
        ''' <remarks>At least one "mass points" feature class must be provided for the tool to work.</remarks>
        Public Enum TinFeatureTypes
            MassPoints
            HardLine
            SoftLine
            HardClip
            SoftClip
            HardErase
            SoftErase
            HardReplace
            SoftReplace
            HardValueFill
            SoftValueFill
        End Enum

        ''' <summary>
        ''' Enumeration for the typeo of triangulation to be used for the CreateTIN tool
        ''' </summary>
        ''' <remarks>Default is delaunay triangulation. (Same as leaving the checkbox unchecked in ArcToolbox.</remarks>
        Public Enum DelaunayTriangulationTypes
            Delaunay
            ConstrainedDelauncy
        End Enum

        ''' <summary>
        ''' Base class for TIN features used in the 3D Analyst CreateTIN tool.
        ''' </summary>
        ''' <remarks>Note that you cannot create objects of this class. You have to create
        ''' objects of the child classes that are more specific.</remarks>
        Public MustInherit Class TiNFeaturesBase

            Protected m_pFeatureClass As IFeatureClass
            Protected m_sHeightField As String
            Protected m_eType As TinFeatureTypes
            Protected m_sTagField As String

            Public ReadOnly Property FeatureClass As IFeatureClass
                Get
                    Return m_pFeatureClass
                End Get
            End Property

            Public ReadOnly Property HeightField As String
                Get
                    If String.IsNullOrEmpty(m_sHeightField) Then
                        Return "<None>"
                    Else
                        Return m_sHeightField
                    End If
                End Get
            End Property

            Public ReadOnly Property Type As TinFeatureTypes
                Get
                    Return m_eType
                End Get
            End Property

            Public ReadOnly Property TypeAsString As String
                Get
                    Dim sResult As String = ""
                    Select Case m_eType
                        Case TinFeatureTypes.MassPoints : sResult = "Mass_Points"
                        Case TinFeatureTypes.HardLine : sResult = "Hard_Line"
                        Case TinFeatureTypes.SoftLine : sResult = "Soft_Line"
                        Case TinFeatureTypes.HardClip : sResult = "Hard_Clip"
                        Case TinFeatureTypes.SoftClip : sResult = "Soft_Clip"
                        Case TinFeatureTypes.HardErase : sResult = "Hard_Erase"
                        Case TinFeatureTypes.SoftErase : sResult = "Soft_Erase"
                        Case TinFeatureTypes.HardReplace : sResult = "Hard_Replace"
                        Case TinFeatureTypes.SoftReplace : sResult = "Soft_Replease"
                        Case TinFeatureTypes.HardValueFill : sResult = "HardValue_Fill"
                        Case TinFeatureTypes.SoftValueFill : sResult = "SoftValue_Fill"
                        Case Else
                            Throw New Exception("Unhandled Tin Feature Type.")
                    End Select
                    Return sResult

                End Get
            End Property

            Public ReadOnly Property TagField As String
                Get
                    If String.IsNullOrEmpty(m_sTagField) Then
                        Return "<None>"
                    Else
                        Return m_sTagField
                    End If
                End Get
            End Property

            Public ReadOnly Property CreateTinFeaturesString As String
                Get
                    'sResult &= m_pFeatureClass.ToString
                    Dim pDataset As IDataset = CType(m_pFeatureClass, IDataset)
                    Dim fullPath As String = IO.Path.Combine(pDataset.Workspace.PathName, pDataset.Name)

                    If Not GISDataStructures.IsFileGeodatabase(fullPath) Then
                        fullPath &= ".shp"
                    End If

                    If fullPath.Contains(" ") Then
                        fullPath = "'" & fullPath & "'"
                    End If

                    Dim sResult As String = fullPath
                    sResult &= " " & HeightField
                    sResult &= " " & TypeAsString
                    sResult &= " " & TagField
                    Return sResult
                End Get
            End Property
        End Class

        Public Class TinMassPoints
            Inherits TiNFeaturesBase

            Public Sub New(ByVal pFeatureClass As IFeatureClass, ByVal sHeightField As String)

                If TypeOf pFeatureClass Is IFeatureClass Then
                    If Not (pFeatureClass.ShapeType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint OrElse
                        pFeatureClass.ShapeType = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint) Then
                        Throw New Exception("The mass points feature class is not of geometry type ""point"" or ""multipoint"".")
                    End If
                Else
                    Throw New ArgumentNullException("The mass points feature class is not valid.")
                End If

                If String.IsNullOrEmpty(sHeightField) Then
                    Throw New Exception("The height field cannot be null or empty.")
                Else
                    If Not String.Compare(sHeightField, "Shape.Z") = 0 Then
                        If pFeatureClass.FindField(sHeightField) < 0 Then
                            Dim ex As New ArgumentOutOfRangeException("sHeighField", "The height field cannot be found in the feature class.")
                            ex.Data("Height field") = sHeightField
                            Dim pDS As IDataset = pFeatureClass
                            ex.Data("Dataset") = pFeatureClass.AliasName
                            ex.Data("Workspace") = pDS.Workspace.PathName
                            Throw ex
                        End If
                    End If
                End If

                m_pFeatureClass = pFeatureClass
                m_sHeightField = sHeightField
                m_eType = TinFeatureTypes.MassPoints
                m_sTagField = ""
            End Sub

        End Class

        Public Class TinFeatures
            Inherits TiNFeaturesBase

            Public Sub New(ByVal pFeatureClass As IFeatureClass, ByVal sHeightField As String, ByVal eType As TinFeatureTypes, Optional ByVal sTagField As String = "")

                If Not TypeOf pFeatureClass Is IFeatureClass Then
                    Throw New ArgumentNullException("The feature class is not valid.")
                End If

                Select Case eType
                    Case TinFeatureTypes.MassPoints
                        Throw New Exception("Use the Tin mass points feature class instead.")

                    Case TinFeatureTypes.HardLine, TinFeatureTypes.SoftLine
                        If String.IsNullOrEmpty(sHeightField) Then
                            Throw New Exception("The height field cannot be null or empty for hard or soft breaklines.")
                        Else
                            If pFeatureClass.FindField(sHeightField) < 0 Then
                                Dim ex As New ArgumentOutOfRangeException("sHeighField", "The height field cannot be found in the feature class.")
                                ex.Data("Height field") = sHeightField
                                Dim pDS As IDataset = pFeatureClass
                                ex.Data("Dataset") = pFeatureClass.AliasName
                                ex.Data("Workspace") = pDS.Workspace.PathName
                                Throw ex
                            End If
                        End If
                End Select

                If Not String.IsNullOrEmpty(sTagField) Then
                    If pFeatureClass.FindField(sTagField) < 0 Then
                        Dim ex As New Exception("Unable to find the tag field.")
                        ex.Data("Tag field") = sTagField
                        Throw ex
                    Else
                        If pFeatureClass.Fields.Field(pFeatureClass.FindField(sTagField)).Type <> esriFieldType.esriFieldTypeInteger Then
                            Dim ex As New Exception("Tag fields used for CreateTIN must be of type integer.")
                            ex.Data("Tag field") = sTagField
                            Throw ex
                        End If
                    End If
                End If

                m_pFeatureClass = pFeatureClass
                m_sHeightField = sHeightField
                m_eType = eType
                m_sTagField = ""

            End Sub
        End Class

        ''' <summary>
        ''' Creates a TIN Surface
        ''' </summary>
        ''' <param name="sOutTIN">Path to the output TIN dataset</param>
        ''' <param name="pSR">Spatial Reference for the output TIN</param>
        ''' <param name="dFeatures">List of input features</param>
        ''' <param name="eTriangulationType">Delaunay or constrainted delaunay</param>
        ''' <returns>Output TIN dataset</returns>
        ''' <remarks>Requires 3D Analyst
        ''' http://resources.arcgis.com/en/help/main/10.1/index.html#//00q90000001v000000</remarks>
        Public Function CreateTIN(ByVal sOutTIN As String,
                                  ByVal pSR As ESRI.ArcGIS.Geoprocessing.GPSpatialReference,
                                  ByVal dFeatures As Dictionary(Of String, TiNFeaturesBase),
                                  Optional ByVal eTriangulationType As DelaunayTriangulationTypes = DelaunayTriangulationTypes.Delaunay
                               ) As GISDataStructures.TINDataSource

            Dim GP As New Geoprocessor
            Dim createTINTool As New ESRI.ArcGIS.Analyst3DTools.CreateTin
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            If GISDataStructures.TINDataSource.Exists(sOutTIN) Then
                Dim ex As New Exception("The output TIN dataset already exists.")
                ex.Data("Output TIN") = sOutTIN
                Throw ex
            End If

            If Not TypeOf pSR Is ISpatialReference Then
                Throw New Exception("The output spatial reference cannot be null.")
            End If

            If dFeatures Is Nothing OrElse dFeatures.Count < 1 Then
                Throw New Exception("The dictionary of feature classes cannot be null or empty")
            Else
                Dim nMassPoints As Integer = 0
                For Each aSetOfFeatures As TiNFeaturesBase In dFeatures.Values
                    If aSetOfFeatures.Type = TinFeatureTypes.MassPoints Then
                        nMassPoints += 1
                        Exit For
                    End If
                Next
                If nMassPoints < 1 Then
                    Throw New Exception("The dictionary of feature classes, must contain at least one of type mass points")
                End If
            End If

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            createTINTool.out_tin = sOutTIN
            createTINTool.spatial_reference = pSR

            If eTriangulationType = DelaunayTriangulationTypes.Delaunay Then
                createTINTool.constrained_delaunay = "delaunay"
            Else
                createTINTool.constrained_delaunay = "CONSTRAINED_DELAUNAY"
            End If

            Dim sFeatures As String = ""
            For Each aFeature As TiNFeaturesBase In dFeatures.Values
                sFeatures &= aFeature.CreateTinFeaturesString & ";"
            Next
            sFeatures = sFeatures.Substring(0, sFeatures.Length - 1)
            createTINTool.in_features = sFeatures

            Dim gTin As GISDataStructures.TINDataSource = Nothing
            Try
                GP.Execute(createTINTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

                gTin = New GISDataStructures.TINDataSource(sOutTIN)

            Catch ex As Exception
                ex.Data("out TIN") = sOutTIN
                ex.Data("Features") = sFeatures
                ex.Data("Triangulation") = createTINTool.constrained_delaunay.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return gTin

        End Function

    End Module

End Namespace