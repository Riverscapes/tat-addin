
Namespace GISCode.ErrorHandling

    Public Class GISException
        Inherits Exception
        '
        ' The order of these errors is important. It is used by the CHaMP Topo toolbar 
        ' to determine which image icon to display in the validate window. Do not change
        ' order add items without reading that code.
        '
        Public Enum ErrorTypes
            Warning
            CriticalError
            Missing
        End Enum

        Private m_eType As ErrorTypes
        Private m_sSolution As String

        Public ReadOnly Property Type As ErrorTypes
            Get
                Return m_eType
            End Get
        End Property

        Public ReadOnly Property TypeAsString
            Get
                Return GetTypeAsString(m_eType)
            End Get
        End Property

        Public ReadOnly Property Solution As String
            Get
                Return m_sSolution
            End Get
        End Property

        Public Sub New(eType As ErrorTypes, sMessage As String, Optional sSolution As String = "")
            MyBase.New(sMessage)
            m_eType = eType
            m_sSolution = sSolution
        End Sub

        Public Sub New(eType As ErrorTypes, sMessage As String, exInner As Exception, Optional sSolution As String = "")
            MyBase.New(sMessage, exInner)
            m_eType = eType
            m_sSolution = sSolution
        End Sub

        Public Shared Function GetTypeAsString(eType As ErrorTypes) As String
            Dim sResult As String = "Unknown"
            Select Case eType
                Case ErrorTypes.CriticalError : sResult = "Error"
                Case ErrorTypes.Missing : sResult = "Missing"
                Case ErrorTypes.Warning : sResult = "Warning"
            End Select
            Return sResult
        End Function

        Public Function GetListItem() As System.Windows.Forms.ListViewItem

            Dim lItem As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem(GetTypeAsString(Type))
            Dim nImageIndex As Integer = 0

            lItem.ImageKey = nImageIndex
            'lItem.StateImageIndex = Type ' m_eStatus
            'lItem.SubItems.Add(StatusString(m_eStatus))
            'lItem.SubItems.Add(m_sPath)
            lItem.SubItems.Add(Message) ' m_sRemarks)
            If String.IsNullOrEmpty(Solution) Then
                Dim sExceptionData As String = String.Empty
                For Each sDataKey As String In Data.Keys
                    sExceptionData &= sDataKey & ", """ & Data.Item(sDataKey) & """; "
                Next
                If Not String.IsNullOrEmpty(sExceptionData) Then
                    lItem.SubItems.Add(sExceptionData.Substring(0, sExceptionData.Length - 2))
                End If
            Else
                lItem.SubItems.Add(Solution) ' m_sResolution)
            End If

            Select Case Type '  m_eStatus
                Case ErrorTypes.Missing, ErrorTypes.CriticalError ' StatusTypes.Missing, StatusTypes.Invalid
                    lItem.ForeColor = Drawing.Color.Red

                Case ErrorTypes.Warning ' StatusTypes.Warning
                    lItem.ForeColor = Drawing.Color.DarkOrange
            End Select

            Return lItem

        End Function

    End Class

End Namespace