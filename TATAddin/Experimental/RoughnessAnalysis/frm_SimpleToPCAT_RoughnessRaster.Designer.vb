<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_SimpleToPCAT_RoughnessRaster
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frm_SimpleToPCAT_RoughnessRaster))
        Me.btn_Cancel = New System.Windows.Forms.Button()
        Me.btn_Run = New System.Windows.Forms.Button()
        Me.btn_Help = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.label_CellSizeUnits = New System.Windows.Forms.Label()
        Me.lbl_CellSize = New System.Windows.Forms.Label()
        Me.updn_CellSize = New System.Windows.Forms.NumericUpDown()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtBox_RawPointCloud = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.cmb_ZField = New System.Windows.Forms.ComboBox()
        Me.txtBox_ExtentShp = New System.Windows.Forms.TextBox()
        Me.btn_ExtentShp = New System.Windows.Forms.Button()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btn_OpenRawPointFile = New System.Windows.Forms.Button()
        Me.yResolution = New System.Windows.Forms.NumericUpDown()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.xResolution = New System.Windows.Forms.NumericUpDown()
        Me.txtBox_SpatialReference = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btn_SpatialReference = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.nToCalculateStats = New System.Windows.Forms.NumericUpDown()
        Me.ttip = New System.Windows.Forms.ToolTip(Me.components)
        Me.btn_OutputRaster = New System.Windows.Forms.Button()
        Me.txtBox_OutputRaster = New System.Windows.Forms.TextBox()
        Me.GroupBox2.SuspendLayout()
        CType(Me.updn_CellSize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.yResolution, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.xResolution, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nToCalculateStats, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btn_Cancel
        '
        Me.btn_Cancel.Location = New System.Drawing.Point(502, 445)
        Me.btn_Cancel.Name = "btn_Cancel"
        Me.btn_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.btn_Cancel.TabIndex = 14
        Me.btn_Cancel.Text = "Cancel"
        Me.btn_Cancel.UseVisualStyleBackColor = True
        '
        'btn_Run
        '
        Me.btn_Run.Location = New System.Drawing.Point(400, 445)
        Me.btn_Run.Name = "btn_Run"
        Me.btn_Run.Size = New System.Drawing.Size(75, 23)
        Me.btn_Run.TabIndex = 13
        Me.btn_Run.Text = "Run Tool"
        Me.btn_Run.UseVisualStyleBackColor = True
        '
        'btn_Help
        '
        Me.btn_Help.Image = My.Resources.Resources.Help
        Me.btn_Help.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btn_Help.Location = New System.Drawing.Point(31, 445)
        Me.btn_Help.Name = "btn_Help"
        Me.btn_Help.Size = New System.Drawing.Size(64, 23)
        Me.btn_Help.TabIndex = 11
        Me.btn_Help.Text = "Help"
        Me.btn_Help.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btn_Help.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.label_CellSizeUnits)
        Me.GroupBox2.Controls.Add(Me.lbl_CellSize)
        Me.GroupBox2.Controls.Add(Me.updn_CellSize)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.txtBox_OutputRaster)
        Me.GroupBox2.Controls.Add(Me.btn_OutputRaster)
        Me.GroupBox2.Location = New System.Drawing.Point(29, 288)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(546, 134)
        Me.GroupBox2.TabIndex = 10
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Output"
        '
        'label_CellSizeUnits
        '
        Me.label_CellSizeUnits.AutoSize = True
        Me.label_CellSizeUnits.Location = New System.Drawing.Point(199, 34)
        Me.label_CellSizeUnits.Name = "label_CellSizeUnits"
        Me.label_CellSizeUnits.Size = New System.Drawing.Size(31, 13)
        Me.label_CellSizeUnits.TabIndex = 36
        Me.label_CellSizeUnits.Text = "Units"
        '
        'lbl_CellSize
        '
        Me.lbl_CellSize.AutoSize = True
        Me.lbl_CellSize.Location = New System.Drawing.Point(7, 34)
        Me.lbl_CellSize.Name = "lbl_CellSize"
        Me.lbl_CellSize.Size = New System.Drawing.Size(116, 13)
        Me.lbl_CellSize.TabIndex = 35
        Me.lbl_CellSize.Text = "Output Raster Cell Size"
        '
        'updn_CellSize
        '
        Me.updn_CellSize.DecimalPlaces = 4
        Me.updn_CellSize.Location = New System.Drawing.Point(129, 32)
        Me.updn_CellSize.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.updn_CellSize.Minimum = New Decimal(New Integer() {1, 0, 0, 262144})
        Me.updn_CellSize.Name = "updn_CellSize"
        Me.updn_CellSize.Size = New System.Drawing.Size(64, 20)
        Me.updn_CellSize.TabIndex = 34
        Me.updn_CellSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.updn_CellSize.ThousandsSeparator = True
        Me.updn_CellSize.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(44, 91)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(76, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Output Raster:"
        '
        'txtBox_RawPointCloud
        '
        Me.txtBox_RawPointCloud.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_RawPointCloud.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_RawPointCloud.Location = New System.Drawing.Point(155, 47)
        Me.txtBox_RawPointCloud.Name = "txtBox_RawPointCloud"
        Me.txtBox_RawPointCloud.ReadOnly = True
        Me.txtBox_RawPointCloud.Size = New System.Drawing.Size(342, 20)
        Me.txtBox_RawPointCloud.TabIndex = 8
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label17)
        Me.GroupBox1.Controls.Add(Me.cmb_ZField)
        Me.GroupBox1.Controls.Add(Me.txtBox_ExtentShp)
        Me.GroupBox1.Controls.Add(Me.btn_ExtentShp)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.btn_OpenRawPointFile)
        Me.GroupBox1.Controls.Add(Me.yResolution)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.xResolution)
        Me.GroupBox1.Controls.Add(Me.txtBox_SpatialReference)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.btn_SpatialReference)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.nToCalculateStats)
        Me.GroupBox1.Location = New System.Drawing.Point(29, 18)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(546, 244)
        Me.GroupBox1.TabIndex = 9
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Inputs"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(89, 148)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(71, 13)
        Me.Label17.TabIndex = 47
        Me.Label17.Text = "Raster Value:"
        '
        'cmb_ZField
        '
        Me.cmb_ZField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmb_ZField.FormattingEnabled = True
        Me.cmb_ZField.Items.AddRange(New Object() {"zmean", "zmean detrended", "zmax", "zmin", "range", "standard deviation", "standard deviation detrended", "skew", "skew detrended", "kurtosis", "kurtosis detrended", "n"})
        Me.cmb_ZField.Location = New System.Drawing.Point(166, 143)
        Me.cmb_ZField.Name = "cmb_ZField"
        Me.cmb_ZField.Size = New System.Drawing.Size(176, 21)
        Me.cmb_ZField.TabIndex = 44
        '
        'txtBox_ExtentShp
        '
        Me.txtBox_ExtentShp.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_ExtentShp.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_ExtentShp.Enabled = False
        Me.txtBox_ExtentShp.Location = New System.Drawing.Point(126, 177)
        Me.txtBox_ExtentShp.Name = "txtBox_ExtentShp"
        Me.txtBox_ExtentShp.ReadOnly = True
        Me.txtBox_ExtentShp.Size = New System.Drawing.Size(342, 20)
        Me.txtBox_ExtentShp.TabIndex = 42
        '
        'btn_ExtentShp
        '
        Me.btn_ExtentShp.Enabled = False
        Me.btn_ExtentShp.Image = My.Resources.Resources.BrowseFolder
        Me.btn_ExtentShp.Location = New System.Drawing.Point(481, 171)
        Me.btn_ExtentShp.Name = "btn_ExtentShp"
        Me.btn_ExtentShp.Size = New System.Drawing.Size(26, 23)
        Me.btn_ExtentShp.TabIndex = 41
        Me.btn_ExtentShp.UseVisualStyleBackColor = True
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Enabled = False
        Me.Label9.Location = New System.Drawing.Point(39, 180)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(81, 13)
        Me.Label9.TabIndex = 43
        Me.Label9.Text = "Extent Polygon:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(272, 63)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(240, 13)
        Me.Label7.TabIndex = 14
        Me.Label7.Text = "Minimum Number of Points To Calculate Statistics"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(25, 214)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(95, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Spatial Reference:"
        '
        'btn_OpenRawPointFile
        '
        Me.btn_OpenRawPointFile.Image = My.Resources.Resources.BrowseFolder
        Me.btn_OpenRawPointFile.Location = New System.Drawing.Point(481, 26)
        Me.btn_OpenRawPointFile.Name = "btn_OpenRawPointFile"
        Me.btn_OpenRawPointFile.Size = New System.Drawing.Size(26, 23)
        Me.btn_OpenRawPointFile.TabIndex = 1
        Me.btn_OpenRawPointFile.UseVisualStyleBackColor = True
        '
        'yResolution
        '
        Me.yResolution.DecimalPlaces = 4
        Me.yResolution.Increment = New Decimal(New Integer() {50, 0, 0, 131072})
        Me.yResolution.Location = New System.Drawing.Point(166, 112)
        Me.yResolution.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.yResolution.Minimum = New Decimal(New Integer() {1, 0, 0, 262144})
        Me.yResolution.Name = "yResolution"
        Me.yResolution.Size = New System.Drawing.Size(62, 20)
        Me.yResolution.TabIndex = 13
        Me.yResolution.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.yResolution.ThousandsSeparator = True
        Me.yResolution.Value = New Decimal(New Integer() {2000, 0, 0, 196608})
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 35)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(116, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Input Raw Point Cloud:"
        '
        'xResolution
        '
        Me.xResolution.DecimalPlaces = 4
        Me.xResolution.Increment = New Decimal(New Integer() {50, 0, 0, 131072})
        Me.xResolution.Location = New System.Drawing.Point(166, 86)
        Me.xResolution.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.xResolution.Minimum = New Decimal(New Integer() {1, 0, 0, 262144})
        Me.xResolution.Name = "xResolution"
        Me.xResolution.Size = New System.Drawing.Size(62, 20)
        Me.xResolution.TabIndex = 12
        Me.xResolution.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.xResolution.ThousandsSeparator = True
        Me.xResolution.Value = New Decimal(New Integer() {2000, 0, 0, 196608})
        '
        'txtBox_SpatialReference
        '
        Me.txtBox_SpatialReference.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_SpatialReference.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_SpatialReference.Location = New System.Drawing.Point(126, 212)
        Me.txtBox_SpatialReference.Name = "txtBox_SpatialReference"
        Me.txtBox_SpatialReference.ReadOnly = True
        Me.txtBox_SpatialReference.Size = New System.Drawing.Size(342, 20)
        Me.txtBox_SpatialReference.TabIndex = 3
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(133, 119)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(12, 13)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "y"
        '
        'btn_SpatialReference
        '
        Me.btn_SpatialReference.Image = My.Resources.Resources.BrowseFolder
        Me.btn_SpatialReference.Location = New System.Drawing.Point(481, 209)
        Me.btn_SpatialReference.Name = "btn_SpatialReference"
        Me.btn_SpatialReference.Size = New System.Drawing.Size(26, 23)
        Me.btn_SpatialReference.TabIndex = 3
        Me.btn_SpatialReference.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(133, 93)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(12, 13)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "x"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(123, 63)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(107, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Sample Window Size"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(272, 88)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(13, 13)
        Me.Label8.TabIndex = 16
        Me.Label8.Text = "n"
        '
        'nToCalculateStats
        '
        Me.nToCalculateStats.Location = New System.Drawing.Point(296, 86)
        Me.nToCalculateStats.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        Me.nToCalculateStats.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.nToCalculateStats.Name = "nToCalculateStats"
        Me.nToCalculateStats.Size = New System.Drawing.Size(57, 20)
        Me.nToCalculateStats.TabIndex = 15
        Me.nToCalculateStats.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.nToCalculateStats.Value = New Decimal(New Integer() {4, 0, 0, 0})
        '
        'btn_OutputRaster
        '
        Me.btn_OutputRaster.Image = My.Resources.Resources.Save
        Me.btn_OutputRaster.Location = New System.Drawing.Point(481, 86)
        Me.btn_OutputRaster.Name = "btn_OutputRaster"
        Me.btn_OutputRaster.Size = New System.Drawing.Size(26, 23)
        Me.btn_OutputRaster.TabIndex = 4
        Me.btn_OutputRaster.UseVisualStyleBackColor = True
        '
        'txtBox_OutputRaster
        '
        Me.txtBox_OutputRaster.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_OutputRaster.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_OutputRaster.Location = New System.Drawing.Point(126, 89)
        Me.txtBox_OutputRaster.Name = "txtBox_OutputRaster"
        Me.txtBox_OutputRaster.ReadOnly = True
        Me.txtBox_OutputRaster.Size = New System.Drawing.Size(342, 20)
        Me.txtBox_OutputRaster.TabIndex = 5
        '
        'frm_SimpleToPCAT_RoughnessRaster
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(607, 487)
        Me.Controls.Add(Me.btn_Cancel)
        Me.Controls.Add(Me.btn_Run)
        Me.Controls.Add(Me.btn_Help)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.txtBox_RawPointCloud)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frm_SimpleToPCAT_RoughnessRaster"
        Me.Text = "Simple ToPCAT Roughness"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.updn_CellSize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.yResolution, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.xResolution, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nToCalculateStats, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btn_Cancel As System.Windows.Forms.Button
    Friend WithEvents btn_Run As System.Windows.Forms.Button
    Friend WithEvents btn_Help As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents label_CellSizeUnits As System.Windows.Forms.Label
    Friend WithEvents lbl_CellSize As System.Windows.Forms.Label
    Friend WithEvents updn_CellSize As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtBox_RawPointCloud As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents cmb_ZField As System.Windows.Forms.ComboBox
    Friend WithEvents txtBox_ExtentShp As System.Windows.Forms.TextBox
    Friend WithEvents btn_ExtentShp As System.Windows.Forms.Button
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btn_OpenRawPointFile As System.Windows.Forms.Button
    Friend WithEvents yResolution As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents xResolution As System.Windows.Forms.NumericUpDown
    Friend WithEvents txtBox_SpatialReference As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btn_SpatialReference As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents nToCalculateStats As System.Windows.Forms.NumericUpDown
    Friend WithEvents ttip As System.Windows.Forms.ToolTip
    Friend WithEvents txtBox_OutputRaster As System.Windows.Forms.TextBox
    Friend WithEvents btn_OutputRaster As System.Windows.Forms.Button
End Class
