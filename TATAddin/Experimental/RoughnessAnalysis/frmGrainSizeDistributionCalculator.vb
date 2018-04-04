Public Class frmGrainSizeDistributionCalculator

    Private Sub frmGrainSizeDistributionCalculator_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        chkCreateCDF.Checked = False
        txtCDFOutputPath.Text = Nothing
        txtCDFOutputPath.Enabled = False
        btnOutputCDFFolder.Enabled = False

        'Populate grain size unit drop down with linear unit classes
        cboGrainSizeUnits.Items.Add(New LinearUnitClass("millimeters (mm)", NumberFormatting.LinearUnits.mm))
        cboGrainSizeUnits.Items.Add(New LinearUnitClass("centimeters (cm)", NumberFormatting.LinearUnits.cm))
        'Dim i As Integer = cboGrainSizeUnits.Items.Add(New LinearUnitClass("meters (m)", NumberFormatting.LinearUnits.m))
        cboGrainSizeUnits.Items.Add(New LinearUnitClass("meters (m)", NumberFormatting.LinearUnits.m))
        cboGrainSizeUnits.Items.Add(New LinearUnitClass("kilometers (km)", NumberFormatting.LinearUnits.km))
        cboGrainSizeUnits.Items.Add(New LinearUnitClass("inches (in)", NumberFormatting.LinearUnits.inch))
        cboGrainSizeUnits.Items.Add(New LinearUnitClass("feet (ft)", NumberFormatting.LinearUnits.ft))
        cboGrainSizeUnits.Items.Add(New LinearUnitClass("yards (yd)", NumberFormatting.LinearUnits.yard))
        cboGrainSizeUnits.Items.Add(New LinearUnitClass("miles (mi)", NumberFormatting.LinearUnits.mile))
        'cboGrainSizeUnits.SelectedIndex = i

    End Sub


    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click

        If Not ValidateForm() Then
            Exit Sub
        End If

        Dim sChannelUnitFieldName As String = cboChannelUnitField.SelectedItem.ToString()
        Dim LinearUnitsGrainSize As LinearUnitClass = cboGrainSizeUnits.SelectedItem
        Dim eLinearUnits As NumberFormatting.LinearUnits = LinearUnitsGrainSize.LinearUnit

        System.Windows.Forms.Application.UseWaitCursor = True
        Dim gPolygonFC As GISCode.GISDataStructures.PolygonDataSource = GISCode.GCD.GrainSizeDistributionCalculator.ProcessRaster(txtGrainSampleFile.Text,
                                                                                                                                  txtInputPolygon.Text,
                                                                                                                                  sChannelUnitFieldName,
                                                                                                                                  eLinearUnits,
                                                                                                                                  chkAppendGrainSizeValues.Checked,
                                                                                                                                  chkAppendNRasterCells.Checked,
                                                                                                                                  chkCreateCDF.Checked,
                                                                                                                                  txtCDFOutputPath.Text)
        System.Windows.Forms.Application.UseWaitCursor = False

        If My.Settings.AddOutputLayersToMap Then
            Try
                gPolygonFC.AddToMap(My.ThisApplication)
            Catch ex As Exception
                'Do nothing
            End Try
        End If

        Me.Close()

    End Sub

    Private Sub btn_OpenRawPointFile_Click(sender As System.Object, e As System.EventArgs) Handles btn_OpenRawPointFile.Click


        Dim gInputRaster As GISDataStructures.Raster = GISDataStructures.Raster.BrowseOpen("Select Surface Roughness Raster")
        If Not gInputRaster Is Nothing Then
            Dim sOutputRasterPath As String = gInputRaster.FullPath

            If Not String.IsNullOrEmpty(sOutputRasterPath) Then
                txtGrainSampleFile.Text = sOutputRasterPath
            End If
        End If

    End Sub

    Private Sub btnInputPolygon_Click(sender As System.Object, e As System.EventArgs) Handles btnInputPolygon.Click

        cboChannelUnitField.Items.Clear()

        Dim pFC As GISDataStructures.VectorDataSource
        pFC = GISDataStructures.VectorDataSource.BrowseOpen("Select Point Vector Data", Nothing, "Point Vector Data", GISDataStructures.GeometryTypes.Polygon, 0)
        If pFC Is Nothing Then
            Exit Sub
        End If
        txtInputPolygon.Text = pFC.FullPath
        'GISDataStructures.VectorDataSource.BrowseOpen(txtBox_PointCloudShp, "Select Point Vector Data", Nothing, GISDataStructures.BrowseGISTypes.Any, My.ThisApplication, 0)

        If Not pFC Is Nothing Then
            If Not GISDataStructures.VectorDataSource.Exists(txtInputPolygon.Text, ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon) Then
                MsgBox("The polygon feature class provided does not exist or is not a polygon feature class. Please select a different polygon feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Exit Sub
            End If
        Else
            MsgBox("Please select a valid polygon feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If

        If Not pFC Is Nothing Then
            For i As Integer = 0 To pFC.FeatureClass.Fields.FieldCount - 1
                If pFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeDouble Or
                    pFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeInteger Or
                    pFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeSingle Or
                    pFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeSmallInteger Then
                    cboChannelUnitField.Items.Add(pFC.FeatureClass.Fields.Field(i).Name)
                End If
            Next i
        End If

    End Sub

    Private Sub btnOutputCDFFolder_Click(sender As System.Object, e As System.EventArgs) Handles btnOutputCDFFolder.Click

        Dim sFolder As String = GISCode.FileSystem.BrowseToFolder("Select the GCD Project Parent Directory", My.Settings.LastUsedProjectFolder)
        If String.IsNullOrEmpty(sFolder) Then
            Exit Sub
        End If

        txtCDFOutputPath.Text = sFolder

    End Sub

    Private Sub chkCreateCDF_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkCreateCDF.CheckedChanged

        If chkCreateCDF.Checked Then
            txtCDFOutputPath.Text = Nothing
            txtCDFOutputPath.Enabled = True
            btnOutputCDFFolder.Enabled = True
        ElseIf chkCreateCDF.Checked = False Then
            txtCDFOutputPath.Text = Nothing
            txtCDFOutputPath.Enabled = False
            btnOutputCDFFolder.Enabled = False
        End If

    End Sub

    Private Sub chkAppendGrainSizeValues_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkAppendGrainSizeValues.CheckedChanged

        If chkAppendGrainSizeValues.Checked = True Then
            chkAppendNRasterCells.Enabled = True
        ElseIf chkAppendGrainSizeValues.Checked = False Then
            chkAppendNRasterCells.Checked = False
            chkAppendNRasterCells.Enabled = False
        End If

    End Sub

    Private Function ValidateForm() As Boolean

        If String.IsNullOrEmpty(txtGrainSampleFile.Text) Then
            MsgBox("Please provide a surface roughness raster.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If cboGrainSizeUnits.SelectedItem Is Nothing Then
            MsgBox("Please select a linear unit  from the Surface Roughness Vertical Units drop-down menu.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If String.IsNullOrEmpty(txtGrainSampleFile.Text) Then
            MsgBox("Please provide a channel unit polygon.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If cboChannelUnitField.SelectedItem Is Nothing Then
            MsgBox("Please select a field from the Unique Channel Field drop-down menu. The field must have a unique value for each record in the channel unit polygon.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If chkCreateCDF.Checked Then
            If Not String.IsNullOrEmpty(txtCDFOutputPath.Text) Then
                'TODO: Check if the folder is empty

            Else
                MsgBox("Please provide a folder to save the CDF figures to.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Return False
            End If
        End If

        Return True

    End Function

    Private Sub btnHelp_Click(sender As System.Object, e As System.EventArgs) Handles btnHelp.Click
        System.Diagnostics.Process.Start(My.Resources.HelpBaseURL & "gcd-command-reference/gcd-analysis-menu/b-roughness-analysis-submenu/grain-size-distribution-estimator")
    End Sub

End Class