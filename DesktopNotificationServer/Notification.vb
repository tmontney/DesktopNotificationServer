Public Class Notification
    Public Property Title As String
    Public Property Body As String
    Public Property Sender As String
    Public Property Severity As SeverityEnum

    Public Enum SeverityEnum As Integer
        None = 0
        Low = 1
        Normal = 2
        High = 3
    End Enum

    Public Overrides Function ToString() As String
        Return Title & "," & Sender & "," & Body & "," & Severity
    End Function
End Class
