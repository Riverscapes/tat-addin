Imports ESRI.ArcGIS.esriSystem

Namespace GISCode

    Public Class Extensions

        Public Enum ESRI_Extensions
            SpatialAnalyst
            Analyst3D
            NetworkAnalyst
        End Enum

        Private Const m_sSpatialAnalyst As String = "Spatial Analyst"
        Private Const m_sNetworkAnalyst As String = "Network Analyst"
        Private Const m_s3DAnalyst As String = "3D Analyst"

        Public Shared Function ObtainExtension(eExtension As ESRI_Extensions, Optional bShowWarningMessages As Boolean = False) As esriExtensionState

            ' get a variable to the extension manager
            Dim factoryType As Type = Type.GetTypeFromProgID("esriSystem.ExtensionManager")
            Dim extensionManager As IExtensionManager = CType(Activator.CreateInstance(factoryType), IExtensionManager)

            'obtain a reference to the necessary extension
            Dim sExtensionName As String = ""
            Dim uid As IUID = New UIDClass()
            uid.Value = GetExtensionUID(eExtension, sExtensionName)
            Dim extension As IExtension = extensionManager.FindExtension(uid)
            Dim extensionConfig As IExtensionConfig = CType(extension, IExtensionConfig)

            ' check if the extension is already enabled
            If Not extensionConfig.State = esriExtensionState.esriESEnabled Then
                If Not (extensionConfig.State = esriExtensionState.esriESUnavailable) Then
                    ' Enable the license.
                    extensionConfig.State = esriExtensionState.esriESEnabled
                Else ' Handle the case when the license is not available.
                    ' Provide an error message or exit to avoid running unavailable functionality.
                    If bShowWarningMessages Then
                        MsgBox("The " & sExtensionName & " extension is unavailable. Check the extension configurations under the Customize/Extensions menu in ArcMap.", MsgBoxStyle.Information, My.Resources.ApplicationNameLong)
                    End If
                    Return esriExtensionState.esriESUnavailable
                End If
            End If

            Return extensionConfig.State

        End Function

        Private Shared Function GetExtensionUID(eExtension As ESRI_Extensions, ByRef sExtensionName As String) As String

            Dim sExtensionUID As String = String.Empty
            Select Case eExtension
                Case ESRI_Extensions.SpatialAnalyst
                    sExtensionName = m_sSpatialAnalyst
                    sExtensionUID = "esriSpatialAnalystUI.SAExtension"

                Case ESRI_Extensions.Analyst3D
                    sExtensionName = m_s3DAnalyst
                    sExtensionUID = "esricore.DDDEnvironment"

                Case ESRI_Extensions.NetworkAnalyst
                    sExtensionName = m_sNetworkAnalyst
                    sExtensionUID = "esriNetworkAnalystUI.NetworkAnalystExtension"

                Case Else
                    Dim ex As New Exception("Unhandled extension type.")
                    ex.Data("Extension Enum") = eExtension.ToString
                    Throw ex
            End Select

            Return sExtensionUID

        End Function

    End Class

End Namespace