<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_CoincidentPointsShp
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frm_CoincidentPointsShp))
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.btn_Cancel = New System.Windows.Forms.Button()
        Me.btn_Run = New System.Windows.Forms.Button()
        Me.txtBox_RawPointCloud = New System.Windows.Forms.TextBox()
        Me.btn_RawPointCloud = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtBox_ExtentShp = New System.Windows.Forms.TextBox()
        Me.btn_ExtentShp = New System.Windows.Forms.Button()
        Me.btnPreviewFile = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.NumericUpDown_Precision = New System.Windows.Forms.NumericUpDown()
        Me.ChkBox_SetPrecision = New System.Windows.Forms.CheckBox()
        Me.Cmb_Box_Seperator = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btn_SpatialReference = New System.Windows.Forms.Button()
        Me.txtBox_SpatialReference = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtBox_OutputShp = New System.Windows.Forms.TextBox()
        Me.btn_OutputShp = New System.Windows.Forms.Button()
        Me.ttip = New System.Windows.Forms.ToolTip(Me.components)
        Me.GroupBox1.SuspendLayout()
        CType(Me.NumericUpDown_Precision, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnHelp
        '
        Me.btnHelp.Image = My.Resources.Resources.Help
        Me.btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnHelp.Location = New System.Drawing.Point(26, 340)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(60, 23)
        Me.btnHelp.TabIndex = 45
        Me.btnHelp.Text = "Help"
        Me.btnHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'btn_Cancel
        '
        Me.btn_Cancel.Location = New System.Drawing.Point(466, 340)
        Me.btn_Cancel.Name = "btn_Cancel"
        Me.btn_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.btn_Cancel.TabIndex = 44
        Me.btn_Cancel.Text = "Cancel"
        Me.btn_Cancel.UseVisualStyleBackColor = True
        '
        'btn_Run
        '
        Me.btn_Run.Location = New System.Drawing.Point(369, 340)
        Me.btn_Run.Name = "btn_Run"
        Me.btn_Run.Size = New System.Drawing.Size(75, 23)
        Me.btn_Run.TabIndex = 43
        Me.btn_Run.Text = "Run Tool"
        Me.btn_Run.UseVisualStyleBackColor = True
        '
        'txtBox_RawPointCloud
        '
        Me.txtBox_RawPointCloud.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_RawPointCloud.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_RawPointCloud.Location = New System.Drawing.Point(177, 26)
        Me.txtBox_RawPointCloud.Name = "txtBox_RawPointCloud"
        Me.txtBox_RawPointCloud.ReadOnly = True
        Me.txtBox_RawPointCloud.Size = New System.Drawing.Size(295, 20)
        Me.txtBox_RawPointCloud.TabIndex = 41
        '
        'btn_RawPointCloud
        '
        Me.btn_RawPointCloud.Image = CType(resources.GetObject("btn_RawPointCloud.Image"), System.Drawing.Image)
        Me.btn_RawPointCloud.Location = New System.Drawing.Point(490, 26)
        Me.btn_RawPointCloud.Name = "btn_RawPointCloud"
        Me.btn_RawPointCloud.Size = New System.Drawing.Size(32, 23)
        Me.btn_RawPointCloud.TabIndex = 40
        Me.btn_RawPointCloud.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.txtBox_ExtentShp)
        Me.GroupBox1.Controls.Add(Me.btn_ExtentShp)
        Me.GroupBox1.Controls.Add(Me.btnPreviewFile)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.NumericUpDown_Precision)
        Me.GroupBox1.Controls.Add(Me.ChkBox_SetPrecision)
        Me.GroupBox1.Controls.Add(Me.Cmb_Box_Seperator)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.btn_SpatialReference)
        Me.GroupBox1.Controls.Add(Me.txtBox_SpatialReference)
        Me.GroupBox1.Location = New System.Drawing.Point(26, 11)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(515, 221)
        Me.GroupBox1.TabIndex = 46
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Inputs"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(6, 148)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(119, 13)
        Me.Label6.TabIndex = 42
        Me.Label6.Text = "Filter Polygon (optional):"
        '
        'txtBox_ExtentShp
        '
        Me.txtBox_ExtentShp.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_ExtentShp.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_ExtentShp.Enabled = False
        Me.txtBox_ExtentShp.Location = New System.Drawing.Point(151, 145)
        Me.txtBox_ExtentShp.Name = "txtBox_ExtentShp"
        Me.txtBox_ExtentShp.ReadOnly = True
        Me.txtBox_ExtentShp.Size = New System.Drawing.Size(295, 20)
        Me.txtBox_ExtentShp.TabIndex = 44
        '
        'btn_ExtentShp
        '
        Me.btn_ExtentShp.Enabled = False
        Me.btn_ExtentShp.Image = My.Resources.Resources.BrowseFolder
        Me.btn_ExtentShp.Location = New System.Drawing.Point(464, 145)
        Me.btn_ExtentShp.Name = "btn_ExtentShp"
        Me.btn_ExtentShp.Size = New System.Drawing.Size(32, 23)
        Me.btn_ExtentShp.TabIndex = 43
        Me.btn_ExtentShp.UseVisualStyleBackColor = True
        '
        'btnPreviewFile
        '
        Me.btnPreviewFile.Location = New System.Drawing.Point(370, 92)
        Me.btnPreviewFile.Name = "btnPreviewFile"
        Me.btnPreviewFile.Size = New System.Drawing.Size(126, 23)
        Me.btnPreviewFile.TabIndex = 41
        Me.btnPreviewFile.Text = "Preview First Lines"
        Me.btnPreviewFile.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(197, 52)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(53, 13)
        Me.Label5.TabIndex = 40
        Me.Label5.Text = "Precision:"
        '
        'NumericUpDown_Precision
        '
        Me.NumericUpDown_Precision.Enabled = False
        Me.NumericUpDown_Precision.Location = New System.Drawing.Point(267, 51)
        Me.NumericUpDown_Precision.Name = "NumericUpDown_Precision"
        Me.NumericUpDown_Precision.Size = New System.Drawing.Size(45, 20)
        Me.NumericUpDown_Precision.TabIndex = 39
        '
        'ChkBox_SetPrecision
        '
        Me.ChkBox_SetPrecision.AutoSize = True
        Me.ChkBox_SetPrecision.Location = New System.Drawing.Point(75, 51)
        Me.ChkBox_SetPrecision.Name = "ChkBox_SetPrecision"
        Me.ChkBox_SetPrecision.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.ChkBox_SetPrecision.Size = New System.Drawing.Size(88, 17)
        Me.ChkBox_SetPrecision.TabIndex = 38
        Me.ChkBox_SetPrecision.Text = "Set Precision"
        Me.ChkBox_SetPrecision.UseVisualStyleBackColor = True
        '
        'Cmb_Box_Seperator
        '
        Me.Cmb_Box_Seperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Cmb_Box_Seperator.FormattingEnabled = True
        Me.Cmb_Box_Seperator.Items.AddRange(New Object() {"Comma", "Period", "Semi-Colon", "Colon", "Space", "Tab"})
        Me.Cmb_Box_Seperator.Location = New System.Drawing.Point(200, 94)
        Me.Cmb_Box_Seperator.Name = "Cmb_Box_Seperator"
        Me.Cmb_Box_Seperator.Size = New System.Drawing.Size(121, 21)
        Me.Cmb_Box_Seperator.TabIndex = 37
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(56, 22)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 13)
        Me.Label2.TabIndex = 35
        Me.Label2.Text = "Raw Point Cloud:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(4, 188)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(141, 13)
        Me.Label3.TabIndex = 36
        Me.Label3.Text = "Spatial Reference (optional):"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 97)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(182, 13)
        Me.Label1.TabIndex = 25
        Me.Label1.Text = "Input the separator of point cloud file:"
        '
        'btn_SpatialReference
        '
        Me.btn_SpatialReference.Image = CType(resources.GetObject("btn_SpatialReference.Image"), System.Drawing.Image)
        Me.btn_SpatialReference.Location = New System.Drawing.Point(464, 183)
        Me.btn_SpatialReference.Name = "btn_SpatialReference"
        Me.btn_SpatialReference.Size = New System.Drawing.Size(32, 23)
        Me.btn_SpatialReference.TabIndex = 28
        Me.btn_SpatialReference.UseVisualStyleBackColor = True
        '
        'txtBox_SpatialReference
        '
        Me.txtBox_SpatialReference.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_SpatialReference.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_SpatialReference.Location = New System.Drawing.Point(151, 185)
        Me.txtBox_SpatialReference.Name = "txtBox_SpatialReference"
        Me.txtBox_SpatialReference.ReadOnly = True
        Me.txtBox_SpatialReference.Size = New System.Drawing.Size(295, 20)
        Me.txtBox_SpatialReference.TabIndex = 29
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.txtBox_OutputShp)
        Me.GroupBox2.Controls.Add(Me.btn_OutputShp)
        Me.GroupBox2.Location = New System.Drawing.Point(23, 238)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(518, 85)
        Me.GroupBox2.TabIndex = 47
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Output"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(59, 30)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(89, 13)
        Me.Label4.TabIndex = 37
        Me.Label4.Text = "Output Shapefile:"
        '
        'txtBox_OutputShp
        '
        Me.txtBox_OutputShp.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_OutputShp.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_OutputShp.Location = New System.Drawing.Point(154, 30)
        Me.txtBox_OutputShp.Name = "txtBox_OutputShp"
        Me.txtBox_OutputShp.ReadOnly = True
        Me.txtBox_OutputShp.Size = New System.Drawing.Size(295, 20)
        Me.txtBox_OutputShp.TabIndex = 22
        '
        'btn_OutputShp
        '
        Me.btn_OutputShp.Image = CType(resources.GetObject("btn_OutputShp.Image"), System.Drawing.Image)
        Me.btn_OutputShp.Location = New System.Drawing.Point(467, 30)
        Me.btn_OutputShp.Name = "btn_OutputShp"
        Me.btn_OutputShp.Size = New System.Drawing.Size(32, 23)
        Me.btn_OutputShp.TabIndex = 21
        Me.btn_OutputShp.UseVisualStyleBackColor = True
        '
        'frm_CoincidentPointsShp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(564, 375)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btn_Cancel)
        Me.Controls.Add(Me.btn_Run)
        Me.Controls.Add(Me.txtBox_RawPointCloud)
        Me.Controls.Add(Me.btn_RawPointCloud)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBox2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frm_CoincidentPointsShp"
        Me.Text = "Coincident Points Tool"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.NumericUpDown_Precision, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents btn_Cancel As System.Windows.Forms.Button
    Friend WithEvents btn_Run As System.Windows.Forms.Button
    Friend WithEvents txtBox_RawPointCloud As System.Windows.Forms.TextBox
    Friend WithEvents btn_RawPointCloud As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtBox_ExtentShp As System.Windows.Forms.TextBox
    Friend WithEvents btn_ExtentShp As System.Windows.Forms.Button
    Friend WithEvents btnPreviewFile As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents NumericUpDown_Precision As System.Windows.Forms.NumericUpDown
    Friend WithEvents ChkBox_SetPrecision As System.Windows.Forms.CheckBox
    Friend WithEvents Cmb_Box_Seperator As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btn_SpatialReference As System.Windows.Forms.Button
    Friend WithEvents txtBox_SpatialReference As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtBox_OutputShp As System.Windows.Forms.TextBox
    Friend WithEvents btn_OutputShp As System.Windows.Forms.Button
    Friend WithEvents ttip As System.Windows.Forms.ToolTip
End Class
