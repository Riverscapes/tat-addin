Public Class frm_InterploationError

    Private m_pArcMap As ESRI.ArcGIS.Framework.IApplication
    Private m_gPointFC As GISDataStructures.PointDataSource
    Private m_gSurveyExtent As GISDataStructures.PolygonDataSource
    Private m_gInputRaster As GISDataStructures.Raster
    Private m_SpatialRef As ESRI.ArcGIS.Geometry.ISpatialReference

    Private Sub btn_PointCloudShp_Click(sender As System.Object, e As System.EventArgs) Handles btn_PointCloudShp.Click

        cmb_ZField.Items.Clear()

        m_gPointFC = GISDataStructures.PointDataSource.BrowseOpen("Select Point Vector Data", Nothing, "Point Vector Data", 0)

        If m_gPointFC Is Nothing Then
            Exit Sub
        End If
        txtBox_PointCloudShp.Text = m_gPointFC.FullPath
        'GISDataStructures.VectorDataSource.BrowseOpen(txtBox_PointCloudShp, "Select Point Vector Data", Nothing, GISDataStructures.BrowseGISTypes.Any, My.ThisApplication, 0)

        If Not m_gPointFC Is Nothing Then
            If Not GISDataStructures.VectorDataSource.Exists(txtBox_PointCloudShp.Text, ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint) Then
                MsgBox("The point feature class provided does not exist or is not a point feature class. Please select a different point feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Exit Sub
            End If
        Else
            MsgBox("Please select a valid point feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If

        CheckInputFeaturesSpatialReferences(m_gPointFC, txtBox_PointCloudShp)

        'TODO MAKE IT SO THIS LIST EXCLUDES SHAPEFIELDS, GEOMETRY FIELDS, OID FIELDS, ETC.
        If Not m_gPointFC Is Nothing Then
            For i As Integer = 0 To m_gPointFC.FeatureClass.Fields.FieldCount - 1
                If m_gPointFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeDouble Or
                    m_gPointFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeInteger Or
                    m_gPointFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeSingle Or
                    m_gPointFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeSmallInteger Then
                    cmb_ZField.Items.Add(m_gPointFC.FeatureClass.Fields.Field(i).Name)
                ElseIf m_gPointFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeGeometry Then
                    Dim shapeField As ESRI.ArcGIS.Geodatabase.IField = m_gPointFC.FeatureClass.Fields.Field(i)
                    Dim geometryDef As ESRI.ArcGIS.Geodatabase.IGeometryDef = shapeField.GeometryDef
                    If geometryDef.HasZ Then
                        cmb_ZField.Items.Add(m_gPointFC.FeatureClass.Fields.Field(i).Name & ".Z")
                    End If
                End If
            Next i
        End If

        For i As Integer = 0 To cmb_ZField.Items.Count - 1
            If cmb_ZField.Items.Item(i).ToString() = "z" Or cmb_ZField.Items.Item(i).ToString() = "Z" Then
                cmb_ZField.SelectedItem = cmb_ZField.Items.Item(i)
            Else
                cmb_ZField.SelectedItem = cmb_ZField.Items.Item(0)
            End If
        Next i

    End Sub



    Private Sub btn_ExtentShp_Click(sender As System.Object, e As System.EventArgs) Handles btn_ExtentShp.Click

        m_gSurveyExtent = GISDataStructures.VectorDataSource.BrowseOpen("Select Polygon Vector Data", Nothing, "Polygon Vector Data", GISDataStructures.GeometryTypes.Polygon, 0)

        If m_gSurveyExtent Is Nothing Then
            Exit Sub
        End If

        If TypeOf m_gSurveyExtent Is GISDataStructures.PolygonDataSource Then
            If Not String.IsNullOrEmpty(m_gSurveyExtent.FullPath) Then
                txtBox_ExtentShp.Text = m_gSurveyExtent.FullPath
                CheckInputFeaturesSpatialReferences(m_gSurveyExtent, txtBox_PointCloudShp)
            End If
        End If

        If Not m_gSurveyExtent Is Nothing Then
            If Not GISDataStructures.VectorDataSource.Exists(txtBox_ExtentShp.Text, ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon) Then
                MsgBox("The polygon feature class provided does not exist or is not a point feature class. Please select a different polygon feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Exit Sub
            End If
        Else
            MsgBox("Please select a valid polygon feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If

    End Sub



    Private Sub btnInputRaster_Click(sender As System.Object, e As System.EventArgs) Handles btnInputRaster.Click

        m_gInputRaster = GISDataStructures.Raster.BrowseOpen("Select Input Raster")
        If TypeOf m_gInputRaster Is GISDataStructures.Raster Then
            If Not String.IsNullOrEmpty(m_gInputRaster.FullPath) Then
                txtInputRaster.Text = m_gInputRaster.FullPath
            End If
        End If

    End Sub

    Private Sub btn_SpatialReference_Click(sender As System.Object, e As System.EventArgs) Handles btn_SpatialReference.Click

        TopCAT.WindowsFormAssistant.OpenFileDialog("Spatial Reference", txtBox_SpatialReference)

        If IO.File.Exists(txtBox_SpatialReference.Text) Then
            If String.Compare(IO.Path.GetExtension(txtBox_SpatialReference.Text), ".prj") = 0 Then
                'm_SpatialRef = txtBox_SpatialReference.Text
                'Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TopCAT.GIS.LoadProjectedCoordinateSystem(txtBox_SpatialReference.Text)
                m_SpatialRef = TopCAT.GIS.LoadProjectedCoordinateSystem(txtBox_SpatialReference.Text)
                m_SpatialRef = TryCast(m_SpatialRef, ESRI.ArcGIS.Geometry.ISpatialReference)
                Try
                    Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TryCast(m_SpatialRef, ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem)
                    label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
                Catch ex As Exception
                    'Do Nothing 
                End Try

                Exit Sub
            Else
                Dim shapefileProjectedCheck As Boolean = TopCAT.WindowsFormAssistant.CheckIfShapefileHasPrjFile(txtBox_SpatialReference.Text)
                If shapefileProjectedCheck Then
                    Dim joinPath = IO.Path.Combine(IO.Path.GetDirectoryName(txtBox_SpatialReference.Text), IO.Path.GetFileNameWithoutExtension(txtBox_SpatialReference.Text) & ".prj")
                    m_SpatialRef = TopCAT.GIS.LoadProjectedCoordinateSystem(joinPath)
                    m_SpatialRef = TryCast(m_SpatialRef, ESRI.ArcGIS.Geometry.ISpatialReference)
                    'm_SpatialRef = joinPath
                    'Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TopCAT.GIS.LoadProjectedCoordinateSystem(joinPath)
                    Try
                        Dim projectedCoordinateSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = TryCast(m_SpatialRef, ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem)
                        label_CellSizeUnits.Text = projectedCoordinateSystem.CoordinateUnit.Name
                    Catch
                        'Do Nothing
                    End Try

                    Exit Sub
                End If
            End If
        Else
            MsgBox("The provided spatial reference file does not exist.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If


    End Sub

    Private Sub btn_OutputRaster_Click(sender As System.Object, e As System.EventArgs) Handles btn_OutputRaster.Click

        Dim sOutputRasterPath As String = GISDataStructures.Raster.BrowseSave("Select Raster Output Location")
        If Not String.IsNullOrEmpty(sOutputRasterPath) Then
            txtBox_OutputDEM.Text = sOutputRasterPath
        End If

    End Sub


    Private Sub btn_Run_Click(sender As System.Object, e As System.EventArgs) Handles btn_Run.Click

        If Not ValidateForm() Then
            Exit Sub
        End If

        ''Test of new code
        Dim pErrorGenerator As ErrorGenerator_InterpolationError = New ErrorGenerator_InterpolationError(m_gPointFC,
                                                                                                         m_gSurveyExtent,
                                                                                                         m_gInputRaster)
        Dim gRaster As GISDataStructures.Raster = pErrorGenerator.ExecuteUI(txtBox_OutputDEM.Text, cmb_ZField.SelectedItem)

        If My.Settings.AddOutputLayersToMap Then
            Try
                Dim pMXDoc As ESRI.ArcGIS.ArcMapUI.IMxDocument = My.ArcMap.Document
                If GISDataStructures.Raster.Exists(gRaster.FullPath) Then
                    gRaster.AddToMap(My.ArcMap.Application)
                End If
            Catch ex As Exception
                'Do Nothing
            End Try
        End If

        'Try
        '    IO.Directory.Delete(sTINFullPath, True)
        'Catch ex As Exception
        '    'Do nothing
        'End Try

        Me.Close()


    End Sub

    Private Sub btn_Cancel_Click(sender As System.Object, e As System.EventArgs) Handles btn_Cancel.Click

        Me.Close()

    End Sub


    Private Function ValidateForm()

        If String.IsNullOrEmpty(txtBox_PointCloudShp.Text) Then
            MsgBox("Please provide a point feature class as input.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If (cmb_ZField.SelectedIndex = -1) Then
            MsgBox("Please select a field to provide the elevation value for the DEM.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If String.IsNullOrEmpty(txtBox_ExtentShp.Text) Then
            MsgBox("Please provide an extent polygon as input.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If Not String.IsNullOrEmpty(txtInputRaster.Text) Then
            If Not GISDataStructures.Raster.Exists(txtInputRaster.Text) Then
                MsgBox("The input raster provided does not exist or is not valid. Please provide a different input raster.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Return False
            End If
        ElseIf String.IsNullOrEmpty(txtInputRaster.Text) Then
            MsgBox("Please provide an input raster.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If String.IsNullOrEmpty(txtBox_SpatialReference.Text) Then
            MsgBox("Please provide a spatial reference for the output files.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        ElseIf m_SpatialRef Is Nothing Then
            MsgBox("Please provide a spatial reference for the output files.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If Not String.IsNullOrEmpty(txtBox_OutputDEM.Text) Then
            If GISDataStructures.Raster.Exists(txtBox_OutputDEM.Text) Then
                MsgBox("The output path you provided is an already existing raster. Please provide a different path and filename to save the output raster to.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Return False
            End If
        ElseIf String.IsNullOrEmpty(txtBox_OutputDEM.Text) Then
            MsgBox("Please provide a path and filename to save the output raster to.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        Return True

    End Function


    Private Sub CheckInputFeaturesSpatialReferences(ByVal pFC As GISDataStructures.VectorDataSource, ByVal pVectorTextBox As System.Windows.Forms.TextBox)

        If Not pFC.SpatialReference Is Nothing Then
            If Not String.IsNullOrEmpty(pFC.SpatialReference.Name) Then
                If Not m_SpatialRef Is Nothing Then
                    If String.Compare(pFC.SpatialReference.Name, "Unknown") = 0 Then
                        MsgBox(IO.Path.GetFileName(pVectorTextBox.Text) & " is not projected." & vbCrLf & vbCrLf & "This could cause problems when creating TIN.")
                    Else
                        If Not pFC.CheckSpatialReferenceMatches(m_SpatialRef) Then
                            MsgBox(IO.Path.GetFileName(pVectorTextBox.Text) & " does not match the spatial reference already provided." & vbCrLf & vbCrLf & "This could cause problems when creating TIN.")
                        End If
                    End If
                Else
                    If String.Compare(pFC.SpatialReference.Name, "Unknown") = 0 Then
                        MsgBox(IO.Path.GetFileName(pVectorTextBox.Text) & " is not projected." & vbCrLf & vbCrLf & "This could cause problems when creating TIN.")
                    Else
                        m_SpatialRef = pFC.SpatialReference
                        txtBox_SpatialReference.Text = pFC.SpatialReference.Name
                    End If
                End If
            Else
                MsgBox(IO.Path.GetFileName(pVectorTextBox.Text) & " is not projected." & vbCrLf & vbCrLf & "This could cause problems when creating TIN.")
            End If
        Else
            MsgBox(IO.Path.GetFileName(pVectorTextBox.Text) & " is not projected." & vbCrLf & vbCrLf & "This could cause problems when creating TIN.")
        End If

    End Sub

    Private Function CollectUnkownSpatialReferenceResponse(ByVal pVectorDataSource As GISDataStructures.VectorDataSource) As Boolean
        Dim response As New MsgBoxResult
        response = MsgBox("The spatial reference of the input feature " & pVectorDataSource.DatasetName & " is unknown, this can cause problems when creating a TIN." & vbCrLf & vbCrLf & "Would you like to try and create a TIN and/or DEM anyhow?", MsgBoxStyle.YesNo, My.Resources.ApplicationNameLong)
        If response = MsgBoxResult.Yes Then
            Return True
        ElseIf response = MsgBoxResult.No Then
            Return False
        End If
    End Function

    Private Sub btnHelp_Click(sender As System.Object, e As System.EventArgs) Handles btnHelp.Click
        System.Diagnostics.Process.Start(My.Resources.HelpBaseURL & "gcd-command-reference/gcd-analysis-menu/a-uncertainty-analysis-submenu/c-raster-ba/i-create-interpolation-error-surface")
    End Sub

End Class