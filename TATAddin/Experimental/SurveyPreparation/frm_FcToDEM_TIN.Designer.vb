<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_FcToDEM_TIN
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frm_FcToDEM_TIN))
        Me.btn_Help = New System.Windows.Forms.Button()
        Me.btn_Cancel = New System.Windows.Forms.Button()
        Me.btn_Run = New System.Windows.Forms.Button()
        Me.txtBox_ExtentShp = New System.Windows.Forms.TextBox()
        Me.btn_ExtentShp = New System.Windows.Forms.Button()
        Me.txtBox_PointCloudShp = New System.Windows.Forms.TextBox()
        Me.btn_PointCloudShp = New System.Windows.Forms.Button()
        Me.btn_SpatialReference = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblSpatialRef = New System.Windows.Forms.Label()
        Me.cmb_ZField = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtBox_SpatialReference = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.bxInterpolationMethod = New System.Windows.Forms.GroupBox()
        Me.rbNaturalNeighbors = New System.Windows.Forms.RadioButton()
        Me.rbLinear = New System.Windows.Forms.RadioButton()
        Me.grbTriangulationMethod = New System.Windows.Forms.GroupBox()
        Me.rbDelaunay = New System.Windows.Forms.RadioButton()
        Me.rbContrainedDelaunay = New System.Windows.Forms.RadioButton()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtBox_OutputDEM = New System.Windows.Forms.TextBox()
        Me.btn_OutputDEM = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.chk_CreateDEM = New System.Windows.Forms.CheckBox()
        Me.chk_DeleteTIN = New System.Windows.Forms.CheckBox()
        Me.label_CellSizeUnits = New System.Windows.Forms.Label()
        Me.updn_CellSize = New System.Windows.Forms.NumericUpDown()
        Me.lbl_CellSize = New System.Windows.Forms.Label()
        Me.txtBox_OutputTIN = New System.Windows.Forms.TextBox()
        Me.btn_OutputTIN = New System.Windows.Forms.Button()
        Me.ttip = New System.Windows.Forms.ToolTip(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.bxInterpolationMethod.SuspendLayout()
        Me.grbTriangulationMethod.SuspendLayout()
        CType(Me.updn_CellSize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btn_Help
        '
        Me.btn_Help.Image = My.Resources.Resources.Help
        Me.btn_Help.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btn_Help.Location = New System.Drawing.Point(20, 395)
        Me.btn_Help.Name = "btn_Help"
        Me.btn_Help.Size = New System.Drawing.Size(61, 23)
        Me.btn_Help.TabIndex = 32
        Me.btn_Help.Text = "Help"
        Me.btn_Help.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btn_Help.UseVisualStyleBackColor = True
        '
        'btn_Cancel
        '
        Me.btn_Cancel.Location = New System.Drawing.Point(437, 395)
        Me.btn_Cancel.Name = "btn_Cancel"
        Me.btn_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.btn_Cancel.TabIndex = 31
        Me.btn_Cancel.Text = "Cancel"
        Me.btn_Cancel.UseVisualStyleBackColor = True
        '
        'btn_Run
        '
        Me.btn_Run.Location = New System.Drawing.Point(340, 395)
        Me.btn_Run.Name = "btn_Run"
        Me.btn_Run.Size = New System.Drawing.Size(75, 23)
        Me.btn_Run.TabIndex = 30
        Me.btn_Run.Text = "Run Tool"
        Me.btn_Run.UseVisualStyleBackColor = True
        '
        'txtBox_ExtentShp
        '
        Me.txtBox_ExtentShp.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_ExtentShp.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_ExtentShp.Location = New System.Drawing.Point(175, 112)
        Me.txtBox_ExtentShp.Name = "txtBox_ExtentShp"
        Me.txtBox_ExtentShp.ReadOnly = True
        Me.txtBox_ExtentShp.Size = New System.Drawing.Size(289, 20)
        Me.txtBox_ExtentShp.TabIndex = 28
        '
        'btn_ExtentShp
        '
        Me.btn_ExtentShp.Image = My.Resources.Resources.BrowseFolder
        Me.btn_ExtentShp.Location = New System.Drawing.Point(470, 110)
        Me.btn_ExtentShp.Name = "btn_ExtentShp"
        Me.btn_ExtentShp.Size = New System.Drawing.Size(27, 23)
        Me.btn_ExtentShp.TabIndex = 27
        Me.btn_ExtentShp.UseVisualStyleBackColor = True
        '
        'txtBox_PointCloudShp
        '
        Me.txtBox_PointCloudShp.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_PointCloudShp.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_PointCloudShp.Location = New System.Drawing.Point(175, 40)
        Me.txtBox_PointCloudShp.Name = "txtBox_PointCloudShp"
        Me.txtBox_PointCloudShp.ReadOnly = True
        Me.txtBox_PointCloudShp.Size = New System.Drawing.Size(289, 20)
        Me.txtBox_PointCloudShp.TabIndex = 26
        '
        'btn_PointCloudShp
        '
        Me.btn_PointCloudShp.Image = My.Resources.Resources.BrowseFolder
        Me.btn_PointCloudShp.Location = New System.Drawing.Point(470, 40)
        Me.btn_PointCloudShp.Name = "btn_PointCloudShp"
        Me.btn_PointCloudShp.Size = New System.Drawing.Size(27, 23)
        Me.btn_PointCloudShp.TabIndex = 25
        Me.btn_PointCloudShp.UseVisualStyleBackColor = True
        '
        'btn_SpatialReference
        '
        Me.btn_SpatialReference.Image = My.Resources.Resources.BrowseFolder
        Me.btn_SpatialReference.Location = New System.Drawing.Point(471, 152)
        Me.btn_SpatialReference.Name = "btn_SpatialReference"
        Me.btn_SpatialReference.Size = New System.Drawing.Size(26, 23)
        Me.btn_SpatialReference.TabIndex = 24
        Me.btn_SpatialReference.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.lblSpatialRef)
        Me.GroupBox1.Controls.Add(Me.cmb_ZField)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.txtBox_SpatialReference)
        Me.GroupBox1.Location = New System.Drawing.Point(20, 13)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(492, 176)
        Me.GroupBox1.TabIndex = 33
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Inputs"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(3, 65)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(144, 13)
        Me.Label6.TabIndex = 20
        Me.Label6.Text = "Field to Create Surface From:"
        '
        'lblSpatialRef
        '
        Me.lblSpatialRef.AutoSize = True
        Me.lblSpatialRef.Location = New System.Drawing.Point(3, 142)
        Me.lblSpatialRef.Name = "lblSpatialRef"
        Me.lblSpatialRef.Size = New System.Drawing.Size(114, 13)
        Me.lblSpatialRef.TabIndex = 6
        Me.lblSpatialRef.Text = "Spatial Reference File:"
        '
        'cmb_ZField
        '
        Me.cmb_ZField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmb_ZField.FormattingEnabled = True
        Me.cmb_ZField.Location = New System.Drawing.Point(155, 62)
        Me.cmb_ZField.Name = "cmb_ZField"
        Me.cmb_ZField.Size = New System.Drawing.Size(120, 21)
        Me.cmb_ZField.TabIndex = 19
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 102)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(81, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Extent Polygon:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(2, 30)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(111, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Point Cloud Shapefile:"
        '
        'txtBox_SpatialReference
        '
        Me.txtBox_SpatialReference.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_SpatialReference.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_SpatialReference.Location = New System.Drawing.Point(155, 139)
        Me.txtBox_SpatialReference.Name = "txtBox_SpatialReference"
        Me.txtBox_SpatialReference.ReadOnly = True
        Me.txtBox_SpatialReference.Size = New System.Drawing.Size(289, 20)
        Me.txtBox_SpatialReference.TabIndex = 3
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.bxInterpolationMethod)
        Me.GroupBox2.Controls.Add(Me.grbTriangulationMethod)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.txtBox_OutputDEM)
        Me.GroupBox2.Controls.Add(Me.btn_OutputDEM)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.chk_CreateDEM)
        Me.GroupBox2.Controls.Add(Me.chk_DeleteTIN)
        Me.GroupBox2.Controls.Add(Me.label_CellSizeUnits)
        Me.GroupBox2.Controls.Add(Me.updn_CellSize)
        Me.GroupBox2.Controls.Add(Me.lbl_CellSize)
        Me.GroupBox2.Controls.Add(Me.txtBox_OutputTIN)
        Me.GroupBox2.Controls.Add(Me.btn_OutputTIN)
        Me.GroupBox2.Location = New System.Drawing.Point(20, 212)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(492, 164)
        Me.GroupBox2.TabIndex = 34
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Output"
        '
        'bxInterpolationMethod
        '
        Me.bxInterpolationMethod.Controls.Add(Me.rbNaturalNeighbors)
        Me.bxInterpolationMethod.Controls.Add(Me.rbLinear)
        Me.bxInterpolationMethod.Location = New System.Drawing.Point(344, 44)
        Me.bxInterpolationMethod.Margin = New System.Windows.Forms.Padding(2)
        Me.bxInterpolationMethod.Name = "bxInterpolationMethod"
        Me.bxInterpolationMethod.Padding = New System.Windows.Forms.Padding(2)
        Me.bxInterpolationMethod.Size = New System.Drawing.Size(139, 63)
        Me.bxInterpolationMethod.TabIndex = 22
        Me.bxInterpolationMethod.TabStop = False
        Me.bxInterpolationMethod.Text = "Interpolation Method"
        '
        'rbNaturalNeighbors
        '
        Me.rbNaturalNeighbors.AutoSize = True
        Me.rbNaturalNeighbors.Location = New System.Drawing.Point(5, 41)
        Me.rbNaturalNeighbors.Margin = New System.Windows.Forms.Padding(2)
        Me.rbNaturalNeighbors.Name = "rbNaturalNeighbors"
        Me.rbNaturalNeighbors.Size = New System.Drawing.Size(110, 17)
        Me.rbNaturalNeighbors.TabIndex = 1
        Me.rbNaturalNeighbors.Text = "Natural Neighbors"
        Me.rbNaturalNeighbors.UseVisualStyleBackColor = True
        '
        'rbLinear
        '
        Me.rbLinear.AutoSize = True
        Me.rbLinear.Checked = True
        Me.rbLinear.Location = New System.Drawing.Point(5, 18)
        Me.rbLinear.Margin = New System.Windows.Forms.Padding(2)
        Me.rbLinear.Name = "rbLinear"
        Me.rbLinear.Size = New System.Drawing.Size(54, 17)
        Me.rbLinear.TabIndex = 0
        Me.rbLinear.TabStop = True
        Me.rbLinear.Text = "Linear"
        Me.rbLinear.UseVisualStyleBackColor = True
        '
        'grbTriangulationMethod
        '
        Me.grbTriangulationMethod.Controls.Add(Me.rbDelaunay)
        Me.grbTriangulationMethod.Controls.Add(Me.rbContrainedDelaunay)
        Me.grbTriangulationMethod.Location = New System.Drawing.Point(200, 44)
        Me.grbTriangulationMethod.Margin = New System.Windows.Forms.Padding(2)
        Me.grbTriangulationMethod.Name = "grbTriangulationMethod"
        Me.grbTriangulationMethod.Padding = New System.Windows.Forms.Padding(2)
        Me.grbTriangulationMethod.Size = New System.Drawing.Size(139, 63)
        Me.grbTriangulationMethod.TabIndex = 21
        Me.grbTriangulationMethod.TabStop = False
        Me.grbTriangulationMethod.Text = "Triangulation Method"
        '
        'rbDelaunay
        '
        Me.rbDelaunay.AutoSize = True
        Me.rbDelaunay.Location = New System.Drawing.Point(5, 41)
        Me.rbDelaunay.Margin = New System.Windows.Forms.Padding(2)
        Me.rbDelaunay.Name = "rbDelaunay"
        Me.rbDelaunay.Size = New System.Drawing.Size(70, 17)
        Me.rbDelaunay.TabIndex = 1
        Me.rbDelaunay.TabStop = True
        Me.rbDelaunay.Text = "Delaunay"
        Me.rbDelaunay.UseVisualStyleBackColor = True
        '
        'rbContrainedDelaunay
        '
        Me.rbContrainedDelaunay.AutoSize = True
        Me.rbContrainedDelaunay.Location = New System.Drawing.Point(5, 18)
        Me.rbContrainedDelaunay.Margin = New System.Windows.Forms.Padding(2)
        Me.rbContrainedDelaunay.Name = "rbContrainedDelaunay"
        Me.rbContrainedDelaunay.Size = New System.Drawing.Size(124, 17)
        Me.rbContrainedDelaunay.TabIndex = 0
        Me.rbContrainedDelaunay.TabStop = True
        Me.rbContrainedDelaunay.Text = "Contrained Delaunay"
        Me.rbContrainedDelaunay.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(44, 125)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(69, 13)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Output DEM:"
        '
        'txtBox_OutputDEM
        '
        Me.txtBox_OutputDEM.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_OutputDEM.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_OutputDEM.Enabled = False
        Me.txtBox_OutputDEM.Location = New System.Drawing.Point(119, 122)
        Me.txtBox_OutputDEM.Name = "txtBox_OutputDEM"
        Me.txtBox_OutputDEM.ReadOnly = True
        Me.txtBox_OutputDEM.Size = New System.Drawing.Size(324, 20)
        Me.txtBox_OutputDEM.TabIndex = 10
        '
        'btn_OutputDEM
        '
        Me.btn_OutputDEM.Enabled = False
        Me.btn_OutputDEM.Image = My.Resources.Resources.Save
        Me.btn_OutputDEM.Location = New System.Drawing.Point(450, 120)
        Me.btn_OutputDEM.Name = "btn_OutputDEM"
        Me.btn_OutputDEM.Size = New System.Drawing.Size(27, 23)
        Me.btn_OutputDEM.TabIndex = 9
        Me.btn_OutputDEM.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(8, 23)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(107, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Output TIN Location:"
        '
        'chk_CreateDEM
        '
        Me.chk_CreateDEM.AutoSize = True
        Me.chk_CreateDEM.Location = New System.Drawing.Point(20, 45)
        Me.chk_CreateDEM.Name = "chk_CreateDEM"
        Me.chk_CreateDEM.Size = New System.Drawing.Size(84, 17)
        Me.chk_CreateDEM.TabIndex = 8
        Me.chk_CreateDEM.Text = "Create DEM"
        Me.chk_CreateDEM.UseVisualStyleBackColor = True
        '
        'chk_DeleteTIN
        '
        Me.chk_DeleteTIN.AutoSize = True
        Me.chk_DeleteTIN.Enabled = False
        Me.chk_DeleteTIN.Location = New System.Drawing.Point(119, 44)
        Me.chk_DeleteTIN.Name = "chk_DeleteTIN"
        Me.chk_DeleteTIN.Size = New System.Drawing.Size(78, 17)
        Me.chk_DeleteTIN.TabIndex = 13
        Me.chk_DeleteTIN.Text = "Delete TIN"
        Me.chk_DeleteTIN.UseVisualStyleBackColor = True
        '
        'label_CellSizeUnits
        '
        Me.label_CellSizeUnits.AutoSize = True
        Me.label_CellSizeUnits.Location = New System.Drawing.Point(138, 91)
        Me.label_CellSizeUnits.Name = "label_CellSizeUnits"
        Me.label_CellSizeUnits.Size = New System.Drawing.Size(31, 13)
        Me.label_CellSizeUnits.TabIndex = 20
        Me.label_CellSizeUnits.Text = "Units"
        '
        'updn_CellSize
        '
        Me.updn_CellSize.DecimalPlaces = 4
        Me.updn_CellSize.Enabled = False
        Me.updn_CellSize.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.updn_CellSize.Location = New System.Drawing.Point(70, 89)
        Me.updn_CellSize.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.updn_CellSize.Minimum = New Decimal(New Integer() {1, 0, 0, 262144})
        Me.updn_CellSize.Name = "updn_CellSize"
        Me.updn_CellSize.Size = New System.Drawing.Size(62, 20)
        Me.updn_CellSize.TabIndex = 11
        Me.updn_CellSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.updn_CellSize.ThousandsSeparator = True
        Me.updn_CellSize.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'lbl_CellSize
        '
        Me.lbl_CellSize.AutoSize = True
        Me.lbl_CellSize.Location = New System.Drawing.Point(17, 91)
        Me.lbl_CellSize.Name = "lbl_CellSize"
        Me.lbl_CellSize.Size = New System.Drawing.Size(47, 13)
        Me.lbl_CellSize.TabIndex = 12
        Me.lbl_CellSize.Text = "Cell Size"
        '
        'txtBox_OutputTIN
        '
        Me.txtBox_OutputTIN.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_OutputTIN.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_OutputTIN.Location = New System.Drawing.Point(119, 20)
        Me.txtBox_OutputTIN.Name = "txtBox_OutputTIN"
        Me.txtBox_OutputTIN.ReadOnly = True
        Me.txtBox_OutputTIN.Size = New System.Drawing.Size(325, 20)
        Me.txtBox_OutputTIN.TabIndex = 1
        '
        'btn_OutputTIN
        '
        Me.btn_OutputTIN.Image = My.Resources.Resources.Save
        Me.btn_OutputTIN.Location = New System.Drawing.Point(451, 20)
        Me.btn_OutputTIN.Name = "btn_OutputTIN"
        Me.btn_OutputTIN.Size = New System.Drawing.Size(26, 23)
        Me.btn_OutputTIN.TabIndex = 0
        Me.btn_OutputTIN.UseVisualStyleBackColor = True
        '
        'frm_FcToDEM_TIN
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(533, 430)
        Me.Controls.Add(Me.btn_Help)
        Me.Controls.Add(Me.btn_Cancel)
        Me.Controls.Add(Me.btn_Run)
        Me.Controls.Add(Me.txtBox_ExtentShp)
        Me.Controls.Add(Me.btn_ExtentShp)
        Me.Controls.Add(Me.txtBox_PointCloudShp)
        Me.Controls.Add(Me.btn_PointCloudShp)
        Me.Controls.Add(Me.btn_SpatialReference)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBox2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frm_FcToDEM_TIN"
        Me.Text = "Point Cloud Shapefile to TIN and/or DEM"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.bxInterpolationMethod.ResumeLayout(False)
        Me.bxInterpolationMethod.PerformLayout()
        Me.grbTriangulationMethod.ResumeLayout(False)
        Me.grbTriangulationMethod.PerformLayout()
        CType(Me.updn_CellSize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btn_Help As System.Windows.Forms.Button
    Friend WithEvents btn_Cancel As System.Windows.Forms.Button
    Friend WithEvents btn_Run As System.Windows.Forms.Button
    Friend WithEvents txtBox_ExtentShp As System.Windows.Forms.TextBox
    Friend WithEvents btn_ExtentShp As System.Windows.Forms.Button
    Friend WithEvents txtBox_PointCloudShp As System.Windows.Forms.TextBox
    Friend WithEvents btn_PointCloudShp As System.Windows.Forms.Button
    Friend WithEvents btn_SpatialReference As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblSpatialRef As System.Windows.Forms.Label
    Friend WithEvents cmb_ZField As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtBox_SpatialReference As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtBox_OutputDEM As System.Windows.Forms.TextBox
    Friend WithEvents btn_OutputDEM As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents chk_CreateDEM As System.Windows.Forms.CheckBox
    Friend WithEvents chk_DeleteTIN As System.Windows.Forms.CheckBox
    Friend WithEvents label_CellSizeUnits As System.Windows.Forms.Label
    Friend WithEvents updn_CellSize As System.Windows.Forms.NumericUpDown
    Friend WithEvents lbl_CellSize As System.Windows.Forms.Label
    Friend WithEvents txtBox_OutputTIN As System.Windows.Forms.TextBox
    Friend WithEvents btn_OutputTIN As System.Windows.Forms.Button
    Friend WithEvents ttip As System.Windows.Forms.ToolTip
    Friend WithEvents grbTriangulationMethod As System.Windows.Forms.GroupBox
    Friend WithEvents rbDelaunay As System.Windows.Forms.RadioButton
    Friend WithEvents rbContrainedDelaunay As System.Windows.Forms.RadioButton
    Friend WithEvents bxInterpolationMethod As System.Windows.Forms.GroupBox
    Friend WithEvents rbNaturalNeighbors As System.Windows.Forms.RadioButton
    Friend WithEvents rbLinear As System.Windows.Forms.RadioButton
End Class
