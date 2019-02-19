Imports Microsoft.Isam.Esent.Collections.Generic

Public Class CoincidentPoints

    Public Shared Function UseSortShellCommand(ByVal tempDir As String, ByVal pInFilePath As String)

        Dim sortBatFile As String = "sort /M 712000 /R " &
                                        Chr(34) & pInFilePath & Chr(34) & " " &
                                       "/O " & Chr(34) & tempDir & "\" & System.IO.Path.GetFileNameWithoutExtension(pInFilePath) & "_Sorted.txt" & Chr(34) & vbCrLf

        'CREATE THE BATCH FILE IN THE %TEMP% FOLDER CREATED BY WindowsManagement.CreateTemporaryDirectory
        Dim sortBatFilePath As String = IO.Path.Combine(tempDir, "runSort.bat")

        Dim batFileWriter As New System.IO.StreamWriter(sortBatFilePath, False)
        batFileWriter.WriteLine(sortBatFile)
        batFileWriter.Close()
        Shell(sortBatFilePath, AppWinStyle.NormalFocus, True)

        If System.IO.File.Exists(tempDir & "\" & System.IO.Path.GetFileNameWithoutExtension(pInFilePath) & "_Sorted.txt") = True Then
            Dim sortedPointCloudPath As String = tempDir & "\" & System.IO.Path.GetFileNameWithoutExtension(pInFilePath) & "_Sorted.txt"
            Return sortedPointCloudPath
        Else
            Return Nothing
        End If

    End Function

    Public Shared Function GetLineCount(ByVal pFile As String)

        Dim fileReader As New System.IO.StreamReader(pFile, detectEncodingFromByteOrderMarks:=True)
        Dim ct As Integer = 0
        Do While (fileReader.Peek() > -1)
            fileReader.ReadLine()
            ct += 1
        Loop
        fileReader.Close()
        Return ct
    End Function

    Public Shared Sub SubsetFileByChunks(ByVal tempDir As String, ByVal inFile As String)

        Using reader As New IO.StreamReader(inFile)
            Const blockSize As Integer = 50000 * 1024
            Dim buffer(blockSize - 1) As Char
            Dim charCount As Integer
            'Dim builder As New System.Text.StringBuilder
            Dim ct As Integer = 0
            Do Until reader.EndOfStream
                Using fileWriter As New IO.StreamWriter(tempDir & "\" & System.IO.Path.GetFileNameWithoutExtension(inFile) & "_Sub" & ct.ToString() & ".txt")
                    charCount = reader.ReadBlock(buffer, 0, blockSize)
                    Dim builder As New System.Text.StringBuilder
                    'builder.Clear()
                    builder.Append(buffer, 0, charCount)

                    If charCount = blockSize Then
                        'Read up to the next line break and append to the block just read.
                        builder.Append(reader.ReadLine())
                    End If

                    Dim textBlock = builder.ToString()
                    fileWriter.Write(textBlock)
                    textBlock = Nothing
                    builder = Nothing
                    ct += 1
                End Using
            Loop
        End Using

    End Sub

    Public Shared Sub findCoincidentPointsSmallFile(ByVal inFile As String,
                                                    ByVal outFilePath As String,
                                                    ByVal seperator As String,
                                                    ByVal precision As Integer,
                                                    Optional ByVal setPrecision As Boolean = False,
                                                    Optional ByVal largeFileProcess As Boolean = False)

        Using fileReader As New System.IO.StreamReader(inFile, detectEncodingFromByteOrderMarks:=True)
            fileReader.ReadLine() 'Skip Header
            Dim valueDict As Dictionary(Of String, Double) = New Dictionary(Of String, Double) '(capacity:=25000000)
            Using fileWriter As New System.IO.StreamWriter(System.IO.Path.GetDirectoryName(outFilePath) & "\" & System.IO.Path.GetFileNameWithoutExtension(outFilePath) & ".txt", False)

                If largeFileProcess = False Then
                    fileWriter.WriteLine("X,Y,Uncert")
                End If

                Try

                    Do While (fileReader.Peek() > -1)
                        Dim newLine As String = fileReader.ReadLine.Replace(vbCr, "").Replace(vbLf, "")

                        If String.Compare(newLine, "") = 0 Then
                            Continue Do
                        End If

                        Dim key As String
                        Dim Z As Single

                        Z = System.Single.Parse(newLine.Split(seperator)(2))

                        key = newLine.Split(seperator)(0) & seperator & newLine.Split(seperator)(1)

                        If setPrecision = True Then
                            key = Math.Round(System.Double.Parse(key.Split(seperator)(0)), precision) & seperator & Math.Round(System.Double.Parse(key.Split(seperator)(1)), precision)
                        ElseIf setPrecision = False Then
                            key = newLine.Split(seperator)(0) & seperator & newLine.Split(seperator)(1)
                        End If



                        If valueDict.ContainsKey(key) = False Then
                            valueDict.Add(key, Z)
                        ElseIf valueDict.ContainsKey(key) Then
                            Dim outLine As String = key.Split(seperator)(0) & "," & key.Split(seperator)(1) & "," & Math.Abs(valueDict(key) - Z)
                            fileWriter.WriteLine(outLine)
                            valueDict.Remove(key)
                        End If

                    Loop

                    If largeFileProcess = False Then
                        'MsgBox("File was fully processed and written to output.", MsgBoxStyle.OkOnly)
                    End If

                Catch ex As Exception
                    If largeFileProcess = False Then
                        MsgBox("File was not able to fully process due to its size. Writing what was processed to text file output.", MsgBoxStyle.OkOnly)
                    End If
                Finally
                    valueDict.Clear()
                    valueDict = Nothing
                End Try
            End Using
        End Using



    End Sub

    Public Shared Sub findCoincidentPointsLargeFile(ByVal inFile As String,
                                              ByVal outFilePath As String,
                                              ByVal seperator As String,
                                              ByVal precision As Integer,
                                              ByVal tempDir As String,
                                              Optional ByVal setPrecision As Boolean = False)

        Using fileReader As New System.IO.StreamReader(inFile, detectEncodingFromByteOrderMarks:=True)
            fileReader.ReadLine() 'Skip Header
            'Dim valueDict As PersistentDictionary(Of String, Double) = New PersistentDictionary(Of String, Double)(tempDir)
            Dim valueDict As Dictionary(Of String, Double) = Nothing

            Using fileWriter As New System.IO.StreamWriter(System.IO.Path.GetDirectoryName(outFilePath) & "\" & System.IO.Path.GetFileNameWithoutExtension(outFilePath) & ".txt", False)
                fileWriter.WriteLine("X,Y,Uncert")


                Try

                    Do While (fileReader.Peek() > -1)
                        Dim newLine As String = fileReader.ReadLine.Replace(vbCr, "").Replace(vbLf, "")

                        If String.Compare(newLine, "") = 0 Then
                            Continue Do
                        End If

                        Dim key As String
                        Dim Z As Single

                        Z = System.Single.Parse(newLine.Split(seperator)(2))

                        key = newLine.Split(seperator)(0) & seperator & newLine.Split(seperator)(1)

                        If setPrecision = True Then
                            key = Math.Round(System.Double.Parse(key.Split(seperator)(0)), precision) & seperator & Math.Round(System.Double.Parse(key.Split(seperator)(1)), precision)
                        ElseIf setPrecision = False Then
                            key = newLine.Split(seperator)(0) & seperator & newLine.Split(seperator)(1)
                        End If

                        If valueDict.ContainsKey(key) = False Then
                            valueDict.Add(key, Z)
                        ElseIf valueDict.ContainsKey(key) Then
                            Dim outLine As String = key.Split(seperator)(0) & "," & key.Split(seperator)(1) & "," & Math.Abs(valueDict(key) - Z)
                            fileWriter.WriteLine(outLine)
                            valueDict.Remove(key)
                        End If

                    Loop

                    'valueDict.Dispose()
                    MsgBox("File was fully processed and written to output.", MsgBoxStyle.OkOnly)
                Catch ex As Exception

                    MsgBox("File was not able to fully process due to its size. Writing what was processed to output.", MsgBoxStyle.OkOnly)

                Finally
                    'valueDict.Dispose()
                    System.IO.Directory.Delete(tempDir, True)
                End Try
            End Using
        End Using
    End Sub

    Public Shared Sub MergeFiles(ByVal pInputFile As String, ByVal pOutputFolder As String)

        Dim streamWriter As New IO.StreamWriter(pOutputFolder & "\" & System.IO.Path.GetFileNameWithoutExtension(pInputFile) & "_CoincidentPoints.txt")
        streamWriter.WriteLine("x,y,uncert")
        streamWriter.Close()

        Dim tempDir As String = IO.Path.Combine(Environ("TEMP"), "CoincidentPoints")
        Dim allFiles = System.IO.Directory.GetFiles(tempDir, "*_Coincident.txt")
        For Each file As String In allFiles
            System.IO.File.AppendAllText(pOutputFolder & "\" & System.IO.Path.GetFileNameWithoutExtension(pInputFile) & "_CoincidentPoints.txt", System.IO.File.ReadAllText(file))
        Next file

    End Sub

    Public Shared Function CreateCoincidentPointsShapefile(ByVal pOutputShpPath As String, ByVal spatialRefPath As String)


        Dim pythonFile = IO.Path.Combine(IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location),
                                         My.Resources.ScriptDirectory,
                                         "toPCAT_outPutToShp_OsgeoOption.py")

        If Not IO.File.Exists(pythonFile) Then
            MsgBox("The python script used to run this tool is missing from your GCD installation folder.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
            Return False
        End If

        'CREATE THE BATCH FILE IN THE %TEMP% FOLDER OF THE USER
        Dim sBatchFilePath As String = Environ("TEMP")
        sBatchFilePath = IO.Path.Combine(sBatchFilePath, "runCoincidentPointsToShapefile.bat")

        'CREATE BATCH TEMPLATE
        Dim batchTemplate As String
        Dim esriPythonPath As String = WindowsManagement.getESRI_PythonPath()

        batchTemplate = esriPythonPath & " " &
                        Chr(34) & pythonFile & Chr(34) & " " &
                        "--decimatedInputTxt " & Chr(34) & pOutputShpPath & Chr(34) & " " &
                        "--outputShp " & Chr(34) & System.IO.Path.GetDirectoryName(pOutputShpPath) & "\" & System.IO.Path.GetFileNameWithoutExtension(pOutputShpPath) & ".shp" & Chr(34) & " " &
                        "--SpatialReference " & Chr(34) & spatialRefPath & Chr(34) & vbCrLf

        Dim batchWriter As New System.IO.StreamWriter(sBatchFilePath, False)
        batchWriter.WriteLine(batchTemplate)
        batchWriter.Close()

        Shell(sBatchFilePath, AppWinStyle.NormalFocus, True)

        If System.IO.File.Exists(System.IO.Path.GetDirectoryName(pOutputShpPath) & "\" & System.IO.Path.GetFileNameWithoutExtension(pOutputShpPath) & ".shp") = True Then
            Return True
        Else
            Return False
        End If

    End Function


End Class
