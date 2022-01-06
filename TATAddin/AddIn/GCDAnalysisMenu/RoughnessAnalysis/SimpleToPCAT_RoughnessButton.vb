﻿Public Class SimpleToPCAT_RoughnessButton
    Inherits ESRI.ArcGIS.Desktop.AddIns.Button

    Protected Overrides Sub OnClick()

        Dim frm As New frm_SimpleToPCAT_RoughnessRaster(My.ThisApplication)
        Try
            frm.ShowDialog()
        Catch ex As Exception
            ExceptionUI.HandleException(ex)
        End Try

        My.ArcMap.Application.CurrentTool = Nothing
    End Sub

    Protected Overrides Sub OnUpdate()
        Enabled = My.ArcMap.Application IsNot Nothing
    End Sub
End Class