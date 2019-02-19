Imports System.Windows.Forms
Imports Microsoft.Win32
Imports System.IO

Public Class frm_RawPointCloudToShp

    Public Sub New(ByRef pApp As ESRI.ArcGIS.Framework.IApplication)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        m_pArcMap = pApp

    End Sub

    Private m_pArcMap As ESRI.ArcGIS.Framework.IApplication

    Private Sub btn_OutputShp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OutputShp.Click

        'TopCAT.WindowsFormAssistant.SaveFileDialog("Shapefile", txtBox_OutputShp)
        Dim sOutputShpPath As String = GISDataStructures.VectorDataSource.BrowseSave("Save Point Vector Data", Nothing, Nothing, 0)
        If Not String.IsNullOrEmpty(sOutputShpPath) Then
            txtBox_OutputShp.Text = sOutputShpPath
        End If

    End Sub

    Private Sub txtBox_OutputShp_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_OutputShp.TextChanged

    End Sub

    Private Sub btnPreviewFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPreviewFile.Click

        If Not String.IsNullOrEmpty(txtBox_RawPointCloud.Text) Then
            If IO.File.Exists(txtBox_RawPointCloud.Text) Then
                TopCAT.ToPCAT_Assistant.PreviewFirstLine(txtBox_RawPointCloud.Text)
            End If
        End If

    End Sub

    Private Sub btn_RawPointCloud_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_RawPointCloud.Click

        TopCAT.WindowsFormAssistant.OpenFileDialog("Raw Point Cloud", txtBox_RawPointCloud)

    End Sub

    Private Sub btn_SpatialReference_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_SpatialReference.Click

        TopCAT.WindowsFormAssistant.OpenFileDialog("Spatial Reference", txtBox_SpatialReference)

        Dim shapefileProjectedCheck As Boolean = TopCAT.WindowsFormAssistant.CheckIfShapefileHasPrjFile(txtBox_SpatialReference.Text)
        If shapefileProjectedCheck = False Then
            txtBox_SpatialReference.Text = String.Empty
            MsgBox("The shapefile selected is not projected. Please select a different .shp file or .prj file", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If

    End Sub

    'Private Sub chk_ChangeSeparator_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_ChangeSeparator.CheckedChanged

    '    If chk_ChangeSeparator.Checked = True Then
    '        cmbBox_SelectSeparator.Enabled = True
    '    ElseIf chk_ChangeSeparator.Checked = False Then
    '        cmbBox_SelectSeparator.SelectedItem = cmbBox_SelectSeparator.Items.Item(0)
    '        cmbBox_SelectSeparator.Enabled = False
    '    End If

    'End Sub

    Private Sub cmbBox_SelectSeparator_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbBox_SelectSeparator.SelectedIndexChanged

        cmbBox_SelectSeparator.SelectedItem = cmbBox_SelectSeparator.SelectedItem

    End Sub

    Private Sub txtBox_SpatialReference_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_SpatialReference.TextChanged

    End Sub

    Private Sub btn_Run_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Run.Click

        If Not ValidateForm() Then
            Exit Sub
        End If

        Dim joinPath = {IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location), My.Resources.ScriptDirectory, "rawPointCloudToShp.py"}
        Dim pythonFile As String = [String].Join("\", joinPath)

        If Not IO.File.Exists(pythonFile) Then
            MsgBox("The python script used to run this tool is missing from your GCD installation folder.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If

        'CREATE THE BATCH FILE IN THE %TEMP% FOLDER OF THE USER
        Dim sBatchFilePath As String = Environ("TEMP")
        sBatchFilePath = IO.Path.Combine(sBatchFilePath, "runRawPointCloudToShp.bat")

        'CREATE BATCH TEMPLATE
        Dim batchTemplate As String
        Dim optionsTemplate As String = Nothing
        'TO DO COLLECT THE PARAMETERS ENTERED IN THE ADVANCED DETRENDING MENU

        'TODO CODE IN WHERE PYTHON ARCGIS IS LIVING
        Dim esriPythonPath As String = WindowsManagement.getESRI_PythonPath()

        batchTemplate = esriPythonPath & " " &
                        Chr(34) & pythonFile & Chr(34) & " " &
                        Chr(34) & txtBox_OutputShp.Text & Chr(34) & " " &
                        Chr(34) & txtBox_RawPointCloud.Text & Chr(34) & " " &
                        "--Separator " & cmbBox_SelectSeparator.SelectedItem.ToString & " " &
                        "--SpatialReference " & Chr(34) & txtBox_SpatialReference.Text & Chr(34) & vbCrLf &
                        "@pause"

        'batchTemplate = [String].Join(" ", batchTemplate, optionsTemplate)

        'batchTemplate = [String].Join(vbCrLf, batchTemplate, "@pause")

        'MsgBox("This is the batch file: " & vbCrLf & batchTemplate)
        Dim batchWriter As New System.IO.StreamWriter(sBatchFilePath, False)
        batchWriter.WriteLine(batchTemplate)
        batchWriter.Close()


        'TO DO WRITE BATCH FILE WITH ATTENTION TO THE OPTIONS FROM ADVANCED DETRENDING MENU

        'RUN THE BATCH FILE
        Shell(sBatchFilePath, AppWinStyle.NormalFocus, True)
        'MsgBox("Your file: " & m_filePath & " was successfully decimated." & vbCrLf &
        '      "Decimation resolution x: " & xRes & vbCrLf &
        '      "Decimation resolution y: " & yRes & vbCrLf &
        '      "Minimum points used to calculate statistics: " & nMin & vbCrLf & vbCrLf &
        '      "The detrended standard deviation for each sample window was calculated as " & stdevDetrendedOption.Value.ToString &
        '      " standard deviations from the " & m_DetrendedOptionString & "." & vbCrLf & vbCrLf, MsgBoxStyle.OkOnly, "Decimation Complete")

        If My.Settings.AddOutputLayersToMap Then
            Try
                TopCAT.GIS.AddShapefile(m_pArcMap, txtBox_OutputShp.Text)
            Catch ex As Exception
                'Do nothing
            End Try
        End If

        Me.Close()


    End Sub


    Private Sub btn_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click

        Me.Close()

    End Sub


    Private Sub btn_Help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Help.Click

        System.Diagnostics.Process.Start(My.Resources.HelpBaseURL & "gcd-command-reference/data-prep-menu/c-survey-preparation-menu/i-create-point-feature-class")

    End Sub

    Private Function ValidateForm() As Boolean

        If String.IsNullOrEmpty(txtBox_OutputShp.Text) Then
            MsgBox("Please provide a file path to give to the output shapefile.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If String.IsNullOrEmpty(txtBox_RawPointCloud.Text) Then
            MsgBox("Please provide a raw point cloud as input.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If (cmbBox_SelectSeparator.SelectedIndex = -1) Then
            MsgBox("Please select a separator symbol for this file. For help doing so click the preview first lines button.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        Return True
    End Function

    Private Sub frm_RawPointCloudToShp_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ttip.SetToolTip(btn_RawPointCloud, "Press this button to open a file dialog and select a raw point cloud file.")
        ttip.SetToolTip(txtBox_RawPointCloud, "Displays the file name of the point cloud. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(cmbBox_SelectSeparator, "Choose the column separator of the raw point cloud input file.")
        ttip.SetToolTip(btn_SpatialReference, "Press this button to open a file dialog and select a file (.prj or .shp) containing spatial reference information.")
        ttip.SetToolTip(txtBox_SpatialReference, "Displays the file name of the spatial reference. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(btn_OutputShp, "Press this button to open a file dialog to name and save the output ppint cloud shapefile to.")
        ttip.SetToolTip(txtBox_OutputShp, "Displays the file name of the output shapefile. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(btn_Run, "Click to run the analysis")
        ttip.SetToolTip(btn_Help, "Click to go to the tool documentation.")
        ttip.SetToolTip(btn_Cancel, "Cancel analysis and exit the tool window.")
    End Sub
End Class
