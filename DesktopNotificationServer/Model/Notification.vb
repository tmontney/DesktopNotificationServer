Public Class NotificationBin
    Private Shared Property Notifications As List(Of Notification)

    Public Shared Property InteractToRead As Boolean

    Public Shared Event NewNotification(ByVal Notification As Notification)
    Public Shared Event NotificationRemoved(ByVal Notification As Notification)

    Public Shared Sub Initialize()
        Notifications = New List(Of Notification)
    End Sub

    Public Shared Sub Add(ByVal Notification As Notification)
        Notifications.Add(Notification)
        RaiseEvent NewNotification(Notification)
    End Sub

    Public Shared Sub Remove(ByVal Notification As Notification)
        Notifications.Remove(Notification)
        RaiseEvent NotificationRemoved(Notification)
    End Sub

    Public Shared Function GetItem(ByVal Index As Integer) As Notification
        If Index > -1 And Index < Notifications.Count Then
            If InteractToRead = False Then Notifications(Index).Read = True
            Return Notifications(Index)
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function GetItem(ByVal Item As Notification) As Notification
        Return GetItem(Notifications.IndexOf(Item))
    End Function
End Class

Public Class Notification
    Public Property Title As String
    Public Property Body As String
    Public Property Sender As String
    Public Property Severity As SeverityEnum
    Public Property Read As Boolean

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
