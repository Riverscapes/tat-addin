Public Class AboutButton
  Inherits ESRI.ArcGIS.Desktop.AddIns.Button

  Public Sub New()

  End Sub

  Protected Overrides Sub OnClick()

        'Dim frm As New AboutForm
        'Try
        '    frm.ShowDialog()
        'Catch ex As Exception
        '    ExceptionUI.HandleException(ex)
        'End Try


        My.ArcMap.Application.CurrentTool = Nothing
  End Sub

  Protected Overrides Sub OnUpdate()
    Enabled = My.ArcMap.Application IsNot Nothing
  End Sub
End Class
