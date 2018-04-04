Imports System.Xml

Namespace GISCode.ErrorHandling

    Public MustInherit Class ExceptionBase

        ''' <summary>
        ''' Recursive function that appends exception data and inner exceptions to the exception message
        ''' </summary>
        ''' <param name="ex">Exception to process</param>
        ''' <param name="sMessage">Message text that is updated by this method</param>
        ''' <remarks>Note that this method is recursive</remarks>
        Protected Shared Sub AppendExceptionInformation(ex As Exception, ByRef sMessage As String)

            If TypeOf ex.InnerException Is Exception Then
                ' At least one more exception level to report, so make it clear where the break is.
                sMessage &= "------------------------------------------------"
                sMessage &= vbNewLine & "EXCEPTION"
            End If
            sMessage &= vbNewLine & ex.Message
            If Not String.IsNullOrEmpty(ex.StackTrace) Then
                sMessage &= vbNewLine & " --- Stacktrace --- "
                sMessage &= vbNewLine & ex.StackTrace
            End If

            If ex.Data.Contains("Parameters") Then
                Dim params As Collections.Hashtable = ex.Data("Parameters")
                sMessage &= vbNewLine & " --- Parameters --- "
                For Each de As Collections.DictionaryEntry In params
                    sMessage &= vbNewLine & " " & de.Key & " = " & de.Value
                Next
            End If

            If ex.Data.Count > 0 Then
                sMessage &= vbNewLine & " --- Exception Data --- "
                For Each de As Collections.DictionaryEntry In ex.Data
                    If Not de.Key Is Nothing Then
                        sMessage &= vbNewLine & " " & de.Key.ToString & " = "
                        If de.Value Is Nothing Then
                            sMessage &= "Nothing"
                        Else
                            sMessage &= de.Value.ToString
                        End If
                    End If
                Next
            End If

            If TypeOf ex.InnerException Is Exception Then
                AppendExceptionInformation(ex.InnerException, sMessage)
            End If

        End Sub

        ''' <summary>
        ''' Returns a string that contains the name and version of the software tool in which the error has occurred
        ''' </summary>
        ''' <remarks>This method is overridden in every derived class and never used directly. The only
        ''' information that should be injected into the message here is that pertaining the the "tool" itelsef.
        ''' i.e. the host software product (RBT Desktop, RBT Console, GCD Desktop etc). Do not include anything
        ''' about ArcGIS or other DLL dependencies that might not be used by other tools.</remarks>
        Protected Shared Sub AppendToolInformation(ByRef sMessage As String)

            Dim sToolSpecificMessage As String = String.Empty

            Try
                sToolSpecificMessage = String.Format("{0}Tool: {1} version {2}{0}", vbNewLine, My.Resources.ApplicationNameLong, System.Reflection.Assembly.GetExecutingAssembly.GetName.Version)
                sMessage &= sToolSpecificMessage
            Catch ex As Exception
                '
                ' Do Nothing. This information is a nice-to-have, and not essential
                '
            End Try

        End Sub

        Public Shared Sub AddGPMessagesToException(ByRef ex As Exception, GP As ESRI.ArcGIS.Geoprocessor.Geoprocessor)

            For i As Integer = 0 To GP.MessageCount - 1
                ex.Data("GP Message " & i.ToString) = GP.GetMessage(i)
            Next
        End Sub

        Public Shared Sub ProcessGPExceptions(ByVal GP As ESRI.ArcGIS.Geoprocessor.Geoprocessor)

            If Not TypeOf GP Is ESRI.ArcGIS.Geoprocessor.Geoprocessor Then
                Throw New Exception("Invalid Geoprocessor")
            End If

            Dim GPmessage As String
            GPmessage = ""
            For index As Integer = 0 To GP.MessageCount - 1
                GPmessage &= GP.GetMessage(index) & vbNewLine
            Next
            If GP.MaxSeverity > 1 Then
                Throw New Exception(GPmessage)
            Else
                Debug.WriteLine(GPmessage)
            End If
        End Sub

    End Class

End Namespace