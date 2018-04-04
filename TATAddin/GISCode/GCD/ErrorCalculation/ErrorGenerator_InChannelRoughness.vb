
Public Class ErrorGenerator_InChannelRoughness


    Private m_gChannelUnitsFC As GISDataStructures.PolygonDataSource
    Private m_dChannelUnitWithGrainSize As Dictionary(Of UInteger, GISCode.CHaMP.ChannelUnit)
    Private m_gDEM As GISDataStructures.Raster

    ''' <summary>
    ''' RBT Constructor
    ''' </summary>
    ''' <param name="gChannelUnits">Feature class of channel units</param>
    ''' <param name="dChannelUnitWithGrainSize">dictionary of ChannelUnitWithGrainSize objects, the key to the dictionary is the channel unit id</param>
    ''' <param name="gDEM">DEM</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal gChannelUnits As GISDataStructures.PolygonDataSource, ByVal dChannelUnitWithGrainSize As Dictionary(Of UInteger, GISCode.CHaMP.ChannelUnit), gDEM As GISDataStructures.Raster)
        m_gChannelUnitsFC = gChannelUnits
        m_dChannelUnitWithGrainSize = dChannelUnitWithGrainSize
        m_gDEM = gDEM
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sOutputRasterPath">Path to create output raster at</param>
    ''' <param name="sUniqueChannelFieldName">Field used to identify unique channel units, typically is "Unit_Numbe"</param>
    ''' <param name="bCreateCDF">OPTIONAL: Boolean flag to tell function if a cumulative distribution function should be created and saved for each channel unit</param>
    ''' <param name="sOutputCDFFolder">OPTIONAL: folder to save cumulative distribution functions to</param>
    ''' <returns>GISDataStructures.Raster that is concurrent and orthogonal with m_gDEM</returns>
    ''' <remarks></remarks>
    Public Function Execute(ByVal sOutputRasterPath As String, ByVal sUniqueChannelFieldName As String, Optional ByVal bCreateCDF As Boolean = False, Optional ByVal sOutputCDFFolder As String = Nothing) As GISDataStructures.Raster

        'Create a temporary copy of the polygon feature class
        Dim sTempPolygonPath As String = GISCode.WorkspaceManager.GetTempShapeFile("TempCUs")
        GISCode.GP.DataManagement.CopyFeatures(m_gChannelUnitsFC.FeatureClass, sTempPolygonPath, True)
        Dim gTempPolygonFC As GISCode.GISDataStructures.PolygonDataSource
        If System.IO.File.Exists(sTempPolygonPath) Then
            gTempPolygonFC = New GISCode.GISDataStructures.PolygonDataSource(sTempPolygonPath)

            If gTempPolygonFC.FeatureCount < 1 Then
                Debug.WriteLine("The temporary copy of the channel units feature class is empty.")
                Return Nothing
            End If
            gTempPolygonFC.AddField("D50", ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeDouble)
        Else
            Throw New Exception(String.Format("Error accessing polygon feature class {0}.", m_gChannelUnitsFC.FullPath))
            Exit Function
        End If

        ' The temporary ShapeFile will truncate the field name if it is more than 10 chars (e.g. "Unit_Number" becomes "Unit_Numbe")
        Dim sTempCUField As String = sUniqueChannelFieldName
        If gTempPolygonFC.GISDataStorageType = GISDataStructures.GISDataStorageTypes.ShapeFile Then
            sTempCUField = sTempCUField.Substring(0, 10)
        End If

        Dim pKeys As System.Collections.ICollection = m_dChannelUnitWithGrainSize.Keys
        Dim pResults As GCD.GrainSizeDistributionCalculator.GrainSizeDistributionResults

        For Each iKey In pKeys
            pResults = GISCode.GCD.GrainSizeDistributionCalculator.ProcessRBT_ChannelUnits(m_dChannelUnitWithGrainSize(iKey))

            'Update the feature class
            Dim fcBuffer As ESRI.ArcGIS.Geodatabase.IFeatureBuffer = gTempPolygonFC.FeatureClass.CreateFeatureBuffer()
            Dim queryFilter As ESRI.ArcGIS.Geodatabase.IQueryFilter = New ESRI.ArcGIS.Geodatabase.QueryFilterClass()
            Dim iChannelUnitValue As UInteger = iKey
            'Use channel unit number field, typically "Unit_Numbe" to access the specific channel unit in the feature class
            queryFilter.WhereClause = String.Format("""{0}"" = {1}", sTempCUField, iChannelUnitValue)
            Dim iD50FieldIndex As Integer = gTempPolygonFC.FindField("D50")
            Dim updateCursor As ESRI.ArcGIS.Geodatabase.ICursor = gTempPolygonFC.FeatureClass.Update(queryFilter, False)
            Dim pRow As ESRI.ArcGIS.Geodatabase.IRow = updateCursor.NextRow()

            While Not pRow Is Nothing
                pRow.Value(iD50FieldIndex) = pResults.D50
                updateCursor.UpdateRow(pRow)
                pRow = updateCursor.NextRow()
            End While

            Dim comReferencesLeft As Integer
            Do
                comReferencesLeft = System.Runtime.InteropServices.Marshal.ReleaseComObject(updateCursor) _
                                    + System.Runtime.InteropServices.Marshal.ReleaseComObject(fcBuffer)
            Loop While (comReferencesLeft > 0)

            Debug.WriteLine("D50: " & pResults.D50 & vbCrLf)

            If bCreateCDF Then
                Dim sCDFFolderName As String = System.IO.Path.GetFileNameWithoutExtension(m_gChannelUnitsFC.FullPath)
                Dim sCDFFolderPath As String = System.IO.Path.Combine(sOutputCDFFolder, sCDFFolderName & "_CDFs")
                If Not System.IO.Directory.Exists(sCDFFolderPath) Then
                    System.IO.Directory.CreateDirectory(sCDFFolderPath)
                End If

                Dim sCDFFilePath As String = System.IO.Path.Combine(sCDFFolderPath, "CU_" & iChannelUnitValue & ".png")
                GISCode.GCD.GrainSizeDistributionCalculator.CreateCDFPlot(pResults, sCDFFilePath)
            End If

        Next

        GISCode.GP.Conversion.PolygonToRaster_conversion(gTempPolygonFC, "D50", sOutputRasterPath, m_gDEM)

        Dim gResult As GISDataStructures.Raster = Nothing
        ' Temporary fix. The copy raster routine seems to be messing up the projection on 
        ' the resultant raster. So assume that the output project is identical to the
        ' input project and simply use the geoprocessing routine to define it.

        'Validate raster output was created and that it has a spatial reference
        Dim pSR As ESRI.ArcGIS.Geometry.ISpatialReference = Nothing
        If TypeOf m_gDEM Is GISDataStructures.Raster Then
            pSR = m_gDEM.SpatialReference
            If System.IO.File.Exists(sOutputRasterPath) Then
                gResult = New GISDataStructures.Raster(sOutputRasterPath)
                GP.DataManagement.DefineProjection(gResult.RasterDataset, pSR)
            Else
                Throw New Exception(String.Format("The in channel roughness raster {0} was not created.", sOutputRasterPath))
                Exit Function
            End If
        End If

        ' Safely try and delete the temporary feature class. This is helpful if this tool is called in batch.
        Try
            GP.DataManagement.Delete(sTempPolygonPath)
        Catch ex As Exception
            Debug.WriteLine("Error deleting temporary feature class " & sTempPolygonPath)
        End Try

        Return gResult

    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        m_gChannelUnitsFC = Nothing
    End Sub
End Class
