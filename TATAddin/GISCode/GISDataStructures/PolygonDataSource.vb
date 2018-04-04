Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry

Namespace GISCode.GISDataStructures

    Public Class PolygonDataSource
        Inherits VectorDataSource

#Region "Properties"

        Public ReadOnly Property FirstPolygonArea As Double
            Get
                Dim pFCursor As IFeatureCursor = FeatureClass.Search(Nothing, True)
                Dim pFPoly As IFeature = pFCursor.NextFeature
                Dim pBoundingPolygon As IPolygon = pFPoly.ShapeCopy
                Dim pArea As IArea = pBoundingPolygon
                Return pArea.Area
            End Get
        End Property

        ''' <summary>
        ''' Cumulative length of all polylines in the feature class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TotalArea As Double
            Get
                Dim fTotal As Double = 0
                Dim pFCursor As IFeatureCursor = FeatureClass.Search(Nothing, True)
                Dim nShapeField As Integer = FeatureClass.FindField(FeatureClass.ShapeFieldName)
                Dim pFeature As IFeature = pFCursor.NextFeature
                Dim pArea As IArea
                Do While TypeOf pFeature Is IFeature
                    pArea = pFeature.Shape
                    fTotal += pArea.Area
                    pFeature = pFCursor.NextFeature
                Loop
                Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)
                pFCursor = Nothing
                Return fTotal
            End Get
        End Property

