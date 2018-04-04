Imports ESRI.ArcGIS.Geometry

Namespace GISCode

    Public Class SurfaceInterpolator

        Private m_gPoints As GISDataStructures.PointDataSource
        Private m_sPointField As String
        Private m_gExtent As GISDataStructures.PolygonDataSource
        Private m_gReferenceRaster As GISDataStructures.Raster

        ''' <summary>
        ''' UI Constuctor
        ''' </summary>
        ''' <param name="pPoints">Points to be interpolated from</param>
        ''' <param name="sField">Field name of field to be used for interpolation</param>
        ''' <param name="pExtent">Survey Extent</param>
        ''' <remarks></remarks>
        Public Sub New(pPoints As GISDataStructures.PointDataSource, sField As String, pExtent As GISDataStructures.PolygonDataSource)
            m_gPoints = pPoints
            m_sPointField = sField
            m_gExtent = pExtent
        End Sub

        ''' <summary>
        ''' RBT Constructor
        ''' </summary>
        ''' <param name="pPoints">Qa_QcRawPoints</param>
        ''' <param name="sField">Field name for point quality field, eg "POINT_QUALITY"</param>
        ''' <param name="pExtent">Survey Extent</param>
        ''' <param name="gReferenceRaster">DEM</param>
        ''' <remarks></remarks>
        Public Sub New(pPoints As GISDataStructures.PointDataSource, sField As String, pExtent As GISDataStructures.PolygonDataSource, gReferenceRaster As GISDataStructures.Raster)
            m_gPoints = pPoints
            m_sPointField = sField
            m_gExtent = pExtent
            m_gReferenceRaster = gReferenceRaster
        End Sub

        ''' <summary>
        ''' Interpolates an orthogonal and concurrent surface with the provided reference raster
        ''' </summary>
        ''' <param name="bDeleteTin">boolean to determine if the TIN should be deleted</param>
        ''' <param name="sOutTin">path to create the potentially temporary TIN</param>
        ''' <param name="sOutDEM">path for the output Raster</param>
        ''' <param name="eInterpolationType">TIN Triangulation type</param>
        ''' <returns>GISDataStructures.Raster that is concurrent and orthogonal with reference raster</returns>
        ''' <remarks></remarks>
        Public Function ExecuteRBT(bDeleteTin As Boolean, sOutTin As String, sOutDEM As String, Optional ByVal eInterpolationType As GP.Analyst3D.DelaunayTriangulationTypes = GP.Analyst3D.DelaunayTriangulationTypes.ConstrainedDelauncy) As GISDataStructures.Raster

            Dim spatialRef As ESRI.ArcGIS.Geometry.ISpatialReference = Nothing
            Dim dCellSize As Double
            If TypeOf (m_gReferenceRaster) Is GISDataStructures.Raster Then
                spatialRef = m_gReferenceRaster.SpatialReference
                dCellSize = m_gReferenceRaster.CellSize
                If Not m_gPoints.CheckSpatialReferenceMatches(spatialRef) Then
                    Throw New System.Exception(String.Format("The spatial reference of {0} is either unknown or does not match the spatial reference {1} provided for this operation", m_gPoints.FullPath, m_gReferenceRaster.SpatialReference))
                End If

                If Not m_gExtent.CheckSpatialReferenceMatches(spatialRef) Then
                    Throw New System.Exception(String.Format("The spatial reference of {0} is either unknown or does not match the spatial reference {1} provided for this operation", m_gExtent.FullPath, m_gReferenceRaster.SpatialReference))
                End If
            ElseIf Not TypeOf (m_gReferenceRaster) Is GISDataStructures.Raster Then
                Throw New System.Exception("The reference raster used is not valid raster")
            End If

            ' Do the guts here!
            Dim pMassPoints As GISCode.GP.Analyst3D.TinMassPoints = New GISCode.GP.Analyst3D.TinMassPoints(m_gPoints.FeatureClass, m_sPointField)

            'Craete the TinFeatures for TinFeatureTypes.HardClip
            Dim pSoftClip As GISCode.GP.Analyst3D.TinFeatures = New GISCode.GP.Analyst3D.TinFeatures(m_gExtent.FeatureClass, Nothing, GISCode.GP.Analyst3D.Analyst3D.TinFeatureTypes.SoftClip)

            'Create a dictionary of string paths to feature classes and/or TinFeature/TinFeatureMass
            Dim dFeatures As New Dictionary(Of String, GP.Analyst3D.TiNFeaturesBase)

            dFeatures.Add(m_gPoints.FullPath, pMassPoints)
            dFeatures.Add(m_gExtent.FullPath, pSoftClip)

            Dim gTIN As GISCode.GISDataStructures.TINDataSource = GP.Analyst3D.CreateTIN(sOutTin, spatialRef, dFeatures, eInterpolationType)

            'Had to change Sampling method from Natural Neighbors to Linear (would not work in ESRI geoprocessing tool either)

            Dim sTemp As String = WorkspaceManager.GetTempRaster("IErr")
            Dim gExtent As GISDataStructures.ExtentRectangle = New GISDataStructures.ExtentRectangle(m_gReferenceRaster.Extent.YMax, m_gReferenceRaster.Extent.XMin, m_gReferenceRaster.Extent.XMax, m_gReferenceRaster.Extent.YMin)
            GP.Analyst3D.TINtoRaster(gTIN, sTemp, m_gReferenceRaster.CellSize, GP.Analyst3D.RasterDataTypes.FloatValues, GP.Analyst3D.SamplingMetods.Linear, 1, gExtent) ', GP.Analyst3D.RasterDataTypes.FloatValues, GP.Analyst3D.SamplingMetods.Linear, 1.0, 2.0, m_gReferenceRaster)
            GP.DataManagement.CopyRaster(sTemp, sOutDEM)

            If bDeleteTin Then
                Try
                    gTIN.Delete()
                Catch ex As Exception
                    'Do nothing
                End Try
            End If

            ' Validate that the things created are valid!
            Dim gResult As GISDataStructures.Raster = Nothing
            If GISDataStructures.Raster.Exists(sOutDEM) Then
                gResult = New GISDataStructures.Raster(sOutDEM)
            End If

            Return gResult

        End Function

        Public Function ExecuteUI(bCreateDEM As Boolean,
                                  bDeleteTin As Boolean,
                                  fCellSize As Double,
                                  spatialRef As ESRI.ArcGIS.Geometry.ISpatialReference,
                                  sOutTin As String,
                                  sOutDEM As String,
                                  eTriangulationType As GP.Analyst3D.DelaunayTriangulationTypes,
                                  Optional eInterpolationMethod As GP.Analyst3D.SamplingMetods = GP.Analyst3D.SamplingMetods.Linear) As GISDataStructures.Raster

            'Check if spatial references of input feature classes match the global spatial reference object
            'if they do not but the they are unknown (i.e.) can be projected by later operations allow the user the option to continue
            Dim bContinue As Boolean = False
            If Not m_gPoints.CheckSpatialReferenceMatches(spatialRef) Then
                If String.Compare(m_gPoints.SpatialReference.Name, "Unknown") = 0 Then
                    bContinue = CollectUnkownSpatialReferenceResponse(m_gPoints.DatasetName)
                    If bContinue = False Then
                        Return Nothing
                    End If
                Else
                    MsgBox(m_gPoints.DatasetName & " does not match the provided spatial reference.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                    Return Nothing
                End If
            End If

            If Not m_gExtent.CheckSpatialReferenceMatches(spatialRef) Then
                If String.Compare(m_gExtent.SpatialReference.Name, "Unkown") = 0 Then
                    bContinue = CollectUnkownSpatialReferenceResponse(m_gExtent.DatasetName)
                    If bContinue = False Then
                        Return Nothing
                    End If
                Else
                    MsgBox(m_gExtent.DatasetName & " does not match the provided spatial reference.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                    Return Nothing
                End If
            End If


            ' Do the guts here!
            Dim pMassPoints As GISCode.GP.Analyst3D.TinMassPoints = New GISCode.GP.Analyst3D.TinMassPoints(m_gPoints.FeatureClass, m_sPointField)

            'Craete the TinFeatures for TinFeatureTypes.HardClip
            Dim pSoftClip As GISCode.GP.Analyst3D.TinFeatures = New GISCode.GP.Analyst3D.TinFeatures(m_gExtent.FeatureClass, Nothing, GISCode.GP.Analyst3D.Analyst3D.TinFeatureTypes.SoftClip)

            'Create a dictionary of string paths to feature classes and/or TinFeature/TinFeatureMass
            Dim dFeatures As New Dictionary(Of String, GP.Analyst3D.TiNFeaturesBase)

            dFeatures.Add(m_gPoints.FullPath, pMassPoints)
            dFeatures.Add(m_gExtent.FullPath, pSoftClip)


            Dim pTIN As GISCode.GISDataStructures.TINDataSource = GP.Analyst3D.CreateTIN(sOutTin, spatialRef, dFeatures, eTriangulationType)

            'Had to change Sampling method from Natural Neighbors to Linear (would not work in ESRI geoprocessing tool either)
            If bCreateDEM Then

                GP.Analyst3D.TINtoRaster_Orthogonal(pTIN,
                                                    sOutDEM,
                                                    fCellSize,
                                                    GISCode.GP.Analyst3D.RasterDataTypes.FloatValues,
                                                    eInterpolationMethod,
                                                    1.0,
                                                    2.0)

            End If

            If bDeleteTin Then
                Try
                    pTIN.Delete()
                Catch ex As Exception
                    'Do nothing
                End Try
            End If

            ' Validate that the things created are valid!
            Dim gResult As GISDataStructures.Raster = Nothing
            If GISDataStructures.Raster.Exists(sOutDEM) Then
                gResult = New GISDataStructures.Raster(sOutDEM)
            End If

            Return gResult

        End Function

        Private Function CollectUnkownSpatialReferenceResponse(ByVal sVectorName As String) As Boolean
            Dim response As New MsgBoxResult
            response = MsgBox("The spatial reference of the input feature " & sVectorName & " is unknown, this can cause problems when creating a TIN." & vbCrLf & vbCrLf & "Would you like to try and create a TIN and/or DEM anyhow?", MsgBoxStyle.YesNo, My.Resources.ApplicationNameLong)
            If response = MsgBoxResult.Yes Then
                Return True
            ElseIf response = MsgBoxResult.No Then
                Return False
            End If
        End Function

    End Class

End Namespace