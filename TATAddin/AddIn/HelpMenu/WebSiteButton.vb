Public Class WebSiteButton
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Protected Overrides Sub OnClick()

        Try
            Process.Start(My.Resources.WebSiteURL)
        Catch ex As Exception
            ExceptionUI.HandleException(ex)
        End Try


        My.ArcMap.Application.CurrentTool = Nothing
    End Sub

    Protected Overrides Sub OnUpdate()
        Enabled = My.ArcMap.Application IsNot Nothing
    End Sub

End Class
