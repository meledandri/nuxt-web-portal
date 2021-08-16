Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Web.Http
Imports System.Web.Http.Description
Imports WebApi
Imports WebApi.Data

Namespace Controllers
    Public Class AppConfigsController
        Inherits System.Web.Http.ApiController

        Private db As New ApplicationDbContext

        ' GET: api/AppConfigs
        Function GetAppConfigs() As IQueryable(Of AppConfigs)
            Return db.AppConfigs
        End Function

        ' GET: api/AppConfigs/5
        <ResponseType(GetType(AppConfigs))>
        Async Function GetAppConfigs(ByVal id As String) As Task(Of IHttpActionResult)
            Dim appConfigs As AppConfigs = Await db.AppConfigs.FindAsync(id)
            If IsNothing(appConfigs) Then
                Return NotFound()
            End If

            Return Ok(appConfigs)
        End Function

        ' PUT: api/AppConfigs/5
        <ResponseType(GetType(Void))>
        Async Function PutAppConfigs(ByVal id As String, ByVal appConfigs As AppConfigs) As Task(Of IHttpActionResult)
            If Not ModelState.IsValid Then
                Return BadRequest(ModelState)
            End If

            If Not id = appConfigs.App Then
                Return BadRequest()
            End If

            db.Entry(appConfigs).State = EntityState.Modified

            Try
                Await db.SaveChangesAsync()
            Catch ex As DbUpdateConcurrencyException
                If Not (AppConfigsExists(id)) Then
                    Return NotFound()
                Else
                    Throw
                End If
            End Try

            Return StatusCode(HttpStatusCode.NoContent)
        End Function

        ' POST: api/AppConfigs
        <ResponseType(GetType(AppConfigs))>
        Async Function PostAppConfigs(ByVal appConfigs As AppConfigs) As Task(Of IHttpActionResult)
            If Not ModelState.IsValid Then
                Return BadRequest(ModelState)
            End If

            db.AppConfigs.Add(appConfigs)

            Try
                Await db.SaveChangesAsync()
            Catch ex As DbUpdateException
                If (AppConfigsExists(appConfigs.App)) Then
                    Return Conflict()
                Else
                    Throw
                End If
            End Try

            Return CreatedAtRoute("DefaultApi", New With {.id = appConfigs.App}, appConfigs)
        End Function

        ' DELETE: api/AppConfigs/5
        <ResponseType(GetType(AppConfigs))>
        Async Function DeleteAppConfigs(ByVal id As String) As Task(Of IHttpActionResult)
            Dim appConfigs As AppConfigs = Await db.AppConfigs.FindAsync(id)
            If IsNothing(appConfigs) Then
                Return NotFound()
            End If

            db.AppConfigs.Remove(appConfigs)
            Await db.SaveChangesAsync()

            Return Ok(appConfigs)
        End Function

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing) Then
                db.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Function AppConfigsExists(ByVal id As String) As Boolean
            Return db.AppConfigs.Count(Function(e) e.App = id) > 0
        End Function
    End Class
End Namespace