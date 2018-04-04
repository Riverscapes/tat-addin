Imports System.Runtime.InteropServices

Namespace GISCode.Toolbox

    Public Module ToolBox

        ''' <summary>
        ''' Run a python script using the argument parameters
        ''' </summary>
        ''' <param name="sToolName">Name of the tool to run</param>
        ''' <param name="lParameters">List of parameter values, specific to the tool</param>
        ''' <remarks>http://gis.stackexchange.com/questions/11543/how-can-i-execute-my-custom-models-tools-in-vb-net</remarks>
        Public Sub RunTool(sToolboxPath As String, sToolName As String, lParameters As List(Of String))

            Dim pGp As ESRI.ArcGIS.Geoprocessing.IGeoProcessor = New ESRI.ArcGIS.Geoprocessing.GeoProcessor
            Dim pParamArray As ESRI.ArcGIS.esriSystem.IVariantArray = New ESRI.ArcGIS.esriSystem.VarArray
            Dim pResult As ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult = Nothing

            Try
                ' Add the custom toolbox containing the model tool
                'sToolboxPath = IO.Path.Combine(IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "champ-topo-toolbar-python\ChaMPGISTools2013.tbx")

                ' Check that the toolbox file is present
                If Not IO.File.Exists(sToolboxPath) Then
                    Dim ex As New Exception("Toolbox does not exist")
                    ex.Data("Toolbox Path") = sToolboxPath
                    Throw ex
                End If

                pGp.OverwriteOutput = True
                pGp.AddToolbox(sToolboxPath)

                ' Check that duplicate toolboxes are not present
                Dim sToolboxName As String = IO.Path.GetFileNameWithoutExtension(sToolboxPath)
                If CheckForDuplicateToolboxes(pGp, sToolboxName) Then
                    MsgBox("Error: Duplicate toolboxes detected. Please remove all copies of " & sToolboxName & " from ArcToolbox and rerun this tool.", MsgBoxStyle.Critical, My.Resources.ApplicationNameLong)
                    Dim ex As New Exception("Duplicate Toolbox")
                    ex.Data("Toolbox Name") = sToolboxName
                    Throw ex
                End If

                ' Prepare Parameters
                For Each sParam As String In lParameters
                    pParamArray.Add(sParam)
                Next

                'pParamArray = New ESRI.ArcGIS.esriSystem.VarArray

                ' Execute the Model tool
                pResult = pGp.Execute(sToolName, pParamArray, Nothing)
                Dim gp_msg As String = ""

            Catch ex As Exception
                'MsgBox("Error with Python Geoprocessing. Please check the Geoprocessing Results Panel.", MsgBoxStyle.Critical, My.Resources.ApplicationNameLong)
                Dim ex2 As New Exception("Error processing toolbox.", ex)
                ex2.Data("Tool Name") = sToolName
                ex2.Data("Toolbox Path") = sToolboxPath
                '
                ' Parameters
                '
                If TypeOf lParameters Is List(Of String) Then
                    For i As Integer = 0 To lParameters.Count - 1
                        ex2.Data.Add("Parameter " & i.ToString, lParameters(i).ToString)
                    Next
                End If
                '
                ' Add the geoprocessing messages to the exception information
                '
                If TypeOf pResult Is ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult Then
                    If pResult.Status <> ESRI.ArcGIS.esriSystem.esriJobStatus.esriJobSucceeded Then
                        For i As Integer = 0 To pResult.MessageCount - 1
                            ex2.Data.Add("GP Result Message " & i.ToString, pResult.GetMessage(i))
                        Next
                    End If
                End If

                For i As Integer = 0 To pGp.MessageCount - 1
                    ex2.Data("GP Message " & i.ToString) = pGp.GetMessage(i)
                Next
                Throw ex2
            End Try
        End Sub

        ''' <summary>
        ''' Run a python script using the argument parameters
        ''' </summary>
        ''' <param name="sToolName">Name of the tool to run</param>
        ''' <param name="lParameters">List of parameter values, specific to the tool</param>
        ''' <remarks>http://gis.stackexchange.com/questions/11543/how-can-i-execute-my-custom-models-tools-in-vb-net</remarks>
        Public Sub RunCHaMPTopoToolbarTool(sToolName As String, lParameters As List(Of String))

            ' Add the custom toolbox containing the model tool
            Dim sCurrentCHaMPPythonToolboxName As String = "CHaMPGISTools.tbx"
            Dim sToolboxPath As String = IO.Path.Combine(IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "champ-topo-toolbar-python\" & sCurrentCHaMPPythonToolboxName)

            RunTool(sToolboxPath, sToolName, lParameters)

        End Sub

        Public Sub StoreNote(sSurveyGDB As String, sToolName As String, sStatus As String, Optional sMessages As String = "")

            Try
                Dim pWS As ESRI.ArcGIS.Geodatabase.IWorkspace = GISDataStructures.GetWorkspace(sSurveyGDB, GISDataStructures.GISDataStorageTypes.FileGeodatase)
                If TypeOf pWS Is ESRI.ArcGIS.Geodatabase.IWorkspace Then
                    Dim pTable As ESRI.ArcGIS.Geodatabase.ITable = GISCode.Table.Open(pWS, "Log")
                    If TypeOf pTable Is ESRI.ArcGIS.Geodatabase.ITable Then

                        Dim pRow As ESRI.ArcGIS.Geodatabase.IRow = pTable.CreateRow()
                        Dim rowSubTypes As ESRI.ArcGIS.Geodatabase.IRowSubtypes = pRow
                        rowSubTypes.InitDefaultValues()

                        Dim nStatusField As Integer = pTable.FindField("Status")
                        If nStatusField >= 0 AndAlso Not String.IsNullOrEmpty(sStatus) Then
                            pRow.Value(nStatusField) = sStatus.Substring(0, Math.Min(10, sStatus.Length))
                        End If

                        Dim nToolField As Integer = pTable.FindField("ToolName")
                        If nToolField >= 0 AndAlso Not String.IsNullOrEmpty(sToolName) Then
                            pRow.Value(nToolField) = sToolName.Substring(0, Math.Min(20, sToolName.Length))
                        End If

                        Dim nTimeField As Integer = pTable.FindField("TIMESTAMP")
                        If nTimeField >= 0 Then
                            Dim sTime As String = Now.ToString("ddd MMM dd HH:mm:ss yyy")
                            pRow.Value(nTimeField) = sTime.Substring(0, Math.Min(30, sTime.Length))
                        End If

                        Dim nMessageField As Integer = pTable.FindField("Message")
                        If nMessageField >= 0 AndAlso Not String.IsNullOrEmpty(sMessages) Then
                            ' GDB tables don't have a good way of showing line breaks
                            sMessages = sMessages.Replace(vbNewLine, ", ")
                            sMessages = sMessages.Replace(vbTab, " ")
                            If sMessages.Length > 255 Then
                                ' limited field length
                                sMessages = sMessages.Substring(0, Math.Min(1000, sMessages.Length))
                            End If
                            pRow.Value(nMessageField) = sMessages
                        End If

                        Try
                            pRow.Store()
                        Catch ex As Exception
                            Throw New Exception("Failed to store row in table")
                        Finally
                            If TypeOf pRow Is ESRI.ArcGIS.Geodatabase.IRow Then
                                Runtime.InteropServices.Marshal.ReleaseComObject(pRow)
                            End If
                        End Try
                    Else
                        'Throw New Exception("Unable to open Log table in survey geodatabase")
                    End If
                Else
                    Throw New Exception("Unable to open Survey GDB as workspace")
                End If
            Catch ex As Exception
                ex.Data("Survey GDB") = sSurveyGDB
                ex.Data("Table") = "Log"
                Throw
            End Try

        End Sub

        Public Sub StoreUserComputerNote(ByVal sSurveyGDB As String)

            Try
                Dim sMsg As String = "Machine: " & Environment.MachineName & ", UserName: " & Environment.UserName & ", OS: " & Environment.OSVersion.VersionString
                StoreNote(sSurveyGDB, "CHaMP Topo Toolbar", "Message", sMsg)

            Catch ex As Exception
                Debug.WriteLine("Error obtaining computer and user name information for purposes of writing it to the log file." & ex.Message)
            End Try

        End Sub

        Private Function CheckIfToolboxInGeoprocessor(ByVal pGp As ESRI.ArcGIS.Geoprocessing.IGeoProcessor, ByVal sToolBoxName As String)
            Dim bExists As Boolean = False

            Dim lToolbox As ESRI.ArcGIS.Geoprocessing.IGpEnumList = pGp.ListToolboxes("*")

            Dim tool As String = lToolbox.Next()
            While Not String.IsNullOrEmpty(tool)
                Debug.Print("Toolbox name: {0}", tool)
                If tool.Contains(sToolBoxName) Then
                    bExists = True
                    Return bExists
                End If
                tool = lToolbox.Next()
            End While
            Return bExists

        End Function

        Private Function CheckForDuplicateToolboxes(ByVal pGp As ESRI.ArcGIS.Geoprocessing.IGeoProcessor, ByVal sToolBoxName As String)
            Dim bDuplicate As Boolean = False

            Dim lToolbox As ESRI.ArcGIS.Geoprocessing.IGpEnumList = pGp.ListToolboxes(sToolBoxName)

            Dim tool As String = lToolbox.Next()
            Dim iCount As Integer = 0
            While Not String.IsNullOrEmpty(tool)
                Debug.Print("Toolbox name: {0}", tool)
                If tool.Contains(sToolBoxName) Then
                    iCount = iCount + 1
                End If
                tool = lToolbox.Next()
            End While

            If iCount >= 2 Then
                bDuplicate = True
            End If

            Return bDuplicate
        End Function

    End Module

End Namespace