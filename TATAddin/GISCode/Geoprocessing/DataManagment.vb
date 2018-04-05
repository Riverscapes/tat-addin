#Region "Imports"
Imports System.IO
Imports ESRI.ArcGIS.DataManagementTools
Imports ESRI.ArcGIS.Geoprocessor
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.CartographyTools
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.Carto

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

Namespace GISCode.GP.DataManagement

    Public Enum DissolveMultiPartEnum
        MULTI_PART
        SINGLE_PART
    End Enum

    Public Enum DissolveUnsplitLinesEnum
        DISSOLVE_LINES
        UNSPLIT_LINES
    End Enum

    Public Module DataManagement
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="gVectorDataSource">Vector data source containing the field to drop</param>
        ''' <param name="sFieldToDrop"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000004n000000
        ''' License Level: All Levels</remarks>
        Public Sub DeleteField(gVectorDataSource As GISDataStructures.VectorDataSource,
                               ByVal sFieldToDrop As String)


            If Not TypeOf gVectorDataSource Is GISDataStructures.VectorDataSource Then
                Throw New ArgumentNullException("gVectorDataSource", "The input feature class is null")
            End If

            If Not gVectorDataSource.Exists Then
                Throw New ArgumentException("gVectorDataSource", "The input feature class does not exist")
            End If

            If String.IsNullOrEmpty(sFieldToDrop) Then
                Throw New Exception("drop_field is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim DeleteFieldTool As New DeleteField
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            DeleteFieldTool.in_table = gVectorDataSource.FeatureClass
            DeleteFieldTool.drop_field = sFieldToDrop

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(DeleteFieldTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("Feature Class") = gVectorDataSource.FullPath
                ex.Data("Field to drop") = sFieldToDrop
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Add a field to a feature class
        ''' </summary>
        ''' <param name="gVectorDataSource">Feature class to which the field will be added</param>
        ''' <param name="sFieldName">Name of the field to add</param>
        ''' <param name="eFieldType">ESRI enumeration of field types</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000047000000
        ''' License Level: All Levels</remarks>
        Public Sub AddField(ByVal gVectorDataSource As GISDataStructures.VectorDataSource,
                            ByVal sFieldName As String,
                            ByVal eFieldType As esriFieldType,
                            ByVal sLength As Integer)

            Dim sFieldType As String = ""

            Try
                If Not TypeOf gVectorDataSource Is GISDataStructures.VectorDataSource Then
                    Throw New ArgumentNullException("gVectorDataSource", "The input feature class is null")
                End If

                If Not gVectorDataSource.Exists Then
                    Throw New ArgumentException("gVectorDataSource", "The input feature class does not exist")
                End If

                If String.IsNullOrEmpty(sFieldName) Then
                    Throw New Exception("field_name is null or empty string.")
                End If

                If sFieldName.Contains(" ") Then
                    Throw New ArgumentException("sFieldName", "The field name cannot contain spaces")
                End If

                If gVectorDataSource.GISDataStorageType = GISDataStructures.GISDataStorageTypes.ShapeFile Then
                    If sFieldName.Length > 10 Then
                        Throw New ArgumentException("sFieldName", "The field name cannot be longer than 10 characters for shapefiles")
                    End If
                End If

                Select Case eFieldType
                    Case esriFieldType.esriFieldTypeInteger : sFieldType = "LONG"
                    Case esriFieldType.esriFieldTypeString : sFieldType = "STRING"
                    Case esriFieldType.esriFieldTypeDouble : sFieldType = "DOUBLE"
                    Case esriFieldType.esriFieldTypeSmallInteger : sFieldType = "SHORT"
                    Case esriFieldType.esriFieldTypeDate : sFieldType = "DATE"
                    Case esriFieldType.esriFieldTypeGUID : sFieldType = "GUID"
                    Case Else
                        Throw New Exception("Invalid field type for new field")
                End Select

                If String.IsNullOrEmpty(sFieldType) Then
                    Throw New Exception("field_type is null or empty string.")
                End If

                If sLength < 1 Then
                    Throw New ArgumentOutOfRangeException("The field length cannot be less than zero")
                End If

            Catch ex As Exception
                ex.Data("Feature class") = gVectorDataSource.FullPath
                ex.Data("Field to add") = sFieldName
                ex.Data("Field type") = eFieldType.ToString
                Throw
            End Try


            Dim GP As New Geoprocessor
            Dim AddFieldTool As New AddField
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            AddFieldTool.in_table = gVectorDataSource.FeatureClass
            AddFieldTool.field_name = sFieldName
            AddFieldTool.field_type = sFieldType

            If eFieldType = esriFieldType.esriFieldTypeString OrElse eFieldType = esriFieldType.esriFieldTypeBlob Then
                AddFieldTool.field_length = sLength
            End If

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(AddFieldTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("Feature Class") = gVectorDataSource.FeatureClass
                ex.Data("fieldName") = sFieldName
                ex.Data("fieldType") = sFieldType
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub AddField(ByVal inTable As String,
                            ByVal fieldName As String,
                            ByVal fieldType As String)

            If String.IsNullOrEmpty(inTable) Then
                Throw New Exception("in_table is null or empty string.")
            End If

            If String.IsNullOrEmpty(fieldName) Then
                Throw New Exception("field_name is null or empty string.")
            End If

            If String.IsNullOrEmpty(fieldType) Then
                Throw New Exception("field_type is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim AddFieldTool As New AddField
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            AddFieldTool.in_table = inTable
            AddFieldTool.field_name = fieldName
            AddFieldTool.field_type = fieldType

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()
            Try
                GP.Execute(AddFieldTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("in_table") = inTable
                ex.Data("field_name") = fieldName
                ex.Data("field_type") = fieldType
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        Public Sub Rename(ByVal sInName As String, ByVal sOutName As String)

            If String.IsNullOrEmpty(sInName) Then
                Throw New Exception("sInName is null or empty string.")
            End If

            If String.IsNullOrEmpty(sOutName) Then
                Throw New Exception("sOutName is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim RenameTool As New ESRI.ArcGIS.DataManagementTools.Rename
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            RenameTool.in_data = sInName
            RenameTool.out_data = sOutName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(RenameTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("InName") = sInName
                ex.Data("OutName") = sOutName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="layerName"></param>
        ''' <param name="whereClause"></param>
        ''' <param name="selectionType"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000071000000
        ''' License Levels: All Levels</remarks>
        Public Sub SelectLayerByAttribute(ByVal layerName As String,
                                          ByVal whereClause As String,
                                          Optional ByVal selectionType As String = "NEW_SELECTION")

            If String.IsNullOrEmpty(layerName) Then
                Throw New Exception("LayerName is null or empty string.")
            End If

            'If String.IsNullOrEmpty(whereClause) Then
            '    Throw New Exception("where_clause is null or empty string.")
            'End If

            If String.IsNullOrEmpty(selectionType) Then
                Throw New Exception("selection_type is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim SelectLayerByAttributeTool As New SelectLayerByAttribute
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            SelectLayerByAttributeTool.in_layer_or_view = layerName
            SelectLayerByAttributeTool.where_clause = whereClause
            SelectLayerByAttributeTool.selection_type = selectionType

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(SelectLayerByAttributeTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("layerName") = layerName
                ex.Data("whereClause") = whereClause
                ex.Data("selectionType") = selectionType
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="whereClause"></param>
        ''' <param name="selectionType"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000071000000
        ''' License Levels: All Levels</remarks>
        Public Sub SelectLayerByAttribute(ByVal pFeatureLayer As IFeatureLayer,
                                          ByVal whereClause As String,
                                          Optional ByVal selectionType As String = "NEW_SELECTION")

            If Not TypeOf pFeatureLayer Is IFeatureLayer Then
                Throw New Exception("The feature layer is not valid")
            End If

            'If String.IsNullOrEmpty(whereClause) Then
            '    Throw New Exception("where_clause is null or empty string.")
            'End If

            If String.IsNullOrEmpty(selectionType) Then
                Throw New Exception("selection_type is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim SelectLayerByAttributeTool As New SelectLayerByAttribute
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            SelectLayerByAttributeTool.in_layer_or_view = pFeatureLayer
            SelectLayerByAttributeTool.where_clause = whereClause
            SelectLayerByAttributeTool.selection_type = selectionType

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(SelectLayerByAttributeTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("layerName") = pFeatureLayer.Name
                ex.Data("whereClause") = whereClause
                ex.Data("selectionType") = selectionType
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="layerName"></param>
        ''' <remarks>PGB 11 jul 2011 - this used to take a feature class for the first argument. Unsure why when it's
        ''' simply passing to GP as string.
        ''' http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000006p000000
        ''' License Levels: All Levels</remarks>
        Public Sub MakeFeatureLayer(ByVal inFeatures As String,
                                    ByVal layerName As String)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(layerName) Then
                Throw New Exception("LayerName is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim MakeFeatureLayerTool As New MakeFeatureLayer
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            MakeFeatureLayerTool.in_features = inFeatures
            MakeFeatureLayerTool.out_layer = layerName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(MakeFeatureLayerTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("layerName") = layerName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Function MakeFeatureLayer(ByVal inFeatures As IFeatureClass, ByVal sLayerName As String) As IFeatureLayer

            If Not TypeOf inFeatures Is IFeatureClass Then
                Throw New Exception("Null feature class passed.")
            End If

            If String.IsNullOrEmpty(sLayerName) Then
                Throw New Exception("Empty layer name provided.")
            End If

            Dim pFL As IFeatureLayer = New FeatureLayer()
            pFL.FeatureClass = inFeatures
            pFL.Name = sLayerName
            Return pFL

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="layerName"></param>
        ''' <param name="outFeatures"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000035000000
        ''' License Level: All Levels</remarks>
        Public Sub CopyFeatures(ByVal layerName As String,
                                ByVal outFeatures As FileInfo)

            If String.IsNullOrEmpty(layerName) Then
                Throw New Exception("LayerName is null or empty string.")
            End If

            If Not TypeOf outFeatures Is FileInfo Then
                Throw New Exception("Invalid ouput FeatureClass")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim CopyFeaturesTool As New CopyFeatures
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CopyFeaturesTool.in_features = layerName
            CopyFeaturesTool.out_feature_class = outFeatures.FullName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CopyFeaturesTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("layerName") = layerName
                ex.Data("outFeatures") = outFeatures.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub CopyFeatures(ByVal layerName As String,
                                ByVal outFeatures As String)

            If String.IsNullOrEmpty(layerName) Then
                Throw New Exception("LayerName is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("out_features is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim CopyFeaturesTool As New CopyFeatures
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CopyFeaturesTool.in_features = layerName
            CopyFeaturesTool.out_feature_class = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CopyFeaturesTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

            Catch ex As Exception
                ex.Data("layerName") = layerName
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub CopyFeatures(ByVal pFeatureLayer As IFeatureLayer,
                             ByVal outFeatures As String)

            If pFeatureLayer Is Nothing Then
                Throw New Exception("LayerName is null")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("out_features is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim CopyFeaturesTool As New CopyFeatures
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CopyFeaturesTool.in_features = pFeatureLayer
            CopyFeaturesTool.out_feature_class = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CopyFeaturesTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

            Catch ex As Exception
                ex.Data("layerName") = pFeatureLayer.Name
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="layerName"></param>
        ''' <param name="outFeatures"></param>
        ''' <param name="bSetProcessingExtent">True uses the input layer extent as the processing extent. False does not specify an extent.</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000035000000
        ''' License Level: All Levels
        ''' PGB Jan 2015. The batch substrate raster generator in the CHaMP topo toolbar
        ''' needs to call this function multiple times. Unfortunately this somehow retains
        ''' the spatial extent of the initial input feature class and so subsequent calls
        ''' produce and empty feature class. The new optional parameter lets this special
        ''' case pass "True" and then the input layer extent is used as the processing 
        ''' extent for the geoprocess. All other uses of this method can leave this parameter
        ''' off and use the default "False"</remarks>
        Public Sub CopyFeatures(ByVal layerName As IFeatureClass,
                                ByVal outFeatures As String,
                                Optional bSetProcessingExtent As Boolean = False)

            If Not TypeOf (layerName) Is IFeatureClass Then
                Throw New Exception("LayerName is not an ifeatureclass.")
            End If

            If layerName Is Nothing Then
                Throw New Exception("LayerName is nothing.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("outFeatures is null or empty")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim CopyFeaturesTool As New CopyFeatures
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CopyFeaturesTool.in_features = layerName
            CopyFeaturesTool.out_feature_class = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            If bSetProcessingExtent Then
                Dim pExtent As IEnvelope = DirectCast(layerName, IGeoDataset).Extent
                Dim pRect As New GISDataStructures.ExtentRectangle(pExtent.YMax, pExtent.XMin, pExtent.XMax, pExtent.YMin)
                GP.SetEnvironmentValue("extent", pRect.Rectangle)
            End If

            Try
                GP.Execute(CopyFeaturesTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inRaster"></param>
        ''' <param name="outRaster"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000094000000
        ''' License Level: All Levels</remarks>
        Public Sub CopyRaster(ByVal inRaster As String,
                              ByVal outRaster As String, Optional ByVal bDeleteIfExists As Boolean = False)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("in_raster is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("out_raster is null or empty string.")
            End If

            If Not GISDataStructures.Raster.Exists(inRaster) Then
                Dim ex As New Exception("Original Raster does not exist")
                ex.Data("In Raster") = inRaster
            End If

            If GISDataStructures.Raster.Exists(outRaster) Then
                If bDeleteIfExists Then
                    DataManagement.Delete(outRaster, esriDatasetType.esriDTRasterDataset)
                Else
                    Dim ex As New Exception("Output raster already exists")
                    ex.Data("Out Raster") = outRaster
                    Throw ex
                End If
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim CopyRasterTool As New CopyRaster
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CopyRasterTool.in_raster = inRaster
            CopyRasterTool.out_rasterdataset = outRaster

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CopyRasterTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("outRaster") = outRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Calculate a Field in a feature class
        ''' </summary>
        ''' <param name="inTable">Feature Class</param>
        ''' <param name="field">Field name (no parentheses)</param>
        ''' <param name="expression">If using a field, use angle brackets for VB and Exclamation marks for Python</param>
        ''' <param name="type">VB or PYTHON (see remarks)</param>
        ''' <param name="code">Complex code expression.</param>
        ''' <remarks>http://edndoc.esri.com/arcobjects/9.2/net/shared/geoprocessing/data_management_tools/calculate_field_data_management_.htm
        ''' Note that ArcGIS Server does not include VBA and so it's safest to use Python for cross platform compatibility.
        ''' http://support.esri.com/en/knowledgebase/techarticles/detail/36571
        ''' License Level: All Levels</remarks>
        Public Sub CalculateField_management(ByVal inTable As String,
                                             ByVal field As String,
                                             ByVal expression As String,
                                             Optional ByVal type As String = "PYTHON",
                                             Optional ByVal code As String = " ")

            If String.IsNullOrEmpty(inTable) Then
                Throw New Exception("intable is null or empty string.")
            End If

            If String.IsNullOrEmpty(field) Then
                Throw New Exception("field is null or empty string.")
            End If

            If String.IsNullOrEmpty(expression) Then
                Throw New Exception("expression is null or empty string.")
            End If

            If String.IsNullOrEmpty(type) Then
                Throw New Exception("type is null or empty string.")
            End If

            Debug.Assert(String.Compare(type, "PYTHON", True) = 0, "Use only Python syntax so that code works on ArcGIS Server which does not support VBA and VB syntax.")

            Dim GP As New Geoprocessor
            Dim CalcFieldTool As New CalculateField
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CalcFieldTool.in_table = inTable
            CalcFieldTool.field = field
            CalcFieldTool.expression = expression
            CalcFieldTool.expression_type = type
            CalcFieldTool.code_block = code

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CalcFieldTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTable") = inTable
                ex.Data("field") = field
                ex.Data("expression") = expression
                ex.Data("type") = type
                ex.Data("code") = code
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' Calculate a Field in a feature class
        ''' </summary>
        ''' <param name="pFC">Feature Class</param>
        ''' <param name="field">Field name (no parentheses)</param>
        ''' <param name="expression">If using a field, use angle brackets for VB and Exclamation marks for Python</param>
        ''' <param name="etype">VB or PYTHON (see remarks)</param>
        ''' <param name="code">Complex code expression.</param>
        ''' <remarks>http://edndoc.esri.com/arcobjects/9.2/net/shared/geoprocessing/data_management_tools/calculate_field_data_management_.htm
        ''' Note that ArcGIS Server does not include VBA and so it's safest to use Python for cross platform compatibility.
        ''' http://support.esri.com/en/knowledgebase/techarticles/detail/36571
        ''' License Level: All Levels</remarks>
        Public Sub CalculateField_management(ByVal pFC As IFeatureClass,
                                             ByVal field As String,
                                             ByVal expression As String,
                                             Optional ByVal eType As GISCode.Geoprocessing.SyntaxTypes = Geoprocessing.SyntaxTypes.Python,
                                             Optional ByVal code As String = " ")

            If Not TypeOf pFC Is IFeatureClass Then
                Throw New Exception("intable is null or empty string.")
            End If

            If String.IsNullOrEmpty(field) Then
                Throw New Exception("field is null or empty string.")
            End If

            If String.IsNullOrEmpty(expression) Then
                Throw New Exception("expression is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim CalcFieldTool As New CalculateField
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CalcFieldTool.in_table = pFC
            CalcFieldTool.field = field
            CalcFieldTool.expression = expression
            CalcFieldTool.expression_type = Geoprocessing.GetSyntaxType(eType)
            CalcFieldTool.code_block = code

            'Dim pParamArray As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            'Dim sPath As String = GISCode.GISDataStructures.VectorDataSource.GetFeatureClassPath(pFC)
            'pParamArray.Add(sPath)
            'pParamArray.Add(field)
            'pParamArray.Add(expression)
            'pParamArray.Add("PYTHON")

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CalcFieldTool, Nothing)

                'Dim gpb As ESRI.ArcGIS.Geoprocessor.Geoprocessor = New ESRI.ArcGIS.Geoprocessor.Geoprocessor()
                'gpb.OverwriteOutput = True
                'Register to receive the geoprocessor event when the tools have completed execution.
                'AddHandler gpb.ToolExecuted, AddressOf gpToolExecuted
                'Dim gpResult As ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult2 = CType(GP.ExecuteAsync("CalculateField", pParamArray), ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult2)

                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTable") = DirectCast(pFC, IDataset).FullName
                ex.Data("field") = field
                ex.Data("expression") = expression
                ex.Data("type") = eType.ToString
                ex.Data("code") = code
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' Calculate a Field in a feature class
        ''' </summary>
        ''' <param name="pTable">Feature Class</param>
        ''' <param name="field">Field name (no parentheses)</param>
        ''' <param name="expression">If using a field, use angle brackets for VB and Exclamation marks for Python</param>
        ''' <param name="etype">VB or PYTHON (see remarks)</param>
        ''' <param name="code">Complex code expression.</param>
        ''' <remarks>http://edndoc.esri.com/arcobjects/9.2/net/shared/geoprocessing/data_management_tools/calculate_field_data_management_.htm
        ''' Note that ArcGIS Server does not include VBA and so it's safest to use Python for cross platform compatibility.
        ''' http://support.esri.com/en/knowledgebase/techarticles/detail/36571
        ''' License Level: All Levels</remarks>
        Public Sub CalculateField_management(ByVal pTable As ITable,
                                             ByVal field As String,
                                             ByVal expression As String,
                                             Optional ByVal eType As GISCode.Geoprocessing.SyntaxTypes = Geoprocessing.SyntaxTypes.Python,
                                             Optional ByVal code As String = " ")

            If Not TypeOf pTable Is ITable Then
                Throw New Exception("intable is null or empty string.")
            End If

            If String.IsNullOrEmpty(field) Then
                Throw New Exception("field is null or empty string.")
            End If

            If String.IsNullOrEmpty(expression) Then
                Throw New Exception("expression is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim CalcFieldTool As New CalculateField
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CalcFieldTool.in_table = pTable
            CalcFieldTool.field = field
            CalcFieldTool.expression = expression
            CalcFieldTool.expression_type = Geoprocessing.GetSyntaxType(eType)
            CalcFieldTool.code_block = code

            'Dim pParamArray As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            'Dim sPath As String = GISCode.GISDataStructures.VectorDataSource.GetFeatureClassPath(pFC)
            'pParamArray.Add(sPath)
            'pParamArray.Add(field)
            'pParamArray.Add(expression)
            'pParamArray.Add("PYTHON")

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CalcFieldTool, Nothing)

                'Dim gpb As ESRI.ArcGIS.Geoprocessor.Geoprocessor = New ESRI.ArcGIS.Geoprocessor.Geoprocessor()
                'gpb.OverwriteOutput = True
                'Register to receive the geoprocessor event when the tools have completed execution.
                'AddHandler gpb.ToolExecuted, AddressOf gpToolExecuted
                'Dim gpResult As ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult2 = CType(GP.ExecuteAsync("CalculateField", pParamArray), ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult2)

                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTable") = DirectCast(pTable, IDataset).FullName
                ex.Data("field") = field
                ex.Data("expression") = expression
                ex.Data("type") = eType.ToString
                ex.Data("code") = code
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' Calculate a Field in a feature class
        ''' </summary>
        ''' <param name="pFL">Feature Class</param>
        ''' <param name="field">Field name (no parentheses)</param>
        ''' <param name="expression">If using a field, use angle brackets for VB and Exclamation marks for Python</param>
        ''' <param name="type">VB or PYTHON (see remarks)</param>
        ''' <param name="code">Complex code expression.</param>
        ''' <remarks>http://edndoc.esri.com/arcobjects/9.2/net/shared/geoprocessing/data_management_tools/calculate_field_data_management_.htm
        ''' Note that ArcGIS Server does not include VBA and so it's safest to use Python for cross platform compatibility.
        ''' http://support.esri.com/en/knowledgebase/techarticles/detail/36571
        ''' License Level: All Levels</remarks>
        Public Sub CalculateField_management(ByVal pFL As IFeatureLayer,
                                             ByVal field As String,
                                             ByVal expression As String,
                                             Optional ByVal type As String = "PYTHON",
                                             Optional ByVal code As String = " ")

            If Not TypeOf pFL Is IFeatureLayer Then
                Throw New Exception("intable is null or empty string.")
            End If

            If String.IsNullOrEmpty(field) Then
                Throw New Exception("field is null or empty string.")
            End If

            If String.IsNullOrEmpty(expression) Then
                Throw New Exception("expression is null or empty string.")
            End If

            If String.IsNullOrEmpty(type) Then
                Throw New Exception("type is null or empty string.")
            End If

            Debug.Assert(String.Compare(type, "PYTHON", True) = 0, "Use only Python syntax so that code works on ArcGIS Server which does not support VBA and VB syntax.")

            Dim GP As New Geoprocessor
            Dim CalcFieldTool As New CalculateField
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CalcFieldTool.in_table = pFL
            CalcFieldTool.field = field
            CalcFieldTool.expression = expression
            CalcFieldTool.expression_type = type
            CalcFieldTool.code_block = code

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CalcFieldTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inTable") = pFL.Name ' DirectCast(pFL, IDataset).FullName
                ex.Data("field") = field
                ex.Data("expression") = expression
                ex.Data("type") = type
                ex.Data("code") = code
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inRaster"></param>
        ''' <param name="rectangle"></param>
        ''' <param name="outRaster"></param>
        ''' <param name="template"></param>
        ''' <param name="noData"></param>
        ''' <param name="clipGeometry"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000009n000000
        ''' TODO: Duplicated in Analysis
        ''' License Levels: All</remarks>
        Public Sub Clip_management(ByVal inRaster As FileInfo,
                                   ByVal rectangle As String,
                                   ByVal outRaster As FileInfo,
                                   Optional ByVal template As FileInfo = Nothing,
                                   Optional ByVal noData As Integer = 0,
                                   Optional ByVal clipGeometry As Boolean = True)

            'If TypeOf inRaster Is FileInfo Then
            '    If Not inRaster.Exists Then
            '        Throw New Exception("Input raster does not exist.")
            '    End If
            'Else
            '    Throw New Exception("Invalid input raster.")
            'End If

            If String.IsNullOrEmpty(rectangle) Then
                Throw New Exception("rectangle is null or empty string.")
            End If

            If Not TypeOf outRaster Is FileInfo Then
                Throw New Exception("Invalid output raster.")
            End If

            Dim GP As New Geoprocessor
            Dim ClipTool As New Clip
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ClipTool.in_raster = inRaster.FullName
            ClipTool.rectangle = rectangle
            ClipTool.out_raster = outRaster.FullName
            If template IsNot Nothing Then
                ClipTool.in_template_dataset = template.FullName
            End If

            ClipTool.nodata_value = 0
            ClipTool.clipping_geometry = clipGeometry

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(ClipTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster.FullName
                ex.Data("rectangle") = rectangle
                ex.Data("outRaster") = outRaster.FullName
                ex.Data("template") = template.FullName
                ex.Data("noData") = noData.ToString
                ex.Data("clipGeometry") = clipGeometry.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub Clip_management(ByVal inRaster As String,
                                   ByVal rectangle As String,
                                   ByVal outRaster As String,
                                   Optional ByVal template As String = Nothing,
                                   Optional ByVal noData As Integer = 9999,
                                   Optional ByVal clipGeometry As Boolean = True)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("in_raster is null or empty string.")
            End If

            If String.IsNullOrEmpty(rectangle) Then
                Throw New Exception("rectangle is null or empty string.")
            End If

            If String.IsNullOrEmpty(outRaster) Then
                Throw New Exception("out_raster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim ClipTool As New Clip
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            ClipTool.in_raster = inRaster
            ClipTool.in_template_dataset = template
            ClipTool.rectangle = rectangle
            ClipTool.out_raster = outRaster
            ClipTool.nodata_value = noData
            ClipTool.clipping_geometry = clipGeometry

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(ClipTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("rectangle") = rectangle
                ex.Data("outRaster") = outRaster
                ex.Data("template") = template
                ex.Data("noData") = noData.ToString
                ex.Data("clipGeometry") = clipGeometry.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' Append two datasets together
        ''' </summary>
        ''' <param name="sInputDataset">The dataset that will get added to the target dataset</param>
        ''' <param name="sTargetDataset">The recipient dataset that will receive the data from the input dataset</param>
        ''' <param name="bTestSchema">If True then the schema of the input and target datasets must match</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000050000000
        ''' License Levels: All</remarks>
        Public Sub Append(ByVal sInputDataset As String,
                                 ByVal sTargetDataset As String,
                                 Optional ByVal bTestSchema As Boolean = True)

            If String.IsNullOrEmpty(sInputDataset) Then
                Throw New ArgumentNullException("sInputDataset", "Input dataset does not exist.")
            End If

            If String.IsNullOrEmpty(sTargetDataset) Then
                Throw New ArgumentNullException("sTargetDataset", "Target dataset does not exist.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim AppendTool As New Append
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            AppendTool.inputs = sInputDataset
            AppendTool.target = sTargetDataset

            If bTestSchema Then
                AppendTool.schema_type = "TEST"
            Else
                AppendTool.schema_type = "NO_TEST"
            End If

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(AppendTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

            Catch ex As Exception
                ex.Data("Input dataset") = sInputDataset.ToString
                ex.Data("Target dataset") = sTargetDataset.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="outDatasetPath"></param>
        ''' <param name="outName"></param>
        ''' <param name="spatialReference"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//0017000000pv000000
        ''' License Levels: All</remarks>
        Public Sub CreateFeatureDataset(ByVal outDatasetPath As IWorkspace,
                                        ByVal outName As String,
                                        ByVal spatialReference As ISpatialReference)

            If Not TypeOf outDatasetPath Is IWorkspace Then
                Throw New Exception("Invalid out dataset path.")
            End If

            If String.IsNullOrEmpty(outName) Then
                Throw New Exception("out_name is null or empty string.")
            End If

            If Not TypeOf spatialReference Is ISpatialReference Then
                Throw New Exception("Invalid spatial reference.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim CreateFeatureDataset As New CreateFeatureDataset
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CreateFeatureDataset.out_dataset_path = outDatasetPath
            CreateFeatureDataset.out_name = outName
            CreateFeatureDataset.spatial_reference = spatialReference

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CreateFeatureDataset, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("outDatasetPath") = outDatasetPath.ToString
                ex.Data("outName") = outName
                ex.Data("spatialReference") = spatialReference.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inPoints"></param>
        ''' <param name="outFeatures"></param>
        ''' <param name="distance"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00700000002s000000
        ''' License Levels: ArcInfo Only</remarks>
        Public Sub AggregatePoints(ByVal inPoints As String,
                                   ByVal outFeatures As FileInfo,
                                   ByVal distance As String)

            If String.IsNullOrEmpty(inPoints) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If Not TypeOf outFeatures Is FileInfo Then
                Throw New Exception("Invalid output FeatureClass")
            End If

            If String.IsNullOrEmpty(distance) Then
                Throw New Exception("distance is null or empty string.")
            End If


            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim AggregatePoints As New AggregatePoints
            Dim bAddToMap As Boolean = GP.AddOutputsToMap

            AggregatePoints.in_features = inPoints
            AggregatePoints.out_feature_class = outFeatures.FullName
            AggregatePoints.aggregation_distance = distance

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(AggregatePoints, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

            Catch ex As Exception
                ex.Data("inPoints") = inPoints
                ex.Data("outFeatures") = outFeatures.FullName
                ex.Data("distance") = distance
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddToMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inJoinLayer"></param>
        ''' <param name="inJoinField"></param>
        ''' <param name="joinTable"></param>
        ''' <param name="joinField"></param>
        ''' <param name="joinType"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000064000000
        ''' License Level: All</remarks>
        Public Sub AddJoin_management(ByVal inJoinLayer As String,
                                      ByVal inJoinField As String,
                                      ByVal joinTable As String,
                                      ByVal joinField As String,
                                      Optional ByVal joinType As Boolean = False)

            If String.IsNullOrEmpty(inJoinLayer) Then
                Throw New Exception("injoin is null or empty string.")
            End If

            If String.IsNullOrEmpty(inJoinField) Then
                Throw New Exception("injoinfield is null or empty string.")
            End If

            If String.IsNullOrEmpty(joinTable) Then
                Throw New Exception("jointable is null or empty string.")
            End If

            If String.IsNullOrEmpty(joinField) Then
                Throw New Exception("joinfield is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim JoinTool As New AddJoin
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            JoinTool.in_layer_or_view = inJoinLayer
            JoinTool.in_field = inJoinField
            JoinTool.join_table = joinTable
            JoinTool.join_field = joinField
            JoinTool.join_type = joinType

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(JoinTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inJoinLayer") = inJoinLayer
                ex.Data("inJoinField") = inJoinField
                ex.Data("joinTable") = joinTable
                ex.Data("joinField") = joinField
                ex.Data("joinType") = joinType.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pFeatureLayer"></param>
        ''' <param name="inJoinField"></param>
        ''' <param name="joinTable"></param>
        ''' <param name="joinField"></param>
        ''' <param name="joinType"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000064000000
        ''' License Level: All</remarks>
        Public Sub AddJoin_management(ByVal pFeatureLayer As IFeatureLayer,
                                      ByVal inJoinField As String,
                                      ByVal joinTable As String,
                                      ByVal joinField As String,
                                      Optional ByVal joinType As Boolean = False)

            If pFeatureLayer Is Nothing Then
                Throw New Exception("injoin is null")
            End If

            If String.IsNullOrEmpty(inJoinField) Then
                Throw New Exception("injoinfield is null or empty string.")
            End If

            If String.IsNullOrEmpty(joinTable) Then
                Throw New Exception("jointable is null or empty string.")
            End If

            If String.IsNullOrEmpty(joinField) Then
                Throw New Exception("joinfield is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim JoinTool As New AddJoin
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            JoinTool.in_layer_or_view = pFeatureLayer
            JoinTool.in_field = inJoinField
            JoinTool.join_table = joinTable
            JoinTool.join_field = joinField
            JoinTool.join_type = joinType

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(JoinTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inJoinLayer") = pFeatureLayer.Name
                ex.Data("inJoinField") = inJoinField
                ex.Data("joinTable") = joinTable
                ex.Data("joinField") = joinField
                ex.Data("joinType") = joinType.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        Public Sub JoinField_management(ByVal inJoinLayer As FileInfo,
                                        ByVal inJoinField As String,
                                        ByVal joinTable As FileInfo,
                                        ByVal joinField As String,
                                        Optional ByVal joinType As Boolean = False)

            If TypeOf inJoinLayer Is FileInfo Then
                If Not inJoinLayer.Exists Then
                    Throw New Exception("Input join data does not exist.")
                End If
            Else
                Throw New Exception("Invalid input join data.")
            End If

            If String.IsNullOrEmpty(inJoinField) Then
                Throw New Exception("injoinfield is null or empty string.")
            End If

            If TypeOf joinTable Is FileInfo Then
                If Not joinTable.Exists Then
                    Throw New Exception("Join table does not exist.")
                End If
            Else
                Throw New Exception("Invalid join table.")
            End If

            If String.IsNullOrEmpty(joinField) Then
                Throw New Exception("joinfield is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim JoinTool As New JoinField
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            JoinTool.in_data = inJoinLayer.FullName
            JoinTool.in_field = inJoinField
            JoinTool.join_table = joinTable.FullName
            JoinTool.join_field = joinField

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(JoinTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inJoinLayer") = inJoinLayer.FullName
                ex.Data("inJoinField") = inJoinField
                ex.Data("joinTable") = joinTable.FullName
                ex.Data("joinField") = joinField
                ex.Data("joinType") = joinType.ToString
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inJoinLayer"></param>
        ''' <param name="inJoinField"></param>
        ''' <param name="joinTable"></param>
        ''' <param name="joinField"></param>
        ''' <param name="field"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000065000000
        ''' License Level: All</remarks>
        Public Sub JoinField(ByVal inJoinLayer As String,
                             ByVal inJoinField As String,
                             ByVal joinTable As String,
                             ByVal joinField As String,
                             Optional ByVal field As String = "Distance")

            If String.IsNullOrEmpty(inJoinLayer) Then
                Throw New Exception("injoin is null or empty string.")
            End If

            If String.IsNullOrEmpty(inJoinField) Then
                Throw New Exception("injoinfield is null or empty string.")
            End If

            If String.IsNullOrEmpty(joinTable) Then
                Throw New Exception("jointable is null or empty string.")
            End If

            If String.IsNullOrEmpty(joinField) Then
                Throw New Exception("joinfield is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim JoinTool As New JoinField
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            JoinTool.in_data = inJoinLayer
            JoinTool.in_field = inJoinField
            JoinTool.join_table = joinTable
            JoinTool.join_field = joinField
            JoinTool.fields = field

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(JoinTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inJoinLayer") = inJoinLayer
                ex.Data("inJoinField") = inJoinField
                ex.Data("joinTable") = joinTable
                ex.Data("joinField") = joinField
                ex.Data("field") = field
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <param name="pointLocation"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000003p000000
        ''' License Level: ArcInfo</remarks>
        Public Sub FeatureVerticesToPoints(ByVal inFeatures As FileInfo,
                                           ByVal outFeatures As FileInfo,
                                           Optional ByVal pointLocation As String = "ALL")

            If TypeOf inFeatures Is FileInfo Then
                If Not inFeatures.Exists Then
                    Throw New Exception("Input features do not exist.")
                End If
            Else
                Throw New Exception("Invalid input FeatureClass")
            End If

            If Not TypeOf outFeatures Is FileInfo Then
                Throw New Exception("Invalid output FeatureClass")
            End If

            Dim GP As New Geoprocessor
            Dim FeatureVerticesToPointsTool As New FeatureVerticesToPoints
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FeatureVerticesToPointsTool.in_features = inFeatures.FullName
            FeatureVerticesToPointsTool.out_feature_class = outFeatures.FullName
            FeatureVerticesToPointsTool.point_location = pointLocation

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(FeatureVerticesToPointsTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures.FullName
                ex.Data("outFeatures") = outFeatures.FullName
                ex.Data("VARABLE") = pointLocation
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <param name="tolerance"></param>
        ''' <param name="algorithm"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//007000000012000000
        ''' License Levels: ArcEditor, ArcInfo</remarks>
        Public Sub SmoothLine(ByVal inFeatures As FileInfo,
                              ByVal outFeatures As FileInfo,
                              ByVal tolerance As String,
                              Optional ByVal algorithm As String = "PAEK")

            If TypeOf inFeatures Is FileInfo Then
                If Not inFeatures.Exists Then
                    Throw New Exception("Input features do not exist.")
                End If
            Else
                Throw New Exception("Invalid input FeatureClass")
            End If

            If Not TypeOf outFeatures Is FileInfo Then
                Throw New Exception("Invalid ouput FeatureClass")
            End If

            If String.IsNullOrEmpty(tolerance) Then
                Throw New Exception("Tolerance is null or empty string.")
            End If


            Dim GP As New Geoprocessor
            Dim SmoothLineTool As New SmoothLine
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            SmoothLineTool.in_features = inFeatures.FullName
            SmoothLineTool.out_feature_class = outFeatures.FullName
            SmoothLineTool.tolerance = tolerance
            SmoothLineTool.algorithm = algorithm

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(SmoothLineTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures.FullName
                ex.Data("outFeatures") = outFeatures.FullName
                ex.Data("tolerance") = tolerance
                ex.Data("algorithm") = algorithm
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub SmoothLine(ByVal inFeatures As String,
                              ByVal outFeatures As String,
                              ByVal tolerance As String,
                              Optional ByVal algorithm As String = "PAEK")

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("out_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(tolerance) Then
                Throw New Exception("Tolerance is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim SmoothLineTool As New SmoothLine
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            SmoothLineTool.in_features = inFeatures
            SmoothLineTool.out_feature_class = outFeatures
            SmoothLineTool.tolerance = tolerance
            SmoothLineTool.algorithm = algorithm

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(SmoothLineTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ex.Data("tolerance") = tolerance
                ex.Data("algorithm") = algorithm
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub FeatureVerticesToPoints(ByVal inFeatures As String,
                                           ByVal outFC As String,
                                           Optional ByVal pointLocation As String = "BOTH_ENDS")

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFC) Then
                Throw New Exception("out_fc is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim FeatureVertToPoints As New FeatureVerticesToPoints
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FeatureVertToPoints.in_features = inFeatures
            FeatureVertToPoints.out_feature_class = outFC
            FeatureVertToPoints.point_location = pointLocation

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()


            Try
                GP.Execute(FeatureVertToPoints, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFC") = outFC
                ex.Data("pointLocation") = pointLocation
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub FeatureVerticesToPoints(ByVal inFeatures As IFeatureClass,
                                ByVal outFC As String,
                                Optional ByVal pointLocation As String = "BOTH_ENDS")

            If Not TypeOf (inFeatures) Is FeatureClass Then
                Throw New Exception("in_features is not a featureclass.")
            End If

            If inFeatures Is Nothing Then
                Throw New Exception("in_features is nothing.")
            End If


            If String.IsNullOrEmpty(outFC) Then
                Throw New Exception("out_fc is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim FeatureVertToPoints As New FeatureVerticesToPoints
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            FeatureVertToPoints.in_features = inFeatures
            FeatureVertToPoints.out_feature_class = outFC
            FeatureVertToPoints.point_location = pointLocation

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()


            Try
                GP.Execute(FeatureVertToPoints, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFC") = outFC
                ex.Data("pointLocation") = pointLocation
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000003s000000
        ''' License Levels: All</remarks>
        Public Sub PointsToLine(ByVal inFeatures As String,
                                ByVal outFeatures As String)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("out_features is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim PointsToLine As New PointsToLine
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            PointsToLine.Input_Features = inFeatures
            PointsToLine.Output_Feature_Class = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(PointsToLine, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <param name="bNeighbourhoodOption">True outputs multiple polygons. False outputs a single polygon</param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000003t000000
        ''' License Level: ArcInfo
        ''' PGB 4th December 2012. This method is used in several places including the Thiessen routine in Centerline. Originally
        ''' the "neighbourhood" option was not being set. This worked for the centerlines because it outputs may polygons and tracks
        ''' which points are left and right (needed for the centerline bank identification). Nick added the False option on 25 Oct
        ''' 2012 which stopped the centerline routine from working. This option is now set as an argument so different code can
        ''' control how to make this option work.</remarks>
        Public Sub PolygonToLine(ByVal inFeatures As String,
                                 ByVal outFeatures As String,
                                 ByVal bNeighbourhoodOption As Boolean)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("out_features is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim PolygonToLine As New PolygonToLine
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            PolygonToLine.in_features = inFeatures
            PolygonToLine.neighbor_option = bNeighbourhoodOption 'if False this will create a single polyline feature as opposed to many
            PolygonToLine.out_feature_class = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(PolygonToLine, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000055000000
        ''' License Level: All</remarks>
        Public Sub Merge(ByVal inFeatures As String,
                         ByVal outFeatures As String)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("out_features is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim Merge As New Merge
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Merge.inputs = inFeatures
            Merge.output = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Merge, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <param name="max"></param>
        ''' <param name="min"></param>
        ''' <remarks>License Level: ArcInfo</remarks>
        Public Sub CollapseDualLinestoCenterline(ByVal inFeatures As String,
                                                 ByVal outFeatures As String,
                                                 ByVal max As String,
                                                 Optional ByVal min As String = "0")

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("out_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(max) Then
                Throw New Exception("max is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim Centerline As New CollapseDualLinesToCenterline
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Centerline.in_features = inFeatures
            Centerline.out_feature_class = outFeatures
            Centerline.maximum_width = max
            Centerline.minimum_width = min

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Centerline, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ex.Data("max") = max
                ex.Data("min") = min
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000003r000000
        ''' License Level: All</remarks>
        Public Sub MultipartToSinglePart(ByVal inFeatures As String,
                                         ByVal outFeatures As String)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("out_features is null or empty string.")
            End If

            Debug.Assert(Not String.IsNullOrEmpty(inFeatures))
            Debug.Assert(Not String.IsNullOrEmpty(outFeatures))

            Dim GP As New Geoprocessor
            Dim MultiToSingle As New MultipartToSinglepart
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            MultiToSingle.in_features = inFeatures
            MultiToSingle.out_feature_class = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(MultiToSingle, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inLayer"></param>
        ''' <param name="overlapType"></param>
        ''' <param name="selectFeatures"></param>
        ''' <param name="searchDistance"></param>
        ''' <param name="type"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000072000000
        ''' License Level: All</remarks>
        Public Sub SelectLayerByLocation(ByVal inLayer As String, _
                                                 Optional ByVal overlapType As String = Nothing, _
                                                 Optional ByVal selectFeatures As String = Nothing, _
                                                 Optional ByVal searchDistance As String = Nothing, _
                                                 Optional ByVal type As String = Nothing)

            If String.IsNullOrEmpty(inLayer) Then
                Throw New Exception("in_layer is null or empty string.")
            End If

            Debug.Assert(Not String.IsNullOrEmpty(inLayer))

            Dim GP As New Geoprocessor
            Dim SelectLayer As New SelectLayerByLocation
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            SelectLayer.in_layer = inLayer
            SelectLayer.overlap_type = overlapType
            SelectLayer.select_features = selectFeatures
            SelectLayer.search_distance = searchDistance
            SelectLayer.selection_type = type

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(SelectLayer, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inLayer") = inLayer
                ex.Data("overlapType") = overlapType
                ex.Data("selectFeatures") = selectFeatures
                ex.Data("searchDistance") = searchDistance
                ex.Data("type") = type
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000036000000
        ''' License Level: All</remarks>
        Public Sub DeleteFeatures(ByVal inFeatures As String)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            Debug.Assert(Not String.IsNullOrEmpty(inFeatures))

            Dim GP As New Geoprocessor
            Dim Delete As New DeleteFeatures
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Delete.in_features = inFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Delete, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pFL"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001700000036000000
        ''' License Level: All</remarks>
        Public Sub DeleteFeatures(ByVal pFL As IFeatureLayer)

            If Not TypeOf pFL Is IFeatureLayer Then
                Throw New Exception("Argument feature layer not defined.")
            End If

            Dim GP As New Geoprocessor
            Dim Delete As New DeleteFeatures
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Delete.in_features = pFL

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(Delete, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = pFL.Name
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000005n000000
        ''' License Level: All</remarks>
        Public Sub Dissolve(ByVal inFeatures As String,
                            ByVal outFeatures As String)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("sInFeatures is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("sOutFeatures is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim Dissolve As New Dissolve
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Dissolve.in_features = inFeatures
            Dissolve.out_feature_class = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.ClearMessages()

            Try
                GP.AddOutputsToMap = False
                GP.TemporaryMapLayers = False
                GP.Execute(Dissolve, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000005n000000
        ''' Implements all features of Dissolve
        ''' Original Dissolve method was left intact for backwards compatability
        ''' FP April 17 2013
        ''' dissolve_field and statistics_fields have not been tested
        ''' License Level: All</remarks>
        Public Sub Dissolve(ByVal inFeatures As String,
                            ByVal outFeatures As String,
                            ByVal multi_part As DissolveMultiPartEnum,
                            ByVal unsplit_lines As DissolveUnsplitLinesEnum,
                            Optional ByVal dissolve_field As String = "",
                            Optional ByVal statistics_fields As String = "")

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("sInFeatures is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("sOutFeatures is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim DissolveTool As New Dissolve
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            DissolveTool.in_features = inFeatures
            DissolveTool.out_feature_class = outFeatures
            If multi_part = DissolveMultiPartEnum.MULTI_PART Then
                DissolveTool.multi_part = True
            Else
                DissolveTool.multi_part = False
            End If
            If unsplit_lines = DissolveUnsplitLinesEnum.UNSPLIT_LINES Then
                DissolveTool.unsplit_lines = True
            Else
                DissolveTool.unsplit_lines = False
            End If
            If Not String.IsNullOrEmpty(dissolve_field) Then
                DissolveTool.dissolve_field = dissolve_field
            End If
            If Not String.IsNullOrEmpty(statistics_fields) Then
                DissolveTool.statistics_fields = statistics_fields
            End If

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.ClearMessages()

            Try
                GP.AddOutputsToMap = False
                GP.TemporaryMapLayers = False
                GP.Execute(DissolveTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ex.Data("multi_part") = multi_part
                ex.Data("unsplit_lines") = unsplit_lines
                ex.Data("dissolve_field") = dissolve_field
                ex.Data("statistics_fields") = statistics_fields
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <param name="condition"></param>
        ''' <param name="area"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000005q000000
        ''' License Level: ArcInfo</remarks>
        Public Sub EliminatePolygonPart(ByVal inFeatures As String,
                                        ByVal outFeatures As String,
                                        ByVal condition As String,
                                        ByVal area As String)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("sInFeatures is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("sOutFeatures is null or empty string.")
            End If

            If String.IsNullOrEmpty(condition) Then
                Throw New Exception("condition is null or empty string.")
            End If

            If String.IsNullOrEmpty(area) Then
                Throw New Exception("area is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim Eliminate As New EliminatePolygonPart
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            Eliminate.in_features = inFeatures
            Eliminate.out_feature_class = outFeatures
            Eliminate.condition = condition
            Eliminate.part_area = area

            GP.AddOutputsToMap = False
            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.ClearMessages()

            Try
                GP.Execute(Eliminate, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ex.Data("condition") = condition
                ex.Data("area") = area
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' Permanently deletes data from disk
        ''' </summary>
        ''' <param name="inData"></param>
        ''' <remarks>
        ''' http://help.arcgis.com/en/sdk/10.0/arcobjects_net/componenthelp/index.html#/Overview/004700003p09000000/
        ''' All types of geographic data supported by ArcGIS, as well as toolboxes and workspaces (folders, geodatabases), can be deleted. If the specified item is a workspace, all contained items are also deleted.
        ''' REMOVES LOCKS - Frank
        ''' Note that is the file doesnt exists, it returns a COM error
        ''' License Level: All</remarks>
        Public Sub Delete(ByVal inData As String)

            If String.IsNullOrEmpty(inData) Then
                Throw New Exception("inData is null or empty string.")
            End If

            'GP fails if raster does not exists
            'if it doesnt exists, there is no need to delete it
            If Not GISDataStructures.Raster.Exists(inData) Then
                If Not GISDataStructures.VectorDataSource.Exists(inData) Then
                    Exit Sub
                End If
            End If

            Dim GP As New Geoprocessor
            Dim DeleteTool As New Delete
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            DeleteTool.in_data = inData

            'GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(DeleteTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inData") = inData
                Dim trimmedfilename As String = FileSystem.TrimFilename(inData, 80)
                ex.Data("UIMessage") = "Could not delete '" & trimmedfilename & "'."
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Permanently deletes data from disk
        ''' </summary>
        ''' <param name="inData">ESRI dataset... Table, feature class raster, workspace etc</param>
        ''' <param name="eType">Enumeration defining what type of data is being deleted</param>
        ''' <remarks>PGB 1 May 2012: The geoprocessing call to "delete" will fail if the dataset does
        ''' not exist. Therefore, this method will check if the dataset exists first. This is done
        ''' using the custom "Exists" method. Even though this method is stored in the FeatureClass
        ''' namespace it can also check if rasters and tables etc exist.
        ''' License Level: All</remarks>
        Public Sub Delete(ByVal inData As String, ByVal eType As esriDatasetType)

            If String.IsNullOrEmpty(inData) Then
                Throw New Exception("inData is null or empty string.")
            End If
            '
            ' GP fails if raster does not exists.
            ' if it doesnt exists, there is no need to delete it.
            '
            If eType = esriDatasetType.esriDTRasterDataset Then
                If Not GISDataStructures.Raster.Exists(inData) Then
                    Exit Sub
                End If
            Else
                If GISDataStructures.IsFileGeodatabase(inData) Then
                    If Not GISDataStructures.VectorDataSource.Exists(inData) Then
                        Exit Sub
                    End If
                Else
                    If Not GISDataStructures.VectorDataSource.Exists(inData) Then
                        Exit Sub
                    End If
                End If
            End If

            Dim GP As New Geoprocessor
            Dim DeleteTool As New Delete
            Dim bAddToMap As Boolean = GP.AddOutputsToMap

            DeleteTool.in_data = inData

            'GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(DeleteTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inData") = inData
                Dim trimmedfilename As String = FileSystem.TrimFilename(inData, 80)
                ex.Data("UIMessage") = "Could not delete '" & trimmedfilename & "'."
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddToMap
            End Try

        End Sub

        ''' <summary>
        ''' Implementation of datamanagement copy geoprocessing
        ''' </summary>
        ''' <param name="in_data"></param>
        ''' <param name="out_data"></param>
        ''' <remarks>
        ''' http://help.arcgis.com/en/sdk/10.0/arcobjects_net/componenthelp/index.html#/Members/004700002vp3000000/
        ''' License Level: All</remarks>
        Public Sub Copy(ByVal in_data As String,
                      ByVal out_data As String)

            If String.IsNullOrEmpty(in_data) Then
                Throw New Exception("in_data is null or empty string.")
            End If

            If String.IsNullOrEmpty(out_data) Then
                Throw New Exception("out_data is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim CopyTool As New ESRI.ArcGIS.DataManagementTools.Copy
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CopyTool.in_data = in_data
            CopyTool.out_data = out_data

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(CopyTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("in_data") = in_data
                ex.Data("out_data") = out_data
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' Calculate statistics for a raster
        ''' </summary>
        ''' <param name="inRaster"></param>
        ''' <remarks>PGB 6 Jan 2012. Taken from Detrending class and moved here for
        ''' consistency.
        ''' License Level: All</remarks>
        Public Sub CalculateStatistics(ByVal inRaster As String)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("inRaster is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim CalculateStatisticsTool As New ESRI.ArcGIS.DataManagementTools.CalculateStatistics
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            CalculateStatisticsTool.in_raster_dataset = inRaster
            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(CalculateStatisticsTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        Public Sub DefineProjection(ByVal sInDataset As String, ByVal pSpatialReference As ISpatialReference)

            If String.IsNullOrEmpty(sInDataset) Then
                Throw New Exception("sInDataset is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim pDefineProjection As New ESRI.ArcGIS.DataManagementTools.DefineProjection
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            pDefineProjection.in_dataset = sInDataset
            pDefineProjection.coor_system = pSpatialReference

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(pDefineProjection, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("sInDataset") = sInDataset
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        Public Sub DefineProjection(ByVal pDS As IRasterDataset, ByVal pSpatialReference As ISpatialReference)

            Dim GP As New Geoprocessor
            Dim pDefineProjection As New ESRI.ArcGIS.DataManagementTools.DefineProjection
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            pDefineProjection.in_dataset = pDS
            pDefineProjection.coor_system = pSpatialReference

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(pDefineProjection, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("sInDataset") = pDS.FullName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        Public Sub RepairGeometry(ByVal sInDataset As String)

            If String.IsNullOrEmpty(sInDataset) Then
                Throw New Exception("sInDataset is null or empty string.")
            End If

            Dim GP As New Geoprocessor
            Dim pRepair As New ESRI.ArcGIS.DataManagementTools.RepairGeometry
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            pRepair.in_features = sInDataset

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(pRepair, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("sInDataset") = sInDataset
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' Get a particular property of a raster (min, max etc)
        ''' </summary>
        ''' <param name="sInDataset">Full path to a raster dataset</param>
        ''' <param name="sProperty">One of the specified property strings. See remarks.</param>
        ''' <remarks>
        ''' MAXIMUM—Returns the largest value of all cells in the input raster. 
        ''' MINIMUM—Returns the smallest value of all cells in the input raster. 
        ''' MEAN—Returns the average of all cells in the input raster. 
        ''' STD—Returns the standard deviation of all cells in the input raster. 
        ''' UNIQUEVALUECOUNT—Returns the number of unique values in the input raster. 
        ''' TOP—Returns the top or YMax value of the extent. 
        ''' LEFT—Returns the left or XMin value of the extent. 
        ''' RIGHT—Returns the right or XMax value of the extent. 
        ''' BOTTOM—Returns the bottom or YMin value of the extent. 
        ''' CELLSIZEX—Returns the cell size in the x-direction. 
        ''' CELLSIZEY—Returns the cell size in the y-direction. 
        ''' VALUETYPE—Returns the type of the cell value in the input raster. 
        ''' COLUMNCOUNT—Returns the number of columns in the input raster. 
        ''' ROWCOUNT—Returns the number of rows in the input raster. 
        ''' BANDCOUNT—Returns the number of bands in the input raster. </remarks>
        Public Function GetRasterProperties(ByVal sInDataset As String, ByVal sProperty As String) As Double

            If String.IsNullOrEmpty(sInDataset) Then
                Throw New ArgumentNullException("sInDataset", "The input raster dataset is null or empty")
            End If


            If String.IsNullOrEmpty(sProperty) Then
                Throw New ArgumentNullException("sProperty", "The raster property argument is null or empty")
            End If

            Dim fResult As Double
            Dim GP As New Geoprocessor
            Dim pRasterProp As New ESRI.ArcGIS.DataManagementTools.GetRasterProperties
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            pRasterProp.in_raster = sInDataset
            pRasterProp.property_type = sProperty.ToUpper

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(pRasterProp, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

                fResult = pRasterProp.property

            Catch ex As Exception
                ex.Data("sInDataset") = sInDataset
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return fResult

        End Function

        ''' <summary>
        ''' Get the value of a raster at a particular location
        ''' </summary>
        ''' <param name="sRasterPath">Full path to the raster</param>
        ''' <param name="XCoord">X coordinate as double</param>
        ''' <param name="YCoord">Y coordinate as double</param>
        ''' <param name="fCellValueResult">The value at the location of X and Y. Check the function return for whether there is data at this point</param>
        ''' <returns>True if there is data at the location. False if NoData</returns>
        ''' <remarks></remarks>
        Public Function GetCellValue(ByVal sRasterPath As String, ByVal XCoord As Double, ByVal YCoord As Double, ByRef fCellValueResult As Double) As Boolean

            If String.IsNullOrEmpty(sRasterPath) Then
                Throw New ArgumentNullException("sRasterPath", "The input raster dataset is null or empty")
            End If

            If XCoord <= 0 Then
                Dim ex As New ArgumentNullException("XCoord", "The X coordinate must be greater than 0")
                ex.Data("X Coord") = XCoord
                Throw ex
            End If

            If YCoord <= 0 Then
                Dim ex As New ArgumentNullException("YCoord", "The Y coordinate must be greater than 0")
                ex.Data("Y Coord") = YCoord
                Throw ex
            End If

            Dim bResult As Boolean = False
            Dim GP As New Geoprocessor
            Dim pGetCellValue As New ESRI.ArcGIS.DataManagementTools.GetCellValue
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            pGetCellValue.in_raster = sRasterPath
            pGetCellValue.location_point = XCoord.ToString & " " & YCoord.ToString

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(pGetCellValue, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)

                bResult = Double.TryParse(pGetCellValue.out_string, fCellValueResult)

            Catch ex As Exception
                ex.Data("sRasterPath") = sRasterPath
                ex.Data("X Coord") = XCoord
                ex.Data("Y Coord") = YCoord
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

            Return bResult

        End Function

        ''' <summary>
        ''' Get the value of a raster at a particular location
        ''' </summary>
        ''' <param name="sRasterPath">Full path to the raster</param>
        ''' <param name="fCellValueResult">The value at the location of X and Y. Check the function return for whether there is data at this point</param>
        ''' <returns>True if there is data at the location. False if NoData</returns>
        ''' <remarks></remarks>
        Public Function GetCellValue(ByVal sRasterPath As String, ByVal pPoint As IPoint, ByRef fCellValueResult As Double) As Boolean

            Return GetCellValue(sRasterPath, pPoint.X, pPoint.Y, fCellValueResult)

        End Function

        ''' <summary>
        ''' Splits line at vertices
        ''' </summary>
        ''' <param name="inFeatures"></param>
        ''' <param name="outFeatures"></param>
        ''' <remarks>http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00170000003t000000
        ''' License Level: ArcInfo
        ''' </remarks>
        Public Sub SplitLineAtVertices(ByVal inFeatures As String,
                                 ByVal outFeatures As String)

            If String.IsNullOrEmpty(inFeatures) Then
                Throw New Exception("in_features is null or empty string.")
            End If

            If String.IsNullOrEmpty(outFeatures) Then
                Throw New Exception("out_features is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim SplitlineTool As New SplitLine
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            SplitlineTool.in_features = inFeatures
            SplitlineTool.out_feature_class = outFeatures

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(SplitlineTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inFeatures") = inFeatures
                ex.Data("outFeatures") = outFeatures
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pTable"></param>
        ''' <param name="sFields">Separate multiple fields with semi-colons</param>
        ''' <param name="bUnique"></param>
        ''' <param name="bAscending"></param>
        ''' <param name="sIndexName"></param>
        ''' <remarks></remarks>
        Public Sub AddIndex(ByVal pTable As ITable, ByVal sFields As String, ByVal bUnique As Boolean, Optional ByVal bAscending As Boolean = True, Optional ByVal sIndexName As String = "")

            If pTable Is Nothing Then
                Throw New ArgumentNullException("pTable", "The table cannot be null or empty.")
            End If

            If String.IsNullOrEmpty(sFields) Then
                Throw New ArgumentException("The field to be indexed cannot be null or empty", "sfield")
            Else
                If pTable.FindField(sFields) < 0 Then
                    Throw New ArgumentException("The field to be indexed cannot be found in the table.", "sfield")
                End If
            End If


            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim addIndexTool As New AddIndex
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            addIndexTool.in_table = pTable
            addIndexTool.fields = sFields
            addIndexTool.unique = bUnique
            addIndexTool.ascending = bAscending
            addIndexTool.index_name = sIndexName

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(addIndexTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("Fields") = sFields
                ex.Data("Unique") = bUnique.ToString
                ex.Data("Ascending") = bAscending.ToString
                ex.Data("Index Name") = sIndexName
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' Calculates attribute tables for raster
        ''' </summary>
        ''' <param name="inRaster">input raster to calculate attribute table on</param>
        ''' <param name="overwrite">Indicate if attribute table should be overwritten if it exists</param>
        ''' <remarks>
        ''' http://resources.arcgis.com/en/help/main/10.1/index.html#//0017000000m2000000
        ''' License Level: All
        ''' </remarks>
        Public Sub BuildRasterAttributeTable(ByVal inRaster As String,
                              Optional ByVal overwrite As Boolean = True)

            If String.IsNullOrEmpty(inRaster) Then
                Throw New Exception("inRaster is null or empty string.")
            End If



            Dim GP As New Geoprocessor
            Dim BuildRasterAttributeTableTool As New BuildRasterAttributeTable
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            BuildRasterAttributeTableTool.in_raster = inRaster
            BuildRasterAttributeTableTool.overwrite = overwrite

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(BuildRasterAttributeTableTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("inRaster") = inRaster
                ex.Data("overwrite") = overwrite
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub

        ''' <summary>
        ''' Compacts a personal or file geodatabase. Compacting rearranges how the geodatabase is stored on disk, often reducing its size and improving performance.
        ''' </summary>
        ''' <param name="inGeodatabase">Full, absolute path to a file geodatabase</param>
        ''' <remarks>
        ''' http://resources.arcgis.com/en/help/main/10.1/index.html#//0017000000m2000000
        ''' License Level: All
        ''' </remarks>
        Public Sub Compact(ByVal inGeodatabase As String)

            If String.IsNullOrEmpty(inGeodatabase) Then
                Throw New Exception("Geodatabase is null or empty string.")
            Else
                If System.IO.Directory.Exists(inGeodatabase) Then
                    If Not inGeodatabase.ToLower.EndsWith(".gdb") Then
                        Dim ex As New Exception("Path does not appear to be a valid file geodatabase.")
                        ex.Data("Path") = inGeodatabase
                        Throw ex
                    End If
                Else
                    Dim ex As New Exception("File geodatabase does not exist.")
                    ex.Data("Path") = inGeodatabase
                    Throw ex
                End If
            End If

            Dim GP As New Geoprocessor
            Dim compactTool As New Compact
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            compactTool.in_workspace = inGeodatabase

            GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False
            GP.ClearMessages()

            Try
                GP.Execute(compactTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data("Geodatabase") = inGeodatabase
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try
        End Sub
    End Module

End Namespace