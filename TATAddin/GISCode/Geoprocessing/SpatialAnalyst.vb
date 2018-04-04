Imports System.IO
Imports ESRI.ArcGIS.SpatialAnalystTools
Imports ESRI.ArcGIS.Geoprocessor
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.SpatialStatisticsTools
Imports ESRI.ArcGIS.GeoAnalyst

Namespace GISCode.GP.SpatialAnalyst

    Public Enum FocalStatisticsTypeEnum
        MEAN
        MAJORITY
        MAXIMUM
        MEDIAN
        MINIMUM
        MINORITY
        RANGE
        STD
        SUM
        VARIETY
    End Enum

    Public Class clsRasterNeighborhood

        Private _string As String

        Public Sub SetAnnulus(ByVal innerRadius As Double,
                                ByVal outerRadius As Double, _
                                ByVal unitsType As esriGeoAnalysisUnitsEnum)
            _string = "ANNULUS " & innerRadius & " " & outerRadius & " " & GetUnitsString(unitsType)

        End Sub

        Public Sub SetCircle(ByVal radius As Double,
                                ByVal unitsType As esriGeoAnalysisUnitsEnum)
            _string = "CIRCLE " & radius & " " & GetUnitsString(unitsType)
        End Sub

        Public Sub SetRectangle(ByVal width As Double,
                                ByVal height As Double,
                                ByVal unitsType As esriGeoAnalysisUnitsEnum)
            _string = "RECTANGLE " & width & " " & height & " " & GetUnitsString(unitsType)
        End Sub

        Public Sub SetWedge(ByVal radius As Double,
                            ByVal startAngle As Double,
                            ByVal endAngle As Double,
                            ByVal unitsType As esriGeoAnalysisUnitsEnum)
            _string = "WEDGE " & radius & " " & startAngle & " " & endAngle & " " & GetUnitsString(unitsType)
        End Sub

        Public Overrides Function ToString() As String
            Return _string
        End Function

        Private Function GetUnitsString(ByVal unitsType As esriGeoAnalysisUnitsEnum) As String
            Select Case unitsType
                Case esriGeoAnalysisUnitsEnum.esriUnitsCells
                    Return "CELL"
                Case esriGeoAnalysisUnitsEnum.esriUnitsMap
                    Return "MAP"
                Case Else
                    Dim ex As New Exception("Unhandled geo analyst units")
                    ex.Data("Units type") = unitsType.ToString
                    Throw ex
            End Select
        End Function
    End Class

    Public Module SpatialAnalyst

        Public Const sZonalStatsCountFld As String = "COUNT"
        Public Const sZonalStatsAreaFld As String = "AREA"
        Public Const sZonalStatsMinFld As String = "MIN"
        Public Const sZonalStatsMaxFld As String = "MAX"
        Public Const sZonalStatsRangeFld As String = "RANGE"
        Public Const sZonalStatsMeanFld As String = "MEAN"
        Public Const sZonalStatsStdFld As String = "STD"
        Public Const sZonalStatsSumFld As String = "SUM"


        Public Function PointDensity(ByVal gPointFeatures As GISDataStructures.PointDataSource,
                            ByVal gReferenceRaster As GISDataStructures.Raster,
                            ByVal outRaster As String,
                            ByVal sNeighbourhood As String,
                            Optional ByVal sAreaUnits As String = "",
                            Optional ByVal inPopulationField As String = "NONE") As GISDataStructures.Raster

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            Else
                If GISDataStructures.Raster.Exists(outRaster) Then
                    Dim ex As New Exception("The output raster path already exists.")
                    ex.Data("Raster Path") = outRaster
                    Throw ex
                End If
            End If

            If String.IsNullOrEmpty(sNeighbourhood) Then
                Throw New Exception("Neighbourhood is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim gpPointDensity As New PointDensity
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            gpPointDensity.in_point_features = gPointFeatures.FeatureClass
            gpPointDensity.population_field = inPopulationField
            gpPointDensity.out_raster = outRaster
            gpPointDensity.cell_size = gReferenceRaster.CellSize
            gpPointDensity.neighborhood = sNeighbourhood
            gpPointDensity.area_unit_scale_factor = sAreaUnits

            SetConcurrentEnvironment(GP, gReferenceRaster)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Dim gResult As GISDataStructures.Raster = Nothing
            Try
                GP.Execute(gpPointDensity, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
                If GISDataStructures.Raster.Exists(outRaster) Then
                    gResult = New GISDataStructures.Raster(outRaster)
                Else
                    Throw New Exception("The output raster does not exist.")
                End If

            Catch ex As Exception
                ex.Data("inPointFeatures") = gPointFeatures.FullPath
                ex.Data("inReferenceRaster") = gReferenceRaster.FullPath
                ex.Data("outRaster") = outRaster
                ex.Data("sNeighbourhood") = sNeighbourhood
                ex.Data("sAreaUnits") = sAreaUnits
                ex.Data("inPopulationField") = inPopulationField
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return gResult

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="incon1">Input raster representing the true or false result of the desired condition. It can be of integer or floating point type.</param>
        ''' <param name="inTrueRasterOrConstant">The input whose values will be used as the output cell values if the condition is true. It can be an integer or a floating point raster, or a constant value.</param>
        ''' <param name="inFalseRasterOrConstant">The input whose values will be used as the output cell values if the condition is false. It can be an integer or a floating point raster, or a constant value.</param>
        ''' <param name="outRaster">The output raster.</param>
        ''' <param name="sWhereClause">e.g. "Value > 0.5"</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z00000005000000.htm
        ''' NOTE 1: The word "con" is a reserved word within ArcGIS. Do not attempt to call the output raster "con" or "con.tif" etc
        ''' Note 2: PGB 16 Jun 2014. Fed up with multiple versions of this geoprocessing routine. Combining all into one.
        ''' 
        ''' Syntax: Con (in_conditional_raster, in_true_raster_or_constant, {in_false_raster_or_constant}, {where_clause})
        ''' License: Requires Spatial Analyst</remarks>
        Public Sub Con(ByVal inCon1 As String,
                          ByVal inTrueRasterOrConstant As String,
                          ByVal inFalseRasterOrConstant As String,
                          ByVal outRaster As String,
                          ByVal sWhereClause As String)

            If Not GISDataStructures.Raster.Exists(inCon1) Then
                Dim ex As New Exception("The input conditional raster is null or does not exist.")
                ex.Data("Raster: ") = inCon1
                Throw ex
            End If

            If String.IsNullOrEmpty(inTrueRasterOrConstant) AndAlso String.IsNullOrEmpty(inFalseRasterOrConstant) Then
                Throw New Exception("The input true and false rasters cannot both be empty.")
            End If

            If Not String.IsNullOrEmpty(inTrueRasterOrConstant) Then
                If IsNumeric(inTrueRasterOrConstant) Then
                    ' This input can be an integer or float
                Else
                    If Not GISDataStructures.Raster.Exists(inTrueRasterOrConstant) Then
                        Dim ex As New Exception("The input true raster is null or does not exist.")
                        ex.Data("True Raster: ") = inTrueRasterOrConstant
                        Throw ex
                    End If
                End If
            End If

            If Not String.IsNullOrEmpty(inFalseRasterOrConstant) Then
                If IsNumeric(inFalseRasterOrConstant) Then
                    ' This input can be an integer or float
                Else
                    If Not GISDataStructures.Raster.Exists(inFalseRasterOrConstant) Then
                        Dim ex As New Exception("The input false raster is null or does not exist.")
                        ex.Data("False Raster: ") = inFalseRasterOrConstant
                        Throw ex
                    End If
                End If
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("The output raster cannot be null or empty.")
            Else
                If GISDataStructures.Raster.Exists(outRaster) Then
                    Dim ex As New Exception("The output raster already exists.")
                    ex.Data("Output raster: ") = outRaster
                    Throw ex
                Else
                    Dim sRasterName As String = IO.Path.GetFileNameWithoutExtension(outRaster)
                    If String.Compare(outRaster, "con", True) = 0 Then
                        Dim ex As New Exception("The output raster contains the word 'con', which is a reserved word and not allowed as the raster dataset name.")
                        ex.Data("Output raster") = outRaster
                        Throw ex
                    End If
                End If
            End If


            Dim GP As New Geoprocessor
            Dim ConTool As New Con
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ConTool.in_conditional_raster = inCon1

            If Not String.IsNullOrEmpty(inTrueRasterOrConstant) Then
                ConTool.in_true_raster_or_constant = inTrueRasterOrConstant
            End If

            If Not String.IsNullOrEmpty(inFalseRasterOrConstant) Then
                ConTool.in_false_raster_or_constant = inFalseRasterOrConstant
            End If

            ConTool.out_raster = outRaster.ToString

            If Not String.IsNullOrEmpty(sWhereClause) Then
                ConTool.where_clause = sWhereClause
            End If

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            Try
                GP.ClearMessages()
                GP.Execute(ConTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("incon1") = inCon1
                ex.Data("in true raster") = inTrueRasterOrConstant
                ex.Data("in false raster") = inFalseRasterOrConstant
                ex.Data("outcon") = outRaster
                ex.Data("whereclause") = sWhereClause
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Inverse distance weighted function
        ''' </summary>
        ''' <param name="gPoints">The point feature class on which to interpolate</param>
        ''' <param name="zField">The field in the point feature class that contains the values on which to interpolate</param>
        ''' <param name="outRaster">Full path to the raster to create</param>
        ''' <param name="gExtentRaster">Optional raster to use as the extent for the output. If not provided then the output adopts the extent of the input points.</param>
        ''' <remarks></remarks>
        Public Function IDW(ByVal gPoints As GISDataStructures.PointDataSource,
                       ByVal zField As String,
                       ByVal outRaster As String,
                       ByVal gExtentRaster As GISDataStructures.Raster) As GISDataStructures.Raster

            If Not TypeOf gPoints Is GISDataStructures.PointDataSource Then
                Throw New Exception("inPoints is null or empty string.")
            End If

            If String.IsNullOrEmpty(zField) Then
                Throw New Exception("zField is null or empty string.")
            Else
                If gPoints.FindField(zField) < 0 Then
                    Throw New Exception("The z field '" & zField & "' does not exist in feature class " & gPoints.FullPath)
                End If
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim pIDW As New Idw
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            pIDW.in_point_features = gPoints.FeatureClass
            pIDW.z_field = zField
            pIDW.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            '
            ' Use the optional raster extent if provided. Make sure that it is a file
            ' based raster before attempting to use GDAL.
            '
            If Not gExtentRaster Is Nothing Then
                SetConcurrentEnvironment(GP, gExtentRaster)
                GP.ClearMessages()
            End If

            Dim gResult As GISDataStructures.Raster = Nothing
            Try
                GP.Execute(pIDW, Nothing)
                gResult = New GISDataStructures.Raster(outRaster)
                ExceptionBase.ProcessGPExceptions(GP)

            Catch ex As Exception
                ex.Data("inPoints") = gPoints.FullPath
                ex.Data("zField") = zField
                ex.Data("outRaster") = outRaster

                If TypeOf gExtentRaster Is GISDataStructures.Raster Then
                    ex.Data("Extent Raster") = gExtentRaster.FullPath
                End If

                ExceptionBase.AddGPMessagesToException(ex, GP)

                Throw

            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return gResult

        End Function

        ''' <summary>
        ''' Configure the geoprocessing environment to ensure concurrent outputs
        ''' </summary>
        ''' <param name="GP">Geoprocessor</param>
        ''' <param name="gExtentRaster">The path to the raster whose properties will be used for concurrency</param>
        ''' <remarks>PGB 8 Jul 2011
        ''' There are three properties for concurrency: Extent, cells aligning and cell size.
        ''' The DEMInfo class uses GDAL inside. Therefore any raster passed must not be in a file geodatabase</remarks>
        Public Sub SetConcurrentEnvironment(ByRef GP As Geoprocessor,
                                             ByVal gExtentRaster As GISDataStructures.Raster)


            If gExtentRaster Is Nothing Then
                Throw New ArgumentNullException("The extent raster is null")
            End If
            '
            ' Set the processing extent to the input raster
            '
            Dim pRDS As IGeoDataset = gExtentRaster.RasterDataset
            Dim pExtent As IEnvelope = pRDS.Extent
            Dim sRectangle As String = gExtentRaster.ExtentAsString
            GP.SetEnvironmentValue("extent", sRectangle)
            '
            ' Set the snap raster to the input raster
            '
            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)

            ' Set the cell size to the input raster
            Dim pRL As IRasterLayer = New RasterLayer
            pRL.CreateFromDataset(pRDS)
            GP.SetEnvironmentValue("cellSize", gExtentRaster.CellSize)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inRaster"></param>
        ''' <param name="inMask"></param>
        ''' <param name="outRaster"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z0000002n000000.htm
        ''' License: Requires Spatial Analyst</remarks>
        Public Sub ExtractByMask(ByVal inRaster As FileInfo,
                                 ByVal inMask As FileInfo,
                                 ByVal outRaster As FileInfo)

            If TypeOf inRaster Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inRaster.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inRaster.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid VARIABLE.")
            End If

            If TypeOf inMask Is FileInfo Then
                'If Not inMask.Exists Then
                '    Throw New Exception("Input mask does not exist.")
                'End If
            Else
                Throw New Exception("Invalid VARIABLE.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim Extract As New ExtractByMask
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Extract.in_raster = inRaster.FullName
            Extract.in_mask_data = inMask.FullName
            Extract.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Extract, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("inMask") = inMask.FullName
                ex.Data("outRaster") = outRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Power(ByVal inRasterOne As String,
                         ByVal inRasterTwo As String,
                         ByVal outRaster As FileInfo)

            If String.IsNullOrEmpty(inRasterOne) Then
                Throw New Exception("inRasterOne is null or empty string.")
            End If

            If String.IsNullOrEmpty(inRasterTwo) Then
                Throw New Exception("inRasterTwo is null or empty string.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim Power As New Power
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Power.in_raster_or_constant1 = inRasterOne
            Power.in_raster_or_constant2 = inRasterTwo
            Power.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Power, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRasterOne") = inRasterOne
                ex.Data("inRasterTwo") = inRasterTwo
                ex.Data("outRaster") = outRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Plus(ByVal inRasterOne As String,
                        ByVal inRasterTwo As String,
                        ByVal outRaster As FileInfo)

            If String.IsNullOrEmpty(inRasterOne) Then
                Throw New Exception("inRasterOne is null or empty string.")
            End If

            If String.IsNullOrEmpty(inRasterTwo) Then
                Throw New Exception("inRasterTwo is null or empty string.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim Plus As New Plus
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Plus.in_raster_or_constant1 = inRasterOne
            Plus.in_raster_or_constant2 = inRasterTwo
            Plus.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Plus, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRasterOne") = inRasterOne
                ex.Data("inRasterTwo") = inRasterTwo
                ex.Data("outRaster") = outRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub SquareRoot(ByVal inRasterOne As String,
                              ByVal outRaster As FileInfo)

            If String.IsNullOrEmpty(inRasterOne) Then
                Throw New Exception("inRasterOne is null or empty string.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim SquareRoot As New SquareRoot
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            SquareRoot.in_raster_or_constant = inRasterOne
            SquareRoot.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(SquareRoot, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRasterOne") = inRasterOne
                ex.Data("outRaster") = outRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub
        ''' <summary>
        ''' The order of inputs is relevant for this tool. If both inputs are integer, the output will be an integer raster; otherwise, it will be a floating-point raster.
        ''' </summary>
        ''' <param name="inRasterOne">The input from which to subtract the values in the second input.</param>
        ''' <param name="inRasterTwo">The input values to subtract from the values in the first input.</param>
        ''' <param name="outraster">The output raster.</param>
        ''' <remarks></remarks>
        Public Sub Minus(ByVal inRasterOne As String,
                         ByVal inRasterTwo As String,
                         ByVal outRaster As FileInfo)

            If String.IsNullOrEmpty(inRasterOne) Then
                Throw New Exception("inRasterOne is null or empty string.")
            End If

            If String.IsNullOrEmpty(inRasterTwo) Then
                Throw New Exception("inRasterTwo is null or empty string.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim Minus As New Minus
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Minus.in_raster_or_constant1 = inRasterOne
            Minus.in_raster_or_constant2 = inRasterTwo
            Minus.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Minus, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRasterOne") = inRasterOne
                ex.Data("inRasterTwo") = inRasterTwo
                ex.Data("outRaster") = outRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Minus(ByVal inRasterOne As String,
                         ByVal inRasterTwo As String,
                         ByVal outRaster As String)

            If String.IsNullOrEmpty(inRasterOne) Then
                Throw New Exception("inRasterOne is null or empty string.")
            End If

            If String.IsNullOrEmpty(inRasterTwo) Then
                Throw New Exception("inRasterTwo is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outraster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim Minus As New Minus
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Minus.in_raster_or_constant1 = inRasterOne
            Minus.in_raster_or_constant2 = inRasterTwo
            Minus.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Minus, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRasterOne") = inRasterOne
                ex.Data("inRasterTwo") = inRasterTwo
                ex.Data("outRaster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Plus(ByVal inRasterOne As String,
                        ByVal inRasterTwo As String,
                        ByVal outRaster As String)

            If String.IsNullOrEmpty(inRasterOne) Then
                Throw New Exception("inRasterOne is null or empty string.")
            End If

            If String.IsNullOrEmpty(inRasterTwo) Then
                Throw New Exception("inRasterTwo is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outraster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim Plus As New Plus
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Plus.in_raster_or_constant1 = inRasterOne
            Plus.in_raster_or_constant2 = inRasterTwo
            Plus.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Plus, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRasterOne") = inRasterOne
                ex.Data("inRasterTwo") = inRasterTwo
                ex.Data("outRaster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub SetNull(ByVal inConditional As String,
                           ByVal inFalseOrConstant As String,
                           ByVal outRaster As FileInfo,
                           Optional ByVal whereClause As String = """VALUE"" < 0")

            If String.IsNullOrEmpty(inConditional) Then
                Throw New Exception("inConditional is null or empty string.")
            End If

            If String.IsNullOrEmpty(inFalseOrConstant) Then
                Throw New Exception("inFalseOrConstant is null or empty string.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim SetNull As New SetNull
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            SetNull.in_conditional_raster = inConditional
            SetNull.in_false_raster_or_constant = inFalseOrConstant
            SetNull.where_clause = whereClause
            SetNull.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(SetNull, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inConditional") = inConditional
                ex.Data("inFalseOrConstant") = inFalseOrConstant
                ex.Data("outRaster") = outRaster.FullName
                ex.Data("whereClause") = whereClause
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub SetNull(ByVal inConditional As String,
                           ByVal inFalseOrConstant As String,
                           ByVal outRaster As String,
                           Optional ByVal whereClause As String = """VALUE"" < 0")

            If String.IsNullOrEmpty(inConditional) Then
                Throw New Exception("inConditional is null or empty string.")
            End If

            If String.IsNullOrEmpty(inFalseOrConstant) Then
                Throw New Exception("inFalseOrConstant is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim SetNull As New SetNull
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            SetNull.in_conditional_raster = inConditional
            SetNull.in_false_raster_or_constant = inFalseOrConstant
            SetNull.where_clause = whereClause
            SetNull.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(SetNull, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inConditional") = inConditional
                ex.Data("inFalseOrConstant") = inFalseOrConstant
                ex.Data("outRaster") = outRaster
                ex.Data("whereClause") = whereClause
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Square(ByVal inRaster As String,
                          ByVal outRaster As FileInfo)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("inRaster is null or empty string.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim Square As New Square
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Square.in_raster_or_constant = inRaster
            Square.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Square, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("outRaster") = outRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Filter(ByVal inRaster As FileInfo,
                          ByVal outRaster As FileInfo,
                          Optional ByVal filterType As String = "LOW")

            If TypeOf inRaster Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inRaster.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inRaster.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim FilterTool As New Filter
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FilterTool.in_raster = inRaster.FullName
            FilterTool.out_raster = outRaster.FullName
            FilterTool.filter_type = filterType

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(FilterTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("outRaster") = outRaster.FullName
                ex.Data("filterType") = filterType
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Fill(ByVal inSurfaceRaster As FileInfo,
                        ByVal outSurfaceRaster As FileInfo)

            If TypeOf inSurfaceRaster Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inSurfaceRaster.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inSurfaceRaster.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outSurfaceRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim FillTool As New Fill
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FillTool.in_surface_raster = inSurfaceRaster.FullName
            FillTool.out_surface_raster = outSurfaceRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(FillTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inSurfaceRaster") = inSurfaceRaster.FullName
                ex.Data("outSurfaceRaster") = outSurfaceRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Minus(ByVal inMinus1 As FileInfo,
                         ByVal inMinus2 As FileInfo,
                         ByVal outMinus As FileInfo)

            If TypeOf inMinus1 Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inMinus1.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inMinus1.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If TypeOf inMinus2 Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inMinus2.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inMinus2.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outMinus Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim MinusTool As New Minus
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            MinusTool.in_raster_or_constant1 = inMinus1.FullName
            MinusTool.in_raster_or_constant2 = inMinus2.FullName
            MinusTool.out_raster = outMinus.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(MinusTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inMinus1") = inMinus1.FullName
                ex.Data("inMinus2") = inMinus2.FullName
                ex.Data("outMinus") = outMinus.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub RegionGroup_sa(ByVal inRaster As FileInfo,
                                  ByVal outRaster As FileInfo)

            If TypeOf inRaster Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inRaster.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inRaster.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim RegionTool As New RegionGroup
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            RegionTool.in_raster = inRaster.FullName
            RegionTool.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(RegionTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("outRaster") = outRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub RegionGroup_sa(ByVal inRaster As String,
                                  ByVal outRaster As String,
                                  Optional ByVal numNeighbors As String = "FOUR",
                                  Optional ByVal zoneCondition As String = "WITHIN",
                                  Optional ByVal addLink As Boolean = True)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("inRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim RegionTool As New RegionGroup
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            RegionTool.in_raster = inRaster
            RegionTool.out_raster = outRaster
            RegionTool.number_neighbors = numNeighbors
            RegionTool.zone_connectivity = zoneCondition
            RegionTool.add_link = addLink
            'RegionTool.excluded_value = exclude

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(RegionTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("outRaster") = outRaster
                ex.Data("numNeighbors") = numNeighbors
                ex.Data("zoneCondition") = zoneCondition
                ex.Data("addLink") = addLink
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Int_sa(ByVal inRaster As FileInfo,
                          ByVal outRaster As FileInfo)

            If TypeOf inRaster Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inRaster.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inRaster.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim IntTool As New Int
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            IntTool.in_raster_or_constant = inRaster.FullName
            IntTool.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(IntTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("outRaster") = outRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Int_sa(ByVal inRaster As String,
                          ByVal outRaster As String)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("inRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim IntTool As New Int
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            IntTool.in_raster_or_constant = inRaster
            IntTool.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(IntTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("outRaster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Filter_sa(ByVal inRaster As FileInfo,
                             ByVal outRaster As FileInfo)

            If TypeOf inRaster Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inRaster.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inRaster.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim FilterTool As New Filter
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FilterTool.in_raster = inRaster.FullName
            FilterTool.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(FilterTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("outRaster") = outRaster.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        'http://webhelp.esri.com/arcgisdesktop/9.2/index.cfm?TopicName=Zonal_Geometry_as_Table
        Public Sub ZonalGeometryAsTable_sa(ByVal inZoneData As FileInfo,
                                           ByVal zoneField As String,
                                           ByVal outTable As FileInfo)

            If TypeOf inZoneData Is FileInfo Then
                If Not inZoneData.Exists Then
                    Throw New Exception("Input zone data do not exist.")
                End If
            Else
                Throw New Exception("Invalid input zone data.")
            End If

            If String.IsNullOrEmpty(zoneField) Then
                Throw New Exception("zoneField is null or empty string.")
            End If

            If Not TypeOf outTable Is FileInfo Then
                Throw New Exception("Invalid output table.")
            End If

            Dim GP As New Geoprocessor
            Dim ZonalTool As New ZonalGeometryAsTable
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ZonalTool.in_zone_data = inZoneData.FullName
            ZonalTool.zone_field = zoneField
            ZonalTool.out_table = outTable.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = True
            GP.ClearMessages()

            Try
                GP.Execute(ZonalTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inZoneData") = inZoneData.FullName
                ex.Data("zoneField") = zoneField
                ex.Data("outTable") = outTable.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub ZonalGeometry(ByVal inZoneData As String,
                                 ByVal zoneField As String,
                                 ByVal outRaster As String,
                                 Optional ByVal geometryType As String = Nothing)

            If String.IsNullOrEmpty(inZoneData) Then
                Throw New Exception("inZoneData is null or empty string.")
            End If

            If String.IsNullOrEmpty(zoneField) Then
                Throw New Exception("zoneField is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim ZonalTool As New ZonalGeometry
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ZonalTool.in_zone_data = inZoneData
            ZonalTool.zone_field = zoneField
            ZonalTool.geometry_type = geometryType
            'ZonalTool.cell_size = cellsize
            ZonalTool.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(ZonalTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inZoneData") = inZoneData
                ex.Data("zoneField") = zoneField
                ex.Data("outRaster") = outRaster
                ex.Data("geometryType") = geometryType
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Contour_sa(ByVal inRaster As FileInfo,
                              ByVal outRaster As FileInfo,
                              Optional ByVal interval As Double = 0.1)

            If TypeOf inRaster Is FileInfo Then
                If Not GISCode.GISDataStructures.Raster.Exists(inRaster.FullName) Then
                    Dim ex As New Exception("Input raster does not exist.")
                    ex.Data("inRaster") = inRaster.FullName
                    Throw ex
                End If
            Else
                Throw New Exception("Invalid input raster.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim ContourTool As New Contour
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ContourTool.in_raster = inRaster.FullName
            ContourTool.out_polyline_features = outRaster.FullName
            ContourTool.contour_interval = interval

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(ContourTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("outRaster") = outRaster.FullName
                ex.Data("interval") = interval.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub ZonalStatistics_sa(ByVal inZoneData As FileInfo,
                                      ByVal zoneField As String,
                                      ByVal inValueRaster As FileInfo,
                                      ByVal outRaster As FileInfo,
                                      Optional ByVal statistics As String = "MAXIMUM",
                                      Optional ByVal ignore As Boolean = True)

            If TypeOf inZoneData Is FileInfo Then
                If Not inZoneData.Exists Then
                    Throw New Exception("Input zone data do not exist.")
                End If
            Else
                Throw New Exception("Invalid input zone data.")
            End If

            If String.IsNullOrEmpty(zoneField) Then
                Throw New Exception("zoneField is null or empty string.")
            End If

            If TypeOf inValueRaster Is FileInfo Then
                If Not inValueRaster.Exists Then
                    Throw New Exception("Input value raster does not exist.")
                End If
            Else
                Throw New Exception("Invalid input value raster.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim ZonalTool As New ZonalStatistics
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ZonalTool.in_zone_data = inZoneData.FullName
            ZonalTool.zone_field = zoneField
            ZonalTool.statistics_type = statistics
            ZonalTool.in_value_raster = inValueRaster.FullName
            ZonalTool.out_raster = outRaster.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()


            Try
                GP.Execute(ZonalTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inZoneData") = inZoneData.FullName
                ex.Data("zoneField") = zoneField
                ex.Data("inValueRaster") = inValueRaster.FullName
                ex.Data("outRaster") = outRaster.FullName
                ex.Data("statistics") = statistics
                ex.Data("ignore") = ignore.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inSurfaceRaster">The input raster representing a continuous surface.</param>
        ''' <param name="outSurfaceRaster">The output surface raster after the sinks have been filled.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z00000050000000.htm</remarks>
        Public Sub Fill(ByVal inSurfaceRaster As String,
                        ByVal outSurfaceRaster As String)

            If String.IsNullOrEmpty(inSurfaceRaster) Then
                Throw New Exception("inSurfaceRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outSurfaceRaster) Then
                Throw New Exception("outSurfaceRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim FillTool As New Fill
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FillTool.in_surface_raster = inSurfaceRaster
            FillTool.out_surface_raster = outSurfaceRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(FillTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inSurfaceRaster") = inSurfaceRaster
                ex.Data("outSurfaceRaster") = outSurfaceRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inSurfaceRaster">The input raster representing a continuous surface.</param>
        ''' <param name="outFlowRaster">The output raster that shows the flow direction from each cell to its steepest downslope neighbor.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z00000052000000.htm</remarks>
        Public Sub Flow_Direction(ByVal inSurfaceRaster As String,
                                  ByVal outFlowRaster As String)

            If String.IsNullOrEmpty(inSurfaceRaster) Then
                Throw New Exception("inSurfaceRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFlowRaster) Then
                Throw New Exception("outFlowRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim FlowDirTool As New FlowDirection
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FlowDirTool.in_surface_raster = inSurfaceRaster
            FlowDirTool.out_flow_direction_raster = outFlowRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(FlowDirTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inSurfaceRaster") = inSurfaceRaster
                ex.Data("outFlowRaster") = outFlowRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFlowRaster">The input raster that shows the direction of flow out of each cell.</param>
        ''' <param name="outAccumulationRaster">The output raster that shows the accumulated flow to each cell.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z00000051000000.htm</remarks>
        Public Sub Flow_Accumulation(ByVal inFlowRaster As String,
                                     ByVal outAccumulationRaster As String)

            If String.IsNullOrEmpty(inFlowRaster) Then
                Throw New Exception("inFlowRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outAccumulationRaster) Then
                Throw New Exception("outAccumulationRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim FlowAccumTool As New FlowAccumulation
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FlowAccumTool.in_flow_direction_raster = inFlowRaster
            FlowAccumTool.out_accumulation_raster = outAccumulationRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(FlowAccumTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFlowRaster") = inFlowRaster
                ex.Data("outAccumulationRaster") = outAccumulationRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>This tool is technically not supported for use with scripting - it can work if you take some time to play with the expression syntax.
        ''' 
        ''' </summary>
        ''' <param name="expression"></param>
        ''' <param name="outRaster"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z000000z7000000.htm
        ''' PGB Jun 2014 - This method is now overloaded in SpatialAnalystGDAL.vb with a version that 
        ''' takes a RasterGDAL for the processing extent</remarks>
        Public Function Raster_Calculator(ByVal expression As String,
                                     ByVal outRaster As String) As GISDataStructures.Raster

            If String.IsNullOrEmpty(expression) Then
                Throw New Exception("expression is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            If Not GISCode.Extensions.ObtainExtension(Extensions.ESRI_Extensions.SpatialAnalyst, True) = ESRI.ArcGIS.esriSystem.esriExtensionState.esriESEnabled Then
                Throw New Exception("The Spatial Analyst Extension is Unavailable.")
            End If

            Dim GP As New Geoprocessor
            Dim RasterCalc As New RasterCalculator
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap
            Dim gResult As GISDataStructures.Raster = Nothing

            RasterCalc.expression = expression
            RasterCalc.output_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath) ' IO.Path.GetDirectoryName(sOutputRaster))
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(RasterCalc, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

                If GISDataStructures.Raster.Exists(outRaster) Then
                    gResult = New GISDataStructures.Raster(outRaster)
                End If

            Catch ex As Exception
                ex.Data("expression") = expression
                ex.Data("outRaster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return gResult

        End Function

        ''' <summary>This tool is technically not supported for use with scripting - it can work if you take some time to play with the expression syntax.
        ''' 
        ''' </summary>
        ''' <param name="expression"></param>
        ''' <param name="outRaster"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z000000z7000000.htm</remarks>
        Public Function Raster_Calculator(ByVal expression As String,
                                     ByVal outRaster As String,
                                      ByVal gExtentRaster As GISDataStructures.Raster) As GISDataStructures.Raster

            If String.IsNullOrEmpty(expression) Then
                Throw New Exception("expression is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            If Not GISCode.Extensions.ObtainExtension(Extensions.ESRI_Extensions.SpatialAnalyst, True) = ESRI.ArcGIS.esriSystem.esriExtensionState.esriESEnabled Then
                Throw New Exception("The Spatial Analyst Extension is Unavailable.")
            End If

            Dim GP As New Geoprocessor
            Dim RasterCalc As New RasterCalculator
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap
            Dim gResult As GISDataStructures.Raster = Nothing

            RasterCalc.expression = expression
            RasterCalc.output_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath) ' IO.Path.GetDirectoryName(sOutputRaster))
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            '
            ' Ensure that the geoprocessing environment is configured to produce concurrent results
            '
            SetConcurrentEnvironment(GP, gExtentRaster)
            GP.ClearMessages()

            Try
                GP.Execute(RasterCalc, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

                If GISDataStructures.Raster.Exists(outRaster) Then
                    gResult = New GISDataStructures.Raster(outRaster)
                End If

            Catch ex As Exception
                ex.Data("expression") = expression
                ex.Data("outRaster") = outRaster
                If TypeOf gExtentRaster Is GISDataStructures.Raster Then
                    ex.Data("extentRaster") = gExtentRaster.FullPath
                End If
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return gResult

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inStreamRaster">An input raster that represents a linear stream network.</param>
        ''' <param name="inFlowRaster">The input raster that shows the direction of flow out of each cell.</param>
        ''' <param name="outRaster">The output stream link raster. It will be of integer type.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z00000056000000.htm</remarks>
        Public Sub Stream_Link(ByVal inStreamRaster As String,
                               ByVal inFlowRaster As String,
                               ByVal outRaster As String)

            If String.IsNullOrEmpty(inStreamRaster) Then
                Throw New Exception("inStreamRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(inFlowRaster) Then
                Throw New Exception("inFlowRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim StreamLink As New StreamLink
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            StreamLink.in_stream_raster = inStreamRaster
            StreamLink.in_flow_direction_raster = inFlowRaster
            StreamLink.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(StreamLink, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inStreamRaster") = inStreamRaster
                ex.Data("inFlowRaster") = inFlowRaster
                ex.Data("outRaster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inZone">Dataset that defines the zones. The zones can be defined by an integer raster or a feature layer.</param>
        ''' <param name="zoneField">Field that holds the values that define each zone.</param>
        ''' <param name="inValueRaster">Raster that contains the values on which to calculate a statistic.</param>
        ''' <param name="outRaster">The output zonal statistics raster.</param>
        ''' <param name="statisticsType">Statistic type to be calculated. (e.g. MEAN, MAXIMUM, MINIMUN)</param>
        ''' <param name="noData">Denotes whether NoData values in the Value input will influence the results of the zone that they fall within.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z000000w7000000.htm</remarks>
        Public Sub ZonalStatistics(ByVal inZone As String,
                                   ByVal zoneField As String,
                                   ByVal inValueRaster As String,
                                   ByVal outRaster As String,
                                   Optional ByVal statisticsType As String = "MAXIMUM",
                                   Optional ByVal noData As Boolean = True)

            If String.IsNullOrEmpty(inZone) Then
                Throw New Exception("inZone is null or empty string.")
            End If

            If String.IsNullOrEmpty(zoneField) Then
                Throw New Exception("zoneField is null or empty string.")
            End If

            If String.IsNullOrEmpty(inValueRaster) Then
                Throw New Exception("inValueRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim ZonalStats As New ZonalStatistics
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ZonalStats.in_zone_data = inZone
            ZonalStats.zone_field = zoneField
            ZonalStats.in_value_raster = inValueRaster
            ZonalStats.statistics_type = statisticsType
            ZonalStats.ignore_nodata = noData
            ZonalStats.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(ZonalStats, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inZone") = inZone
                ex.Data("zoneField") = zoneField
                ex.Data("inValueRaster") = inValueRaster
                ex.Data("outRaster") = outRaster
                ex.Data("statisticsType") = statisticsType
                ex.Data("noData") = noData.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inZone">Dataset that defines the zones. The zones can be defined by an integer raster or a feature layer.</param>
        ''' <param name="zoneField">Field that holds the values that define each zone.</param>
        ''' <param name="inValue">Raster that contains the values on which to calculate a statistic.</param>
        ''' <param name="outTable">The output zonal statistics table.</param>
        ''' <param name="statisticsType">Statistic type to be calculated. (e.g. MEAN, MAXIMUM, MINIMUN)</param>
        ''' <param name="noData">Denotes whether NoData values in the Value input will influence the results of the zone that they fall within.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z000000w8000000.htm</remarks>
        Public Sub ZonalStatisticsAsTable(ByVal inZone As String,
                                          ByVal zoneField As String,
                                          ByVal inValue As String,
                                          ByVal outTable As String,
                                          Optional ByVal statisticsType As String = "SUM",
                                          Optional ByVal noData As Boolean = True)

            If String.IsNullOrEmpty(inZone) Then
                Throw New Exception("inZone is null or empty string.")
            End If

            If String.IsNullOrEmpty(zoneField) Then
                Throw New Exception("zoneField is null or empty string.")
            End If

            If String.IsNullOrEmpty(inValue) Then
                Throw New Exception("inValue is null or empty string.")
            End If

            If String.IsNullOrEmpty(outTable) Then
                Throw New Exception("outTable is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim ZonalStats As New ZonalStatisticsAsTable
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ZonalStats.in_zone_data = inZone
            ZonalStats.zone_field = zoneField
            ZonalStats.in_value_raster = inValue
            ZonalStats.statistics_type = statisticsType
            ZonalStats.ignore_nodata = noData
            ZonalStats.out_table = outTable

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(ZonalStats, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inZone") = inZone
                ex.Data("zoneField") = zoneField
                ex.Data("inValue") = inValue
                ex.Data("outTable") = outTable
                ex.Data("statisticsType") = statisticsType
                ex.Data("noData") = noData.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inStreamRaster">An input raster that represents a linear stream network.</param>
        ''' <param name="inFlowRaster">The input raster that shows the direction of flow out of each cell.</param>
        ''' <param name="outPolylineFeatures">Output feature class that will hold the converted streams.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z00000058000000.htm</remarks>
        Public Sub Stream_To_Feature(ByVal inStreamRaster As String,
                                     ByVal inFlowRaster As String,
                                     ByVal outPolylineFeatures As String)

            If String.IsNullOrEmpty(inStreamRaster) Then
                Throw New Exception("inStreamRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(inFlowRaster) Then
                Throw New Exception("inFlowRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outPolylineFeatures) Then
                Throw New Exception("outPolylineFeatures is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim StreamToFeature As New StreamToFeature
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            StreamToFeature.in_stream_raster = inStreamRaster
            StreamToFeature.in_flow_direction_raster = inFlowRaster
            StreamToFeature.out_polyline_features = outPolylineFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(StreamToFeature, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inStreamRaster") = inStreamRaster
                ex.Data("inFlowRaster") = inFlowRaster
                ex.Data("outPolylineFeatures") = outPolylineFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="gInPoints">The input point features defining the locations from which you want to extract the raster cell values.</param>
        ''' <param name="gRaster">The raster dataset whose values will be extracted. It can be an integer or floating-point type raster.</param>
        ''' <param name="outPoints">The output point feature dataset containing the extracted raster values.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z0000002t000000.htm</remarks>
        Public Function ExtractValuesToPoints(ByVal gInPoints As GISDataStructures.PointDataSource,
                                         ByVal gRaster As GISDataStructures.Raster,
                                         ByVal outPoints As String,
                                         Optional ByVal interpolate As Boolean = True) As GISDataStructures.PointDataSource

            If gInPoints Is Nothing Then
                Throw New ArgumentNullException("gInPoints", "inPoints is null or empty")
            End If

            If gInPoints.FeatureCount <= 0 Then
                Throw New Exception("The point feature class point used to extract points does not contain any points.")
            End If

            If gRaster Is Nothing Then
                Throw New ArgumentNullException("gRaster", "Raster is null or empty")
            End If

            If String.IsNullOrEmpty(outPoints) Then
                Throw New Exception("outPoints is null or empty string.")
            End If

            Dim gResult As GISDataStructures.PointDataSource = Nothing
            Dim GP As New Geoprocessor
            Dim ExtractToPoints As New ExtractValuesToPoints
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ExtractToPoints.in_point_features = gInPoints.FeatureClass
            ExtractToPoints.in_raster = gRaster.RasterDataset
            ExtractToPoints.interpolate_values = interpolate
            ExtractToPoints.out_point_features = outPoints

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(ExtractToPoints, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
                gResult = New GISDataStructures.PointDataSource(outPoints)

            Catch ex As Exception
                ex.Data("inPoints") = gInPoints.FullPath
                ex.Data("inRaster") = gRaster.FullPath
                ex.Data("outPoints") = outPoints
                ex.Data("interpolate") = interpolate.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return gResult

        End Function

        ''' <summary>
        ''' Inverse distance weighted function
        ''' </summary>
        ''' <param name="gPoints">The point feature class on which to interpolate</param>
        ''' <param name="zField">The field in the point feature class that contains the values on which to interpolate</param>
        ''' <param name="outRaster">Full path to the raster to create</param>
        ''' <remarks></remarks>
        Public Function IDW(ByVal gPoints As GISDataStructures.PointDataSource,
                       ByVal zField As String,
                       ByVal outRaster As String) As GISDataStructures.Raster

            If Not TypeOf gPoints Is GISDataStructures.PointDataSource Then
                Throw New Exception("inPoints is null or empty string.")
            End If

            If String.IsNullOrEmpty(zField) Then
                Throw New Exception("zField is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim pIDW As New Idw
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            pIDW.in_point_features = gPoints.FeatureClass
            pIDW.z_field = zField
            pIDW.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            Dim gResult As GISDataStructures.Raster = Nothing
            Try
                GP.Execute(pIDW, Nothing)
                gResult = New GISDataStructures.Raster(outRaster)
                ExceptionBase.ProcessGPExceptions(GP)

            Catch ex As Exception
                ex.Data("inPoints") = gPoints.FullPath
                ex.Data("zField") = zField
                ex.Data("outRaster") = outRaster

                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return gResult

        End Function

        'Public Sub FocalStats(ByVal inRaster As String,
        '                      ByVal neighborhood As RasterNeighborhoodClass,
        '                      ByVal outRaster As String,
        '                      ByVal extentRaster As String)

        '    If String.IsNullOrEmpty(inRaster) Then
        '        Throw New Exception("inRaster is null or empty string.")
        '    End If

        '    If String.IsNullOrEmpty(outRaster) Then
        '        Throw New Exception("outRaster is null or empty string.")
        '    End If

        '    If String.IsNullOrEmpty(outRaster) Then
        '        Throw New Exception("outRaster is null or empty string.")
        '    End If

        '    Dim GP As New Geoprocessor
        '    Dim Focal As New FocalStatistics
        '    Dim bAddtoMap As Boolean = GP.AddOutputsToMap

        '    neighborhood = neighborhood
        '    Focal.in_raster = inRaster
        '    Focal.neighborhood = neighborhood
        '    Focal.out_raster = outRaster

        '    GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
        '    GP.TemporaryMapLayers = False
        '    GP.AddOutputsToMap = False
        '    Dim extentGDAL As New GISDataStructures.RasterGDAL(extentRaster)
        '    SetConcurrentEnvironment(GP, extentGDAL)
        '    GP.ClearMessages()

        '    Try
        '        GP.Execute(Focal, Nothing)
        '        ExceptionBase.ProcessGPExceptions(GP)
        '    Catch ex As Exception
        '        ex.Data("inRaster", inRaster)
        '        ex.Data("neighborhood", neighborhood.ToString)
        '        ex.Data("outRaster") =outRaster)
        '        ex.Data("extentRaster") =extentRaster)
        '        ExceptionBase.AddGPMessagesToException(ex, GP)
        '        Throw ex
        '    Finally
        '        GP.AddOutputsToMap = bAddtoMap
        '    End Try
        'End Sub

        Public Sub PathDistance(ByVal inSourceData As String,
                                ByVal inCostRaster As String,
                                ByVal outDistanceRaster As String,
                                ByVal outBacklinkRaster As String)

            If String.IsNullOrEmpty(inSourceData) Then
                Throw New Exception("inSourceData is null or empty string.")
            End If

            If String.IsNullOrEmpty(inCostRaster) Then
                Throw New Exception("inCostRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outDistanceRaster) Then
                Throw New Exception("outDistanceRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outBacklinkRaster) Then
                Throw New Exception("outBacklinkRaster is null or empty string.")
            End If

            Dim PathDistanceTool As New PathDistance
            PathDistanceTool.in_source_data = inSourceData
            PathDistanceTool.out_distance_raster = outDistanceRaster
            PathDistanceTool.out_backlink_raster = outBacklinkRaster
            PathDistanceTool.in_cost_raster = inCostRaster

            Dim filepath As New FileInfo(outDistanceRaster)
            Dim WorkspacePath As String = filepath.DirectoryName
            Dim GP As Geoprocessor = New Geoprocessor()
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            GP.SetEnvironmentValue("extent", "MAXOF")
            GP.SetEnvironmentValue("snapraster", inCostRaster)
            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(PathDistanceTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("OutflowRaster") = inSourceData
                ex.Data("CostRaster") = inCostRaster
                ex.Data("DistanceRaster") = outDistanceRaster
                ex.Data("BacklinkRaster") = outBacklinkRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        Public Sub CostPath(ByVal inDestinationData As String,
                            ByVal inDistanceRaster As String,
                            ByVal inBacklinkRaster As String,
                            ByVal outRaster As String)

            If String.IsNullOrEmpty(inDestinationData) Then
                Throw New Exception("inDestinationData is null or empty string.")
            End If

            If String.IsNullOrEmpty(inDistanceRaster) Then
                Throw New Exception("inDistanceRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(inBacklinkRaster) Then
                Throw New Exception("inBacklinkRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim CostPathTool As New CostPath
            CostPathTool.in_destination_data = inDestinationData
            CostPathTool.in_cost_distance_raster = inDistanceRaster
            CostPathTool.in_cost_backlink_raster = inBacklinkRaster
            CostPathTool.out_raster = outRaster

            Dim filepath As New FileInfo(outRaster)
            Dim WorkspacePath As String = filepath.DirectoryName
            Dim GP As Geoprocessor = New Geoprocessor()
            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.SetEnvironmentValue("extent", "MAXOF")
            GP.SetEnvironmentValue("snapraster", inDistanceRaster)
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap
            GP.AddOutputsToMap = False

            GP.ClearMessages()
            Try
                GP.Execute(CostPathTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

            Catch ex As Exception
                ex.Data("InFlow") = inDestinationData
                ex.Data("DistanceRaster") = inDistanceRaster
                ex.Data("BacklinkRaster") = inBacklinkRaster
                ex.Data("OutputRaster") = outRaster
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try


        End Sub

        Public Sub Contour(ByVal inRaster As String,
                           ByVal outPolylineFeatures As String,
                           ByVal interval As Double,
                           Optional ByVal baseContour As Double = 0)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("inRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outPolylineFeatures) Then
                Throw New Exception("outPolylineFeatures is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim Contour As New Contour
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Contour.in_raster = inRaster
            Contour.out_polyline_features = outPolylineFeatures
            Contour.contour_interval = interval
            Contour.base_contour = baseContour

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Contour, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("outPolylineFeatures") = outPolylineFeatures
                ex.Data("interval") = interval.ToString
                ex.Data("baseContour") = baseContour.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub CalculateAreas_stats(ByVal inFeatures As String,
                                        ByVal outFeatures As String)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("inFeatures is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("outFeatures is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim Areas As New CalculateAreas
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Areas.Input_Feature_Class = inFeatures
            Areas.Output_Feature_Class = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Areas, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Shrink_sa(ByVal inRaster As String,
                             ByVal outRaster As String,
                             ByVal cells As Integer,
                             ByVal zone As String)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("inRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            If cells < 0 Then
                Throw New Exception("Cell is less than zero.")
            End If

            If String.IsNullOrEmpty(zone) Then
                Throw New Exception("zone is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim Shrink As New Shrink
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Shrink.in_raster = inRaster
            Shrink.out_raster = outRaster
            Shrink.number_cells = cells
            Shrink.zone_values = zone

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Shrink, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("outRaster") = outRaster
                ex.Data("cells") = cells.ToString
                ex.Data("zone") = zone
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="gRaster"></param>
        ''' <param name="outRaster"></param>
        ''' <param name="measurement"></param>
        ''' <param name="bObtainExtension">Console applications cannot use the ExtensionManager that the ObtainExtension method uses. Instead they use the
        ''' AOInit.CheckoutExtension method to obtain spatial analyst. Therefore this method should only do the extension
        ''' check in UI products.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Slope(ByVal gRaster As GISDataStructures.Raster,
                             ByVal outRaster As String,
                             Optional ByVal measurement As String = "DEGREE",
                             Optional bObtainExtension As Boolean = True) As GISDataStructures.Raster ' ,
            'Optional gExtentRaster As GISDataStructures.Raster = Nothing) As GISDataStructures.Raster

            ' Console applications cannot use the ExtensionManager that the ObtainExtension method uses. Instead they use the
            ' AOInit.CheckoutExtension method to obtain spatial analyst. Therefore this method should only do the extension
            ' check in UI products.
            If bObtainExtension Then
                If Not GISCode.Extensions.ObtainExtension(Extensions.ESRI_Extensions.SpatialAnalyst, False) = ESRI.ArcGIS.esriSystem.esriExtensionState.esriESEnabled Then
                    Throw New Exception("The Spatial Analyst Extension is Unavailable.")
                End If
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            Else
                If GISDataStructures.Raster.Exists(outRaster) Then
                    Dim ex As New Exception("The output raster already exists.")
                    ex.Data("Output raster") = outRaster
                    Throw ex
                End If
            End If

            If String.IsNullOrEmpty(measurement) Then
                Throw New Exception("measurement is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim gpSlope As New Slope
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            gpSlope.in_raster = gRaster.RasterDataset
            gpSlope.out_raster = outRaster
            gpSlope.output_measurement = measurement

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            'If TypeOf gExtentRaster Is GISDataStructures.Raster Then
            '    Dim extentGDAL As New GISDataStructures.RasterGDAL(sExtentRaster)
            '    SetConcurrentEnvironment(GP, extentGDAL)
            'End If

            GP.ClearMessages()

            Dim gResult As GISDataStructures.Raster = Nothing
            Try
                GP.Execute(gpSlope, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
                If GISDataStructures.Raster.Exists(outRaster) Then
                    gResult = New GISDataStructures.Raster(outRaster)
                Else
                    Throw New Exception("The output raster does not exist.")
                End If

            Catch ex As Exception
                ex.Data("inRaster") = gRaster.FullPath
                ex.Data("outRaster") = outRaster
                ex.Data("measurement") = measurement
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return gResult

        End Function

        'Public Function PointDensity(ByVal gPointFeatures As GISDataStructures.PointDataSource,
        '                        ByVal gReferenceRaster As GISDataStructures.Raster,
        '                        ByVal outRaster As String,
        '                        ByVal sNeighbourhood As String,
        '                        Optional sAreaUnits As String = "",
        '                        Optional ByVal inPopulationField As String = "NONE") As GISDataStructures.Raster

        '    If String.IsNullOrEmpty(outRaster) Then
        '        Throw New Exception("outRaster is null or empty string.")
        '    Else
        '        If GISDataStructures.Raster.Exists(outRaster) Then
        '            Dim ex As New Exception("The output raster path already exists.")
        '            ex.Data("Raster Path") =outRaster)
        '            Throw ex
        '        End If
        '    End If

        '    If String.IsNullOrEmpty(sNeighbourhood) Then
        '        Throw New Exception("Neighbourhood is null or empty string.")
        '    End If

        '    Dim GP As New Geoprocessor
        '    Dim gpPointDensity As New PointDensity
        '    Dim bAddtoMap As Boolean = GP.AddOutputsToMap

        '    gpPointDensity.in_point_features = gPointFeatures.FeatureClass
        '    gpPointDensity.population_field = inPopulationField
        '    gpPointDensity.out_raster = outRaster
        '    gpPointDensity.cell_size = gReferenceRaster.CellSize
        '    gpPointDensity.neighborhood = sNeighbourhood
        '    gpPointDensity.area_unit_scale_factor = sAreaUnits

        '    GP.TemporaryMapLayers = False
        '    GP.AddOutputsToMap = False
        '    GP.ClearMessages()

        '    Dim gResult As GISDataStructures.Raster = Nothing
        '    Try
        '        GP.Execute(gpPointDensity, Nothing)
        '        ExceptionBase.ProcessGPExceptions(GP)
        '        If GISDataStructures.Raster.Exists(outRaster) Then
        '            gResult = New GISDataStructures.Raster(outRaster)
        '        Else
        '            Throw New Exception("The output raster does not exist.")
        '        End If

        '    Catch ex As Exception
        '        ex.Data("inPointFeatures") =gPointFeatures.FullPath)
        '        ex.Data("inReferenceRaster") =gReferenceRaster.FullPath)
        '        ex.Data("outRaster") =outRaster)
        '        ex.Data("sNeighbourhood") =sNeighbourhood)
        '        ex.Data("sAreaUnits") =sAreaUnits)
        '        ex.Data("inPopulationField") =inPopulationField)
        '        ExceptionBase.AddGPMessagesToException(ex, GP)
        '        Throw
        '    Finally
        '        GP.AddOutputsToMap = bAddtoMap
        '    End Try

        '    Return gResult

        'End Function

        Public Sub Times(ByVal inRasterorConstantOne As String,
                        ByVal InRasterorConstantTwo As String,
                        ByVal outRaster As String)

            If String.IsNullOrEmpty(inRasterorConstantOne) Then
                Throw New Exception("inRasterorConstantOne is null or empty string.")
            End If

            If String.IsNullOrEmpty(InRasterorConstantTwo) Then
                Throw New Exception("InRasterorConstantTwo is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim Times As New Times
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Times.in_raster_or_constant1 = inRasterorConstantOne
            Times.in_raster_or_constant2 = InRasterorConstantTwo
            Times.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Times, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRasterorConstantOne") = inRasterorConstantOne
                ex.Data("InRasterorConstantTwo") = InRasterorConstantTwo
                ex.Data("outRaster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>This tool is technically not supported for use with scripting - it can work if you take some time to play with the expression syntax.
        ''' 
        ''' </summary>
        ''' <param name="expression"></param>
        ''' <param name="outRaster"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z000000z7000000.htm
        ''' Allows setting extent and cellsize for concurrency</remarks>
        Public Sub Raster_Calculator(ByVal expression As String,
                                     ByVal outRaster As String,
                                    ByVal ExtentRectangle As String,
                                     ByVal cellSize As Double)

            If String.IsNullOrEmpty(expression) Then
                Throw New Exception("expression is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            If Not GISCode.Extensions.ObtainExtension(Extensions.ESRI_Extensions.SpatialAnalyst, True) = ESRI.ArcGIS.esriSystem.esriExtensionState.esriESEnabled Then
                Throw New Exception("The Spatial Analyst Extension is Unavailable.")
            End If

            Dim GP As New Geoprocessor
            Dim RasterCalc As New RasterCalculator
            Dim bAddOutputsToMap As Boolean = GP.AddOutputsToMap
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            RasterCalc.expression = expression
            RasterCalc.output_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath) ' IO.Path.GetDirectoryName(sOutputRaster))
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            '
            ' Ensure that the geoprocessing environment is configured to produce concurrent results
            '
            GP.SetEnvironmentValue("extent", ExtentRectangle)
            GP.SetEnvironmentValue("cellSize", cellSize)

            GP.ClearMessages()

            Try
                GP.Execute(RasterCalc, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("expression") = expression
                ex.Data("outRaster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            GP.AddOutputsToMap = bAddOutputsToMap

        End Sub

        Public Sub Hillshade(ByVal in_raster As String, ByVal out_raster As String, Optional bObtainExtension As Boolean = True)

            ' Console applications cannot use the ExtensionManager that the ObtainExtension method uses. Instead they use the
            ' AOInit.CheckoutExtension method to obtain spatial analyst. Therefore this method should only do the extension
            ' check in UI products.
            If bObtainExtension Then
                If Not GISCode.Extensions.ObtainExtension(Extensions.ESRI_Extensions.SpatialAnalyst, False) = ESRI.ArcGIS.esriSystem.esriExtensionState.esriESEnabled Then
                    Throw New Exception("The Spatial Analyst Extension is Unavailable.")
                End If
            End If

            Dim GP As New Geoprocessor
            Dim HillShadeTool As New ESRI.ArcGIS.SpatialAnalystTools.HillShade
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            HillShadeTool.in_raster = in_raster
            HillShadeTool.out_raster = out_raster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(HillShadeTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("in_raster") = in_raster
                ex.Data("out_raster") = out_raster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub EuclideanDistance(ByVal inRaster As String, ByVal outDistanceRaster As String)

            Dim GP As New Geoprocessor
            Dim EuclideanTool As New ESRI.ArcGIS.SpatialAnalystTools.EucDistance
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            EuclideanTool.in_source_data = inRaster
            EuclideanTool.out_distance_raster = outDistanceRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(EuclideanTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("in_raster") = inRaster
                ex.Data("out_raster") = outDistanceRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub CellStatistics(ByVal inRasters As String, ByVal outRaster As String)

            Dim GP As New Geoprocessor
            Dim CellStatistics As New ESRI.ArcGIS.SpatialAnalystTools.CellStatistics
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CellStatistics.in_rasters_or_constants = inRasters
            CellStatistics.statistics_type = "SUM"
            CellStatistics.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(CellStatistics, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("in_raster") = inRasters
                ex.Data("out_raster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z00000038000000.htm
        ''' </remarks>
        Public Sub Nibble(ByVal in_raster As String,
                              ByVal in_mask_raster As String,
                              ByVal nibble_values As Boolean,
                              ByVal out_raster As String)

            If String.IsNullOrEmpty(in_raster) Then
                Throw New Exception("in_raster is null or empty string.")
            End If

            If String.IsNullOrEmpty(in_mask_raster) Then
                Throw New Exception("in_mask_raster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim NibbleTool As New Nibble
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            NibbleTool.in_raster = in_raster
            NibbleTool.in_mask_raster = in_mask_raster
            NibbleTool.nibble_values = nibble_values
            NibbleTool.out_raster = out_raster


            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(NibbleTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("in_raster") = in_raster
                ex.Data("in_mask_raster") = in_mask_raster
                ex.Data("nibble_values") = nibble_values
                ex.Data("out_raster") = out_raster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="in_raster"></param>
        ''' <param name="number_cells"></param>
        ''' <param name="zone_values"></param>
        ''' <param name="out_raster"></param>
        ''' <remarks>
        ''' http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z00000036000000.htm
        ''' License: Requires Spatial Analyst</remarks>
        Public Sub Expand(ByVal in_raster As String,
                          ByVal number_cells As Integer,
                          ByVal zone_values As String,
                          ByVal out_raster As String)

            'Validate inputs
            If String.IsNullOrEmpty(in_raster) Then
                Throw New Exception("Empty or null in_raster")
            End If

            If number_cells <= 0 Then
                'FP - originally, this was greater than 1, but 1 is a valid value
                'In the example from ESRI, http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z00000036000000.htm
                'They write "The value must be an integer greater than 1.", but their example uses a value of 1 "OutRas = Expand(InRas1, 1, [5])"
                Throw New Exception("number_cells must be an integer greater than 0.")
            End If

            If String.IsNullOrEmpty(out_raster) Then
                Throw New Exception("Empty or null out_raster")
            End If


            Dim GP As New Geoprocessor
            Dim ExpandTool As New ESRI.ArcGIS.SpatialAnalystTools.Expand
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ExpandTool.in_raster = in_raster
            ExpandTool.number_cells = number_cells
            ExpandTool.zone_values = zone_values
            ExpandTool.out_raster = out_raster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            Try
                GP.ClearMessages()
                GP.Execute(ExpandTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("in_raster") = in_raster
                ex.Data("number_cells") = number_cells
                ex.Data("zone_values") = zone_values
                ex.Data("out_raster") = out_raster
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
        ''' <param name="neighborhood"></param>
        ''' <param name="outRaster"></param>
        ''' <param name="extentRaster"></param>
        ''' <remarks>
        ''' http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//009z000000qs000000.htm
        ''' http://help.arcgis.com/en/sdk/10.0/arcobjects_net/componenthelp/index.html#/Members/004700002w54000000/
        ''' </remarks>
        Public Sub FocalStats(ByVal inRaster As String,
                              ByVal neighborhood As clsRasterNeighborhood,
                              ByVal statistics_type As FocalStatisticsTypeEnum,
                              ByVal outRaster As String,
                              Optional ByVal extentRaster As GISDataStructures.Raster = Nothing)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("inRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim FocalStatisticsTool As New FocalStatistics
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FocalStatisticsTool.in_raster = inRaster
            FocalStatisticsTool.neighborhood = neighborhood.ToString
            FocalStatisticsTool.statistics_type = statistics_type.ToString
            FocalStatisticsTool.out_raster = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            If TypeOf extentRaster Is GISDataStructures.Raster Then
                ' SetConcurrentEnvironment(GP, extentRaster)
                GP.SetEnvironmentValue("extent", extentRaster.ExtentAsString)
            End If
            GP.ClearMessages()

            Try
                GP.Execute(FocalStatisticsTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("neighborhood") = neighborhood.ToString
                ex.Data("outRaster") = outRaster
                ex.Data("extentRaster") = extentRaster.FullPath
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Function CreateConstantRaster(ByVal fValue As Double, ByVal sOutputRaster As String, ByVal pEnvelope As IEnvelope, ByVal fCellSize As Double, Optional ByVal sType As String = "FLOAT") As GISDataStructures.Raster

            If String.IsNullOrEmpty(sOutputRaster) Then
                Throw New ArgumentNullException("sOutputRaster", "The output raster path is null or empty")
            End If

            If Not TypeOf pEnvelope Is IEnvelope Then
                Throw New ArgumentNullException("pExtent", "The envelope is null or empty")
            End If

            If fCellSize <= 0 Then
                Throw New ArgumentOutOfRangeException("fCellSize", "The cell size must be greater than zero")
            End If

            sType = sType.ToUpper
            If Not (String.Compare(sType, "FLOAT", False) = 0 OrElse String.Compare(sType, "INTEGER") = 0) Then
                Throw New ArgumentOutOfRangeException("sType", "The raster type string is case sensitive and must be either ""FLOAT"" or ""INTEGER""")
            End If

            Dim GP As New Geoprocessor
            Dim CreateTool As New ESRI.ArcGIS.SpatialAnalystTools.CreateConstantRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CreateTool.out_raster = sOutputRaster
            CreateTool.extent = pEnvelope
            CreateTool.data_type = sType
            CreateTool.cell_size = fCellSize
            CreateTool.constant_value = fValue

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            Dim gResult As GISDataStructures.Raster = Nothing

            Try
                GP.ClearMessages()
                GP.Execute(CreateTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

                gResult = New GISDataStructures.Raster(sOutputRaster)
            Catch ex As Exception
                ex.Data("output raster") = sOutputRaster
                ex.Data("Type") = sType
                ex.Data("Cell Size") = fCellSize
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return gResult

        End Function

        ''' <summary>
        ''' ReMap Table Class for defining the inputs to the Reclassify geoprocessing tool below.
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ReMapTable

            ''' <summary>
            ''' Private class for storing the classes in a remap table.
            ''' </summary>
            ''' <remarks>The only public access to the members is through the
            ''' ToString method that formats the class in the correct way for the GP reclassify tool</remarks>
            Private Class ReMapValue

                Private m_fFrom As Double
                Private m_fTo As Double
                Private m_nNewValue As Integer

                ''' <summary>
                ''' 
                ''' </summary>
                ''' <param name="fFrom">Lower bound of the input values for this class</param>
                ''' <param name="fTo">Upper bound of the input values for this class</param>
                ''' <param name="nNewValue">New value for values falling between "from" and "to" values in the original raster.</param>
                ''' <remarks></remarks>
                Public Sub New(fFrom As Double, fTo As Double, nNewValue As Integer)

                    If fFrom > fTo Then
                        Dim ex As New Exception("Invalid remap table value for Spatial Analyst Reclassify geoprocessing tool.")
                        ex.Data("From") = fFrom
                        ex.Data("To") = fTo
                        ex.Data("New Value") = nNewValue
                        Throw ex
                    End If

                    m_fFrom = fFrom
                    m_fTo = fTo
                    m_nNewValue = nNewValue
                End Sub

                Public Overrides Function ToString() As String
                    Return String.Format("{0} {1} {2}", m_fFrom, m_fTo, m_nNewValue)
                End Function
            End Class

            Private m_dReMap As List(Of ReMapValue)
            Private m_bIncludeMissingValues As Boolean
            Private m_bHasNoDataValue As Boolean
            Private m_nNoDataValue As Integer

            Public Sub New(bIncludeMissingValues As Boolean)
                m_dReMap = New List(Of ReMapValue)
                m_bIncludeMissingValues = bIncludeMissingValues
                m_bHasNoDataValue = False
            End Sub

            Public Sub AddClass(fFrom As Double, fTo As Double, nNewValue As Integer)
                m_dReMap.Add(New ReMapValue(fFrom, fTo, nNewValue))
            End Sub

            Public Sub AddNoData(nNewValue As Integer)
                m_bHasNoDataValue = True
                m_nNoDataValue = nNewValue
            End Sub

            ''' <summary>
            ''' Retrieve the list of remap classes in the correct format for use in the Reclassify geoprocessing tool
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property ReMapTableString As String
                Get
                    Dim sResult As String = String.Empty
                    For Each aValue As ReMapValue In m_dReMap
                        If Not String.IsNullOrEmpty(sResult) Then
                            sResult &= ";"
                        End If
                        sResult &= aValue.ToString
                    Next

                    If m_bHasNoDataValue Then
                        If Not String.IsNullOrEmpty(sResult) Then
                            sResult &= ";"
                        End If
                        sResult &= "NoData " & m_nNoDataValue.ToString
                    End If

                    If sResult.EndsWith(";") Then
                        sResult = sResult.Substring(0, sResult.Length - 1)
                    End If
                    Return sResult

                End Get
            End Property

            Public ReadOnly Property IncludeMissingValues As String
                Get
                    If m_bIncludeMissingValues Then
                        Return "DATA"
                    Else
                        Return "NODATA"
                    End If
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Reclassify a raster
        ''' </summary>
        ''' <param name="gInputRaster">Existing input raster</param>
        ''' <param name="theRemapTable">Reclassification table. See the ReMapTable Class above</param>
        ''' <param name="sOutputRaster">Output raster that is produced by the reclassification</param>
        ''' <param name="sReclassField">Name of the field used for reclassification. Leave as default "Value" in most cases.</param>
        ''' <returns>The INTEGER raster output of the reclassify tool</returns>
        ''' <remarks>http://webhelp.esri.com/arcgisdesktop/9.3/index.cfm?TopicName=Reclassify</remarks>
        Public Function Reclassify(gInputRaster As GISDataStructures.Raster, theRemapTable As ReMapTable, sOutputRaster As String, Optional sReclassField As String = "Value") As GISDataStructures.Raster

            If TypeOf gInputRaster Is GISDataStructures.Raster Then
                If Not GISDataStructures.Raster.Exists(gInputRaster.FullPath) Then
                    Dim ex As New Exception("The input raster does not exist.")
                    ex.Data("Input raster") = gInputRaster.FullPath
                    Throw ex
                End If
            Else
                Throw New Exception("The input raster is not valid.")
            End If

            If String.IsNullOrEmpty(theRemapTable.ReMapTableString) Then
                Throw New Exception("The reclassify table is empty.")
            End If

            If String.IsNullOrEmpty(sOutputRaster) Then
                Throw New Exception("outRaster is null or empty string.")
            Else
                If GISDataStructures.Raster.Exists(sOutputRaster) Then
                    Dim ex As New Exception("The output raster already exists.")
                    ex.Data("Output raster") = sOutputRaster
                    Throw ex
                End If
            End If

            Dim GP As New Geoprocessor
            Dim reclassTool As New Reclassify
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            reclassTool.in_raster = gInputRaster.FullPath
            reclassTool.reclass_field = sReclassField
            reclassTool.remap = theRemapTable.ReMapTableString
            reclassTool.out_raster = sOutputRaster
            reclassTool.missing_values = theRemapTable.IncludeMissingValues

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(reclassTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = gInputRaster.FullPath
                ex.Data("Reclass Field") = sReclassField
                ex.Data("Remap Table") = theRemapTable.ReMapTableString
                ex.Data("Data NoData") = theRemapTable.IncludeMissingValues
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Dim gResult As GISDataStructures.Raster = Nothing
            If GISDataStructures.Raster.Exists(sOutputRaster) Then
                gResult = New GISDataStructures.Raster(sOutputRaster)
            End If

            Return gResult

        End Function

    End Module

End Namespace
