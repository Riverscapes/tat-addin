#Region "Imports"

Imports System.Windows.Forms
Imports System.IO

Imports ESRI.ArcGIS.ArcMapUI
Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesFile
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.Catalog
Imports ESRI.ArcGIS.Geometry.esriGeometryType
Imports ESRI.ArcGIS.DataSourcesRaster
Imports System.Linq
'
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' PGB - 8 Jun 2012. Do not import any modules or classes that are within the GISCode 
' namespace. The parent project for this code file (GCD, RBT etc) should import the
' "GISCode" namespace, making all the child libraries visible without needing to specify
' the child namespace here. For example "GCD.GISCode.ArcMap" or' "RBT.GISCode.ArcMap".
' This makes the code portable between different products.
'
' In the methods below, reference any code stored in the GISCode folder simply using
' the GISCode namespace. e.g. GISCode.ArcMap.FeatureClas...
'
#End Region

Namespace GISCode.ArcMap

    Public Module ArcMap

        ''' <summary>
        ''' This is the string used as the group layer name for group layers representing GCD DEMs of Difference.
        ''' </summary>
        ''' <remarks>TODO: This should ideally be a string resource somewhere, but for now it is stored here as a constant.</remarks>
        Private Const sDoDGroupLayerName As String = "DoD"

#Region "Enumerations"

        ''' <summary>
        ''' Enumeration representing different types of layers that can be loaded into the ArcMap Table of Contents
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum eEsriLayerType
            Esri_DataLayer '{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}
            Esri_GeoFeatureLayer '{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}
            Esri_GraphicsLayer '{34B2EF81-F4AC-11D1-A245-080009B6F22B}
            Esri_FDOGraphicsLayer '{5CEAE408-4C0A-437F-9DB3-054D83919850}
            Esri_CoverageAnnotationLayer '{0C22A4C7-DAFD-11D2-9F46-00C04F6BC78E}
            Esri_GroupLayer '{EDAD6644-1810-11D1-86AE-0000F8751720}
            Esri_AnyLayer
        End Enum

        ''' <summary>
        ''' Enumeration representing the different options for polyline shapefiles - i.e. whether to restrict a list to all
        ''' polyline shapefiles, or just those with cross sections or exclude cross sections.
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum eShapeFileStore
            OnlyIncludeXStores
            ExcludeXSStores
            AllFeatureClasses
        End Enum

        ''' <summary>
        ''' Enumeration between common raster types that can be added to the map. 
        ''' </summary>
        ''' <remarks>Note that DEM refers to both DEM surveys, and the DEMs that are
        ''' produced by the PCtoDEM tool. Also note that DoD are not included here 
        ''' because they have special inputs and have their own AddtoMap function.
        ''' </remarks>
        Public Enum GCDRasterType
            DEM
            Surface
            ErrorCalc
            WaterSurface
        End Enum

        ''' <summary>
        ''' GCD raster types that can be displayed in the ArcMap table of contents
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum RasterLayerTypes
            SlopePercentRise
            SlopeDegrees
            PointDensity
            DoD
            DEM
            ErrorSurfaces
            PointQuality
            Roughness
            InterpolationError
            GrainSizeStatistic
            'Added 12/9/2013 Hensleigh - Added because SlopePercentRise symbology was being added when type was undefined
            Undefined
        End Enum

        Public Enum eDirectionType
            Upstream
            Downstream
            Both
        End Enum

        Public Enum eLimitType
            Distance
            Number
        End Enum

