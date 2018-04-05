Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.DataSourcesFile
Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.CatalogUI
Imports ESRI.ArcGIS.Catalog
Imports ESRI.ArcGIS.Framework
Imports GCDAddIn.GISCode.ErrorHandling.ExceptionUI
Namespace GISCode.GISDataStructures

    ''' <summary>
    ''' Basic GIS data geometry types
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum GeometryTypes
        Point
        Line
        Polygon
    End Enum

    ''' <summary>
    ''' Represents any existing point, line or polygon vector GIS data source
    ''' </summary>
    ''' <remarks></remarks>
    Public Class VectorDataSource
        Inherits GISCode.GISDataStructures.GISDataSource

#Region "Members"

        Private m_pFeatureClass As IFeatureClass

#End Region

#Region "Properties"

        ''' <summary>
        ''' The feature class representing the data source
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FeatureClass As IFeatureClass
            Get
                Return m_pFeatureClass
            End Get
        End Property

        ''' <summary>
        ''' The shape type (point, line or polygon) of the data source
        ''' </summary>
        ''' <value></value>
        ''' <returns>Point, line or polygon</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GeometryType As GeometryTypes
            Get
                Dim eType As GeometryTypes
                Select Case m_pFeatureClass.ShapeType
                    Case esriGeometryType.esriGeometryLine, esriGeometryType.esriGeometryPolyline : eType = GeometryTypes.Line
                    Case esriGeometryType.esriGeometryPoint, esriGeometryType.esriGeometryMultipoint : eType = GeometryTypes.Point
                    Case esriGeometryType.esriGeometryPolygon : eType = GeometryTypes.Polygon
                    Case Else
                        Dim ex As New GISException(GISException.ErrorTypes.CriticalError, "Invalid feature class shape type")
                        ex.Data("Path") = FullPath
                        ex.Data("Actual shape type") = m_pFeatureClass.ShapeType.ToString
                        Throw ex
                End Select
                Return eType
            End Get
        End Property

        ''' <summary>
        ''' The field name containing the unique feature/object ID
        ''' </summary>
        ''' <value></value>
        ''' <returns>The field name containing the unique feature/object ID</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property OIDFieldName As String
            Get
                Return m_pFeatureClass.OIDFieldName
            End Get
        End Property

        ''' <summary>
        ''' The index of the object/feature ID field
        ''' </summary>
        ''' <value></value>
        ''' <returns>The index of the object/feature ID field</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property OIDFieldIndex As Integer
            Get
                If m_pFeatureClass.HasOID Then
                    Return m_pFeatureClass.FindField(m_pFeatureClass.OIDFieldName)
                Else
                    Return -1
                End If
            End Get
        End Property

        ''' <summary>
        ''' The total number of features in the feature class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FeatureCount As Integer
            Get
                Return m_pFeatureClass.FeatureCount(Nothing)
            End Get
        End Property

        ''' <summary>
        ''' True if the feature class is 3D and has Z values
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Is3D As Boolean
            Get
                Dim pField As IField = FeatureClass.Fields.Field(FeatureClass.FindField(FeatureClass.ShapeFieldName))
                Return pField.GeometryDef.HasZ
            End Get
        End Property

        ''' <summary>
        ''' Test if a feature class is a Cross Section store.
        ''' </summary>
        ''' <returns>True if the argument feature class is of the correct format to store cross sections.</returns>
        ''' <remarks>Tests that the argument feature class is:
        ''' a) Polyline
        ''' b) Projected in metres
        ''' c) Has the key fields required to be a cross section store
        ''' </remarks>
        Public ReadOnly Property IsCrossSectionStore As Boolean
            Get
                Try
                    If FeatureClass.ShapeType = esriGeometryType.esriGeometryPolyline Then
                        'If Is3D Then
                        'If IsMetres Then
                        '
                        ' Check for the main cross section fields. Don't go crazy... want to
                        ' make this tolerant of changing fields and most should be considered
                        ' optional. Only check a few important ones to confirm that the
                        ' argument shapefile is indeed a XS store.
                        '
                        If FindField(CrossSectionFields.Length) >= 0 And _
                            FindField(CrossSectionFields.Name) >= 0 And _
                            FindField(CrossSectionFields.StationSeparation) >= 0 Then

                            Return True

                            'End If
                            'End If
                        End If
                    End If
                Catch ex As Exception
                    Dim ex2 As New Exception("Error determining if feature class is a cross section store.", ex)
                    ex2.Data.Add("Feature Class", Me.FullPath)
                    Throw ex2
                End Try
                ' 
                ' If got to here then this isn't a cross section store.
                '
                Return False

            End Get
        End Property

        Public ReadOnly Property GetFirstGeometry As IGeometry
            Get
                Dim pGeom As IGeometry = Nothing
                Dim pCursor As IFeatureCursor = FeatureClass.Search(Nothing, True)
                Dim pFeature As IFeature = pCursor.NextFeature
                If Not pFeature Is Nothing Then
                    pGeom = pFeature.ShapeCopy
                End If
                Runtime.InteropServices.Marshal.ReleaseComObject(pCursor)
                Return pGeom
            End Get
        End Property

        ''' <summary>
        ''' Returns true if there is at least one multi-part feature in the feature class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>I considered having this method return an integer count of
        ''' the multi-part features, but feared that this process could get slow
        ''' for very large feature classes. Therefore, I just did a quick and dirty
        ''' boolean implementation.</remarks>
        Public ReadOnly Property ContainsMultiPartFeatures As Boolean
            Get
                Dim bResult As Boolean = False
                Dim pCursor As IFeatureCursor = FeatureClass.Search(Nothing, True)
                Dim pFeature As IFeature = pCursor.NextFeature
                Do While TypeOf pFeature Is IFeature
                    Dim pGeom As IGeometry = pFeature.ShapeCopy
                    Dim pGeomColl As IGeometryCollection = pGeom
                    If Not pGeomColl Is Nothing AndAlso pGeomColl.GeometryCount > 1 Then
                        bResult = True
                    End If
                    pFeature = pCursor.NextFeature
                Loop
                Runtime.InteropServices.Marshal.ReleaseComObject(pCursor)
                Return bResult
            End Get
        End Property

#End Region

#Region "Constructors and Desctructors"

        Public Sub New(sFullPath As String)
            MyBase.New(sFullPath)

            Dim pFWS As IFeatureWorkspace = TryCast(Workspace, IFeatureWorkspace)
            If TypeOf pFWS Is IFeatureWorkspace Then
                Select Case GISDataStorageType
                    Case GISDataStorageTypes.CAD
                        Dim sFile As String = IO.Path.GetFileName(IO.Path.GetDirectoryName(sFullPath))
                        Dim sFeatures As String = IO.Path.GetFileName(sFullPath)
                        Dim sFC As String = sFile & ":" & sFeatures
                        m_pFeatureClass = pFWS.OpenFeatureClass(sFC)

                    Case Else
                        m_pFeatureClass = pFWS.OpenFeatureClass(IO.Path.GetFileName(sFullPath))
                End Select
                GeoDataset = DirectCast(m_pFeatureClass, IGeoDataset)
            Else
                Throw New GISException(GISException.ErrorTypes.CriticalError, "The workspace is not a feature workspace")
            End If
            'm_pFeatureClass = MyBase.GeoDataset

            'If GeometryType <> eGeometryType Then
            '    Dim ex As New GISException(GISException.ErrorTypes.CriticalError, "Invalid geometry type.")
            '    ex.Data("Feature Class", FullPath)
            '    ex.Data("Actual geometry type", GeometryType)
            '    ex.Data("Expected geometry type", eGeometryType)
            '    Throw ex
            'End If
        End Sub

        Public Sub New(pFeatureClass As IFeatureClass)
            Me.New(GetFeatureClassPath(pFeatureClass))
        End Sub

        Protected Overrides Sub Finalize()
            'If TypeOf m_pFeatureClass Is IFeatureClass Then
            '    Runtime.InteropServices.Marshal.ReleaseComObject(m_pFeatureClass)
            'End If
            MyBase.Finalize()
        End Sub

#End Region

        Public Overrides Function Validate(ByRef lErrors As List(Of ErrorHandling.GISException), gReferenceSR As ISpatialReference) As Boolean
            MyBase.Validate(lErrors, gReferenceSR)

            Try
                If FeatureCount < 1 Then
                    Throw New ErrorHandling.GISException(ErrorHandling.GISException.ErrorTypes.Warning, "Empty feature class. There are no features in this feature class.", "Add features to " & FullPath)
                End If

            Catch ex As ErrorHandling.GISException
                ex.Data("Vector Data Source Path") = FullPath
                lErrors.Add(ex)
            End Try

            Return lErrors.Count = 0

        End Function

        Public Function FeatureCountWhere(sWhereClause As String) As Integer

            If String.IsNullOrEmpty(sWhereClause) Then
                'Dim ex As New Exception("The where clause cannot be blank. Use the property ""FeatureCount"" for the total number of features.")
                'ex.Data("Path", FullPath)
                'Throw ex
                Return FeatureCount
            Else
                Dim pQry As IQueryFilter = New QueryFilter
                pQry.WhereClause = sWhereClause
                Return m_pFeatureClass.FeatureCount(pQry)
            End If

        End Function

        Public Function FindField(sFieldName As String) As Integer
            If String.IsNullOrEmpty(sFieldName) Then
                Throw New ArgumentNullException(sFieldName, "The field name cannot be null or empty")
            End If

            Return m_pFeatureClass.FindField(sFieldName)
        End Function

        Public Function GetGeometries(sWhereClause As String, ByRef lGeometries As List(Of IGeometry)) As Integer

            If lGeometries Is Nothing Then
                lGeometries = New List(Of IGeometry)
            End If

            Dim pQry As ESRI.ArcGIS.Geodatabase.IQueryFilter = Nothing
            If Not String.IsNullOrEmpty(sWhereClause) Then
                pQry = New ESRI.ArcGIS.Geodatabase.QueryFilter
                pQry.WhereClause = sWhereClause
            End If

            Dim nCount As Integer = 0
            Dim pFCursor As IFeatureCursor = Nothing
            Try
                pFCursor = FeatureClass.Search(pQry, True)
                Dim pFeature As IFeature = pFCursor.NextFeature
                Do While TypeOf pFeature Is IFeature
                    nCount += 1
                    lGeometries.Add(pFeature.ShapeCopy)
                    pFeature = pFCursor.NextFeature
                Loop
            Catch ex As Exception
                ex.Data("Feature class") = FullPath
                ex.Data("Where Clause") = sWhereClause
                Throw
            Finally
                If TypeOf pFCursor Is IFeatureCursor Then
                    Runtime.InteropServices.Marshal.ReleaseComObject(pFCursor)
                    pFCursor = Nothing
                End If
            End Try

            Return nCount

        End Function

        ''' <summary>
        ''' Given an existing feature class path, or directory, this function returns a new feature class path in the same workspace with a unique, unused, name.
        ''' </summary>
        ''' <param name="sInputPath">Fully qualified feature class path on which to base</param>
        ''' <param name="sRootName">The root name for the new feature class. If blank then the input path feature class name is used.</param>
        ''' <param name="nMax">The Maximum permissible length of the feature class name. Leave blank or pass zero for unlimited length.</param>
        ''' <returns></returns>
        ''' <remarks>If the input is a file geodatabase then the dataset name is simply the name. If the
        ''' input is a shapefile then the dataset name includes the ".shp" extension.
        ''' PGB 28 Aug 2013 - new approach to opening workspace factory</remarks>
        Public Shared Function GetNewSafeName(sInputPath As String, Optional sRootName As String = "", Optional nMax As Byte = 0) As String

            Dim bFileGDB As Boolean = GISDataStructures.IsFileGeodatabase(sInputPath)

            Dim wsFact As IWorkspaceFactory2
            Dim sWorkspacePath As String
            Dim sFDSName As String = Nothing
            Dim nLastSlash As Integer = sInputPath.LastIndexOf(IO.Path.DirectorySeparatorChar)
            Dim sExtension As String = "shp" 'IO.Path.GetExtension(sInputPath)
            Dim sOriginalDSName As String

            If bFileGDB Then
                Dim aType As Type = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory")
                Dim obj As System.Object = Activator.CreateInstance(aType)
                wsFact = obj

                Dim i As Integer = sInputPath.IndexOf(".gdb")
                sWorkspacePath = sInputPath.Substring(0, i + 4)
                '
                ' The next few lines were to get the FeatureDataset name in FGDB, but are no longer needed.
                '
                'If nLastSlash >= 0 Then
                '    sFDSName = sInputPath.Substring(i + 5, nLastSlash - (i + 5))
                'End If

                sOriginalDSName = sInputPath.Substring(nLastSlash + 1)
            Else
                '
                ' PGB 18 Aug 2011 - sInputPath can be a folder **or** a full path to an existing shapefile. Therefore,
                ' we need to call GetDirectoryName(), but if sInputPath is already just a directory, then it will strip
                ' the last directory name off the end. So determine if sInputPath is a directory.
                '
                Dim sInputDir As New IO.DirectoryInfo(sInputPath)
                ' Dim f As New FileInfo(sExtension)

                Dim aType As Type = Type.GetTypeFromProgID("esriDataSourcesFile.ShapefileWorkspaceFactory")
                Dim obj As System.Object = Activator.CreateInstance(aType)
                wsFact = obj

                If sInputDir.Exists Then
                    sWorkspacePath = sInputPath
                Else
                    sWorkspacePath = IO.Path.GetDirectoryName(sInputPath)
                End If
                sOriginalDSName = IO.Path.GetFileNameWithoutExtension(sInputPath)
            End If

            'changed because shapfileworkspace doesnt support IWorkspace2 - Frank
            Dim pWS As IWorkspace = wsFact.OpenFromFile(sWorkspacePath, 0)
            Dim nCount As Integer = 0
            Dim sNewDSName As String

            Do
                If String.IsNullOrEmpty(sRootName) Then
                    If String.IsNullOrEmpty(sOriginalDSName) Then
                        sNewDSName = "ds"
                    Else
                        sNewDSName = sOriginalDSName
                    End If
                Else
                    sNewDSName = IO.Path.GetFileNameWithoutExtension(sRootName)
                End If
                sNewDSName = FileSystem.RemoveDangerousCharacters(sNewDSName)

                If nCount > 0 Then
                    sNewDSName &= nCount.ToString()
                End If

                If Not bFileGDB Then
                    If Not String.IsNullOrEmpty(sExtension) Then
                        sNewDSName = IO.Path.ChangeExtension(sNewDSName, sExtension)
                        'sNewDSName &= sExtension
                    End If
                End If

                nCount += 1

                'changed because shapfileworkspace doesnt support IWorkspace2 - Frank
                'Loop While pWS.NameExists(esriDatasetType.esriDTFeatureClass, sNewDSName) AndAlso nCount < 9999
            Loop While NameExistsInWorkspace(pWS, esriDatasetType.esriDTFeatureClass, sNewDSName) AndAlso nCount < 9999

            Dim sOutputPath As String = sWorkspacePath
            If Not bFileGDB Then
                If Not String.IsNullOrEmpty(sFDSName) Then
                    sOutputPath = IO.Path.Combine(sOutputPath, sFDSName)
                End If
            End If
            sOutputPath = IO.Path.Combine(sOutputPath, sNewDSName)

            Return sOutputPath

        End Function

        Public Overridable Function Exists() As Boolean
            Return Exists(Me.FullPath)
        End Function

        ''' <summary>
        ''' Determines if a feature class exists already
        ''' </summary>
        ''' <param name="sPath">Full path to the feature class</param>
        ''' <param name="eType">Desired geometry type. Defaults to "Any"</param>
        ''' <returns>True if feature class exists or otherwise false.</returns>
        ''' <remarks></remarks>
        Public Shared Function Exists(sPath As String, Optional eType As esriGeometryType = esriGeometryType.esriGeometryAny) As Boolean

            Dim bResult As Boolean = False

            If String.IsNullOrEmpty(sPath) Then
                ' Throw New ArgumentNullException("sPath", "Invalid or NULL feature class full path")
                Return False
            End If

            Try
                Dim eStorageType As GISDataStorageTypes
                If GISDataStructures.IsFileGeodatabase(sPath) Then
                    eStorageType = GISDataStorageTypes.FileGeodatase
                Else
                    eStorageType = GISDataStorageTypes.ShapeFile
                End If

                Dim pWS As IWorkspace = GISDataStructures.GetWorkspace(sPath, eStorageType) '  Workspace(sPath)
                If Not TypeOf pWS Is IFeatureWorkspace Then
                    Return False
                End If
                Dim pFWS As IFeatureWorkspace = pWS
                '
                ' PGB 22 Nov 2012
                ' The IWorkspace2.Exists() method seems really unreliable. Unfortunately we have to loop to get 
                ' a reliable result
                '
                Dim pDSNames As IEnumDatasetName = pWS.DatasetNames(esriDatasetType.esriDTFeatureClass)
                Dim pDSN As IDatasetName = pDSNames.Next
                Do While TypeOf pDSN Is IDatasetName
                    If String.Compare(pDSN.Name, IO.Path.GetFileNameWithoutExtension(sPath), True) = 0 OrElse _
                        String.Compare(pDSN.Name, IO.Path.GetFileName(sPath), True) = 0 Then

                        If eType = esriGeometryType.esriGeometryAny Then
                            bResult = True
                        Else
                            Dim pFC As IFeatureClass = pFWS.OpenFeatureClass(pDSN.Name)
                            If pFC.ShapeType = eType Then
                                bResult = True
                                Exit Do
                            Else
                                Dim ex As New Exception("The feature class exists but is of different shape type")
                                ex.Data("Actual type") = pFC.ShapeType.ToString
                                Throw ex
                            End If
                        End If
                    End If
                    pDSN = pDSNames.Next
                Loop
                '
                ' Now check all the feature datasets in the workspace
                '
                Dim pFDSNames As IEnumDatasetName = pWS.DatasetNames(esriDatasetType.esriDTFeatureDataset)
                Dim pFDS As IDatasetName = pFDSNames.Next
                Do While TypeOf pFDS Is IDatasetName AndAlso Not bResult
                    pDSNames = pFDS.SubsetNames
                    pDSN = pDSNames.Next
                    Do While TypeOf pDSN Is IDatasetName
                        If String.Compare(pDSN.Name, IO.Path.GetFileNameWithoutExtension(sPath), True) = 0 OrElse _
                            String.Compare(pDSN.Name, IO.Path.GetFileName(sPath), True) = 0 Then
                            Dim pFC As IFeatureClass = pFWS.OpenFeatureClass(pDSN.Name)
                            If eType = esriGeometryType.esriGeometryAny OrElse pFC.ShapeType = eType Then
                                bResult = True
                                Exit Do
                            Else
                                Dim ex As New Exception("The feature class exists but is of different shape type")
                                ex.Data("Actual type") = pFC.ShapeType.ToString
                                Throw ex
                            End If
                        End If
                        pDSN = pDSNames.Next
                    Loop
                    pFDS = pFDSNames.Next
                Loop

            Catch ex As Exception
                Dim ex2 As New Exception("Error checking the existance of a feature class", ex)
                ex2.Data("Path") = sPath
                ex2.Data("Geometry Type") = eType.ToString
                Throw ex2
            End Try

            Return bResult

        End Function

        ''' <summary>
        ''' Delete all fields from a vector data source except for the shape and OID fields
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub DeleteNonEssentialFields()
            Dim lFieldsToKeep As New List(Of String)
            DeleteNonEssentialFields(lFieldsToKeep)
        End Sub

        ''' <summary>
        ''' Delete all non-essential fields except the ones listed
        ''' </summary>
        ''' <param name="lFieldsToKeep">List of fields that should be kept</param>
        ''' <remarks></remarks>
        Public Sub DeleteNonEssentialFields(ByVal lFieldsToKeep As List(Of String))
            '
            ' Loop over all the fields in the feature class and build a list of the non-essential
            ' field names. Skip the shape field and OID (feature ID) field. Don't actually do the
            ' delete here or you will break the integrity of the Fields collection and therefore 
            ' the loop
            '
            Dim sFieldsToDelete As New List(Of String)
            For i As Integer = 0 To FeatureClass.Fields.FieldCount - 1
                If String.Compare(FeatureClass.Fields.Field(i).Name, FeatureClass.OIDFieldName, True) <> 0 Then
                    If String.Compare(FeatureClass.Fields.Field(i).Name, FeatureClass.ShapeFieldName, True) <> 0 Then
                        If String.Compare(FeatureClass.Fields.Field(i).Name, "Shape_Length", True) <> 0 Then
                            If String.Compare(FeatureClass.Fields.Field(i).Name, "Shape_Area", True) <> 0 Then
                                Dim bMatch As Boolean = False
                                For Each sField As String In lFieldsToKeep
                                    If String.Compare(sField, FeatureClass.Fields.Field(i).Name, True) = 0 Then
                                        bMatch = True
                                    End If
                                Next

                                If Not bMatch Then
                                    If Not sFieldsToDelete.Contains(FeatureClass.Fields.Field(i).Name) Then
                                        sFieldsToDelete.Add(FeatureClass.Fields.Field(i).Name)
                                    End If
                                End If
                            End If
                        End If

                    End If
                End If
            Next

            ' PGB 12 Sep 2014. Note that if the list is empty then all non-essential
            ' fields will be delete. But ShapeFiles require one field other than the OID
            ' and Shape field.
            If Me.GISDataStorageType = GISDataStorageTypes.ShapeFile Then
                If sFieldsToDelete.Count > 0 Then
                    sFieldsToDelete.RemoveAt(0)
                End If
            End If

            ' Now delete the fields
            For Each sField In sFieldsToDelete
                Try
                    GP.DataManagement.DeleteField(Me, sField)
                Catch ex As Exception
                    ex.Data("Field") = sField
                    ex.Data("Feature Class") = FullPath
                    Throw
                End Try
            Next
        End Sub


        Public Shared Function CreateFeatureClass(ByVal outFeatureClass As String, ByVal eType As GISDataStructures.BasicGISTypes, ByVal b3D As Boolean, ByVal pSpatRef As ISpatialReference, Optional ByVal pInputFields As IFields2 = Nothing) As VectorDataSource

            If String.IsNullOrEmpty(outFeatureClass) Then
                Throw New Exception("Null or empty feature class name")
            End If

            If Not TypeOf pSpatRef Is ISpatialReference Then
                Throw New Exception("Invalid or null spatial reference")
            End If

            Dim pFCResult As IFeatureClass = Nothing
            Dim gResult As VectorDataSource = Nothing

            Dim sWorkspacePath As String = GetWorkspacePath(outFeatureClass) ' IO.Path.GetDirectoryName(outFeatureClass) ' FeatureClass.GetWorkspacePath(outFeatureClass)
            Dim pWorkFact As IWorkspaceFactory
            Dim sFeatureClassName As String
            Dim pFDS As IFeatureDataset = Nothing

            If IsFileGeodatabase(outFeatureClass) Then
                pWorkFact = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.FileGeodatase)  'New FileGDBWorkspaceFactory
                sFeatureClassName = outFeatureClass.Substring(sWorkspacePath.Length + IO.Path.DirectorySeparatorChar.ToString.Length)
            Else
                pWorkFact = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.ShapeFile)
                sFeatureClassName = IO.Path.GetFileNameWithoutExtension(outFeatureClass)
                '
                ' Delete any existing shapefile
                '
                If VectorDataSource.Exists(outFeatureClass) Then
                    GISCode.GP.DataManagement.Delete(outFeatureClass)
                    'GISCode.ShapeFile.DeleteShapefile(outFeatureClass)
                End If
            End If

            Dim pWork As IFeatureWorkspace = pWorkFact.OpenFromFile(sWorkspacePath, 0)
            '
            ' Check to see if the feature class is in a feature dataset
            '
            Dim i As Integer = sFeatureClassName.LastIndexOf(IO.Path.DirectorySeparatorChar)
            If i >= 0 Then
                Dim sFeatureDatasetName As String = sFeatureClassName.Substring(0, i)
                sFeatureClassName = sFeatureClassName.Substring(i + 1)

                Dim pWS As IWorkspace = pWork
                Dim pWS2 As IWorkspace2 = pWork
                If pWS2.NameExists(esriDatasetType.esriDTFeatureDataset, sFeatureDatasetName) Then
                    pFDS = pWork.OpenFeatureDataset(sFeatureDatasetName)
                End If
            End If

            Try
                'Create shape field
                Dim pGDef As IGeometryDef = New GeometryDef
                Dim pGDefEdit As IGeometryDefEdit = pGDef

                Select Case eType
                    Case BasicGISTypes.Point : pGDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint
                    Case BasicGISTypes.Line : pGDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline
                    Case BasicGISTypes.Polygon : pGDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon
                    Case Else
                        Dim ex As New Exception("Invalid GIS data type")
                        ex.Data("Type") = eType.ToString
                        Throw ex
                End Select
                pGDefEdit.HasZ_2 = b3D

                'Set projection
                pGDefEdit.SpatialReference_2 = pSpatRef

                Dim pField As IField = New Field
                Dim pFieldEdit As IFieldEdit = pField
                pFieldEdit.Name_2 = "Shape"
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry
                pFieldEdit.GeometryDef_2 = pGDef

                Dim pFields As IFields2 = New Fields
                Dim pFieldsEdit As IFieldsEdit = pFields
                pFieldsEdit.AddField(pField)

                If Not pInputFields Is Nothing Then
                    For index As Integer = 0 To pInputFields.FieldCount - 1
                        Dim pInputField As IField
                        pInputField = pInputFields.Field(index)
                        pFieldsEdit.AddField(pInputField)
                    Next
                End If

                'Dim fcDescription As IFeatureClassDescription = New FeatureClassDescriptionClass
                'Dim ocDescription As IObjectClassDescription = fcDescription
                'Dim thefields As IFields = ocDescription.RequiredFields
                'Dim theFieldsEdit As IFieldsEdit = thefields

                Dim sFullPath As String = sWorkspacePath
                If TypeOf pFDS Is IFeatureDataset Then
                    sFullPath = IO.Path.Combine(sFullPath, pFDS.BrowseName)
                    pFCResult = pFDS.CreateFeatureClass(sFeatureClassName, pFields, Nothing, Nothing, esriFeatureType.esriFTSimple, "Shape", "")
                Else
                    pFCResult = pWork.CreateFeatureClass(sFeatureClassName, pFields, Nothing, Nothing, esriFeatureType.esriFTSimple, "Shape", "")
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFields)
                sFullPath = IO.Path.Combine(sFullPath, sFeatureClassName)

                Dim pWS As IWorkspace = pWork
                If pWS.Type = esriWorkspaceType.esriFileSystemWorkspace Then
                    sFullPath = IO.Path.ChangeExtension(sFullPath, "shp")
                End If

                Select Case eType
                    Case BasicGISTypes.Point
                        If b3D Then
                            gResult = New PointDataSource3D(sFullPath)
                        Else
                            gResult = New PointDataSource(sFullPath)
                        End If

                    Case BasicGISTypes.Line
                        gResult = New PolylineDataSource(sFullPath)

                    Case BasicGISTypes.Polygon
                        gResult = New PolygonDataSource(sFullPath)
                End Select

            Catch ex As Exception
                Dim e As New Exception("Error creating new feature class", ex)
                e.Data.Add("outFeatureClass", outFeatureClass)
                e.Data.Add("eType", eType.ToString)
                e.Data.Add("b3D", b3D.ToString)
                Throw e
            End Try

            Return gResult

        End Function

        Public Shared Function CreateFeatureClassUnprojected(ByVal outFeatureClass As String, ByVal eType As GISDataStructures.BasicGISTypes, ByVal b3D As Boolean, Optional ByVal pInputFields As IFields2 = Nothing) As VectorDataSource
            If String.IsNullOrEmpty(outFeatureClass) Then
                Throw New Exception("Null or empty feature class name")
            End If

            Dim pFCResult As IFeatureClass = Nothing
            Dim gResult As VectorDataSource = Nothing

            Dim sWorkspacePath As String = GetWorkspacePath(outFeatureClass) ' IO.Path.GetDirectoryName(outFeatureClass) ' FeatureClass.GetWorkspacePath(outFeatureClass)
            Dim pWorkFact As IWorkspaceFactory
            Dim sFeatureClassName As String = IO.Path.GetFileNameWithoutExtension(outFeatureClass)
            Dim pFDS As IFeatureDataset = Nothing

            If IsFileGeodatabase(outFeatureClass) Then
                pWorkFact = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.FileGeodatase)  'New FileGDBWorkspaceFactory
                'sFeatureClassName = outFeatureClass.Substring(sWorkspacePath.Length + IO.Path.DirectorySeparatorChar.ToString.Length)
            Else
                pWorkFact = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.ShapeFile)
                sFeatureClassName = IO.Path.GetFileNameWithoutExtension(outFeatureClass)
                '
                ' Delete any existing shapefile
                '
                If VectorDataSource.Exists(outFeatureClass) Then
                    GISCode.GP.DataManagement.Delete(outFeatureClass)
                    'GISCode.ShapeFile.DeleteShapefile(outFeatureClass)
                End If
            End If

            Dim pWork As IFeatureWorkspace = pWorkFact.OpenFromFile(sWorkspacePath, 0)

            '
            ' Check to see if the feature class is in a feature dataset
            '
            Dim i As Integer = sFeatureClassName.LastIndexOf(IO.Path.DirectorySeparatorChar)
            If i >= 0 Then
                Dim sFeatureDatasetName As String = sFeatureClassName.Substring(0, i)
                sFeatureClassName = sFeatureClassName.Substring(i + 1)

                Dim pWS As IWorkspace = pWork
                Dim pWS2 As IWorkspace2 = pWork
                If pWS2.NameExists(esriDatasetType.esriDTFeatureDataset, sFeatureDatasetName) Then
                    pFDS = pWork.OpenFeatureDataset(sFeatureDatasetName)
                End If
            End If

            Try
                'Create shape field
                Dim pGDef As IGeometryDef = New GeometryDef
                Dim pGDefEdit As IGeometryDefEdit = pGDef

                Select Case eType
                    Case BasicGISTypes.Point : pGDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint
                    Case BasicGISTypes.Line : pGDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline
                    Case BasicGISTypes.Polygon : pGDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon
                    Case Else
                        Dim ex As New Exception("Invalid GIS data type")
                        ex.Data("Type") = eType.ToString
                        Throw ex
                End Select
                pGDefEdit.HasZ_2 = b3D

                'Set projection
                Dim unKnownSpatialRef As ISpatialReference = New ESRI.ArcGIS.Geometry.UnknownCoordinateSystem
                unKnownSpatialRef.SetDomain(-10000000.0, 10000000.0, -10000000.0, 10000000.0)
                unKnownSpatialRef.SetZDomain(-150.0, 30000)
                pGDefEdit.SpatialReference_2 = unKnownSpatialRef

                Dim pField As IField = New Field
                Dim pFieldEdit As IFieldEdit = pField
                pFieldEdit.Name_2 = "Shape"
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry
                pFieldEdit.GeometryDef_2 = pGDef

                Dim pFields As IFields2 = New Fields
                Dim pFieldsEdit As IFieldsEdit = pFields
                pFieldsEdit.AddField(pField)

                If Not pInputFields Is Nothing Then
                    For index As Integer = 0 To pInputFields.FieldCount - 1
                        Dim pInputField As IField
                        pInputField = pInputFields.Field(index)
                        pFieldsEdit.AddField(pInputField)
                    Next
                End If


                'Dim fcDescription As IFeatureClassDescription = New FeatureClassDescriptionClass
                'Dim ocDescription As IObjectClassDescription = fcDescription
                'Dim thefields As IFields = ocDescription.RequiredFields
                'Dim theFieldsEdit As IFieldsEdit = thefields

                Dim sFullPath As String = sWorkspacePath
                If TypeOf pFDS Is IFeatureDataset Then
                    sFullPath = IO.Path.Combine(sFullPath, pFDS.BrowseName)
                    pFCResult = pFDS.CreateFeatureClass(sFeatureClassName, pFields, Nothing, Nothing, ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTSimple, "Shape", "")
                Else
                    pFCResult = pWork.CreateFeatureClass(sFeatureClassName, pFields, Nothing, Nothing, ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTSimple, "Shape", "")
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFields)
                sFullPath = IO.Path.Combine(sFullPath, sFeatureClassName)

                Dim pWS As IWorkspace = pWork
                If pWS.Type = esriWorkspaceType.esriFileSystemWorkspace Then
                    sFullPath = IO.Path.ChangeExtension(sFullPath, "shp")
                End If

                'Release COM Objects
                'Dim comReferencesLeft As Integer
                'Do
                '    comReferencesLeft = System.Runtime.InteropServices.Marshal.ReleaseComObject(pFDS) _
                '        + System.Runtime.InteropServices.Marshal.ReleaseComObject(pWork)
                'Loop While (comReferencesLeft > 0)

                ''Cast back to specific VectorDataSource
                'Select Case eType
                Select Case eType
                    Case BasicGISTypes.Point
                        If b3D Then
                            gResult = New PointDataSource3D(sFullPath)
                        Else
                            gResult = New PointDataSource(sFullPath)
                        End If

                    Case BasicGISTypes.Line
                        gResult = New PolylineDataSource(sFullPath)

                    Case BasicGISTypes.Polygon
                        gResult = New PolygonDataSource(sFullPath)
                End Select

            Catch ex As Exception
                Dim e As New Exception("Error creating new feature class", ex)
                e.Data.Add("outFeatureClass", outFeatureClass)
                e.Data.Add("eType", eType.ToString)
                'e.Data.Add("b3D", b3D.ToString)
                Throw e
            End Try

            Return gResult
            'Return pFCResult

        End Function

        ''' <summary>
        ''' Given an existing feature class path, or directory, this function returns a new feature class path in the same workspace with a unique, unused, name.
        ''' </summary>
        ''' <param name="sInputPath">Fully qualified feature class path on which to base</param>
        ''' <param name="sRootName">The root name for the new feature class. If blank then the input path feature class name is used.</param>
        ''' <param name="nMax">The Maximum permissible length of the feature class name. Leave blank or pass zero for unlimited length.</param>
        ''' <returns></returns>
        ''' <remarks>If the input is a file geodatabase then the dataset name is simply the name. If the
        ''' input is a shapefile then the dataset name includes the ".shp" extension.</remarks>
        Public Shared Function GetNewSafeNameFeatureClass(ByVal sInputPath As String, Optional ByVal sRootName As String = "", Optional ByVal nMax As Byte = 0) As String

            Dim bFileGDB As Boolean = GISDataStructures.IsFileGeodatabase(sInputPath)

            Dim wsFact As IWorkspaceFactory2
            Dim sWorkspacePath As String
            Dim sFDSName As String = Nothing
            Dim nLastSlash As Integer = sInputPath.LastIndexOf(IO.Path.DirectorySeparatorChar)
            Dim sExtension As String = "shp" 'IO.Path.GetExtension(sInputPath)
            Dim sOriginalDSName As String

            If bFileGDB Then
                Dim aType As Type = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory")
                Dim obj As System.Object = Activator.CreateInstance(aType)
                wsFact = obj

                Dim i As Integer = sInputPath.IndexOf(".gdb")
                sWorkspacePath = sInputPath.Substring(0, i + 4)
                '
                ' The next few lines were to get the FeatureDataset name in FGDB, but are no longer needed.
                '
                'If nLastSlash >= 0 Then
                '    sFDSName = sInputPath.Substring(i + 5, nLastSlash - (i + 5))
                'End If

                sOriginalDSName = sInputPath.Substring(nLastSlash + 1)
            Else
                '
                ' PGB 18 Aug 2011 - sInputPath can be a folder **or** a full path to an existing shapefile. Therefore,
                ' we need to call GetDirectoryName(), but if sInputPath is already just a directory, then it will strip
                ' the last directory name off the end. So determine if sInputPath is a directory.
                '
                Dim sInputDir As New IO.DirectoryInfo(sInputPath)
                ' Dim f As New FileInfo(sExtension)

                Dim aType As Type = Type.GetTypeFromProgID("esriDataSourcesFile.ShapefileWorkspaceFactory")
                Dim obj As System.Object = Activator.CreateInstance(aType)
                wsFact = obj

                If sInputDir.Exists Then
                    sWorkspacePath = sInputPath
                Else
                    sWorkspacePath = IO.Path.GetDirectoryName(sInputPath)
                End If
                sOriginalDSName = IO.Path.GetFileNameWithoutExtension(sInputPath)
            End If

            'changed because shapfileworkspace doesnt support IWorkspace2 - Frank
            Dim pWS As IWorkspace = wsFact.OpenFromFile(sWorkspacePath, 0)
            Dim nCount As Integer = 0
            Dim sNewDSName As String

            Do
                If String.IsNullOrEmpty(sRootName) Then
                    If String.IsNullOrEmpty(sOriginalDSName) Then
                        sNewDSName = "ds"
                    Else
                        sNewDSName = sOriginalDSName
                    End If
                Else
                    sNewDSName = IO.Path.GetFileNameWithoutExtension(sRootName)
                End If
                sNewDSName = FileSystem.RemoveDangerousCharacters(sNewDSName)

                If nCount > 0 Then
                    sNewDSName &= nCount.ToString()
                End If

                If Not bFileGDB Then
                    If Not String.IsNullOrEmpty(sExtension) Then
                        sNewDSName = IO.Path.ChangeExtension(sNewDSName, sExtension)
                        'sNewDSName &= sExtension
                    End If
                End If

                nCount += 1


                'changed because shapfileworkspace doesnt support IWorkspace2 - Frank
                'Loop While pWS.NameExists(esriDatasetType.esriDTFeatureClass, sNewDSName) AndAlso nCount < 9999
            Loop While NameExistsInWorkspace(pWS, esriDatasetType.esriDTFeatureClass, sNewDSName) AndAlso nCount < 9999

            Dim sOutputPath As String = sWorkspacePath
            If Not bFileGDB Then
                't
                ' hi frank
                'again
                'from the rbt
                ' from the GCD again
                If Not String.IsNullOrEmpty(sFDSName) Then
                    sOutputPath = IO.Path.Combine(sOutputPath, sFDSName)
                End If
            End If
            sOutputPath = IO.Path.Combine(sOutputPath, sNewDSName)

            Return sOutputPath

        End Function

        ''' <summary>
        ''' Adds a field to an existing featureClass
        ''' </summary>
        ''' <param name="sFieldname">The name of the field to be added</param>
        ''' <param name="eType">Data type of the field that should be added.</param>
        ''' <returns>The index of the field once it is added</returns>
        ''' <remarks>This method can only create integer and double fields
        ''' PGB 26 Jan 2012 - updated to handle string files as its now used by the cross section layout code in the
        ''' backend.
        ''' PGB 26 Jan 2012 - updated to not simply return if the field already exists
        ''' PGB 28 Aug 2013 - updated to allow caller to specify if the field should be Nullable or not.
        ''' PGB 02 Sep 2013 - the nullable setting was removed. DBF tables in shapefiles cannot be nulled.</remarks>
        Public Function AddField(ByVal sFieldName As String, ByVal eType As esriFieldType, Optional ByVal nLength As Integer = 0) As Integer

            If String.IsNullOrEmpty(sFieldName) Then
                Throw New Exception("Null or empty field name passed")
            End If

            If GISDataStorageType = GISDataStorageTypes.ShapeFile Then
                If sFieldName.Length > 10 Then
                    Dim ex As New GISException(GISException.ErrorTypes.CriticalError, "The field name is longer than the allowable limit", "Use a shorter field name")
                    ex.Data("Full Path") = FullPath
                    ex.Data("Field Name") = sFieldName
                    ex.Data("Field Name length") = sFieldName.Length.ToString
                    ex.Data("Max allowed field length") = "10"
                    Throw ex
                End If
            End If

            If eType = esriFieldType.esriFieldTypeString AndAlso nLength < 1 Then
                Dim ex As New Exception("Invalid field length passed")
                ex.Data("FieldName") = sFieldName
                ex.Data("Length") = nLength
                Throw ex
            End If

            If Not (eType = esriFieldType.esriFieldTypeDate OrElse _
                eType = esriFieldType.esriFieldTypeDouble OrElse _
                eType = esriFieldType.esriFieldTypeInteger OrElse _
                eType = esriFieldType.esriFieldTypeSingle OrElse _
                eType = esriFieldType.esriFieldTypeSmallInteger OrElse _
                eType = esriFieldType.esriFieldTypeString) Then

                Dim ex As New Exception("This method can only create integer and double fields")
                ex.Data("Field Name") = sFieldName
                ex.Data("Type") = eType.ToString
                Throw ex
            End If

            Dim iField As Integer = FindField(sFieldName)
            If iField < 0 Then
                '
                ' Remove any dangerous characters
                '
                sFieldName = sFieldName.Replace(",", String.Empty)
                sFieldName = sFieldName.Replace("'", String.Empty)
                sFieldName = sFieldName.Replace("""", String.Empty)

                Dim textField As IField = New FieldClass()
                Dim textFieldEdit As IFieldEdit = CType(textField, IFieldEdit)

                textFieldEdit.Name_2 = sFieldName
                textFieldEdit.Type_2 = eType
                'textFieldEdit.IsNullable_2 = bAllowNulls

                If eType = esriFieldType.esriFieldTypeString AndAlso nLength > 0 Then
                    textFieldEdit.Length_2 = nLength
                End If

                Me.FeatureClass.AddField(textField)
                iField = FindField(sFieldName)
            End If

            Return iField

        End Function

        ''' <summary>
        ''' Fills a combobox with the names of fields from a feature class
        ''' </summary>
        ''' <param name="cboFields">Combobox dropdown list control</param>
        ''' <param name="sDefaultFieldToSelect">The name of a field that if it exists, is preselected</param>
        ''' <remarks>PGB 18 May 2012</remarks>
        Public Sub FillComboWithFields(ByRef cboFields As System.Windows.Forms.ComboBox, ByVal sDefaultFieldToSelect As String, ByVal eType As esriFieldType, Optional ByVal bClearCbomItemsFirst As Boolean = True)

            If bClearCbomItemsFirst Then
                cboFields.Items.Clear()
            End If

            Try
                Dim pFields As IFields = FeatureClass.Fields
                Dim Field As IField = Nothing
                Dim nDescriptionField As Integer = -1
                Dim nLastDescriptionField As Integer = -1
                Dim j As Integer

                'Interate through the fields in the collection
                For i As Integer = 0 To pFields.FieldCount - 1
                    Field = pFields.Field(i)

                    'Add field to combobox
                    If Field.Type = eType Then
                        Dim TextField As String
                        TextField = Field.AliasName
                        If Not cboFields.Items.Contains(TextField) Then
                            j = cboFields.Items.Add(TextField)
                            If String.Compare(TextField, sDefaultFieldToSelect, True) = 0 Then
                                nDescriptionField = j
                            ElseIf String.Compare(TextField, sDefaultFieldToSelect, True) = 0 Then
                                nLastDescriptionField = j
                            End If
                        End If
                    End If
                Next

                If nLastDescriptionField >= 0 Then
                    cboFields.SelectedIndex = nLastDescriptionField
                Else
                    If nDescriptionField >= 0 Then
                        cboFields.SelectedIndex = nDescriptionField
                    End If
                End If

            Catch ex As Exception
                ex.Data("Full Path") = FullPath
                ex.Data("Combobox") = cboFields.Name
                ex.Data("Default field to select") = sDefaultFieldToSelect
                ex.Data("Type") = eType.ToString
                ex.Data("Clear Items First") = bClearCbomItemsFirst.ToString
                Throw
            End Try

        End Sub

        '
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' The following methods were taken from the old "Features" namespace.
        '
        ''' <summary>
        ''' Open an ArcCatalog browser window and navigate to a feature class
        ''' </summary>
        ''' <param name="sTitle">Browser window title</param>
        ''' <param name="sFolder">Output folder of selected feature class</param>
        ''' <param name="sFCName">Output feature class name</param>
        ''' <param name="eType">Optional enumeration for filtering the browser with a single geometry type</param>
        ''' <returns>The fully qualified (geoprocessing compatible) path to the selected feature class</returns>
        ''' <remarks>This method now works with both ShapeFile sources and geodatabase sources.</remarks>
        Public Shared Function BrowseOpen(ByVal sTitle As String, _
                                             ByRef sFolder As String, _
                                             ByRef sFCName As String, _
                                             ByVal eType As BrowseGISTypes, _
                                             ByVal hParentWindowHandle As System.IntPtr) As GISDataStructures.VectorDataSource

            Dim gResult As VectorDataSource = Nothing
            Dim pGxDialog As IGxDialog = New GxDialog
            Dim pFilterCol As IGxObjectFilterCollection = pGxDialog
            '
            ' Add the necessary filters to the browse dialog
            '
            'pFilterCol.AddFilter(New GxFilterLayers, False)
            '
            ' Now add the filters based on the optional geometry enumerations
            '
            Select Case eType
                Case GeometryTypes.Point : pFilterCol.AddFilter(New GxFilterPointFeatureClasses, True)
                Case GeometryTypes.Line : pFilterCol.AddFilter(New GxFilterPolylineFeatureClasses, True)
                Case GeometryTypes.Polygon : pFilterCol.AddFilter(New GxFilterPolygonFeatureClasses, True)
                Case BrowseGISTypes.Any : pFilterCol.AddFilter(New GxFilterFeatureClasses, True)
                Case Else
                    Dim ex As New Exception("Invalid browse type for vector feature class")
                    ex.Data("Type") = eType.ToString
                    Throw ex
            End Select

            'sFolder = ""
            'sFCName = ""

            Dim pEnumGx As IEnumGxObject = Nothing
            Dim pGxObject As IGxObject = Nothing

            pGxDialog.RememberLocation = True
            pGxDialog.AllowMultiSelect = False
            pGxDialog.Title = sTitle
            pGxDialog.ButtonCaption = "Select"

            If Not String.IsNullOrEmpty(sFolder) Then
                pGxDialog.StartingLocation = sFolder
            End If
            pGxDialog.Name = sFCName


            Try

                If pGxDialog.DoModalOpen(hParentWindowHandle.ToInt32, pEnumGx) Then
                    pGxObject = pEnumGx.Next
                    '
                    ' PGB 2 Jun 2012. The 2012 CHaMP data requires the use of AutoCad point DXF files.
                    ' The GISDataSource is not designed for these and so switching to just using strings.
                    '
                    'Dim gisData As New GISDataSource(pGxObject.FullName)
                    'Dim sFile As New FileInfo(pGxObject.FullName)
                    sFCName = pGxObject.BaseName
                    sFolder = IO.Path.GetDirectoryName(pGxObject.FullName) ' gisData.Workspace.PathName ' sFile.Directory.FullName

                    Dim gTemp As New VectorDataSource(pGxObject.FullName)
                    eType = gTemp.GeometryType

                    Select Case eType
                        Case GeometryTypes.Point : gResult = New PointDataSource(pGxObject.FullName)
                        Case GeometryTypes.Line : gResult = New PolylineDataSource(pGxObject.FullName)
                        Case GeometryTypes.Polygon : gResult = New PolygonDataSource(pGxObject.FullName)
                    End Select
                End If

            Catch ex As Exception
                Dim ex2 As New Exception("Error browsing to feature class", ex)
                ex2.Data("Title") = sTitle
                ex2.Data("Folder") = sFolder
                ex2.Data("File") = sFCName
                ex2.Data("Geometry") = eType.ToString
                Throw ex2
            End Try

            Return gResult

        End Function

        Public Shared Function BrowseOpen(ByVal cbo As System.Windows.Forms.ComboBox,
                                          ByVal sTitle As String,
                                          ByVal eGeomType As GISDataStructures.GeometryTypes,
                                          ByVal hParentWindowHandle As System.IntPtr) As GISDataStructures.VectorDataSource

            Dim sName As String = String.Empty
            Dim sFolder As String = String.Empty
            Dim gFC As GISDataStructures.VectorDataSource = Nothing
            If TypeOf cbo.SelectedItem Is VectorDataSource Then
                gFC = cbo.SelectedItem
                sName = IO.Path.GetFileNameWithoutExtension(gFC.FullPath)
                sFolder = IO.Path.GetDirectoryName(gFC.FullPath)
            End If

            gFC = GISDataStructures.VectorDataSource.BrowseOpen(sTitle, sFolder, sName, eGeomType, hParentWindowHandle)
            If TypeOf gFC Is VectorDataSource Then
                gFC.AddToCombo(cbo)
            End If

            Return gFC

        End Function

        ''' <summary>
        ''' Browse to an existing feature class
        ''' </summary>
        ''' <param name="txt">Textbox that will hold the output full path, and might already hold an existing path</param>
        ''' <param name="sFormTitle">Title of the browse form</param>
        ''' <param name="sErrorMessage">Error message for pop up if anything goes wrong</param>
        ''' <param name="eGeometryType">Geometry type to filter the browse window</param>
        ''' <param name="pArcMap">ArcMap for adding layer to the map. Pass nothing to skip</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function BrowseOpen(ByRef txt As System.Windows.Forms.TextBox,
                                          ByVal sFormTitle As String,
                                          ByVal sErrorMessage As String,
                                          ByVal eGeometryType As BrowseGISTypes,
                                          ByVal pArcMap As IApplication,
                                          ByVal hPArentWindowHandle As System.IntPtr) As GISDataStructures.VectorDataSource

            Dim sFolder As String = String.Empty
            Dim sName As String = String.Empty
            Dim gResult As GISDataStructures.VectorDataSource = Nothing
            If Not String.IsNullOrEmpty(txt.Text) Then
                Try
                    If GISDataStructures.VectorDataSource.Exists(txt.Text) Then
                        gResult = New GISDataStructures.VectorDataSource(txt.Text)
                        sFolder = gResult.WorkspacePath
                        sName = gResult.DatasetName
                    End If
                Catch ex As Exception
                End Try
            End If

            Try
                gResult = BrowseOpen(sFormTitle, sFolder, sName, eGeometryType, hPArentWindowHandle)
                If Not gResult Is Nothing Then
                    txt.Text = gResult.FullPath
                    If Not pArcMap Is Nothing Then
                        gResult.AddToMap(pArcMap)
                    End If
                End If
            Catch ex As Exception
                ExceptionUI.HandleException(ex, sErrorMessage)
            End Try

            Return gResult

        End Function


        ''' <summary>
        ''' Open an ArcCatalog window to create a new feature class
        ''' </summary>
        ''' <param name="sTitle">Browse window title</param>
        ''' <param name="sFolder">Output folder name</param>
        ''' <param name="sName">Output layer name</param>
        ''' <param name="StartingLocation">Starting folder location</param>
        ''' <returns>Full path including folder and file name</returns>
        ''' <remarks></remarks>
        Public Shared Function BrowseSave(ByVal sTitle As String, _
                                   ByRef sFolder As String, _
                                   ByRef sName As String, _
                                   ByVal hParentWindowHandle As System.IntPtr, _
                                   Optional ByVal StartingLocation As String = Nothing, _
                                   Optional ByVal eGeomType As GISDataStructures.GeometryTypes = Nothing) As String

            Dim pGxDialog As IGxDialog = New GxDialog
            Dim pFilterCol As IGxObjectFilterCollection = pGxDialog

            'pFilterCol.AddFilter(New GxFilterLayers, False) ' this is the default filter
            'pFilterCol.AddFilter(New GxFilterFeatureClasses, False)
            'pFilterCol.AddFilter(new
            '
            ' Now add the filters based on the optional geometry enumerations
            '
            Select Case eGeomType
                Case BasicGISTypes.Point : pFilterCol.AddFilter(New GxFilterPointFeatureClasses, True)
                Case BasicGISTypes.Line : pFilterCol.AddFilter(New GxFilterPolylineFeatureClasses, True)
                Case BasicGISTypes.Polygon : pFilterCol.AddFilter(New GxFilterPolygonFeatureClasses, True)
                Case Else
                    pFilterCol.AddFilter(New GxFilterFeatureClasses, True)
            End Select

            If Not String.IsNullOrEmpty(sFolder) Then
                pGxDialog.StartingLocation = sFolder
            End If

            If Not String.IsNullOrEmpty(sName) Then
                pGxDialog.Name = sName
            End If

            pGxDialog.RememberLocation = True
            pGxDialog.AllowMultiSelect = False
            pGxDialog.Title = sTitle

            Dim sResult As String = String.Empty

            Try
                If pGxDialog.DoModalSave(hParentWindowHandle.ToInt32()) Then
                    sFolder = pGxDialog.FinalLocation.FullName
                    sName = pGxDialog.Name
                    sResult = IO.Path.Combine(sFolder, sName)
                    ConfirmExtension(sResult)
                End If

            Catch ex As Exception
                Throw New Exception("Data.BrowseSaveLayer() There was an error reading the spatial data source: " & ex.Message)
            End Try

            Return sResult

        End Function

        ''' <summary>
        ''' Browse for a new output feature class
        ''' </summary>
        ''' <param name="sBrowseFormTitle">Title on the open/save browse form</param>
        ''' <param name="txtOutputFeatureClass">Textbox containing the current and new feature class name</param>
        ''' <param name="sWorkspaceFolder">Workspace (file GDB or folder) where output will be created</param>
        ''' <param name="ttpToolTip">Form Tooltip that will receive the full file path to the new feature class</param>
        ''' <param name="eGeomType">Output geometry type for filering the browse window</param>
        ''' <returns>True if the browse was successful and user selected a new feature class</returns>
        ''' <remarks>PGB 15 March 2012
        ''' Use this method within the click event of the browse button for output feature class on forms
        ''' that have a single vector output. Note the use of ByRef and that this method alters certain
        ''' arguments to receive the name and path of the new feature class.</remarks>
        Public Shared Function BrowseSave(ByVal sBrowseFormTitle As String,
                                                 ByRef txtOutputFeatureClass As System.Windows.Forms.TextBox,
                                                 ByRef sWorkspaceFolder As String,
                                                 ByRef ttpToolTip As System.Windows.Forms.ToolTip,
                                                 ByVal hParentWindowHandle As System.IntPtr,
                                                 Optional ByVal eGeomType As GISDataStructures.GeometryTypes = Nothing) As Boolean

            Dim bResult As Boolean = False
            Dim sName As String = txtOutputFeatureClass.Text
            Dim sFolder As String = sWorkspaceFolder

            Try
                Dim sFullPath As String = BrowseSave(sBrowseFormTitle, sFolder, sName, hParentWindowHandle, sWorkspaceFolder, eGeomType)
                If Not String.IsNullOrEmpty(sFullPath) Then
                    txtOutputFeatureClass.Text = IO.Path.GetFileName(sFullPath)
                    sWorkspaceFolder = sFolder
                    ttpToolTip.SetToolTip(txtOutputFeatureClass, sFullPath)
                    bResult = True
                End If

            Catch ex As Exception
                ex.Data("Browse Title") = sBrowseFormTitle
                ex.Data("Output Feature Class TextBox") = txtOutputFeatureClass.Text
                ex.Data("Workspace folder") = sWorkspaceFolder
                ex.Data("Geometry Type") = eGeomType.ToString
                ExceptionUI.HandleException(ex, "Error browsing to output feature class.")
            End Try

            Return bResult
        End Function

        ''' <summary>
        ''' Alters the file extension depending on file geodatabase or not.
        ''' </summary>
        ''' <param name="inFullPath">Full path to an existing or new feature class</param>
        ''' <remarks></remarks>
        Public Shared Sub ConfirmExtension(ByRef inFullPath As String)

            If String.IsNullOrEmpty(inFullPath) Then
                Throw New ArgumentNullException("inFullPath", "Null or empty feature class full path")
            End If

            If GISCode.GISDataStructures.IsFileGeodatabase(inFullPath) Then
                inFullPath = IO.Path.ChangeExtension(inFullPath, Nothing)
            Else
                inFullPath = IO.Path.ChangeExtension(inFullPath, "shp")
            End If

        End Sub

        Public Function GetClosestFeature(ByVal pSearchPt As IPoint, Optional ByVal fTolerance As Double = 1) As IFeature
            '
            ' Set up the search envelope using the tolerance
            '
            Dim pSrchEnv As IEnvelope
            pSrchEnv = pSearchPt.Envelope
            pSrchEnv.Width = fTolerance
            pSrchEnv.Height = fTolerance
            pSrchEnv.CenterAt(pSearchPt)
            '
            ' Loop over all the features and find the closest one.
            '
            Dim pProximity As IProximityOperator = pSearchPt
            Dim pFC As IFeatureCursor = FeatureClass.Search(Nothing, True)
            Dim pFeature As IFeature = pFC.NextFeature
            Dim pResult As IFeature = Nothing
            Dim fTempDist As Double
            Dim fMinDist As Double = -1
            Do While TypeOf pFeature Is IFeature
                'pGeom = pTestFeature.Shape
                fTempDist = pProximity.ReturnDistance(pFeature.Shape)
                If fTempDist < fMinDist OrElse fMinDist = -1 Then
                    fMinDist = fTempDist
                    pResult = pFeature
                End If
                pFeature = pFC.NextFeature
            Loop
            Runtime.InteropServices.Marshal.ReleaseComObject(pFC)
            pFC = Nothing

            Return pResult

        End Function

        ''' <summary>
        ''' Takes a raw field name
        ''' </summary>
        ''' <param name="sFieldName">Raw field name that needs to be wrapped for use in an SQL statement</param>
        ''' <returns>The same field name but wrapped in the appropriate delimeters (e.g. quotes or square braces)</returns>
        ''' <remarks>PGB Nov 2014. This was taken from code that James Hensleigh developed. It should react to the
        ''' storage type of the dataset and put the correct delimeters around the field name.</remarks>
        Public Function WrapFieldForSQL(sFieldName As String) As String

            Try
                Dim sqlSyntax As ESRI.ArcGIS.Geodatabase.ISQLSyntax = DirectCast(DirectCast(FeatureClass, ESRI.ArcGIS.Geodatabase.IDataset).Workspace, ESRI.ArcGIS.Geodatabase.ISQLSyntax)
                Dim fieldPrefixDelimiter As String = sqlSyntax.GetSpecialCharacter(ESRI.ArcGIS.Geodatabase.esriSQLSpecialCharacters.esriSQL_DelimitedIdentifierPrefix)
                Dim fieldSuffixDelimter As String = sqlSyntax.GetSpecialCharacter(ESRI.ArcGIS.Geodatabase.esriSQLSpecialCharacters.esriSQL_DelimitedIdentifierSuffix)
                sFieldName = [String].Format("{0}{1}{2}", fieldPrefixDelimiter, sFieldName, fieldSuffixDelimter)
            Catch ex As Exception
                ex.Data("Feature Class") = FullPath
                Throw
            End Try

            Return sFieldName

        End Function

        Public Shared Sub SelectFeature(ByVal pGeoLayer As ESRI.ArcGIS.Carto.IGeoFeatureLayer, ByVal nOID As Long)

            If Not TypeOf pGeoLayer Is ESRI.ArcGIS.Carto.IGeoFeatureLayer Then
                Throw New Exception("The layer is null or empty")
            End If

            If nOID <= 0 Then
                Dim ex As New Exception("The OID must be greater than zero")
                ex.Data("OID") = nOID.ToString
            End If

            pGeoLayer.Selectable = True
            Dim pQry As IQueryFilter = New QueryFilter
            pQry.WhereClause = """" & pGeoLayer.FeatureClass.OIDFieldName & """ = " & nOID

            Dim pFSel As ESRI.ArcGIS.Carto.IFeatureSelection = pGeoLayer
            pFSel.SelectFeatures(pQry, ESRI.ArcGIS.Carto.esriSelectionResultEnum.esriSelectionResultNew, True)

        End Sub

        ''' <summary>
        ''' Gets the full path to the feature class
        ''' </summary>
        ''' <param name="pFeatureClass">A valid, existing feature class</param>
        ''' <returns>Full file path to the feature class</returns>
        ''' <remarks>Works with shapefiles or file geodatabases</remarks>
        Public Shared Function GetFeatureClassPath(ByVal pFeatureClass As IFeatureClass) As String

            Dim pDataset As IDataset = pFeatureClass
            Dim pWorkspace As IWorkspace = pDataset.Workspace
            Dim sResult As String = pWorkspace.PathName
            If TypeOf pFeatureClass.FeatureDataset Is IFeatureDataset Then
                sResult = IO.Path.Combine(sResult, pFeatureClass.FeatureDataset.BrowseName)
            End If
            sResult = IO.Path.Combine(sResult, pDataset.BrowseName)
            If Not GISDataStructures.IsFileGeodatabase(sResult) Then
                sResult = IO.Path.ChangeExtension(sResult, "shp")
            End If
            Return sResult
        End Function
    End Class

End Namespace
