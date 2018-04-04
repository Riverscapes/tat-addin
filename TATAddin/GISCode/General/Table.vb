#Region "Imports"

Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.DataSourcesFile

#End Region

Namespace GISCode.Table

    Public Module Table

        ''' <summary>
        ''' Gets a new Table name that is guaranteed to be unique within a workspace
        ''' </summary>
        ''' <param name="pWorkspace">Workspace where the table will be created</param>
        ''' <param name="sRootName">Base name for the table. Integers are appended until the name is guaranteed to be unique</param>
        ''' <returns>Safe name of the table (exluding the workspace path)</returns>
        ''' <remarks>PGB 2 Aug 2013. Note that the type of workspace factory that was used to generate the pWorkspace is 
        ''' IMPORTANT!!!!! If you pass in an IWorkspace object that was created from a rasterworkspacefactory then it will
        ''' be unable to loop over and find any existing tables. The name returned is therefore not guaranteed to be unique.</remarks>
        Public Function GetSafeName(pWorkspace As IWorkspace, sRootName As String) As String

            Dim sSafeName As String = ""

            If Not TypeOf pWorkspace Is IWorkspace Then
                Dim e As New Exception("Null workspace passed.")
                e.Data.Add("sRootName", sRootName)
                Throw e
            End If

            Try
                '
                ' Clean up the base/root name and remove any dangerous characters.
                '
                sRootName = IO.Path.GetFileNameWithoutExtension(sRootName)
                sRootName = FileSystem.RemoveDangerousCharacters(sRootName)
                '
                ' Continue to loop over all datasets in the workspace, adding integers at the end of the
                ' candidate table name, until one is found to be unique.
                '
                Dim pEDSName As IEnumDatasetName = pWorkspace.DatasetNames(esriDatasetType.esriDTTable)
                Dim pDSName As IName = pEDSName.Next
                Dim index As Integer = 0
                Do
                    'bUnique = True
                    sSafeName = sRootName
                    If index > 0 Then
                        sSafeName &= index.ToString
                    End If
                   
                    index += 1
                Loop While GISDataStructures.NameExistsInWorkspace(pWorkspace, esriDatasetType.esriDTAny, sSafeName) AndAlso index < 999

            Catch ex As Exception
                ex.Data("sRootName") = sRootName
                Throw ex
            End Try

            sSafeName = IO.Path.Combine(pWorkspace.PathName, sSafeName)
            If pWorkspace.Type = esriWorkspaceType.esriFileSystemWorkspace Then
                sSafeName = IO.Path.ChangeExtension(sSafeName, "dbf")
            End If

            Return sSafeName

        End Function

        ''' <summary>
        ''' Get a new safe table name in a workspace
        ''' </summary>
        ''' <param name="pWorkspace"></param>
        ''' <param name="sRootName"></param>
        ''' <returns>Full path of the new, unique table name</returns>
        ''' <remarks>PGB 30 Apr 2012. Scared by the existing GetSafeName method which seems to be not finding
        ''' when certain table names exist. So created this method during crunch to get GCD out.
        ''' PGB 15 Jan 2014. Note that Iworkspace2 only works on Geodatabases.</remarks>
        Public Function GetSafeNameFull(pWorkspace As IWorkspace2, sRootName As String) As String

            Dim sSafeName As String = ""

            If Not TypeOf pWorkspace Is IWorkspace2 Then
                Dim e As New Exception("Null workspace passed.")
                e.Data("sRootName") = sRootName
                Throw e
            End If

            Dim pWS As IWorkspace2 = pWorkspace

            Try
                '
                ' Clean up the base/root name and remove any dangerous characters.
                '
                sRootName = IO.Path.GetFileNameWithoutExtension(sRootName)
                sRootName = FileSystem.RemoveDangerousCharacters(sRootName)
                '
                ' Continue to loop over all datasets in the workspace, adding integers at the end of the
                ' candidate table name, until one is found to be unique.

                Dim index As Integer = 0
                Do
                    sSafeName = sRootName
                    If index > 0 Then
                        sSafeName &= index.ToString
                    End If
                    index += 1
                Loop While pWS.NameExists(esriDatasetType.esriDTTable, sSafeName) AndAlso index < 999

            Catch ex As Exception
                ex.Data("sRootName") = sRootName
                Throw
            End Try
            '
            ' Set the file extension if not in a file geodatabase
            '
            sSafeName = IO.Path.Combine(pWorkspace.PathName, sSafeName)
            If Not GISDataStructures.IsFileGeodatabase(pWorkspace.PathName) Then
                sSafeName = IO.Path.ChangeExtension(sSafeName, "dbf")
            End If

            Return sSafeName

        End Function

        ''' <summary>
        ''' Given the full path to a dBASE (.dbf) table this function will return the table as an ITable.
        ''' </summary>
        ''' <param name="sTablePath">Full path to an existing dBASE (.dbf) table.</param>
        ''' <returns>Table as ITable.</returns>
        ''' <remarks></remarks>
        Public Function OpenDBFTable(ByVal sTablePath As String) As ITable

            Dim aType As Type = Type.GetTypeFromProgID("esriDataSourcesFile.ShapefileWorkspaceFactory")
            Dim obj As System.Object = Activator.CreateInstance(aType)
            Dim pWSFact As IWorkspaceFactory = obj
            Dim pWS As IWorkspace = pWSFact.OpenFromFile(IO.Path.GetDirectoryName(sTablePath), 0)
            Dim pFWorkspace As IFeatureWorkspace = pWS
            Dim pTable As ITable = pFWorkspace.OpenTable(IO.Path.GetFileName(sTablePath))

            Return pTable

        End Function

        Public Function Open(pWS As IWorkspace, sTableName As String) As ITable

            If Not TypeOf pWS Is IWorkspace Then
                Throw New Exception("Undefined workspace.")
            End If

            If String.IsNullOrEmpty(sTableName) Then
                Throw New Exception("Empty table name")
            End If

            Dim pResult As ITable = Nothing
            Dim pEDS As IEnumDataset = pWS.Datasets(esriDatasetType.esriDTTable)
            Dim pDS As IDataset = pEDS.Next
            Do
                If pDS.Name = sTableName Then
                    If TypeOf pDS Is ITable Then
                        pResult = pDS
                        Exit Do
                    End If
                End If
                pDS = pEDS.Next
            Loop While TypeOf pDS Is IDataset

            Return pResult

        End Function

        Public Function Create(ByVal sPath As String,
                               Optional ByVal pInputFields As ESRI.ArcGIS.Geodatabase.IFields2 = Nothing,
                               Optional ByVal bDeleteIfExists As Boolean = False) As ESRI.ArcGIS.Geodatabase.ITable

            Dim pTableResult As ESRI.ArcGIS.Geodatabase.ITable = Nothing

            Try

                Dim sWorkspacePath As String = GISDataStructures.GetWorkspacePath(sPath)
                Dim pWorkFact As ESRI.ArcGIS.Geodatabase.IWorkspaceFactory
                Dim sTableResultName As String

                If GISDataStructures.IsFileGeodatabase(sPath) Then
                    pWorkFact = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.FileGeodatase)  'New FileGDBWorkspaceFactory
                    sTableResultName = sPath.Substring(sWorkspacePath.Length + IO.Path.DirectorySeparatorChar.ToString.Length)
                Else
                    pWorkFact = GISCode.ArcMap.GetWorkspaceFactory(GISDataStructures.GISDataStorageTypes.ShapeFile)
                    sTableResultName = IO.Path.GetFileNameWithoutExtension(sPath)
                End If

                If Exists(sPath) Then
                    If bDeleteIfExists = True Then
                        Delete(sPath)
                    ElseIf bDeleteIfExists = False Then
                        Dim e As Exception = New Exception("Error creating new table. Table already exists and delete is set to false.")
                        e.Data.Add("output table", sPath)
                        Throw e
                    End If
                End If

                Dim pWork As ESRI.ArcGIS.Geodatabase.IFeatureWorkspace = pWorkFact.OpenFromFile(sWorkspacePath, 0)

                Dim pFields As ESRI.ArcGIS.Geodatabase.IFields2 = New ESRI.ArcGIS.Geodatabase.Fields
                Dim pFieldsEdit As ESRI.ArcGIS.Geodatabase.IFieldsEdit = pFields

                If Not pInputFields Is Nothing Then
                    For index As Integer = 0 To pInputFields.FieldCount - 1
                        Dim pInputField As ESRI.ArcGIS.Geodatabase.IField
                        pInputField = pInputFields.Field(index)
                        pFieldsEdit.AddField(pInputField)
                    Next
                End If

                Dim sFullPath As String = sWorkspacePath
                If TypeOf pTableResult Is ESRI.ArcGIS.Geodatabase.ITable Then
                    sFullPath = IO.Path.Combine(sFullPath, pTableResult.BrowseName)
                    pTableResult = pWork.CreateTable(sTableResultName, pFields, Nothing, Nothing, "")
                Else
                    pTableResult = pWork.CreateTable(sTableResultName, pFields, Nothing, Nothing, "")
                End If

            Catch ex As Exception
                Dim e As New Exception("Error creating new File GDB table", ex)
                e.Data.Add("output Table", sPath)
                Throw e
            End Try

            Return pTableResult


        End Function

        Public Function Exists(ByVal sPath As String) As Boolean

            Dim bResult As Boolean = False

            If String.IsNullOrEmpty(sPath) Then
                ' Throw New ArgumentNullException("sPath", "Invalid or NULL feature class full path")
                Return False
            End If

            Try
                Dim eStorageType As GISDataStructures.GISDataStorageTypes
                If GISDataStructures.IsFileGeodatabase(sPath) Then
                    eStorageType = GISDataStructures.GISDataStorageTypes.FileGeodatase
                Else
                    eStorageType = GISDataStructures.GISDataStorageTypes.ShapeFile
                End If

                Dim pWS As ESRI.ArcGIS.Geodatabase.IWorkspace = GISDataStructures.GetWorkspace(sPath, eStorageType) '  Workspace(sPath)
                If Not TypeOf pWS Is ESRI.ArcGIS.Geodatabase.IFeatureWorkspace Then
                    Dim ex As New Exception("The workspace must be a feature workspace")
                    ex.Data.Add("Feature class path", sPath)
                    Throw ex
                End If
                Dim pFWS As ESRI.ArcGIS.Geodatabase.IFeatureWorkspace = pWS
                '
                ' PGB 22 Nov 2012
                ' The IWorkspace2.Exists() method seems really unreliable. Unfortunately we have to loop to get 
                ' a reliable result
                '
                Dim pDSNames As ESRI.ArcGIS.Geodatabase.IEnumDatasetName = pWS.DatasetNames(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTTable)
                Dim pDSN As ESRI.ArcGIS.Geodatabase.IDatasetName = pDSNames.Next
                Do While TypeOf pDSN Is ESRI.ArcGIS.Geodatabase.IDatasetName

                    If String.Compare(pDSN.Name, IO.Path.GetFileNameWithoutExtension(sPath), True) = 0 OrElse _
                        String.Compare(pDSN.Name, IO.Path.GetFileName(sPath), True) = 0 Then


                        Dim pTable As ESRI.ArcGIS.Geodatabase.ITable = pFWS.OpenTable(pDSN.Name)
                        bResult = True
                        Exit Do
                    End If

                    pDSN = pDSNames.Next
                Loop

            Catch ex As Exception
                ex.Data.Add("Path", sPath)
                Throw ex
            End Try

            Return bResult

        End Function

        Public Sub Delete(ByVal sPath As String)

            If String.IsNullOrEmpty(sPath) Then
                Throw New Exception("inData is null or empty string.")
            End If

            Dim GP As New ESRI.ArcGIS.Geoprocessor.Geoprocessor
            Dim DeleteTool As New ESRI.ArcGIS.DataManagementTools.Delete
            Dim bAddtoMap As Boolean = GP.AddOutputsToMap

            If Not GISDataStructures.IsFileGeodatabase(sPath) Then
                sPath = sPath & ".dbf"
            End If

            DeleteTool.in_data = sPath

            'GP.SetEnvironmentValue("workspace", WorkspaceManager.WorkspacePath)
            GP.TemporaryMapLayers = False
            GP.AddOutputsToMap = False

            GP.ClearMessages()

            Try
                GP.Execute(DeleteTool, Nothing)
                ExceptionBase.ProcessGPExceptions(GP)
            Catch ex As Exception
                ex.Data.Add("inData", sPath)
                Dim trimmedfilename As String = FileSystem.TrimFilename(sPath, 80)
                ex.Data("UIMessage") = "Could not delete '" & trimmedfilename & "'."
                ExceptionBase.AddGPMessagesToException(ex, GP)
                Throw ex
            Finally
                GP.AddOutputsToMap = bAddtoMap
            End Try

        End Sub

        'Public Sub GetValueFromTable(sTableFile As String, sFieldName As String, ByRef nOutputValue As Integer)

        'End Sub

        Public Sub GetValueFromTable(sTablePath As String, sFieldName As String, ByRef fOutputValue As Double)

            Dim pTable As ITable = Table.OpenDBFTable(sTablePath)

            fOutputValue = 0
            Dim nFldIndex As Integer = pTable.FindField(sFieldName)
            If nFldIndex > 0 Then
                Dim pFC As ICursor = pTable.Search(Nothing, True)
                Dim pRow As IRow = pFC.NextRow
                If TypeOf pRow Is IRow Then
                    fOutputValue = pRow.Value(nFldIndex)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Returns a dictionary of of keys and values from the table
        ''' </summary>
        ''' <param name="sTablePath">Full path to the table</param>
        ''' <param name="sKeyFieldName">Fieldname for keys. Must be an integer field.</param>
        ''' <param name="sValueFieldName">Fieldname for values. Must be a single or double field.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Opens the table and calls GetValuesFromTable to do actual work.
        ''' FP April 18 2013
        ''' </remarks>
        Public Function GetValuesFromTable(sTablePath As String, sKeyFieldName As String, sValueFieldName As String) As Dictionary(Of Integer, Double)
            Try

                Dim pTable As ITable
                Dim dicTableValues As Dictionary(Of Integer, Double)
                Dim nKeyFldIndex As Integer
                Dim nValueFldIndex As Integer
                Dim pKeyField As IField
                Dim pValueField As IField

                '
                'Validation
                '
                'check input strings are not empty
                If String.IsNullOrEmpty(sTablePath) Then
                    Throw New ArgumentNullException("sTablePath", "The sTablePath input string cannot be null or empty")
                End If
                If String.IsNullOrEmpty(sKeyFieldName) Then
                    Throw New ArgumentNullException("sKeyFieldName", "The sKeyFieldName input string cannot be null or empty")
                End If
                If String.IsNullOrEmpty(sValueFieldName) Then
                    Throw New ArgumentNullException("sValueFieldName", "The sValueFieldName input string cannot be null or empty")
                End If

                Try
                    pTable = Table.Open(GISDataStructures.GetWorkspace(sTablePath, GISDataStructures.GetWorkspaceType(sTablePath)), IO.Path.GetFileNameWithoutExtension(sTablePath))
                Catch ex As Exception
                    Dim ex2 As New Exception("Could not open table '" & sTablePath & "'", ex)
                    Throw ex2
                End Try

                'check that field exists
                nKeyFldIndex = pTable.FindField(sKeyFieldName)
                If nKeyFldIndex < 0 Then
                    Throw New ArgumentException("sKeyFieldName", "Field '" & sKeyFieldName & "' does not exists in table '" & sTablePath & "'")
                End If

                nValueFldIndex = pTable.FindField(sValueFieldName)
                If nValueFldIndex < 0 Then
                    Throw New ArgumentException("sValueFieldName", "Field '" & sValueFieldName & "' does not exists in table '" & sTablePath & "'")
                End If

                'check that field is right type
                pKeyField = pTable.Fields.Field(nKeyFldIndex)
                If Not pKeyField.Type = esriFieldType.esriFieldTypeInteger And Not pKeyField.Type = esriFieldType.esriFieldTypeSmallInteger Then
                    Throw New ArgumentException("sKeyFieldName", "Field '" & sKeyFieldName & "' is not an integer field")
                End If
                pValueField = pTable.Fields.Field(nValueFldIndex)
                If Not pValueField.Type = esriFieldType.esriFieldTypeDouble And Not pValueField.Type = esriFieldType.esriFieldTypeSingle Then
                    Throw New ArgumentException("sValueFieldName", "Field '" & sValueFieldName & "' is not an double field")
                End If

                'call GetValuesFromTable with iTable to do actual work
                dicTableValues = GetValuesFromTable(pTable, sKeyFieldName, sValueFieldName)

                Return dicTableValues

            Catch ex As Exception
                ex.Data("sTablePath") = sTablePath
                ex.Data("sKeyFieldName") = sKeyFieldName
                ex.Data("sValueFieldName") = sValueFieldName
                Throw
            End Try

        End Function

        ''' <summary>
        ''' Returns a dictionary of of keys and values from the table
        ''' </summary>
        ''' <param name="pTable">iTable to analyse</param>
        ''' <param name="sKeyFieldName">Fieldname for keys. Must be an integer field.</param>
        ''' <param name="sValueFieldName">Fieldname for values. Must be a single or double field.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Loops through all rows in a table and returns a dictionary with the first value in sValueFieldName
        ''' that is associated with a key in sKeyFieldName
        ''' If a key has more than one value, only the first value encountered will be returned.
        ''' FP April 18 2013
        ''' </remarks>
        Public Function GetValuesFromTable(pTable As ITable, sKeyFieldName As String, sValueFieldName As String) As Dictionary(Of Integer, Double)
            Try

                Dim dicTableValues As New Dictionary(Of Integer, Double)
                Dim nKeyFldIndex As Integer
                Dim nValueFldIndex As Integer
                Dim pKeyField As IField
                Dim pValueField As IField
                Dim pFC As ICursor
                Dim pRow As IRow
                Dim iKey As Integer
                Dim dValue As Double

                '
                'Validation
                '
                'check pTable is an ITable
                If Not TypeOf (pTable) Is ITable Then
                    Throw New ArgumentException("pTable", "pTable input must have an ITable interface")
                End If
                'check input strings are not empty
                If String.IsNullOrEmpty(sKeyFieldName) Then
                    Throw New ArgumentNullException("sKeyFieldName", "The sKeyFieldName input string cannot be null or empty.")
                End If
                If String.IsNullOrEmpty(sValueFieldName) Then
                    Throw New ArgumentNullException("sValueFieldName", "The sValueFieldName input string cannot be null or empty.")
                End If

                'check that field exists
                nKeyFldIndex = pTable.FindField(sKeyFieldName)
                If nKeyFldIndex < 0 Then
                    Throw New ArgumentException("sKeyFieldName", "Field '" & sKeyFieldName & "' does not exists in table.")
                End If

                nValueFldIndex = pTable.FindField(sValueFieldName)
                If nValueFldIndex < 0 Then
                    Throw New ArgumentException("sValueFieldName", "Field '" & sValueFieldName & "' does not exists in table.")
                End If

                'check that field is right type
                pKeyField = pTable.Fields.Field(nKeyFldIndex)
                If Not pKeyField.Type = esriFieldType.esriFieldTypeInteger And Not pKeyField.Type = esriFieldType.esriFieldTypeSmallInteger And Not pKeyField.Type = esriFieldType.esriFieldTypeOID Then
                    Throw New ArgumentException("sKeyFieldName", "Field '" & sKeyFieldName & "' is not an integer field")
                End If
                pValueField = pTable.Fields.Field(nValueFldIndex)
                If Not pValueField.Type = esriFieldType.esriFieldTypeDouble And Not pValueField.Type = esriFieldType.esriFieldTypeSingle Then
                    Throw New ArgumentException("sValueFieldName", "Field '" & sValueFieldName & "' is not an double field")
                End If
                '
                ' end validation
                '

                'create cursor
                pFC = pTable.Search(Nothing, True)
                pRow = pFC.NextRow

                'loop through all rows
                While TypeOf pRow Is IRow

                    'get values.
                    'We already checked during validation that the fields exists and are of the correct type
                    iKey = pRow.Value(nKeyFldIndex)
                    dValue = pRow.Value(nValueFldIndex)

                    If Not dicTableValues.ContainsKey(iKey) Then
                        'only add the value for the first key
                        dicTableValues.Add(iKey, dValue)
                    End If

                    'next row
                    pRow = pFC.NextRow
                End While

                'return dictionary
                Return dicTableValues

            Catch ex As Exception
                ex.Data("sKeyFieldName") = sKeyFieldName
                ex.Data("sValueFieldName") = sValueFieldName
                Throw
            End Try

        End Function

        'Public Sub GetValueFromTable(sTableFile As String, sFieldName As String, ByRef sOutputValue As String)

        'End Sub

        Public Function AddField(pTable As ITable, ByVal sFieldName As String, ByVal eType As esriFieldType, Optional nLength As Integer = 0) As Integer

            If String.IsNullOrEmpty(sFieldName) Then
                Throw New Exception("Null or empty field name passed")
            End If

            If eType = esriFieldType.esriFieldTypeString AndAlso nLength < 1 Then
                Dim ex As New Exception("Invalid field length passed")
                ex.Data("FieldName") = sFieldName
                ex.Data("Length") = nLength
                Throw ex
            End If

            Debug.Assert(eType <> esriFieldType.esriFieldTypeDate Or eType <> esriFieldType.esriFieldTypeDouble Or eType <> esriFieldType.esriFieldTypeInteger Or _
                                 eType <> esriFieldType.esriFieldTypeSingle Or eType <> esriFieldType.esriFieldTypeSmallInteger Or eType <> esriFieldType.esriFieldTypeString, _
                                 "This method can only create integer and double fields")

            Dim iField As Integer = pTable.FindField(sFieldName)
            If iField < 0 Then
                '
                ' Remove any dangerous characters
                '
                sFieldName = sFieldName.Replace(",", String.Empty)
                sFieldName = sFieldName.Replace("'", String.Empty)
                sFieldName = sFieldName.Replace("""", String.Empty)

                If sFieldName.Length >= 10 Then
                    Dim ex As New Exception("The field name specified is more than 10 characters long.")
                    ex.Data("Field Name") = sFieldName
                    Throw ex
                End If

                Dim textField As IField = New FieldClass()
                Dim textFieldEdit As IFieldEdit = CType(textField, IFieldEdit)

                textFieldEdit.Name_2 = sFieldName
                textFieldEdit.Type_2 = eType

                If eType = esriFieldType.esriFieldTypeString AndAlso nLength > 0 Then
                    textFieldEdit.Length_2 = nLength
                End If

                pTable.AddField(textField)
                iField = pTable.FindField(sFieldName)
            End If

            Return iField

        End Function

        Public Function CreateField(ByVal sFieldName As String,
                                    ByVal eType As esriFieldType,
                                    ByVal bEditable As Boolean,
                                    ByVal sAlias As String,
                                    Optional ByVal iLength As Integer = 50) As ESRI.ArcGIS.Geodatabase.IField

            If String.IsNullOrEmpty(sFieldName) Then
                Throw New Exception("Null or empty field name passed")
            End If

            If eType = esriFieldType.esriFieldTypeString AndAlso iLength < 1 Then
                Dim ex As New Exception("Invalid field length passed")
                ex.Data("FieldName") = sFieldName
                ex.Data("Length") = iLength
                Throw ex
            End If


            Dim pField As ESRI.ArcGIS.Geodatabase.IField2 = New ESRI.ArcGIS.Geodatabase.FieldClass
            Dim pFieldEdit As ESRI.ArcGIS.Geodatabase.IFieldEdit2 = CType(pField, ESRI.ArcGIS.Geodatabase.IFieldEdit)

            Select Case eType

                'Set name and type for all fields below
                Case esriFieldType.esriFieldTypeDate,
                     esriFieldType.esriFieldTypeDouble,
                     esriFieldType.esriFieldTypeInteger,
                     esriFieldType.esriFieldTypeOID,
                     esriFieldType.esriFieldTypeSingle,
                     esriFieldType.esriFieldTypeSmallInteger,
                     esriFieldType.esriFieldTypeString

                    pFieldEdit.Name_2 = sFieldName
                    pFieldEdit.Type_2 = eType

                    'Set  alias and editable for all fields but OID
                    If Not eType = esriFieldType.esriFieldTypeOID Then
                        pFieldEdit.AliasName_2 = sAlias
                        pFieldEdit.Editable_2 = True

                        'Set field length of string field
                        If eType = esriFieldType.esriFieldTypeString Then
                            pFieldEdit.Length_2 = iLength
                        End If

                    End If

                Case Else
                    Throw New Exception("Unsupported field type when creating table field.")

            End Select

            Return pField

        End Function

        Public Function CalculateFieldStats(ByVal pFC As ESRI.ArcGIS.Geodatabase.IFeatureClass, ByVal sFieldName As String, Optional pQueryFilter As ESRI.ArcGIS.Geodatabase.IQueryFilter = Nothing) As ESRI.ArcGIS.esriSystem.IStatisticsResults

            If String.IsNullOrEmpty(sFieldName) Then
                Throw New Exception("Null or empty field name passed")
            End If

            Dim pStatsResults As ESRI.ArcGIS.esriSystem.IStatisticsResults = Nothing

            If pFC IsNot Nothing Then
                If TypeOf pFC Is ESRI.ArcGIS.Geodatabase.IFeatureClass Then

                    Dim pSearchCursor As ESRI.ArcGIS.Geodatabase.IFeatureCursor = pFC.Search(pQueryFilter, False)
                    Dim pStats As ESRI.ArcGIS.Geodatabase.IDataStatistics = New ESRI.ArcGIS.Geodatabase.DataStatistics()
                    pStats.Cursor = pSearchCursor
                    pStats.Field = sFieldName
                    pStatsResults = pStats.Statistics()
                End If
            End If

            Return pStatsResults

        End Function

    End Module

End Namespace