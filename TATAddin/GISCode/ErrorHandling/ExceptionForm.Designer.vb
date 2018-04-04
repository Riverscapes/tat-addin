Namespace GISCode.ErrorHandling

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ExceptionForm
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
            Me.btnDetails = New System.Windows.Forms.Button()
            Me.btnOK = New System.Windows.Forms.Button()
            Me.lblSupportEmail = New System.Windows.Forms.LinkLabel()
            Me.lblException = New System.Windows.Forms.Label()
            Me.Details = New System.Windows.Forms.GroupBox()
            Me.txtDetails = New System.Windows.Forms.TextBox()
            Me.Details.SuspendLayout()
            Me.SuspendLayout()
            '
            'btnDetails
            '
            Me.btnDetails.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnDetails.Location = New System.Drawing.Point(375, 43)
            Me.btnDetails.Name = "btnDetails"
            Me.btnDetails.Size = New System.Drawing.Size(75, 23)
            Me.btnDetails.TabIndex = 2
            Me.btnDetails.Text = "Details >>"
            Me.btnDetails.UseVisualStyleBackColor = True
            '
            'btnOK
            '
            Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.btnOK.Location = New System.Drawing.Point(375, 10)
            Me.btnOK.Name = "btnOK"
            Me.btnOK.Size = New System.Drawing.Size(75, 23)
            Me.btnOK.TabIndex = 3
            Me.btnOK.Text = "Close"
            Me.btnOK.UseVisualStyleBackColor = True
            '
            'lblSupportEmail
            '
            Me.lblSupportEmail.LinkArea = New System.Windows.Forms.LinkArea(73, 15)
            Me.lblSupportEmail.Location = New System.Drawing.Point(10, 36)
            Me.lblSupportEmail.Name = "lblSupportEmail"
            Me.lblSupportEmail.Size = New System.Drawing.Size(356, 37)
            Me.lblSupportEmail.TabIndex = 7
            Me.lblSupportEmail.TabStop = True
            Me.lblSupportEmail.Text = "If the problem persists, please copy the details below and email them to lblSuppo" & _
        "rtEmail."
            Me.lblSupportEmail.UseCompatibleTextRendering = True
            '
            'lblException
            '
            Me.lblException.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lblException.Location = New System.Drawing.Point(10, 10)
            Me.lblException.Name = "lblException"
            Me.lblException.Size = New System.Drawing.Size(329, 20)
            Me.lblException.TabIndex = 9
            Me.lblException.Text = "An exception occured during processing"
            '
            'Details
            '
            Me.Details.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Details.Controls.Add(Me.txtDetails)
            Me.Details.Location = New System.Drawing.Point(10, 76)
            Me.Details.Name = "Details"
            Me.Details.Size = New System.Drawing.Size(440, 175)
            Me.Details.TabIndex = 10
            Me.Details.TabStop = False
            Me.Details.Text = "Details"
            '
            'txtDetails
            '
            Me.txtDetails.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.txtDetails.Location = New System.Drawing.Point(10, 20)
            Me.txtDetails.Multiline = True
            Me.txtDetails.Name = "txtDetails"
            Me.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.txtDetails.Size = New System.Drawing.Size(420, 145)
            Me.txtDetails.TabIndex = 3
            '
            'ExceptionForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(467, 262)
            Me.Controls.Add(Me.Details)
            Me.Controls.Add(Me.lblException)
            Me.Controls.Add(Me.lblSupportEmail)
            Me.Controls.Add(Me.btnOK)
            Me.Controls.Add(Me.btnDetails)
            Me.MaximizeBox = False
            Me.MaximumSize = New System.Drawing.Size(1024, 768)
            Me.MinimizeBox = False
            Me.MinimumSize = New System.Drawing.Size(475, 107)
            Me.Name = "ExceptionForm"
            Me.ShowIcon = False
            Me.Text = "Exception"
            Me.Details.ResumeLayout(False)
            Me.Details.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents btnDetails As System.Windows.Forms.Button
        Friend WithEvents btnOK As System.Windows.Forms.Button
        Friend WithEvents lblSupportEmail As System.Windows.Forms.LinkLabel
        Friend WithEvents lblException As System.Windows.Forms.Label
        Friend WithEvents Details As System.Windows.Forms.GroupBox
        Friend WithEvents txtDetails As System.Windows.Forms.TextBox
    End Class

End Namespace