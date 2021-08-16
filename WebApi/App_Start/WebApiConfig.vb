Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Http
Imports System.Web.Http.Cors
Imports Newtonsoft.Json.Serialization

Public Module WebApiConfig
    Public Sub Register(ByVal config As HttpConfiguration)
        ' Servizi e configurazione dell'API Web

        ' Route dell'API Web
        config.MapHttpAttributeRoutes()

        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )

        '// Enable CORS for the Vue App
        'Dim Cors = New EnableCorsAttribute("http://localhost:8080", "*", "*")
        Dim Cors = New EnableCorsAttribute("*", "*", "*")
        config.EnableCors(Cors)

        '// Set JSON formatter as default one And remove XmlFormatter

        Dim jsonFormatter = config.Formatters.JsonFormatter

        jsonFormatter.SerializerSettings.ContractResolver = New CamelCasePropertyNamesContractResolver()

        '// Remove the XML formatter
        config.Formatters.Remove(config.Formatters.XmlFormatter)

        jsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc



    End Sub
End Module
