
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
            log.Info("[POST]" & vbTab & "api/addConfig")

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
            log.Info("[GET]" & vbTab & "api/activeCompanies")

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
            '            .displayName = "Administrator"
            '            .email = "sergio.meledandri@pro360web.com"
            '            .insertDate = Now
            '            .password = cripta("Password123!")
            '            .PasswordMustChange = False
            '            .LockoutEnabled = False
            '            .userName = "admin"
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

            'Dim fl As List(Of FileSystemInfo) = FileSystemInfoList(My.Application.Info.DirectoryPath)
            'For Each fsi As FileSystemInfo In fl
            '    Dim entryType As String = "File"

            '    If (fsi.Attributes And FileAttributes.Directory) = FileAttributes.Directory Then
            '        entryType = "Folder"
            '        log.Error("[" & entryType & "]" & fsi.FullName)
            '    Else
            '        log.Info("[" & entryType & "]" & fsi.FullName)

            '    End If
            'Next

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

                Dim un As New Users
                Dim ui As Integer = (From u In db.Users Select u).Count
                If ui = 0 Then
                    With un
                        .displayName = "Administrator"
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

                'Dim mdc As New mdClass
                'mdc.mdClassName = "MD Classe 1"
                'db.mdClass.Add(mdc)
                'db.SaveChanges()

                'Dim mda As New mdActivity
                'Try
                '    mda.mdActivityName = "Registrazione Prodotto"
                '    db.mdActivity.Add(mda)
                '    db.SaveChanges()

                'Catch ex As Exception
                '    log.Error("Errore nell'inserimento di un record in mdActivity", ex)
                'End Try



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
                    .displayName = "MD User 1"
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
                '    .displayName = "MD User 2"
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
                    .mdClassID = mdClass_enum.Class_I
                    .companyID = cp.companyID
                    .mdCode = "MD0123"
                End With
                db.Products.Add(p)
                db.SaveChanges()

                'Dim ts As New mdTasksStates
                'ts.mdTasksStatesName = "Creato"
                'db.mdTasksStates.Add(ts)
                'db.SaveChanges()


                Dim ed As New Editions
                With ed
                    .editionName = "Prima Edizione"
                    .productID = p.productID
                    .certificationPlan = "IT-MD-000001234"
                    .mdActivityID = mdActivity_enum.product_registration
                    .editionNotes = "Note automatiche prima edizione."
                    .deadline = Now.AddDays(20)
                    .structureID = 1
                    .mdTasksStatesID = mdTaskStates_enum.created
                    .ownerID = un.userID
                End With
                db.Editions.Add(ed)
                db.SaveChanges()

                Dim ed2 As New Editions
                With ed2
                    .editionName = "Altra Edizione"
                    .productID = p.productID
                    .certificationPlan = "IT-MD-00000999"
                    .mdActivityID = mdActivity_enum.product_registration
                    .editionNotes = "Note automatiche prima edizione."
                    .deadline = Now.AddDays(20)
                    .structureID = 1
                    .asZipFile = False
                    .mdTasksStatesID = mdTaskStates_enum.created
                    .ownerID = un.userID
                End With
                db.Editions.Add(ed2)
                db.SaveChanges()







                'Dim t As New mdTasks
                'With t
                '    .editionID = ed.editionID
                '    .mdTasksStatesID = ts.mdTasksStatesID
                '    .ownerID = uc.userID
                'End With
                'db.mdTasks.Add(t)
                'db.SaveChanges()

                'Dim ts2 As New mdTasksStates
                'ts2.mdTasksStatesName = "Caricato"
                'db.mdTasksStates.Add(ts2)
                'db.SaveChanges()

                insMenu()



            End If
            init_mdTasksStates()
            init_mdActivity()
            init_mdClass()
            init_StructureDetails()
            Dim ncd As Integer = (From nd In db.Details).Count
            If ncd = 0 Then
                Dim progr As Integer = 1000
                'createCustomStructureDB("D:\TechFile\Dossier\Garza\v_1_0", 1, 1, 0, progr)
                Try
                    createTemplateStructureDB(1, "6ce5d4d1-8f9a-407b-9f56-f68b9c8cc8b8")
                    createTemplateStructureDB(2, "6ce5d4d1-8f9a-407b-9f56-f68b9c8cc8b8")

                Catch ex As Exception
                    log.Error(ex.Message)
                End Try
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

            '################### VOCE MENU FAB
            Dim m11 As New AppMenu
            With m11
                .Name = "TechFile Light"
                .Link = "."
                .parentID = 0
                .icon = "fas fa-sitemap"
                .Permissions = "on_fab"
                .flagVisible = True
                .order = 0
                .destination = "fab"
            End With
            db.AppMenu.Add(m11)




            db.SaveChanges()
        End Sub

        Private Sub init_mdTasksStates()
            Dim dt As New DataTable
            dt = EnumToDataTable(GetType(mdTaskStates_enum), "mdTasksStatesID", "mdTasksStatesName")
            For Each row As DataRow In dt.Rows
                Dim ts As New mdTasksStates
                ts.mdTasksStatesID = row("mdTasksStatesID")
                ts.mdTasksStatesName = row("mdTasksStatesName")
                Try
                    db.mdTasksStates.AddOrUpdate(ts)
                    db.SaveChanges()

                Catch ex As Exception
                    log.Error("init_mdTasksStates : " & ex.Message)
                End Try
            Next
        End Sub


        Private Sub init_mdActivity()
            Dim dt As New DataTable
            dt = EnumToDataTable(GetType(mdActivity_enum), "mdActivityID", "mdActivityName")
            For Each row As DataRow In dt.Rows
                Dim ts As New mdActivity
                ts.mdActivityID = row("mdActivityID")
                ts.mdActivityName = row("mdActivityName")
                Try
                    db.mdActivity.AddOrUpdate(ts)
                    db.SaveChanges()

                Catch ex As Exception
                    log.Error("init_mdActivity : " & ex.Message)
                End Try
            Next
        End Sub

        Private Sub init_mdClass()
            Dim dt As New DataTable
            dt = EnumToDataTable(GetType(mdClass_enum), "mdClassID", "mdClassName")
            For Each row As DataRow In dt.Rows
                Dim ts As New mdClass
                ts.mdClassID = row("mdClassID")
                ts.mdClassName = row("mdClassName")
                Try
                    db.mdClass.AddOrUpdate(ts)
                    db.SaveChanges()

                Catch ex As Exception
                    log.Error("init_mdClass : " & ex.Message)
                End Try
            Next
        End Sub

        Private Sub init_StructureDetails()
            Dim store_path As String = Path.Combine(My.Application.Info.DirectoryPath, "init")
            Dim full_name As String = Path.Combine(My.Application.Info.DirectoryPath, "init\Structures.xml")
            Dim c As Integer = (From n In db.StructureDetails Select n).Count
            If c = 0 Then
                If File.Exists(full_name) Then
                    Dim ds As New DataSet
                    Try
                        ds.ReadXml(full_name)

                    Catch ex As Exception
                        log.Error("init_StructureDetails : " & ex.Message)
                    End Try

                    Dim s As List(Of Structures) = New List(Of Structures)
                    Try
                        s = toEntityFramework(Of Structures)(ds.Tables("Structures"))
                    Catch ex As Exception
                        log.Error("init_StructureDetails: " & ex.Message)
                    End Try

                    For Each str As Structures In s
                        Try
                            db.Structures.AddOrUpdate(str)
                            db.SaveChanges()
                        Catch ex As Exception
                            log.Error("init_StructureDetails: " & ex.Message)
                        End Try
                    Next



                    Dim sd As List(Of StructureDetails) = New List(Of StructureDetails)
                    Try
                        sd = toEntityFramework(Of StructureDetails)(ds.Tables("StructureDetails"))

                    Catch ex As Exception
                        log.Error("init_StructureDetails: " & ex.Message)
                    End Try

                    For Each strd As StructureDetails In sd
                        Try
                            db.StructureDetails.AddOrUpdate(strd)
                            db.SaveChanges()
                        Catch ex As Exception
                            log.Error("init_StructureDetails: " & ex.Message)
                        End Try
                    Next



                End If
            Else
                Dim listStructures As List(Of Structures) = (From s In db.Structures Select s).ToList()
                Dim dtStructures As DataTable = ToDataTable(Of Structures)(listStructures)
                dtStructures.TableName = "Structures"

                Dim listStructureDetails As List(Of StructureDetails) = (From sd In db.StructureDetails Select sd).ToList()
                Dim dtStructureDetails As DataTable = ToDataTable(Of StructureDetails)(listStructureDetails)
                dtStructureDetails.TableName = "StructureDetails"

                Dim ds As New DataSet("Strusctures")
                ds.Tables.Add(dtStructures)
                ds.Tables.Add(dtStructureDetails)

                If Not Directory.Exists(store_path) Then Directory.CreateDirectory(store_path)
                If File.Exists(full_name) Then File.Delete(full_name)
                ds.WriteXml(full_name, XmlWriteMode.WriteSchema)

            End If

        End Sub

    End Class
End Namespace