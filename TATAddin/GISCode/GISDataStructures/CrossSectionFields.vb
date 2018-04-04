Imports ESRI.ArcGIS.Geodatabase

Namespace GISCode.GISDataStructures

    ''' <summary>
    ''' List of field names used in cross section shapefiles.
    ''' </summary>
    ''' <remarks>PGB 26 Jan 2012 - This list was originally stored as string resources in the RBT user interface
    ''' extension code. However, it's needed by both the front and back end code and so I have restructured it as 
    ''' a public class that both can use. Note: simply moving it to a resource file in the backend would have been
    ''' more complex for making it accessible to the front end.</remarks>
    Public Class CrossSectionFields

        Private m_Fields As Dictionary(Of String, Integer)

        Public Const CreateDate As String = "Date"
        Public Const WettedWidth As String = "WetWidth"
        ' Public Const Width As String = "Width"
        ' Public Const AverageDepth As String = "AvDepth"
        'Public Const WidthToDepth As String = "Wid2Depth"
        Public Const WidthToDepth As String = "W2MxDepth" 'renamed from "Wid2Depth" to be "W2MxDepth" (note 10 character field limit.) according to UF ticket # 36
        Public Const WidthToAvDepth As String = "W2AvDepth" 'added according to UF ticket # 36

        Public Const Gradient As String = "Gradient"
        Public Const Sinuosity As String = "Sinuosity"
        Public Const RKM As String = "RKM"

        Public Const Name As String = "Name"
        Public Const Length As String = "XSLength"
        Public Const CenterLine As String = "CLine"
        Public Const DEM As String = "DEM"
        Public Const Banks As String = "Banks"
        Public Const Extension As String = "Extension"
        Public Const StationSeparation As String = "StatSep"
        Public Const NumberOfStations As String = "NumStat"
        '
        ' PGB - 25 Aug 2008 - new to support XS measurements
        '
        Public Const BankfullElev As String = "BFElev"
        Public Const BankfullArea As String = "BFArea"
        Public Const BankfullWidth As String = "BFWidth"
        Public Const HydraulicRadius As String = "HRadius"
        Public Const MaxDepth As String = "MaxDepth"
        Public Const MeanDepth As String = "MeanDepth"
        Public Const WettedPerimeter As String = "WetPerim"
        '
        ' CHaMP
        '
        Public Const Elevation As String = "Elevation"
        'Public Const Depth As String = "Depth"
        Public Const Sequence As String = "Seq"
        Public Const Distance As String = "Distance"
        Public Const CHaMPGradient As String = "grad"

        Public Const DryWidth As String = "DryWidth"
        Public Const IsValid As String = "IsValid"
        Public Const Channel As String = "Channel"


        Public Sub New()

            m_Fields = New Dictionary(Of String, Integer)

        End Sub

        ''' <summary>
        ''' Adds the all the fields for cross section shapefile
        ''' </summary>
        ''' <param name="gCrossSections">Feature class to which the fields will be added</param>
        ''' <remarks>This is a combination of fields used for command line and user interface versions. Call
        ''' this method to set up the fields in the final 3D cross section file.</remarks>
        Public Sub ConfirmFields(ByVal gCrossSections As GISDataStructures.PolylineDataSource, ByVal nStringfieldLength As Integer)

            m_Fields.Add(Sequence, gCrossSections.AddField(Sequence, esriFieldType.esriFieldTypeInteger))
            m_Fields.Add(Distance, gCrossSections.AddField(Distance, esriFieldType.esriFieldTypeDouble))
            m_Fields.Add(CreateDate, gCrossSections.AddField(CreateDate, esriFieldType.esriFieldTypeDate))
            m_Fields.Add(Name, gCrossSections.AddField(Name, esriFieldType.esriFieldTypeString, nStringfieldLength))
            m_Fields.Add(Length, gCrossSections.AddField(Length, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(CenterLine, gCrossSections.AddField(CenterLine, esriFieldType.esriFieldTypeString, nStringfieldLength))
            m_Fields.Add(DEM, gCrossSections.AddField(DEM, esriFieldType.esriFieldTypeString, nStringfieldLength))
            m_Fields.Add(Banks, gCrossSections.AddField(Banks, esriFieldType.esriFieldTypeString, nStringfieldLength))
            m_Fields.Add(Extension, gCrossSections.AddField(Extension, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(StationSeparation, gCrossSections.AddField(StationSeparation, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(NumberOfStations, gCrossSections.AddField(NumberOfStations, esriFieldType.esriFieldTypeInteger))
            '
            ' PGB - 25 Aug 2008 - new fields to support XS measurements
            '
            m_Fields.Add(BankfullElev, gCrossSections.AddField(BankfullElev, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(BankfullArea, gCrossSections.AddField(BankfullArea, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(BankfullWidth, gCrossSections.AddField(BankfullWidth, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(HydraulicRadius, gCrossSections.AddField(HydraulicRadius, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(MaxDepth, gCrossSections.AddField(MaxDepth, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(MeanDepth, gCrossSections.AddField(MeanDepth, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(WettedPerimeter, gCrossSections.AddField(WettedPerimeter, esriFieldType.esriFieldTypeSingle))
            '
            ' KMB - 12 May 2009 - added width/depth field
            '
            m_Fields.Add(WidthToDepth, gCrossSections.AddField(WidthToDepth, esriFieldType.esriFieldTypeSingle))
            '
            ' PGB - 26 Jan 2012 - added CHaMP fields not already present
            '
            m_Fields.Add(WettedWidth, gCrossSections.AddField(WettedWidth, esriFieldType.esriFieldTypeSingle))
            m_Fields.Add(DryWidth, gCrossSections.AddField(DryWidth, esriFieldType.esriFieldTypeSingle))
            'm_Fields.Add(AverageDepth, ShapeFile.AddField(pFCClass, AverageDepth, esriFieldType.esriFieldTypeDouble))

            m_Fields.Add(IsValid, gCrossSections.AddField(IsValid, esriFieldType.esriFieldTypeSmallInteger))

            '
            ' FP - 22 July 2013 - added W2AvDepth field as described in UF Ticket # 36
            '
            m_Fields.Add(WidthToAvDepth, gCrossSections.AddField(WidthToAvDepth, esriFieldType.esriFieldTypeSingle))


        End Sub

        Public Function GetfieldIndex(ByVal sFieldName As String) As Integer

            If String.IsNullOrEmpty(sFieldName) Then
                Return -1
            Else
                If m_Fields.ContainsKey(sFieldName) Then
                    Return m_Fields(sFieldName)
                Else
                    Return -1
                End If
            End If

        End Function

    End Class

End Namespace