<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBootstrapRoughness
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
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.grbInput = New System.Windows.Forms.GroupBox()
        Me.chkConvertToMillimeters = New System.Windows.Forms.CheckBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cboUnits = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.nudPercentData = New System.Windows.Forms.NumericUpDown()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.nudIterations = New System.Windows.Forms.NumericUpDown()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnOpenDEM = New System.Windows.Forms.Button()
        Me.txtDEMPath = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnOpenChannel = New System.Windows.Forms.Button()
        Me.txtChannelExtent = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnOpenSurveyExtent = New System.Windows.Forms.Button()
        Me.txtSurveyExtent = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnOpenSurveyPoints = New System.Windows.Forms.Button()
        Me.txtSurveyPoints = New System.Windows.Forms.TextBox()
        Me.grbOutput = New System.Windows.Forms.GroupBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtOutputRaster = New System.Windows.Forms.TextBox()
        Me.btnSaveOutputRaster = New System.Windows.Forms.Button()
        Me.grbInput.SuspendLayout()
        CType(Me.nudPercentData, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudIterations, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grbOutput.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOK.Location = New System.Drawing.Point(429, 361)
        Me.btnOK.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 5
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(508, 361)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 6
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnHelp
        '
        Me.btnHelp.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnHelp.Location = New System.Drawing.Point(9, 361)
        Me.btnHelp.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(75, 23)
        Me.btnHelp.TabIndex = 7
        Me.btnHelp.Text = "Help"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'grbInput
        '
        Me.grbInput.Controls.Add(Me.chkConvertToMillimeters)
        Me.grbInput.Controls.Add(Me.Label8)
        Me.grbInput.Controls.Add(Me.cboUnits)
        Me.grbInput.Controls.Add(Me.Label6)
        Me.grbInput.Controls.Add(Me.nudPercentData)
        Me.grbInput.Controls.Add(Me.Label5)
        Me.grbInput.Controls.Add(Me.nudIterations)
        Me.grbInput.Controls.Add(Me.Label4)
        Me.grbInput.Controls.Add(Me.btnOpenDEM)
        Me.grbInput.Controls.Add(Me.txtDEMPath)
        Me.grbInput.Controls.Add(Me.Label3)
        Me.grbInput.Controls.Add(Me.btnOpenChannel)
        Me.grbInput.Controls.Add(Me.txtChannelExtent)
        Me.grbInput.Controls.Add(Me.Label2)
        Me.grbInput.Controls.Add(Me.btnOpenSurveyExtent)
        Me.grbInput.Controls.Add(Me.txtSurveyExtent)
        Me.grbInput.Controls.Add(Me.Label1)
        Me.grbInput.Controls.Add(Me.btnOpenSurveyPoints)
        Me.grbInput.Controls.Add(Me.txtSurveyPoints)
        Me.grbInput.Location = New System.Drawing.Point(10, 11)
        Me.grbInput.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grbInput.Name = "grbInput"
        Me.grbInput.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grbInput.Size = New System.Drawing.Size(574, 236)
        Me.grbInput.TabIndex = 8
        Me.grbInput.TabStop = False
        Me.grbInput.Text = "Inputs"
        '
        'chkConvertToMillimeters
        '
        Me.chkConvertToMillimeters.AutoSize = True
        Me.chkConvertToMillimeters.Location = New System.Drawing.Point(110, 161)
        Me.chkConvertToMillimeters.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.chkConvertToMillimeters.Name = "chkConvertToMillimeters"
        Me.chkConvertToMillimeters.Size = New System.Drawing.Size(125, 17)
        Me.chkConvertToMillimeters.TabIndex = 20
        Me.chkConvertToMillimeters.Text = "Convert to millimeters"
        Me.chkConvertToMillimeters.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(263, 161)
        Me.Label8.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(101, 13)
        Me.Label8.TabIndex = 19
        Me.Label8.Text = "Vertical unit of DEM"
        '
        'cboUnits
        '
        Me.cboUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboUnits.FormattingEnabled = True
        Me.cboUnits.Location = New System.Drawing.Point(370, 158)
        Me.cboUnits.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.cboUnits.Name = "cboUnits"
        Me.cboUnits.Size = New System.Drawing.Size(92, 21)
        Me.cboUnits.TabIndex = 18
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(185, 198)
        Me.Label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(234, 13)
        Me.Label6.TabIndex = 17
        Me.Label6.Text = "Percent of data to keep as subset each iteration"
        '
        'nudPercentData
        '
        Me.nudPercentData.DecimalPlaces = 2
        Me.nudPercentData.Increment = New Decimal(New Integer() {5, 0, 0, 131072})
        Me.nudPercentData.Location = New System.Drawing.Point(423, 196)
        Me.nudPercentData.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.nudPercentData.Maximum = New Decimal(New Integer() {99, 0, 0, 131072})
        Me.nudPercentData.Minimum = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudPercentData.Name = "nudPercentData"
        Me.nudPercentData.Size = New System.Drawing.Size(38, 20)
        Me.nudPercentData.TabIndex = 16
        Me.nudPercentData.Value = New Decimal(New Integer() {80, 0, 0, 131072})
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(4, 198)
        Me.Label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(102, 13)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "Number of Iterations"
        '
        'nudIterations
        '
        Me.nudIterations.Location = New System.Drawing.Point(110, 196)
        Me.nudIterations.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.nudIterations.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.nudIterations.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudIterations.Name = "nudIterations"
        Me.nudIterations.Size = New System.Drawing.Size(49, 20)
        Me.nudIterations.TabIndex = 14
        Me.nudIterations.Value = New Decimal(New Integer() {50, 0, 0, 0})
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(4, 132)
        Me.Label4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(34, 13)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "DEM:"
        '
        'btnOpenDEM
        '
        Me.btnOpenDEM.Image = My.Resources.Resources.BrowseFolder
        Me.btnOpenDEM.Location = New System.Drawing.Point(542, 130)
        Me.btnOpenDEM.Name = "btnOpenDEM"
        Me.btnOpenDEM.Size = New System.Drawing.Size(26, 23)
        Me.btnOpenDEM.TabIndex = 12
        Me.btnOpenDEM.UseVisualStyleBackColor = True
        '
        'txtDEMPath
        '
        Me.txtDEMPath.Location = New System.Drawing.Point(110, 132)
        Me.txtDEMPath.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtDEMPath.Name = "txtDEMPath"
        Me.txtDEMPath.Size = New System.Drawing.Size(428, 20)
        Me.txtDEMPath.TabIndex = 11
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(4, 97)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(128, 13)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Channel Extent (optional):"
        '
        'btnOpenChannel
        '
        Me.btnOpenChannel.Image = My.Resources.Resources.BrowseFolder
        Me.btnOpenChannel.Location = New System.Drawing.Point(542, 92)
        Me.btnOpenChannel.Name = "btnOpenChannel"
        Me.btnOpenChannel.Size = New System.Drawing.Size(26, 23)
        Me.btnOpenChannel.TabIndex = 9
        Me.btnOpenChannel.UseVisualStyleBackColor = True
        '
        'txtChannelExtent
        '
        Me.txtChannelExtent.Location = New System.Drawing.Point(137, 94)
        Me.txtChannelExtent.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtChannelExtent.Name = "txtChannelExtent"
        Me.txtChannelExtent.Size = New System.Drawing.Size(400, 20)
        Me.txtChannelExtent.TabIndex = 8
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(4, 63)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(76, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Survey Extent:"
        '
        'btnOpenSurveyExtent
        '
        Me.btnOpenSurveyExtent.Image = My.Resources.Resources.BrowseFolder
        Me.btnOpenSurveyExtent.Location = New System.Drawing.Point(542, 58)
        Me.btnOpenSurveyExtent.Name = "btnOpenSurveyExtent"
        Me.btnOpenSurveyExtent.Size = New System.Drawing.Size(26, 23)
        Me.btnOpenSurveyExtent.TabIndex = 6
        Me.btnOpenSurveyExtent.UseVisualStyleBackColor = True
        '
        'txtSurveyExtent
        '
        Me.txtSurveyExtent.Location = New System.Drawing.Point(110, 60)
        Me.txtSurveyExtent.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtSurveyExtent.Name = "txtSurveyExtent"
        Me.txtSurveyExtent.Size = New System.Drawing.Size(428, 20)
        Me.txtSurveyExtent.TabIndex = 5
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(4, 26)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(102, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Input Survey Points:"
        '
        'btnOpenSurveyPoints
        '
        Me.btnOpenSurveyPoints.Image = My.Resources.Resources.BrowseFolder
        Me.btnOpenSurveyPoints.Location = New System.Drawing.Point(542, 24)
        Me.btnOpenSurveyPoints.Name = "btnOpenSurveyPoints"
        Me.btnOpenSurveyPoints.Size = New System.Drawing.Size(26, 23)
        Me.btnOpenSurveyPoints.TabIndex = 3
        Me.btnOpenSurveyPoints.UseVisualStyleBackColor = True
        '
        'txtSurveyPoints
        '
        Me.txtSurveyPoints.Location = New System.Drawing.Point(110, 26)
        Me.txtSurveyPoints.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtSurveyPoints.Name = "txtSurveyPoints"
        Me.txtSurveyPoints.Size = New System.Drawing.Size(428, 20)
        Me.txtSurveyPoints.TabIndex = 0
        '
        'grbOutput
        '
        Me.grbOutput.Controls.Add(Me.Label7)
        Me.grbOutput.Controls.Add(Me.txtOutputRaster)
        Me.grbOutput.Controls.Add(Me.btnSaveOutputRaster)
        Me.grbOutput.Location = New System.Drawing.Point(9, 263)
        Me.grbOutput.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grbOutput.Name = "grbOutput"
        Me.grbOutput.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.grbOutput.Size = New System.Drawing.Size(574, 80)
        Me.grbOutput.TabIndex = 9
        Me.grbOutput.TabStop = False
        Me.grbOutput.Text = "Output"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(6, 41)
        Me.Label7.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(101, 13)
        Me.Label7.TabIndex = 20
        Me.Label7.Text = "Output Raster Path:"
        '
        'txtOutputRaster
        '
        Me.txtOutputRaster.Location = New System.Drawing.Point(111, 41)
        Me.txtOutputRaster.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtOutputRaster.Name = "txtOutputRaster"
        Me.txtOutputRaster.Size = New System.Drawing.Size(428, 20)
        Me.txtOutputRaster.TabIndex = 18
        '
        'btnSaveOutputRaster
        '
        Me.btnSaveOutputRaster.Image = My.Resources.Resources.BrowseFolder
        Me.btnSaveOutputRaster.Location = New System.Drawing.Point(543, 38)
        Me.btnSaveOutputRaster.Name = "btnSaveOutputRaster"
        Me.btnSaveOutputRaster.Size = New System.Drawing.Size(26, 23)
        Me.btnSaveOutputRaster.TabIndex = 19
        Me.btnSaveOutputRaster.UseVisualStyleBackColor = True
        '
        'frmBootstrapRoughness
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(592, 393)
        Me.Controls.Add(Me.grbOutput)
        Me.Controls.Add(Me.grbInput)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmBootstrapRoughness"
        Me.Text = "Create Bootstrap Roughness Raster "
        Me.grbInput.ResumeLayout(False)
        Me.grbInput.PerformLayout()
        CType(Me.nudPercentData, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudIterations, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grbOutput.ResumeLayout(False)
        Me.grbOutput.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents grbInput As System.Windows.Forms.GroupBox
    Friend WithEvents txtSurveyPoints As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnOpenSurveyPoints As System.Windows.Forms.Button
    Friend WithEvents nudIterations As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnOpenDEM As System.Windows.Forms.Button
    Friend WithEvents txtDEMPath As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnOpenChannel As System.Windows.Forms.Button
    Friend WithEvents txtChannelExtent As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnOpenSurveyExtent As System.Windows.Forms.Button
    Friend WithEvents txtSurveyExtent As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents nudPercentData As System.Windows.Forms.NumericUpDown
    Friend WithEvents grbOutput As System.Windows.Forms.GroupBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtOutputRaster As System.Windows.Forms.TextBox
    Friend WithEvents btnSaveOutputRaster As System.Windows.Forms.Button
    Friend WithEvents cboUnits As System.Windows.Forms.ComboBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents chkConvertToMillimeters As System.Windows.Forms.CheckBox
End Class
