<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_InterploationError
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frm_InterploationError))
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.btn_Cancel = New System.Windows.Forms.Button()
        Me.btn_Run = New System.Windows.Forms.Button()
        Me.btn_OutputRaster = New System.Windows.Forms.Button()
        Me.txtBox_ExtentShp = New System.Windows.Forms.TextBox()
        Me.btn_ExtentShp = New System.Windows.Forms.Button()
        Me.txtBox_PointCloudShp = New System.Windows.Forms.TextBox()
        Me.btn_PointCloudShp = New System.Windows.Forms.Button()
        Me.txtBox_SpatialReference = New System.Windows.Forms.TextBox()
        Me.btn_SpatialReference = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.cmb_ZField = New System.Windows.Forms.ComboBox()
        Me.btnInputRaster = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtInputRaster = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.label_CellSizeUnits = New System.Windows.Forms.Label()
        Me.lbl_CellSize = New System.Windows.Forms.Label()
        Me.updn_CellSize = New System.Windows.Forms.NumericUpDown()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtBox_OutputDEM = New System.Windows.Forms.TextBox()
        Me.GroupBox1.SuspendLayout()
        CType(Me.updn_CellSize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnHelp
        '
        Me.btnHelp.Image = My.Resources.Resources.Help
        Me.btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnHelp.Location = New System.Drawing.Point(23, 360)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(61, 23)
        Me.btnHelp.TabIndex = 55
        Me.btnHelp.Text = "Help"
        Me.btnHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'btn_Cancel
        '
        Me.btn_Cancel.Location = New System.Drawing.Point(427, 360)
        Me.btn_Cancel.Name = "btn_Cancel"
        Me.btn_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.btn_Cancel.TabIndex = 54
        Me.btn_Cancel.Text = "Cancel"
        Me.btn_Cancel.UseVisualStyleBackColor = True
        '
        'btn_Run
        '
        Me.btn_Run.Location = New System.Drawing.Point(331, 360)
        Me.btn_Run.Name = "btn_Run"
        Me.btn_Run.Size = New System.Drawing.Size(75, 23)
        Me.btn_Run.TabIndex = 53
        Me.btn_Run.Text = "Run Tool"
        Me.btn_Run.UseVisualStyleBackColor = True
        '
        'btn_OutputRaster
        '
        Me.btn_OutputRaster.Image = CType(resources.GetObject("btn_OutputRaster.Image"), System.Drawing.Image)
        Me.btn_OutputRaster.Location = New System.Drawing.Point(429, 67)
        Me.btn_OutputRaster.Name = "btn_OutputRaster"
        Me.btn_OutputRaster.Size = New System.Drawing.Size(26, 23)
        Me.btn_OutputRaster.TabIndex = 51
        Me.btn_OutputRaster.UseVisualStyleBackColor = True
        '
        'txtBox_ExtentShp
        '
        Me.txtBox_ExtentShp.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_ExtentShp.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_ExtentShp.Location = New System.Drawing.Point(169, 119)
        Me.txtBox_ExtentShp.Name = "txtBox_ExtentShp"
        Me.txtBox_ExtentShp.ReadOnly = True
        Me.txtBox_ExtentShp.Size = New System.Drawing.Size(266, 20)
        Me.txtBox_ExtentShp.TabIndex = 50
        '
        'btn_ExtentShp
        '
        Me.btn_ExtentShp.Image = My.Resources.Resources.BrowseFolder
        Me.btn_ExtentShp.Location = New System.Drawing.Point(451, 117)
        Me.btn_ExtentShp.Name = "btn_ExtentShp"
        Me.btn_ExtentShp.Size = New System.Drawing.Size(26, 23)
        Me.btn_ExtentShp.TabIndex = 49
        Me.btn_ExtentShp.UseVisualStyleBackColor = True
        '
        'txtBox_PointCloudShp
        '
        Me.txtBox_PointCloudShp.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_PointCloudShp.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_PointCloudShp.Location = New System.Drawing.Point(169, 38)
        Me.txtBox_PointCloudShp.Name = "txtBox_PointCloudShp"
        Me.txtBox_PointCloudShp.ReadOnly = True
        Me.txtBox_PointCloudShp.Size = New System.Drawing.Size(266, 20)
        Me.txtBox_PointCloudShp.TabIndex = 48
        '
        'btn_PointCloudShp
        '
        Me.btn_PointCloudShp.Image = My.Resources.Resources.BrowseFolder
        Me.btn_PointCloudShp.Location = New System.Drawing.Point(451, 38)
        Me.btn_PointCloudShp.Name = "btn_PointCloudShp"
        Me.btn_PointCloudShp.Size = New System.Drawing.Size(26, 23)
        Me.btn_PointCloudShp.TabIndex = 47
        Me.btn_PointCloudShp.UseVisualStyleBackColor = True
        '
        'txtBox_SpatialReference
        '
        Me.txtBox_SpatialReference.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_SpatialReference.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_SpatialReference.Location = New System.Drawing.Point(169, 202)
        Me.txtBox_SpatialReference.Name = "txtBox_SpatialReference"
        Me.txtBox_SpatialReference.ReadOnly = True
        Me.txtBox_SpatialReference.Size = New System.Drawing.Size(266, 20)
        Me.txtBox_SpatialReference.TabIndex = 46
        '
        'btn_SpatialReference
        '
        Me.btn_SpatialReference.Image = My.Resources.Resources.BrowseFolder
        Me.btn_SpatialReference.Location = New System.Drawing.Point(451, 199)
        Me.btn_SpatialReference.Name = "btn_SpatialReference"
        Me.btn_SpatialReference.Size = New System.Drawing.Size(26, 23)
        Me.btn_SpatialReference.TabIndex = 45
        Me.btn_SpatialReference.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.cmb_ZField)
        Me.GroupBox1.Controls.Add(Me.btnInputRaster)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.txtInputRaster)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(22, 13)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(480, 229)
        Me.GroupBox1.TabIndex = 56
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Inputs"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(-2, 61)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(144, 13)
        Me.Label6.TabIndex = 61
        Me.Label6.Text = "Field to Create Surface From:"
        '
        'cmb_ZField
        '
        Me.cmb_ZField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmb_ZField.FormattingEnabled = True
        Me.cmb_ZField.Location = New System.Drawing.Point(150, 58)
        Me.cmb_ZField.Name = "cmb_ZField"
        Me.cmb_ZField.Size = New System.Drawing.Size(120, 21)
        Me.cmb_ZField.TabIndex = 60
        '
        'btnInputRaster
        '
        Me.btnInputRaster.Image = My.Resources.Resources.BrowseFolder
        Me.btnInputRaster.Location = New System.Drawing.Point(429, 147)
        Me.btnInputRaster.Name = "btnInputRaster"
        Me.btnInputRaster.Size = New System.Drawing.Size(26, 23)
        Me.btnInputRaster.TabIndex = 58
        Me.btnInputRaster.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(68, 153)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(68, 13)
        Me.Label5.TabIndex = 59
        Me.Label5.Text = "Input Raster:"
        '
        'txtInputRaster
        '
        Me.txtInputRaster.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtInputRaster.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtInputRaster.Location = New System.Drawing.Point(147, 150)
        Me.txtInputRaster.Name = "txtInputRaster"
        Me.txtInputRaster.ReadOnly = True
        Me.txtInputRaster.Size = New System.Drawing.Size(266, 20)
        Me.txtInputRaster.TabIndex = 58
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(22, 192)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(114, 13)
        Me.Label3.TabIndex = 41
        Me.Label3.Text = "Spatial Reference File:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(55, 106)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(81, 13)
        Me.Label2.TabIndex = 40
        Me.Label2.Text = "Extent Polygon:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(25, 25)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(111, 13)
        Me.Label1.TabIndex = 39
        Me.Label1.Text = "Point Cloud Shapefile:"
        '
        'label_CellSizeUnits
        '
        Me.label_CellSizeUnits.AutoSize = True
        Me.label_CellSizeUnits.Location = New System.Drawing.Point(209, 34)
        Me.label_CellSizeUnits.Name = "label_CellSizeUnits"
        Me.label_CellSizeUnits.Size = New System.Drawing.Size(31, 13)
        Me.label_CellSizeUnits.TabIndex = 33
        Me.label_CellSizeUnits.Text = "Units"
        '
        'lbl_CellSize
        '
        Me.lbl_CellSize.AutoSize = True
        Me.lbl_CellSize.Location = New System.Drawing.Point(20, 34)
        Me.lbl_CellSize.Name = "lbl_CellSize"
        Me.lbl_CellSize.Size = New System.Drawing.Size(116, 13)
        Me.lbl_CellSize.TabIndex = 32
        Me.lbl_CellSize.Text = "Output Raster Cell Size"
        '
        'updn_CellSize
        '
        Me.updn_CellSize.DecimalPlaces = 3
        Me.updn_CellSize.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.updn_CellSize.Location = New System.Drawing.Point(142, 32)
        Me.updn_CellSize.Minimum = New Decimal(New Integer() {1, 0, 0, 196608})
        Me.updn_CellSize.Name = "updn_CellSize"
        Me.updn_CellSize.Size = New System.Drawing.Size(61, 20)
        Me.updn_CellSize.TabIndex = 31
        Me.updn_CellSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.updn_CellSize.ThousandsSeparator = True
        Me.updn_CellSize.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.label_CellSizeUnits)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.txtBox_OutputDEM)
        Me.GroupBox2.Controls.Add(Me.lbl_CellSize)
        Me.GroupBox2.Controls.Add(Me.btn_OutputRaster)
        Me.GroupBox2.Controls.Add(Me.updn_CellSize)
        Me.GroupBox2.Location = New System.Drawing.Point(22, 248)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(480, 106)
        Me.GroupBox2.TabIndex = 57
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Output"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(9, 74)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(127, 13)
        Me.Label4.TabIndex = 42
        Me.Label4.Text = "Interpolation Error Raster:"
        '
        'txtBox_OutputDEM
        '
        Me.txtBox_OutputDEM.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_OutputDEM.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_OutputDEM.Location = New System.Drawing.Point(142, 70)
        Me.txtBox_OutputDEM.Name = "txtBox_OutputDEM"
        Me.txtBox_OutputDEM.ReadOnly = True
        Me.txtBox_OutputDEM.Size = New System.Drawing.Size(266, 20)
        Me.txtBox_OutputDEM.TabIndex = 30
        '
        'frm_InterploationError
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(525, 395)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btn_Cancel)
        Me.Controls.Add(Me.btn_Run)
        Me.Controls.Add(Me.txtBox_ExtentShp)
        Me.Controls.Add(Me.btn_ExtentShp)
        Me.Controls.Add(Me.txtBox_PointCloudShp)
        Me.Controls.Add(Me.btn_PointCloudShp)
        Me.Controls.Add(Me.txtBox_SpatialReference)
        Me.Controls.Add(Me.btn_SpatialReference)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBox2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frm_InterploationError"
        Me.Text = "Interpolation Error"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.updn_CellSize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents btn_Cancel As System.Windows.Forms.Button
    Friend WithEvents btn_Run As System.Windows.Forms.Button
    Friend WithEvents btn_OutputRaster As System.Windows.Forms.Button
    Friend WithEvents txtBox_ExtentShp As System.Windows.Forms.TextBox
    Friend WithEvents btn_ExtentShp As System.Windows.Forms.Button
    Friend WithEvents txtBox_PointCloudShp As System.Windows.Forms.TextBox
    Friend WithEvents btn_PointCloudShp As System.Windows.Forms.Button
    Friend WithEvents txtBox_SpatialReference As System.Windows.Forms.TextBox
    Friend WithEvents btn_SpatialReference As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents label_CellSizeUnits As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lbl_CellSize As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents updn_CellSize As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtBox_OutputDEM As System.Windows.Forms.TextBox
    Friend WithEvents btnInputRaster As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtInputRaster As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cmb_ZField As System.Windows.Forms.ComboBox
End Class
