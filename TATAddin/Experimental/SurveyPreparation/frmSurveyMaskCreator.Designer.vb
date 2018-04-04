<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSurveyMaskCreator
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSurveyMaskCreator))
        Me.btn_Cancel = New System.Windows.Forms.Button()
        Me.btn_Run = New System.Windows.Forms.Button()
        Me.btn_Help = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtOutputShp = New System.Windows.Forms.TextBox()
        Me.btn_OutputShp = New System.Windows.Forms.Button()
        Me.txtBox_RawPointCloud = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
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
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.yResolution, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.xResolution, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nToCalculateStats, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btn_Cancel
        '
        Me.btn_Cancel.Location = New System.Drawing.Point(498, 312)
        Me.btn_Cancel.Name = "btn_Cancel"
        Me.btn_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.btn_Cancel.TabIndex = 21
        Me.btn_Cancel.Text = "Cancel"
        Me.btn_Cancel.UseVisualStyleBackColor = True
        '
        'btn_Run
        '
        Me.btn_Run.Location = New System.Drawing.Point(396, 312)
        Me.btn_Run.Name = "btn_Run"
        Me.btn_Run.Size = New System.Drawing.Size(75, 23)
        Me.btn_Run.TabIndex = 20
        Me.btn_Run.Text = "Run Tool"
        Me.btn_Run.UseVisualStyleBackColor = True
        '
        'btn_Help
        '
        Me.btn_Help.Image = My.Resources.Resources.Help
        Me.btn_Help.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btn_Help.Location = New System.Drawing.Point(27, 312)
        Me.btn_Help.Name = "btn_Help"
        Me.btn_Help.Size = New System.Drawing.Size(64, 23)
        Me.btn_Help.TabIndex = 18
        Me.btn_Help.Text = "Help"
        Me.btn_Help.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btn_Help.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.txtOutputShp)
        Me.GroupBox2.Controls.Add(Me.btn_OutputShp)
        Me.GroupBox2.Location = New System.Drawing.Point(25, 216)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(546, 80)
        Me.GroupBox2.TabIndex = 17
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Output"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 37)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(113, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Survey Mask Polygon:"
        '
        'txtOutputShp
        '
        Me.txtOutputShp.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtOutputShp.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtOutputShp.Location = New System.Drawing.Point(126, 35)
        Me.txtOutputShp.Name = "txtOutputShp"
        Me.txtOutputShp.ReadOnly = True
        Me.txtOutputShp.Size = New System.Drawing.Size(342, 20)
        Me.txtOutputShp.TabIndex = 5
        '
        'btn_OutputShp
        '
        Me.btn_OutputShp.Image = My.Resources.Resources.Save
        Me.btn_OutputShp.Location = New System.Drawing.Point(481, 32)
        Me.btn_OutputShp.Name = "btn_OutputShp"
        Me.btn_OutputShp.Size = New System.Drawing.Size(26, 23)
        Me.btn_OutputShp.TabIndex = 4
        Me.btn_OutputShp.UseVisualStyleBackColor = True
        '
        'txtBox_RawPointCloud
        '
        Me.txtBox_RawPointCloud.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_RawPointCloud.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_RawPointCloud.Location = New System.Drawing.Point(151, 47)
        Me.txtBox_RawPointCloud.Name = "txtBox_RawPointCloud"
        Me.txtBox_RawPointCloud.ReadOnly = True
        Me.txtBox_RawPointCloud.Size = New System.Drawing.Size(342, 20)
        Me.txtBox_RawPointCloud.TabIndex = 15
        '
        'GroupBox1
        '
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
        Me.GroupBox1.Location = New System.Drawing.Point(25, 18)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(546, 185)
        Me.GroupBox1.TabIndex = 16
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Inputs"
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
        Me.Label2.Location = New System.Drawing.Point(25, 148)
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
        Me.txtBox_SpatialReference.Location = New System.Drawing.Point(126, 146)
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
        Me.btn_SpatialReference.Location = New System.Drawing.Point(481, 143)
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
        'frmSurveyMaskCreator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(599, 352)
        Me.Controls.Add(Me.btn_Cancel)
        Me.Controls.Add(Me.btn_Run)
        Me.Controls.Add(Me.btn_Help)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.txtBox_RawPointCloud)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSurveyMaskCreator"
        Me.Text = "Survey Mask Creator"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
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
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtOutputShp As System.Windows.Forms.TextBox
    Friend WithEvents btn_OutputShp As System.Windows.Forms.Button
    Friend WithEvents txtBox_RawPointCloud As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
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
End Class
