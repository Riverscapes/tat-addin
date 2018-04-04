Namespace GISCode.ErrorHandling

    Public Class ExceptionForm

        Private DetailsExpanded = False

        Private Sub btnDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDetails.Click
            ShowDetails(Not DetailsExpanded)
        End Sub

        Private Sub ShowDetails(bShow As Boolean)

            If bShow Then
                Height = 300
                btnDetails.Text = "Details <<"
            Else
                Height = 115
                btnDetails.Text = "Details >>"
            End If

            DetailsExpanded = Not DetailsExpanded
        End Sub

        Private Sub ExceptionForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.Text = My.Resources.ApplicationNameLong & " Error"

            lblSupportEmail.Text = lblSupportEmail.Text.Replace("lblSupportEmail", My.Resources.SupportEmail)
            lblSupportEmail.LinkArea = New System.Windows.Forms.LinkArea(lblSupportEmail.LinkArea.Start, My.Resources.SupportEmail.Length)

            ShowDetails(False)
            DetailsExpanded = False

        End Sub

        Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblSupportEmail.LinkClicked
            Dim sSupportEmail As String = String.Format("mailto:{0}", My.Resources.SupportEmail)
            Process.Start(sSupportEmail)
        End Sub
    End Class

End Namespace