#End Region

        Public Sub New(sFullPath As String)
            MyBase.New(sFullPath)

            If GeometryType <> GeometryTypes.Polygon Then
                Dim ex As New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.CriticalError, "Invalid geometry type.")
                ex.Data("Feature Class") = FullPath
                ex.Data("Actual geometry type") = GeometryType
                ex.Data("Expected geometry type") = GeometryTypes.Polygon
                Throw ex
            End If

        End Sub

        'Public Overrides Sub Validate()
        '    MyBase.Validate()
        'End Sub

        Public Overrides Function Exists() As Boolean
            Return VectorDataSource.Exists(FullPath, ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
        End Function

        Public Sub CalculatePolygonAreas(sAreaFieldName As String)

            If String.IsNullOrEmpty(sAreaFieldName) Then
                Throw New ArgumentNullException("The area field name cannot be null or empty")
            End If

            Dim iAreaField As Integer = FindField(sAreaFieldName)
            If iAreaField >= 0 Then
                iAreaField = AddField(sAreaFieldName, esriFieldType.esriFieldTypeDouble)
                If iAreaField < 0 Then
                    Dim ex As New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.CriticalError, "Failed to add field to feature class")
                    ex.Data("Feature class") = FullPath
                    ex.Data("Field") = sAreaFieldName
                    Throw ex
                End If

                Dim pFeature As IFeature
                Dim pPolygon As IPolygon
                Dim dArea As Double = 0
                Dim pFCursor As IFeatureCursor = Me.FeatureClass.Search(Nothing, True)
                pFeature = pFCursor.NextFeature

                Do While TypeOf pFeature Is IFeature
                    pPolygon = pFeature.ShapeCopy
                    Dim pArea As IArea = pPolygon
                    dArea = pArea.Area
                    pFeature.Value(iAreaField) = dArea
                    pFeature.Store()
                    pFeature = pFCursor.NextFeature
                Loop
                pFCursor.Flush()

                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)
                'System.Runtime.InteropServices.Marshal.ReleaseComObject(pFCHabitat)
                pFCursor = Nothing
                'pFCHabitat = Nothing

            End If

        End Sub

        ''' <summary>
        ''' Loop over all the features and find the closest one.
        ''' </summary>
        ''' <param name="pQry">Optional query filter to only loop over specific features</param>
        ''' <returns>Largest polygon in the feature class</returns>
        ''' <remarks></remarks>
        Public Function GetLargestPolygon(Optional pQry As IQueryFilter = Nothing) As IPolygon

            Dim pFC As IFeatureCursor = FeatureClass.Search(pQry, True)
            Dim pFeature As IFeature = pFC.NextFeature
            Dim pResult As IPolygon = Nothing
            Dim fLargestArea As Double = -1
            Dim pArea As IArea = Nothing
            Do While TypeOf pFeature Is IFeature
                pArea = pFeature.Shape
                If pArea.Area > fLargestArea Then
                    fLargestArea = pArea.Area
                    pResult = pFeature.ShapeCopy
                End If
                Runtime.InteropServices.Marshal.ReleaseComObject(pFeature)
                pFeature = pFC.NextFeature
            Loop
            Runtime.InteropServices.Marshal.ReleaseComObject(pFC)
            pFC = Nothing

            Return pResult

        End Function

        '
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' The following methods were taken from the old "Features" namespace.
        '
        ''' <summary>
        ''' Open an ArcCatalog browser window and navigate to a feature class
        ''' </summary>
        ''' <param name="sTitle">Browser window title</param>
        ''' <param name="sFolder">Output folder of selected feature class</param>
        ''' <param name="sFCName">Output feature class name</param>
        ''' <returns>The fully qualified (geoprocessing compatible) path to the selected feature class</returns>
        ''' <remarks>This method now works with both ShapeFile sources and geodatabase sources.</remarks>
        Public Shared Shadows Function BrowseOpen(ByVal sTitle As String, ByRef sFolder As String, ByRef sFCName As String, hParentWindowHandle As System.IntPtr) As GISDataStructures.PolygonDataSource
            Return GISDataStructures.VectorDataSource.BrowseOpen(sTitle, sFolder, sFCName, GISCode.GISDataStructures.GeometryTypes.Polygon, hParentWindowHandle)
        End Function

        'Public Function SelectPolygons(ByVal gThalweg As GISDataStructures.LongLine) As List(Of IPolygon)

        '    Dim lPolygons As List(Of IPolygon)
        '    lPolygons = Me.GetPolygons()

        '    Dim lSelectedPolygons As New List(Of IPolygon)
        '    For Each pPolygon In lPolygons
        '        Dim pRelationalOperator As IRelationalOperator = pPolygon
        '        If pRelationalOperator.Contains(gThalweg.FirstFeature) Then
        '            lSelectedPolygons.Add(pPolygon)
        '        End If
        '    Next

        '    Return lSelectedPolygons

        'End Function

        Public Function GetPolygons() As List(Of IPolygon)

            Dim exteriorRingGeometryBag As IGeometryBag
            Dim exteriorRingGeometryCollection As IGeometryCollection
            Dim exteriorRingGeometry As IGeometry

            ' PGB - 14 Nov 2014 - rename this variable to exterior polygon?
            Dim pInteriorPolygon As IGeometryCollection

            'get all polygons
            'maybe move to polygon?
            Dim lPolygons As New List(Of IPolygon)


            Dim fcWaterExtent As IFeatureClass = Me.FeatureClass
            Dim pFCursor As IFeatureCursor = fcWaterExtent.Search(Nothing, True)
            Dim pFeature As IFeature = pFCursor.NextFeature
            While Not pFeature Is Nothing
                Dim pPolygon As IPolygon4 = pFeature.ShapeCopy
                exteriorRingGeometryBag = pPolygon.ExteriorRingBag

                For index = 0 To pPolygon.ExteriorRingCount - 1
                    exteriorRingGeometryCollection = exteriorRingGeometryBag
                    exteriorRingGeometry = exteriorRingGeometryCollection.Geometry(index)
                    pInteriorPolygon = New Polygon
                    pInteriorPolygon.AddGeometry(exteriorRingGeometry)
                    lPolygons.Add(pInteriorPolygon)
                Next
                pFeature = pFCursor.NextFeature
            End While

            If pFCursor IsNot Nothing Then
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)
                pFCursor = Nothing
            End If

            Return lPolygons

        End Function

    End Class

End Namespace