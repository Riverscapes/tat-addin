'
' PGB 1 Oct 2011
' New general purpose module for collecting together generic
' statistical functions
'
Namespace GISCode.Statistics

    Public Module Statistics

        ''' <summary>
        ''' Returns the mean for a list of values
        ''' </summary>
        ''' <param name="dValues">List of doudbles</param>
        ''' <param name="nCount">Return value specifying the number of elements that were used to generate the count</param>
        ''' <param name="bIgnoreNeg9999">True indicates that values in the dictionary of -9999 should be ignored</param>
        ''' <returns>Statistical mean, or zero if there are no values in the list</returns>
        ''' <remarks></remarks>
        Public Function Mean(dValues As Dictionary(Of Double, Double).ValueCollection, ByRef nCount As Integer, Optional bIgnoreNeg9999 As Boolean = True) As Double


            If TypeOf dValues Is Dictionary(Of Double, Double).ValueCollection Then
                If dValues.Count < 1 Then
                    Return 0
                End If
            Else
                Throw New Exception("Invalid list of values passed as argument.")
            End If

            nCount = 0
            Dim fMean As Double = 0
            For Each value As Double In dValues
                If Not bIgnoreNeg9999 OrElse (bIgnoreNeg9999 AndAlso value > -9998) Then
                    fMean += value
                    nCount += 1
                End If
            Next

            If fMean <> 0 AndAlso nCount <> 0 Then
                fMean = fMean / nCount
            Else
                fMean = 0
                nCount = 0
            End If

            Return fMean

        End Function

        Public Function FilteredMean(dValues As Dictionary(Of Double, Double).ValueCollection, fStdDevThreshold As Double, ByRef nCount As Integer, Optional bIgnoreNeg9999 As Boolean = True) As Double

            If TypeOf dValues Is Dictionary(Of Double, Double).ValueCollection Then
                If dValues.Count < 1 Then
                    Return 0
                End If
            Else
                Throw New Exception("Invalid list of values passed as argument.")
            End If

            If fStdDevThreshold <= 0 Then
                Throw New Exception("Invalid threshold provided.")
            End If
            '
            ' Get the mean and standard deviation of all the records in the dictionary
            '
            Dim fFullMean As Double = Mean(dValues, nCount)
            Dim fFullStdDev = StandardDeviation(dValues, fFullMean)
            nCount = 0
            Dim fFilteredMean As Double = 0
            Dim fDifferenceThreshold As Double = (fStdDevThreshold * fFullStdDev)

            For Each value As Double In dValues
                If Math.Abs(value - fFullMean) < fDifferenceThreshold Then
                    If Not bIgnoreNeg9999 OrElse (bIgnoreNeg9999 AndAlso value > -9998) Then
                        fFilteredMean += value
                        nCount += 1
                    End If
                End If
            Next

            If fFilteredMean <> 0 AndAlso nCount <> 0 Then
                fFilteredMean = fFilteredMean / nCount
            Else
                fFilteredMean = 0
                nCount = 0
            End If

            Return fFilteredMean

        End Function

        ''' <summary>
        ''' Calculate the standard deviation of the filtered set of values
        ''' </summary>
        ''' <param name="dValues">Full list of values</param>
        ''' <param name="fStdDevThreshold">The number of standard deviations beyond the full mean that will be excluded from the calculation</param>
        ''' <param name="fFilteredMean">The filtered mean value. i.e. the mean of the values that are within the threshold of the filtered mean.</param>
        ''' <returns>Filtered standard deviation</returns>
        ''' <remarks>This method first calculates the full mean and standard deviation. It then calculates the threshold to exclude values. This
        ''' is a certain number of standard deviations from the mean. The filtered standard deviation is then calculated for just those values that
        ''' are within the threshold of the full mean.</remarks>
        Public Function FilteredStandardDeviation(dValues As Dictionary(Of Double, Double).ValueCollection, fStdDevThreshold As Double, fFilteredMean As Double, Optional bIgnoreNeg9999 As Boolean = True) As Double

            If TypeOf dValues Is Dictionary(Of Double, Double).ValueCollection Then
                If dValues.Count < 1 Then
                    Return 0
                End If
            Else
                Throw New Exception("Invalid list of values passed as argument.")
            End If

            If fStdDevThreshold <= 0 Then
                Throw New Exception("Invalid threshold provided.")
            End If

            Dim nCount As Integer
            Dim fFullMean As Double = Mean(dValues, nCount)
            Dim fFullStdDev As Double = StandardDeviation(dValues, fFullMean)
            Dim fDifferenceThreshold As Double = (fStdDevThreshold * fFullStdDev)

            Dim fDifference As Double = 0
            nCount = 0
            For Each value As Double In dValues
                If Math.Abs(value - fFullMean) < fDifferenceThreshold Then
                    If Not bIgnoreNeg9999 OrElse (bIgnoreNeg9999 AndAlso value > -9998) Then
                        fDifference += Math.Pow(fFilteredMean - value, 2)
                        nCount += 1
                    End If
                End If
            Next

            If fDifference > 0 AndAlso nCount <> 0 Then
                fDifference = fDifference / nCount
                fDifference = Math.Sqrt(fDifference)
            Else
                fDifference = 0
            End If

            Return fDifference

        End Function

        ''' <summary>
        ''' Overloaded standard deviation calculation. See other method for comments.
        ''' </summary>
        ''' <param name="dValues"></param>
        ''' <param name="fStdDevThreshold"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FilteredStandardDeviation(dValues As Dictionary(Of Double, Double).ValueCollection, fStdDevThreshold As Double, ByRef nFilteredCount As Integer) As Double

            Dim nCount As Integer
            Dim fFilteredMean As Double = FilteredMean(dValues, fStdDevThreshold, nCount)
            Return FilteredStandardDeviation(dValues, fStdDevThreshold, fFilteredMean, nFilteredCount)

        End Function

        ''' <summary>
        ''' Statistical standard deviation
        ''' </summary>
        ''' <param name="dValues">List of double values</param>
        ''' <param name="fMean">The mean of the values</param>
        ''' <returns>The statistical standard deviation or zero if there are no values in the list</returns>
        ''' <remarks>http://en.wikipedia.org/wiki/Standard_deviation</remarks>
        Public Function StandardDeviation(dValues As Dictionary(Of Double, Double).ValueCollection, fMean As Double) As Double

            If TypeOf dValues Is Dictionary(Of Double, Double).ValueCollection Then
                If dValues.Count < 1 Then
                    Return 0
                End If
            Else
                Throw New Exception("Invalid list of values passed as argument.")
            End If

            Dim fDifference As Double
            Dim nCount As Integer

            For Each value As Double In dValues
                fDifference += Math.Pow(fMean - value, 2)
                nCount += 1
            Next

            If fDifference > 0 AndAlso nCount <> 0 Then
                fDifference = fDifference / nCount
                fDifference = Math.Sqrt(fDifference)
            Else
                fDifference = 0
            End If

            Return fDifference

        End Function

        ''' <summary>
        ''' Statistical standard deviation
        ''' </summary>
        ''' <param name="dValues">List of double values</param>
        ''' <returns>Statistical standard deviation or zero if there are no values in the list</returns>
        ''' <remarks>Calling this overloaded version will pre-calculate the mean for you.</remarks>
        Public Function StandardDeviation(dValues As Dictionary(Of Double, Double).ValueCollection) As Double

            Dim nCount As Integer
            Dim fMean As Double = Mean(dValues, nCount)
            Return StandardDeviation(dValues, fMean)

        End Function

        Public Function CoefficientOfVariation(dValues As Dictionary(Of Double, Double).ValueCollection) As Double

            Dim nCount As Integer
            Dim fMean As Double = Mean(dValues, nCount)
            Dim fStandardDeviation As Double = StandardDeviation(dValues, fMean)

            Dim fCV As Double = 0
            If fMean <> 0 AndAlso fStandardDeviation <> 0 Then
                fCV = (fStandardDeviation / fMean)
            End If

            Return fCV

        End Function

        Public Function CoefficientOfVariation(fStandardDeviation As Double, fMean As Double) As Double

            Dim fCV As Double = 0

            If fMean <> 0 AndAlso fStandardDeviation <> 0 Then
                fCV = (fStandardDeviation / fMean)
            End If

            Return fCV

        End Function
    End Module

End Namespace