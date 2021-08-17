﻿
Imports System.Data.Entity
Imports System.IO
Imports System.Net
Imports System.Web.Http
Imports Newtonsoft.Json
Imports System.Data.Entity.Migrations
Imports log4net

Namespace Controllers
    '<RoutePrefix("api/liste")>

    <RoutePrefix("api")>
    Public Class AuthController
        Inherits System.Web.Http.ApiController
        Private db As New ApplicationDbContext
        Private ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)


        '<Route("YourView")>
        'Public Function GetYourView() As JRisposta
        '    Dim r As New JRisposta
        '    Dim t
        '    Try
        '        t = db.Database.SqlQuery(Of AppConfig)("Select * from AppConfigs").ToList

        '    Catch ex As Exception
        '        Dim m As String = ex.Message
        '    End Try
        '    'Dim t = db.Database.SqlQuery("Select * from AppConfig",
        '    'Return (From c In db.YourView Select c Where c.App = "common" Or c.App = "webapi" Order By c.Parameter)
        '    r.add("list", t)
        '    Return r
        'End Function



        <Route("login")>
        Public Function PostLogin(model As LoginBindingModel) As JRisposta
            Dim r As New JRisposta
            If Not ModelState.IsValid Then
                r.stato = JRisposta.Stati.Errato
                r.add("ModelState", ModelState.Values)
                r.messaggio = "Errore nei dati trasmessi"
                Return r
            End If

            Dim usr As Users = (From u In db.Users Where u.UserName = model.UserName And u.password = cripta(model.password) Select u).FirstOrDefault

            If IsNothing(usr) Then
                r.stato = JRisposta.Stati.Errato
                r.messaggio = "Le credenziali fornite non sono corrette."
            Else
                Dim ut As New JRisposta(True)
                ut.add("UserName", usr.UserName)
                ut.add("DisplayName", usr.DisplayName)
                ut.add("companyID", usr.companyID)
                ut.add("email", usr.email)
                ut.add("lastAccess", usr.lastAccess)
                ut.add("PasswordMustChange", usr.PasswordMustChange)
                ut.add("TwoFactorEnabled", usr.TwoFactorEnabled)

                usr.lastAccess = Now
                usr.AccessFailedCount = 0
                db.Users.Attach(usr)
                db.Entry(usr).State = EntityState.Modified
                db.SaveChanges()
                r.add("userInfo", ut)
            End If

            Return r

        End Function


    End Class
End Namespace