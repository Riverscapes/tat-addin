'
' Philip Bailey
' 27 Sep 2011
' Class to represent the ancsilliary data for channel units. These are the polygons that survey crews 
' record in the field to represent CHaMP Tier 1 or Tier 2 habitat units; pools & riffles etc.
'

#Region "Imports"

Imports System.Xml
Imports ESRI.ArcGIS.Geodatabase
Imports System.Text
Imports System.Collections

#End Region

Namespace GISCode.CHaMP

    Public Enum SingleFeatureForGroupEnum
        Maximum
        Minimum
    End Enum

    Public Class ChannelUnit

        Private Const m_fDepthTolerance As Double = 0.01

#Region "Members"

        Private m_nNumber As UInteger
        Private m_sTier1 As String
        Private m_sTier2 As String
        Private m_nSegmentNumber As UInteger


        Private m_fVolume As Double
        Private m_fArea As Double
        '
        ' PGB - Jan 2014. New area field for the area of the actual
        ' channel unit polygon. In the RBT Console the original Area
        ' stores the area of the crew WSDEM - DEM inside the channel
        ' unit polygon. This can actually be quite a bit smaller than 
        ' the polygon area. Need both for comparison.
        Private m_fPolygonArea As Double

        '
        ' PGB 17 April 2013 - New values for pool depth and tail depth
        '
        Private m_fMaxDepth As Double = Double.NaN
        Private m_fDepthAtThalwegExit As Double = Double.NaN

        Private m_dGrainSizeSample As Dictionary(Of GISCode.GCD.GrainSizeDistributionCalculator.GrainSizeClasses, UInteger)
        Private m_dCumulativeSum As Dictionary(Of GISCode.GCD.GrainSizeDistributionCalculator.GrainSizeClasses, UInteger)

#End Region

