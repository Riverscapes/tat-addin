Imports ESRI.ArcGIS.Geometry

' Philip Bailey 
' 5 March 2013

Namespace GISCode

    Public Class NumberFormatting
        '
        ' Write these conversion values as multipliers from the first
        ' unit to the second unit.
        '
        ' Distance
        Public Const MetresToMilimetres As Double = 1000
        Public Const MetresToCentiMetres As Double = 100
        Public Const MetresToKilometres As Double = 0.001
        Public Const MetresToInches As Double = 39.3701
        Public Const MetrestoFeet As Double = 3.28084
        Public Const MetresToYards As Double = 1.09361
        Public Const MetrestoMiles As Double = 0.000621371


        ' Area
        Public Const SqMetreToSqMilimetre As Double = 1000000.0
        Public Const SqMetreToSqCentimetre As Double = 10000
        Public Const SqMetreToSqKilometre As Double = 10 ^ -6
        Public Const SqMetreToHectare As Double = 0.0001
        Public Const SqMetreToSqInch As Double = 1550
        Public Const SqMetreToSqFoot As Double = 10.7639
        Public Const SqMetreToSqYard As Double = 1.19599
        Public Const SqMetreToSqMile As Double = 0.0000003861
        Public Const SqMetreToAcre As Double = 0.000247105

        ' Volume
        Public Const CubicMetresInCubicMillimetres As Double = 1000000000
        Public Const CubicMetresInCubicCentimetres As Double = 1000000
        Public Const CubicMetresInCupsUS As Double = 4226.75
        Public Const CubicMetresInLitres As Double = 1000
        Public Const CubicMetresInCubicInch As Double = 61023.7
        Public Const CubicMetresInCubicFeet As Double = 35.3147
        Public Const CubicMetresInAcreFeet As Double = 0.000810713194
        Public Const CubicMetresInUSGallons As Double = 264.172
        Public Const CubicMetresInCubicYards As Double = 1.30795062
        Public Const CubicMetresInCubicKm As Double = 10 ^ -10
        Public Const CubicMetresInCubicMiles As Double = 2.39912759 * 10 ^ -10

        ''' <summary>
        ''' Measures of linear units, such as "metres"
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum LinearUnits
            mm
            cm
            m
            km
            inch
            ft
            yard
            mile
        End Enum

        ''' <summary>
        ''' Measures of areas such as "square metres"
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum AreaUnits
            sqmm
            sqcm
            sqm
            sqkm
            hectare
            sqin
            sqft
            sqyd
            sqmi
            acre
        End Enum

        ''' <summary>
        ''' Measures of volumetric units, such as metres cubed
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum VolumeUnits
            mm3
            cm3
            cupsUS
            litres
            m3
            inch3
            feet3
            gallons
            yard3
            acrefeet
            km3
            mi3
        End Enum

        ''' <summary>
        ''' Convert from one linear unit to another
        ''' </summary>
        ''' <param name="eFrom">The input linear units from which you want to convert</param>
        ''' <param name="eTo">The output linear units for the result</param>
        ''' <param name="fValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Convert(eFrom As LinearUnits, eTo As LinearUnits, fValue As Double) As Double

            Dim fResult As Double = 0

            Try
                Select Case eFrom

                    Case LinearUnits.mm
                        Select Case eTo
                            Case LinearUnits.mm : fResult = fValue
                            Case LinearUnits.cm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToCentiMetres
                            Case LinearUnits.m : fResult = fValue / MetresToMilimetres
                            Case LinearUnits.km : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToKilometres
                            Case LinearUnits.inch : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToInches
                            Case LinearUnits.ft : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoFeet
                            Case LinearUnits.yard : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToYards
                            Case LinearUnits.mile : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoMiles
                            Case Else : Throw New Exception
                        End Select

                    Case LinearUnits.cm
                        Select Case eTo
                            Case LinearUnits.mm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToMilimetres
                            Case LinearUnits.cm : fResult = fValue
                            Case LinearUnits.m : fResult = fValue / MetresToCentiMetres
                            Case LinearUnits.km : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToKilometres
                            Case LinearUnits.inch : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToInches
                            Case LinearUnits.ft : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoFeet
                            Case LinearUnits.yard : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToYards
                            Case LinearUnits.mile : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoMiles
                            Case Else : Throw New Exception
                        End Select

                    Case LinearUnits.m
                        Select Case eTo
                            Case LinearUnits.mm : fResult = fValue * NumberFormatting.MetresToMilimetres
                            Case LinearUnits.cm : fResult = fValue * MetresToCentiMetres
                            Case LinearUnits.m : fResult = fValue
                            Case LinearUnits.km : fResult = fValue * MetresToKilometres
                            Case LinearUnits.inch : fResult = fValue * MetresToInches
                            Case LinearUnits.ft : fResult = fValue * MetrestoFeet
                            Case LinearUnits.yard : fResult = fValue * MetresToYards
                            Case LinearUnits.mile : fResult = fValue * MetrestoMiles
                            Case Else : Throw New Exception
                        End Select


                    Case LinearUnits.km
                        Select Case eTo
                            Case LinearUnits.mm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToMilimetres
                            Case LinearUnits.cm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToCentiMetres
                            Case LinearUnits.m : fResult = fValue / MetresToKilometres
                            Case LinearUnits.km : fResult = fValue
                            Case LinearUnits.inch : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToInches
                            Case LinearUnits.ft : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoFeet
                            Case LinearUnits.yard : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToYards
                            Case LinearUnits.mile : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoMiles
                            Case Else : Throw New Exception
                        End Select

                    Case LinearUnits.inch
                        Select Case eTo
                            Case LinearUnits.mm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToMilimetres
                            Case LinearUnits.cm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToCentiMetres
                            Case LinearUnits.m : fResult = fValue / MetresToInches
                            Case LinearUnits.km : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToKilometres
                            Case LinearUnits.inch : fResult = fValue
                            Case LinearUnits.ft : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoFeet
                            Case LinearUnits.yard : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToYards
                            Case LinearUnits.mile : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoMiles
                            Case Else : Throw New Exception
                        End Select

                    Case LinearUnits.ft
                        Select Case eTo
                            Case LinearUnits.mm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToMilimetres
                            Case LinearUnits.cm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToCentiMetres
                            Case LinearUnits.m : fResult = fValue / MetrestoFeet
                            Case LinearUnits.km : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToKilometres
                            Case LinearUnits.inch : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToInches
                            Case LinearUnits.ft : fResult = fValue
                            Case LinearUnits.yard : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToYards
                            Case LinearUnits.mile : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoMiles
                            Case Else : Throw New Exception
                        End Select

                    Case LinearUnits.yard
                        Select Case eTo
                            Case LinearUnits.mm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToMilimetres
                            Case LinearUnits.cm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToCentiMetres
                            Case LinearUnits.m : fResult = fValue / MetresToYards
                            Case LinearUnits.km : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToKilometres
                            Case LinearUnits.inch : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToInches
                            Case LinearUnits.ft : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoFeet
                            Case LinearUnits.yard : fResult = fValue
                            Case LinearUnits.mile : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoMiles
                            Case Else : Throw New Exception
                        End Select

                    Case LinearUnits.mile
                        Select Case eTo
                            Case LinearUnits.mm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToMilimetres
                            Case LinearUnits.cm : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToCentiMetres
                            Case LinearUnits.m : fResult = fValue / MetrestoMiles
                            Case LinearUnits.km : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToKilometres
                            Case LinearUnits.inch : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToInches
                            Case LinearUnits.ft : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetrestoFeet
                            Case LinearUnits.yard : fResult = Convert(eFrom, LinearUnits.m, fValue) * MetresToYards
                            Case LinearUnits.mile : fResult = fValue
                            Case Else : Throw New Exception
                        End Select

                    Case Else : Throw New Exception

                End Select

            Catch ex As Exception
                Dim ex2 As New Exception("Unhandled volume unit conversation", ex)
                ex2.Data.Add("From units", eFrom)
                ex2.Data.Add("To units", eTo)
                Throw ex2
            End Try

            Return fResult

        End Function

        Public Shared Function Convert(eFrom As AreaUnits, eTo As AreaUnits, fValue As Double) As Double

            Dim fResult As Double = 0

            Try

                Select Case eFrom

                    Case AreaUnits.sqmm
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = fValue
                            Case AreaUnits.sqcm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqm : fResult = fValue / NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqkm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqin : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqft : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqyd : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqmi : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.acre : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToAcre
                            Case AreaUnits.hectare : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToHectare
                            Case Else : Throw New Exception
                        End Select

                    Case AreaUnits.sqcm
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqcm : fResult = fValue
                            Case AreaUnits.sqm : fResult = fValue / NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqkm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqin : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqft : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqyd : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqmi : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.acre : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToAcre
                            Case AreaUnits.hectare : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToHectare
                            Case Else : Throw New Exception
                        End Select

                    Case AreaUnits.sqm
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = fValue * NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqcm : fResult = fValue * NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqm : fResult = fValue
                            Case AreaUnits.sqkm : fResult = fValue * NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqin : fResult = fValue * NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqft : fResult = fValue * NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqyd : fResult = fValue * NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqmi : fResult = fValue * NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.acre : fResult = fValue * NumberFormatting.SqMetreToAcre
                            Case AreaUnits.hectare : fResult = fValue * NumberFormatting.SqMetreToHectare
                            Case Else : Throw New Exception
                        End Select

                    Case AreaUnits.sqkm
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqcm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqm : fResult = fValue / NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqkm : fResult = fValue
                            Case AreaUnits.sqin : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqft : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqyd : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqmi : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.acre : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToAcre
                            Case AreaUnits.hectare : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToHectare
                            Case Else : Throw New Exception
                        End Select

                    Case AreaUnits.sqin
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqcm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqm : fResult = fValue / NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqkm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqin : fResult = fValue
                            Case AreaUnits.sqft : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqyd : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqmi : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.acre : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToAcre
                            Case AreaUnits.hectare : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToHectare
                            Case Else : Throw New Exception
                        End Select

                    Case AreaUnits.sqft
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqcm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqm : fResult = fValue / NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqkm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqin : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqft : fResult = fValue
                            Case AreaUnits.sqyd : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqmi : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.acre : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToAcre
                            Case AreaUnits.hectare : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToHectare
                            Case Else : Throw New Exception
                        End Select

                    Case AreaUnits.sqyd
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqcm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqm : fResult = fValue / NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqkm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqin : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqft : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqyd : fResult = fValue
                            Case AreaUnits.sqmi : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.acre : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToAcre
                            Case AreaUnits.hectare : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToHectare
                            Case Else : Throw New Exception
                        End Select

                    Case AreaUnits.sqmi
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqcm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqm : fResult = fValue / NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.sqkm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqin : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqft : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqyd : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqmi : fResult = fValue
                            Case AreaUnits.acre : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToAcre
                            Case AreaUnits.hectare : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToHectare
                            Case Else : Throw New Exception
                        End Select

                    Case AreaUnits.acre
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqcm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqm : fResult = fValue / NumberFormatting.SqMetreToAcre
                            Case AreaUnits.sqkm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqin : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqft : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqyd : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqmi : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.acre : fResult = fValue
                            Case AreaUnits.hectare : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToHectare
                            Case Else : Throw New Exception
                        End Select

                    Case AreaUnits.hectare
                        Select Case eTo
                            Case AreaUnits.sqmm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMilimetre
                            Case AreaUnits.sqcm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqCentimetre
                            Case AreaUnits.sqm : fResult = fValue / NumberFormatting.SqMetreToHectare
                            Case AreaUnits.sqkm : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqKilometre
                            Case AreaUnits.sqin : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqInch
                            Case AreaUnits.sqft : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqFoot
                            Case AreaUnits.sqyd : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqYard
                            Case AreaUnits.sqmi : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToSqMile
                            Case AreaUnits.acre : fResult = Convert(eFrom, AreaUnits.sqm, fValue) * NumberFormatting.SqMetreToAcre
                            Case AreaUnits.hectare : fResult = fValue
                            Case Else : Throw New Exception
                        End Select

                    Case Else : Throw New Exception

                End Select


            Catch ex As Exception
                Dim ex2 As New Exception("Unhandled volume unit conversation", ex)
                ex2.Data.Add("From units", eFrom)
                ex2.Data.Add("To units", eTo)
                Throw ex2
            End Try

            Return fResult

        End Function

        Public Shared Function Convert(eFrom As VolumeUnits, eTo As VolumeUnits, fValue As Double) As Double

            Dim fResult As Double = 0
            Try
                Select Case eFrom

                    Case VolumeUnits.mm3
                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = fValue
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception
                        End Select

                    Case VolumeUnits.cm3
                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = fValue
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception
                        End Select

                    Case VolumeUnits.cupsUS
                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = fValue
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception
                        End Select

                    Case VolumeUnits.litres
                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = fValue
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception
                        End Select

                    Case VolumeUnits.m3
                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = fValue * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = fValue * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = fValue * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = fValue * CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue
                            Case VolumeUnits.acrefeet : fResult = fValue * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = fValue * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = fValue * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = fValue * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = fValue * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception
                        End Select

                    Case VolumeUnits.km3
                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInCubicKm
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = fValue
                        End Select

                    Case VolumeUnits.acrefeet

                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.acrefeet : fResult = fValue
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception
                        End Select

                    Case VolumeUnits.inch3

                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = fValue
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception
                        End Select

                    Case VolumeUnits.feet3

                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = fValue
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception
                        End Select

                    Case VolumeUnits.gallons

                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = fValue
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception

                        End Select

                    Case VolumeUnits.yard3

                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = fValue
                            Case VolumeUnits.mi3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                            Case Else : Throw New Exception
                        End Select

                    Case VolumeUnits.mi3
                        Select Case eTo
                            Case VolumeUnits.mm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicMillimetres
                            Case VolumeUnits.cm3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicCentimetres
                            Case VolumeUnits.cupsUS : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCupsUS
                            Case VolumeUnits.litres : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInLitres
                            Case VolumeUnits.m3 : fResult = fValue / NumberFormatting.CubicMetresInCubicMiles
                            Case VolumeUnits.acrefeet : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInAcreFeet
                            Case VolumeUnits.inch3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicInch
                            Case VolumeUnits.feet3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicFeet
                            Case VolumeUnits.gallons : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInUSGallons
                            Case VolumeUnits.yard3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicYards
                            Case VolumeUnits.mi3 : fResult = fValue
                            Case VolumeUnits.km3 : fResult = Convert(eFrom, VolumeUnits.m3, fValue) * NumberFormatting.CubicMetresInCubicKm
                        End Select

                    Case Else : Throw New Exception

                End Select

            Catch ex As Exception
                Dim ex2 As New Exception("Unhandled volume unit conversation", ex)
                ex2.Data("From units") = eFrom
                ex2.Data("To units") = eTo
                Throw ex2
            End Try

            Return fResult

        End Function

        Public Shared Function GetUnitsAsString(ByVal eUnits As LinearUnits, Optional ByVal bParentheses As Boolean = False, Optional ByVal nPower As Integer = 1) As String

            Dim sResult As String = String.Empty
            Select Case eUnits
                Case LinearUnits.mm : sResult = "mm"
                Case LinearUnits.cm : sResult = "cm"
                Case LinearUnits.m : sResult = "m"
                Case LinearUnits.km : sResult = "km"
                Case LinearUnits.inch : sResult = "in"
                Case LinearUnits.ft : sResult = "ft"
                Case LinearUnits.yard : sResult = "yd"
                Case LinearUnits.mile : sResult = "mile"
                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eUnits.ToString
                    Throw ex
            End Select

            Dim unitLabel As String = Nothing
            Select Case nPower

                Case 2
                    unitLabel = "²"
                Case 3
                    unitLabel = "³"

            End Select

            If bParentheses Then
                sResult = " (" & sResult & unitLabel & ")"
            End If

            Return sResult

        End Function

        Public Shared Function GetUnitsAsString(eUnits As AreaUnits, Optional bParentheses As Boolean = False) As String

            Dim sResult As String = String.Empty
            Select Case eUnits
                Case AreaUnits.sqmm : sResult = "mm²"
                Case AreaUnits.sqcm : sResult = "cm²"
                Case AreaUnits.sqm : sResult = "m²"
                Case AreaUnits.sqkm : sResult = "km²"
                Case AreaUnits.sqin : sResult = "in²"
                Case AreaUnits.sqft : sResult = "ft²"
                Case AreaUnits.sqyd : sResult = "yd²"
                Case AreaUnits.sqmi : sResult = "mi²"
                Case AreaUnits.acre : sResult = "acres"
                Case AreaUnits.hectare : sResult = "hectares"
                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eUnits.ToString
                    Throw ex
            End Select

            If bParentheses Then
                sResult = " (" & sResult & ")"
            End If
            Return sResult

        End Function

        Public Shared Function GetUnitsAsString(eUnits As VolumeUnits, Optional bParentheses As Boolean = False) As String

            Dim sResult As String = String.Empty
            Select Case eUnits
                Case VolumeUnits.mm3 : sResult = "mm³"
                Case VolumeUnits.cm3 : sResult = "cm³"
                Case VolumeUnits.cupsUS : sResult = "US cups"
                Case VolumeUnits.litres : sResult = "l"
                Case VolumeUnits.m3 : sResult = "m³"
                Case VolumeUnits.inch3 : sResult = "in³"
                Case VolumeUnits.feet3 : sResult = "ft³"
                Case VolumeUnits.acrefeet : sResult = "acre ft"
                Case VolumeUnits.yard3 : sResult = "yd³"
                Case VolumeUnits.gallons : sResult = "gallons"
                Case VolumeUnits.mi3 : sResult = "mi³"
                Case (VolumeUnits.km3) : sResult = "km³"
                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eUnits.ToString
                    Throw ex
            End Select

            If bParentheses Then
                sResult = " (" & sResult & ")"
            End If
            Return sResult

        End Function

        Public Shared Function GetLinearUnits(eLinearUnits As ILinearUnit) As LinearUnits

            Select Case eLinearUnits.Name.ToLower
                Case "meter", "metre"
                    Return LinearUnits.m

                Case "kilometer", "kilometre"
                    Return LinearUnits.km

                Case "feet", "us_feet", "ft"
                    Return LinearUnits.ft

                Case Else
                    Dim ex As New Exception("Unhandled linear units")
                    ex.Data("Linear units") = eLinearUnits.Name
                    Throw ex
            End Select

        End Function

        Public Shared Function GetAreaUnitsRaw(eLinearUnit As LinearUnits) As AreaUnits

            Select Case eLinearUnit

                Case LinearUnits.mm
                    Return AreaUnits.sqmm

                Case LinearUnits.cm
                    Return AreaUnits.sqcm

                Case LinearUnits.m
                    Return AreaUnits.sqm

                Case LinearUnits.km
                    Return AreaUnits.sqkm

                Case LinearUnits.inch
                    Return AreaUnits.sqin

                Case LinearUnits.ft
                    Return AreaUnits.sqft

                Case LinearUnits.yard
                    Return AreaUnits.sqyd

                Case LinearUnits.mile
                    Return AreaUnits.sqmi

                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eLinearUnit.ToString
                    Throw ex
            End Select

        End Function

        Public Shared Function GetAreaUnitsScaled(eLinearUnit As LinearUnits) As AreaUnits

            Select Case eLinearUnit
                Case LinearUnits.mm, LinearUnits.cm, LinearUnits.m, LinearUnits.km
                    Return AreaUnits.hectare

                Case LinearUnits.inch, LinearUnits.ft, LinearUnits.yard, LinearUnits.mile
                    Return AreaUnits.acre
                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eLinearUnit.ToString
                    Throw ex
            End Select

        End Function

        Public Shared Function GetVolumeUnitsRaw(eLinearUnit As LinearUnits) As VolumeUnits

            Select Case eLinearUnit
                Case LinearUnits.mm
                    Return VolumeUnits.mm3

                Case LinearUnits.cm
                    Return VolumeUnits.cm3

                Case LinearUnits.m
                    Return VolumeUnits.m3

                Case LinearUnits.km
                    Return VolumeUnits.km3

                Case LinearUnits.inch
                    Return VolumeUnits.inch3

                Case LinearUnits.ft
                    Return VolumeUnits.feet3

                Case LinearUnits.yard
                    Return VolumeUnits.yard3

                Case LinearUnits.mile
                    Return VolumeUnits.mi3

                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eLinearUnit.ToString
                    Throw ex
            End Select

        End Function

        Public Shared Function GetVolumeUnitsScaled(eLinearUnit As LinearUnits) As VolumeUnits

            Select Case eLinearUnit
                Case LinearUnits.mm, LinearUnits.cm, LinearUnits.m, LinearUnits.km
                    Return VolumeUnits.m3

                Case LinearUnits.inch, LinearUnits.ft, LinearUnits.yard, LinearUnits.mile
                    Return VolumeUnits.acrefeet
                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eLinearUnit.ToString
                    Throw ex
            End Select

        End Function

        Public Shared Function IsMetric(eLinearUnit As LinearUnits) As Boolean
            Select Case eLinearUnit
                Case LinearUnits.mm, LinearUnits.cm, LinearUnits.m, LinearUnits.km
                    Return True
                Case LinearUnits.inch, LinearUnits.ft, LinearUnits.yard, LinearUnits.mile
                    Return False
                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eLinearUnit.ToString
                    Throw ex

            End Select
        End Function

        Public Shared Function IsMetric(eLinearUnit As AreaUnits) As Boolean
            Select Case eLinearUnit
                Case AreaUnits.sqm, AreaUnits.sqkm, AreaUnits.hectare
                    Return True
                Case AreaUnits.sqft, AreaUnits.acre
                    Return False
                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eLinearUnit.ToString
                    Throw ex

            End Select
        End Function

        Public Shared Function IsMetric(eLinearUnit As VolumeUnits) As Boolean
            Select Case eLinearUnit
                Case VolumeUnits.m3, VolumeUnits.mm3, VolumeUnits.cm3, VolumeUnits.litres
                    Return True
                Case VolumeUnits.feet3, VolumeUnits.acrefeet
                    Return False
                Case Else
                    Dim ex As New Exception("Unhandled units")
                    ex.Data("Units") = eLinearUnit.ToString
                    Throw ex

            End Select
        End Function

        Public Shared Function GetLinearUnitsFromESRI(eESRILinearUnits As ESRI.ArcGIS.Geometry.ILinearUnit) As LinearUnits

            Dim sUnits As String = GISDataStructures.GetLinearUnitsAsString(eESRILinearUnits)
            Return GetLinearUnitsFromString(sUnits)

        End Function

        Public Shared Function GetLinearUnitsFromString(sLinearUnits As String) As LinearUnits

            Dim eResult As NumberFormatting.LinearUnits = LinearUnits.m
            Select Case sLinearUnits.ToLower
                Case "mm" : eResult = LinearUnits.mm
                Case "cm" : eResult = LinearUnits.cm
                Case "m" : eResult = LinearUnits.m
                Case "km" : eResult = LinearUnits.km
                Case "in" : eResult = LinearUnits.inch
                Case "ft" : eResult = LinearUnits.ft
                Case "yd" : eResult = LinearUnits.yard
                Case "mi" : eResult = LinearUnits.mile
                Case Else
                    Dim ex As New Exception("Unhandled linear unit type.")
                    ex.Data("Unit") = sLinearUnits
                    Throw ex
            End Select
            Return eResult
        End Function

        Public Shared Sub TestBench()

            Debug.Print("Converting 34.456 metres to kilometres: " & Convert(LinearUnits.m, LinearUnits.km, 34.456) & ", expecting 0.034456", {34.456})
            Debug.Print("Converting 10000423 cubic feet to acre feet: " & Convert(VolumeUnits.feet3, VolumeUnits.acrefeet, 10000423) & ", expecting 229.58", {10000423})

            Dim dblArray As Double() = New Double() {11.3, 0.005, 10000, 17.5, 4.64}

            Dim areaUnitsSet As Array
            areaUnitsSet = System.Enum.GetValues(GetType(NumberFormatting.AreaUnits))
            Dim eLinear As NumberFormatting.AreaUnits

            Debug.Print("AREA CONVERSION TESTS {0}{0}", vbCrLf)
            Dim dblResult As Double
            For Each eLinear In areaUnitsSet
                For i As Integer = 0 To dblArray.Length - 1
                    dblResult = Convert(eLinear, AreaUnits.acre, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.acre, dblResult.ToString()))

                    dblResult = Convert(eLinear, AreaUnits.hectare, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to  {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.hectare, dblResult.ToString()))

                    dblResult = Convert(eLinear, AreaUnits.sqcm, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.sqcm, dblResult.ToString()))

                    dblResult = Convert(eLinear, AreaUnits.sqft, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.sqft, dblResult.ToString()))

                    dblResult = Convert(eLinear, AreaUnits.sqin, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.sqin, dblResult.ToString()))

                    dblResult = Convert(eLinear, AreaUnits.sqkm, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to  {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.sqkm, dblResult.ToString()))

                    dblResult = Convert(eLinear, AreaUnits.sqm, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.sqm, dblResult.ToString()))

                    dblResult = Convert(eLinear, AreaUnits.sqmi, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.sqmi, dblResult.ToString()))

                    dblResult = Convert(eLinear, AreaUnits.sqmm, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.sqmm, dblResult.ToString()))

                    dblResult = Convert(eLinear, AreaUnits.sqyd, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to {2} equals {3}", dblArray(i).ToString(), eLinear.ToString, AreaUnits.sqyd, dblResult.ToString()))
                Next

            Next

            Dim volumeUnitsSet As Array
            volumeUnitsSet = System.Enum.GetValues(GetType(NumberFormatting.VolumeUnits))
            Dim eVolume As NumberFormatting.VolumeUnits

            Debug.Print("{0}{0}VOLUME CONVERSION TESTS {0}{0}", vbCrLf)

            For Each eVolume In volumeUnitsSet
                For i As Integer = 0 To dblArray.Length - 1
                    dblResult = Convert(eVolume, VolumeUnits.acrefeet, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.acrefeet, dblResult.ToString()))

                    dblResult = Convert(eVolume, VolumeUnits.cm3, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to  {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.cm3, dblResult.ToString()))

                    dblResult = Convert(eVolume, VolumeUnits.cupsUS, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.cupsUS, dblResult.ToString()))

                    dblResult = Convert(eVolume, VolumeUnits.feet3, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.feet3, dblResult.ToString()))

                    dblResult = Convert(eVolume, VolumeUnits.gallons, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.gallons, dblResult.ToString()))

                    dblResult = Convert(eVolume, VolumeUnits.inch3, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to  {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.inch3, dblResult.ToString()))

                    dblResult = Convert(eVolume, VolumeUnits.litres, dblArray(i))
                    Debug.Print(String.Format("{0} in  {1} to  {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.litres, dblResult.ToString()))

                    dblResult = Convert(eVolume, VolumeUnits.m3, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.m3, dblResult.ToString()))

                    dblResult = Convert(eVolume, VolumeUnits.mm3, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.mm3, dblResult.ToString()))

                    dblResult = Convert(eVolume, VolumeUnits.yard3, dblArray(i))
                    Debug.Print(String.Format("{0} in {1} to {2} equals {3}", dblArray(i).ToString(), eVolume.ToString, VolumeUnits.yard3, dblResult.ToString()))
                Next

            Next


                'Debug.Assert(Math.Round(Convert(AreaUnits.sqm, AreaUnits.sqft, 11.3), 3) = 121.632)
                'Debug.Assert(Math.Round(Convert(AreaUnits.acre, AreaUnits.sqft, 11.3), 0) = 492228)
                'Debug.Assert(Math.Round(Convert(AreaUnits.sqin, AreaUnits.sqmm, 11.3), 3) = 7290.308)



        End Sub

    End Class

    ''' <summary>
    ''' Use this class to display the linear units in a dropdownlist
    ''' </summary>
    ''' <remarks></remarks>
    Public Class LinearUnitClass
        Private m_eLinearUnit As NumberFormatting.LinearUnits
        Private m_sDisplayName As String

        Public Sub New(sDisplayName As String, eLinearUnit As NumberFormatting.LinearUnits)
            m_sDisplayName = sDisplayName
            m_eLinearUnit = eLinearUnit
        End Sub

        Public Sub New(eLinearUnit As NumberFormatting.LinearUnits)
            m_sDisplayName = NumberFormatting.GetUnitsAsString(eLinearUnit)
            m_eLinearUnit = eLinearUnit
        End Sub

        Public ReadOnly Property LinearUnit As NumberFormatting.LinearUnits
            Get
                Return m_eLinearUnit
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return m_sDisplayName
        End Function

        Public Function GetUnitsAsString(Optional ByVal bParentheses As Boolean = False)
            Return GISCode.NumberFormatting.GetUnitsAsString(m_eLinearUnit, bParentheses)
        End Function

    End Class

End Namespace
