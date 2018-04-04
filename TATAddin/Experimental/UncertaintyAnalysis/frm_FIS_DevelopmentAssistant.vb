Public Class frm_FIS_DevelopmentAssistant

    Dim m_Parameter1Raster_gxDialog As New ESRI.ArcGIS.CatalogUI.GxDialog
    Dim m_Parameter2Raster_gxDialog As New ESRI.ArcGIS.CatalogUI.GxDialog
    Dim m_OutputFolderDialog As New System.Windows.Forms.FolderBrowserDialog

    Private Sub btn_CoincidentPointsFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_CoincidentPointsFile.Click

        TopCAT.WindowsFormAssistant.OpenFileDialog("Coincident Points File", txtBox_CoincidentPointFile_FileDialog)

    End Sub

    Private Sub btn_Parameter1Raster_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Parameter1Raster.Click

        Dim sRaster1Path = TopCAT.WindowsFormAssistant.OpenGxFileDialog("Raster", "Select a Raster", 0)
        txtBox_Parameter1Raster.Text = sRaster1Path

    End Sub

    Private Sub btn_Parameter2Raster_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Parameter2Raster.Click


        Dim sRaster2Path = TopCAT.WindowsFormAssistant.OpenGxFileDialog("Raster", "Select a Raster", 0)
        txtBox_Parameter2Raster.Text = sRaster2Path

    End Sub

    Private Sub btn_OutputFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OutputFolder.Click

        m_OutputFolderDialog.Description = "Select a path to contain the output from ToPCAT"
        m_OutputFolderDialog.ShowNewFolderButton = True
        m_OutputFolderDialog.ShowDialog()

        txtBox_OutputFileDialog.Text = m_OutputFolderDialog.SelectedPath

    End Sub

    Private Sub btn_Run_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Run.Click

        If Not ValidateForm() Then
            Exit Sub
        End If

        'Dim rasterWorkspace1 = TopCAT.GIS.OpenRasterWorkspace(System.IO.Path.GetDirectoryName(m_Parameter1Raster_gxDialog.InternalCatalog.SelectedObject.FullName))
        Dim rasterWorkspace1 = TopCAT.GIS.OpenRasterWorkspace(System.IO.Path.GetDirectoryName(txtBox_Parameter1Raster.Text))
        'Dim raster1 = TopCAT.GIS.OpenRasterDataset(rasterWorkspace1, m_Parameter1Raster_gxDialog.InternalCatalog.SelectedObject.Name)
        Dim raster1 = TopCAT.GIS.OpenRasterDataset(rasterWorkspace1, IO.Path.GetFileName(txtBox_Parameter1Raster.Text))
        'Dim raster1Stats = GIS.ReportRasterStats(raster1)

        'Dim rasterWorkspace2 = TopCAT.GIS.OpenRasterWorkspace(System.IO.Path.GetDirectoryName(m_Parameter2Raster_gxDialog.InternalCatalog.SelectedObject.FullName))
        Dim rasterWorkspace2 = TopCAT.GIS.OpenRasterWorkspace(System.IO.Path.GetDirectoryName(txtBox_Parameter2Raster.Text))
        'Dim raster2 = TopCAT.GIS.OpenRasterDataset(rasterWorkspace2, m_Parameter2Raster_gxDialog.InternalCatalog.SelectedObject.Name)
        Dim raster2 = TopCAT.GIS.OpenRasterDataset(rasterWorkspace2, IO.Path.GetFileName(txtBox_Parameter2Raster.Text))
        'Dim raster2Stats = GIS.ReportRasterStats(raster2)

        'Create a folder to house the results
        Dim resultsDir As String = IO.Path.Combine(m_OutputFolderDialog.SelectedPath, "FIS_DevelopmentResults")
        System.IO.Directory.CreateDirectory(resultsDir)

        Dim outputPathCoincidentPointsWithRasterValues As String = System.IO.Path.Combine(resultsDir,
                                                                                          System.IO.Path.GetFileNameWithoutExtension(txtBox_CoincidentPointFile_FileDialog.Text)) & "_WithRasterValues.txt"

        Dim rasVal1Temp As New List(Of Single)
        Dim rasVal2Temp As New List(Of Single)
        Dim xValTemp As New List(Of Double)
        Dim yValTemp As New List(Of Double)
        Dim uncertValTemp As New List(Of Single)

        Using streamReader = New System.IO.StreamReader(txtBox_CoincidentPointFile_FileDialog.Text)
            streamReader.ReadLine()
            Do Until streamReader.EndOfStream
                Dim newLine As String = streamReader.ReadLine()
                Dim X As Double = Double.Parse(newLine.Split(",")(0))
                Dim Y As Double = Double.Parse(newLine.Split(",")(1))
                xValTemp.Add(X)
                yValTemp.Add(Y)
                uncertValTemp.Add(Single.Parse(newLine.Split(",")(2)))
                rasVal1Temp.Add(Single.Parse(TopCAT.GIS.ExtractRasterValuesAtXY_ToTextFile(raster1, X, Y)))
                rasVal2Temp.Add(Single.Parse(TopCAT.GIS.ExtractRasterValuesAtXY_ToTextFile(raster2, X, Y)))
                'Dim Uncert As Double = Double.Parse(newLine.Split(",")(2))

                'Dim rasVal1 = GIS.ExtractRasterValuesAtXY_ToTextFile(raster1, X, Y)
                'Dim rasVal2 = GIS.ExtractRasterValuesAtXY_ToTextFile(raster2, X, Y)
                'If rasVal1 > 0 And rasVal2 > 0 Then
                '    'fileWriter.WriteLine(X & "," & Y & "," & Uncert & "," & rasVal1 & "," & rasVal2)
                'End If

            Loop
        End Using


        Dim medianParam1 = TopCAT.Stats.GetMedian(rasVal1Temp)
        Dim medianParam2 = TopCAT.Stats.GetMedian(rasVal2Temp)
        Dim medianUncert = TopCAT.Stats.GetMedian(uncertValTemp)
        Dim standardDeviationParam1 = TopCAT.Stats.GetStandardDeviation(rasVal1Temp)
        Dim standardDeviationParam2 = TopCAT.Stats.GetStandardDeviation(rasVal2Temp)
        Dim standardDeviationUncert = TopCAT.Stats.GetStandardDeviation(uncertValTemp)
        Dim classifiedRasterValues1 = TopCAT.Stats.ClassifyRasterValues(rasVal1Temp, medianParam1, standardDeviationParam1)
        Dim classifiedRasterValues2 = TopCAT.Stats.ClassifyRasterValues(rasVal2Temp, medianParam2, standardDeviationParam2)
        Dim classifiedUncertValues = TopCAT.Stats.ClassifyRasterValues(uncertValTemp, medianUncert, standardDeviationUncert)

        Using fileWriter = New System.IO.StreamWriter(outputPathCoincidentPointsWithRasterValues)
            fileWriter.WriteLine("X,Y,Uncert,UncertClass," & cmbBox_Parameter1ColumnAlias.SelectedItem.ToString() & ",Param1Class," & cmbBox_Parameter2ColumnAlias.SelectedItem.ToString() & ",Param2Class")
            For i As Integer = 0 To xValTemp.Count - 1
                If rasVal1Temp.Item(i) > 0 And rasVal2Temp.Item(i) > 0 Then
                    fileWriter.WriteLine(xValTemp.Item(i) & "," & yValTemp.Item(i) & "," & uncertValTemp.Item(i) & "," & classifiedUncertValues.Item(i) & "," & rasVal1Temp.Item(i) & "," & classifiedRasterValues1.Item(i) & "," & rasVal2Temp.Item(i) & "," & classifiedRasterValues2.Item(i))
                End If

            Next
        End Using


        'Using fileWriter = New System.IO.StreamWriter(outputPathCoincidentPointsWithRasterValues)
        '    fileWriter.WriteLine("X,Y,Uncert," & cmbBox_Parameter1ColumnAlias.SelectedItem.ToString() & "," & cmbBox_Parameter2ColumnAlias.SelectedItem.ToString())
        '    Using streamReader = New System.IO.StreamReader(txtBox_CoincidentPointFile_FileDialog.Text)
        '        streamReader.ReadLine()
        '        Do Until streamReader.EndOfStream
        '            Dim newLine As String = streamReader.ReadLine()
        '            Dim X As Double = Double.Parse(newLine.Split(",")(0))
        '            Dim Y As Double = Double.Parse(newLine.Split(",")(1))
        '            Dim Uncert As Double = Double.Parse(newLine.Split(",")(2))

        '            Dim rasVal1 = TopCAT.GIS.ExtractRasterValuesAtXY_ToTextFile(raster1, X, Y)
        '            Dim rasVal2 = TopCAT.GIS.ExtractRasterValuesAtXY_ToTextFile(raster2, X, Y)
        '            If rasVal1 > 0 And rasVal2 > 0 Then
        '                fileWriter.WriteLine(X & "," & Y & "," & Uncert & "," & rasVal1 & "," & rasVal2)
        '            End If
        '        Loop
        '    End Using
        'End Using

        Dim rPath As String = WindowsManagement.getR_Path()
        Dim rBasePath As String = System.IO.Path.GetDirectoryName(rPath)
        Dim rBatFile As String

        Dim rFile = IO.Path.Combine(IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location),
                                    "Resources\RScripts\rRoughDraftDevelopFIS_BoostedWithVB_CommandLineVersion0.1.txt")

        If Not IO.File.Exists(rFile) Then
            MsgBox("The R script used to run this tool is missing from your GCD installation folder.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If

        rBatFile = "cd " & rBasePath & vbCrLf &
                   "RScript " & Chr(34) & rFile & Chr(34) & " --args " &
                   Chr(34) & outputPathCoincidentPointsWithRasterValues & Chr(34) & " " &
                   Chr(34) & resultsDir & Chr(34) & " " &
                   Chr(34) & cmbBox_Parameter1ColumnAlias.SelectedItem & Chr(34) & " " &
                   Chr(34) & cmbBox_Parameter2ColumnAlias.SelectedItem & Chr(34) & " " & vbCrLf &
                   "@pause"


        'WRITE THE BATCH FILE   
        Dim rBatFilePath As String = Environ("TEMP")
        rBatFilePath = IO.Path.Combine(rBatFilePath, "runExplorationofParametersInfluenceOnUncertainty.bat")
        'MsgBox("This is the batch file: " & vbCrLf & batchTemplateOutShp)
        Dim batFileWriter2 As New System.IO.StreamWriter(rBatFilePath, False)
        batFileWriter2.WriteLine(rBatFile)
        batFileWriter2.Close()

        'RUN THE BATCH FILE
        Shell(rBatFilePath, AppWinStyle.NormalFocus, True)

        Me.Close()

    End Sub

    Private Sub btn_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click
        Me.Close()
    End Sub

    Private Function ValidateForm() As Boolean

        If String.IsNullOrEmpty(txtBox_CoincidentPointFile_FileDialog.Text) Then
            MsgBox("Please select a coincident point file to process.", MsgBoxStyle.OkOnly, "No Coincident Points File Selected")
            Return False
        ElseIf String.Compare(IO.Path.GetExtension(txtBox_CoincidentPointFile_FileDialog.Text), ".txt") <> 0 Then
            MsgBox("The input for coincident points file needs to be the text version of the coincident points file.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        ElseIf String.IsNullOrEmpty(m_OutputFolderDialog.SelectedPath) Then
            MsgBox("Please select a folder to save the output to.", MsgBoxStyle.OkOnly, "No Folder Selected to Save Output")
            Return False
        ElseIf String.IsNullOrEmpty(txtBox_Parameter1Raster.Text) Then
        MsgBox("No raster selected for parameter 1." & vbCrLf & vbCrLf & "Please select a raster from the parameter 1 raster dialog.", MsgBoxStyle.OkOnly, "No raster selected")
        Return False
        ElseIf String.IsNullOrEmpty(txtBox_Parameter2Raster.Text) Then
        MsgBox("No raster selected for parameter 2." & vbCrLf & vbCrLf & "Please select a raster from the parameter 2 raster dialog.", MsgBoxStyle.OkOnly, "No raster selected")
            Return False
            'ElseIf Not String.Compare(m_Parameter1Raster_gxDialog.InternalCatalog.SelectedObject.Category, "Raster Dataset") = 0 Then
            '    MsgBox("No raster selected for parameter 1." & vbCrLf & vbCrLf & "Please select a raster from the parameter 1 raster dialog.", MsgBoxStyle.OkOnly, "No raster selected")
            '    Return False
            'ElseIf Not String.Compare(m_Parameter2Raster_gxDialog.InternalCatalog.SelectedObject.Category, "Raster Dataset") = 0 Then
            '    MsgBox("No raster selected for parameter 2." & vbCrLf & vbCrLf & "Please select a raster from the parameter 2 raster dialog.", MsgBoxStyle.OkOnly, "No raster selected")
            '    Return False
            'ElseIf String.Compare(m_Parameter1Raster_gxDialog.InternalCatalog.SelectedObject.FullName, m_Parameter2Raster_gxDialog.InternalCatalog.SelectedObject.FullName) = 0 Then
            '    MsgBox("Parameter 1 raster cannot be the same as Parameter 2 raster", MsgBoxStyle.OkOnly, "Raster Selection Error")
            '    Return False
        ElseIf String.IsNullOrEmpty(cmbBox_Parameter1ColumnAlias.SelectedItem) Then
        MsgBox("Please select an alias for column 1. This will be used in naming figures and axes produced from this tool.", MsgBoxStyle.OkOnly, "No Column Alias Selected")
        Return False
        ElseIf String.IsNullOrEmpty(cmbBox_Parameter2ColumnAlias.SelectedItem) Then
        MsgBox("Please select an alias for column 2. This will be used in naming figures and axes produced from this tool.", MsgBoxStyle.OkOnly, "No Column Alias Selected")
        Return False
        End If

        Return True
    End Function

    Private Sub frm_FIS_DevelopmentAssistant_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            Dim rPath As String = WindowsManagement.getR_Path()
            If String.IsNullOrEmpty(rPath) Then
                Throw New Exception("cannot find R")
            End If
        Catch ex As Exception
            MsgBox("This tool requires R statistical software to run. Please download R to run this tool.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Me.Close()
        End Try
    End Sub
End Class