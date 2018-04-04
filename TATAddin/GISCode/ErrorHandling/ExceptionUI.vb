Namespace GISCode.ErrorHandling

    Public Class ExceptionUI
        Inherits ExceptionBase

        ''' <summary>
        ''' Handle exception in user interface tools
        ''' </summary>
        ''' <param name="ex">Generic Exception</param>
        ''' <param name="UIMessage">Optional main user interface message for form</param>
        ''' <remarks></remarks>
        Public Shared Sub HandleException(ByVal ex As System.Exception, ByVal UIMessage As String)

            Dim sMessage As String = String.Empty
            AppendExceptionInformation(ex, sMessage)

            'Dim availBytes As New PerformanceCounter("Memory", "Available Bytes", "")
            'Dim sAvailBytes As String = availBytes.NextValue().ToString()
            'sMessage &= vbNewLine & "Available memory in bytes: " & sAvailBytes & vbNewLine
            sMessage &= "Windows: " & My.Computer.Info.OSVersion & vbNewLine
            sMessage &= "Date: " & Now.ToString & vbNewLine
            ExceptionUI.AppendToolInformation(sMessage)
            '
            ' Ensure the wait cursor is reverted back to the default cursor before showing any message box
            '
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default

            'if the exception has a UIMessage parameter, it overrides the optional UIMessage passed to the method
            If ex.Data.Contains("UIMessage") Then
                UIMessage = ex.Data("UIMessage")
            End If

            Debug.WriteLine(sMessage)
            Debug.WriteLine(Now)
            Dim myFrm As New ExceptionForm
            myFrm.txtDetails.Text = sMessage.Trim
            If UIMessage.Length > 0 Then
                Dim origWidth = myFrm.lblException.Width
                myFrm.lblException.Text = UIMessage
                If myFrm.lblException.Width - origWidth > 0 Then
                    myFrm.Width += myFrm.lblException.Width - origWidth
                End If
            End If
            myFrm.ShowDialog()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="lEx">List of GISExceptions returned from GISDataStructures validation.</param>
        ''' <remarks>PGB 2 Sep 2013. Overloaded method to speed up validating of GIS data structures. Simply pass
        ''' the list of errors from the validation here and if any errors exist then the first one will be
        ''' displayed using the usual error message handling.</remarks>
        Protected Sub HandleException(ByVal lEx As List(Of GISException))
            If TypeOf lEx Is List(Of GISException) Then
                If lEx.Count > 0 Then
                    HandleException(lEx(0))
                End If
            End If
        End Sub

        Public Shared Sub HandleException(ByVal ex As Exception)
            ExceptionUI.HandleException(ex, "")
        End Sub

        ''' <summary>
        ''' Returns a string that contains the name and version of the software tool in which the error has occurred
        ''' </summary>
        ''' <remarks></remarks>
        Protected Shared Shadows Sub AppendToolInformation(ByRef sMessage As String)

            Dim sToolSpecificMessage As String = String.Empty

            ' First insert the base class tool information
            ExceptionBase.AppendToolInformation(sMessage)

            Try
                '
                Dim sToolSpecificMessageUI As String = "ArcMap: " & Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion & vbNewLine
                sMessage &= sToolSpecificMessageUI
            Catch ex As Exception
                '
                ' Do Nothing. This information is a nice-to-have, and not essential
                '
            End Try

        End Sub

    End Class

End Namespace