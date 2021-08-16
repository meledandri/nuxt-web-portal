Imports System
Imports System.Configuration
Imports System.Net
Imports System.Threading
Imports System.Xml
Imports NWebDav.Server
Imports NWebDav.Server.Http
Imports NWebDav.Server.HttpListener
Imports NWebDav.Server.Logging
Imports NWebDav.Server.Stores
Imports NWebDav.Sample.HttpListener.LogAdapters

Module Module1

    Sub Main()

        ' Obtain the HTTP binding settings
        Dim webdavProtocol = If(My.Settings.webdav_protocol, "http")
        Dim webdavIp = If(My.Settings.webdav_ip, "127.0.0.1")
        Dim webdavPort = If(My.Settings.webdav_port, "11111")

        Using httpListener = New Net.HttpListener()
            ' Add the prefix
            httpListener.Prefixes.Add($"{webdavProtocol}://{webdavIp}:{webdavPort}/")
            Console.WriteLine($"{webdavProtocol}://{webdavIp}:{webdavPort}/")
            ' Use basic authentication if requested
            Dim webdavUseAuthentication = My.Settings.webdav_authentication

            If webdavUseAuthentication Then
                ' Check if HTTPS is enabled
                If Not Equals(webdavProtocol, "https") Then Console.WriteLine("Most WebDAV clients cannot use authentication on a non-HTTPS connection")

                ' Set the authentication scheme and realm
                httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic
                httpListener.Realm = "WebDAV server"
            Else
                ' Allow anonymous access
                httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous
            End If

            ' Start the HTTP listener
            httpListener.Start()

            ' Start dispatching requests
            Dim cancellationTokenSource = New CancellationTokenSource()
            DispatchHttpRequestsAsync(httpListener, cancellationTokenSource.Token)

            ' Wait until somebody presses return
            Console.WriteLine("WebDAV server running. Press 'x' to quit.")

            While Console.ReadKey().KeyChar <> "x"c
            End While

            cancellationTokenSource.Cancel()
        End Using
    End Sub



    Private Async Sub DispatchHttpRequestsAsync(ByVal httpListener As Net.HttpListener, ByVal cancellationToken As CancellationToken)
        ' Create a request handler factory that uses basic authentication
        Dim requestHandlerFactory = New RequestHandlerFactory()

        ' Create WebDAV dispatcher
        Dim homeFolder = "D:\test" ' Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        Dim webDavDispatcher = New WebDavDispatcher(New DiskStore(homeFolder), requestHandlerFactory)

        ' Determine the WebDAV username/password for authorization
        ' (only when basic authentication is enabled)
        Dim webdavUsername = "test" ' IIf(My.Settings.Properties("webdav_username") Is Nothing, "test", My.Settings("webdav_username"))
        Dim webdavPassword = "test" ' IIf(My.Settings.Properties("webdav_password") Is Nothing, "test", My.Settings("webdav_password"))
        Dim httpListenerContext As HttpListenerContext

        While Not cancellationToken.IsCancellationRequested AndAlso (CSharpImpl.__Assign(httpListenerContext, Await httpListener.GetContextAsync().ConfigureAwait(False))) IsNot Nothing
            ' Determine the proper HTTP context
            Dim httpContext As IHttpContext

            If httpListenerContext.Request.IsAuthenticated Then
                httpContext = New HttpBasicContext(httpListenerContext, checkIdentity:=Function(i) Equals(i.Name, webdavUsername) AndAlso Equals(i.Password, webdavPassword))
            Else
                httpContext = New HttpContext(httpListenerContext)
            End If

            ' Dispatch the request
            Await webDavDispatcher.DispatchRequestAsync(httpContext).ConfigureAwait(False)
        End While
    End Sub

    Private Class CSharpImpl
        <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
        Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Module
