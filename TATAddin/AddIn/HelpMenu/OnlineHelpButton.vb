﻿Public Class OnlineHelpButton
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Protected Overrides Sub OnClick()

        Try
            Process.Start(My.Resources.HelpBaseURL)
        Catch ex As Exception
            ExceptionUI.HandleException(ex)
        End Try


        My.ArcMap.Application.CurrentTool = Nothing
    End Sub

    Protected Overrides Sub OnUpdate()
        Enabled = My.ArcMap.Application IsNot Nothing
    End Sub

End Class
