Imports Microsoft.Win32
Imports System.IO
Imports System.Windows
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesFile

Public Class frm_FcToDEM_TIN

    Public Sub New(ByRef pApp As ESRI.ArcGIS.Framework.IApplication)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        m_pArcMap = pApp

    End Sub

    Private m_pArcMap As ESRI.ArcGIS.Framework.IApplication
    Private m_eInterpolationMethod As GP.Analyst3D.SamplingMetods
    Private m_eTriangulationMethod As GP.Analyst3D.DelaunayTriangulationTypes

    Public Property Filter As String

    'Dim m_SpatialRef As String
    Dim m_SpatialRef As ESRI.ArcGIS.Geometry.ISpatialReference
    Dim m_outTIN_FileDialog As New System.Windows.Forms.SaveFileDialog
    Dim m_ExtentShp_FileDialog As New System.Windows.Forms.OpenFileDialog


    Private Sub btn_SpatialReference_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_SpatialReference.Click

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

    Private Sub btn_PointCloudShp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_PointCloudShp.Click

        cmb_ZField.Items.Clear()
        'Error_btn_PointCloudShp.Clear()

        ''''''''''''''''''''''''
        'OLD FILE DIALOG
        '
        'TopCAT.WindowsFormAssistant.OpenFileDialog("Shapefile", txtBox_PointCloudShp)
        'If Not IO.File.Exists(txtBox_PointCloudShp.Text) Then
        '    MsgBox("The point feature class provided does not exist. Please select a different point feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
        '    Exit Sub
        'End If
        '''''''''''''''''''''''''''

        Dim pFC As GISDataStructures.VectorDataSource
        pFC = GISDataStructures.VectorDataSource.BrowseOpen("Select Point Vector Data", Nothing, "Point Vector Data", GISDataStructures.GeometryTypes.Point, 0)
        If pFC Is Nothing Then
            Exit Sub
        End If
        txtBox_PointCloudShp.Text = pFC.FullPath
        'GISDataStructures.VectorDataSource.BrowseOpen(txtBox_PointCloudShp, "Select Point Vector Data", Nothing, GISDataStructures.BrowseGISTypes.Any, My.ThisApplication, 0)

        If Not pFC Is Nothing Then
            If Not GISDataStructures.VectorDataSource.Exists(txtBox_PointCloudShp.Text, ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint) Then
                MsgBox("The point feature class provided does not exist or is not a point feature class. Please select a different point feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Exit Sub
            End If
        Else
            MsgBox("Please select a valid point feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If

        CheckInputFeaturesSpatialReferences(pFC, txtBox_PointCloudShp)

        'TODO MAKE IT SO THIS LIST EXCLUDES SHAPEFIELDS, GEOMETRY FIELDS, OID FIELDS, ETC.
        If Not pFC Is Nothing Then
            For i As Integer = 0 To pFC.FeatureClass.Fields.FieldCount - 1
                If pFC.FeatureClass.Fields.Field(i).Type = esriFieldType.esriFieldTypeDouble Or
                    pFC.FeatureClass.Fields.Field(i).Type = esriFieldType.esriFieldTypeInteger Or
                    pFC.FeatureClass.Fields.Field(i).Type = esriFieldType.esriFieldTypeSingle Or
                    pFC.FeatureClass.Fields.Field(i).Type = esriFieldType.esriFieldTypeSmallInteger Then
                    cmb_ZField.Items.Add(pFC.FeatureClass.Fields.Field(i).Name)
                ElseIf pFC.FeatureClass.Fields.Field(i).Type = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeGeometry Then
                    Dim shapeField As ESRI.ArcGIS.Geodatabase.IField = pFC.FeatureClass.Fields.Field(i)
                    Dim geometryDef As ESRI.ArcGIS.Geodatabase.IGeometryDef = shapeField.GeometryDef
                    If geometryDef.HasZ Then
                        cmb_ZField.Items.Add(pFC.FeatureClass.Fields.Field(i).Name & ".Z")
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

    Private Sub txtBox_PointCloudShp_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_PointCloudShp.TextChanged

    End Sub

    Private Sub btn_ExtentShp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ExtentShp.Click

        Dim pFC As GISDataStructures.VectorDataSource
        pFC = GISDataStructures.VectorDataSource.BrowseOpen("Select Polygon Vector Data", Nothing, "Polygon Vector Data", GISDataStructures.GeometryTypes.Polygon, 0)

        If pFC Is Nothing Then
            Exit Sub
        End If

        txtBox_ExtentShp.Text = pFC.FullPath

        If Not pFC Is Nothing Then
            If Not GISDataStructures.VectorDataSource.Exists(txtBox_ExtentShp.Text, ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon) Then
                MsgBox("The polygon feature class provided does not exist or is not a point feature class. Please select a different polygon feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Exit Sub
            End If
        Else
            MsgBox("Please select a valid polygon feature class.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Exit Sub
        End If

        CheckInputFeaturesSpatialReferences(pFC, txtBox_ExtentShp)

    End Sub

    Private Sub txtBox_ExtentShp_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_ExtentShp.TextChanged

    End Sub

    Private Sub btn_OutputTIN_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OutputTIN.Click

        Dim sOutputTINPath As String = GISDataStructures.TINDataSource.BrowseSave("Select TIN Output Location")
        If Not String.IsNullOrEmpty(sOutputTINPath) Then
            txtBox_OutputTIN.Text = sOutputTINPath
        End If

    End Sub


    Private Sub txtBox_OutputTIN_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_OutputTIN.TextChanged

    End Sub


    Private Sub chk_CreateDEM_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_CreateDEM.CheckedChanged

        If chk_CreateDEM.Checked = False Then
            btn_OutputDEM.Enabled = False
            txtBox_OutputDEM.Enabled = False
            updn_CellSize.Enabled = False
            chk_DeleteTIN.Enabled = False
            bxInterpolationMethod.Enabled = False
        ElseIf chk_CreateDEM.Checked = True Then
            btn_OutputDEM.Enabled = True
            txtBox_OutputDEM.Enabled = True
            updn_CellSize.Enabled = True
            chk_DeleteTIN.Enabled = True
            bxInterpolationMethod.Enabled = True
        End If

    End Sub

    Private Sub btn_OutputDEM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OutputDEM.Click

        Dim sOutputRasterPath As String = GISDataStructures.Raster.BrowseSave("Select Raster Output Location")
        If Not String.IsNullOrEmpty(sOutputRasterPath) Then
            txtBox_OutputDEM.Text = sOutputRasterPath
        End If

    End Sub

    Private Sub txtBox_OutputDEM_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBox_OutputDEM.TextChanged

    End Sub

    Private Sub updn_CellSize_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles updn_CellSize.ValueChanged

    End Sub

    Private Sub chk_DeleteTIN_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_DeleteTIN.CheckedChanged

        If chk_DeleteTIN.Checked = True Then

        End If

    End Sub

    Private Sub btn_Run_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Run.Click

        If Not ValidateForm() Then
            Exit Sub
        End If

        Dim gMassPoints As GISDataStructures.PointDataSource = New GISDataStructures.PointDataSource(txtBox_PointCloudShp.Text)
        Dim sValueField As String = cmb_ZField.SelectedItem.ToString
        Dim gExtent As GISDataStructures.PolygonDataSource = New GISDataStructures.PolygonDataSource(txtBox_ExtentShp.Text)
        Dim pSurfaceInterpolator As SurfaceInterpolator = New SurfaceInterpolator(gMassPoints, sValueField, gExtent)

        Dim gResult = pSurfaceInterpolator.ExecuteUI(chk_CreateDEM.Checked,
                                                     chk_DeleteTIN.Checked,
                                                     updn_CellSize.Value,
                                                     m_SpatialRef,
                                                     txtBox_OutputTIN.Text,
                                                     txtBox_OutputDEM.Text,
                                                     m_eTriangulationMethod,
                                                     m_eInterpolationMethod)

        If Not TypeOf (gResult) Is GISDataStructures.Raster Then
            Throw New Exception("An error occured when during the process of interpolating a surface.")
        Else
            gResult.AddToMap(My.ThisApplication)
        End If

        Me.Close()

    End Sub

    Private Sub btn_Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click

        Me.Close()

    End Sub

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

    Private Sub btn_Help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Help.Click

        System.Diagnostics.Process.Start(My.Resources.HelpBaseURL & "gcd-command-reference/data-prep-menu/c-survey-preparation-menu/ii-create-tin-and-or-dem")

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

        If String.IsNullOrEmpty(txtBox_OutputTIN.Text) Then
            MsgBox("Please provide a filepath for the output TIN.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        If chk_CreateDEM.Checked Then
            If String.IsNullOrEmpty(txtBox_OutputDEM.Text) Then
                MsgBox("Please provide a filepath for the output DEM.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                Return False
            End If
        End If


        If String.IsNullOrEmpty(txtBox_SpatialReference.Text) Then
            MsgBox("Please provide a spatial reference for the output files.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        ElseIf m_SpatialRef Is Nothing Then
            MsgBox("Please provide a spatial reference for the output files.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        Return True
    End Function

    Private Sub frm_ShpToTIN_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        rbContrainedDelaunay.Checked = True

        'Tooltips
        ttip.SetToolTip(btn_PointCloudShp, "Press this button to open a file dialog and select a raw point cloud file.")
        ttip.SetToolTip(txtBox_PointCloudShp, "Displays the file name of the point cloud. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(cmb_ZField, "Select the field to create the TIN and/or DEM from")
        ttip.SetToolTip(btn_ExtentShp, "Press this button to open the file dialog and select a polygon shapefile representing the extent of the point cloud.")
        ttip.SetToolTip(txtBox_ExtentShp, "Displays the file name of the input extent polygon. Do not type the file name, use the selection dialog button to the right.")
        ttip.SetToolTip(btn_SpatialReference, "Press this button to open a file dialog and select a file (.prj or .shp) containing spatial reference information.")
        ttip.SetToolTip(txtBox_SpatialReference, "Displays the file name of the spatial reference. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(txtBox_OutputTIN, "Displays the file name of the Output TIN. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(btn_OutputTIN, "Press this button to open a file dialog to select a location and name for the output TIN.")
        ttip.SetToolTip(chk_CreateDEM, "Check this box to enable the creation of a DEM from the TIN.")
        ttip.SetToolTip(chk_DeleteTIN, "Check this box to delete the created TIN and only include the DEM in the output.")
        ttip.SetToolTip(btn_OutputDEM, "Press this button to open a file dialog to name and save the output DEM to.")
        ttip.SetToolTip(txtBox_OutputDEM, "Diplays the file name of the output DEM. Use the selection button to the right to populate this field.")
        ttip.SetToolTip(updn_CellSize, "Select the cell size of the output raster.")
        ttip.SetToolTip(btn_Run, "Click to run the analysis")
        ttip.SetToolTip(btn_Help, "Click to go to the tool documentation.")
        ttip.SetToolTip(btn_Cancel, "Cancel analysis and exit the tool window.")

        'Set Interpolation and Traingulation variables
        m_eTriangulationMethod = GP.Analyst3D.DelaunayTriangulationTypes.ConstrainedDelauncy
        m_eInterpolationMethod = GP.Analyst3D.SamplingMetods.Linear

        'Disable Interpolation Method group box (Create DEM needs to be selected to enable it)
        bxInterpolationMethod.Enabled = False

    End Sub

    Private Sub TraingulationMethod_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbContrainedDelaunay.CheckedChanged, rbDelaunay.CheckedChanged

        If rbContrainedDelaunay.Checked Then
            m_eTriangulationMethod = GISCode.GP.Analyst3D.DelaunayTriangulationTypes.ConstrainedDelauncy
        ElseIf rbDelaunay.Checked Then
            m_eTriangulationMethod = GISCode.GP.Analyst3D.DelaunayTriangulationTypes.Delaunay
        End If

    End Sub


    Private Sub InterpolationMethod_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbLinear.CheckedChanged, rbNaturalNeighbors.CheckedChanged

        If rbLinear.Checked Then
            m_eInterpolationMethod = GISCode.GP.Analyst3D.SamplingMetods.Linear
        ElseIf rbNaturalNeighbors.Checked Then
            m_eInterpolationMethod = GISCode.GP.Analyst3D.SamplingMetods.NaturalNeighbours
        End If

    End Sub


End Class