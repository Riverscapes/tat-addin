Public Class frmBootstrapRoughness

    Private m_gPoints As GISDataStructures.PointDataSource
    Private m_gSurveyExtent As GISDataStructures.PolygonDataSource
    Private m_gInChannel As GISDataStructures.PolygonDataSource
    Private m_gDEM As GISDataStructures.Raster

    Private Sub frmBootstrapRoughness_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        'Populate grain size unit drop down with linear unit classes
        cboUnits.Items.Add(New LinearUnitClass("millimeters (mm)", NumberFormatting.LinearUnits.mm))
        cboUnits.Items.Add(New LinearUnitClass("centimeters (cm)", NumberFormatting.LinearUnits.cm))
        Dim i As Integer = cboUnits.Items.Add(New LinearUnitClass("meters (m)", NumberFormatting.LinearUnits.m))
        cboUnits.Items.Add(New LinearUnitClass("kilometers (km)", NumberFormatting.LinearUnits.km))
        cboUnits.Items.Add(New LinearUnitClass("inches (in)", NumberFormatting.LinearUnits.inch))
        cboUnits.Items.Add(New LinearUnitClass("feet (ft)", NumberFormatting.LinearUnits.ft))
        cboUnits.Items.Add(New LinearUnitClass("yards (yd)", NumberFormatting.LinearUnits.yard))
        cboUnits.Items.Add(New LinearUnitClass("miles (mi)", NumberFormatting.LinearUnits.mile))
        'cboUnits.SelectedIndex = i

        cboUnits.Enabled = False

    End Sub


    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click

        If Not ValidateForm() Then
            Exit Sub
        End If

        Try
            'Create ErrorGenerator_BootstrapRoughness
            Dim errGeneratorBootstrap As ErrorGenerator_BootstrapRoughness = New ErrorGenerator_BootstrapRoughness(m_gPoints, m_gSurveyExtent, m_gInChannel, m_gDEM)

            'Call to Python Script through Execute method of ErrorGenerator_BootstrapRoughness
            Dim eUnits As Object = Nothing
            If chkConvertToMillimeters.Checked Then
                Dim eLinearUnits As LinearUnitClass = cboUnits.SelectedItem
                eUnits = eLinearUnits.LinearUnit
            End If

            System.Windows.Forms.Application.UseWaitCursor = True
            Dim gResult As GISDataStructures.Raster = errGeneratorBootstrap.Execute(nudIterations.Value,
                                                                                    nudPercentData.Value,
                                                                                    WorkspaceManager.GetDefaultWorkspace(My.Resources.Manufacturer, My.Resources.ApplicationNameShort),
                                                                                    txtOutputRaster.Text,
                                                                                    eUnits)

            System.Windows.Forms.Application.UseWaitCursor = False

            If My.Settings.AddOutputLayersToMap Then
                Try
                    If TypeOf gResult Is GISDataStructures.Raster Then
                        gResult.AddToMap(My.ArcMap.Application)
                    End If
                Catch ex As Exception
                    'Do Nothing
                End Try
            End If

        Catch ex As Exception
            ExceptionUI.HandleException(ex)
        End Try



        Me.Close()
    End Sub

    Private Sub btnOpenSurveyPoints_Click(sender As System.Object, e As System.EventArgs) Handles btnOpenSurveyPoints.Click

        'Dim pFC As GISDataStructures.PointDataSource = GISDataStructures.VectorDataSource.BrowseOpen("Select Point Vector Data", Nothing, "Point Vector Data", GISDataStructures.GeometryTypes.Point, 0)
        m_gPoints = GISDataStructures.VectorDataSource.BrowseOpen("Select Point Vector Data", Nothing, "Point Vector Data", GISDataStructures.GeometryTypes.Point, 0)
        If Not m_gPoints Is Nothing Then
            txtSurveyPoints.Text = m_gPoints.FullPath
        End If

    End Sub

    Private Sub btnOpenSurveyExtent_Click(sender As System.Object, e As System.EventArgs) Handles btnOpenSurveyExtent.Click

        'Dim pFC As GISDataStructures.PolygonDataSource = GISDataStructures.VectorDataSource.BrowseOpen("Select Polygon Vector Data", Nothing, "Point Vector Data", GISDataStructures.GeometryTypes.Polygon, 0)
        m_gSurveyExtent = GISDataStructures.VectorDataSource.BrowseOpen("Select Polygon Vector Data", Nothing, "Point Vector Data", GISDataStructures.GeometryTypes.Polygon, 0)
        If Not m_gSurveyExtent Is Nothing Then
            txtSurveyExtent.Text = m_gSurveyExtent.FullPath
        End If

    End Sub


    Private Sub btnOpenChannel_Click(sender As System.Object, e As System.EventArgs) Handles btnOpenChannel.Click

        'Dim pFC As GISDataStructures.PolygonDataSource = GISDataStructures.VectorDataSource.BrowseOpen("Select Polygon Vector Data", Nothing, "Point Vector Data", GISDataStructures.GeometryTypes.Polygon, 0)
        m_gInChannel = GISDataStructures.VectorDataSource.BrowseOpen("Select Polygon Vector Data", Nothing, "Point Vector Data", GISDataStructures.GeometryTypes.Polygon, 0)
        If Not m_gInChannel Is Nothing Then
            txtChannelExtent.Text = m_gInChannel.FullPath
        End If

    End Sub


    Private Sub btnOpenDEM_Click(sender As System.Object, e As System.EventArgs) Handles btnOpenDEM.Click

        'Dim gInputRaster As GISDataStructures.Raster = GISDataStructures.Raster.BrowseOpen("Select DEM")
        m_gDEM = GISDataStructures.Raster.BrowseOpen("Select DEM")
        If Not m_gDEM Is Nothing Then
            Dim sOutputRasterPath As String = m_gDEM.FullPath

            If Not String.IsNullOrEmpty(sOutputRasterPath) Then
                txtDEMPath.Text = sOutputRasterPath
            End If
        End If

    End Sub


    Private Sub btnSaveOutputRaster_Click(sender As System.Object, e As System.EventArgs) Handles btnSaveOutputRaster.Click

        Dim sOutputRasterPath As String = GISDataStructures.Raster.BrowseSave("Select Raster Output Location")
        If Not String.IsNullOrEmpty(sOutputRasterPath) Then
            txtOutputRaster.Text = sOutputRasterPath
        End If

    End Sub

    Private Sub chkConvertToMillimeters_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkConvertToMillimeters.CheckedChanged

        If chkConvertToMillimeters.Checked Then
            cboUnits.Enabled = True
        ElseIf chkConvertToMillimeters.Checked = False Then
            cboUnits.Enabled = False
            cboUnits.SelectedIndex = -1
        End If

    End Sub

    Private Function ValidateForm()

        If String.IsNullOrEmpty(txtSurveyPoints.Text) Then
            MsgBox("Please provide the point feature class used to create the DEM through the Input Survey Points dialog.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If String.IsNullOrEmpty(txtSurveyExtent.Text) Then
            MsgBox("Please provide a polygon feature class of the survey extent from the Survey Extent dialog.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If String.IsNullOrEmpty(txtDEMPath.Text) Then
            MsgBox("Please provide a digital elevation model (DEM) from the DEM dialog.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If chkConvertToMillimeters.Checked Then
            If cboUnits.SelectedIndex < 0 Then
                MsgBox("Please select the vertical unit of the DEM from the vertical unit drop down menu.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Return False
            End If
        End If

        If String.IsNullOrEmpty(txtOutputRaster.Text) Then
            MsgBox("Please provide an filename for the output bootstrap roughness raster.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        Return True
    End Function

    Private Sub btnHelp_Click(sender As System.Object, e As System.EventArgs) Handles btnHelp.Click
        System.Diagnostics.Process.Start(My.Resources.HelpBaseURL & "gcd-command-reference/gcd-analysis-menu/b-roughness-analysis-submenu/bootstrap-roughness-modeler")
    End Sub

End Class