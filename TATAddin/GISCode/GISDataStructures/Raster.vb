Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.DataSourcesRaster
Imports ESRI.ArcGIS.CatalogUI
Imports ESRI.ArcGIS.Catalog
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.GeoAnalyst
Imports ESRI.ArcGIS.SpatialAnalyst
Imports ESRI.ArcGIS.Analyst3D
Imports ESRI.ArcGIS.Display
Imports GCDAddIn.GISCode.ErrorHandling.ExceptionUI

Namespace GISCode.GISDataStructures

    Public Class Raster
        Inherits GISDataSource

        Public Const NullRasterValue As Double = -9999

        Public Enum RasterTypes
            ASCII
            ESRIGrid
            IMG
            FileGeodatabase
            TIFF
        End Enum

#Region "Layer Constants"

        Private Const SymbologyDefault As String = "DEM_Default.lyr"
        Private Const SymbologyDEM As String = "DEM_Default.lyr"
        Private Const SymbologyDoD As String = "DOD_Classed_1m.lyr"
        Private Const SymbologyError As String = "Error_Default.lyr"
        Private Const SymbologyPointDensity As String = "PointDensity.lyr"
        Private Const SymbologyPointQuality As String = "PointQuality.lyr"
        Private Const SymbologyRoughness As String = "Roughness.lyr"
        Private Const SymbologySlope As String = "Slope.lyr"
        Private Const SymbologySlopeDegrees As String = "SlopeDegrees.lyr"
        Private Const SymbologySlopePercentRise As String = "SlopePercentRise.lyr"


#End Region

#Region "Members"

        Private m_eRasterType As RasterTypes
        Private m_pRasterDataset As IRasterDataset

#End Region

#Region "Constructors"

        Public Sub New(ByVal sFullPath As String, Optional bCalculateStatistics As Boolean = True)
            MyBase.New(sFullPath)

            If GISDataStorageType = GISDataStorageTypes.FileGeodatase Then
                m_eRasterType = RasterTypes.FileGeodatabase
            Else
                If IO.Path.HasExtension(sFullPath) Then
                    Dim sExtension As String = IO.Path.GetExtension(sFullPath)
                    If String.Compare(sExtension, ".img", True) = 0 Then
                        m_eRasterType = RasterTypes.IMG
                    ElseIf String.Compare(sExtension, ".tif", True) = 0 Then
                        m_eRasterType = RasterTypes.TIFF
                    End If
                Else
                    m_eRasterType = RasterTypes.ESRIGrid
                End If
            End If

            Dim pWS As IWorkspace = Workspace
            If pWS.Type = esriWorkspaceType.esriLocalDatabaseWorkspace Then
                If TypeOf pWS Is IRasterWorkspaceEx Then
                    Dim pRasWS As IRasterWorkspaceEx = pWS
                    m_pRasterDataset = pRasWS.OpenRasterDataset(IO.Path.GetFileName(sFullPath))
                End If
            Else
                If TypeOf pWS Is IRasterWorkspace Then
                    Dim pRasWS As IRasterWorkspace = pWS
                    Try
                        m_pRasterDataset = pRasWS.OpenRasterDataset(IO.Path.GetFileName(sFullPath))
                    Catch ex As Exception
                        '
                        ' Open raster dataset will fail when the raster doesn't exist.
                        ' Improve by looping though dataset names prior to opening?
                        '
                    End Try
                End If
            End If
            MyBase.GeoDataset = m_pRasterDataset

            If bCalculateStatistics Then
                If TypeOf m_pRasterDataset Is IRasterBandCollection Then
                    Try
                        Dim pRasterBandColl As IRasterBandCollection = m_pRasterDataset ' Me.Geodataset
                        CalculateStatistics(pRasterBandColl)
                    Catch ex As Exception
                        Debug.WriteLine("Error calculating statistics for: " & Me.FullPath)
                    End Try
                End If
            End If

        End Sub

        ''' <summary>
        ''' Creates a new raster object from a raster layer
        ''' </summary>
        ''' <param name="pRasterLayer">IRasterLayer object</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal pRasterLayer As IRasterLayer)
            Me.New(pRasterLayer.FilePath)
        End Sub

        Protected Overrides Sub Finalize()
            'If TypeOf m_pRasterDataset Is IRasterDataset Then
            '    Runtime.InteropServices.Marshal.ReleaseComObject(m_pRasterDataset)
            '    m_pRasterDataset = Nothing
            'End If
            MyBase.Finalize()
        End Sub

        ''' <summary>
        ''' Create a contour feature class at the specified elevation
        ''' </summary>
        ''' <param name="fElevation"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateContour(ByVal fElevation As Double) As IFeatureClass

            Dim pcntArr(1) As Double
            pcntArr(0) = fElevation

            Dim pcntList As Object = pcntArr

            ' Declare the output object
            Dim pFClassOut As IFeatureClass = Nothing

            ' Call the method
            Try
                Dim pSurfaceOp As ISurfaceOp = New RasterSurfaceOp
                pFClassOut = pSurfaceOp.ContourList(Me.Raster, pcntList)

            Catch ex As Exception
                Dim ex2 As New Exception("Error generating raster contour", ex)
                ex2.Data.Add("Raster", FullPath)
                ex2.Data.Add("Elevation", fElevation.ToString)
                Throw ex2
            End Try

            Return pFClassOut

        End Function

#End Region

#Region "Properties"

        Public ReadOnly Property RasterType
            Get
                Return m_eRasterType
            End Get
        End Property

        Public ReadOnly Property RasterDataset As IRasterDataset
            Get
                Return MyBase.GeoDataset
            End Get
        End Property

        Public ReadOnly Property RasterLayer As IRasterLayer
            Get
                Dim pRLResult As IRasterLayer = New RasterLayer
                pRLResult.CreateFromDataset(RasterDataset)
                Return pRLResult
            End Get
        End Property

        Public ReadOnly Property Raster As IRaster
            Get
                Return RasterLayer.Raster
            End Get
        End Property

        Public ReadOnly Property CellSize As Double
            Get
                Dim pRasProp As IRasterProps = Raster
                Dim iHeightPixels As Integer = pRasProp.Height
                Dim dHeightExtend As Double = pRasProp.Extent.Height
                Dim fCellSize As Double = dHeightExtend / iHeightPixels
                Return fCellSize
            End Get
        End Property

        Public ReadOnly Property Minimum() As Double
            Get
                Return RasterStatistics.Minimum
            End Get
        End Property

        Public ReadOnly Property Maximum() As Double
            Get
                Return RasterStatistics.Maximum
            End Get
        End Property

        Public ReadOnly Property Mean As Double
            Get
                Return RasterStatistics.Mean
            End Get
        End Property

        Public ReadOnly Property StandardDeviation As Double
            Get
                Return RasterStatistics.StandardDeviation
            End Get
        End Property

        Public ReadOnly Property RasterStatistics As IRasterStatistics
            Get
                Dim bandCollection As IRasterBandCollection = Geodataset
                Dim rasterBand As IRasterBand = bandCollection.Item(0)
                Dim rasterStats As IRasterStatistics = rasterBand.Statistics
                Return rasterStats
            End Get
        End Property

        'Public ReadOnly Property ExtentAsPolygon
        '    Get
        '        m_pRasterDataset.
        '    End Get
        'End Property

        Public Shadows ReadOnly Property Geodataset As IGeoDataset
            Get
                Return MyBase.GeoDataset ' DirectCast(RasterDataset, IGeoDataset)
            End Get
        End Property

        Public ReadOnly Property Surface As ISurface
            Get
                Dim pSurface As ISurface
                Dim pSurf As IRasterSurface = New RasterSurface
                Dim pBands As IRasterBandCollection = RasterDataset
                pSurf.RasterBand = pBands.Item(0)
                pSurface = pSurf
                Return pSurface
            End Get
        End Property

#End Region

