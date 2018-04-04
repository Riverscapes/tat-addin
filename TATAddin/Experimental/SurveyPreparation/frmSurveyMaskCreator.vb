Imports System.IO
Imports Microsoft.Win32

Public Class frmSurveyMaskCreator

    Public Sub New(ByRef pApp As ESRI.ArcGIS.Framework.IApplication)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        m_pArcMap = pApp

    End Sub

    Private m_pArcMap As ESRI.ArcGIS.Framework.IApplication
    Public Property Filter As String

    Dim m_SpatialRef As String

    Private Sub btn_OpenRawPointFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OpenRawPointFile.Click

        'TopCAT.MBES_ToolsNew.frm_ToPCAT.Error_NotSpaceDelimited.Clear()
        TopCAT.WindowsFormAssistant.OpenFileDialog("Raw Point Cloud", txtBox_RawPointCloud)

        TopCAT.ToPCAT_Assistant.CheckIfToPCAT_Ready(txtBox_RawPointCloud.Text)

    End Sub

    Private Sub xResolution_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles xResolution.Click

        If xResolution.Value <= 0 Then
            TopCAT.ToPCAT_Assistant.ResolutionWarning()
            xResolution.Value = 1
        End If

    End Sub

    Private Sub yResolution_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles yResolution.Click

        If yResolution.Value <= 0 Then
            TopCAT.ToPCAT_Assistant.ResolutionWarning()
            yResolution.Value = 1
        End If

    End Sub

    Private Sub nToCalculateStats_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nToCalculateStats.Click

        If nToCalculateStats.Value < 2 Then
            MsgBox("This value is invalid! Value must be greater than 2. Change your input.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            nToCalculateStats.Value = 2
        End If

    End Sub

    Private Sub btn_SpatialReference_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_SpatialReference.Click

        TopCAT.WindowsFormAssistant.OpenFileDialog("Spatial Reference", txtBox_SpatialReference)

        If IO.Path.GetExtension(txtBox_SpatialReference.Text) = ".prj" Then
            m_SpatialRef = txtBox_SpatialReference.Text
            'Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = GIS.LoadProjectedCoordinateSystem(m_SpatialRef)
            'label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
            Exit Sub
        End If

        Dim shapefileProjectedCheck As Boolean = TopCAT.WindowsFormAssistant.CheckIfShapefileHasPrjFile(txtBox_SpatialReference.Text)
        If shapefileProjectedCheck = True Then
            m_SpatialRef = IO.Path.Combine(IO.Path.GetDirectoryName(txtBox_SpatialReference.Text), IO.Path.GetFileNameWithoutExtension(txtBox_SpatialReference.Text) & ".prj")
            'Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = GIS.LoadProjectedCoordinateSystem(m_SpatialRef)
            'label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
            Exit Sub
        End If



    End Sub


    Private Sub btn_OutputShp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OutputShp.Click

        'TopCAT.WindowsFormAssistant.SaveFileDialog("Shapefile", txtOutputShp)
        Dim sOutputShpPath As String = GISDataStructures.VectorDataSource.BrowseSave("Save Polygon Vector Data", Nothing, Nothing, 0, Nothing, GISDataStructures.GeometryTypes.Polygon)
        If Not String.IsNullOrEmpty(sOutputShpPath) Then
            txtOutputShp.Text = sOutputShpPath
        End If


    End Sub

    Private Sub btn_Run_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Run.Click

        If Not ValidateForm() Then
            Exit Sub
        End If

        TopCAT.ToPCAT_Assistant.RunToPCat(txtBox_RawPointCloud.Text,
                                          xResolution.Value,
                                          yResolution.Value,
                                          nToCalculateStats.Value.ToString())

        Dim tempDir = WindowsManagement.CreateTemporaryDirectory("SurveyMask")
        TopCAT.ToPCAT_Assistant.MoveToPCAT_TextFiles(txtBox_RawPointCloud.Text, tempDir)

        Try
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor

            ''''''''''''''''''''''''''''''''''
            'NEW CODE TO UTILIZE THE TEMPORARY XY EVENT LAYER TO GET AN EXTENT AND USE THAT AS INPUT WHEN CREATING THE RASTER
            '

            Dim zStatPath As String = tempDir & "\" & System.IO.Path.GetFileNameWithoutExtension(txtBox_RawPointCloud.Text) & "_zstat.txt"

            'If a z field is being used that does not rely on statistics to calculate then append the underpopulated_zstat file to the zstat file to create the XYEventLayer
            Using streamWriter As New IO.StreamWriter(zStatPath, True)
                Dim underpopulatedZstatPath As String = tempDir & "\" & System.IO.Path.GetFileNameWithoutExtension(txtBox_RawPointCloud.Text) & "_underpopulated_zstat.txt"
                Using streamReader As New IO.StreamReader(underpopulatedZstatPath)
                    streamReader.ReadLine()
                    Do While (streamReader.Peek() > -1)
                        Dim newLine As String = streamReader.ReadLine

                        If String.Compare(newLine, "") = 0 Then
                            Continue Do
                        End If
                        streamWriter.WriteLine(newLine)

                    Loop
                End Using
            End Using

            Dim geoProcessorEngine As ESRI.ArcGIS.Geoprocessing.IGeoProcessor2 = New ESRI.ArcGIS.Geoprocessing.GeoProcessor()
            geoProcessorEngine.SetEnvironmentValue("scratchWorkspace", tempDir)
            geoProcessorEngine.AddOutputsToMap = False
            Dim geoProcessingUtility As ESRI.ArcGIS.Geoprocessing.IGPUtilities3 = New ESRI.ArcGIS.Geoprocessing.GPUtilitiesClass()
            Dim tempPointLayerParameters As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            tempPointLayerParameters.Add(zStatPath)
            tempPointLayerParameters.Add("x")
            tempPointLayerParameters.Add("y")
            tempPointLayerParameters.Add("in_memory")
            tempPointLayerParameters.Add(m_SpatialRef)
            Dim geoProcessingResults As ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult2
            geoProcessingResults = CType(geoProcessorEngine.Execute("MakeXYEventLayer_management", tempPointLayerParameters, Nothing), ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult2)
            Dim geoProcessingValues As ESRI.ArcGIS.Geodatabase.IGPValue = geoProcessingResults.GetOutput(0)
            Dim inMemoryLyrPath As String = geoProcessingValues.GetAsText()
            Dim pGL As ESRI.ArcGIS.Carto.ILayer = geoProcessingUtility.MakeLayer(geoProcessingResults.GetOutput(0), inMemoryLyrPath, Nothing)


            Dim pointToRasterParameters As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            pointToRasterParameters.Add(inMemoryLyrPath)
            pointToRasterParameters.Add("n")
            pointToRasterParameters.Add(tempDir & "\temp_n.tif")
            pointToRasterParameters.Add("")
            pointToRasterParameters.Add("")
            pointToRasterParameters.Add(xResolution.Value.ToString())

            TopCAT.MyGeoprocessing.RunToolGeoprocessingTool("PointToRaster_conversion", geoProcessorEngine, pointToRasterParameters, True)
            Dim thisIsNothing As String = Nothing

            Dim rasterWorkspace = TopCAT.GIS.OpenRasterWorkspace(tempDir)
            Dim tempRas As ESRI.ArcGIS.Geodatabase.IRasterDataset = rasterWorkspace.OpenRasterDataset("temp_n.tif")
            Dim numRemap As ESRI.ArcGIS.GeoAnalyst.INumberRemap
            numRemap = New ESRI.ArcGIS.GeoAnalyst.NumberRemapClass()
            numRemap.MapRange(1.0, 10000000.0, 1)
            Dim reclassOp As ESRI.ArcGIS.GeoAnalyst.IReclassOp
            reclassOp = New ESRI.ArcGIS.GeoAnalyst.RasterReclassOpClass()
            Dim rasOut As ESRI.ArcGIS.Geodatabase.IRaster
            rasOut = reclassOp.ReclassByRemap(tempRas, numRemap, True)

            'Dim reclassifyRasterParameters As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray

            'reclassifyRasterParameters.Add(tempDir & "\temp_n.img")
            'reclassifyRasterParameters.Add("Value")
            'reclassifyRasterParameters.Add("[[1,1000000000,1]]")
            'reclassifyRasterParameters.Add(tempDir & "\temp_nRe.img")

            'Geoprocessing.RunToolGeoprocessingTool("Reclassify", geoProcessorEngine, reclassifyRasterParameters)

            Dim rasterToPolygonParameters As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            rasterToPolygonParameters.Add(rasOut)
            rasterToPolygonParameters.Add(tempDir & "\temp_nRe.shp")
            rasterToPolygonParameters.Add("NO_SIMPLIFY")

            TopCAT.MyGeoprocessing.RunToolGeoprocessingTool("RasterToPolygon_conversion", geoProcessorEngine, rasterToPolygonParameters, True)

            'Clean up the polygon shapefile by combining all polygons in shapefile that have the same gridcode
            Dim dissolveMaskParameters As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            dissolveMaskParameters.Add(tempDir & "\temp_nRe.shp")
            dissolveMaskParameters.Add(txtOutputShp.Text)
            dissolveMaskParameters.Add("GRIDCODE")
            dissolveMaskParameters.Add("")
            dissolveMaskParameters.Add("MULTI_PART")

            TopCAT.MyGeoprocessing.RunToolGeoprocessingTool("Dissolve_management", geoProcessorEngine, dissolveMaskParameters, True)

            'remove reference to tempRas to enable the temporary directory to be deleted
            Dim remainingReferences As Integer = 0
            Do
                remainingReferences = System.Runtime.InteropServices.Marshal.ReleaseComObject(tempRas)
            Loop While remainingReferences > 0

            System.IO.Directory.Delete(tempDir, True)

            If My.Settings.AddOutputLayersToMap Then
                Try
                    TopCAT.GIS.AddShapefile(m_pArcMap, txtOutputShp.Text)
                Catch ex As Exception
                    'Do nothing
                End Try
            End If

        Catch ex As Exception
            ExceptionUI.HandleException(ex)
        Finally
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
        End Try

        Me.Close()

    End Sub

    Private Sub btn_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click

        Me.Close()

    End Sub

    Private Function ValidateForm()

        'Check the different inputs to make sure that the inputs will not cause an error to be thrown
        If String.IsNullOrEmpty(txtBox_RawPointCloud.Text) Then
            MsgBox("Please select a point cloud to process.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        ElseIf String.IsNullOrEmpty(txtOutputShp.Text) Then
            MsgBox("Please select provide a path to save the output to.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        Return True
    End Function

    Private Sub btn_Help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Help.Click

        System.Diagnostics.Process.Start(My.Resources.HelpBaseURL & "gcd-command-reference/data-prep-menu/c-survey-preparation-menu/ii-create-survey-extent-polygon")

    End Sub

    Private Sub frm_SimpleToPCAT_SR_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ttip.SetToolTip(btn_OpenRawPointFile, "Press this button to open a file dialog and select a raw point cloud file.")
        ttip.SetToolTip(txtBox_RawPointCloud, "Displays the file name of the point cloud. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(xResolution, "Select x dimension of the decimation sample window. In units of raw point cloud file.")
        ttip.SetToolTip(yResolution, "Select y dimension of the decimation sample window. In units of raw point cloud file.")
        ttip.SetToolTip(nToCalculateStats, "Select the minimum number of points necessary to calculate sample window statistics. Can't be less than 2 points.")
        ttip.SetToolTip(btn_SpatialReference, "Press this button to open a file dialog and select a file (.prj or .shp) containing spatial reference information.")
        ttip.SetToolTip(txtBox_SpatialReference, "Displays the file name of the spatial reference. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(btn_Run, "Click to run the analysis")
        ttip.SetToolTip(btn_Help, "Click to go to the tool documentation.")
        ttip.SetToolTip(btn_Cancel, "Cancel analysis and exit the tool window.")
    End Sub


End Class