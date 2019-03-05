Imports Grapevine.Interfaces.Server

Namespace Routes
    Public Class Actions
        Public Shared Property DisplayNotification As Func(Of IHttpContext,
        IHttpContext) = Function(x)
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

                                NotificationBin.Add(n)
                            Else
                                StatusCode = Grapevine.Shared.HttpStatusCode.BadRequest
                            End If
                            x.Response.StatusCode = StatusCode
                            Return x
                        End Function

    End Class

    Public Class Paths
        Public Const Notification As String = "/API/NOTIFICATION"
    End Class
End Namespace
