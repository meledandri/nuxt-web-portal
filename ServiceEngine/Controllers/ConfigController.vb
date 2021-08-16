
Imports System.Data.Entity
Imports System.IO
Imports System.Net
Imports System.Web.Http
Imports Newtonsoft.Json


Namespace Controllers
    '<RoutePrefix("api/liste")>

    <RoutePrefix("api")>
    Public Class ConfigController
        Inherits System.Web.Http.ApiController
        Private db As New ApplicationDbContext

        <Route("config")>
        Public Function GetConfig() As List(Of AppConfig)


            Return (From c In db.AppConfig Select c Where c.App = "common" Or c.App = "webapi" Order By c.Parameter).ToList

        End Function


        <Route("addConfig")>
        Public Function PostAddConfig(model As AppConfig) As JRisposta
            Dim r As New JRisposta
            If Not ModelState.IsValid Then
                r.stato = JRisposta.Stati.Errato
                r.add("ModelState", ModelState.Values)
                r.messaggio = "Errore nei dati trasmessi"
                Return r
            End If

            Dim count As Integer = (From c In db.AppConfig Select c Where c.App = model.App And c.Parameter = model.Parameter).Count
            If count = 0 Then
                Try
                    db.AppConfig.Add(model)
                    db.SaveChanges()
                Catch ex As Exception
                    r.stato = r.Stati.Attenzione
                    r.messaggio = "Inserimento non riuscito: " & ex.Message
                End Try
            Else
                Try
                    db.AppConfig.Attach(model)
                    db.Entry(model).State = EntityState.Modified
                    db.SaveChanges()
                Catch ex As Exception
                    r.stato = r.Stati.Attenzione
                    r.messaggio = "Inserimento non riuscito: " & ex.Message
                End Try

            End If
            Dim list As List(Of AppConfig) = (From c In db.AppConfig Select c Where c.App = "common" Or c.App = "webapi" Order By c.Parameter).ToList
            r.add("list", list)
            Return r

        End Function

    End Class
End Namespace