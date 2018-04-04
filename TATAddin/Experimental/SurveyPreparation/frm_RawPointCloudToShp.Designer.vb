<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_RawPointCloudToShp
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frm_RawPointCloudToShp))
        Me.btn_Help = New System.Windows.Forms.Button()
        Me.btn_Cancel = New System.Windows.Forms.Button()
        Me.btn_Run = New System.Windows.Forms.Button()
        Me.txtBox_RawPointCloud = New System.Windows.Forms.TextBox()
        Me.txtBox_OutputShp = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnPreviewFile = New System.Windows.Forms.Button()
        Me.cmbBox_SelectSeparator = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btn_RawPointCloud = New System.Windows.Forms.Button()
        Me.btn_SpatialReference = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtBox_SpatialReference = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btn_OutputShp = New System.Windows.Forms.Button()
        Me.ttip = New System.Windows.Forms.ToolTip(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btn_Help
        '
        Me.btn_Help.Image = My.Resources.Resources.Help
        Me.btn_Help.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btn_Help.Location = New System.Drawing.Point(18, 265)
        Me.btn_Help.Name = "btn_Help"
        Me.btn_Help.Size = New System.Drawing.Size(61, 23)
        Me.btn_Help.TabIndex = 30
        Me.btn_Help.Text = "Help"
        Me.btn_Help.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btn_Help.UseVisualStyleBackColor = True
        '
        'btn_Cancel
        '
        Me.btn_Cancel.Location = New System.Drawing.Point(494, 265)
        Me.btn_Cancel.Name = "btn_Cancel"
        Me.btn_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.btn_Cancel.TabIndex = 29
        Me.btn_Cancel.Text = "Cancel"
        Me.btn_Cancel.UseVisualStyleBackColor = True
        '
        'btn_Run
        '
        Me.btn_Run.Location = New System.Drawing.Point(397, 265)
        Me.btn_Run.Name = "btn_Run"
        Me.btn_Run.Size = New System.Drawing.Size(75, 23)
        Me.btn_Run.TabIndex = 28
        Me.btn_Run.Text = "Run Tool"
        Me.btn_Run.UseVisualStyleBackColor = True
        '
        'txtBox_RawPointCloud
        '
        Me.txtBox_RawPointCloud.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_RawPointCloud.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_RawPointCloud.Location = New System.Drawing.Point(189, 39)
        Me.txtBox_RawPointCloud.Name = "txtBox_RawPointCloud"
        Me.txtBox_RawPointCloud.ReadOnly = True
        Me.txtBox_RawPointCloud.Size = New System.Drawing.Size(308, 20)
        Me.txtBox_RawPointCloud.TabIndex = 26
        '
        'txtBox_OutputShp
        '
        Me.txtBox_OutputShp.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_OutputShp.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_OutputShp.Location = New System.Drawing.Point(189, 207)
        Me.txtBox_OutputShp.Name = "txtBox_OutputShp"
        Me.txtBox_OutputShp.ReadOnly = True
        Me.txtBox_OutputShp.Size = New System.Drawing.Size(308, 20)
        Me.txtBox_OutputShp.TabIndex = 25
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnPreviewFile)
        Me.GroupBox1.Controls.Add(Me.cmbBox_SelectSeparator)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.btn_RawPointCloud)
        Me.GroupBox1.Controls.Add(Me.btn_SpatialReference)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.txtBox_SpatialReference)
        Me.GroupBox1.Location = New System.Drawing.Point(16, 10)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(553, 151)
        Me.GroupBox1.TabIndex = 31
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Inputs"
        '
        'btnPreviewFile
        '
        Me.btnPreviewFile.Location = New System.Drawing.Point(369, 67)
        Me.btnPreviewFile.Name = "btnPreviewFile"
        Me.btnPreviewFile.Size = New System.Drawing.Size(112, 23)
        Me.btnPreviewFile.TabIndex = 12
        Me.btnPreviewFile.Text = "Preview First Lines"
        Me.btnPreviewFile.UseVisualStyleBackColor = True
        '
        'cmbBox_SelectSeparator
        '
        Me.cmbBox_SelectSeparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBox_SelectSeparator.FormattingEnabled = True
        Me.cmbBox_SelectSeparator.Items.AddRange(New Object() {"Comma", "Period", "Semi-Colon", "Colon", "Space"})
        Me.cmbBox_SelectSeparator.Location = New System.Drawing.Point(224, 67)
        Me.cmbBox_SelectSeparator.Name = "cmbBox_SelectSeparator"
        Me.cmbBox_SelectSeparator.Size = New System.Drawing.Size(121, 21)
        Me.cmbBox_SelectSeparator.TabIndex = 11
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(29, 116)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(141, 13)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Spatial Reference (optional):"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(78, 32)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Raw Point Cloud:"
        '
        'btn_RawPointCloud
        '
        Me.btn_RawPointCloud.Image = CType(resources.GetObject("btn_RawPointCloud.Image"), System.Drawing.Image)
        Me.btn_RawPointCloud.Location = New System.Drawing.Point(491, 29)
        Me.btn_RawPointCloud.Name = "btn_RawPointCloud"
        Me.btn_RawPointCloud.Size = New System.Drawing.Size(29, 23)
        Me.btn_RawPointCloud.TabIndex = 3
        Me.btn_RawPointCloud.UseVisualStyleBackColor = True
        '
        'btn_SpatialReference
        '
        Me.btn_SpatialReference.Image = CType(resources.GetObject("btn_SpatialReference.Image"), System.Drawing.Image)
        Me.btn_SpatialReference.Location = New System.Drawing.Point(491, 110)
        Me.btn_SpatialReference.Name = "btn_SpatialReference"
        Me.btn_SpatialReference.Size = New System.Drawing.Size(29, 23)
        Me.btn_SpatialReference.TabIndex = 8
        Me.btn_SpatialReference.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 70)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(205, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Input the separator of your point cloud file:"
        '
        'txtBox_SpatialReference
        '
        Me.txtBox_SpatialReference.BackColor = System.Drawing.SystemColors.HighlightText
        Me.txtBox_SpatialReference.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtBox_SpatialReference.Location = New System.Drawing.Point(173, 113)
        Me.txtBox_SpatialReference.Name = "txtBox_SpatialReference"
        Me.txtBox_SpatialReference.ReadOnly = True
        Me.txtBox_SpatialReference.Size = New System.Drawing.Size(308, 20)
        Me.txtBox_SpatialReference.TabIndex = 9
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.btn_OutputShp)
        Me.GroupBox2.Location = New System.Drawing.Point(16, 168)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(553, 78)
        Me.GroupBox2.TabIndex = 32
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Output"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(78, 42)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(89, 13)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "Output Shapefile:"
        '
        'btn_OutputShp
        '
        Me.btn_OutputShp.Image = My.Resources.Resources.Save
        Me.btn_OutputShp.Location = New System.Drawing.Point(491, 39)
        Me.btn_OutputShp.Name = "btn_OutputShp"
        Me.btn_OutputShp.Size = New System.Drawing.Size(29, 23)
        Me.btn_OutputShp.TabIndex = 1
        Me.btn_OutputShp.UseVisualStyleBackColor = True
        '
        'frm_RawPointCloudToShp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 298)
        Me.Controls.Add(Me.btn_Help)
        Me.Controls.Add(Me.btn_Cancel)
        Me.Controls.Add(Me.btn_Run)
        Me.Controls.Add(Me.txtBox_RawPointCloud)
        Me.Controls.Add(Me.txtBox_OutputShp)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBox2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frm_RawPointCloudToShp"
        Me.Text = "Raw Point Cloud to Shapefile"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btn_Help As System.Windows.Forms.Button
    Friend WithEvents btn_Cancel As System.Windows.Forms.Button
    Friend WithEvents btn_Run As System.Windows.Forms.Button
    Friend WithEvents txtBox_RawPointCloud As System.Windows.Forms.TextBox
    Friend WithEvents txtBox_OutputShp As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents cmbBox_SelectSeparator As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btn_RawPointCloud As System.Windows.Forms.Button
    Friend WithEvents btn_SpatialReference As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtBox_SpatialReference As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btn_OutputShp As System.Windows.Forms.Button
    Friend WithEvents ttip As System.Windows.Forms.ToolTip
    Friend WithEvents btnPreviewFile As System.Windows.Forms.Button
End Class
