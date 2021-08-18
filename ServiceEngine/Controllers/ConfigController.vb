
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
    Public Class ConfigController
        Inherits System.Web.Http.ApiController
        Private db As New ApplicationDbContext
        Private ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

        <Route("appConfig")>
        Public Function GetConfig() As List(Of AppConfig)
            log.Info("[GET]" & vbTab & "api/appConfig")
            init()
            Return (From c In db.AppConfig Select c Where c.App = "common" Or c.App = "webapi" Order By c.Parameter).ToList

        End Function


        <Route("YourView")>
        Public Function GetYourView() As JRisposta
            Dim r As New JRisposta
            Dim t
            Try
                t = db.Database.SqlQuery(Of AppConfig)("Select * from AppConfigs").ToList

            Catch ex As Exception
                Dim m As String = ex.Message
            End Try
            'Dim t = db.Database.SqlQuery("Select * from AppConfig",
            'Return (From c In db.YourView Select c Where c.App = "common" Or c.App = "webapi" Order By c.Parameter)
            r.add("list", t)
            Return r
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


            db.AppConfig.AddOrUpdate(model)
            db.SaveChanges()

            Dim list As List(Of AppConfig) = (From c In db.AppConfig Select c Where c.App = "common" Or c.App = "webapi" Order By c.Parameter).ToList
            r.add("list", list)
            Return r

        End Function

        <Route("activeCompanies")>
        Public Function GetActiveCompanies() As List(Of Companies)
            'Dim li As Integer = (From ci In db.Companies).Count
            'If li = 0 Then
            '    Dim cn As New Companies
            '    cn.BusinessName = "Gruppo SASI"
            '    cn.isHidden = False
            '    cn.insertDate = Now
            '    db.Companies.Add(cn)
            '    db.SaveChanges()

            '    Dim ui As Integer = (From u In db.Users Select u).Count
            '    If ui = 0 Then
            '        Dim un As New Users
            '        With un
            '            .DisplayName = "Administrator"
            '            .email = "sergio.meledandri@pro360web.com"
            '            .insertDate = Now
            '            .password = cripta("Password123!")
            '            .PasswordMustChange = False
            '            .LockoutEnabled = False
            '            .UserName = "admin"
            '            .companyID = cn.companyID
            '        End With
            '        db.Users.Add(un)
            '        db.SaveChanges()
            '    End If

            'End If

            Dim l As List(Of Companies) = (From c In db.Companies Select c Where c.isHidden = False Order By c.BusinessName).ToList
            Return l

        End Function


        Private Sub init()
            Dim li As Integer = (From ci In db.Companies).Count
            If li = 0 Then
                log.Info("[INIT]" & vbTab & "ConfigController\Init")
                Dim cn As New Companies
                cn.BusinessName = "Gruppo SASI"
                cn.isHidden = False
                cn.insertDate = Now
                db.Companies.Add(cn)
                db.SaveChanges()

                Dim ui As Integer = (From u In db.Users Select u).Count
                If ui = 0 Then
                    Dim un As New Users
                    With un
                        .DisplayName = "Administrator"
                        .email = "sergio.meledandri@pro360web.com"
                        .insertDate = Now
                        .password = cripta("Password123!")
                        .PasswordMustChange = False
                        .LockoutEnabled = False
                        .UserName = "admin"
                        .companyID = cn.companyID
                    End With
                    db.Users.Add(un)
                    db.SaveChanges()
                End If


                insMenu()



            End If

        End Sub

        Private Sub insMenu()



            '################### VOCE MENU On
            Dim m0 As New AppMenu
            With m0
                .Name = "Login"
                .Link = "/"
                .parentID = 0
                .icon = "fas fa-sign-in-alt"
                .Permissions = "on_admin"
                .flagVisible = True
                .order = 0
                .destination = "on"
            End With
            db.AppMenu.Add(m0)


            '################### VOCE MENU On
            Dim m1 As New AppMenu
            With m1
                .Name = "Gest Fabbricante"
                .Link = "/on/fab_edit"
                .parentID = 0
                .icon = "fa-users"
                .Permissions = "on_admin"
                .flagVisible = True
                .order = 1
                .destination = "on"
            End With
            db.AppMenu.Add(m1)



            '################### VOCE MENU On
            Dim m2 As New AppMenu
            With m2
                .Name = "Caricamenti"
                .Link = "/on/upload_manager"
                .parentID = 0
                .icon = "fa-upload"
                .Permissions = "on_admin"
                .flagVisible = True
                .order = 2
                .destination = "on"
            End With
            db.AppMenu.Add(m2)











            db.SaveChanges()
        End Sub

    End Class
End Namespace