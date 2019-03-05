Imports Grapevine.Interfaces.Server

Public Class Main
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


        Dim NotificationRoute As New Grapevine.Server.Route(Function(x)
                                                                Dim serializer As New Web.Script.Serialization.JavaScriptSerializer
                                                                Dim JSON As Object = Nothing
                                                                Dim Body As String = String.Empty
                                                                Dim Title As String = String.Empty
                                                                Dim Severity As Notification.SeverityEnum = Notification.SeverityEnum.None
                                                                Dim StatusCode As Grapevine.Shared.HttpStatusCode = Grapevine.Shared.HttpStatusCode.Ok
                                                                Try
                                                                    JSON = serializer.DeserializeObject(x.Request.Payload)
                                                                    Title = JSON("title")
                                                                    Body = JSON("body")
                                                                    Severity = Integer.Parse(JSON("severity"))
                                                                Catch ex As Exception
                                                                End Try

                                                                If JSON IsNot Nothing AndAlso Body <> String.Empty Then
                                                                    Dim Sender As String = x.Request.RemoteEndPoint.Address.ToString()

                                                                    Dim n As New Notification With {
                                                                        .Title = Title,
                                                                        .Body = Body,
                                                                        .Severity = Severity,
                                                                        .Sender = Sender
                                                                    }

                                                                    RaiseEvent NotificationAvailable(n)
                                                                Else
                                                                    StatusCode = Grapevine.Shared.HttpStatusCode.BadRequest
                                                                End If
                                                                x.Response.StatusCode = StatusCode
                                                                Return x
                                                            End Function,
                                                            Grapevine.Shared.HttpMethod.POST,
                                                            "/API/NOTIFICATION")


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