#End Region

        Public Function GetWorkspaceType(ByVal sPath As String) As GISDataStructures.GISDataStorageTypes

            If sPath.ToLower.Contains(".gdb") Then
                Return GISDataStructures.GISDataStorageTypes.FileGeodatase
            ElseIf sPath.ToLower.Contains(".dxf") Then
                Return GISDataStructures.GISDataStorageTypes.CAD
            ElseIf sPath.ToLower.Contains(".tif") Then
                Return GISDataStructures.GISDataStorageTypes.RasterFile
            ElseIf sPath.ToLower.Contains(".img") Then
                Return GISDataStructures.GISDataStorageTypes.RasterFile
            Else
                Return GISDataStructures.GISDataStorageTypes.ShapeFile
            End If

        End Function

        ''' <summary>
        ''' Create a singleton for a workspace factory
        ''' </summary>
        ''' <param name="eGISStorageType"></param>
        ''' <returns></returns>
        ''' <remarks>PGB 28 Aug 2013 - Note that this is the only correct method for creating a workspace factory.
        ''' Do not call "New" to create this singleton classes.
        ''' <![CDATA[<see href="http://forums.esri.com/Thread.asp?c=93&f=993&t=178686"></see>]]> 
        ''' </remarks>
        Public Function GetWorkspaceFactory(ByVal eGISStorageType As GISDataStructures.GISDataStorageTypes) As IWorkspaceFactory

            Dim aType As Type = Nothing
            Dim pWSFact As IWorkspaceFactory = Nothing

            Try
                Select Case eGISStorageType
                    Case GISDataStructures.GISDataStorageTypes.RasterFile
                        aType = Type.GetTypeFromProgID("esriDataSourcesRaster.RasterWorkspaceFactory")
                    Case GISDataStructures.GISDataStorageTypes.ShapeFile
                        aType = Type.GetTypeFromProgID("esriDataSourcesFile.ShapefileWorkspaceFactory")
                    Case GISDataStructures.GISDataStorageTypes.FileGeodatase
                        aType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory")
                    Case GISDataStructures.GISDataStorageTypes.CAD
                        aType = Type.GetTypeFromProgID("esriDataSourcesFile.CadWorkspaceFactory")
                    Case GISDataStructures.GISDataStorageTypes.PersonalGeodatabase
                        aType = Type.GetTypeFromProgID("esriDataSourcesGDB.AccessWorkspaceFactory")
                    Case GISDataStructures.GISDataStorageTypes.TIN
                        aType = Type.GetTypeFromProgID("esriDataSourcesFile.TinWorkspaceFactory")
                    Case Else
                        Throw New Exception("Unhandled GIS storage type")
                End Select

                Dim obj As System.Object = Activator.CreateInstance(aType)
                pWSFact = obj
            Catch ex As Exception
                ex.Data("Workspace Type") = eGISStorageType.ToString
                Throw
            End Try

            Return pWSFact

        End Function

        ''' <summary>
        ''' Use this class when you want a window to be a child of ArcMap.
        ''' </summary>
        ''' <remarks>You can use this class when you want a child window (such as a modal popup) to
        ''' be owned by ArcMap. In the method where you show the child window, you can use this class
        ''' as follows:
        ''' 
        ''' myFrm.ShowDialog(New ArcMapWindow(m_pArcMap))
        '''
        ''' This will ensure that ArcMap can get a handle to the child form.
        ''' </remarks>
        Public Class ArcMapWindow
            Implements System.Windows.Forms.IWin32Window
            Private m_pArcMap As IApplication

            Public Sub New(ByVal pArcMap As IApplication)
                m_pArcMap = pArcMap
            End Sub

            Public ReadOnly Property Handle() As System.IntPtr Implements System.Windows.Forms.IWin32Window.Handle
                Get
                    Return New IntPtr(m_pArcMap.hWnd)
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Checks if ArcInfo license is available
        ''' </summary>
        ''' <returns>True if ArcInfo license is available, otherwise false</returns>
        ''' <remarks></remarks>
        Public Function ArcInfoIsAvailable() As Boolean

            Dim m_pAoInitialize As IAoInitialize = New AoInitialize
            Dim InitializedProduct As esriLicenseProductCode = m_pAoInitialize.InitializedProduct()
            Return InitializedProduct = esriLicenseProductCode.esriLicenseProductCodeAdvanced

        End Function

        ''' <summary>
        ''' Get the containing folder where the data for a layer are stored.
        ''' </summary>
        ''' <param name="pLayer"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFolder(ByRef pLayer As ILayer) As String

            If TypeOf pLayer Is ILayer Then
                Dim pDS As IDataset = pLayer
                Return pDS.Workspace.PathName
            Else
                Return String.Empty
            End If

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pArcMap"></param>
        ''' <param name="aCombo"></param>
        ''' <param name="bSelectSingleton"></param>
        ''' <remarks>PGB - 13 Jan 2015. Ommit ASCII rasters</remarks>
        Public Sub FillComboFromMapRaster(ByRef pArcMap As IApplication, ByRef aCombo As ComboBox, _
             Optional ByVal bSelectSingleton As Boolean = True)

            Debug.Assert(TypeOf pArcMap Is IApplication)
            Debug.Assert(TypeOf aCombo Is ComboBox)

            aCombo.Items.Clear()

            Dim cItem As GISDataStructures.GISDataSource
            Dim mxMap As IMxDocument = pArcMap.Document
            Dim pMap As IMap = mxMap.FocusMap
            Dim pEnumLayers As IEnumLayer
            Dim pUID As IUID = New UID
            pUID.Value = "{D02371C7-35F7-11D2-B1F2-00C04F8EDEFF}" ' Raster Layer

            pEnumLayers = pMap.Layers(pUID, True)
            If TypeOf pEnumLayers Is IEnumLayer Then
                Dim pLayer As ILayer = pEnumLayers.Next
                Do While TypeOf pLayer Is IRasterLayer
                    Dim pRL As IRasterLayer = pLayer

                    ' Prevent ASCII rasters and GRIDs from getting into the dropdown
                    Dim sExt As String = System.IO.Path.GetExtension(pRL.FilePath)
                    If (Not String.IsNullOrEmpty(sExt) AndAlso String.Compare(sExt, ".asc", True) = 0) Then
                        'PGB 13 Jan 2015 - ASCII raster. skip it!
                    ElseIf System.IO.Directory.Exists(pRL.FilePath) Then
                        ' PGB 13 Jan 2014 - GRID. Skip it!
                    Else
                        ' File based or File GDB raster
                        Try
                            cItem = New GISDataStructures.Raster(pRL.FilePath, False) ' GISDataSource(pLayer)
                            If Not String.IsNullOrEmpty(cItem.FullPath) Then
                                aCombo.Items.Add(cItem)
                            End If
                        Catch ex As Exception
                            ' Do nothing. This layer will not get added to the combo box.
                        End Try
                    End If
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pLayer)
                    pLayer = pEnumLayers.Next
                Loop
            End If

            If bSelectSingleton AndAlso aCombo.Items.Count = 1 Then
                aCombo.SelectedIndex = 0
            End If

        End Sub

        ''' <summary>
        ''' Generic utiltity function for filling a combobox with the features layers currently loaded in the open map document in ArcView. 
        ''' </summary>
        ''' <param name="aCombo">The combobox to be filled with items</param>
        ''' <param name="pArcMap">The ArcMap application</param>
        ''' <param name="eGeometryType">Point, Polyline, Polygon or Any</param>
        ''' <param name="bSelectSingleton">If only one item is added to the combobox, then automatically make it the selected item</param>
        ''' <param name="eXSStores">Filter by cross section stores if needed. Ignored otherwise.</param>
        ''' <remarks>You can specify the type of map layers 
        ''' (e.g. polygon) or leave blank to use esriGeometryType.esriGeometryAny and get all layers.</remarks>
        Public Sub FillComboFromMap(ByRef aCombo As ComboBox, _
                                         ByRef pArcMap As IApplication, _
                                         Optional ByVal eGeometryType As esriGeometryType = esriGeometryAny, _
                                         Optional ByVal bSelectSingleton As Boolean = True, _
                                         Optional ByVal eXSStores As eShapeFileStore = eShapeFileStore.AllFeatureClasses)


            Debug.Assert(TypeOf pArcMap Is IApplication)

            Dim mxMap As IMxDocument = pArcMap.Document
            Dim pMap As IMap = mxMap.FocusMap

            Dim i As Long
            Dim pFLayer As IFeatureLayer
            Dim cItem As GISCode.GISDataStructures.GISDataSource
            Dim bInclude As Boolean = True

            aCombo.Text = ""
            aCombo.Items.Clear()
            '
            ' Loop through all layers in the map
            '
            For i = 0 To pMap.LayerCount - 1
                If TypeOf pMap.Layer(i) Is IFeatureLayer Then
                    pFLayer = pMap.Layer(i)
                    '
                    ' PGB 29 May 2012. Only proceed if the feature class has a feature ID. This should
                    ' exclude event themes.
                    '
                    If TypeOf pFLayer.FeatureClass Is IFeatureClass Then

                        Dim pDSName As IDatasetName = DirectCast(pFLayer, IDataLayer).DataSourceName
                        Dim pFCName As IFeatureClassName = pDSName

                        Dim sPath As String = pDSName.WorkspaceName.PathName
                        If Not pFCName.FeatureDatasetName Is Nothing Then
                            sPath = IO.Path.Combine(sPath, pFCName.FeatureDatasetName.Name)
                        End If
                        sPath = IO.Path.Combine(sPath, pDSName.Name)
                        '
                        ' PGB 28 Aug 2013. Missing file extension on the end of the path
                        '
                        Dim pwsN As IWorkspaceName = pDSName.WorkspaceName
                        Dim pWSF As IWorkspaceFactory = pwsN.WorkspaceFactory
                        If pWSF.WorkspaceType = esriWorkspaceType.esriFileSystemWorkspace Then
                            sPath = IO.Path.ChangeExtension(sPath, "shp")
                        End If

                        Dim gLayer As GISDataStructures.VectorDataSource
                        Select Case pFLayer.FeatureClass.ShapeType
                            Case esriGeometryPoint, esriGeometryMultipoint
                                gLayer = New GISDataStructures.PointDataSource(sPath)

                            Case esriGeometryLine, esriGeometryPolyline
                                gLayer = New GISDataStructures.PolylineDataSource(sPath)

                            Case esriGeometryPolygon
                                gLayer = New GISDataStructures.PolygonDataSource(sPath)

                            Case Else
                                Dim ex As New Exception("Unrecognized shape type")
                                ex.Data("Feature class") = sPath
                                Throw ex
                        End Select
                        '
                        ' Only proceed and add the layer if the geometry is of the desired type. Note that
                        ' we consider point and multipoint as the same thing, similar for line and polyline.
                        '
                        If TypeOf gLayer Is GISDataStructures.VectorDataSource Then

                            Dim bShapeTypeMatch As Boolean = (eGeometryType = esriGeometryAny)
                            If Not bShapeTypeMatch Then

                                If (eGeometryType = esriGeometryPoint OrElse eGeometryType = esriGeometryMultipoint) AndAlso _
                                    (pFLayer.FeatureClass.ShapeType = esriGeometryPoint OrElse pFLayer.FeatureClass.ShapeType = esriGeometryMultipoint) Then

                                    bShapeTypeMatch = True

                                ElseIf (eGeometryType = esriGeometryLine OrElse eGeometryType = esriGeometryPolyline) AndAlso _
                                (pFLayer.FeatureClass.ShapeType = esriGeometryLine OrElse pFLayer.FeatureClass.ShapeType = esriGeometryPolyline) Then

                                    bShapeTypeMatch = True

                                ElseIf eGeometryType = esriGeometryPolygon AndAlso pFLayer.FeatureClass.ShapeType = esriGeometryPolygon Then

                                    bShapeTypeMatch = True

                                End If

                                If bShapeTypeMatch Then
                                    '
                                    ' PGB 29 May 2012 - this tool can work with feature classes in any projected coordinate system
                                    '
                                    'If FeatureClassIsFeatureClassInMetres(pFLayer.FeatureClass) Then

                                    Select Case eXSStores
                                        Case eShapeFileStore.OnlyIncludeXStores
                                            bInclude = gLayer.IsCrossSectionStore ' GISCode.FeatureClass.IsCrossSectionStore(pFLayer.FeatureClass)
                                        Case eShapeFileStore.ExcludeXSStores
                                            bInclude = Not gLayer.IsCrossSectionStore ' GISCode.FeatureClass.IsCrossSectionStore(pFLayer.FeatureClass)
                                        Case Else
                                            bInclude = True
                                    End Select

                                    If bInclude Then
                                        cItem = gLayer ' New GISDataSource(pFLayer)
                                        aCombo.Items.Add(gLayer)
                                    End If
                                    'End If
                                End If
                            End If
                        End If
                    End If
                End If
            Next

            If bSelectSingleton AndAlso aCombo.Items.Count = 1 Then
                aCombo.SelectedIndex = 0
            End If

        End Sub

        ''' <summary>
        ''' Generic utiltity function for filling a combobox with the features layers currently loaded in the open map document in ArcView. 
        ''' </summary>
        ''' <param name="aCombo">The combobox to be filled with items</param>
        ''' <param name="pArcMap">The ArcMap application</param>
        ''' <param name="eGeometryType">Point, Polyline, Polygon or Any</param>
        ''' <remarks>You can specify the type of map layers 
        ''' (e.g. polygon) or leave blank to use esriGeometryType.esriGeometryAny and get all layers.
        ''' PGB 17 May 2014. New method that uses UIDs to get all layers recursively</remarks>
        Public Sub FillComboFromMap(ByRef aCombo As ComboBox, eGeometryType As GISCode.GISDataStructures.GeometryTypes, pArcMap As IApplication)

            Dim pUID As IUID = New UID
            pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}" ' UID for Geo Feature Layers

            Dim mxMap As IMxDocument = pArcMap.Document
            Dim pMap As IMap = mxMap.FocusMap
            Dim pLayers As IEnumLayer = pMap.Layers(pUID, True)
            Dim pL As ILayer = pLayers.Next
            Do While TypeOf pL Is ILayer

                If TypeOf pL Is IFeatureLayer Then
                    Dim pFL As IFeatureLayer = pL
                    Dim bShapeMatch As Boolean = False

                    Select Case pFL.FeatureClass.ShapeType
                        Case esriGeometryPoint, esriGeometryMultipoint
                            If eGeometryType = GISDataStructures.GeometryTypes.Point Then
                                bShapeMatch = True
                            End If

                        Case esriGeometryLine, esriGeometryPolyline
                            If eGeometryType = GISDataStructures.GeometryTypes.Line Then
                                bShapeMatch = True
                            End If

                        Case esriGeometryPolygon
                            If eGeometryType = GISDataStructures.GeometryTypes.Polygon Then
                                bShapeMatch = True
                            End If
                    End Select

                    If bShapeMatch Then
                        Dim sPath As String = GetPathFromFeatureLayer(pFL)
                        If Not String.IsNullOrEmpty(sPath) Then

                            Dim gItem As GISDataStructures.VectorDataSource = Nothing
                            Select Case eGeometryType
                                Case GISDataStructures.GeometryTypes.Point
                                    gItem = New GISDataStructures.PointDataSource(sPath)

                                Case GISDataStructures.GeometryTypes.Line
                                    gItem = New GISDataStructures.PolylineDataSource(sPath)

                                Case GISDataStructures.GeometryTypes.Polygon
                                    gItem = New GISDataStructures.PolygonDataSource(sPath)
                            End Select
                            aCombo.Items.Add(gItem)
                        End If
                    End If
                End If
                pL = pLayers.Next
            Loop

        End Sub

        Public Function GetFeatureLayerBySource(pArcMap As IApplication, sFullPath As String) As IFeatureLayer

            Dim pUID As IUID = New UID
            pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}" ' UID for Geo Feature Layers

            Dim mxMap As IMxDocument = pArcMap.Document
            Dim pMap As IMap = mxMap.FocusMap
            Dim pLayers As IEnumLayer = pMap.Layers(pUID, True)
            Dim pL As ILayer = pLayers.Next
            Do While TypeOf pL Is ILayer

                If TypeOf pL Is IFeatureLayer Then
                    Dim pFL As IFeatureLayer = pL
                    Dim sLayerPath As String = GetPathFromFeatureLayer(pFL)
                    If String.Compare(sLayerPath, sFullPath, True) = 0 Then
                        Return pFL
                    End If
                End If
                pL = pLayers.Next
            Loop
            Return Nothing

        End Function

        Public Function GetPathFromFeatureLayer(pFL As IFeatureLayer) As String

            'check if featureclass is nothing (this can happen if the underlying file has been deleted but the layer is still in the TOC - FP Sep 10 2014
            If pFL.FeatureClass Is Nothing Then
                Return String.Empty
            End If
            Dim sPath As String = DirectCast(pFL.FeatureClass, IDataset).Workspace.PathName
            If TypeOf pFL.FeatureClass.FeatureDataset Is IFeatureDataset Then
                sPath = IO.Path.Combine(sPath, pFL.FeatureClass.FeatureDataset.Name)
            End If
            sPath = IO.Path.Combine(sPath, DirectCast(pFL.FeatureClass, IDataset).Name)

            If DirectCast(pFL.FeatureClass, IDataset).Workspace.Type = esriWorkspaceType.esriFileSystemWorkspace Then
                sPath = IO.Path.ChangeExtension(sPath, "shp")
            End If
            Return sPath
        End Function

        ''' <summary>
        ''' Code taken from EDN on Jul 10 2007. Retrieves all layers from the current focus map that
        ''' have the type specified by eType. Note that this code was enhanced from the copy taken off
        ''' the internet. The method pMap.Layers() throws an exception when there are no layers in the map.
        ''' </summary>
        ''' <param name="sLayerName">Name of the layer</param>
        ''' <param name="pArcMap">ArcMap</param>
        ''' <param name="eType">Optional constraint to look for a layer of a certain type. Pass Nothing to look for any type.</param>
        ''' <returns>ILayer if found, otherwise nothing</returns>
        ''' <remarks>PGB - 27 - Jul 2007 - For some reason, the Layers() call throws an exception when it is called for
        ''' a group layer and there are feature layers in the legend, but not group layers. It is
        ''' commented out for now.</remarks>
        Public Function GetLayerByName(ByVal sLayerName As String, ByRef pArcMap As IApplication, Optional ByVal eType As eEsriLayerType = Nothing) As ILayer

            If String.IsNullOrEmpty(sLayerName) Then
                Return Nothing
            End If
            Debug.Assert(TypeOf pArcMap Is IApplication)

            Dim mxMap As IMxDocument = pArcMap.Document
            Dim pMap As IMap = mxMap.FocusMap

            Dim pEnumLayer As IEnumLayer
            Dim pLayer As ILayer
            Dim pID As IUID = New UIDClass

            If pMap.LayerCount < 1 Then
                Return Nothing
            End If

            Select Case eType
                Case eEsriLayerType.Esri_DataLayer
                    pID.Value = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}"
                Case eEsriLayerType.Esri_GeoFeatureLayer
                    pID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"
                Case eEsriLayerType.Esri_GraphicsLayer
                    pID.Value = "{34B2EF81-F4AC-11D1-A245-080009B6F22B}"
                Case eEsriLayerType.Esri_FDOGraphicsLayer
                    pID.Value = "{5CEAE408-4C0A-437F-9DB3-054D83919850}"
                Case eEsriLayerType.Esri_CoverageAnnotationLayer
                    pID.Value = "{0C22A4C7-DAFD-11D2-9F46-00C04F6BC78E}"
                Case eEsriLayerType.Esri_GroupLayer
                    pID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}"
                Case Else
                    pID = Nothing
            End Select
            pEnumLayer = pMap.Layers(pID, True)

            pEnumLayer.Reset()
            pLayer = pEnumLayer.Next
            Do While Not pLayer Is Nothing
                If LCase(pLayer.Name) = LCase(sLayerName) Then
                    Return pLayer
                End If
                pLayer = pEnumLayer.Next
            Loop

            Return Nothing

        End Function



        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pArcMap"></param>
        ''' <param name="sFolder"></param>
        ''' <param name="sFileName"></param>
        ''' <returns></returns>
        ''' <remarks>TODO: Check that this method works recursively. 
        ''' TODO: Improve this method to work with Raster data sources to</remarks>
        Public Function GetLayerBySource(ByVal pArcMap As IApplication, ByVal sFolder As String, ByVal sFileName As String) As IFeatureLayer

            Debug.Assert(TypeOf pArcMap Is IApplication)
            Debug.Assert(Not String.IsNullOrEmpty(sFolder))
            Debug.Assert(Not String.IsNullOrEmpty(sFileName))

            Dim mxMap As IMxDocument = pArcMap.Document
            Dim pMap As IMap = mxMap.FocusMap

            Try
                Dim FileNameWithoutExtension As String = System.IO.Path.GetFileNameWithoutExtension(sFileName)
                For i As Integer = 0 To pMap.LayerCount - 1
                    Dim player As ILayer = pMap.Layer(i)
                    If Not TypeOf (player) Is IGroupLayer Then




                        Dim pDS As IDataset = player
                        Try

                            If LCase(pDS.BrowseName) = LCase(FileNameWithoutExtension) AndAlso LCase(pDS.Workspace.PathName) = LCase(sFolder) Then
                                Return pDS
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                Next
                Return Nothing
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                Debug.WriteLine(ex.StackTrace)
                Return Nothing
            End Try
        End Function

        Public Function GetLayerBySource(ByVal pArcMap As IApplication, ByVal sFullPath As String) As ILayer

            Dim pResult As ILayer = Nothing
            If TypeOf pArcMap Is IApplication Then
                If Not String.IsNullOrEmpty(sFullPath) Then

                    Dim mxMap As IMxDocument = pArcMap.Document
                    Dim pMap As IMap = mxMap.FocusMap
                    Dim pLayer As ILayer = Nothing

                    Try
                        For i As Integer = 0 To pMap.LayerCount - 1
                            pLayer = pMap.Layer(i)
                            Dim sLayerPath As String = String.Empty

                            If TypeOf pLayer Is IGeoFeatureLayer Then
                                sLayerPath = GetPathFromFeatureLayer(pLayer)
                            ElseIf TypeOf pLayer Is IRasterLayer Then
                                sLayerPath = DirectCast(pLayer, IRasterLayer).FilePath
                            End If

                            If String.Compare(sLayerPath, sFullPath, True) = 0 Then
                                pResult = pLayer
                                Exit For
                            End If
                        Next
                    Catch ex As Exception
                        Dim ex2 As New Exception("Error getting layer by source", ex)
                        ex2.Data.Add("Path", sFullPath)
                        If TypeOf pLayer Is ILayer Then
                            ex2.Data.Add("Layer", pLayer.Name)
                        End If
                        Throw ex2
                    End Try
                End If
            End If

            Return pResult

        End Function

        Public Function IsLayerInMap(ByVal pArcMap As IApplication, ByVal Folder As String, ByVal FileName As String) As Boolean

            Try
                Return GetLayerBySource(pArcMap, Folder, FileName) IsNot Nothing

            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                Debug.WriteLine(ex.StackTrace)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Removes a layer from the ArcMap Table of Contents
        ''' </summary>
        ''' <param name="pArcMap">ArcMap application</param>
        ''' <param name="pLayer">The layer to be removed</param>
        ''' <remarks></remarks>
        Public Sub RemoveLayer(ByRef pArcMap As IApplication, ByVal pLayer As ILayer)

            If TypeOf pArcMap Is IApplication AndAlso TypeOf pLayer Is ILayer Then
                Dim pMXDoc As IMxDocument = pArcMap.Document
                Dim pMap As IMap = pMXDoc.FocusMap
                pMap.DeleteLayer(pLayer)
            End If

        End Sub

        ''' <summary>
        ''' Remove a layer from the ArcMap table of contents
        ''' </summary>
        ''' <param name="pArcMap"></param>
        ''' <param name="sFullPath">Full path to the dataset to remove</param>
        ''' <remarks>Overloaded to allow for removing by path</remarks>
        Public Sub RemoveLayer(ByVal pArcMap As IApplication, ByVal sFullPath As String)

            Dim pLayer As ILayer = GISCode.ArcMap.GetLayerBySource(pArcMap, sFullPath)
            If TypeOf pLayer Is ILayer Then
                RemoveLayer(pArcMap, pLayer)
            End If

        End Sub

        ' ''' <summary>
        ' ''' Determines if a file system path represents an ESRI File Geodatabase
        ' ''' </summary>
        ' ''' <param name="sPath">Full file system path. e.g. D:\Data\MyData.gdb/slope or D:Data\MySlope.shp</param>
        ' ''' <returns>True if the path represents a file geodatabase</returns>
        ' ''' <remarks></remarks>
        'Public Function IsPathAFileGeodatabase(sPath As String) As Boolean

        '    If String.IsNullOrEmpty(sPath) Then
        '        Return False
        '    Else
        '        Return sPath.ToLower.Contains(".gdb") ' & IO.Path.DirectorySeparatorChar)
        '    End If

        'End Function

        Public Function GetArcMapDisplayUnits(ByVal pArcMap As IApplication) As esriUnits

            If TypeOf pArcMap.Document Is IMxDocument Then
                Dim pMxDocument As IMxDocument = pArcMap.Document
                If TypeOf pMxDocument.FocusMap Is IMap Then
                    Dim pMap As IMap = pMxDocument.FocusMap
                    Return pMap.MapUnits
                End If
            End If

            Return Nothing

        End Function

        ''' <summary>
        ''' Gets the display units of the current ArcMap map document as a string, wrapping parentheses if requested
        ''' </summary>
        ''' <param name="pArcMap">ArcMap application</param>
        ''' <param name="bAddParentheses">If true, then parentheses "()" are wrapped around the output units string. i.e. "(m)"</param>
        ''' <returns>String representation of the ArcMap display units. e.g. "ft" for feet.</returns>
        ''' <remarks></remarks>
        Public Function GetArcMapDisplayUnitsAsString(ByVal pArcMap As IApplication, Optional ByVal bAddParentheses As Boolean = False) As String

            Dim sResult As String = String.Empty
            Dim DisplayUnit As esriUnits = GetArcMapDisplayUnits(pArcMap)
            If Not DisplayUnit = Nothing Then
                sResult = GetUnitsAsString(DisplayUnit)
                If Not String.IsNullOrEmpty(sResult) AndAlso bAddParentheses Then
                    sResult = "(" & sResult & ")"
                End If
            End If

            Return sResult

        End Function

        Public Function GetUnitsAsString(ByVal eUnit As esriUnits, Optional ByVal bParentheses As Boolean = False, Optional ByVal nPower As Short = 0) As String

            If eUnit = Nothing Then
                Return String.Empty
            End If

            If nPower < 0 OrElse nPower > 3 Then
                Dim ex As New Exception("Invalid power number for units display")
                ex.Data("Power") = nPower
                Throw ex
            End If

            Dim sResult As String = ""
            Select Case eUnit
                Case esriUnits.esriCentimeters
                    sResult = "cm"
                Case esriUnits.esriDecimalDegrees
                    sResult = "degrees"
                Case esriUnits.esriDecimeters
                    sResult = "dm"
                Case esriUnits.esriFeet
                    sResult = "ft"
                Case esriUnits.esriInches
                    sResult = "in"
                Case esriUnits.esriKilometers
                    sResult = "km"
                Case esriUnits.esriMeters
                    sResult = "m"
                Case esriUnits.esriMiles
                    sResult = "mi"
                Case esriUnits.esriMillimeters
                    sResult = "mm"
                Case esriUnits.esriNauticalMiles
                    sResult = "nm"
                Case esriUnits.esriPoints
                    sResult = "pt"
                Case esriUnits.esriYards
                    sResult = "yd"
                Case esriUnits.esriUnknownUnits
                    sResult = String.Empty '"unknown"
                Case Else
                    sResult = String.Empty
            End Select

            If Not String.IsNullOrEmpty(sResult) Then

                Select Case nPower
                    Case 2
                        sResult &= "²"

                    Case 3
                        sResult &= "³"
                End Select

                If bParentheses Then
                    sResult = "(" & sResult & ")"
                End If
            End If

            Return sResult

        End Function

        Public Function IsLinearUnitMetric(ByVal eLinearUnit As ILinearUnit) As Boolean

            Dim bResult As Boolean
            Select Case eLinearUnit.Name
                Case "Meter"
                    bResult = True

                Case "Feet", "Foot", "Foot_US"
                    bResult = False

                Case Else
                    Dim ex As New Exception("Unhandled linear units")
                    ex.Data("Units") = eLinearUnit.Name
                    Throw ex
            End Select

            Return bResult

        End Function

        Public Function AddToMap(ByVal pArcMap As IApplication, _
                         ByVal sGroupLayerName As String, _
                         ByVal eRasterType As GCDRasterType, _
                         ByVal sFullPath As String, _
                         ByVal sLayerName As String, _
                         ByVal sSurveyName As String, _
                         Optional ByVal bAddToMapIfPresent As Boolean = False, _
                         Optional ByVal sSymbologyLayerFile As String = "") As IRasterLayer

            Dim pResultLayer As ILayer = Nothing
            Dim pMXDoc As IMxDocument = pArcMap.Document
            Dim pMap As ESRI.ArcGIS.Carto.IMap = pMXDoc.FocusMap
            '
            ' Check if the raster already exists in the map. Don't add it again
            ' unless the GCD settings dictate so.
            '


            If Not bAddToMapIfPresent Then
                pResultLayer = GISCode.ArcMap.GetLayerBySource(pArcMap, sFullPath) '(sLayerName, pArcMap, eEsriLayerType.Esri_DataLayer)
                If TypeOf pResultLayer Is IRasterLayer Then
                    Return pResultLayer
                End If
            End If

            Dim pActiveView As ESRI.ArcGIS.Carto.IActiveView = pMap
            Dim pContentsView As ESRI.ArcGIS.ArcMapUI.IContentsView = pMXDoc.CurrentContentsView ' Get the TOC
            Dim bZoomMap As Boolean = pMap.LayerCount = 0

            Dim pProjectGrpLyr As IGroupLayer = GetGroupLayer(pArcMap, sGroupLayerName)

            'If TypeOf pProjectGrpLyr Is IGroupLayer Then
            Dim pParentGrpLyr As IGroupLayer = pProjectGrpLyr
            Dim pSurveyGrpLyr As IGroupLayer = GISCode.ArcMap.GetLayerByName(sSurveyName, pArcMap, eEsriLayerType.Esri_GroupLayer)
            If TypeOf pSurveyGrpLyr Is IGroupLayer Then
                pParentGrpLyr = pSurveyGrpLyr
            Else
                If Not String.IsNullOrEmpty(sSurveyName) Then
                    pSurveyGrpLyr = New GroupLayerClass
                    pSurveyGrpLyr.Name = sSurveyName
                    Dim pMapLayers As IMapLayers = pMap
                    pMapLayers.InsertLayerInGroup(pProjectGrpLyr, pSurveyGrpLyr, True, 0)
                    'pProjectGrpLyr.Add(pSurveyGrpLyr)
                    pParentGrpLyr = pSurveyGrpLyr
                End If
            End If
            '
            ' Now add the raster layer
            '
            Dim eType As RasterLayerTypes

            Select Case eRasterType

                Case GCDRasterType.ErrorCalc
                    eType = RasterLayerTypes.ErrorSurfaces

                Case GCDRasterType.Surface
                    eType = RasterLayerTypes.SlopePercentRise

                Case Else
                    '
                    ' probably DEM
                    '
                    eType = RasterLayerTypes.DEM
            End Select
            pResultLayer = AddToMapRaster(pMap, sFullPath, sLayerName, pParentGrpLyr, eType, sSymbologyLayerFile)


            ' End If

            If bZoomMap AndAlso TypeOf pResultLayer Is ILayer Then
                pMXDoc.ActiveView.Extent = pResultLayer.AreaOfInterest
            End If

            pMXDoc.UpdateContents()
            pMXDoc.ActiveView.Refresh()
            pContentsView.Refresh(Nothing)
            Return pResultLayer

        End Function

        Public Function AddToMapDoD(ByVal pArcMap As IApplication, _
                                    sGroupLayer As String, _
                                    ByVal sFullPath As String, _
                                    ByVal sNewSurveyName As String, _
                                    ByVal sOldSurveyName As String, _
                                    ByVal sDoDName As String, _
                                    Optional bAddToMapIfPresent As Boolean = False, _
                                    Optional sLayerFile As String = "") As IRasterLayer

            Dim pResultLayer As ILayer = Nothing
            Dim pMXDoc As IMxDocument = pArcMap.Document
            Dim pMap As ESRI.ArcGIS.Carto.IMap = pMXDoc.FocusMap
            Dim pActiveView As ESRI.ArcGIS.Carto.IActiveView = pMap
            Dim pContentsView As ESRI.ArcGIS.ArcMapUI.IContentsView = pMXDoc.CurrentContentsView ' Get the TOC
            Dim pMapLayers As IMapLayers = pMap
            '
            ' Check if the raster already exists in the map. Don't add it again
            ' unless the GCD settings dictate so.
            '
            If Not bAddToMapIfPresent Then
                pResultLayer = GISCode.ArcMap.GetLayerByName(sDoDName, pArcMap, eEsriLayerType.Esri_DataLayer)
                If TypeOf pResultLayer Is IRasterLayer Then
                    Return pResultLayer
                End If
            End If

            Dim pProjectGrpLyr As IGroupLayer = GetGroupLayer(pArcMap, sGroupLayer)

            If TypeOf pProjectGrpLyr Is IGroupLayer Then
                Dim pDoDLyr As IGroupLayer = GISCode.ArcMap.GetLayerByName(sDoDGroupLayerName, pArcMap, eEsriLayerType.Esri_GroupLayer)
                If Not TypeOf pDoDLyr Is IGroupLayer Then
                    pDoDLyr = New GroupLayerClass
                    pDoDLyr.Name = sDoDGroupLayerName
                    'pProjectGrpLyr.Add(pDoDLyr)
                    pMapLayers.InsertLayerInGroup(pProjectGrpLyr, pDoDLyr, True, 0)
                End If

                Dim surveyPairName As String = sNewSurveyName & " - " & sOldSurveyName
                Dim pSurveyPairLyr As IGroupLayer = GISCode.ArcMap.GetLayerByName(surveyPairName, pArcMap, eEsriLayerType.Esri_GroupLayer)
                If Not TypeOf pSurveyPairLyr Is IGroupLayer Then
                    pSurveyPairLyr = New GroupLayerClass
                    pSurveyPairLyr.Name = surveyPairName
                    'pDoDLyr.Add(pSurveyPairLyr)
                    pMapLayers.InsertLayerInGroup(pDoDLyr, pSurveyPairLyr, True, 0)
                End If

                AddToMapRaster(pMap, sFullPath, sDoDName, pSurveyPairLyr, RasterLayerTypes.DoD, sLayerFile)

                pMXDoc.UpdateContents()
                pMXDoc.ActiveView.Refresh()
                pContentsView.Refresh(Nothing)
                Return pResultLayer
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Add a raster or feature layer to the current ArcMap document.
        ''' </summary>
        ''' <param name="pArcMap">ArcMap application</param>
        ''' <param name="sSource">Full path of the GIS data source</param>
        ''' <param name="sDisplayName">Name to be used into the Table of Contents</param>
        ''' <param name="eType">Raster or Vector</param>
        ''' <param name="sGroupLayer">Optional name of a Group Layer (will create if needed)</param>
        ''' <param name="sSymbologyLayerFile">Optional symbology layer file</param>
        ''' <param name="bAddToMapIfPresent">If True then the layer is added even if it exists in the map already</param>
        ''' <returns>pointer to the layer added to the map - or the layer that was already in the map</returns>
        ''' <remarks>PGB 16 May 2012. New consolidated AddToMap method for both raster and vector.</remarks>
        Public Function AddToMap(pArcMap As IApplication, sSource As String, sDisplayName As String, eType As GISCode.GISDataStructures.BasicGISTypes, Optional sGroupLayer As String = "", Optional sSymbologyLayerFile As String = "", Optional bAddToMapIfPresent As Boolean = False) As ILayer

            If Not TypeOf pArcMap Is IApplication Then
                Return Nothing
            End If
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Check the arguments
            '
            If String.IsNullOrEmpty(sSource) Then
                Throw New ArgumentNullException("Source Path", "Null or empty data source path")
            End If

            If String.IsNullOrEmpty(sDisplayName) Then
                Throw New ArgumentNullException("Display Name", "Null or empty display Name")
            End If

            If Not String.IsNullOrEmpty(sSymbologyLayerFile) Then
                If Not IO.File.Exists(sSymbologyLayerFile) Then
                    Throw New ArgumentOutOfRangeException("Symbology Layer File", sSymbologyLayerFile, "Symbology layer file does not exist")
                End If
            End If
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Only add if it doesn't exist already
            '
            Dim pResultLayer As ILayer = GISCode.ArcMap.GetLayerBySource(pArcMap, sSource)
            If TypeOf pResultLayer Is ILayer Then
                If Not bAddToMapIfPresent Then
                    Return pResultLayer
                End If
            End If
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Get or create the group layer if one is specified.
            '
            Dim pGrpLayer As IGroupLayer = Nothing
            If Not String.IsNullOrEmpty(sGroupLayer) Then
                pGrpLayer = GetGroupLayer(pArcMap, sGroupLayer)
            End If

            Dim pMXDoc As IMxDocument = pArcMap.Document
            Dim pMap As ESRI.ArcGIS.Carto.IMap = pMXDoc.FocusMap

            If eType = GISDataStructures.BasicGISTypes.Raster Then
                '
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' RASTER
                '
                Dim gRaster As New GISDataStructures.Raster(sSource)
                gRaster.CalculateStatistics()
                Dim pRL As IRasterLayer = gRaster.RasterLayer
                If Not String.IsNullOrEmpty(sSymbologyLayerFile) Then
                    pRL.ApplySymbology(pRL, sSymbologyLayerFile)
                End If
                pResultLayer = pRL
            Else
                '
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' VECTOR
                '
                'Dim eVectorType As GISDataStructures.GeometryTypes
                'Select Case eType
                '    Case GISDataStructures.BasicGISTypes.Point
                '        eVectorType = GISDataStructures.GeometryTypes.Point
                '    Case GISDataStructures.BasicGISTypes.Line
                '        eVectorType = GISDataStructures.GeometryTypes.Line
                '    Case GISDataStructures.BasicGISTypes.Polygon
                '        eVectorType = GISDataStructures.GeometryTypes.Polygon
                '    Case Else
                '        Throw New Exception("Unhandled geometry type")
                'End Select

                Dim gF As New GISDataStructures.VectorDataSource(sSource)
                Dim pFL = New FeatureLayer
                pFL.FeatureClass = gF.FeatureClass
                pResultLayer = pFL

                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFL.FeatureClass)
            End If
            '
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Finally, add the layer to the map
            '
            Dim pMapLayers As IMapLayers = pMap
            pResultLayer.Name = sDisplayName
            If pGrpLayer Is Nothing Then
                pMapLayers.InsertLayer(pResultLayer, True, 0)
            Else
                pMapLayers.InsertLayerInGroup(pGrpLayer, pResultLayer, True, 0)
            End If

            'Release lock on raster layer - FP Jan 10 2012
            If pMapLayers IsNot Nothing Then
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pMapLayers)
                pMapLayers = Nothing
            End If

            pMXDoc.UpdateContents()
            pMXDoc.ActiveView.Refresh()
            pMXDoc.CurrentContentsView.Refresh(Nothing)

            Return pResultLayer

        End Function

        ''' <summary>
        ''' Add a raster to the current ArcMap document.
        ''' </summary>
        ''' <param name="pMap">Current Map Document</param>
        ''' <param name="sSource">Full path to the raster source</param>
        ''' <param name="sDisplayName">Name use for the raster layer in the TOC</param>
        ''' <param name="pGrpLayer">Optional group layer into which the raster layer will be added</param>
        ''' <param name="lyrType">Optional GCD layer type (i.e. DEM, associated surface, Error Grid).</param>
        ''' <param name="sSymobologyLayerFile">Optional symbology file that will be used to display the raster.</param>
        ''' <returns>The layer representing the raster in the current ArcMap document</returns>
        ''' <remarks>this is a PRIVATE function only called by the other publically accessible AddToMap() functions</remarks>
        Private Function AddToMapRaster(ByVal pMap As IMap, ByVal sSource As String, ByVal sDisplayName As String, _
                                               Optional ByVal pGrpLayer As IGroupLayer = Nothing, Optional ByVal lyrType As RasterLayerTypes = Nothing, _
                                               Optional sSymobologyLayerFile As String = "") As ILayer

            Dim pLayer As ILayer = Nothing
            Dim pGXLayer As IGxLayer = New GxLayerClass
            Dim pGXFile As IGxFile = pGXLayer

            If sSource.ToLower.EndsWith("lyr") Then

                ' Simply load the source into a layer file
                pGXFile.Path = sSource
                pLayer = pGXLayer.Layer

            Else
                '
                ' Create a layer for this data source. Get the default symbology for this raster type
                ' apply the symbology to the new layer and then add it to the map
                '
                Dim pRR As IRasterRenderer = Nothing
                If Not String.IsNullOrEmpty(sSymobologyLayerFile) AndAlso IO.File.Exists(sSymobologyLayerFile) Then

                    ' Load this layer file and retrieve the symbology from it.
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

                End If
                '
                ' PGB 27 Feb 2012 - Ensure to calculate the raster statistics so that the bankfull slider and other
                ' user elements that rely on them, work on them OK.
                '
                Dim gRaster As New GISDataStructures.Raster(sSource)
                gRaster.CalculateStatistics()
                '
                ' Now load the data source into a layer file and apply the symbology from above if one was created.
                '
                Dim pRL As IRasterLayer = gRaster.RasterLayer
                If TypeOf pRR Is IRasterRenderer Then
                    pRL.Renderer = pRR
                End If
                pLayer = pRL
            End If
            '
            ' Add the layer file to the map
            '
            If TypeOf pLayer Is ILayer Then
                Dim pMapLayers As IMapLayers = pMap
                pLayer.Name = sDisplayName
                If pGrpLayer Is Nothing Then
                    'pMap.AddLayer(pLayer)
                    pMapLayers.InsertLayer(pLayer, True, 0)
                Else
                    'pGrpLayer.Add(pLayer)
                    pMapLayers.InsertLayerInGroup(pGrpLayer, pLayer, True, 0)
                End If

                'Release lock on raster layer - FP Jan 10 2012
                If pMapLayers IsNot Nothing Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pMapLayers)
                    pMapLayers = Nothing
                End If
            End If

            Return pLayer

        End Function

        ''' <summary>
        ''' Get the group layer from the ArcMap TOC with the specified name, creating it if needed.
        ''' </summary>
        ''' <param name="pArcMap">ArcMap</param>
        ''' <param name="sName">The name of the group layer</param>
        ''' <param name="bCreateIfNeeded">If true then the group layer will be created if it doesn't exist</param>
        ''' <returns>The group layer </returns>
        ''' <remarks>The default is to create the group layer if it doesn't exist</remarks>
        Public Function GetGroupLayer(ByVal pArcMap As IApplication, sName As String, Optional bCreateIfNeeded As Boolean = True) As IGroupLayer

            If String.IsNullOrEmpty(sName) Then
                '
                ' This route might be needed if the GCD calls this function without an open project.
                '
                Return Nothing
            Else
                '
                ' Try and get the group layer with the name
                '
                Dim pMXDoc As IMxDocument = pArcMap.Document
                Dim pMap As ESRI.ArcGIS.Carto.IMap = pMXDoc.FocusMap
                Dim pGrpLayer As IGroupLayer = GISCode.ArcMap.GetLayerByName(sName, pArcMap, eEsriLayerType.Esri_GroupLayer)
                If Not TypeOf pGrpLayer Is IGroupLayer Then
                    pGrpLayer = New GroupLayerClass
                    pGrpLayer.Name = sName
                    Dim pMapLayers As IMapLayers = pMap
                    pMapLayers.InsertLayer(pGrpLayer, True, 0)
                End If
                Return pGrpLayer
            End If

        End Function

        Public Function GetGroupLayer(pArcMap As IApplication, sName As String, pParentGroupLayer As IGroupLayer, Optional bCreateIfNeeded As Boolean = True) As IGroupLayer

            If String.IsNullOrEmpty(sName) Then
                '
                ' This route might be needed if the GCD calls this function without an open project.
                '
                Return Nothing
            Else
                '
                ' Try and find the group layer already in the hierarchy
                Dim pResultLayer As IGroupLayer = Nothing
                Dim pCompositeLayer As ICompositeLayer = pParentGroupLayer
                For i = 0 To pCompositeLayer.Count - 1
                    If String.Compare(pCompositeLayer.Layer(i).Name, sName, True) = 0 Then
                        pResultLayer = pCompositeLayer.Layer(i)
                    End If
                Next

                If pResultLayer Is Nothing Then
                    '
                    ' Try and get the group layer with the name
                    '
                    Dim pMXDoc As IMxDocument = pArcMap.Document
                    Dim pMap As ESRI.ArcGIS.Carto.IMap = pMXDoc.FocusMap
                    pResultLayer = New GroupLayerClass
                    pResultLayer.Name = sName
                    Dim pMapLayers As IMapLayers = pMap
                    'pMapLayers.InsertLayer(pResultLayer, True, 0)
                    pMapLayers.InsertLayerInGroup(pParentGroupLayer, pResultLayer, True, 0)

                End If
                Return pResultLayer
            End If

        End Function

        ''' <summary>
        ''' Checks to see if a group layer exists within a parent group layer
        ''' </summary>
        ''' <param name="pArcMap"></param>
        ''' <param name="sName">String name of group layer to search for</param>
        ''' <param name="pParentGroupLayer">IGroupLayer of parent group layer</param>
        ''' <returns>Boolean value of True or False</returns>
        ''' <remarks></remarks>
        Public Function SubGroupLayerExists(ByVal pArcMap As IApplication, ByVal sName As String, ByVal pParentGroupLayer As IGroupLayer) As Boolean

            If String.IsNullOrEmpty(sName) Then
                Throw New Exception("Must provide name of group layer to check for existence")
            End If

            ' Try and find the group layer already in the hierarchy
            Dim bPresent As Boolean = False
            Dim pCompositeLayer As ICompositeLayer = pParentGroupLayer
            For i = 0 To pCompositeLayer.Count - 1
                If String.Compare(pCompositeLayer.Layer(i).Name, sName, True) = 0 Then
                    bPresent = True
                End If
            Next

            Return bPresent

        End Function

        Public Function AddDEMGroupLayer(ByVal pArcMap As IApplication, ByVal sProjectName As String) As IGroupLayer
            '
            ' Try and get the group layer with the project name
            '
            Dim pGrpLayer As IGroupLayer
            pGrpLayer = GISCode.ArcMap.GetLayerByName(sProjectName, pArcMap, eEsriLayerType.Esri_GroupLayer) ', eEsriLayerType.Esri_GroupLayer)

            If Not TypeOf pGrpLayer Is IGroupLayer Then
                pGrpLayer = New GroupLayerClass
                pGrpLayer.Name = sProjectName

            End If

            Return pGrpLayer

        End Function

        Public Sub SetupProgressBar(ByVal m_application As IApplication, ByVal sMessage As String, ByVal iMaxStep As Integer, Optional ByVal iPosition As Integer = 0)
            If m_application Is Nothing Then
                Exit Sub
            End If
            Dim pStatusBar As IStatusBar
            Dim pProgbar As IStepProgressor
            pStatusBar = m_application.StatusBar
            pProgbar = pStatusBar.ProgressBar

            pProgbar.Position = iPosition
            pStatusBar.ShowProgressBar(sMessage, 0, iMaxStep, 1, True)
            Debug.Print(sMessage)
        End Sub

        Public Sub StepProgressBar(ByVal m_application As IApplication)
            If m_application Is Nothing Then
                Exit Sub
            End If
            Dim pStatusBar As IStatusBar
            pStatusBar = m_application.StatusBar
            pStatusBar.StepProgressBar()
            '
            ' PGB 5 July 2012
            ' Need to allow the UI to update when using the progress bar within intensive looping
            '
            Application.DoEvents()
        End Sub

        Public Sub HideProgressBar(ByVal m_application As IApplication)
            If m_application Is Nothing Then
                Exit Sub
            End If
            Dim pStatusBar As IStatusBar
            pStatusBar = m_application.StatusBar
            pStatusBar.HideProgressBar()
        End Sub

        Public Sub SetStatus(ByRef arcmap As IApplication, ByVal message As String)
            If arcmap Is Nothing Then
                Exit Sub
            End If
            Dim pStatusBar As IStatusBar
            pStatusBar = arcmap.StatusBar

            pStatusBar.Message(0) = message
            Application.DoEvents()
        End Sub

        ''' <summary>
        ''' Get the license level of the current ArcMap
        ''' </summary>
        ''' <returns>ESRI license level enumeration</returns>
        ''' <remarks></remarks>
        Public Function GetArcGISLicenseLevel() As ESRILicenseInfo
            Dim ESRILicenseInfo As ESRI.ArcGIS.esriSystem.IESRILicenseInfo = New ESRI.ArcGIS.esriSystem.ESRILicenseInfoClass()
            Return ESRILicenseInfo
        End Function

        Public Function IsInEditSession(pArcMap As IApplication) As Boolean

            Dim pEditorUID As UID = New UID()
            pEditorUID.Value = "esriEditor.Editor"
            Dim pEditor As ESRI.ArcGIS.Editor.IEditor = pArcMap.FindExtensionByCLSID(pEditorUID)
            Return pEditor.EditState <> ESRI.ArcGIS.Editor.esriEditState.esriStateNotEditing

            'Dim mxMap As IMxDocument = pArcMap.Document
            'Dim pMap As IMap = mxMap.FocusMap
            'Dim pLayer As ILayer = Nothing

            'Try
            '    For i As Integer = 0 To pMap.LayerCount - 1
            '        pLayer = pMap.Layer(i)
            '        Dim sLayerPath As String = String.Empty

            '        If TypeOf pLayer Is IGeoFeatureLayer Then
            '            Dim pGFLayer As IGeoFeatureLayer = pLayer
            '            Dim pFLayer As IFeatureLayer = pLayer
            '            Dim pFClass As IFeatureClass = pFLayer.FeatureClass
            '            Dim pGDS As IGeoDataset = pFClass
            '            Dim pDS As IWorkspace = pFClass


            '        End If
            '    Next
            'Catch ex As Exception

            'End Try

            'Dim pEditor As ieditor


            ''Check to see if you are in an operation.
            'Dim workspaceEdit As IWorkspaceEdit2 = CType(m_editor.EditWorkspace, IWorkspaceEdit2)

            'If (Not workspaceEdit.IsInEditOperation) Then
            '    ' Close the first operation.
            '    m_editor.StopOperation("edit")
            'End If

        End Function

        Public Sub RemoveLayersfromTOC(ByRef application As IApplication, ByVal directory As String)
            'loop through TOC
            'if layer path directory = directory
            'remove from TOC

            Dim mxMap As IMxDocument = CType(application.Document, IMxDocument)
            Dim pMap As IMap = mxMap.FocusMap
            Dim pMapLayers As IMapLayers = pMap

            Try
                For i As Integer = 0 To pMap.LayerCount - 1
                    Dim player As ILayer = pMap.Layer(i)
                    If Not TypeOf (player) Is IGroupLayer Then
                        Dim pDS As IDataset = player
                        Try
                            If LCase(directory) = LCase(pDS.Workspace.PathName) Then
                                pMap.DeleteLayer(player)
                            End If
                        Catch ex As Exception

                        End Try
                    Else
                        'remove all from group layer
                        RemoveLayersfromGroupLayer(player, directory)
                    End If

                    'remove lock on raster layer - FP Jan 10 2012
                    If player IsNot Nothing Then
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(player)
                        player = Nothing
                    End If

                Next
                mxMap.UpdateContents()
                mxMap.ActiveView.Refresh()

                Dim pContentsView As ESRI.ArcGIS.ArcMapUI.IContentsView = mxMap.CurrentContentsView ' Get the TOC
                pContentsView.Refresh(Nothing)

                If mxMap IsNot Nothing Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(mxMap)
                    mxMap = Nothing
                End If

                If pContentsView IsNot Nothing Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pContentsView)
                    pContentsView = Nothing
                End If


            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                Debug.WriteLine(ex.StackTrace)
            End Try
        End Sub

        Public Sub RemoveLayersfromGroupLayer(ByRef pGroupLayer As IGroupLayer, ByVal directory As String)
            Dim pLayer As ILayer
            Dim pCompositeLayer As ICompositeLayer

            Dim i As Integer
            Dim LayersToDelete As New List(Of ILayer)

            pCompositeLayer = pGroupLayer

            For i = 1 To pCompositeLayer.Count

                pLayer = pCompositeLayer.Layer(i - 1)

                If Not TypeOf (pLayer) Is IGroupLayer Then
                    Try
                        Dim pDS As IDataset = pLayer
                        Dim LayerDirectoryname As String = LCase(pDS.Workspace.PathName)
                        'strip ending "\" is there
                        If LayerDirectoryname.EndsWith(IO.Path.DirectorySeparatorChar) Then
                            LayerDirectoryname = LayerDirectoryname.Substring(0, LayerDirectoryname.Length - 1)
                        End If

                        If LCase(directory) = LayerDirectoryname Then
                            LayersToDelete.Add(pLayer)
                        End If

                    Catch ex As Exception
                        Debug.WriteLine(ex.Message)
                    End Try
                Else
                    'remove all from group layer
                    RemoveLayersfromGroupLayer(pLayer, directory)
                End If

            Next

            For Each pDeleteLayer As ILayer In LayersToDelete

                pGroupLayer.Delete(pDeleteLayer)

                If pDeleteLayer IsNot Nothing Then
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pDeleteLayer)
                    pDeleteLayer = Nothing
                End If

            Next

            If pGroupLayer IsNot Nothing Then
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pGroupLayer)
                pGroupLayer = Nothing
            End If

        End Sub

        ''' <summary>
        ''' Remove the group layer with the specified name from the ArcMap ToC
        ''' </summary>
        ''' <param name="pArcMap"></param>
        ''' <param name="sGroupLayerName"></param>
        ''' <remarks></remarks>
        Public Sub RemoveGroupLayer(ByVal pArcMap As IApplication, ByVal sGroupLayerName As String)

            GISCode.ArcMap.SetStatus(pArcMap, "Removing group layer from map document...")

            Dim pMXDoc As ESRI.ArcGIS.ArcMapUI.IMxDocument = pArcMap.Document
            Dim pMap As ESRI.ArcGIS.Carto.IMap = pMXDoc.FocusMap

            Dim pUID As ESRI.ArcGIS.esriSystem.IUID = New ESRI.ArcGIS.esriSystem.UID
            pUID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}"
            Dim pEnum As IEnumLayer = pMap.Layers(pUID, True)
            Dim pL As ILayer = pEnum.Next
            Do While TypeOf pL Is ILayer
                If String.Compare(sGroupLayerName, pL.Name, True) = 0 Then
                    pMap.DeleteLayer(pL)
                End If
                pL = pEnum.Next
            Loop

            GISCode.ArcMap.SetStatus(pArcMap, "")

        End Sub

        ''' <summary>
        ''' This class is used when comparing user ArcMap version to past, target and future versions of ArcMap
        ''' </summary>
        ''' <remarks>Currently implemented in Addin\GCDExtension</remarks>
        Public Class ESRIVersions
            Private m_Past As System.Version
            Private m_Target As System.Version
            Private m_Future As System.Version
            Private m_User As System.Version

            Public Sub New()
                m_Past = New System.Version(My.Resources.PastESRIVersion)
                m_Target = New System.Version(My.Resources.TargetESRIVersion)
                m_Future = New System.Version(My.Resources.FutureESRIVersion)
                Dim userVersionString = GetArcGISVersion()

                If Not String.IsNullOrEmpty(userVersionString) Then
                    m_User = New System.Version(userVersionString)
                Else
                    m_User = New System.Version("0.0")
                End If
            End Sub

            Public ReadOnly Property Past As System.Version
                Get
                    Return m_Past
                End Get
            End Property

            Public ReadOnly Property Target As System.Version
                Get
                    Return m_Target
                End Get
            End Property

            Public ReadOnly Property Future As System.Version
                Get
                    Return m_Future
                End Get
            End Property

            Public ReadOnly Property User As System.Version
                Get
                    Return m_User
                End Get
            End Property
        End Class

        ''' <summary>
        ''' Checks the ArcGIS version 
        ''' </summary>
        ''' <returns>version number being utilized by the machine as a string</returns>
        ''' <remarks>implemented in Addin\GCDExtension in the OnStartUp() method</remarks>
        Public Function GetArcGISVersion() As String

            Dim sArcGISRegistryKey As String = String.Empty
            If GISCode.OperatingSystem.Is64BitOperatingSystem() Then
                sArcGISRegistryKey = "SOFTWARE\\Wow6432Node\\ESRI\\ArcGIS"
            Else
                sArcGISRegistryKey = "SOFTWARE\\ESRI\\ArcGIS"
            End If

            Dim sRealVersion As String = String.Empty
            Dim regKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sArcGISRegistryKey, False)
            If TypeOf regKey Is Microsoft.Win32.RegistryKey Then
                If regKey.GetValueNames.Contains("RealVersion") Then
                    sRealVersion = regKey.GetValue("RealVersion", "")
                Else
                    Throw New Exception("The registry key (" & sArcGISRegistryKey & ") does not contain a value for RealVersion.")
                End If
            Else
                Throw New Exception("Unable to find the registry key " & sArcGISRegistryKey & " needed to find the ArcGIS version number.")
            End If
            Return sRealVersion

        End Function

        Public Sub MakeSingleLayerSelectable(pArcMap As IApplication, pTheSelectableLayer As IFeatureLayer)

            Dim pMXDoc As IMxDocument = pArcMap.Document
            Dim pMap As IMap = pMXDoc.FocusMap
            Dim pUID As IUID = New UID
            pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"
            Dim pEnum As IEnumLayer = pMap.Layers(pUID, True)
            Dim pL As ILayer = pEnum.Next
            Do While TypeOf pL Is ILayer
                If TypeOf pL Is IFeatureLayer Then
                    Dim pFL As IFeatureLayer = pL
                    pFL.Selectable = False
                End If
                pL = pEnum.Next
            Loop
            pTheSelectableLayer.Selectable = True
        End Sub

        Public Sub MakeAllLayersSelectable(ByVal pArcMap As IApplication)

            Dim pMXDoc As IMxDocument = pArcMap.Document
            Dim pMap As IMap = pMXDoc.FocusMap
            Dim pUID As IUID = New UID
            pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"
            Dim pEnum As IEnumLayer = pMap.Layers(pUID, True)
            Dim pL As ILayer = pEnum.Next
            Do While TypeOf pL Is ILayer
                If TypeOf pL Is IFeatureLayer Then
                    Dim pFL As IFeatureLayer = pL
                    pFL.Selectable = True
                End If
                pL = pEnum.Next
            Loop
        End Sub
    End Module
End Namespace

