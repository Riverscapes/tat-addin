Namespace GISCode.GISDataStructures

    ''' <summary>
    ''' Represents rectangular extents
    ''' </summary>
    ''' <remarks>PGB - Aug 2012 - This class represents rectangle extents. It's mainly intended for
    ''' use with DEM raster extents and orthogonality. Note: this class does not perform any orthogonality
    ''' calculations itself. It is intended to capture just the coordinates. This class is used by the
    ''' RasterExtentAdjuster for actually performing orthogonality changes.</remarks>
    Public Class ExtentRectangle

#Region "Members"
        Private m_fTop As Double
        Private m_fLeft As Double
        Private m_fRight As Double
        Private m_fBottom As Double
#End Region

#Region "Properties"
        Public ReadOnly Property Top As Double
            Get
                Return m_fTop
            End Get
        End Property

        Public ReadOnly Property Left As Double
            Get
                Return m_fLeft
            End Get
        End Property

        Public ReadOnly Property Right As Double
            Get
                Return m_fRight
            End Get
        End Property

        Public ReadOnly Property Bottom As Double
            Get
                Return m_fBottom
            End Get
        End Property

        Public ReadOnly Property Height As Double
            Get
                Return m_fTop - m_fBottom
            End Get
        End Property

        Public ReadOnly Property Width As Double
            Get
                Return m_fRight - m_fLeft
            End Get
        End Property

        ''' <summary>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>The sequence of these argument is important. They must match the sequence that ESRI
        ''' expects when geoprocessing.</remarks>
        Public ReadOnly Property Rectangle As String
            Get
                Return Left & " " & Bottom & " " & Right & " " & Top
            End Get
        End Property

#End Region

#Region "Methods"

        Public Sub New(fTop As Double, fLeft As Double, fRight As Double, fBottom As Double)

            If fTop <= fBottom Then
                Throw New ArgumentOutOfRangeException("fTop", fTop, "The top coordinate cannot be less than or equal to the bottom")
            End If

            If fLeft >= fRight Then
                Throw New ArgumentOutOfRangeException("fLeft", fLeft, "The left coordinate cannot be less than or equal to the right")
            End If

            m_fLeft = fLeft
            m_fTop = fTop
            m_fRight = fRight
            m_fBottom = fBottom

        End Sub

        Public Sub Union(aRectangle As ExtentRectangle)

            Debug.WriteLine("Unioning rectangle")
            Debug.WriteLine(vbTab & "Before  : " & Rectangle)
            Debug.WriteLine(vbTab & "Argument: " & aRectangle.Rectangle)

            m_fLeft = Math.Min(m_fLeft, aRectangle.Left)
            m_fRight = Math.Max(m_fRight, aRectangle.Right)
            m_fBottom = Math.Min(m_fBottom, aRectangle.Bottom)
            m_fTop = Math.Max(m_fTop, aRectangle.Top)

            Debug.WriteLine(vbTab & "After   : " & Rectangle)
        End Sub

        Public Sub Buffer(fDistance As Double)

            If fDistance < 0 Then
                Throw New ArgumentOutOfRangeException("Distance", fDistance, "The buffer distance must be greater than distance")
            ElseIf fDistance > 0 Then

                Debug.WriteLine("Buffering rectangle")
                Debug.WriteLine(vbTab & "Before  : " & Rectangle)

                m_fLeft -= fDistance
                m_fRight += fDistance
                m_fBottom -= fDistance
                m_fTop += fDistance

                Debug.WriteLine(vbTab & "After   : " & Rectangle)
            Else
                Debug.Print("Warning: buffering rectangle by zero distance.")
            End If
        End Sub

        Public Shadows Function ToString()
            Return Rectangle
        End Function

        Public Function IsConcurrent(otherExtent As ExtentRectangle) As Boolean

            Dim bIsConcurrent As Boolean = False
            If Left = otherExtent.Left Then
                If Right = otherExtent.Right Then
                    If Top = otherExtent.Top Then
                        If Bottom = otherExtent.Bottom Then
                            bIsConcurrent = True
                        End If
                    End If
                End If
            End If
            Return bIsConcurrent
        End Function

        Public Sub MakeOrthogonal()
            m_fLeft = Math.Floor(m_fLeft)
            m_fRight = Math.Ceiling(m_fRight)
            m_fTop = Math.Ceiling(m_fTop)
            m_fBottom = Math.Floor(m_fBottom)
        End Sub

        ''' <summary>
        ''' Overloaded method to allow the input of cell size to determine an orthogonal extent from an extent object
        ''' </summary>
        ''' <param name="fCellSize"></param>
        ''' <remarks></remarks>
        Public Sub MakeOrthogonal(ByVal fCellSize As Double, Optional ByVal fPrecision As Double = 2.0, Optional ByVal fBuffer As Integer = 2)

            Dim fOrthogonalityFactor As Double
            If fPrecision < 0 Then
                fOrthogonalityFactor = fCellSize
            Else
                fOrthogonalityFactor = Math.Max(fCellSize, fPrecision)
            End If

            If m_fLeft <> 0 AndAlso fCellSize <> 0 Then
                m_fLeft = m_fLeft / fOrthogonalityFactor
                m_fLeft = Math.Floor(m_fLeft)
                m_fLeft = m_fLeft * fOrthogonalityFactor
            End If
            m_fLeft -= fBuffer

            If m_fRight <> 0 AndAlso fCellSize <> 0 Then
                m_fRight = m_fRight / fOrthogonalityFactor
                m_fRight = Math.Ceiling(m_fRight)
                m_fRight = m_fRight * fOrthogonalityFactor
            End If
            m_fRight += fBuffer

            If m_fTop <> 0 AndAlso fCellSize <> 0 Then
                m_fTop = m_fTop / fOrthogonalityFactor
                m_fTop = Math.Ceiling(m_fTop)
                m_fTop = m_fTop * fOrthogonalityFactor
            End If
            m_fTop += fBuffer

            If m_fBottom <> 0 AndAlso fCellSize <> 0 Then
                m_fBottom = m_fBottom / fOrthogonalityFactor
                m_fBottom = Math.Floor(m_fBottom)
                m_fBottom = m_fBottom * fOrthogonalityFactor
            End If
            m_fBottom -= fBuffer

        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="otherExtent"></param>
        ''' <returns>True if the two rectangle extents have any overlap</returns>
        ''' <remarks>Adapted from posts on 
        ''' http://stackoverflow.com/questions/306316/determine-if-two-rectangles-overlap-each-other</remarks>
        Public Function HasOverlap(ByVal otherExtent As ExtentRectangle) As Boolean

            Dim ulx As Double = Math.Max(Me.Left, otherExtent.Left)
            Dim uly As Double = Math.Max(Me.Bottom, otherExtent.Bottom)
            Dim lrx As Double = Math.Min(Me.Right, otherExtent.Right)
            Dim lry As Double = Math.Min(Me.Top, otherExtent.Top)

            Return ulx <= lrx AndAlso uly <= lry

        End Function

#End Region

    End Class

End Namespace