#Region "Methods"

        'Public Overrides Function Validate(ByRef lErrors As List(Of GISException), gReferenceSR As ISpatialReference) As Boolean
        '    Return MyBase.Validate(lErrors, gReferenceSR)
        'End Function

        Public Function GetValueAtPoint(ByVal pPoint As IPoint, ByRef fResult As Double) As Boolean

            If Not TypeOf pPoint Is IPoint Then
                Throw New ArgumentException("Invalid or null point argument", "pPoint")
            End If

            Dim pIdentify As IIdentify
            Dim pIDArray As IArray
            Dim pRIDObj As IRasterIdentifyObj = Nothing
            Dim bResult As Boolean

            pIdentify = RasterLayer
            pIDArray = pIdentify.Identify(pPoint)

            If TypeOf pIDArray Is IArray Then
                If TypeOf pIDArray.Element(0) Is IRasterIdentifyObj Then
                    pRIDObj = pIDArray.Element(0)
                    If Double.TryParse(pRIDObj.Name.ToString, fResult) Then
                        bResult = True
                    Else
                        fResult = GISDataStructures.Raster.NullRasterValue
                        bResult = False

                        'Dim ex As New Exception("Error obtaining raster value at point")
                        'ex.Data("Raster", FullPath)
                        'ex.Data("X", pPoint.X.ToString)
                        'ex.Data("Y", pPoint.Y.ToString)
                        'Throw ex
                    End If
                End If
            End If

            If Not pRIDObj Is Nothing Then
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pRIDObj)
            End If
            If Not pIDArray Is Nothing Then
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pIDArray)
            End If

            pIdentify = Nothing
            pIDArray = Nothing
            pRIDObj = Nothing
            Return bResult

        End Function

        Public Sub GetRasterProperties(ByRef CellResolution As Double, _
                                       ByRef ExtentLeft As Double, _
                                       ByRef ExtentBottom As Double, _
                                       ByRef ExtentRight As Double, _
                                       ByRef ExtentTop As Double,
                                       Optional ByVal decimals As Integer = -1)

            If decimals < -1 Then
                Throw New ArgumentOutOfRangeException("decimals", decimals, "Precision must be greater than or equal to -1")
            End If

            'Dim fiRaster As New FileInfo(sRasterPath)
            'Dim pRDS As IRasterDataset = Raster.GetRasterDataset(sRasterPath)
            Dim pRL As IRasterLayer = New RasterLayer

            Try
                pRL.CreateFromDataset(RasterDataset)
            Catch ex As Exception
                Dim ex2 As New Exception("Unable to create raster layer from raster dataset", ex)
                ex2.Data.Add("RasterPath", FullPath)
                Throw ex2
            End Try

            Dim pGeoDS As IGeoDataset = pRL.Raster
            Dim pExtent As IEnvelope = pGeoDS.Extent
            Dim pSpat As IProjectedCoordinateSystem = pGeoDS.SpatialReference
            Dim sUnit As String = Nothing
            If TypeOf pSpat Is IProjectedCoordinateSystem Then
                sUnit = GetLinearUnitsAsString(LinearUnits) ' GISCode.ArcMap.GetLinearUnitsAsString(pSpat.CoordinateUnit)
            End If

            CellResolution = CellSize ' GetCellSize(pRL.Raster)
            ExtentTop = pExtent.UpperRight.Y
            ExtentLeft = pExtent.LowerLeft.X
            ExtentRight = pExtent.UpperRight.X
            ExtentBottom = pExtent.LowerLeft.Y

            If decimals > 0 And decimals < 16 Then
                CellResolution = Math.Round(CellResolution, decimals)
                ExtentTop = Math.Round(ExtentTop, decimals)
                ExtentLeft = Math.Round(ExtentLeft, decimals)
                ExtentRight = Math.Round(ExtentRight, decimals)
                ExtentBottom = Math.Round(ExtentBottom, decimals)
            End If

        End Sub

        Public Function GetRasterPropertiesAsString(Optional ByVal precision As Integer = -1) As String

            If precision < -1 Then
                Throw New ArgumentOutOfRangeException("Precision", precision, "Precision must be greater than or equal to -1")
            End If

            Dim pExtent As IEnvelope = Geodataset.Extent
            Dim pSpat As IProjectedCoordinateSystem = Geodataset.SpatialReference
            Dim sUnit As String = Nothing
            If TypeOf pSpat Is IProjectedCoordinateSystem Then
                sUnit = GetLinearUnitsAsString(pSpat.CoordinateUnit)
            End If
            Dim sResult As String = ""

            'output for cellsize
            Dim CellSize As Double = CellSize
            If precision > -1 Then
                CellSize = Math.Round(CellSize, precision)
            End If

            sResult = "Cell size: " & CellSize.ToString

            'output for units
            If Not String.IsNullOrEmpty(sUnit) Then
                sResult &= " " & sUnit
            End If

            'output for rows and columns
            Dim Columns As Double = RasterLayer.ColumnCount
            Dim Rows As Double = RasterLayer.RowCount
            If precision > -1 Then
                Columns = Math.Round(Columns, precision)
                Rows = Math.Round(Rows, precision)
            End If

            sResult &= vbNewLine & "Columns and Rows: " & Columns.ToString("#,##0") & ", " & Rows.ToString("#,##0")
            If TypeOf pSpat Is ISpatialReference Then
                sResult &= vbNewLine & "Spatial Reference: " & pSpat.Name
            End If

            Dim sCoordinateFormat As String = "#,##0.##########"
            sResult &= vbNewLine & vbNewLine

            'output for extent
            Dim Top As Double = pExtent.UpperRight.Y
            Dim Left As Double = pExtent.LowerLeft.X
            Dim Right As Double = pExtent.UpperRight.X
            Dim Bottom As Double = pExtent.LowerLeft.Y

            If precision > -1 Then
                Top = Math.Round(Top, precision)
                Left = Math.Round(Left, precision)
                Right = Math.Round(Right, precision)
                Bottom = Math.Round(Bottom, precision)
            End If

            sResult &= "Extent:"
            sResult &= vbNewLine & "Top: " & Top.ToString(sCoordinateFormat)
            sResult &= vbNewLine & "Left: " & Left.ToString(sCoordinateFormat)
            sResult &= vbNewLine & "Right:  " & Right.ToString(sCoordinateFormat)
            sResult &= vbNewLine & "Bottom: " & Bottom.ToString(sCoordinateFormat)

            Return sResult

        End Function

        Public Sub SymbolizeDEM(ByRef pRL As IRasterLayer)

            'Create the start and end colors
            Dim startColor1 As IRgbColor = New RgbColor
            startColor1.Red = 156
            startColor1.Green = 85
            startColor1.Blue = 31

            Dim endColor1 As IRgbColor = New RgbColor
            endColor1.Red = 255
            endColor1.Green = 255
            endColor1.Blue = 191

            Dim gRamp1 As IAlgorithmicColorRamp = CreateColorRamp(startColor1, endColor1)

            'Create the start and end colors
            Dim startColor2 As IRgbColor = New RgbColor
            startColor2.Red = 255
            startColor2.Green = 255
            startColor2.Blue = 191

            Dim endColor2 As IRgbColor = New RgbColor
            endColor2.Red = 33
            endColor2.Green = 130
            endColor2.Blue = 145

            Dim gRamp2 As IAlgorithmicColorRamp = CreateColorRamp(startColor2, endColor2)

            Dim pStretchColorRamp As IRasterStretchColorRampRenderer = New RasterStretchColorRampRenderer
            Dim pRasterRenderer As IRasterRenderer = pStretchColorRamp
            pRasterRenderer.Raster = Me.Raster
            pRasterRenderer.Update()

            ' create multipart color ramp
            Dim pMPRamp As IMultiPartColorRamp = New MultiPartColorRamp
            pMPRamp.AddRamp(gRamp1)
            pMPRamp.AddRamp(gRamp2)
            pMPRamp.Size = 255
            Dim bOK As Boolean
            pMPRamp.CreateRamp(bOK)


            pStretchColorRamp.BandIndex = 0
            pStretchColorRamp.ColorRamp = pMPRamp

            pRasterRenderer.Update()
            pRasterRenderer.ResamplingType = rstResamplingTypes.RSP_BilinearInterpolation

            pRL.Renderer = pRasterRenderer

        End Sub

        Private Function CreateColorRamp(ByVal cFromColor As IRgbColor, ByVal cToColor As IRgbColor) As ESRI.ArcGIS.Display.IAlgorithmicColorRamp

            'Create a new color algorithmic ramp
            Dim algColorRamp As ESRI.ArcGIS.Display.IAlgorithmicColorRamp = New AlgorithmicColorRamp
            algColorRamp.Algorithm = esriColorRampAlgorithm.esriCIELabAlgorithm

            'Set the Start and End Colors
            algColorRamp.ToColor = cToColor
            algColorRamp.FromColor = cFromColor

            'Set the ramping Alglorithm 
            algColorRamp.Algorithm = esriColorRampAlgorithm.esriCIELabAlgorithm

            'Set the size of the ramp (the number of colors to be derived)
            algColorRamp.Size = 255

            'Create the ramp
            algColorRamp.CreateRamp(True)

            Return algColorRamp

        End Function
