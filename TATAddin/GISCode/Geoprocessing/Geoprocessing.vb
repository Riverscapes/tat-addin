Namespace GISCode.Geoprocessing

    Public Module Geoprocessing

        Public Enum SyntaxTypes
            VB
            Python
            Python_93
        End Enum

        Public Function GetSyntaxType(ByVal eType As SyntaxTypes)
            Dim sType As String = "PYTHON"
            Select Case eType
                Case Geoprocessing.SyntaxTypes.VB
                    sType = "VB"
                Case Geoprocessing.SyntaxTypes.Python_93
                    sType = "PYTHON_9.3"
                Case Else
                    Dim ex As New Exception("Unhandled syntax type")
                    ex.Data("Type") = eType.ToString
            End Select
            Return sType
        End Function

        'Public Function ConvertPythonToVBSyntax(ByVal sPythonSyntax As String) As String

        '    Dim nCount As Integer = 0
        '    Do While sPythonSyntax.Contains("!")
        '        Dim nIndex As Integer = sPythonSyntax.IndexOf("!")
        '        If nIndex < sPythonSyntax.Length AndAlso n Then
        '            If nCount Mod 2 = 0 Then
        '            sPythonSyntax = sPythonSyntax.Substring(0,sPythonSyntax.Length 
        '            Else
        '            sPythonSyntax = 
        '            End If



        '    Loop



        'End Function

    End Module

End Namespace