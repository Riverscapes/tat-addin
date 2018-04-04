Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports System.Runtime.InteropServices

Namespace GISCode.GISDataStructures

    Public Class PolylineDataSource
        Inherits VectorDataSource

        ''' <summary>
        ''' Enumeration representing the from and to nodes of a line
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum EndEnum
            eFrom
            eTo
        End Enum

        ''' <summary>
        ''' Create a new polyline data source
        ''' </summary>
        ''' <param name="sFullPath">Full path to the feature class</param>
        ''' <remarks></remarks>
        Public Sub New(sFullPath As String)
            MyBase.New(sFullPath)

            If GeometryType <> GeometryTypes.Line Then
                Dim ex As New GISException(GISException.ErrorTypes.CriticalError, "Invalid geometry type.")
                ex.Data("Feature Class") = FullPath
                ex.Data("Actual geometry type") = GeometryType
                ex.Data("Expected geometry type") = GeometryTypes.Line
                Throw ex
            End If

        End Sub

        ''' <summary>
        ''' Cumulative length of all polylines in the feature class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TotalLength As Double
            Get
                Dim fLength As Double = 0
                Dim pFCursor As IFeatureCursor = FeatureClass.Search(Nothing, True)
                Dim nShapeField As Integer = FeatureClass.FindField(FeatureClass.ShapeFieldName)
                Dim pFeature As IFeature = pFCursor.NextFeature
                Dim pPolyline As IPolyline
                Do While TypeOf pFeature Is IFeature
                    pPolyline = pFeature.Shape
                    fLength += pPolyline.Length
                    pFeature = pFCursor.NextFeature
                Loop
                Marshal.ReleaseComObject(pFCursor)
                pFCursor = Nothing
                Return fLength

            End Get
        End Property

        ''' <summary>
        ''' The "from" point of the first feature in the feature class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FromPointOfFirstFeature As IPoint
            Get
                Dim pPoint As IPoint = New Point
                Dim pPolyline As IPolyline = FirstFeature
                pPoint.PutCoords(pPolyline.FromPoint.X, pPolyline.FromPoint.Y)
                Return pPoint
            End Get
        End Property

        ''' <summary>
        ''' The "to" point of the first feature in the feature class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ToPointOfFirstFeature As IPoint
            Get
                Dim pPoint As IPoint = New Point
                Dim pPolyline As IPolyline = FirstFeature
                pPoint.PutCoords(pPolyline.ToPoint.X, pPolyline.ToPoint.Y)
                Return pPoint
            End Get
        End Property

        ''' <summary>
        ''' The first feature of the feature class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FirstFeature As IPolyline
            Get
                Dim pPolyline As IPolyline = Nothing
                Dim pFCursor As IFeatureCursor = FeatureClass.Search(Nothing, True)
                Dim pFeature As IFeature = pFCursor.NextFeature
                pPolyline = pFeature.ShapeCopy
                Marshal.ReleaseComObject(pFCursor)
                pFCursor = Nothing
                Return pPolyline
            End Get
        End Property

        ''' <summary>
        ''' Returns the longest line from a featureclass with polyline shapes
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property LongestLine As IPolyline
            Get
                Dim fLength As Double = -1
                Dim MaxLength As Double = -1
                Dim MaxLengthLine As IPolyline = Nothing
                Dim MaxLengthOID As Integer = 0
                Dim pLine As IPolyline = Nothing
                Dim pClone As ESRI.ArcGIS.esriSystem.IClone
                Dim pCursor As IFeatureCursor = FeatureClass.Search(Nothing, True)
                Dim pFeature As IFeature = pCursor.NextFeature

                While pFeature IsNot Nothing
                    pLine = pFeature.Shape
                    fLength = pLine.Length
                    If fLength > MaxLength Then
                        MaxLength = fLength
                        pClone = pLine
                        MaxLengthLine = pClone.Clone
                        MaxLengthOID = pFeature.OID
                    End If
                    pFeature = pCursor.NextFeature
                End While
                Runtime.InteropServices.Marshal.ReleaseComObject(pCursor)
                '
                ' PGB 21 Jun 2013. Why delete this feature?
                '
                'pFClass.GetFeature(MaxLengthOID).Delete()
                Return MaxLengthLine
            End Get
        End Property

        ''' <summary>
        ''' The number of parts comprising the first feature
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PartsCount As Integer
            Get
                Dim pGeomColl As IGeometryCollection = FirstFeature
                Return pGeomColl.GeometryCount
            End Get
        End Property

        'Public Shadows Function Validate(lErrors As List(Of GISException), pReferenceSR As ISpatialReference)
        '    Return MyBase.Validate(lErrors, pReferenceSR)
        'End Function

        Public Overrides Function Exists() As Boolean
            Return VectorDataSource.Exists(FullPath, ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
        End Function

        ''' <summary>
        ''' Extend the specified end of the line by the specified distance
        ''' </summary>
        ''' <param name="pLine">The line to extend</param>
        ''' <param name="eEnd">Enumeration specifying which end to extend</param>
        ''' <param name="fLength">The length to extend the line by. Units are the same as the line</param>
        ''' <returns>The line extended by the specified length</returns>
        ''' <remarks></remarks>
        Public Shared Function ExtendLineByLength(ByVal pLine As ILine, ByVal eEnd As GISDataStructures.PolylineDataSource.EndEnum, ByVal fLength As Double) As ILine

            If fLength <= 0 Then
                Dim ex As New Exception("Invalid cross section extension distance")
                ex.Data("Extension distance") = fLength
                Throw ex
            End If

            Dim pPoint As IPoint = New Point

            If eEnd = GISDataStructures.PolylineDataSource.EndEnum.eFrom Then
                pPoint.X = pLine.FromPoint.X - Math.Cos(pLine.Angle) * fLength
                pPoint.Y = pLine.FromPoint.Y - Math.Sin(pLine.Angle) * fLength
                pLine.FromPoint = pPoint
            Else
                pPoint.X = pLine.ToPoint.X + Math.Cos(pLine.Angle) * fLength
                pPoint.Y = pLine.ToPoint.Y + Math.Sin(pLine.Angle) * fLength
                pLine.ToPoint = pPoint
            End If

            System.Runtime.InteropServices.Marshal.ReleaseComObject(pPoint)
            pPoint = Nothing

            Return pLine

        End Function

        Public Function WriteLineEndPointsToFeatureClass(ByVal sOutEndPoints As String) As Integer
            '
            ' output count of the number of points written to file
            '
            Dim nPointCount As Integer = 0
            Dim pLine As IPolyline = Nothing
            Dim pFCEndPoints As IFeatureClass = Nothing
            'Dim pFCThalweg As IFeatureClass = FeatureClass.GetFeatureClass(sLineFeatureClass)
            'If TypeOf pFCThalweg Is IFeatureClass Then
            'Dim pSpatialRef As ISpatialReference = FeatureClass.GetSpatialReference(pFCThalweg)
            'If TypeOf pSpatialRef Is ISpatialReference Then

            Dim gEndPoints As GISDataStructures.PointDataSource = VectorDataSource.CreateFeatureClass(sOutEndPoints, BasicGISTypes.Point, True, SpatialReference)
            'pFCEndPoints = FeatureClass.CreateNewShapeFile(IO.Path.GetFileName(sEndPoints), IO.Path.GetDirectoryName(sEndPoints), esriGeometryPoint, True, pSpatialRef)
            'End If
            '
            ' Get the first feature from the Thalweg feature class
            '
            Dim pFThalwegCursor As IFeatureCursor = FeatureClass.Search(Nothing, True)
            If TypeOf pFThalwegCursor Is IFeatureCursor Then
                Dim pFeature As IFeature = pFThalwegCursor.NextFeature
                If TypeOf pFeature Is IFeature Then
                    If TypeOf pFeature.Shape Is IPolyline Then
                        pLine = pFeature.Shape
                    End If
                End If
            End If
            'End If

            If Not TypeOf pFCEndPoints Is IFeatureClass Then
                ' Need a shapefile to continue
                ' 
                Return 0
            End If

            Dim nSequenceFld As Integer = gEndPoints.AddField(CrossSectionFields.Sequence, esriFieldType.esriFieldTypeInteger)
            If nSequenceFld <= 0 Then
                '
                ' Need a sequence field to continue
                '
                Return 0
            End If
            '
            ' Use a curve as it allows us to return any distance along the line
            '
            Dim pCurve As ICurve = pLine
            Dim pFCursor As IFeatureCursor
            Dim pFBuffer As IFeatureBuffer = pFCEndPoints.CreateFeatureBuffer
            Dim pFeat As IFeature
            Dim nSequence As Integer = 1
            pFCursor = pFCEndPoints.Insert(True)
            pFeat = pFBuffer
            Dim pPoint As IPoint = pCurve.FromPoint
            Dim zPoint As IZAware = pPoint
            zPoint.ZAware = True
            pPoint.Z = 0


            ' Start point of the line
            pFeat.Shape = pPoint 'pCurve.FromPoint
            pFeat.Value(nSequenceFld) = nSequence
            pFCursor.InsertFeature(pFBuffer)


            ' end point of the line
            pPoint = pCurve.ToPoint
            zPoint = pPoint
            zPoint.ZAware = True
            pPoint.Z = 0


            pFeat.Shape = pPoint ' pCurve.ToPoint
            pFeat.Value(nSequenceFld) = nSequence + 1
            pFCursor.InsertFeature(pFBuffer)
            '
            ' Flush the feature buffer to actually write all features to the shapefile.
            '
            pFCursor.Flush()
            '
            ' Get the current number of features in the feature class
            '
            nPointCount = pFCEndPoints.FeatureCount(Nothing)


            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFBuffer)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFCEndPoints)
            pFCursor = Nothing
            pFBuffer = Nothing
            pFCEndPoints = Nothing

            Return nPointCount

        End Function

        ''' <summary>
        ''' Deletes the nearest line segment to a point
        ''' </summary>
        ''' <param name="pPoint">A valid point</param>
        ''' <remarks>Originally designed as a supporting method to DensifyBanks. FP.</remarks>
        Public Sub DeleteNearestLineToPoint(ByVal pPoint As IPoint)

            Dim pFCursor As IFeatureCursor
            'Dim pFClass As IFeatureClass
            Dim feature As IFeature
            Dim fDistanceAlongCurve As Double = 0
            Dim fDistanceFromCurve As Double = 0
            Dim bRightSide As Boolean = False
            Dim pClosestPointOnCurve As IPoint = New Point
            Dim ClosestDistance As Double = -1
            Dim closestFeature As Feature
            Dim closestFeatureOID As Integer = -1
            Dim pShape As IGeometry
            Dim pCurve As ICurve

            'validating inputs
            If pPoint Is Nothing Then
                Throw New ArgumentNullException("pPoint", "Null or empty argument pPoint. Expected an IPoint object")
            End If

            If Not TypeOf (pPoint) Is IPoint Then
                Throw New ArgumentException("pPoint", "pPoint is not an IPoint object")
            End If

            'If String.IsNullOrEmpty(sLineSegs) Then
            '    Throw New ArgumentNullException("sLineSegs", "Null or empty argument sLineSegs. Expected a path to a line shapefile")
            'End If

            'If Not FeatureClass.Exists(sLineSegs, esriDatasetType.esriDTFeatureClass) Then
            '    Throw New ArgumentException("sLineSegs", "'" & sLineSegs & " does not exist.")
            'End If

            'Get feature cursor
            'pFClass = FeatureClass.GetFeatureClass(sLineSegs)

            'validate featureclass has OID
            'If Not pFClass.HasOID Then
            '    Throw New ArgumentException("sLineSegs", "'" & sLineSegs & " does not have an OID.")
            'End If

            'get feature cursor
            pFCursor = FeatureClass.Search(Nothing, True)
            feature = pFCursor.NextFeature

            While Not feature Is Nothing

                'get shape and validate that its an iCurve
                pShape = feature.ShapeCopy
                If Not TypeOf (pShape) Is ICurve Then
                    Throw New ArgumentException("sLineSegs", "'" & FullPath & " shape type is not ICurve.")
                End If
                pCurve = pShape

                'Get distance from line to point
                pCurve.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, pPoint, False, pClosestPointOnCurve, fDistanceAlongCurve, fDistanceFromCurve, bRightSide)

                'If this is the line segment closest to the point, track the OID
                If fDistanceFromCurve < ClosestDistance Or ClosestDistance < 0 Then
                    ClosestDistance = fDistanceFromCurve
                    closestFeatureOID = feature.OID
                End If

                feature = pFCursor.NextFeature
            End While
            Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)

            'If closest feature was found (closestFeatureOID > -1), delete record.
            'If there are no features in shapefile, closestFeatureOID = -1
            If closestFeatureOID > -1 Then
                Try
                    closestFeature = FeatureClass.GetFeature(closestFeatureOID)
                    closestFeature.Delete()
                Catch ex As Exception
                    Dim ex2 As New Exception("Could not delete feature with OID " & closestFeatureOID & " from '" & FullPath & "'", ex)
                    Throw ex2
                End Try

            End If
        End Sub

        ''' <summary>
        ''' Creates a new field on the point shapefile and populates it with the distance a along the line in the sLine shapefile
        ''' </summary>
        ''' <param name="gPoints">Full path of points shapefile.</param>
        ''' <param name="sDistanceFieldname">Optional name of the distance field. Defaults to "distance". Must not already exist.</param>
        ''' <remarks></remarks>
        Public Sub PointDistanceOnLine(ByVal gPoints As GISDataStructures.PointDataSource,
                                       Optional ByVal sDistanceFieldname As String = "distance")
            'Dim pFCLine As IFeatureClass
            Dim pGeom As IGeometry
            Dim pCurve As ICurve
            'Dim pFCPoints As IFeatureClass
            Dim nDistanceField As Integer
            Dim pFCursor As IFeatureCursor = Nothing
            Dim pPoint As IFeature
            Dim pVertPoint As IPoint
            Dim fDistanceAlongLine As Double
            Dim fDistanceFromLine As Double
            Dim bRightSide As Boolean
            Dim pClosestPointOnThalweg As IPoint = Nothing

            Try
                '
                'Validation
                '
                If String.IsNullOrEmpty(sDistanceFieldname) Then
                    Throw New ArgumentNullException("sDistanceFieldname", "The sDistanceFieldname input string cannot be null or empty")
                End If

                'check that the fieldname doesnt already exists
                nDistanceField = gPoints.FeatureClass.FindField(sDistanceFieldname)
                If nDistanceField >= 0 Then
                    Throw New ArgumentException("sDistanceFieldname", "The field '" & sDistanceFieldname & "' already exists in shapefile '" & gPoints.FullPath & "'.")
                End If

                'add new field
                nDistanceField = gPoints.AddField(sDistanceFieldname, esriFieldType.esriFieldTypeDouble)
                If nDistanceField < 0 Then
                    Throw New Exception("Unable to create the field '" & sDistanceFieldname & "' in shapefile '" & gPoints.FullPath & "'.")
                End If

                'check that there is only on feature
                Dim iFeatureCount As Integer = Me.FeatureClass.FeatureCount(Nothing)
                If FeatureCount <> 1 Then
                    Dim ex As New Exception("There can only be one feature in the line feature class")
                    ex.Data("Feature class") = FullPath
                    ex.Data("Actual Feature Count") = FeatureCount
                    ex.Data("Required feature count") = 1.ToString
                    Throw ex
                End If

                pGeom = GetFirstGeometry
                'Check geometry is of type ICurve
                If Not TypeOf (pGeom) Is ICurve Then
                    Throw New ArgumentException("sLine", "The shapefile '" & FullPath & "' should be a polyline shapefile.")
                End If
                pCurve = pGeom

                'check that the feature only have one part (i.e. single part)
                Dim pGeometryCollection As IGeometryCollection
                pGeometryCollection = pGeom
                If pGeometryCollection.GeometryCount > 1 Then
                    Throw New ArgumentException("sLine", "The polyline in shapefile '" & FullPath & "' should be singlepart.")
                End If

                'Get cursor
                pFCursor = gPoints.FeatureClass.Update(Nothing, True)
                pPoint = pFCursor.NextFeature

                'loop through features
                Do While TypeOf pPoint Is IFeature

                    'find distance along line
                    pVertPoint = pPoint.Shape
                    pCurve.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, pVertPoint, False, pClosestPointOnThalweg, fDistanceAlongLine, fDistanceFromLine, bRightSide)

                    'update new distance field with value for distance along line
                    pPoint.Value(nDistanceField) = fDistanceAlongLine
                    pFCursor.UpdateFeature(pPoint)

                    'next feature
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pPoint)
                    pPoint = pFCursor.NextFeature
                Loop

            Catch ex As Exception
                ex.Data("gLine") = FullPath
                ex.Data("gPoints") = gPoints.FullPath
                ex.Data("sDistanceFieldname") = sDistanceFieldname
                Throw
            Finally
                'cleanup
                If pFCursor IsNot Nothing Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)
                    pFCursor = Nothing
                End If
            End Try

        End Sub

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
        Public Overloads Shared Function BrowseOpen(ByVal sTitle As String, ByRef sFolder As String, ByRef sFCName As String, hParentWindowHandle As System.IntPtr) As GISDataStructures.PolylineDataSource
            Return GISDataStructures.VectorDataSource.BrowseOpen(sTitle, sFolder, sFCName, GISCode.GISDataStructures.GeometryTypes.Line, hParentWindowHandle)
        End Function

        ''' <summary>
        ''' Calculates the length of the polyline and writes it to the shapefile
        ''' </summary>
        ''' <param name="sFieldname"></param>
        ''' <remarks>
        ''' Created by FP July 11 2013
        ''' </remarks>
        Public Sub CalculateLength(Optional ByVal sFieldname As String = "length")

            Dim pFeatureCursor As IFeatureCursor = Nothing
            Dim pFeature As IFeature = Nothing

            Try

                Dim pFC As IFeatureClass = Me.FeatureClass

                Dim iLengthFieldIndex As Integer = Me.AddField(sFieldname, esriFieldType.esriFieldTypeDouble)
                If iLengthFieldIndex < 0 Then
                    Dim sMessage As String = "Could not create field '" & sFieldname & "' in '" & Me.FullPath & "'"
                    Dim ex As New GISException(GISException.ErrorTypes.CriticalError, sMessage)
                    Throw ex
                End If

                pFeatureCursor = pFC.Update(Nothing, True)
                pFeature = pFeatureCursor.NextFeature
                While pFeature IsNot Nothing
                    Dim pPolyline As IPolyline = pFeature.Shape
                    Dim dLength As Double = pPolyline.Length
                    pFeature.Value(iLengthFieldIndex) = dLength
                    pFeatureCursor.UpdateFeature(pFeature)

                    'cleanup
                    If pFeature IsNot Nothing Then
                        Marshal.ReleaseComObject(pFeature)
                        pFeature = Nothing
                    End If

                    pFeature = pFeatureCursor.NextFeature
                End While

                'cleanup
            Catch ex As Exception
                ex.Data("fullpath") = Me.FullPath
                ex.Data("sFieldname") = sFieldname
            Finally
                If pFeatureCursor IsNot Nothing Then
                    Marshal.ReleaseComObject(pFeatureCursor)
                    pFeatureCursor = Nothing
                End If

                If pFeature IsNot Nothing Then
                    Marshal.ReleaseComObject(pFeature)
                    pFeature = Nothing
                End If

            End Try

        End Sub

        'Public Overrides Sub Validate()
        '    MyBase.Validate()
        'End Sub

    End Class

End Namespace