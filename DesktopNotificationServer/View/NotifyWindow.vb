Imports System.Windows.Forms

Public Class NotifyWindow
    Private WithEvents server As Server
    Private WithEvents notify As NotifyIcon
    Private Sub placeholder_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NotificationBin.Initialize()
        NotificationBin.InteractToRead = True

        server = New Server()
        server.IP_Whitelist.Add(New Net.IPEndPoint(Net.IPAddress.Parse("192.168.0.100"), 0))

        Hide()

        notify = New NotifyIcon With {
            .Text = "Remote notification service",
            .Icon = New System.Drawing.Icon("C:\Users\USERNAME_HERE\Pictures\favicon.ico"),
            .Visible = True
        }

        AddHandler NotificationBin.NewNotification, AddressOf NotificationAvailable
    End Sub

    Private Sub NotificationAvailable(ByVal Notification As Notification)
        My.Settings.UnreadNotifications.Add(Notification.ToString())

        notify.ShowBalloonTip(5000, Notification.Title,
                              Notification.Sender & ": " & Notification.Body, Integer.Parse(Notification.Severity))
    End Sub

    'Private Sub NotificationClicked(sender As Object, e As EventArgs) Handles notify.BalloonTipClicked
    '    My.Settings.UnreadNotifications.RemoveAt(My.Settings.UnreadNotifications.Count - 1)
    'End Sub
End Class