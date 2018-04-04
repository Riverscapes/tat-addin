Imports System.Windows.Forms
Imports Microsoft.Win32
Imports System.IO
'Imports Microsoft.Isam.Esent.Collections.Generic
Imports System.Threading


Public Class frm_CoincidentPointsShp

    Public Sub New(ByRef pApp As ESRI.ArcGIS.Framework.IApplication)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        m_pArcMap = pApp

    End Sub

    Private m_pArcMap As ESRI.ArcGIS.Framework.IApplication
    Public Property Filter As String
    Dim m_ExtentShp_gxDialog As New ESRI.ArcGIS.CatalogUI.GxDialog

    'GET THE PATH OF THE INSTALLTION FOLDER

    Private Sub btn_OutputShp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OutputShp.Click

        'TopCAT.WindowsFormAssistant.SaveFileDialog("Shapefile", txtBox_OutputShp)

        Dim sOutputShpPath As String = GISDataStructures.VectorDataSource.BrowseSave("Save Point Vector Data", Nothing, Nothing, 0)
        If Not String.IsNullOrEmpty(sOutputShpPath) Then
            txtBox_OutputShp.Text = sOutputShpPath
        End If


    End Sub

    Private Sub txtBox_OutputShp_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_OutputShp.TextChanged

    End Sub

    Private Sub ChkBox_SetPrecision_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkBox_SetPrecision.CheckedChanged

        If ChkBox_SetPrecision.Checked = True Then
            NumericUpDown_Precision.Enabled = True
        ElseIf ChkBox_SetPrecision.Checked = False Then
            NumericUpDown_Precision.Enabled = False
        End If

    End Sub

    Private Sub btn_RawPointCloud_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_RawPointCloud.Click

        TopCAT.WindowsFormAssistant.OpenFileDialog("Raw Point Cloud", txtBox_RawPointCloud)

    End Sub

    Private Sub txtBox_RawPointCloud_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_RawPointCloud.TextChanged

    End Sub

    Private Sub cmbBox_SelectSeparator_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Box_Seperator.SelectedIndexChanged

        Cmb_Box_Seperator.SelectedItem = Cmb_Box_Seperator.SelectedItem

    End Sub

    Private Sub btnPreviewFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPreviewFile.Click

        TopCAT.ToPCAT_Assistant.PreviewFirstLine(txtBox_RawPointCloud.Text)

    End Sub

    Private Sub btn_ExtentShp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ExtentShp.Click

        TopCAT.WindowsFormAssistant.OpenGxFileDialog("Shapefile", "Select An Extent Shapefile", 0)
        txtBox_ExtentShp.Text = m_ExtentShp_gxDialog.InternalCatalog.SelectedObject.Name

    End Sub

    Private Sub btn_SpatialReference_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_SpatialReference.Click

        TopCAT.WindowsFormAssistant.OpenFileDialog("Spatial Reference", txtBox_SpatialReference)

        'If IO.Path.GetExtension(txtBox_SpatialReference.Text) = ".prj" Then
        '    'm_SpatialRef = m_SpatialRef_FileDialog.FileName
        '    'Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = GIS.LoadProjectedCoordinateSystem(m_SpatialRef)
        '    'label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
        '    Exit Sub
        'End If

        'Dim shapefileProjectedCheck As Boolean = TopCAT.WindowsFormAssistant.CheckIfShapefileHasPrjFile(txtBox_SpatialReference.Text)
        'If shapefileProjectedCheck = True Then
        '    'txtBox_SpatialReference.Text = System.IO.Path.GetFileName(txtBox_SpatialReference.Text)
        '    'm_SpatialRef = [String].Join("\", System.IO.Path.GetDirectoryName(m_SpatialRef_FileDialog.FileName), IO.Path.GetFileNameWithoutExtension(m_SpatialRef_FileDialog.FileName)) & ".prj"
        '    'Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = GIS.LoadProjectedCoordinateSystem(m_SpatialRef)
        '    'label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
        '    Exit Sub
        'End If

    End Sub


    Private Sub txtBox_SpatialReference_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_SpatialReference.TextChanged

    End Sub

    Private Sub btn_Run_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Run.Click

        If Not ValidateForm() Then
            Exit Sub
        End If

        Dim seperator As String = TopCAT.WindowsFormAssistant.GetSeperator(Cmb_Box_Seperator.SelectedItem)

        Dim lineCount = CoincidentPoints.GetLineCount(txtBox_RawPointCloud.Text)
        'TODO Edit coincident points function so precision is an input parameter
        If lineCount * 2 > 25000000 Then
            MsgBox("This file is tool large too process in this version of the coincident points tool.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Me.Close()
            Exit Sub
            'Dim tempDir As String = WindowsManagement.CreateTemporaryDirectory("CoincidentPoints")
            'Dim sortedPointCloud As String = CoincidentPoints.UseSortShellCommand(tempDir, txtBox_RawPointCloud.Text)
            'If String.IsNullOrEmpty(sortedPointCloud) Then

            '    MsgBox("Unable to sort large point cloud, which results in faster processing times." & vbCrLf & vbCrLf &
            '           "Reverting to different method. Please be patient. This could take up to 2 hours to process.", MsgBoxStyle.OkOnly, "Unable to Create Sorted Point Cloud")
            '    CoincidentPoints.findCoincidentPointsLargeFile(txtBox_RawPointCloud.Text,
            '                                                   txtBox_OutputShp.Text,
            '                                                   seperator,
            '                                                   System.Int32.Parse(NumericUpDown_Precision.Value.ToString),
            '                                                   tempDir,
            '                                                   ChkBox_SetPrecision.Checked)

            'ElseIf Not String.IsNullOrEmpty(sortedPointCloud) Then
            '    'Subset the large file into 100 MB chunks to be more easily processed
            '    Try
            '        CoincidentPoints.SubsetFileByChunks(tempDir, sortedPointCloud)
            '    Catch ex As System.OutOfMemoryException
            '        MsgBox("An error occured due to system memory: " & vbCrLf & ex.Message & vbCrLf & vbCrLf &
            '               "Reverting to different method. Please be patient. This could take up to 2 hours to process.", MsgBoxStyle.OkOnly, "Unable to Subset Sorted Point Cloud")
            '        CoincidentPoints.findCoincidentPointsLargeFile(tempDir & "\" & System.IO.Path.GetFileNameWithoutExtension(txtBox_RawPointCloud.Text) & "_Sorted.txt",
            '                                                       txtBox_OutputShp.Text,
            '                                                       seperator,
            '                                                       System.Int32.Parse(NumericUpDown_Precision.Value.ToString),
            '                                                       tempDir,
            '                                                       ChkBox_SetPrecision.Checked)

            '        CoincidentPoints.CreateCoincidentPointsShapefile(System.IO.Path.GetDirectoryName(txtBox_OutputShp.Text) & "\" & System.IO.Path.GetFileNameWithoutExtension(txtBox_OutputShp.Text) & "_CoincidentPoints.txt",
            '                                                                                      txtBox_SpatialReference.Text)
            '        Me.Close()
            '        Exit Sub
            '    End Try


            '    'Loop over subseted sorted point clouds and find coincident points
            '    Dim allSortedFiles = System.IO.Directory.GetFiles(tempDir, "*_Sorted_Sub*.txt")
            '    System.Threading.Tasks.Parallel.ForEach(allSortedFiles, Sub(currentFile)
            '                                                                Dim fileName As String = System.IO.Path.GetFileName(currentFile)
            '                                                                Dim outPath As String = tempDir & "\" & System.IO.Path.GetFileNameWithoutExtension(currentFile) & "_Coincident.txt"
            '                                                                CoincidentPoints.findCoincidentPointsSmallFile(currentFile,
            '                                                                                                               outPath,
            '                                                                                                               seperator,
            '                                                                                                               System.Int32.Parse(NumericUpDown_Precision.Value.ToString),
            '                                                                                                                ChkBox_SetPrecision.Checked,
            '                                                                                                               True)
            '                                                            End Sub)


            '    'Merge all of the coincident point files in temporary directory and merge them into one final file
            '    Dim allCoincidentFiles = System.IO.Directory.GetFiles(tempDir, "*_Coincident.txt")
            '    Dim outputFinalCoincidentPath As String = System.IO.Path.GetDirectoryName(txtBox_OutputShp.Text) & "\" & System.IO.Path.GetFileNameWithoutExtension(txtBox_OutputShp.Text) & ".txt"
            '    Dim streamWriter As New IO.StreamWriter(outputFinalCoincidentPath)
            '    streamWriter.WriteLine("X,Y,Uncert")
            '    streamWriter.Close()
            '    streamWriter = Nothing
            '    For Each file As String In allCoincidentFiles
            '        System.IO.File.AppendAllText(outputFinalCoincidentPath, System.IO.File.ReadAllText(file))
            '    Next file

            '    'Delete temporary directory created to house the intermediate data
            '    System.IO.Directory.Delete(tempDir, True)

            '    'Implement spatial filter
            '    'If Not IsNothing(m_ExtentShp_gxDialog.InternalCatalog.SelectedObject) Then
            '    '    Dim isPointInPolygon As ESRI.ArcGIS.Geometry.IRelationalOperator2 = GIS.CreateSpatialOperator(m_ExtentShp_gxDialog.InternalCatalog.SelectedObject.FullName)


            '    '    Dim outputTextPath As String = System.IO.Path.GetDirectoryName(m_outShp_FileDialog.FileName) & "\" & System.IO.Path.GetFileNameWithoutExtension(m_outShp_FileDialog.FileName) & ".txt"
            '    '    Using fileReader As New StreamReader(outputFinalCoincidentPath)
            '    '        fileReader.ReadLine()
            '    '        Dim builder As New System.Text.StringBuilder

            '    '        Using fileWriter As New System.IO.StreamWriter(System.IO.Path.GetDirectoryName(outputFinalCoincidentPath) & "\" & System.IO.Path.GetFileNameWithoutExtension(outputFinalCoincidentPath) & "_Filtered.txt", False)
            '    '            fileWriter.WriteLine("X,Y,Uncert")
            '    '            Do While (fileReader.Peek() > -1)

            '    '                Dim newLine As String = fileReader.ReadLine.Replace(vbCr, "").Replace(vbLf, "")

            '    '                If String.Compare(newLine, "") = 0 Then
            '    '                    Continue Do
            '    '                End If
            '    '                Dim X As Double = System.Double.Parse(newLine.Split(",")(0))
            '    '                Dim Y As Double = System.Double.Parse(newLine.Split(",")(1))

            '    '                Dim inPoint As ESRI.ArcGIS.Geometry.IPoint = New ESRI.ArcGIS.Geometry.Point
            '    '                inPoint.PutCoords(X, Y)
            '    '                If Not isPointInPolygon.Contains(inPoint) Then
            '    '                    Continue Do
            '    '                Else
            '    '                    Dim outLine As String = newLine.Split(",")(0) & "," & newLine.Split(",")(1) & "," & newLine.Split(",")(2)
            '    '                    builder.Append(outLine & "\n")
            '    '                End If
            '    '            Loop
            '    '            Dim textBlock = builder.ToString()
            '    '            fileWriter.Write(textBlock)
            '    '            textBlock = Nothing
            '    '            builder.Clear()
            '    '            builder = Nothing
            '    '        End Using
            '    '    End Using
            '    'End If



            '    'Create shapefile
            '    Dim shapefileCreationValidator = CoincidentPoints.CreateCoincidentPointsShapefile(outputFinalCoincidentPath,
            '                                                                                      txtBox_SpatialReference.Text)

            '    If Not IsNothing(m_ExtentShp_gxDialog.InternalCatalog.SelectedObject) Then

            '        Dim clipParameters As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            '        clipParameters.Add(txtBox_OutputShp.Text)
            '        clipParameters.Add(m_ExtentShp_gxDialog.InternalCatalog.SelectedObject.FullName)
            '        clipParameters.Add(System.IO.Path.GetDirectoryName(txtBox_OutputShp.Text) & "\" &
            '                                                           System.IO.Path.GetFileNameWithoutExtension(txtBox_OutputShp.Text) & "_Filtered.shp")
            '        Dim geoProcessorEngine As ESRI.ArcGIS.Geoprocessing.GeoProcessor = New ESRI.ArcGIS.Geoprocessing.GeoProcessor()
            '        Dim trackCancel As ESRI.ArcGIS.esriSystem.ITrackCancel = Nothing
            '        TopCAT.MyGeoprocessing.RunToolGeoprocessingTool("Clip_analysis", geoProcessorEngine, clipParameters)

            '    End If

            '    If shapefileCreationValidator = True Then
            '        MsgBox("Shapefile successfully created." & vbCrLf & vbCrLf & "It is located at: " & txtBox_OutputShp.Text, MsgBoxStyle.OkOnly, "Shapefile Successfully Created")
            '    ElseIf shapefileCreationValidator = False Then
            '        MsgBox("Unable to create shapefile.", MsgBoxStyle.OkOnly, "Shapefile Not Created")
            '    End If
            'End If

        Else
            CoincidentPoints.findCoincidentPointsSmallFile(txtBox_RawPointCloud.Text,
                                                           txtBox_OutputShp.Text,
                                                           seperator,
                                                           System.Int32.Parse(NumericUpDown_Precision.Value.ToString),
                                                           ChkBox_SetPrecision.Checked,
                                                           False)

            Dim outputTextPath As String = System.IO.Path.GetDirectoryName(txtBox_OutputShp.Text) & "\" & System.IO.Path.GetFileNameWithoutExtension(txtBox_OutputShp.Text) & ".txt"
            'Implement spatial filter
            'If Not IsNothing(m_ExtentShp_gxDialog.InternalCatalog.SelectedObject) Then
            '    Dim isPointInPolygon As ESRI.ArcGIS.Geometry.IRelationalOperator2 = GIS.CreateSpatialOperator(m_ExtentShp_gxDialog.InternalCatalog.SelectedObject.FullName)


            '    Using fileReader As New StreamReader(outputTextPath)
            '        fileReader.ReadLine()
            '        Dim builder As New System.Text.StringBuilder

            '        Using fileWriter As New System.IO.StreamWriter(System.IO.Path.GetDirectoryName(outputTextPath) & "\" & System.IO.Path.GetFileNameWithoutExtension(outputTextPath) & "_Filtered.txt", False)
            '            fileWriter.WriteLine("X,Y,Uncert")
            '            Do While (fileReader.Peek() > -1)

            '                Dim newLine As String = fileReader.ReadLine.Replace(vbCr, "").Replace(vbLf, "")

            '                If String.Compare(newLine, "") = 0 Then
            '                    Continue Do
            '                End If

            '                Dim X As Double = System.Double.Parse(newLine.Split(",")(0))
            '                Dim Y As Double = System.Double.Parse(newLine.Split(",")(1))
            '                Dim inPoint As ESRI.ArcGIS.Geometry.IPoint = New ESRI.ArcGIS.Geometry.Point
            '                inPoint.PutCoords(X, Y)
            '                If Not isPointInPolygon.Contains(inPoint) Then
            '                    Continue Do
            '                Else
            '                    Dim outLine As String = newLine.Split(",")(0) & "," & newLine.Split(",")(1) & "," & newLine.Split(",")(2)
            '                    builder.Append(outLine & "\n")
            '                End If
            '            Loop
            '            Dim textBlock = builder.ToString()
            '            fileWriter.Write(textBlock)
            '            textBlock = Nothing
            '            builder.Clear()
            '            builder = Nothing
            '        End Using
            '    End Using
            'End If


            'Create shapefile
            Dim shapefileCreationValidator = CoincidentPoints.CreateCoincidentPointsShapefile(outputTextPath,
                                                                                              txtBox_SpatialReference.Text)

            'If Not IsNothing(m_ExtentShp_gxDialog.InternalCatalog.SelectedObject) Then

            '    Dim clipParameters As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            '    clipParameters.Add(txtBox_OutputShp.Text)
            '    clipParameters.Add(m_ExtentShp_gxDialog.InternalCatalog.SelectedObject.FullName)
            '    clipParameters.Add(System.IO.Path.GetDirectoryName(txtBox_OutputShp.Text) & "\" &
            '                                                                System.IO.Path.GetFileNameWithoutExtension(txtBox_OutputShp.Text) & "_Filtered.shp")
            '    Dim geoProcessorEngine As ESRI.ArcGIS.Geoprocessing.GeoProcessor = New ESRI.ArcGIS.Geoprocessing.GeoProcessor()
            '    Dim trackCancel As ESRI.ArcGIS.esriSystem.ITrackCancel = Nothing
            '    TopCAT.MyGeoprocessing.RunToolGeoprocessingTool("Clip_analysis", geoProcessorEngine, clipParameters)

            'End If

            If shapefileCreationValidator = True Then
                'MsgBox("Shapefile successfully created." & vbCrLf & vbCrLf & "It is located at: " & txtBox_OutputShp.Text, MsgBoxStyle.OkOnly, "Shapefile Successfully Created")
                If My.Settings.AddOutputLayersToMap Then
                    Try
                        TopCAT.GIS.AddShapefile(m_pArcMap, txtBox_OutputShp.Text)
                    Catch ex As Exception
                        'Do Nothing
                    End Try
                End If
            ElseIf shapefileCreationValidator = False Then
                MsgBox("Unable to create shapefile.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            End If
        End If

        Me.Close()

    End Sub

    Private Sub btn_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click

        Me.Close()

    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click

        System.Diagnostics.Process.Start(My.Resources.HelpBaseURL & "gcd-command-reference/gcd-analysis-menu/a-uncertainty-analysis-submenu/b-point-cloud-based/i-coincident-points-tool")

    End Sub

    Private Function ValidateForm() As Boolean

        If String.IsNullOrEmpty(txtBox_RawPointCloud.Text) Then
                MsgBox("Please select a point cloud to process.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Return False
        End If
        If (Cmb_Box_Seperator.SelectedIndex = -1) Then
            MsgBox("Please select a separator symbol for this file. For help doing so click the preview first lines button.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If String.IsNullOrEmpty(txtBox_OutputShp.Text) Then
            MsgBox("Please provide a file path to give to the output shapefile.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        Return True
    End Function

    Private Sub frm_CoincidentPtsShp_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ttip.SetToolTip(btn_RawPointCloud, "Press this button to open a file dialog and select a raw point cloud file.")
        ttip.SetToolTip(txtBox_RawPointCloud, "Displays the file name of the point cloud. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(Cmb_Box_Seperator, "Choose the column separator of the raw point cloud input file.")
        ttip.SetToolTip(btn_SpatialReference, "Press this button to open a file dialog and select a file (.prj or .shp) containing spatial reference information.")
        ttip.SetToolTip(txtBox_SpatialReference, "Displays the file name of the spatial reference. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(btn_OutputShp, "Press this button to open a file dialog to name and save the output coincident points shapefile to.")
        ttip.SetToolTip(txtBox_OutputShp, "Displays the file name of the output shapefile. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(btn_Run, "Click to run the analysis")
        ttip.SetToolTip(btnHelp, "Click to go to the tool documentation.")
        ttip.SetToolTip(btn_Cancel, "Cancel analysis and exit the tool window.")
    End Sub

End Class