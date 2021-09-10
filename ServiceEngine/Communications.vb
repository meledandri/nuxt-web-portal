Imports Microsoft.AspNet.SignalR
Imports Microsoft.AspNet.SignalR.Hubs
Imports Microsoft.Owin.Cors
Imports Owin
Imports System.Web
Imports System.Web.Http
Imports Microsoft.Owin.FileSystems
Imports Microsoft.Owin.StaticFiles
Imports Microsoft.Owin
Imports Microsoft.Owin.Hosting
Imports Microsoft.Owin.Security.OAuth
Imports Microsoft.Owin.Extensions
Imports System.IO

Namespace SelfHostedServiceSignalR
    Class Startup
        Public Sub Configuration(ByVal app As IAppBuilder)
            app.UseCors(CorsOptions.AllowAll)
            Dim config As HttpConfiguration = New HttpConfiguration()
            config.MapHttpAttributeRoutes
            config.Routes.MapHttpRoute(
                name:="DefaultApi",
                routeTemplate:="api/{controller}/{id}",
                defaults:=New With {
                Key .id = RouteParameter.[Optional]
                })
            app.Properties("host.AppName") = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            app.UseWebApi(config)
            app.MapSignalR()
            Dim www As String = "./www"
            Try
                www = My.Settings.path_www
            Catch ex As Exception
                If runningMode = runningModes.console Then
                    Console.WriteLine("Parametro -www_dir- non trovato")
                End If
            End Try
            Dim physicalFileSystem
            Try
                physicalFileSystem = New PhysicalFileSystem(www)
            Catch ex As Exception
                physicalFileSystem = New PhysicalFileSystem("./www")
            End Try
            Dim options = New FileServerOptions With {
                .EnableDefaultFiles = True,
                .FileSystem = PhysicalFileSystem
            }
            options.StaticFileOptions.FileSystem = physicalFileSystem
            options.StaticFileOptions.ServeUnknownFileTypes = True
            options.DefaultFilesOptions.DefaultFileNames = {"index.html"}
            app.UseFileServer(options)
        End Sub
    End Class
End Namespace

Namespace SelfHostedServiceSignalR
    <HubName("myHub")>
    Public Class myHub
        Inherits Hub

        Public Sub Send(ByVal name As String, ByVal message As String)
            Clients.All.addMessage(name, message)
            If runningMode = runningModes.console Then Console.WriteLine(String.Format("NEW MESSAGE[{0}] : {1}", name, message))
        End Sub

        Public Sub SendPrivate(ByVal name As String, ByVal message As String, clientId As String)
            'Clients..addMessage(name, message)
            'Clients.User(clientId).send(message)
            'Clients.Client(Context.ConnectionId).sendPrivateMessage(clientId, name, message)
            Clients.All.addPrivateMessage(name, message, clientId)
            If runningMode = runningModes.console Then Console.WriteLine(String.Format("NEW PRIVATE MESSAGE[{0} TO {2}] : {1}", name, message, clientId))
        End Sub

    End Class
End Namespace