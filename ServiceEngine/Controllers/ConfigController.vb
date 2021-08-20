
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

                Dim ac As New AppConfig
                With ac
                    .App = "common"
                    .Parameter = "primary-color"
                    .Value = "#3333FF"
                End With
                db.AppConfig.Add(ac)
                db.SaveChanges()






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

                Dim mdc As New mdClass
                mdc.mdClassName = "MD Classe 1"
                db.mdClass.Add(mdc)
                db.SaveChanges()

                Dim mda As New mdActivity
                Try
                    mda.mdActivityName = "Registrazione Prodotto"
                    db.mdActivity.Add(mda)
                    db.SaveChanges()

                Catch ex As Exception
                    log.Error("Errore nell'inserimento di un record in mdActivity", ex)
                End Try



                Dim cp As New Companies
                cp.BusinessName = "MD Pharma Demo"
                cp.isHidden = False
                cp.insertDate = Now
                db.Companies.Add(cp)
                db.SaveChanges()


                Dim cr As New CompanyRoles
                cr.companyRoleName = "Manufacturer"
                db.CompanyRoles.Add(cr)
                db.SaveChanges()

                Dim cd As New CompanyDetail
                With cd
                    .companyID = cp.companyID
                    .companyRoleID = cr.companyRoleID
                    .country = "Italy"
                    .SRN = "IT-MF-000001234"
                End With
                db.CompanyDetail.Add(cd)
                db.SaveChanges()

                Dim uc As New Users
                With uc
                    .DisplayName = "MD User 1"
                    .email = "user1.md-pharma.srl"
                    .insertDate = Now
                    .password = cripta("Password123!")
                    .PasswordMustChange = True
                    .LockoutEnabled = True
                    .UserName = "user1"
                    .companyID = cp.companyID
                End With
                db.Users.Add(uc)
                db.SaveChanges()


                'Dim uc2 As New Users
                'With uc2
                '    .DisplayName = "MD User 2"
                '    .email = "user2.md-pharma.srl"
                '    .insertDate = Now
                '    .password = cripta("Password123!")
                '    .PasswordMustChange = True
                '    .LockoutEnabled = True
                '    .UserName = "user2"
                '    .companyID = cp.companyID
                'End With
                'db.Users.Add(uc2)
                'db.SaveChanges()


                'Prodotto
                Dim p As New Products
                With p
                    .productName = "Prodotto demo Cerotto"
                    .mdClassID = mdc.mdClassID
                    .companyID = cp.companyID
                End With
                db.Products.Add(p)
                db.SaveChanges()

                Dim s As New Structures
                With s
                    .structureName = "File ZIP"
                    .isMaster = False
                End With
                db.Structures.Add(s)
                db.SaveChanges()

                Dim ed As New Editions
                With ed
                    .editionName = "Prima Edizione"
                    .productID = p.productID
                    .certificationPlan = "IT-MD-000001234"
                    .mdActivityID = mda.mdActivityID
                    .editionNotes = "Note automatiche prima edizione."
                    .deadline = Now.AddDays(20)
                    .StructureID = s.structureID
                End With
                db.Editions.Add(ed)
                db.SaveChanges()

                Dim ts As New mdTasksStates
                ts.mdTasksStatesName = "Creato"
                db.mdTasksStates.Add(ts)
                db.SaveChanges()

                Dim t As New mdTasks
                With t
                    .editionID = ed.editionID
                    .mdTasksStatesID = ts.mdTasksStatesID
                    .ownerID = uc.userID
                End With
                db.mdTasks.Add(t)
                db.SaveChanges()


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



            '################### VOCE MENU FAB
            Dim m10 As New AppMenu
            With m10
                .Name = "Login"
                .Link = "/"
                .parentID = 0
                .icon = "fas fa-sign-in-alt"
                .Permissions = "on_fab"
                .flagVisible = True
                .order = 0
                .destination = "fab"
            End With
            db.AppMenu.Add(m10)




            db.SaveChanges()
        End Sub

    End Class
End Namespace