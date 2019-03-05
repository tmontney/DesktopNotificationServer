Imports Grapevine.Interfaces.Server

Public Class Server
    Private server As Grapevine.Server.RestServer
    Public Event NotificationAvailable(ByVal Notification As Notification)
    Public Property IP_Whitelist As List(Of Net.IPEndPoint)
    Public Sub New()
        Dim opt As New Grapevine.Server.ServerSettings With {
            .Port = 30000,
            .UseHttps = False,
            .Host = FindLocalWiredIPV4()
        }

        IP_Whitelist = New List(Of Net.IPEndPoint)


        Dim NotificationRoute As New Grapevine.Server.Route(Routes.Actions.DisplayNotification,
                                    Grapevine.Shared.HttpMethod.POST, Routes.Paths.Notification)

        server = New Grapevine.Server.RestServer(opt)
        AddHandler server.Router.BeforeRouting, AddressOf BeforeRouting
        AddHandler server.Router.AfterRouting, AddressOf AfterRouting
        server.Router.Register(NotificationRoute)
        server.Start()
    End Sub

    Private Function BeforeRouting(ByVal Context As IHttpContext) As IHttpContext
        Dim Found As Net.IPEndPoint = IP_Whitelist.Find(Function(x)
                                                            Return Context.Request.RemoteEndPoint.Address.Address.ToString() = x.Address.Address.ToString()
                                                        End Function)
        If Found Is Nothing Then
            Context.Response.StatusCode = Grapevine.Shared.HttpStatusCode.Forbidden
            Context.Response.SendResponse(System.Text.ASCIIEncoding.ASCII.GetBytes(String.Empty))
        End If
        Return Context
    End Function

    Private Function AfterRouting(ByVal Context As IHttpContext) As IHttpContext
        If Context.WasRespondedTo = False Then Context.Response.SendResponse(System.Text.ASCIIEncoding.ASCII.GetBytes(String.Empty))
        Return Context
    End Function

    Private Function FindLocalWiredIPV4() As String
        Dim ni As Net.NetworkInformation.NetworkInterface() = Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
        For Each intf As Net.NetworkInformation.NetworkInterface In ni
            Dim Status As Net.NetworkInformation.OperationalStatus = intf.OperationalStatus
            Dim Type As Net.NetworkInformation.NetworkInterfaceType = intf.NetworkInterfaceType

            If Status = Net.NetworkInformation.OperationalStatus.Up And Type = Net.NetworkInformation.NetworkInterfaceType.Ethernet Then
                Dim Addresses As Net.NetworkInformation.UnicastIPAddressInformationCollection = intf.GetIPProperties().UnicastAddresses
                For Each IP As Net.NetworkInformation.UnicastIPAddressInformation In Addresses
                    Dim Family As Net.Sockets.AddressFamily = IP.Address.AddressFamily
                    If Family = Net.Sockets.AddressFamily.InterNetwork Then
                        Return IP.Address.ToString()
                    End If
                Next
            End If
        Next

        Return Net.IPAddress.Loopback.Address.ToString()
    End Function

End Class