#Region "Properties"

        Public Property Area As Double
            Get
                Return m_fArea
            End Get
            Set(value As Double)
                If value >= 0 Then
                    m_fArea = value
                Else
                    m_fArea = 0
                End If
            End Set
        End Property

        ''' <summary>
        ''' See notes above
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PolygonArea As Double
            Get
                Return m_fPolygonArea
            End Get
            Set(value As Double)
                If value >= 0 Then
                    m_fPolygonArea = value
                Else
                    m_fPolygonArea = 0
                End If
            End Set
        End Property

        Public Property Volume As Double
            Get
                Return m_fVolume
            End Get
            Set(value As Double)
                If value >= 0 Then
                    m_fVolume = value
                Else
                    m_fVolume = 0
                End If
            End Set
        End Property

        Public Property MaxDepth As Double
            Get
                Return m_fMaxDepth
            End Get
            Set(value As Double)
                If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                    Throw New ArgumentOutOfRangeException("Value", value, "Invalid max depth")
                Else
                    If value >= 0 Then
                        m_fMaxDepth = value
                    Else
                        Throw New ArgumentOutOfRangeException("Value", value, "Invalid max depth")
                    End If
                End If
            End Set
        End Property

        Public Property DepthAtThalwegExit As Double
            Get
                Return m_fDepthAtThalwegExit
            End Get
            Set(value As Double)
                If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                    Throw New ArgumentOutOfRangeException("Value", value, "Invalid max depth at thalweg exit")
                Else
                    If value >= 0 Then
                        m_fDepthAtThalwegExit = value
                    Else
                        Throw New ArgumentOutOfRangeException("Value", value, "Invalid max depth at Thalweg exit")
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property ResidualDepth As Double
            Get
                If Double.IsInfinity(m_fMaxDepth) OrElse Double.IsNaN(m_fMaxDepth) OrElse Double.IsInfinity(m_fDepthAtThalwegExit) OrElse Double.IsNaN(m_fDepthAtThalwegExit) Then
                    Return Double.NaN
                Else
                    Dim fResidualDepth = MaxDepth - DepthAtThalwegExit
                    If Math.Abs(fResidualDepth) < m_fDepthTolerance Then
                        fResidualDepth = 0
                    Else
                        fResidualDepth = Math.Round(fResidualDepth, 2)
                    End If

                    Return fResidualDepth
                End If
            End Get
        End Property

        Public ReadOnly Property Number As Integer
            Get
                Return m_nNumber
            End Get
        End Property

        Public ReadOnly Property Tier1 As String
            Get
                Return m_sTier1
            End Get
        End Property

        Public ReadOnly Property Tier2 As String
            Get
                Return m_sTier2
            End Get
        End Property

        Public ReadOnly Property SegmentNumber As String
            Get
                Return m_nSegmentNumber
            End Get
        End Property

        Public ReadOnly Property GrainSampleDictionary As Dictionary(Of GISCode.GCD.GrainSizeDistributionCalculator.GrainSizeClasses, UInteger)
            Get
                Return m_dGrainSizeSample
            End Get
        End Property

        Public Property CumulativeSum As Dictionary(Of GISCode.GCD.GrainSizeDistributionCalculator.GrainSizeClasses, UInteger)
            Get
                Return m_dCumulativeSum
            End Get
            Set(value As Dictionary(Of GISCode.GCD.GrainSizeDistributionCalculator.GrainSizeClasses, UInteger))
                m_dCumulativeSum = value
            End Set
        End Property

        ''' <summary>
        ''' Returns true if at least one grain size contains a valid measurement
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>PGB: This is needed because some CHaMP data do not contain grain size observations. In these
        ''' cases the class will just contain zeroes for all the grain size values.</remarks>
        Public ReadOnly Property HasGrainMeasurements As Boolean
            Get
                Dim bResult As Boolean = False
                For Each aSize As Integer In m_dGrainSizeSample.Values
                    If aSize > 0 Then
                        bResult = True
                        Exit For
                    End If
                Next
                Return bResult
            End Get
        End Property

#End Region

        Public Sub New(ByVal nUnitNumber As Integer, ByVal sTier1 As String, ByVal sTier2 As String, ByVal nSegmentNumber As Integer, ByVal iFines As UInteger, _
                       ByVal iSand As UInteger, _
                       ByVal iFineGravel As UInteger, _
                       ByVal iCoarseGravel As UInteger, _
                       ByVal iCobble As UInteger, _
                       ByVal iBoulder As UInteger, _
                       ByVal iBedrock As UInteger)

            If nUnitNumber < 1 Then
                Throw New ArgumentOutOfRangeException("Unit Number", nUnitNumber, "The channel unit number must be greater than zero")
            End If

            If nSegmentNumber < 1 Then
                Throw New ArgumentOutOfRangeException("Segment Number", nSegmentNumber, "The segment number must be greater than zero.")
            End If

            If String.IsNullOrEmpty(sTier1) Then
                Throw New ArgumentNullException("Tier1", "The tier 1 name cannot be null or empty")
            End If

            If String.IsNullOrEmpty(sTier2) Then
                Throw New ArgumentNullException("Tier2", "The tier 2 name cannot be null or empty")
            End If

            m_nNumber = Convert.ToUInt32(nUnitNumber)
            m_nSegmentNumber = Convert.ToUInt32(nSegmentNumber)
            m_sTier1 = sTier1
            m_sTier2 = sTier2
            m_fArea = 0
            m_fPolygonArea = 0
            m_fVolume = 0

            m_dGrainSizeSample = New Dictionary(Of GISCode.GCD.GrainSizeDistributionCalculator.GrainSizeClasses, UInteger)
            m_dGrainSizeSample.Add(GCD.GrainSizeDistributionCalculator.GrainSizeClasses.Fines, iFines)
            m_dGrainSizeSample.Add(GCD.GrainSizeDistributionCalculator.GrainSizeClasses.Sand, iSand)
            m_dGrainSizeSample.Add(GCD.GrainSizeDistributionCalculator.GrainSizeClasses.FineGravel, iFineGravel)
            m_dGrainSizeSample.Add(GCD.GrainSizeDistributionCalculator.GrainSizeClasses.CoarseGravel, iCoarseGravel)
            m_dGrainSizeSample.Add(GCD.GrainSizeDistributionCalculator.GrainSizeClasses.Cobbles, iCobble)
            m_dGrainSizeSample.Add(GCD.GrainSizeDistributionCalculator.GrainSizeClasses.Boulders, iBoulder)
            m_dGrainSizeSample.Add(GCD.GrainSizeDistributionCalculator.GrainSizeClasses.Bedrock, iBedrock)

        End Sub

        Public Sub New(ByVal nUnitNumber As Integer,
             ByVal sTier1 As String,
             ByVal sTier2 As String,
             ByVal nSegmentNumber As Integer,
             ByVal dGrainSizeSample As Dictionary(Of GCD.GrainSizeDistributionCalculator.GrainSizeClasses, UInteger))

            m_dGrainSizeSample = dGrainSizeSample

        End Sub

#Region "Methods"

        Public Shared Sub Load(ByVal aSurvey As Xml.XmlNode, ByRef dict As Dictionary(Of Integer, ChannelUnit), ByVal nSegmentNumber As Integer)

            If dict Is Nothing Then
                dict = New Dictionary(Of Integer, ChannelUnit)
            End If

            Dim nUnitNumber As Integer
            Dim subNode As XmlNode
            Dim sTier1 As String
            Dim sTier2 As String

            For Each aNode As XmlNode In aSurvey.SelectNodes("channel_units/unit")

                subNode = aNode.SelectSingleNode("unit_number")
                If TypeOf subNode Is XmlNode Then
                    If Not Integer.TryParse(subNode.InnerText, nUnitNumber) Then
                        Dim e As New Exception("Invalid channel unit number")
                        e.Data("UnitNumber") = nUnitNumber
                        Throw e
                    End If
                Else
                    Throw New Exception("Missing channel unit number")
                End If

                subNode = aNode.SelectSingleNode("tier1")
                If TypeOf subNode Is XmlNode Then
                    sTier1 = subNode.InnerText
                    If String.IsNullOrEmpty(sTier1) Then
                        Dim e As New Exception("Null or empty channel unit tier 1 name")
                        e.Data("UnitNumber") = nUnitNumber
                        Throw e
                    End If
                Else
                    Throw New Exception("Missing channel unit tier 1")
                End If

                subNode = aNode.SelectSingleNode("tier2")
                If TypeOf subNode Is XmlNode Then
                    sTier2 = subNode.InnerText
                    If String.IsNullOrEmpty(sTier2) Then
                        Dim e As New Exception("Null or empty channel unit tier 2 name")
                        e.Data("UnitNumber") = nUnitNumber
                        Throw e
                    End If
                Else
                    Throw New Exception("Missing channel unit tier 2")
                End If

                Dim nBedrock As UInteger = GetSafeValue(aNode, "bedrock")
                Dim nBouldersgt256 As UInteger = GetSafeValue(aNode, "bouldersgt256")
                Dim nCobbles65255 As UInteger = GetSafeValue(aNode, "cobbles65255")
                Dim nCoarsegravel1764 As UInteger = GetSafeValue(aNode, "coarsegravel1764")
                Dim nFinegravel316 As UInteger = GetSafeValue(aNode, "finegravel316")
                Dim nSand0062 As UInteger = GetSafeValue(aNode, "sand0062")
                Dim nFineslt006 As UInteger = GetSafeValue(aNode, "fineslt006")
                Dim nSumsubstratecolver As UInteger = GetSafeValue(aNode, "sumsubstratecolver")


                dict.Add(nUnitNumber, New ChannelUnit(nUnitNumber, sTier1, sTier2, nSegmentNumber, nFineslt006, nSand0062, nFinegravel316, nCoarsegravel1764, nCobbles65255, nBouldersgt256, nBedrock))
            Next

        End Sub

        Private Shared Function GetSafeValue(theParent As Xml.XmlNode, sNodeName As String) As UInteger

            Dim nResult As Integer = 0
            If TypeOf theParent Is Xml.XmlNode Then
                Dim nodChild As Xml.XmlNode = theParent.SelectSingleNode(sNodeName)
                If TypeOf nodChild Is Xml.XmlNode Then
                    If Not String.IsNullOrEmpty(nodChild.InnerText) Then
                        UInteger.TryParse(nodChild.InnerText, nResult)
                    End If
                End If
            End If
            Return nResult
        End Function



        ' ''' PGB 25 Jul 2014. This is now done more simply in ChaMPMetrics.GetChannelUnitsClipped
        ' ''' <summary>
        ' ''' Obtain the max depth property values from a depth raster
        ' ''' </summary>
        ' ''' <param name="gDepthRaster">Full path to a depth raster</param>
        ' ''' <param name="gChannelUnits">Full path to channel unit polygons</param>
        ' ''' <param name="dict">Dictionary of channel units loaded from XML</param>
        ' ''' <remarks>The channel unit polyons should have a string field called "Unit_Number" that has unique integer IDs (1, 2, 3 etc).
        ' ''' The argument dictionary should be pre-loaded with ChannelUnit objects keyed with the same, matching IDs.
        ' ''' Use Zonal stats on the channel unit polygons to get the Max() value in the depth raster, using the channel unit numbers as the
        ' ''' zones. Then loop through the zonal stats table and populate the dictionary with the Max value</remarks>
        'Public Shared Sub GetMaxDepths(ByVal gDepthRaster As GISDataStructures.Raster, ByVal gChannelUnits As GISDataStructures.ChannelUnits, ByRef dict As Dictionary(Of Integer, ChannelSegment), ByVal nUnitNumberFieldIndex As Integer)

        '    'Check that dict is initialized
        '    If dict Is Nothing Then
        '        Throw New ArgumentException("dict", "The input dictionary dict does is not initialized")
        '    End If

        '    'check that field Unit_Number exists in shapefile
        '    'IMPORTANT: If we are using a shapefile as input, the field name is too long (11 characters)
        '    'Dim gChannelUnits As New GISCode.GISDataStructures.ChannelUnits(sChannelUnits)
        '    'pFCChannelUnits = GISCode.FeatureClass.GetFeatureClass(sChannelUnits)

        '    'check if channel units shapefile contains any features
        '    If gChannelUnits.FeatureCount < 1 Then
        '        Throw New ArgumentException("sChannelUnits", "The shapefile '" & gChannelUnits.FullPath & "' must contains at least one feature.")
        '    End If

        '    ' Check unit_number field exists
        '    Dim iUnitNumberFieldIndex As Integer = gChannelUnits.FindField(GISDataStructures.ChannelUnits.ChannelUnitNumberField)
        '    If iUnitNumberFieldIndex >= 0 Then
        '        'check unit_number field is integer
        '        Dim pUnitNumberField As IField = gChannelUnits.FeatureClass.Fields.Field(iUnitNumberFieldIndex)
        '        If Not pUnitNumberField.Type = esriFieldType.esriFieldTypeInteger AndAlso Not pUnitNumberField.Type = esriFieldType.esriFieldTypeSmallInteger Then
        '            Throw New ArgumentException("sChannelUnits", "Field '" & GISDataStructures.ChannelUnits.ChannelUnitNumberField & "' is not an integer field")
        '        End If
        '    Else
        '        Throw New ArgumentException("sChannelUnits", "The shapefile '" & gChannelUnits.FullPath & "' must contain a field named '" & GISDataStructures.ChannelUnits.ChannelUnitNumberField & "'")
        '    End If

        '    Try
        '        'Get zonal statistics
        '        Dim sTableName As String = IO.Path.Combine(WorkspaceManager.WorkspacePath, "zonalstats")
        '        Dim pWS As IWorkspace = GISDataStructures.GetWorkspace(sTableName, GISDataStructures.GISDataStorageTypes.ShapeFile)
        '        sTableName = GISCode.Table.GetSafeName(pWS, IO.Path.GetFileNameWithoutExtension(sTableName))
        '        GP.SpatialAnalyst.ZonalStatisticsAsTable(gChannelUnits.FullPath, GISDataStructures.ChannelUnits.ChannelUnitNumberField, gDepthRaster.FullPath, sTableName, "MAXIMUM")

        '        'get values from table
        '        Dim dicTableValues As Dictionary(Of Integer, Double) = GISCode.Table.GetValuesFromTable(sTableName, "UNIT_NUMBE", "MAX")

        '        'update dictionary with values from Zonal Statistics
        '        For Each aSegment As ChannelSegment In dict.Values
        '            For Each aChannelUnit As ChannelUnit In aSegment.ChannelUnits.Values
        '                If dicTableValues.ContainsKey(aChannelUnit.Number) Then
        '                    aChannelUnit.MaxDepth = dicTableValues(aChannelUnit.Number)
        '                Else
        '                    Debug.WriteLine("Channel Unit Number found in max depth dictionary that does not exist in channel unit AUX dictionary")
        '                End If
        '            Next
        '        Next

        '    Catch ex As Exception
        '        ex.Data("sDepthRaster") = gDepthRaster.FullPath
        '        ex.Data("sChannelUnits") = gChannelUnits.FullPath
        '        Throw
        '    End Try

        'End Sub


        ''' <summary>
        ''' For each group, removes all features except the one with the maximum or minimum value
        ''' </summary>
        ''' <param name="sShapefile">Full path to shapefile</param>
        ''' <param name="GroupFieldname">Name of group field. Must be an integer field</param>
        ''' <param name="ValueFieldname">Name of value field. Must be a double field.</param>
        ''' <param name="criteria">Either minimum or maximum</param>
        ''' <remarks>
        ''' Currently support method for GetDepthAtThalwegExit
        ''' FP April 19 2013
        ''' </remarks>
        Protected Shared Sub GetSingleFeatureForGroup(ByVal sShapefile As String,
                                             ByVal GroupFieldname As String,
                                             ByVal ValueFieldname As String,
                                             ByVal criteria As SingleFeatureForGroupEnum)
            'Dim pFC As IFeatureClass
            Dim iGroupField As Integer
            Dim iValueField As Integer
            Dim dicGroupValues As New Dictionary(Of Integer, Double)
            Dim dicGroupOIDs As New Dictionary(Of Integer, Integer)
            Dim aMarkedForDeletion As New ArrayList()
            Dim pFCursor As IFeatureCursor = Nothing
            Dim pFeature As IFeature = Nothing
            Dim iGroup As Integer
            Dim dValue As Double
            Dim iOID As Integer
            Dim iOldOID As Integer
            Dim pTable As ITable = Nothing
            Dim sOIDList As New StringBuilder
            Dim pFilter As IQueryFilter = Nothing
            Dim pGroupField As IField
            Dim pValueField As IField

            'TODO: check field is integer and double
            Try

                '
                'Validation
                '
                If String.IsNullOrEmpty(sShapefile) Then
                    Throw New ArgumentNullException("sShapefile", "The sShapefile input string cannot be null or empty")
                End If
                If Not GISCode.GISDataStructures.PointDataSource.Exists(sShapefile, ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint) Then
                    Throw New ArgumentException("sShapefile", "The shapefile '" & sShapefile & "' does not exist")
                End If
                If String.IsNullOrEmpty(GroupFieldname) Then
                    Throw New ArgumentNullException("GroupFieldname", "The GroupFieldname input string cannot be null or empty")
                End If
                If String.IsNullOrEmpty(ValueFieldname) Then
                    Throw New ArgumentNullException("ValueFieldname", "The ValueFieldname input string cannot be null or empty")
                End If

                'open featureclass
                Dim gPoints As GISDataStructures.PointDataSource
                Try
                    'pFC = GISCode.FeatureClass.GetFeatureClass(sShapefile)
                    gPoints = New GISDataStructures.PointDataSource(sShapefile)
                Catch ex As Exception
                    Throw New Exception("Could not open featureclass '" & sShapefile & "'.")
                End Try

                'check that featureclass has an OID
                If Not gPoints.FeatureClass.HasOID Then
                    Throw New ArgumentException("sShapefile", "Featureclass '" & sShapefile & "' must have an OID.")
                End If

                'check that fields exists
                iGroupField = gPoints.FindField(GroupFieldname)
                If iGroupField < 0 Then
                    Throw New ArgumentException("GroupFieldname", "There is no field named '" & GroupFieldname & "' in featureclass '" & sShapefile & "'.")
                End If
                iValueField = gPoints.FindField(ValueFieldname)
                If iValueField < 0 Then
                    Throw New ArgumentException("ValueFieldname", "There is no field named '" & ValueFieldname & "' in featureclass '" & sShapefile & "'.")
                End If

                'check that field is right type
                pGroupField = gPoints.FeatureClass.Fields.Field(iGroupField)
                If Not pGroupField.Type = esriFieldType.esriFieldTypeInteger And Not pGroupField.Type = esriFieldType.esriFieldTypeSmallInteger Then
                    Throw New ArgumentException("GroupFieldname", "Field '" & GroupFieldname & "' is not an integer field")
                End If

                pValueField = gPoints.FeatureClass.Fields.Field(iValueField)
                If Not pValueField.Type = esriFieldType.esriFieldTypeDouble And Not pValueField.Type = esriFieldType.esriFieldTypeSingle Then
                    Throw New ArgumentException("ValueFieldName", "Field '" & ValueFieldname & "' is not an double field")
                End If

                'create cursor
                pFCursor = gPoints.FeatureClass.Search(Nothing, True)
                pFeature = pFCursor.NextFeature

                'loop through features
                Do While TypeOf pFeature Is IFeature

                    'get values
                    iGroup = pFeature.Value(iGroupField)
                    dValue = pFeature.Value(iValueField)
                    iOID = pFeature.OID

                    'always add the first entry
                    If Not dicGroupValues.ContainsKey(iGroup) Then
                        dicGroupValues.Add(iGroup, dValue)
                        dicGroupOIDs.Add(iGroup, iOID)

                    Else

                        'maximum criteria logic
                        If criteria = SingleFeatureForGroupEnum.Maximum Then

                            'If not higher than existing value, so add to marked for deletion list
                            If dValue <= dicGroupValues(iGroup) Then
                                aMarkedForDeletion.Add(iOID)

                                'Higher than existing value, so add old value to marked for deletion list
                                'and replace with new value
                            Else
                                iOldOID = dicGroupOIDs(iGroup)
                                aMarkedForDeletion.Add(iOldOID)
                                dicGroupOIDs(iGroup) = iOID
                                dicGroupValues(iGroup) = dValue
                            End If

                            'minimum criteria logoc
                        Else

                            'if not lower than existing value, so add to marked for deletion list
                            If dValue >= dicGroupValues(iGroup) Then
                                aMarkedForDeletion.Add(iOID)

                                'Lower than existing value, so add old value to marked for deletion list
                                'and replace with new value
                            Else
                                iOldOID = dicGroupOIDs(iGroup)
                                aMarkedForDeletion.Add(iOldOID)
                                dicGroupOIDs(iGroup) = iOID
                                dicGroupValues(iGroup) = dValue
                            End If
                        End If
                    End If

                    'we now have a list of OID of all features that are not the highest or lowest for the group (depending on criteri)
                    'in aMarkedForDeletion

                    'cleanup and next feature
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeature)
                    pFeature = pFCursor.NextFeature
                Loop

                'cleanup
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)
                pFCursor = Nothing

                'delete oids in marked for deletion list
                'First creates a comma-separated list of all trib OID values
                For Each iOID In aMarkedForDeletion
                    If sOIDList.Length > 0 Then
                        sOIDList.Append(",")
                    End If
                    sOIDList.Append(iOID)
                Next

                'if there are features to be removed, created a filter and delete the rows
                If sOIDList.Length > 0 Then
                    pFilter = New QueryFilter
                    pFilter.WhereClause = """" & gPoints.FeatureClass.OIDFieldName & """ IN (" & sOIDList.ToString & ")"
                    pTable = gPoints.FeatureClass

                    Try
                        pTable.DeleteSearchedRows(pFilter)
                    Catch ex As Exception
                        Throw New Exception("Could not delete features in featureclass '" & sShapefile & "'.")
                    End Try

                End If

            Catch ex As Exception
                ex.Data("sShapefile") = sShapefile
                ex.Data("GroupFieldname") = GroupFieldname
                ex.Data("ValueFieldname") = ValueFieldname
                ex.Data("criteria") = criteria.ToString
                Throw
            Finally
                'cleanup
                If pFCursor IsNot Nothing Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)
                End If
                If pFilter IsNot Nothing Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFilter)
                End If
                If pTable IsNot Nothing Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pTable)
                End If
            End Try

        End Sub

#End Region
    End Class

End Namespace