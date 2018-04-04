Public Class RoughnessModelerButton
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Protected Overrides Sub OnClick()

        Dim frm As New frmBootstrapRoughness
        Try
            If frm.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            End If
        Catch ex As Exception
            ExceptionUI.HandleException(ex)
        End Try

        My.ArcMap.Application.CurrentTool = Nothing
    End Sub

    Protected Overrides Sub OnUpdate()
        Enabled = My.ArcMap.Application IsNot Nothing
    End Sub
End Class
