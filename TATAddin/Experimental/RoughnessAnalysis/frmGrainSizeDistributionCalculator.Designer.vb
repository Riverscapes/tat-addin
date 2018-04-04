<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGrainSizeDistributionCalculator
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
        Me.grbInputs = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.cboGrainSizeUnits = New System.Windows.Forms.ComboBox()
        Me.chkAppendNRasterCells = New System.Windows.Forms.CheckBox()
        Me.chkAppendGrainSizeValues = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cboChannelUnitField = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblGrainInput = New System.Windows.Forms.Label()
        Me.btnInputPolygon = New System.Windows.Forms.Button()
        Me.btn_OpenRawPointFile = New System.Windows.Forms.Button()
        Me.txtInputPolygon = New System.Windows.Forms.TextBox()
        Me.txtGrainSampleFile = New System.Windows.Forms.TextBox()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.chkCreateCDF = New System.Windows.Forms.CheckBox()
        Me.txtCDFOutputPath = New System.Windows.Forms.TextBox()
        Me.grbOutput = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnOutputCDFFolder = New System.Windows.Forms.Button()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.grbInputs.SuspendLayout()
        Me.grbOutput.SuspendLayout()
        Me.SuspendLayout()
        '
        'grbInputs
        '
        Me.grbInputs.Controls.Add(Me.Label6)
        Me.grbInputs.Controls.Add(Me.cboGrainSizeUnits)
        Me.grbInputs.Controls.Add(Me.chkAppendNRasterCells)
        Me.grbInputs.Controls.Add(Me.chkAppendGrainSizeValues)
        Me.grbInputs.Controls.Add(Me.Label5)
        Me.grbInputs.Controls.Add(Me.cboChannelUnitField)
        Me.grbInputs.Controls.Add(Me.Label1)
        Me.grbInputs.Controls.Add(Me.lblGrainInput)
        Me.grbInputs.Controls.Add(Me.btnInputPolygon)
        Me.grbInputs.Controls.Add(Me.btn_OpenRawPointFile)
        Me.grbInputs.Controls.Add(Me.txtInputPolygon)
        Me.grbInputs.Controls.Add(Me.txtGrainSampleFile)
        Me.grbInputs.Location = New System.Drawing.Point(13, 13)
        Me.grbInputs.Name = "grbInputs"
        Me.grbInputs.Size = New System.Drawing.Size(780, 215)
        Me.grbInputs.TabIndex = 0
        Me.grbInputs.TabStop = False
        Me.grbInputs.Text = "Inputs"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(191, 58)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(224, 17)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Surface Roughness Vertical Units:"
        '
        'cboGrainSizeUnits
        '
        Me.cboGrainSizeUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboGrainSizeUnits.FormattingEnabled = True
        Me.cboGrainSizeUnits.Location = New System.Drawing.Point(421, 55)
        Me.cboGrainSizeUnits.Name = "cboGrainSizeUnits"
        Me.cboGrainSizeUnits.Size = New System.Drawing.Size(212, 24)
        Me.cboGrainSizeUnits.TabIndex = 13
        '
        'chkAppendNRasterCells
        '
        Me.chkAppendNRasterCells.AutoSize = True
        Me.chkAppendNRasterCells.Location = New System.Drawing.Point(528, 177)
        Me.chkAppendNRasterCells.Name = "chkAppendNRasterCells"
        Me.chkAppendNRasterCells.Size = New System.Drawing.Size(220, 21)
        Me.chkAppendNRasterCells.TabIndex = 12
        Me.chkAppendNRasterCells.Text = "Append number of raster cells"
        Me.chkAppendNRasterCells.UseVisualStyleBackColor = True
        '
        'chkAppendGrainSizeValues
        '
        Me.chkAppendGrainSizeValues.AutoSize = True
        Me.chkAppendGrainSizeValues.Location = New System.Drawing.Point(162, 177)
        Me.chkAppendGrainSizeValues.Name = "chkAppendGrainSizeValues"
        Me.chkAppendGrainSizeValues.Size = New System.Drawing.Size(335, 21)
        Me.chkAppendGrainSizeValues.TabIndex = 11
        Me.chkAppendGrainSizeValues.Text = "Append grain size metric values to channel units"
        Me.chkAppendGrainSizeValues.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(159, 137)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(147, 17)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Unique Channel Field:"
        '
        'cboChannelUnitField
        '
        Me.cboChannelUnitField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboChannelUnitField.FormattingEnabled = True
        Me.cboChannelUnitField.Location = New System.Drawing.Point(312, 130)
        Me.cboChannelUnitField.Name = "cboChannelUnitField"
        Me.cboChannelUnitField.Size = New System.Drawing.Size(321, 24)
        Me.cboChannelUnitField.TabIndex = 9
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(5, 102)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(148, 17)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Channel Unit Polygon:"
        '
        'lblGrainInput
        '
        Me.lblGrainInput.AutoSize = True
        Me.lblGrainInput.Location = New System.Drawing.Point(5, 28)
        Me.lblGrainInput.Name = "lblGrainInput"
        Me.lblGrainInput.Size = New System.Drawing.Size(183, 17)
        Me.lblGrainInput.TabIndex = 4
        Me.lblGrainInput.Text = "Surface Roughness Raster:"
        '
        'btnInputPolygon
        '
        Me.btnInputPolygon.Image = My.Resources.Resources.BrowseFolder
        Me.btnInputPolygon.Location = New System.Drawing.Point(735, 96)
        Me.btnInputPolygon.Margin = New System.Windows.Forms.Padding(4)
        Me.btnInputPolygon.Name = "btnInputPolygon"
        Me.btnInputPolygon.Size = New System.Drawing.Size(35, 28)
        Me.btnInputPolygon.TabIndex = 3
        Me.btnInputPolygon.UseVisualStyleBackColor = True
        '
        'btn_OpenRawPointFile
        '
        Me.btn_OpenRawPointFile.Image = My.Resources.Resources.BrowseFolder
        Me.btn_OpenRawPointFile.Location = New System.Drawing.Point(735, 25)
        Me.btn_OpenRawPointFile.Margin = New System.Windows.Forms.Padding(4)
        Me.btn_OpenRawPointFile.Name = "btn_OpenRawPointFile"
        Me.btn_OpenRawPointFile.Size = New System.Drawing.Size(35, 28)
        Me.btn_OpenRawPointFile.TabIndex = 2
        Me.btn_OpenRawPointFile.UseVisualStyleBackColor = True
        '
        'txtInputPolygon
        '
        Me.txtInputPolygon.Location = New System.Drawing.Point(159, 102)
        Me.txtInputPolygon.Name = "txtInputPolygon"
        Me.txtInputPolygon.Size = New System.Drawing.Size(569, 22)
        Me.txtInputPolygon.TabIndex = 1
        '
        'txtGrainSampleFile
        '
        Me.txtGrainSampleFile.Location = New System.Drawing.Point(194, 28)
        Me.txtGrainSampleFile.Name = "txtGrainSampleFile"
        Me.txtGrainSampleFile.Size = New System.Drawing.Size(534, 22)
        Me.txtGrainSampleFile.TabIndex = 0
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(587, 343)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(100, 28)
        Me.btnOK.TabIndex = 1
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(693, 343)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 28)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'chkCreateCDF
        '
        Me.chkCreateCDF.AutoSize = True
        Me.chkCreateCDF.Location = New System.Drawing.Point(140, 24)
        Me.chkCreateCDF.Name = "chkCreateCDF"
        Me.chkCreateCDF.Size = New System.Drawing.Size(248, 21)
        Me.chkCreateCDF.TabIndex = 3
        Me.chkCreateCDF.Text = "Create Cumulative Distribution Plot"
        Me.chkCreateCDF.UseVisualStyleBackColor = True
        '
        'txtCDFOutputPath
        '
        Me.txtCDFOutputPath.Location = New System.Drawing.Point(140, 51)
        Me.txtCDFOutputPath.Name = "txtCDFOutputPath"
        Me.txtCDFOutputPath.Size = New System.Drawing.Size(591, 22)
        Me.txtCDFOutputPath.TabIndex = 4
        '
        'grbOutput
        '
        Me.grbOutput.Controls.Add(Me.Label3)
        Me.grbOutput.Controls.Add(Me.btnOutputCDFFolder)
        Me.grbOutput.Controls.Add(Me.txtCDFOutputPath)
        Me.grbOutput.Controls.Add(Me.chkCreateCDF)
        Me.grbOutput.Location = New System.Drawing.Point(10, 235)
        Me.grbOutput.Name = "grbOutput"
        Me.grbOutput.Size = New System.Drawing.Size(783, 96)
        Me.grbOutput.TabIndex = 4
        Me.grbOutput.TabStop = False
        Me.grbOutput.Text = "Output"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(19, 51)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 17)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "CDF Folder:"
        '
        'btnOutputCDFFolder
        '
        Me.btnOutputCDFFolder.Image = My.Resources.Resources.BrowseFolder
        Me.btnOutputCDFFolder.Location = New System.Drawing.Point(738, 48)
        Me.btnOutputCDFFolder.Margin = New System.Windows.Forms.Padding(4)
        Me.btnOutputCDFFolder.Name = "btnOutputCDFFolder"
        Me.btnOutputCDFFolder.Size = New System.Drawing.Size(35, 28)
        Me.btnOutputCDFFolder.TabIndex = 7
        Me.btnOutputCDFFolder.UseVisualStyleBackColor = True
        '
        'btnHelp
        '
        Me.btnHelp.Location = New System.Drawing.Point(12, 343)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(100, 28)
        Me.btnHelp.TabIndex = 5
        Me.btnHelp.Text = "Help"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'frmGrainSizeDistributionCalculator
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(805, 390)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.grbOutput)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.grbInputs)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(823, 435)
        Me.Name = "frmGrainSizeDistributionCalculator"
        Me.Text = "Grain Size Distribution Calculator"
        Me.grbInputs.ResumeLayout(False)
        Me.grbInputs.PerformLayout()
        Me.grbOutput.ResumeLayout(False)
        Me.grbOutput.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents grbInputs As System.Windows.Forms.GroupBox
    Friend WithEvents txtInputPolygon As System.Windows.Forms.TextBox
    Friend WithEvents txtGrainSampleFile As System.Windows.Forms.TextBox
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnInputPolygon As System.Windows.Forms.Button
    Friend WithEvents btn_OpenRawPointFile As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblGrainInput As System.Windows.Forms.Label
    Friend WithEvents chkCreateCDF As System.Windows.Forms.CheckBox
    Friend WithEvents txtCDFOutputPath As System.Windows.Forms.TextBox
    Friend WithEvents grbOutput As System.Windows.Forms.GroupBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnOutputCDFFolder As System.Windows.Forms.Button
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents chkAppendGrainSizeValues As System.Windows.Forms.CheckBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents cboChannelUnitField As System.Windows.Forms.ComboBox
    Friend WithEvents chkAppendNRasterCells As System.Windows.Forms.CheckBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cboGrainSizeUnits As System.Windows.Forms.ComboBox
End Class
