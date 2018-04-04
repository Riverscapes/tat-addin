Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry

Public Class ErrorGenerator_InterpolationError

    Private Const m_sInterpolationErrorField As String = "InterErr"
    Private Const m_TempZField As String = "Z"
    Private Const m_RasterValueField As String = "RASTERVALU"

    Private m_gPoints As GISDataStructures.PointDataSource3D
    Private m_gPointsUI As GISDataStructures.PointDataSource
    Private m_gSurveyExtent As GISDataStructures.PolygonDataSource
    Private m_gDEM As GISDataStructures.Raster
    Private m_gTIN As GISDataStructures.TINDataSource

    Public Sub New(ByVal sTINPath As String, ByVal gSurveyExtent As GISDataStructures.PolygonDataSource, ByVal gRaster As GISDataStructures.Raster)

        Dim sTempPointFCPath = WorkspaceManager.GetTempShapeFile("in_memoryTinNodes")
        GP.Analyst3D.TINNodesToPoints_3D(sTINPath, sTempPointFCPath, m_TempZField)
        m_gPoints = New GISDataStructures.PointDataSource(sTempPointFCPath)
        m_gSurveyExtent = gSurveyExtent
        m_gDEM = gRaster
        m_gTIN = New GISDataStructures.TINDataSource(sTINPath)

    End Sub

    ''' <summary>
    ''' RBT Contructor
    ''' </summary>
    ''' <param name="gTIN">Final TIN used to create DEM</param>
    ''' <param name="gSurveyExtent">Survey Extent</param>
    ''' <param name="bOtainExtension">Console applications cannot use the ExtensionManager that the ObtainExtension method uses. Instead they use the
    ''' AOInit.CheckoutExtension method to obtain spatial analyst. Therefore this method should only do the extension check in UI products.</param>
    ''' <param name="gRaster">DEM</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal gTIN As GISDataStructures.TINDataSource, ByVal gSurveyExtent As GISDataStructures.PolygonDataSource, ByVal gRaster As GISDataStructures.Raster, bOtainExtension As Boolean)

        Dim sTempPointFCPath = WorkspaceManager.GetTempShapeFile("in_memoryTinNodes")
        GP.Analyst3D.TINNodesToPoints_3D(gTIN, sTempPointFCPath, m_TempZField, bOtainExtension)
        m_gPoints = New GISDataStructures.PointDataSource3D(sTempPointFCPath)
        m_gSurveyExtent = gSurveyExtent
        m_gDEM = gRaster
        m_gTIN = gTIN

    End Sub


    ''' <summary>
    ''' UI Constructor
    ''' </summary>
    ''' <param name="gPoints"></param>
    ''' <param name="gSurveyExtent"></param>
    ''' <param name="gRaster"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal gPoints As GISDataStructures.PointDataSource, ByVal gSurveyExtent As GISDataStructures.PolygonDataSource, ByVal gRaster As GISDataStructures.Raster)

        m_gPointsUI = gPoints
        m_gSurveyExtent = gSurveyExtent
        m_gDEM = gRaster

    End Sub

    ''' <summary>
    ''' Creates a surface from the difference between TIN nodes/survyed points and the DEM that was created from them
    ''' </summary>
    ''' <param name="sOutputRaster">path to create output raster</param>
    ''' <returns>GISDataStructures.Raster that is concurrent and orthogonal with m_gDEM</returns>
    ''' <remarks></remarks>
    Public Function Execute(ByVal sOutputRaster As String) As GISDataStructures.Raster

        Dim sTempPointFCPath = WorkspaceManager.GetTempShapeFile("in_memory")

        Dim pTempPointFC As GISDataStructures.PointDataSource = GISCode.GP.SpatialAnalyst.ExtractValuesToPoints(m_gPoints, m_gDEM, sTempPointFCPath)

        Dim geoProcessorEngine As ESRI.ArcGIS.Geoprocessing.IGeoProcessor2 = New ESRI.ArcGIS.Geoprocessing.GeoProcessor()
        geoProcessorEngine.AddOutputsToMap = False
        pTempPointFC.AddField(m_sInterpolationErrorField, ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeSingle)

        Dim pInterErrFieldIndex = pTempPointFC.FindField(m_sInterpolationErrorField)
        Dim pRasterValueField = pTempPointFC.FindField(m_RasterValueField)
        Dim pZFieldIndex As Integer = pTempPointFC.FindField(m_TempZField)

        Dim pFeatureBuffer = pTempPointFC.FeatureClass.CreateFeatureBuffer()
        Dim pUpdateCursor As ESRI.ArcGIS.Geodatabase.IFeatureCursor = pTempPointFC.FeatureClass.Update(Nothing, False)
        Dim pRow As ESRI.ArcGIS.Geodatabase.IRow = pUpdateCursor.NextFeature()
        While Not pRow Is Nothing

            Dim zValue As Double = pRow.Value(pZFieldIndex)

            If pRow.Value(pRasterValueField) < 0 Or Double.IsNaN(zValue) Then
                Debug.WriteLine("No data found.")
                pRow = pUpdateCursor.NextFeature()
                Continue While
            End If

            pRow.Value(pInterErrFieldIndex) = Math.Abs(zValue - pRow.Value(pRasterValueField))

            pUpdateCursor.UpdateFeature(pRow)
            pRow = pUpdateCursor.NextFeature()
        End While

        System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureBuffer)
        System.Runtime.InteropServices.Marshal.ReleaseComObject(pUpdateCursor)

        Dim pSurfaceInterpolator As SurfaceInterpolator = New SurfaceInterpolator(pTempPointFC, m_sInterpolationErrorField, m_gSurveyExtent, m_gDEM)
        Dim sTempTIN As String = WorkspaceManager.GetSafeDirectoryPath("TempTIN")
        Debug.WriteLine("TIN: " & sTempTIN)
        Debug.WriteLine("Raster: " & sOutputRaster)
        Dim gResult = pSurfaceInterpolator.ExecuteRBT(False, sTempTIN, sOutputRaster, GP.Analyst3D.DelaunayTriangulationTypes.ConstrainedDelauncy)

        'Validate raster output was created
        If GISDataStructures.Raster.Exists(sOutputRaster) Then
            gResult = New GISDataStructures.Raster(sOutputRaster)
        Else
            Throw New Exception(String.Format("The interpolation raster {0} was not created.", sOutputRaster))
        End If

        Return gResult
    End Function

    Public Function ExecuteUI(ByVal sOutputRaster As String, ByVal sZFieldName As String) As GISDataStructures.Raster

        Dim sTempPointFCPath = WorkspaceManager.GetTempShapeFile("in_memory")

        Dim pTempPointFC As GISDataStructures.PointDataSource = GISCode.GP.SpatialAnalyst.ExtractValuesToPoints(m_gPointsUI, m_gDEM, sTempPointFCPath)

        Dim geoProcessorEngine As ESRI.ArcGIS.Geoprocessing.IGeoProcessor2 = New ESRI.ArcGIS.Geoprocessing.GeoProcessor()
        geoProcessorEngine.AddOutputsToMap = False
        pTempPointFC.AddField(m_sInterpolationErrorField, ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeDouble)

        Dim pInterErrFieldIndex = pTempPointFC.FindField(m_sInterpolationErrorField)
        Dim pRasterValueField = pTempPointFC.FindField(m_RasterValueField)
        Dim pZFieldIndex As Integer
        Dim shapeFieldName As String = pTempPointFC.FeatureClass.ShapeFieldName
        Dim realShapeFieldName As String = String.Empty
        If String.Compare(sZFieldName, shapeFieldName & ".Z") = 0 Then
            realShapeFieldName = shapeFieldName
        End If


        If String.Compare(realShapeFieldName, shapeFieldName) = 0 Then
            pZFieldIndex = pTempPointFC.FindField(shapeFieldName)
        Else
            pZFieldIndex = pTempPointFC.FindField(sZFieldName)
        End If

        Dim pFeatureBuffer = pTempPointFC.FeatureClass.CreateFeatureBuffer()
        Dim pUpdateCursor As ESRI.ArcGIS.Geodatabase.IFeatureCursor = pTempPointFC.FeatureClass.Update(Nothing, False)
        Dim pRow As ESRI.ArcGIS.Geodatabase.IRow = pUpdateCursor.NextFeature()
        While Not pRow Is Nothing
            If pRow.Value(pRasterValueField) < 0 Then
                Debug.WriteLine("No data found.")
                pRow = pUpdateCursor.NextFeature()
                Continue While
            End If

            If String.Compare(realShapeFieldName, shapeFieldName) = 0 Then
                Dim pPoint As ESRI.ArcGIS.Geometry.IPoint = CType(pRow.Value(pZFieldIndex), ESRI.ArcGIS.Geometry.IPoint)
