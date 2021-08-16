Imports System.Web.Http
'Imports System.Web.Optimization

Public Class WebApiApplication
    Inherits System.Web.HttpApplication

    Protected Sub Application_Start()
        AreaRegistration.RegisterAllAreas()
        GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        'BundleConfig.RegisterBundles(BundleTable.Bundles)
    End Sub

    Private Sub WebApiApplication_BeginRequest(sender As Object, e As EventArgs) Handles Me.BeginRequest
        'HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*")
        'HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "*")
        'HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "*")
        'HttpContext.Current.Request.Headers.Add("Access-Control-Allow-Origin", "*")
        'HttpContext.Current.Request.Headers.Add("Access-Control-Allow-Methods", "*")
        'HttpContext.Current.Request.Headers.Add("Access-Control-Allow-Headers", "*")
        Dim m As String = HttpContext.Current.Request.HttpMethod
        Dim a = m
    End Sub

    Private Sub WebApiApplication_EndRequest(sender As Object, e As EventArgs) Handles Me.EndRequest
        Try
            If Not HttpContext.Current.Response.Headers.AllKeys.Contains("Access-Control-Allow-Origin") Then
                If Not IsNothing(HttpContext.Current.Request.Headers("origin")) Then
                    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", HttpContext.Current.Request.Headers("origin"))
                Else
                    'HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*")
                End If
            End If
            If Not HttpContext.Current.Response.Headers.AllKeys.Contains("Access-Control-Allow-Methods") Then
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "*")
            End If
            If Not HttpContext.Current.Response.Headers.AllKeys.Contains("Access-Control-Allow-Headers") Then
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "*")
            End If

        Catch ex As Exception

        End Try

    End Sub
End Class
