<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_FIS_DevelopmentAssistant
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frm_FIS_DevelopmentAssistant))
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.btn_Cancel = New System.Windows.Forms.Button()
        Me.btn_Run = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.btn_OutputFolder = New System.Windows.Forms.Button()
        Me.txtBox_OutputFileDialog = New System.Windows.Forms.TextBox()
        Me.txtBox_CoincidentPointFile_FileDialog = New System.Windows.Forms.TextBox()
        Me.btn_CoincidentPointsFile = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btn_Parameter2Raster = New System.Windows.Forms.Button()
        Me.btn_Parameter1Raster = New System.Windows.Forms.Button()
        Me.txtBox_Parameter2Raster = New System.Windows.Forms.TextBox()
        Me.txtBox_Parameter1Raster = New System.Windows.Forms.TextBox()
        Me.cmbBox_Parameter2ColumnAlias = New System.Windows.Forms.ComboBox()
        Me.cmbBox_Parameter1ColumnAlias = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnHelp
        '
        Me.btnHelp.Image = My.Resources.Resources.Help
        Me.btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnHelp.Location = New System.Drawing.Point(29, 287)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(58, 23)
        Me.btnHelp.TabIndex = 66
        Me.btnHelp.Text = "Help"
        Me.btnHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'btn_Cancel
        '
        Me.btn_Cancel.Location = New System.Drawing.Point(560, 287)
        Me.btn_Cancel.Name = "btn_Cancel"
        Me.btn_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.btn_Cancel.TabIndex = 65
        Me.btn_Cancel.Text = "Cancel"
        Me.btn_Cancel.UseVisualStyleBackColor = True
        '
        'btn_Run
        '
        Me.btn_Run.Location = New System.Drawing.Point(463, 287)
        Me.btn_Run.Name = "btn_Run"
        Me.btn_Run.Size = New System.Drawing.Size(75, 23)
        Me.btn_Run.TabIndex = 64
        Me.btn_Run.Text = "Run Tool"
        Me.btn_Run.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.btn_OutputFolder)
        Me.GroupBox2.Controls.Add(Me.txtBox_OutputFileDialog)
        Me.GroupBox2.Location = New System.Drawing.Point(29, 199)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(606, 70)
        Me.GroupBox2.TabIndex = 63
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Output"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 29)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(74, 13)
        Me.Label5.TabIndex = 51
        Me.Label5.Text = "Output Folder:"
        '
        'btn_OutputFolder
        '
        Me.btn_OutputFolder.Image = My.Resources.Resources.SaveGIS
        Me.btn_OutputFolder.Location = New System.Drawing.Point(545, 24)
        Me.btn_OutputFolder.Name = "btn_OutputFolder"
        Me.btn_OutputFolder.Size = New System.Drawing.Size(29, 23)
        Me.btn_OutputFolder.TabIndex = 47
        Me.btn_OutputFolder.UseVisualStyleBackColor = True
        '
        'txtBox_OutputFileDialog
        '
        Me.txtBox_OutputFileDialog.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_OutputFileDialog.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_OutputFileDialog.Location = New System.Drawing.Point(86, 26)
        Me.txtBox_OutputFileDialog.Name = "txtBox_OutputFileDialog"
        Me.txtBox_OutputFileDialog.ReadOnly = True
        Me.txtBox_OutputFileDialog.Size = New System.Drawing.Size(437, 20)
        Me.txtBox_OutputFileDialog.TabIndex = 48
        '
        'txtBox_CoincidentPointFile_FileDialog
        '
        Me.txtBox_CoincidentPointFile_FileDialog.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_CoincidentPointFile_FileDialog.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_CoincidentPointFile_FileDialog.Location = New System.Drawing.Point(152, 39)
        Me.txtBox_CoincidentPointFile_FileDialog.Name = "txtBox_CoincidentPointFile_FileDialog"
        Me.txtBox_CoincidentPointFile_FileDialog.ReadOnly = True
        Me.txtBox_CoincidentPointFile_FileDialog.Size = New System.Drawing.Size(400, 20)
        Me.txtBox_CoincidentPointFile_FileDialog.TabIndex = 61
        '
        'btn_CoincidentPointsFile
        '
        Me.btn_CoincidentPointsFile.Image = My.Resources.Resources.BrowseFolder
        Me.btn_CoincidentPointsFile.Location = New System.Drawing.Point(574, 37)
        Me.btn_CoincidentPointsFile.Name = "btn_CoincidentPointsFile"
        Me.btn_CoincidentPointsFile.Size = New System.Drawing.Size(29, 23)
        Me.btn_CoincidentPointsFile.TabIndex = 60
        Me.btn_CoincidentPointsFile.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btn_Parameter2Raster)
        Me.GroupBox1.Controls.Add(Me.btn_Parameter1Raster)
        Me.GroupBox1.Controls.Add(Me.txtBox_Parameter2Raster)
        Me.GroupBox1.Controls.Add(Me.txtBox_Parameter1Raster)
        Me.GroupBox1.Controls.Add(Me.cmbBox_Parameter2ColumnAlias)
        Me.GroupBox1.Controls.Add(Me.cmbBox_Parameter1ColumnAlias)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Location = New System.Drawing.Point(29, 19)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(606, 174)
        Me.GroupBox1.TabIndex = 62
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Inputs"
        '
        'btn_Parameter2Raster
        '
        Me.btn_Parameter2Raster.Image = My.Resources.Resources.BrowseFolder
        Me.btn_Parameter2Raster.Location = New System.Drawing.Point(348, 125)
        Me.btn_Parameter2Raster.Name = "btn_Parameter2Raster"
        Me.btn_Parameter2Raster.Size = New System.Drawing.Size(29, 23)
        Me.btn_Parameter2Raster.TabIndex = 64
        Me.btn_Parameter2Raster.UseVisualStyleBackColor = True
        '
        'btn_Parameter1Raster
        '
        Me.btn_Parameter1Raster.Image = My.Resources.Resources.BrowseFolder
        Me.btn_Parameter1Raster.Location = New System.Drawing.Point(348, 92)
        Me.btn_Parameter1Raster.Name = "btn_Parameter1Raster"
        Me.btn_Parameter1Raster.Size = New System.Drawing.Size(29, 23)
        Me.btn_Parameter1Raster.TabIndex = 63
        Me.btn_Parameter1Raster.UseVisualStyleBackColor = True
        '
        'txtBox_Parameter2Raster
        '
        Me.txtBox_Parameter2Raster.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_Parameter2Raster.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_Parameter2Raster.Location = New System.Drawing.Point(121, 132)
        Me.txtBox_Parameter2Raster.Name = "txtBox_Parameter2Raster"
        Me.txtBox_Parameter2Raster.ReadOnly = True
        Me.txtBox_Parameter2Raster.Size = New System.Drawing.Size(221, 20)
        Me.txtBox_Parameter2Raster.TabIndex = 62
        '
        'txtBox_Parameter1Raster
        '
        Me.txtBox_Parameter1Raster.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_Parameter1Raster.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_Parameter1Raster.Location = New System.Drawing.Point(121, 94)
        Me.txtBox_Parameter1Raster.Name = "txtBox_Parameter1Raster"
        Me.txtBox_Parameter1Raster.ReadOnly = True
        Me.txtBox_Parameter1Raster.Size = New System.Drawing.Size(221, 20)
        Me.txtBox_Parameter1Raster.TabIndex = 61
        '
        'cmbBox_Parameter2ColumnAlias
        '
        Me.cmbBox_Parameter2ColumnAlias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBox_Parameter2ColumnAlias.FormattingEnabled = True
        Me.cmbBox_Parameter2ColumnAlias.Items.AddRange(New Object() {"Roughness", "Slope", "Point Density", "Scan Angle", "Intensity"})
        Me.cmbBox_Parameter2ColumnAlias.Location = New System.Drawing.Point(479, 127)
        Me.cmbBox_Parameter2ColumnAlias.Name = "cmbBox_Parameter2ColumnAlias"
        Me.cmbBox_Parameter2ColumnAlias.Size = New System.Drawing.Size(121, 21)
        Me.cmbBox_Parameter2ColumnAlias.TabIndex = 60
        '
        'cmbBox_Parameter1ColumnAlias
        '
        Me.cmbBox_Parameter1ColumnAlias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBox_Parameter1ColumnAlias.FormattingEnabled = True
        Me.cmbBox_Parameter1ColumnAlias.Items.AddRange(New Object() {"Roughness", "Slope", "Point Density", "Scan Angle", "Intensity"})
        Me.cmbBox_Parameter1ColumnAlias.Location = New System.Drawing.Point(479, 88)
        Me.cmbBox_Parameter1ColumnAlias.Name = "cmbBox_Parameter1ColumnAlias"
        Me.cmbBox_Parameter1ColumnAlias.Size = New System.Drawing.Size(121, 21)
        Me.cmbBox_Parameter1ColumnAlias.TabIndex = 59
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(381, 135)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(92, 13)
        Me.Label7.TabIndex = 58
        Me.Label7.Text = "Parameter 2 Alias:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(381, 94)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(92, 13)
        Me.Label6.TabIndex = 57
        Me.Label6.Text = "Parameter 1 Alias:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 135)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(101, 13)
        Me.Label4.TabIndex = 56
        Me.Label4.Text = "Parameter 2 Raster:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 101)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(101, 13)
        Me.Label3.TabIndex = 55
        Me.Label3.Text = "Parameter 1 Raster:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 23)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(111, 13)
        Me.Label2.TabIndex = 50
        Me.Label2.Text = "Coincident Points File:"
        '
        'frm_FIS_DevelopmentAssistant
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(664, 328)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btn_Cancel)
        Me.Controls.Add(Me.btn_Run)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.txtBox_CoincidentPointFile_FileDialog)
        Me.Controls.Add(Me.btn_CoincidentPointsFile)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frm_FIS_DevelopmentAssistant"
        Me.Text = "FIS Development Assistant"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents btn_Cancel As System.Windows.Forms.Button
    Friend WithEvents btn_Run As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents btn_OutputFolder As System.Windows.Forms.Button
    Friend WithEvents txtBox_OutputFileDialog As System.Windows.Forms.TextBox
    Friend WithEvents txtBox_CoincidentPointFile_FileDialog As System.Windows.Forms.TextBox
    Friend WithEvents btn_CoincidentPointsFile As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btn_Parameter2Raster As System.Windows.Forms.Button
    Friend WithEvents btn_Parameter1Raster As System.Windows.Forms.Button
    Friend WithEvents txtBox_Parameter2Raster As System.Windows.Forms.TextBox
    Friend WithEvents txtBox_Parameter1Raster As System.Windows.Forms.TextBox
    Friend WithEvents cmbBox_Parameter2ColumnAlias As System.Windows.Forms.ComboBox
    Friend WithEvents cmbBox_Parameter1ColumnAlias As System.Windows.Forms.ComboBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