'<<<<<<< HEAD MERGE CONFLICT FROM RBTCONSOLE.
'                pRow.Value(pInterErrFieldIndex) = Math.Abs(pPoint.Z - pRow.Value(pRasterValueField))
'            Else
'                pRow.Value(pInterErrFieldIndex) = Math.Abs(pRow.Value(pZFieldIndex) - pRow.Value(pRasterValueField))
'=======
                pRow.Value(pInterErrFieldIndex) = Math.Abs(pPoint.Z - Double.Parse(pRow.Value(pRasterValueField)))
                Debug.Print(Math.Abs(pPoint.Z - pRow.Value(pRasterValueField)))
            Else
                pRow.Value(pInterErrFieldIndex) = Math.Abs(pRow.Value(pZFieldIndex) - pRow.Value(pRasterValueField))
                Debug.Print(Math.Abs(pRow.Value(pZFieldIndex) - pRow.Value(pRasterValueField)))

            End If
            pUpdateCursor.UpdateFeature(pRow)
            pRow = pUpdateCursor.NextFeature()
        End While

        Dim comReferencesLeft As Integer
        Do
            comReferencesLeft = System.Runtime.InteropServices.Marshal.ReleaseComObject(pUpdateCursor) _
                + System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureBuffer)
        Loop While (comReferencesLeft > 0)

        Dim gRaster As GISDataStructures.Raster = New GISDataStructures.Raster(m_gDEM.FullPath)
        Dim pSurfaceInterpolator As SurfaceInterpolator = New SurfaceInterpolator(pTempPointFC, m_sInterpolationErrorField, m_gSurveyExtent, gRaster)
        Dim sTempTIN As String = WorkspaceManager.GetSafeDirectoryPath("TempTIN")
        Debug.WriteLine("TIN: " & sTempTIN)
        Debug.WriteLine("Raster: " & sOutputRaster)
        Dim gResult = pSurfaceInterpolator.ExecuteRBT(False, sTempTIN, sOutputRaster, GP.Analyst3D.DelaunayTriangulationTypes.ConstrainedDelauncy)

        'Validate raster output was created
        If GISDataStructures.Raster.Exists(sOutputRaster) Then
            gResult = New GISDataStructures.Raster(sOutputRaster)
        Else
            Throw New Exception(String.Format("The interpolation raster {0} was not created.", sOutputRaster))
        End If

        Return gResult


    End Function

    Private Function GetESRIFieldType(sTypeName As String) As ESRI.ArcGIS.Geodatabase.esriFieldType

        Dim d As Double
        Dim s As Single
        Dim i16 As Int16
        Dim i32 As Int32

        Select Case sTypeName
            Case d.GetType().Name
                Return ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeDouble

            Case s.GetType().Name
                Return ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeSingle

            Case i16.GetType().Name
                Return ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeSmallInteger

            Case i32.GetType().Name
                Return ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeInteger

        End Select

    End Function


End Class
