Imports System.IO
Imports Microsoft.Win32

Public Class frm_SimpleToPCAT_RoughnessRaster

    Public Sub New(ByRef pApp As ESRI.ArcGIS.Framework.IApplication)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        m_pArcMap = pApp

    End Sub

    Private m_pArcMap As ESRI.ArcGIS.Framework.IApplication
    Public Property Filter As String
    'Dim m_SpatialRef As String
    Dim m_SpatialRef As ESRI.ArcGIS.Geometry.ISpatialReference

    Private Sub btn_OpenRawPointFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OpenRawPointFile.Click

        'MBES_ToolsNew.frm_ToPCAT.Error_NotSpaceDelimited.Clear()
        TopCAT.WindowsFormAssistant.OpenFileDialog("Raw Point Cloud", txtBox_RawPointCloud)
        If Not TopCAT.ToPCAT_Assistant.CheckIfToPCAT_Ready(txtBox_RawPointCloud.Text) Then
            MsgBox("This file is not in a format accepted by ToPCAT. Use ToPCAT Prep Tool to convert it to a suitable format.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            txtBox_RawPointCloud.Text = String.Empty
            Exit Sub
        End If



    End Sub

    Private Sub txtBox_RawPointCloud_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_RawPointCloud.TextChanged

    End Sub

    Private Sub xResolution_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles xResolution.ValueChanged

        If xResolution.Value <= 0 Then
            TopCAT.ToPCAT_Assistant.ResolutionWarning()
            xResolution.Value = 1
        End If

        updn_CellSize.Value = xResolution.Value

    End Sub

    Private Sub yResolution_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles yResolution.ValueChanged

        If yResolution.Value <= 0 Then
            TopCAT.ToPCAT_Assistant.ResolutionWarning()
            yResolution.Value = 1
        End If

    End Sub

    Private Sub nToCalculateStats_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nToCalculateStats.ValueChanged

        If nToCalculateStats.Value < 2 Then
            System.Windows.Forms.MessageBox.Show("This value is invalid! Value must be greater than 2. Change your input.", "Error with Input Value")
            nToCalculateStats.Value = 2
        End If

    End Sub

    Private Sub btn_SpatialReference_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_SpatialReference.Click

        TopCAT.WindowsFormAssistant.OpenFileDialog("Spatial Reference", txtBox_SpatialReference)

        '''''''''''''''''''''''''''''
        'OLD METHOD
        '
        'If String.Compare(IO.Path.GetExtension(txtBox_SpatialReference.Text), ".prj") = 0 Then
        '    m_SpatialRef = txtBox_SpatialReference.Text
        '    Try
        '        Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TopCAT.GIS.LoadProjectedCoordinateSystem(m_SpatialRef)
        '        label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
        '    Catch ex As Exception
        '        'Do Nothing
        '    End Try

        '    Exit Sub
        'End If

        'Dim shapefileProjectedCheck As Boolean = TopCAT.WindowsFormAssistant.CheckIfShapefileHasPrjFile(txtBox_SpatialReference.Text)
        'If shapefileProjectedCheck = True Then
        '    m_SpatialRef = txtBox_SpatialReference.Text
        '    'm_SpatialRef = [String].Join("\", System.IO.Path.GetDirectoryName(txtBox_SpatialReference.Text), IO.Path.GetFileNameWithoutExtension(txtBox_SpatialReference.Text)) & ".prj"
        '    Try
        '        Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TopCAT.GIS.LoadProjectedCoordinateSystem(m_SpatialRef)
        '        label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
        '    Catch ex As Exception
        '        'Do Nothing
        '    End Try

        '    Exit Sub
        'End If
        '''''''''''''''''''''''''''''''''''''

        If IO.File.Exists(txtBox_SpatialReference.Text) Then
            If String.Compare(IO.Path.GetExtension(txtBox_SpatialReference.Text), ".prj") = 0 Then
                'm_SpatialRef = txtBox_SpatialReference.Text
                'Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TopCAT.GIS.LoadProjectedCoordinateSystem(txtBox_SpatialReference.Text)
                m_SpatialRef = TopCAT.GIS.LoadProjectedCoordinateSystem(txtBox_SpatialReference.Text)
                m_SpatialRef = TryCast(m_SpatialRef, ESRI.ArcGIS.Geometry.ISpatialReference)
                Try
                    Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TryCast(m_SpatialRef, ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem)
                    label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
                Catch ex As Exception
                    'Do Nothing 
                End Try

                Exit Sub
            Else
                Dim shapefileProjectedCheck As Boolean = TopCAT.WindowsFormAssistant.CheckIfShapefileHasPrjFile(txtBox_SpatialReference.Text)
                If shapefileProjectedCheck Then
                    Dim joinPath = IO.Path.Combine(IO.Path.GetDirectoryName(txtBox_SpatialReference.Text), IO.Path.GetFileNameWithoutExtension(txtBox_SpatialReference.Text) & ".prj")
                    m_SpatialRef = TopCAT.GIS.LoadProjectedCoordinateSystem(joinPath)
                    m_SpatialRef = TryCast(m_SpatialRef, ESRI.ArcGIS.Geometry.ISpatialReference)
                    'm_SpatialRef = joinPath
                    'Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TopCAT.GIS.LoadProjectedCoordinateSystem(joinPath)
                    Try
                        Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TryCast(m_SpatialRef, ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem)
                        label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
                    Catch
                        'Do Nothing
                    End Try

                    Exit Sub
                End If
            End If
        Else
            MsgBox("The provided spatial reference file does not exist.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If
    End Sub

    Private Sub txtBox_SpatialReference_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_SpatialReference.TextChanged

    End Sub

    Private Sub updn_CellSize_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles updn_CellSize.ValueChanged

    End Sub

    Private Sub btn_OutputRaster_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OutputRaster.Click

        'TopCAT.WindowsFormAssistant.SaveFileDialog("Raster", txtBox_OutputRaster)
        Dim sOutputRasterPath As String = GISDataStructures.Raster.BrowseSave("Select Raster Output Location")
        If Not String.IsNullOrEmpty(sOutputRasterPath) Then
            txtBox_OutputRaster.Text = sOutputRasterPath
        End If

    End Sub

    Private Sub txtBox_OutputRaster_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_OutputRaster.TextChanged

    End Sub


    Private Sub btn_Run_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Run.Click

        If Not ValidateForm() Then
            Exit Sub
        End If

        Try
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor

            TopCAT.ToPCAT_Assistant.RunToPCat(txtBox_RawPointCloud.Text,
                                              xResolution.Value,
                                              yResolution.Value,
                                              nToCalculateStats.Value.ToString())

            Dim tempDir = WindowsManagement.CreateTemporaryDirectory("ToPCAT_Raster")
            TopCAT.ToPCAT_Assistant.MoveToPCAT_TextFiles(txtBox_RawPointCloud.Text, tempDir)

            Dim zStatPath As String = tempDir & "\" & System.IO.Path.GetFileNameWithoutExtension(txtBox_RawPointCloud.Text) & "_zstat.txt"

            'If a z field is being used that does not rely on statistics to calculate then append the underpopulated_zstat file to the zstat file to create the XYEventLayer
            If cmb_ZField.SelectedItem = "n" Or cmb_ZField.SelectedItem = "zmean" Or cmb_ZField.SelectedItem = "zmin" Or cmb_ZField.SelectedItem = "zmax" Or cmb_ZField.SelectedItem = "range" Then
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
            End If

            ''''''''''''''''''''''''''''''''''
            'NEW CODE TO UTILIZE THE TEMPORARY XY EVENT LAYER TO GET AN EXTENT AND USE THAT AS INPUT WHEN CREATING THE RASTER
            '
            Dim geoProcessorEngine As ESRI.ArcGIS.Geoprocessing.IGeoProcessor2 = New ESRI.ArcGIS.Geoprocessing.GeoProcessor()
            geoProcessorEngine.AddOutputsToMap = False
            Dim geoProcessingUtility As ESRI.ArcGIS.Geoprocessing.IGPUtilities3 = New ESRI.ArcGIS.Geoprocessing.GPUtilitiesClass()
            Dim tempPointLayerParameters As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            tempPointLayerParameters.Add(zStatPath)
            tempPointLayerParameters.Add("x")
            tempPointLayerParameters.Add("y")
            tempPointLayerParameters.Add("in_memory")
            tempPointLayerParameters.Add(m_SpatialRef)
            tempPointLayerParameters.Add(TopCAT.ToPCAT_Assistant.getZstatColumnIndex(cmb_ZField.SelectedItem))
            Dim geoProcessingResults As ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult2
            geoProcessingResults = CType(geoProcessorEngine.Execute("MakeXYEventLayer_management", tempPointLayerParameters, Nothing), ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult2)
            Dim geoProcessingValues As ESRI.ArcGIS.Geodatabase.IGPValue = geoProcessingResults.GetOutput(0)
            Dim inMemoryLyrPath As String = geoProcessingValues.GetAsText()
            Dim pGL As ESRI.ArcGIS.Carto.ILayer = geoProcessingUtility.MakeLayer(geoProcessingResults.GetOutput(0), inMemoryLyrPath, Nothing)
            Dim pGeoDataset As ESRI.ArcGIS.Geodatabase.IGeoDataset = CType(pGL, ESRI.ArcGIS.Geodatabase.IGeoDataset)
            Dim pExtent As ESRI.ArcGIS.Geometry.IEnvelope = pGeoDataset.Extent

            Dim extentRectangle As New GISDataStructures.ExtentRectangle(pExtent.YMax, pExtent.XMin, pExtent.XMax, pExtent.YMin)

            'ENSURE ORTHOGONALITY OF OUTPUT RASTER
            extentRectangle.MakeOrthogonal(updn_CellSize.Value)

            'Set the geoprocessing extent environment
            geoProcessorEngine.SetEnvironmentValue("extent", extentRectangle.Rectangle)

            Dim pointToRasterParameters As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            pointToRasterParameters.Add(tempPointLayerParameters.Element(3))
            pointToRasterParameters.Add(TopCAT.ToPCAT_Assistant.getZstatColumnIndex(cmb_ZField.SelectedItem))
            pointToRasterParameters.Add(txtBox_OutputRaster.Text)
            pointToRasterParameters.Add("")
            pointToRasterParameters.Add("")
            pointToRasterParameters.Add(updn_CellSize.Value.ToString())
            TopCAT.MyGeoprocessing.RunToolGeoprocessingTool("PointToRaster_conversion", geoProcessorEngine, pointToRasterParameters, True)

            Try

                Dim pSR As ESRI.ArcGIS.Geometry.ISpatialReference = Nothing
                If TypeOf m_SpatialRef Is ESRI.ArcGIS.Geometry.ISpatialReference Then
                    pSR = m_SpatialRef
                End If

                If GISDataStructures.Raster.Exists(txtBox_OutputRaster.Text) Then
                    GP.DataManagement.DefineProjection(txtBox_OutputRaster.Text, pSR)
                End If

            Catch
                'Do nothing
            End Try

            System.IO.Directory.Delete(tempDir, True)

            If My.Settings.AddOutputLayersToMap Then
                Try
                    Dim pMXDoc As ESRI.ArcGIS.ArcMapUI.IMxDocument = m_pArcMap.Document
                    Dim gRaster As New GISDataStructures.Raster(txtBox_OutputRaster.Text)
                    gRaster.AddToMap(m_pArcMap)
                Catch ex As Exception
                    'Do Nothing
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

    Private Sub btn_Help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Help.Click

        System.Diagnostics.Process.Start(My.Resources.HelpBaseURL & "gcd-command-reference/gcd-analysis-menu/b-roughness-analysis-submenu/i-simple-topcat-roughness")

    End Sub


    Private Function ValidateForm() As Boolean

        'Check the different inputs to make sure that the inputs will not cause an error to be thrown
        If String.IsNullOrEmpty(txtBox_RawPointCloud.Text) Then
            MsgBox("Please select a point cloud to process.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If (cmb_ZField.SelectedIndex = -1) Then
            MsgBox("Please select a value to from the 'Raster Value' drop-down menu to create the raster from.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If String.IsNullOrEmpty(txtBox_OutputRaster.Text) Then
            MsgBox("Please select provide a path to save the output to.", MsgBoxStyle.OkOnly, "No Filepath Given to Save Output")
            Return False
        ElseIf GISDataStructures.Raster.Exists(txtBox_OutputRaster.Text) Then
            MsgBox("A raster already exists with the path and filename provided to save the raster to. Please provide a different path and/or filename to save the raster to.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If ValidateSavedRasterType(txtBox_OutputRaster.Text) = False Then
            MsgBox("Unsupported raster format by the " & My.Resources.ApplicationNameShort & ". Please save the output raster to a supported datatype.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If updn_CellSize.Value <> xResolution.Value Then
            Dim response As New MsgBoxResult
            response = MsgBox("In general the output cell size should match the sample size window you are decimating to." & vbCrLf & vbCrLf & "Would you like GCD to correct this for you?", MsgBoxStyle.YesNoCancel, My.Resources.ApplicationNameLong)
            If response = MsgBoxResult.Yes Then
                updn_CellSize.Value = xResolution.Value
                updn_CellSize.Refresh()
            ElseIf response = MsgBoxResult.No Then
            ElseIf response = MsgBoxResult.Cancel Then
                Return False
            End If
            Return True
        End If

        Return True
    End Function

    Private Function ValidateSavedRasterType(ByVal sRasterPath As String) As Boolean

        If String.IsNullOrEmpty(sRasterPath) Then
            Throw New ArgumentNullException("sRasterPath", "The path cannot be null or empty")
        End If

        Dim ext = System.IO.Path.GetExtension(sRasterPath)

        Select Case ext

            Case String.Empty
                If GISDataStructures.IsFileGeodatabase(System.IO.Path.GetDirectoryName(sRasterPath)) = False Then
                    Return False
                Else
                    Return True
                End If

            Case ".tif", ".img" : Return True
            Case Else : Return False

        End Select

    End Function

    Private Sub frm_SimpleToPCAT_SR_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        cmb_ZField.SelectedIndex = 6
        ttip.SetToolTip(btn_OpenRawPointFile, "Press this button to open a file dialog and select a raw point cloud file.")
        ttip.SetToolTip(txtBox_RawPointCloud, "Displays the file name of the point cloud. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(xResolution, "Select x dimension of the decimation sample window. In units of raw point cloud file.")
        ttip.SetToolTip(yResolution, "Select y dimension of the decimation sample window. In units of raw point cloud file.")
        ttip.SetToolTip(nToCalculateStats, "Select the minimum number of points necessary to calculate sample window statistics. Can't be less than 2 points.")
        ttip.SetToolTip(btn_ExtentShp, "Press this button to open the file dialog and select a polygon shapefile representing the extent of the point cloud.")
        ttip.SetToolTip(txtBox_ExtentShp, "Displays the file name of the input extent polygon. Do not type the file name, use the selection dialog button to the right.")
        ttip.SetToolTip(btn_SpatialReference, "Press this button to open a file dialog and select a file (.prj or .shp) containing spatial reference information.")
        ttip.SetToolTip(txtBox_SpatialReference, "Displays the file name of the spatial reference. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(btn_OutputRaster, "Press this button to open a file dialog to name and save the output raster to.")
        ttip.SetToolTip(txtBox_OutputRaster, "Diplays the file name of the output raster. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(updn_CellSize, "Select the cell size of the output raster.")
        ttip.SetToolTip(btn_Run, "Click to run the analysis")
        ttip.SetToolTip(btn_Help, "Click to go to the tool documentation.")
        ttip.SetToolTip(btn_Cancel, "Cancel analysis and exit the tool window.")
    End Sub

End Class