#End Region

#Region "Shared Functions"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="eType"></param>
        ''' <param name="bAddPeriod"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Function GetRasterExtension(ByVal eType As RasterTypes, Optional ByVal bAddPeriod As Boolean = True) As String

            Dim sExtension As String = String.Empty
            Select Case eType
                Case RasterTypes.IMG : sExtension = "img"
                Case RasterTypes.TIFF : sExtension = "tif"
            End Select

            If Not String.IsNullOrEmpty(sExtension) Then
                If bAddPeriod Then
                    sExtension = "." & sExtension
                End If
            End If

            Return sExtension

        End Function

        ''' <summary>
        ''' Calculates the raster statistics for a raster.
        ''' </summary>
        ''' <param name="pRasterBandColl">The collection of bands that make up the raster</param>
        ''' <remarks>Raster statistics are needed before any symbology work can be performed on a raster.
        ''' For example, the statistics are needed before the bankfull slider can be used.
        ''' 
        ''' Note that the raster band collection can contain many bands, but that this method only
        ''' reclaculates the statistics for the first band (because this is all we use for DEMs and other
        ''' common RBT and GCD raasters.
        ''' 
        ''' Added check for existing stats and call to ComputeStatsAndHist if HasStatistics is false - Frank Aug 12 2011
        ''' </remarks>
        Public Shared Sub CalculateStatistics(ByVal pRasterBandColl As IRasterBandCollection)

            If Not TypeOf pRasterBandColl Is IRasterBandCollection Then
                Throw New ArgumentNullException("pRasterBandColl", "Invalid raster band collection")
            End If

            Dim bHasStatistiscs As Boolean
            If TypeOf pRasterBandColl Is IRasterBandCollection Then
                If pRasterBandColl.Count > 0 Then
                    Dim pEnum As IEnumRasterBand = pRasterBandColl.Bands
                    Dim pBand As IRasterBand = pEnum.Next
                    Try

                        pBand.HasStatistics(bHasStatistiscs)
                        If bHasStatistiscs Then
                            Try
                                Dim pStats As IRasterStatistics = pBand.Statistics
                                pStats.Recalculate()
                            Catch ex As DivideByZeroException
                                Debug.WriteLine("Exception occurred in ESRI IRasterBand.Statistics routine. Safe to proceed.")
                                '
                                ' Do nothing. Statistics already exist
                                '
                            End Try
                        Else
                            pBand.ComputeStatsAndHist()
                        End If
                    Catch ex As Exception
                        ex.Data("Band") = pBand.Bandname
                        Throw
                    End Try
                End If
            End If

        End Sub

        Public Sub CalculateStatistics()
            CalculateStatistics(Me.RasterDataset)
        End Sub

        Public Shared Sub ApplySymbology(ByRef pRasterLayer As IRasterLayer, ByVal sSymobologyLayerFile As String)

            If String.IsNullOrEmpty(sSymobologyLayerFile) Then
                Throw New ArgumentNullException("Symbology Layer File", "Null or empty symbology layer file")
            End If

            If Not IO.File.Exists(sSymobologyLayerFile) Then
                Dim ex As New ArgumentOutOfRangeException("SymbologyLayerFile", sSymobologyLayerFile, "The symbology layer file doesn't exist")
                ex.Data("Symbology file") = sSymobologyLayerFile
                Throw ex
            End If
            '
            ' Create a layer for this data source. Get the default symbology for this raster type
            ' apply the symbology to the new layer and then add it to the map
            '
            Dim pRR As IRasterRenderer = Nothing
            Dim pLayer As ILayer = Nothing
            Dim pGXLayer As IGxLayer = New GxLayerClass
            Dim pGXFile As IGxFile = pGXLayer
            '
            ' Load this layer file and retrieve the symbology from it.
            '
            pGXFile.Path = sSymobologyLayerFile
            Dim prlayer As IRasterLayer = pGXLayer.Layer
            pRR = prlayer.Renderer
            '
            ' Reset the renderer information. Remember, the renderer from the saved layer file
            ' may have been stretched to a different data range, so reset the labels.
            '
            ' For classified renderers, get the first legend group and reset the text label.
            ' Note: this was for some of the legends that Joe Wheaton provided, they had "T-Score"
            ' as the legend title.
            '
            If TypeOf pRR Is IRasterStretchColorRampRenderer Then
                Dim pStretch As IRasterStretchColorRampRenderer = pRR
                pStretch.ResetLabels()
            ElseIf TypeOf pRR Is ILegendInfo Then
                Dim pLegend As ILegendInfo = pRR
                pLegend.LegendGroup(0).Heading = String.Empty
            End If
            pRR.Update()
            '
            ' Now load the data source into a layer file and apply the symbology from above if one was created.
            '
            If TypeOf pRR Is IRasterRenderer Then
                pRasterLayer.Renderer = pRR
            End If

        End Sub

        Public Shared Function BrowseOpen(ByVal sFormTitle As String, Optional ByRef sWorkspace As String = Nothing, Optional ByRef sName As String = Nothing) As GISDataStructures.Raster

            Dim pGxDialog As IGxDialog = New GxDialog
            Dim pRasterFilter As IGxObjectFilter = New GxFilterRasterDatasets()
            Dim pFilterCol As IGxObjectFilterCollection = Nothing
            Dim pEnumGx As IEnumGxObject = Nothing
            Dim pGxObject As IGxObject = Nothing

            If TypeOf sWorkspace Is String Then
                sWorkspace = ""
            End If

            If TypeOf sName Is String Then
                sName = ""
            End If

            pFilterCol = pGxDialog
            pFilterCol.AddFilter(pRasterFilter, True)
            pGxDialog.RememberLocation = True
            pGxDialog.AllowMultiSelect = False
            pGxDialog.Title = sFormTitle

            If Not String.IsNullOrEmpty(sWorkspace) Then
                If IO.Directory.Exists(sWorkspace) Then
                    pGxDialog.StartingLocation = sWorkspace
                End If
            End If

            Dim rResult As Raster = Nothing
            Try
                If pGxDialog.DoModalOpen(0, pEnumGx) Then
                    pGxObject = pEnumGx.Next
                    Dim sFile As New IO.FileInfo(pGxObject.FullName)
                    sName = pGxObject.Name
                    sWorkspace = sFile.Directory.FullName

                    rResult = New Raster(pGxObject.FullName)
                End If

            Catch ex As DivideByZeroException


            Catch ex As Exception
                ex.Data("Title") = sFormTitle
                ex.Data("Workspace") = sWorkspace
                ex.Data("Name") = sName
                Throw
            End Try

            Return rResult

        End Function

        Public Shared Function BrowseOpen(ByRef cbo As System.Windows.Forms.ComboBox, ByVal sFormTitle As String, Optional ByVal tTip As System.Windows.Forms.ToolTip = Nothing) As GISDataStructures.Raster

            Dim sInput = String.Empty

            Dim rResult As Raster = BrowseOpen(sFormTitle)

            ' New code to check it it is an ESRI grid - this is causing the cbo box to fail when loading
            If rResult IsNot Nothing Then
                If IO.Directory.Exists(rResult.FullPath) Then
                    MsgBox("ESRI Grid format rasters are currently unsupported by the " & My.Resources.ApplicationNameShort & ". Please convert it to a supported datatype.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                    Return Nothing
                ElseIf String.Compare(IO.Path.GetExtension(rResult.FullPath), ".asc", True) = 0 Then
                    MsgBox("ASCII format rasters are currently unsupported by " & My.Resources.ApplicationNameShort & ". Please convert it to a supported datatype.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                    Return Nothing
                End If
            End If

            If TypeOf rResult Is Raster Then

                ' Add the selected feature class to the combobox and make it the selected item.
                Dim nSelectIndex As Integer = -1
                For i = 0 To cbo.Items.Count - 1
                    If TypeOf cbo.Items(i) Is GISDataStructures.Raster Then
                        If String.Compare(rResult.FullPath, cbo.Items(i).FullPath, True) = 0 Then
                            nSelectIndex = i
                            Exit For
                        End If
                    End If
                Next

                If nSelectIndex >= 0 Then
                    cbo.SelectedIndex = nSelectIndex
                Else
                    Dim nIndex As Integer = cbo.Items.Add(rResult)
                    If nIndex >= 0 Then
                        cbo.SelectedIndex = nIndex
                    End If
                End If

                If TypeOf tTip Is System.Windows.Forms.ToolTip Then
                    tTip.SetToolTip(cbo, rResult.FullPath)
                End If
            End If

            Return rResult

        End Function

        Public Shared Function BrowseOpen(ByRef txt As System.Windows.Forms.TextBox,
                                          ByVal sFormTitle As String,
                                          ByVal sErrorMessage As String) As GISDataStructures.Raster

            Dim sFolder As String = String.Empty
            Dim sName As String = String.Empty
            Dim gResult As GISDataStructures.Raster = Nothing
            If Not String.IsNullOrEmpty(txt.Text) Then
                Try
                    If GISDataStructures.Raster.Exists(txt.Text) Then
                        gResult = New GISDataStructures.Raster(txt.Text)
                        sFolder = gResult.WorkspacePath
                        sName = gResult.DatasetName
                    End If
                Catch ex As Exception
                End Try
            End If

            Try
                gResult = BrowseOpen(sFormTitle, sFolder, sName)
                If TypeOf gResult Is GISDataStructures.Raster Then
                    txt.Text = gResult.FullPath
                End If
            Catch ex As Exception
                ExceptionUI.HandleException(ex, sErrorMessage)
            End Try

            Return gResult

        End Function

        Public Shared Function BrowseSave(ByVal sFormTitle As String, Optional ByRef sWorkspace As String = Nothing, Optional ByRef sName As String = Nothing) As String

            Dim pGxDialog As IGxDialog = New GxDialog
            Dim pFilterCol As IGxObjectFilterCollection = pGxDialog
            pFilterCol.AddFilter(New GxFilterRasterDatasets(), True)
            pGxDialog.RememberLocation = True
            pGxDialog.AllowMultiSelect = False
            pGxDialog.Title = sFormTitle

            If Not String.IsNullOrEmpty(sWorkspace) Then
                pGxDialog.StartingLocation = sWorkspace
            End If

            If TypeOf sWorkspace Is String Then
                sWorkspace = String.Empty
            End If

            If TypeOf sName Is String Then
                sName = String.Empty
            End If

            Dim sResult As String = String.Empty

            Try
                If pGxDialog.DoModalSave(0) Then
                    sWorkspace = pGxDialog.FinalLocation.FullName
                    sName = pGxDialog.Name
                    sResult = IO.Path.Combine(sWorkspace, sName) '  rResult = New Raster(IO.Path.Combine(sWorkspace, sName))
                End If

            Catch ex As Exception
                ex.Data("Title") = sFormTitle
                ex.Data("Folder") = sWorkspace
                ex.Data("Name") = sName
                Throw
            End Try

            Return sResult

        End Function


        ''' <summary>
        ''' Core ArcObjects method to copy a raster
        ''' </summary>
        ''' <param name="sOutPath">file path to copy raster to </param>
        ''' <param name="sRasterType">raster format name, defaults to .tif, for list of acceptable formats see http://resources.arcgis.com/en/help/arcobjects-net/componenthelp/index.html#//0025000007z8000000 </param>
        ''' <returns>true if successful, false if failed to copy raster</returns>
        ''' <remarks>This can replace calls to GP.DataManagement.Copy. Need to integrate more raster formats/validating</remarks>
        Public Function Copy(ByVal sOutPath As String, Optional ByVal sRasterType As String = "TIFF")

            Dim bRasterCopied As Boolean = False

            'Create a ISaveAs object from the IRaster so we can save the raster to disk
            Dim pSaveAs As ESRI.ArcGIS.Geodatabase.ISaveAs = CType(Me.Raster, ESRI.ArcGIS.Geodatabase.ISaveAs)

            'Check that indeed the raster can be saved as sRasterType provided
            If pSaveAs.CanSaveAs(sRasterType) Then

                'Save the raster to path provided
                If Not GISDataStructures.Raster.Exists(sOutPath) Then
                    Dim pDataset As ESRI.ArcGIS.Geodatabase.IDataset = pSaveAs.SaveAs(sOutPath, Me.Workspace, "TIFF")
                Else
                    Throw New Exception("The path provided to copy the raster to already exists. Provide a different path.")
                End If

                If Exists(sOutPath) Then
                    bRasterCopied = True
                End If

            End If

            Return bRasterCopied

        End Function

        Public Function Delete()

            Dim bDeleted As Boolean = False

            Dim pDataset As IDataset = CType(Me.RasterDataset, IDataset)
            If TypeOf pDataset Is IDataset Then
                If pDataset.CanDelete() Then
                    pDataset.Delete()
                End If
            End If

            If Not Exists(Me.FullPath) Then
                bDeleted = True
            End If

            Return bDeleted

        End Function

        ''' <summary>
        ''' Given an existing input raster source, this method will produce a new raster name that does not already exist.
        ''' </summary>
        ''' <param name="sInputPath">Input raster name (file geodatabases are OK).</param>
        ''' <param name="eType">Raster type.</param>
        ''' <param name="sRootName">Root name of the output.</param>
        ''' <param name="nMax">The maximum size of the output file name excluding the workspace path, but including any file extension and pattern</param>
        ''' <returns>Fully qualified raster path</returns>
        ''' <remarks>Philip 7 May 2011</remarks>
        '''
        Public Shared Function GetNewSafeName(ByVal sInputPath As String, ByVal eType As RasterTypes, Optional ByVal sRootName As String = "", Optional ByVal nMax As Byte = 0) As String
            '
            ' Validate the inputs
            '
            If String.IsNullOrEmpty(sInputPath) Then
                Throw New ArgumentNullException("InputPath", "Null or empty input path. Expecting raster or workspace path.")
            End If

            If nMax < 0 Then
                Throw New ArgumentOutOfRangeException("Max", nMax, "Invalid max length. Must be greater than or equal to zero.")
            End If

            Debug.Assert(Not String.IsNullOrEmpty(sInputPath))
            Dim bFileGDB As Boolean = GISDataStructures.IsFileGeodatabase(sInputPath)

            Dim wsFact As IWorkspaceFactory2
            Dim sWorkspacePath As String
            Dim nLastSlash As Integer = sInputPath.LastIndexOf(IO.Path.DirectorySeparatorChar)
            Dim sOriginalDSName As String
            Dim sExtension As String = Nothing

            If bFileGDB Then
                wsFact = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.FileGeodatase)  'New FileGDBWorkspaceFactory

                Dim i As Integer = sInputPath.IndexOf(".gdb")
                sWorkspacePath = sInputPath.Substring(0, i + 4)
                sOriginalDSName = sInputPath.Substring(nLastSlash + 1)
            Else
                wsFact = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.RasterFile) ' New RasterWorkspaceFactory

                'Changed by as temp raster with root name was in parent folder of temp workspace - Frank
                Dim dFolder As New IO.DirectoryInfo(sInputPath)
                If dFolder.Exists Then
                    sWorkspacePath = sInputPath
                Else
                    sWorkspacePath = IO.Path.GetDirectoryName(sInputPath)

                End If
                If String.IsNullOrEmpty(sRootName) Then
                    sOriginalDSName = IO.Path.GetFileNameWithoutExtension(sInputPath)
                Else
                    sOriginalDSName = String.Empty
                End If
            End If

            Dim sOutputPath As String = ""

            If Not TypeOf wsFact Is IWorkspaceFactory2 Then
                Dim ex As New Exception("Invalid workspace factory")
                ex.Data("sInputPath") = sInputPath
                ex.Data("sRootName") = sRootName
                ex.Data("nMax") = nMax
                ex.Data("Workspace Path") = sWorkspacePath
                Throw ex
            End If

            Dim pWS As IWorkspace = wsFact.OpenFromFile(sWorkspacePath, 0)
            If Not TypeOf pWS Is IWorkspace Then
                Dim e As New Exception("Unable to open workspace")
                e.Data("sInputPath") = sInputPath
                e.Data("sRootName") = sRootName
                e.Data("nMax") = nMax
                Throw e
            End If

            Dim bContinue As Boolean
            Dim nCount As Integer = 0
            Dim sNewDSName As String


            Try

                Dim RootNameList As New List(Of String)

                Do
                    If String.IsNullOrEmpty(sRootName) Then
                        If String.IsNullOrEmpty(sOriginalDSName) Then
                            sNewDSName = "GCD"
                        Else
                            sNewDSName = sOriginalDSName
                        End If
                    Else
                        sNewDSName = sRootName
                    End If
                    FileSystem.RemoveDangerousCharacters(sNewDSName)

                    If nCount > 0 Then
                        sNewDSName &= nCount.ToString()
                    End If

                    If Not bFileGDB AndAlso String.IsNullOrEmpty(sExtension) Then
                        sExtension = GetRasterExtension(eType)
                    End If
                    sNewDSName = IO.Path.ChangeExtension(sNewDSName, sExtension)
                    '
                    ' Loop through the datasets in this workspace and
                    ' see if this name is already in use
                    '

                    'New method - FP Sep 8 2014
                    'http://help.arcgis.com/en/sdk/10.0/arcobjects_net/componenthelp/index.html#/d/002n000000vz000000.htm
                    If nCount = 0 Then

                        Dim GP As ESRI.ArcGIS.Geoprocessor.Geoprocessor = New ESRI.ArcGIS.Geoprocessor.Geoprocessor()
                        GP.SetEnvironmentValue("workspace", sWorkspacePath)
                        Dim fcs As ESRI.ArcGIS.Geoprocessing.IGpEnumList = GP.ListRasters(sRootName & "*", "")

                        Dim fc As String = fcs.Next()
                        While fc <> ""
                            RootNameList.Add(fc)
                            fc = fcs.Next()
                        End While
                    End If


                    If RootNameList.Contains(sNewDSName) Then
                        bContinue = True
                    Else
                        bContinue = False
                    End If
                    'End New method - FP Sep 8 2014

                    'Old method - FP Sep 8 2014

                    'enumRasters = pWS.DatasetNames(esriDatasetType.esriDTRasterDataset)
                    'pDSN = enumRasters.Next
                    'bContinue = False
                    'Do While TypeOf pDSN Is IDatasetName
                    '    If String.Compare(pDSN.Name, sNewDSName, True) = 0 Then
                    '        bContinue = True
                    '        Exit Do
                    '    End If
                    '    pDSN = enumRasters.Next
                    'Loop

                    'End Old method - FP Sep 8 2014


                    nCount += 1
                Loop While bContinue AndAlso nCount < 9999

                sOutputPath = IO.Path.Combine(sWorkspacePath, sNewDSName)

            Catch ex As Exception
                Dim e As New Exception("Unable find safe raster name.", ex)
                e.Data.Add("sInputPath", sInputPath)
                e.Data.Add("sRootName", sRootName)
                e.Data.Add("nMax", nMax)
                Throw e
            End Try

            Return sOutputPath

        End Function

        Public Shared Function GetNewSafeName(ByVal sWorkspace As String, ByVal sRootName As String) As String

            If GISDataStructures.IsFileGeodatabase(sWorkspace) Then
                Return GetNewSafeName(sWorkspace, RasterTypes.FileGeodatabase, sRootName)
            Else
                Return GetNewSafeName(sWorkspace, GetDefaultRasterType, sRootName)
            End If

        End Function

        Public Shared Function GetNewSafeName(ByVal dFileGDB As IO.DirectoryInfo, ByVal sNewRasterName As String) As String

            If Not TypeOf dFileGDB Is IO.DirectoryInfo Then
                Throw New ArgumentNullException("FileGDB", "Null or empty file GDB")
            End If

            If Not GISCode.GISDataStructures.IsFileGeodatabase(dFileGDB.FullName) Then
                Throw New ArgumentOutOfRangeException("FileGDB", "The argument does not appear to be a file geodatabase")
            End If

            If String.IsNullOrEmpty(sNewRasterName) Then
                Throw New ArgumentNullException("NewRasterName", "Null or empty raster name")
            End If
            sNewRasterName = GISCode.FileSystem.RemoveDangerousCharacters(sNewRasterName)

            Dim sPath As String
            Dim i As Integer = 0

            Do
                sPath = IO.Path.Combine(dFileGDB.FullName, sNewRasterName)
                If i > 0 Then
                    sPath &= i.ToString
                End If
                i += 1
            Loop While Exists(sPath) And i < 9999

            Return sPath

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sFullPath"></param>
        ''' <returns></returns>
        ''' <remarks>PGB 14 Nov 2012. 
        ''' Do not use IWorkspace2 here. The IWorkspace2 object is only allowed for geodatabases
        ''' and shapefile workspaces. It does not work with file system raster workspaces.</remarks>
        Public Shared Function Exists(ByVal sFullPath As String) As Boolean

            Dim bResult As Boolean = False
            Try
                Dim pWkSp As IWorkspace = GetRasterWorkspaceFromPath(sFullPath)
                If TypeOf pWkSp Is IWorkspace Then
                    Dim pEnum As IEnumDatasetName = pWkSp.DatasetNames(esriDatasetType.esriDTRasterDataset)
                    Dim pDS As IDatasetName = pEnum.Next
                    Do While TypeOf pDS Is IDatasetName
                        If String.Compare(pDS.Name, IO.Path.GetFileName(sFullPath), True) = 0 Then
                            bResult = True
                            Exit Do
                        End If
                        pDS = pEnum.Next
                    Loop
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pEnum)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pWkSp)
                End If


            Catch ex As Exception
                Dim ex2 As New Exception("Unable to open raster workspace from path", ex)
                ex2.Data.Add("sFullPath", sFullPath)
                Throw ex2
            End Try

            Return bResult

        End Function

        Public Shared Function GetRasterWorkspaceFromPath(ByVal sPath As String) As IWorkspace

            Dim pRasterWorkspace As IWorkspace = Nothing

            If String.IsNullOrEmpty(sPath) Then
                Throw New Exception("Null or empty feature workspace path")
            End If

            Dim workspaceFactory As IWorkspaceFactory2
            Dim sWorkspacepath As String = GetWorkspacePath(sPath)
            If GISDataStructures.IsFileGeodatabase(sPath) Then
                workspaceFactory = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.FileGeodatase) 'New FileGDBWorkspaceFactory
                If workspaceFactory.IsWorkspace(sWorkspacepath) Then
                    pRasterWorkspace = workspaceFactory.OpenFromFile(sWorkspacepath, 0)
                End If
            Else
                workspaceFactory = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.RasterFile) 'New RasterWorkspaceFactory
                If IO.Directory.Exists(sWorkspacepath) Then
                    pRasterWorkspace = workspaceFactory.OpenFromFile(sWorkspacepath, 0)
                End If
            End If

            Return pRasterWorkspace

        End Function

        ''' <summary>
        ''' Makes a safe copy of a raster (if needed) and then returns the path to the result.
        ''' </summary>
        ''' <param name="eDesiredType">Desired output raster format.</param>
        ''' <param name="bUseTempWorkspace">If true, then the copy is placed in the tempory worpskace folder. False and the output is put in the same folder as the input.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSafeCopy(ByVal eDesiredType As RasterTypes, Optional ByVal bUseTempWorkspace As Boolean = False) As String

            If eDesiredType = RasterTypes.FileGeodatabase Then
                Dim ex As New Exception("The file geodatabase is not considered a safe type and should not be used in Raster.GetSafeCopy()")
            End If

            Dim sOutputPath As String = Me.FullPath
            Dim bMakeACopy As Boolean = False
            Dim sFolder As String = WorkspaceManager.WorkspacePath
            If bUseTempWorkspace Then
                bMakeACopy = True
            ElseIf IsFileGeodatabase(Me.FullPath) Then
                bMakeACopy = True
            End If

            If bMakeACopy Then
                Try
                    If Not System.IO.Directory.Exists(sFolder) Then
                        Dim ex As New Exception("The directory does not exist.")
                        ex.Data("Directory") = sFolder
                        Throw ex
                    End If

                    sOutputPath = GetNewSafeName(sFolder, eDesiredType, IO.Path.GetFileNameWithoutExtension(Me.FullPath), 13)
                    GP.DataManagement.CopyRaster(Me.FullPath, sOutputPath)

                Catch ex As Exception
                    ex.Data("Original Raster") = Me.FullPath
                    ex.Data("Copy Path") = sOutputPath
                    Throw
                End Try
            End If

            Return sOutputPath

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sMapAlgebra"></param>
        ''' <returns></returns>
        ''' <remarks>IMPORTANT: Remember to set the environment when working with Map Algebra - it is different than other
        ''' Spatial Analyst tools since it does not invoke the GP object.</remarks>
        Public Function MapAlgebra(ByVal sMapAlgebra As String) As IRaster

            If String.IsNullOrEmpty(sMapAlgebra) Then
                Throw New ArgumentNullException("sMapAlgebra", "Invalid or empty map algebra string")
            End If

            Dim pMapAlgebraOp As IMapAlgebraOp = New RasterMapAlgebraOp
            Dim env As IRasterAnalysisEnvironment = pMapAlgebraOp
            'Dim workspaceFactory As IWorkspaceFactory = GISCode.ArcMap.GetWorkspaceFactory(Me.GISDataStorageType)  ' New RasterWorkspaceFactoryClass()
            'Dim workspace As IWorkspace = workspaceFactory.OpenFromFile(WorkspaceManager.WorkspacePath, 0)
            env.OutWorkspace = Me.Workspace ' workspace
            Dim rasOut As IRaster

            Try
                pMapAlgebraOp.BindRaster(Me.RasterDataset, "R1")
                rasOut = pMapAlgebraOp.Execute(sMapAlgebra)
            Catch ex As Exception
                ex.Data("Raster dataset") = Me.FullPath
                ex.Data("Map Algebra") = sMapAlgebra
                Throw
            End Try

            Return rasOut

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sMapAlgebra"></param>
        ''' <returns></returns>
        ''' <remarks>IMPORTANT: Remember to set the environment when working with Map Algebra - it is different than other
        ''' Spatial Analyst tools since it does not invoke the GP object.</remarks>
        Public Function MapAlgebra(ByVal sMapAlgebra As String, ByVal gRaster2 As GISDataStructures.Raster) As IRaster

            If String.IsNullOrEmpty(sMapAlgebra) Then
                Throw New ArgumentNullException("sMapAlgebra", "Invalid or empty map algebra string")
            End If

            If Not TypeOf gRaster2 Is GISDataStructures.Raster Then
                Throw New ArgumentNullException("R2", "Invalid or null second raster")
            End If

            Dim pMapAlgebraOp As IMapAlgebraOp = New RasterMapAlgebraOp
            Dim env As IRasterAnalysisEnvironment = pMapAlgebraOp
            'Dim workspaceFactory As IWorkspaceFactory = GISCode.ArcMap.GetWorkspaceFactory(Me.GISDataStorageType) ' New RasterWorkspaceFactoryClass()
            'Dim workspace As IWorkspace = workspaceFactory.OpenFromFile(WorkspaceManager.WorkspacePath, 0)
            env.OutWorkspace = Me.Workspace 'workspace
            Dim rasOut As IRaster

            Try
                pMapAlgebraOp.BindRaster(Me.RasterDataset, "R1")
                pMapAlgebraOp.BindRaster(gRaster2.RasterDataset, "R2")
                rasOut = pMapAlgebraOp.Execute(sMapAlgebra)
            Catch ex As Exception
                ex.Data("Raster dataset") = Me.FullPath
                ex.Data("Map Algebra") = sMapAlgebra
                Throw
            End Try

            Return rasOut

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sMapAlgebra"></param>
        ''' <returns></returns>
        ''' <remarks>IMPORTANT: Remember to set the environment when working with Map Algebra - it is different than other
        ''' Spatial Analyst tools since it does not invoke the GP object.</remarks>
        Public Function MapAlgebra(ByVal sMapAlgebra As String, ByVal pRasterDataset As IRasterDataset) As IRaster

            If String.IsNullOrEmpty(sMapAlgebra) Then
                Throw New ArgumentNullException("sMapAlgebra", "Invalid or empty map algebra string")
            End If

            If Not TypeOf pRasterDataset Is IRasterDataset Then
                Throw New ArgumentNullException("pRasterDataset", "Invalid or null second raster")
            End If

            Dim pMapAlgebraOp As IMapAlgebraOp = New RasterMapAlgebraOp
            Dim env As IRasterAnalysisEnvironment = pMapAlgebraOp
            'Dim workspaceFactory As IWorkspaceFactory = GISCode.ArcMap.GetWorkspaceFactory(Me.GISDataStorageType) 'New RasterWorkspaceFactoryClass()
            'Dim workspace As IWorkspace = workspaceFactory.OpenFromFile(WorkspaceManager.WorkspacePath, 0)
            env.OutWorkspace = Me.Workspace ' workspace
            Dim rasOut As IRaster

            Try
                pMapAlgebraOp.BindRaster(Me.RasterDataset, "R1")
                pMapAlgebraOp.BindRaster(pRasterDataset, "R2")
                rasOut = pMapAlgebraOp.Execute(sMapAlgebra)
            Catch ex As Exception
                ex.Data("Raster dataset") = Me.FullPath
                ex.Data("Map Algebra") = sMapAlgebra
                Throw
            End Try

            Return rasOut

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sMapAlgebra"></param>
        ''' <returns></returns>
        ''' <remarks>IMPORTANT: Remember to set the environment when working with Map Algebra - it is different than other
        ''' Spatial Analyst tools since it does not invoke the GP object.</remarks>
        Public Shared Function MapAlgebra(ByVal sMapAlgebra As String, ByVal pR1 As IRasterDataset, ByVal pR2 As IRasterDataset) As IRaster

            If String.IsNullOrEmpty(sMapAlgebra) Then
                Throw New ArgumentNullException("sMapAlgebra", "Invalid or empty map algebra string")
            End If

            If Not TypeOf pR1 Is IRasterDataset Then
                Throw New ArgumentNullException("pRasterDataset", "Invalid or null first raster")
            End If

            If Not TypeOf pR2 Is IRasterDataset Then
                Throw New ArgumentNullException("pRasterDataset", "Invalid or null second raster")
            End If

            Dim pMapAlgebraOp As IMapAlgebraOp = New RasterMapAlgebraOp
            Dim env As IRasterAnalysisEnvironment = pMapAlgebraOp
            Dim workspaceFactory As IWorkspaceFactory = GISCode.ArcMap.GetWorkspaceFactory(GISDataStorageTypes.RasterFile) 'New RasterWorkspaceFactoryClass()
            Dim workspace As IWorkspace = workspaceFactory.OpenFromFile(WorkspaceManager.WorkspacePath, 0)
            env.OutWorkspace = workspace
            Dim rasOut As IRaster

            Try
                pMapAlgebraOp.BindRaster(pR1, "R1")
                pMapAlgebraOp.BindRaster(pR2, "R2")
                rasOut = pMapAlgebraOp.Execute(sMapAlgebra)
            Catch ex As Exception
                ex.Data("Raster 1") = pR1.CompleteName
                ex.Data("Raster 2") = pR2.CompleteName
                ex.Data("Map Algebra") = sMapAlgebra
                Throw
            End Try

            Return rasOut

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pRaster"></param>
        ''' <param name="sOutFullPath"></param>
        ''' <param name="bReplace"></param>
        ''' <remarks>PGB - 15 Sep 2011 - This method needs further work. Relying on the creation of the gisData object to fail because
        ''' a raster doesn't exist is bad practice. Ideally the GetRasterDataset() method would loop through the workspace and not
        ''' attempt to open rasters that don't exist, and therefore return Nothing.
        ''' 
        '''PGB - 20 Sep 2011 - Upgrade this method. It used to rely on the creation of a new GISDataSource to fail, which is
        '''bad practice. Now loops through workspace looking at names to see if the raster already exists.</remarks>
        Public Shared Sub SaveRaster(ByVal pRaster As IRasterBandCollection, ByVal sOutFullPath As String, Optional ByVal bReplace As Boolean = False)

            If Not TypeOf pRaster Is IRasterBandCollection Then
                Throw New ArgumentException("Invalid raster band collection", "pRaster")
            End If

            'If Not IO.Directory.Exists(sFolder) Then
            '    Throw New ArgumentOutOfRangeException("sFolder", sFolder, "Intended raster output workspace does not exist")
            'End If

            If String.IsNullOrEmpty(sOutFullPath) Then
                Throw New ArgumentNullException("sOutFullPath", "Null or empty output raster name")
            End If

            'Dim pWS As IWorkspace
            Dim bExists As Boolean = False
            ' Dim pDS As IDatasetName


            If GISDataStructures.Raster.Exists(sOutFullPath) Then
                If bReplace Then
                    Try
                        GISCode.GP.DataManagement.Delete(sOutFullPath)
                    Catch ex As Exception
                        ex.Data("Output path") = sOutFullPath
                        Throw
                    End Try
                Else
                    Dim ex As New Exception("Cannot save raster because a raster already exists with the same name.")
                    ex.Data("Raster Band") = pRaster.GetType.ToString
                    ex.Data("Output path") = sOutFullPath
                    ex.Data("Replace") = bReplace.ToString
                    Throw ex
                End If
            End If

            Dim RasterType As String
            Dim extension As String = System.IO.Path.GetExtension(sOutFullPath)
            If String.IsNullOrEmpty(extension) Then
                RasterType = "GRID"
            Else
                Select Case extension
                    Case ".tif" : RasterType = "TIFF"
                    Case ".img" : RasterType = "IMAGINE Image"
                    Case Else
                        Throw New Exception("Raster.SaveRaster(): Invalid raster format extension (" & extension & ")")
                End Select

            End If

            Try
                Dim pOutputWorkspace As IRasterWorkspace = GISDataStructures.Raster.GetRasterWorkspaceFromPath(sOutFullPath)
                Dim prasterDS As IRasterDataset = pRaster.SaveAs(IO.Path.GetFileName(sOutFullPath), pOutputWorkspace, RasterType)
                If TypeOf prasterDS Is IRasterDataset Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(prasterDS)
                End If

            Catch ex As Exception
                ex.Data("Output path") = sOutFullPath
                ex.Data("Raster Type") = RasterType
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Gets the value corresponding to the highest count for a raster
        ''' </summary>
        ''' <param name="excludedValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaxCount(Optional ByVal excludedValue As Integer = -1) As Integer

            'If Not TypeOf pOutputRaster Is IGeoDataset Then
            '    Throw New Exception("Invalid output raster geodataset.")
            'End If

            Dim maxCount As Double = 0
            Dim maxValue As Integer = 0

            Try
                Dim nValue As Integer = Nothing
                Dim nCount As Integer = Nothing
                Dim Count As Double = Nothing
                Dim Value As Integer
                Dim pFeatureCursor As ICursor
                Dim pRow As IRow
                Dim pRaster2 As IRaster2

                'If TypeOf (pOutputRaster) Is IRasterDataset Then
                '    Dim pRasterDataset As IRasterDataset = pOutputRaster
                pRaster2 = RasterDataset.CreateDefaultRaster
                'Else
                'pRaster2 = pOutputRaster
                'End If

                Dim pTable As ITable
                pTable = pRaster2.AttributeTable

                nCount = pTable.FindField("COUNT")
                nValue = pTable.FindField("VALUE")

                pFeatureCursor = pTable.Search(Nothing, False)
                pRow = pFeatureCursor.NextRow

                While Not pRow Is Nothing
                    Count = pRow.Value(nCount)
                    Value = pRow.Value(nValue)
                    If Count > maxCount And Not Value = excludedValue Then
                        maxCount = Count
                        maxValue = Value
                    End If
                    pRow = pFeatureCursor.NextRow
                End While

            Catch ex As Exception
                Throw
            End Try

            Return maxValue

        End Function

        ' ''' <param name="pRaster"></param>
        ' ''' <param name="sFolder"></param>
        ' ''' <param name="sFilename"></param>
        ' ''' <param name="bReplace"></param>
        ' ''' <remarks></remarks>
        'Public Shared Sub SaveRaster(ByVal pRaster As IRasterBandCollection, ByVal sFolder As String, ByVal sFilename As String, Optional ByVal bReplace As Boolean = False)

        '    If Not TypeOf pRaster Is IRasterBandCollection Then
        '        Throw New ArgumentException("Invalid raster band collection", "pRaster")
        '    End If

        '    If Not IO.Directory.Exists(sFolder) Then
        '        Throw New ArgumentOutOfRangeException("sFolder", sFolder, "Intended raster output workspace does not exist")
        '    End If

        '    If String.IsNullOrEmpty(sFilename) Then
        '        Throw New ArgumentNullException("sFileName", "Null or empty raster name")
        '    End If

        '    Dim pWS As IWorkspace
        '    Dim bExists As Boolean = False
        '    Dim pDS As IDatasetName
        '    Try
        '        pWS = GISCode.GISDataStructures.Raster.GetRasterWorkspaceFromPath(IO.Path.Combine(sFolder, sFilename))
        '        Dim pEDS As IEnumDatasetName = pWS.DatasetNames(esriDatasetType.esriDTRasterDataset)
        '        pDS = pEDS.Next
        '        Do While TypeOf pDS Is IDatasetName
        '            If pDS.Name.ToLower = sFilename.ToLower Then
        '                bExists = True
        '                Exit Do
        '            End If
        '            pDS = pEDS.Next
        '        Loop

        '    Catch ex As Exception
        '        Dim ex2 As New Exception("Error opening raster workspace and looping over existing dataset names", ex)
        '        ex2.Data.Add("Raster Band", pRaster.GetType.ToString)
        '        ex2.Data.Add("Folder", sFolder)
        '        ex2.Data.Add("FileName", sFilename)
        '        ex2.Data.Add("Replace", bReplace.ToString)
        '        Throw ex2
        '    End Try

        '    If bExists Then
        '        If bReplace AndAlso TypeOf pDS Is IDatasetName Then
        '            Dim pExistingRaster As IDataset = GetRasterDataset(IO.Path.Combine(sFolder, sFilename))
        '            Try
        '                pExistingRaster.Delete()
        '            Catch ex As Exception
        '                ex.Data("Folder", sFolder)
        '                ex.Data("FileName", sFilename)
        '                Throw ex
        '            End Try
        '        Else
        '            Dim ex As New Exception("Cannot save raster because a raster already exists with the same name.")
        '            ex.Data("Raster Band", pRaster.GetType.ToString)
        '            ex.Data("Folder", sFolder)
        '            ex.Data("File Name", sFilename)
        '            ex.Data("Replace", bReplace.ToString)
        '            Throw ex
        '        End If
        '    End If

        '    Dim RasterType As String
        '    Dim extension As String = System.IO.Path.GetExtension(sFilename)
        '    If String.IsNullOrEmpty(extension) Then
        '        RasterType = "GRID"
        '    Else
        '        Select Case extension
        '            Case ".tif" : RasterType = "TIFF"
        '            Case ".img" : RasterType = "IMAGINE Image"
        '            Case Else
        '                Throw New Exception("Raster.SaveRaster(): Invalid raster format extension (" & extension & ")")
        '        End Select

        '    End If

        '    Try
        '        Dim prasterDS As IRasterDataset = pRaster.SaveAs(sFilename, pWS, RasterType)
        '        If TypeOf prasterDS Is IRasterDataset Then
        '            System.Runtime.InteropServices.Marshal.ReleaseComObject(prasterDS)
        '        End If

        '    Catch ex As Exception
        '        ex.Data("File Name", sFilename)
        '        ex.Data("Workspace", pWS.PathName)
        '        ex.Data("Raster Type", RasterType)
        '        Throw ex
        '    End Try
        'End Sub

        Public Function ApplySymbology(ByVal sSymobologyLayerFile As String) As IRasterLayer

            If String.IsNullOrEmpty(sSymobologyLayerFile) Then
                Throw New ArgumentNullException("Symbology Layer File", "Null or empty symbology layer file")
            End If

            If Not IO.File.Exists(sSymobologyLayerFile) Then
                Throw New ArgumentOutOfRangeException("SymbologyLayerFile", sSymobologyLayerFile, "The symbology layer file doesn't exist")
            End If
            '
            ' Create a layer for this data source. Get the default symbology for this raster type
            ' apply the symbology to the new layer and then add it to the map
            '
            Dim pRasterLayer As IRasterLayer = RasterLayer
            Dim pRR As IRasterRenderer = Nothing
            Dim pLayer As ILayer = Nothing
            Dim pGXLayer As IGxLayer = New GxLayerClass
            Dim pGXFile As IGxFile = pGXLayer
            '
            ' Load this layer file and retrieve the symbology from it.
            '
            pGXFile.Path = sSymobologyLayerFile
            Dim prlayer As IRasterLayer = pGXLayer.Layer
            pRR = prlayer.Renderer
            '
            ' Reset the renderer information. Remember, the renderer from the saved layer file
            ' may have been stretched to a different data range, so reset the labels.
            '
            ' For classified renderers, get the first legend group and reset the text label.
            ' Note: this was for some of the legends that Joe Wheaton provided, they had "T-Score"
            ' as the legend title.
            '
            If TypeOf pRR Is IRasterStretchColorRampRenderer Then
                Dim pStretch As IRasterStretchColorRampRenderer = pRR
                pStretch.ResetLabels()
            ElseIf TypeOf pRR Is ILegendInfo Then
                Dim pLegend As ILegendInfo = pRR
                pLegend.LegendGroup(0).Heading = String.Empty
            End If
            pRR.Update()
            '
            ' Now load the data source into a layer file and apply the symbology from above if one was created.
            '
            If TypeOf pRR Is IRasterRenderer Then
                pRasterLayer.Renderer = pRR
            End If

            Return pRasterLayer

        End Function


        ''' <summary>
        ''' Sets up raster analysis workspace
        ''' </summary>
        ''' <param name="WorkspacePath"></param>
        ''' <remarks>
        ''' Important to set up before performing any spatial analyst calculations
        ''' </remarks>
        Public Shared Sub SetupRasterAnalysisWorkspace(ByVal WorkspacePath As String)

            If String.IsNullOrEmpty(WorkspacePath) Then
                Throw New ArgumentNullException("WorkspacePath", "The workspace path is null or empty")
            Else
                Dim f As New IO.DirectoryInfo(WorkspacePath)
                If Not f.Exists Then
                    Try
                        IO.Directory.CreateDirectory(WorkspacePath)
                    Catch ex As Exception
                        ex.Data("Folder") = WorkspacePath
                        Throw
                    End Try
                End If
            End If

            Try
                Dim eWSFType As GISDataStorageTypes = GISDataStorageTypes.RasterFile
                If GISCode.GISDataStructures.IsFileGeodatabase(WorkspacePath) Then
                    eWSFType = GISDataStorageTypes.FileGeodatase
                End If
                ' Create a RasterAnalysis object.
                Dim rasterAnalysisEnvironment As IRasterAnalysisEnvironment = New RasterAnalysisClass
                Dim pWSF As IWorkspaceFactory = ArcMap.GetWorkspaceFactory(eWSFType)
                Dim pWS As IWorkspace = pWSF.OpenFromFile(WorkspacePath, 0)

                ' Set the new default output workspace.
                If Not pWS Is Nothing Then
                    rasterAnalysisEnvironment.OutWorkspace = pWS
                End If
                ' Set it as the default settings.
                rasterAnalysisEnvironment.SetAsNewDefaultEnvironment()
            Catch ex As Exception
                ex.Data("Workspace path") = WorkspacePath
                Throw
            End Try

        End Sub

        Public Shared Sub ConfirmExtension(ByRef sDatasetName As String, ByVal eType As GISDataStructures.Raster.RasterTypes)
            sDatasetName = IO.Path.ChangeExtension(sDatasetName, GetRasterExtension(eType, False))
        End Sub

        Public Shared Function GetDefaultRasterType() As GISDataStructures.Raster.RasterTypes

            Dim eResult As GISDataStructures.Raster.RasterTypes
            Dim sExt As String = My.Settings.DefaultRasterFormat.ToLower

            If String.IsNullOrEmpty(sExt) Then
                eResult = GISDataStructures.Raster.RasterTypes.TIFF
            Else
                If sExt.ToLower.Contains("image") Then
                    eResult = GISDataStructures.Raster.RasterTypes.IMG ' GISCode.Raster.RasterTypes.Image
                ElseIf sExt.ToLower.Contains("tif") Then
                    eResult = GISDataStructures.Raster.RasterTypes.TIFF 'GISCode.Raster.RasterTypes.TIFF
                Else
                    eResult = Nothing
                End If
            End If

            Return eResult

        End Function

        Public Shared Function CreateConstantRaster(ByVal fValue As Double, ByVal gExtentRaster As GISDataStructures.Raster, ByVal sOutputRaster As String) As GISDataStructures.Raster

            Dim pExtentRaster As IRasterDataset = gExtentRaster.RasterDataset
            Dim fCellSize As Double = gExtentRaster.CellSize

            Return GISCode.GP.SpatialAnalyst.CreateConstantRaster(fValue, sOutputRaster, DirectCast(pExtentRaster, IGeoDataset).Extent, fCellSize)

        End Function

        ''' <summary>
        ''' Creates a raster with the value 1 where there is data and NoData everywhere else
        ''' </summary>
        ''' <param name="sOutputRasterPath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateDataExtentRaster(Optional ByVal sOutputRasterPath As String = "") As GISDataStructures.Raster

            Dim gResult As GISDataStructures.Raster = Nothing
            Try
                If String.IsNullOrEmpty(sOutputRasterPath) Then
                    sOutputRasterPath = WorkspaceManager.GetTempRaster("RasEx")
                Else
                    If GISDataStructures.Raster.Exists(sOutputRasterPath) Then
                        Throw New Exception("The output path already exists")
                    End If
                End If

                GP.SpatialAnalyst.Con(Me.FullPath, "1", "", sOutputRasterPath, "")
                If GISDataStructures.Raster.Exists(sOutputRasterPath) Then
                    gResult = New GISDataStructures.Raster(sOutputRasterPath)
                Else
                    Throw New Exception("Generating data extent raster completed but the output does not exist.")
                End If
            Catch ex As Exception
                ex.Data("Original Raster") = Me.FullPath
                ex.Data("Output Path") = sOutputRasterPath
                Throw
            End Try

            Return gResult

        End Function

        Public Function CreateDataExtentPolygon(Optional ByVal sOutputPolygonPath As String = "", Optional ByVal bSimplify As Boolean = False) As GISDataStructures.PolygonDataSource

            Dim gRaster As GISDataStructures.Raster = CreateDataExtentRaster()

            Dim sSimplify As String = "NO_SIMPLIFY"
            If bSimplify Then
                sSimplify = "SIMPLIFY"
            End If

            If String.IsNullOrEmpty(sOutputPolygonPath) Then
                sOutputPolygonPath = WorkspaceManager.GetTempShapeFile("RasEx")
            End If

            Dim gResult As GISDataStructures.PolygonDataSource = Nothing
            Try
                GP.Conversion.RasterToPolygon_conversion(gRaster.FullPath, sOutputPolygonPath, sSimplify)
                If GISDataStructures.PolygonDataSource.Exists(sOutputPolygonPath, esriGeometryType.esriGeometryPolygon) Then
                    gResult = New GISDataStructures.PolygonDataSource(sOutputPolygonPath)
                    If gResult.FeatureCount < 1 Then
                        Throw New Exception("The raster extent conversion to polygon completed but the feature class is empty")

                    End If
                Else
                    Throw New Exception("The raster extent conversion to polygon completed but the output does not exist.")
                End If
            Catch ex As Exception
                ex.Data("Raster Path") = Me.FullPath
                ex.Data("Output Polygon") = sOutputPolygonPath
                Throw
            End Try

            Return gResult

        End Function

        ''' <summary>
        ''' Checks that a IRasterProps is one of the pixel types provided in the List
        ''' </summary>
        ''' <param name="pRasterProps">IRasterProp to evaluate against the List of pixel types</param>
        ''' <param name="lrstPixelType">List of ESRI.ArcGIS.Geodatabase.rstPixelType to check IRasterProp against</param>
        ''' <returns>Boolean: True if IRasterProp is one of the rstPixelTypes in the List, False if it is not one of the rstPixelTypes in the List</returns>
        ''' <remarks></remarks>
        Public Shared Function CheckPixelType(ByVal pRasterProps As ESRI.ArcGIS.DataSourcesRaster.IRasterProps, ByVal lrstPixelType As List(Of ESRI.ArcGIS.Geodatabase.rstPixelType)) As Boolean

            Dim bResult As Boolean = False

            'Check pixel depth
            If lrstPixelType.Count = 0 Then
                Throw New Exception("List of pixel types cannot be empty.")
            End If

            If Not TypeOf pRasterProps Is ESRI.ArcGIS.DataSourcesRaster.IRasterProps Then
                Throw New Exception("Object provided is not a ESRI.ArcGIS.DataSourcesRaster.IRasterProps.")
            End If

            For Each ePixelType As ESRI.ArcGIS.Geodatabase.rstPixelType In lrstPixelType

                If pRasterProps.PixelType = ePixelType Then
                    bResult = True
                End If

            Next

            Return bResult
        End Function

#End Region

    End Class

End Namespace
