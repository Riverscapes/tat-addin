Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Management

Namespace GISCode.OperatingSystem
    Module OperatingSystem


        Public Function Is64BitOperatingSystem() As Boolean
            If IntPtr.Size = 8 Then
                ' 64-bit programs run only on Win64
                Return True
            Else
                ' 32-bit programs run on both 32-bit and 64-bit Windows
                ' Detect whether the current process is a 32-bit process 
                ' running on a 64-bit system.
                Dim flag As Boolean
                Return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") AndAlso IsWow64Process(GetCurrentProcess(), flag)) AndAlso flag)
            End If
        End Function

        '''.<summary>
        ''' The function determins whether a method exists in the export 
        ''' table of a certain module.
        ''' </summary>
        ''' <param name="moduleName">The name of the module</param>
        ''' <param name="methodName">The name of the method</param>
        ''' <returns>
        ''' The function returns true if the method specified by methodName 
        ''' exists in the export table of the module specified by moduleName.
        ''' </returns>
        Private Function DoesWin32MethodExist(ByVal moduleName As String, ByVal methodName As String) As Boolean
            Dim moduleHandle As IntPtr = GetModuleHandle(moduleName)
            If moduleHandle = IntPtr.Zero Then
                Return False
            End If
            Return (GetProcAddress(moduleHandle, methodName) <> IntPtr.Zero)
        End Function


        <DllImport("kernel32.dll")> _
        Private Function GetCurrentProcess() As IntPtr
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto)> _
        Private Function GetModuleHandle(ByVal moduleName As String) As IntPtr
        End Function

        <DllImport("kernel32", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Private Function GetProcAddress(ByVal hModule As IntPtr, <MarshalAs(UnmanagedType.LPStr)> ByVal procName As String) As IntPtr
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Private Function IsWow64Process(ByVal hProcess As IntPtr, ByRef wow64Process As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function


    End Module

End Namespace
