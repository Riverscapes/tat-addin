Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry

Namespace GISCode.GISDataStructures

    Public Class PointDataSource3D
        Inherits PointDataSource

        Public Sub New(sFilePath As String)
            MyBase.new(sFilePath)
        End Sub

        Public Shadows Function Validate(ByRef lErrors As List(Of GISException), pReferenceSR As ISpatialReference) As Boolean
            MyBase.Validate(lErrors, pReferenceSR)

            Try
                If Not Is3D Then
                    Throw New GISException(GISException.ErrorTypes.CriticalError, "Feature class is not 3D and does not have Z values", "Recreate the feature class capable of storing Z values")
                End If
            Catch ex As GISException
                ex.Data("File Path") = FullPath
                lErrors.Add(ex)
            End Try

            Return lErrors.Count = 0

        End Function

        Public Sub GetMinMaxElevations(ByRef fMinElevation As Double, ByRef fMaxElevation As Double, Optional pQueryFilter As IQueryFilter = Nothing)
            fMinElevation = 0
            fMaxElevation = 0

            Dim bValues As Boolean = False
            Dim fMinZ As Double = 99999
            Dim fMaxZ As Double = 0
            Dim pFC As IFeatureCursor = FeatureClass.Search(pQueryFilter, True)
            Dim pFeature As IFeature = pFC.NextFeature
            Do While TypeOf pFeature Is IFeature
                Dim fZ As Double = DirectCast(pFeature.Shape, IPoint).Z
                bValues = True
                If fZ < fMinZ Then
                    fMinZ = fZ
                End If

                If fZ > fMaxZ Then
                    fMaxZ = fZ
                End If
                pFeature = pFC.NextFeature
            Loop
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFC)

            If bValues Then
                fMinElevation = fMinZ
                fMaxElevation = fMaxZ
            End If

        End Sub

        Public Shared Function Get3DDistance(ByVal pA As IPoint, ByVal pB As IPoint) As Double

            Debug.Assert(TypeOf pA Is IPoint)
            Debug.Assert(TypeOf pB Is IPoint)

            Return Math.Sqrt((pA.X - pB.X) ^ 2 + (pA.Y - pB.Y) ^ 2 + (pA.Z - pB.Z) ^ 2)

        End Function

    End Class

End Namespace