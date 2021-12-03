
Imports System.Data.Entity
Imports System.IO
Imports System.Net
Imports System.Web.Http
Imports Newtonsoft.Json
Imports System.Data.Entity.Migrations
Imports log4net
Imports System.Web.Http.ModelBinding

Namespace Controllers
    <RoutePrefix("api")>
    Public Class ProductsController
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


        <Route("productsList")>
        Public Function GetProductsList() As JRisposta
            log.Info("[GET]" & vbTab & "api/productsList")

            Dim r As New JRisposta
            Dim hidden As Boolean = False
            Dim list As List(Of ProductInfoDataBinding) = (From p In db.Products
                                                           Join ed In db.Editions On p.productID Equals ed.productID
                                                           Join str In db.Structures On ed.structureID Equals str.structureID
                                                           Join cp In db.Companies On p.companyID Equals cp.companyID
                                                           Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                                                           Join act In db.mdActivity On ed.mdActivityID Equals act.mdActivityID
                                                           Where cp.isHidden = hidden
                                                           Select New ProductInfoDataBinding _
                                                               With {.companyID = cp.companyID,
                                                               .BusinessName = cp.BusinessName,
                                                               .productID = p.productID,
                                                               .productName = p.productName,
                                                               .mdClassID = cls.mdClassID,
                                                               .mdClassName = cls.mdClassName,
                                                               .editionID = ed.editionID,
                                                               .editionName = ed.editionName,
                                                               .certificationPlan = ed.certificationPlan,
                                                               .mdActivityID = act.mdActivityID,
                                                               .mdActivityName = act.mdActivityName,
                                                               .editionNotes = ed.editionNotes,
                                                                                                                      .deadline = ed.deadline,
                                                               .structureID = str.structureID,
                                                               .structureName = str.structureName,
                                                               .insertDate = ed.insertDate
                                                               }).ToList




            Dim isHidden As Integer = 0
            If Not hidden Then
                isHidden = (From c In db.Companies Where c.isHidden = True).Count
            End If


            Dim mdc As List(Of mdClass) = (From c In db.mdClass Select c Order By c.mdClassName).ToList
            r.add("mdClass", mdc)

            Dim mda As List(Of mdActivity) = (From a In db.mdActivity Select a Order By a.mdActivityName).ToList
            r.add("mdActivity", mda)

            Dim mds As List(Of Structures) = (From s In db.Structures Select s Order By s.structureName).ToList
            r.add("Structures", mds)

            r.add("list", list)
            r.add("nHidden", isHidden)
            Return r
        End Function


        <Route("tasksList")>
        Public Function GetTasksList() As JRisposta
            log.Info("[GET]" & vbTab & "api/tasksList")

            Dim r As New JRisposta
            Dim hidden As Boolean = False
            Dim list As List(Of TaskInfoDataBindig) = (From p In db.Products
                                                       Join cp In db.Companies On p.companyID Equals cp.companyID
                                                       Join ed In db.Editions On ed.productID Equals p.productID
                                                       Join act In db.mdActivity On ed.mdActivityID Equals act.mdActivityID
                                                       Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                                                       Join tsks In db.mdTasksStates On ed.mdTasksStatesID Equals tsks.mdTasksStatesID
                                                       Join str In db.Structures On ed.structureID Equals str.structureID
                                                       Join u In db.Users On u.userID Equals ed.ownerID
                                                       Where cp.isHidden = hidden
                                                       Select New TaskInfoDataBindig _
                                                               With {.companyID = cp.companyID,
                                                               .BusinessName = cp.BusinessName,
                                                               .productID = p.productID,
                                                               .productName = p.productName,
                                                               .mdClassID = cls.mdClassID,
                                                               .mdClassName = cls.mdClassName,
                                                               .editionID = ed.editionID,
                                                               .editionName = ed.editionName,
                                                               .certificationPlan = ed.certificationPlan,
                                                               .mdActivityID = act.mdActivityID,
                                                               .mdActivityName = act.mdActivityName,
                                                               .editionNotes = ed.editionNotes,
                                                                .deadline = ed.deadline,
                                                               .structureID = str.structureID,
                                                               .structureName = str.structureName,
                                                           .mdTaskStatesID = ed.mdTasksStatesID,
                                                           .mdTaskStatesName = tsks.mdTasksStatesName,
                                                               .insertDate = ed.insertDate,
                                                           .modifiedDate = ed.modifiedDate,
                                                           .ownerID = ed.ownerID,
                                                           .userName = u.userName,
                                                           .displayName = u.displayName,
                                                           .email = u.email
                                                               }).ToList




            Dim isHidden As Integer = 0
            If Not hidden Then
                isHidden = (From c In db.Companies Where c.isHidden = True).Count
            End If


            Dim mdc As List(Of mdClass) = (From c In db.mdClass Select c Order By c.mdClassName).ToList
            r.add("mdClass", mdc)

            Dim mda As List(Of mdActivity) = (From a In db.mdActivity Select a Order By a.mdActivityName).ToList
            r.add("mdActivity", mda)

            Dim mds As List(Of Structures) = (From s In db.Structures Select s Order By s.structureName).ToList
            r.add("Structures", mds)

            r.add("list", list)
            r.add("nHidden", isHidden)
            Return r
        End Function


        <Route("detail/delete")>
        Public Async Function PostDeleteDetail(model As DetailsDataBindingModels_newAttach) As Task(Of IHttpActionResult)
            If Not ModelState.IsValid Then
                Return BadRequest(ModelState)
            End If

            Dim d As Details = (From dt In db.Details Where dt.detailID = model.id Select dt).FirstOrDefault

            Dim editionID As Integer = d.editionID

            Dim temp_dir As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_temp)
            Dim StoragePath As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_storage)
            Dim tempEdition As String = ""
            Dim storeEdition As String = ""
            tempEdition = Path.Combine(temp_dir, editionID)
            storeEdition = Path.Combine(StoragePath, editionID)
            Dim full_path As String = ""
            Dim fullname As String = ""
            Dim fi As FileInfo


            'Se il file in FullPath esiste utilizzo quello altrimenti lo recupero dal percorso relativo

            If File.Exists(d.fullPath) Then
                fullname = d.fullPath
                fi = New FileInfo(fullname)
                Try
                    fi.Delete()
                Catch ex As Exception
                    log.Error(ex.Message, ex)
                End Try
            End If


            If Not IsNothing(d) Then
                db.Details.Remove(d)
                Await db.SaveChangesAsync()
                Return Ok()
            Else
                Return NotFound()
            End If


        End Function





        <Route("detail/rename")>
        Public Async Function PostRenameDetail(model As DetailsDataBindingModels_newAttach) As Task(Of IHttpActionResult)
            If Not ModelState.IsValid Then
                Return BadRequest(ModelState)
            End If

            Dim d As Details = (From dt In db.Details Where dt.detailID = model.id Select dt).FirstOrDefault

            Dim editionID As Integer = d.editionID

            Dim temp_dir As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_temp)
            Dim StoragePath As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_storage)
            Dim tempEdition As String = ""
            Dim storeEdition As String = ""
            tempEdition = Path.Combine(temp_dir, editionID)
            storeEdition = Path.Combine(StoragePath, editionID)
            Dim full_path As String = ""
            Dim fullname As String = ""
            Dim fi As FileInfo


            'Se il file in FullPath esiste utilizzo quello altrimenti lo recupero dal percorso relativo

            If File.Exists(d.fullPath) Then

                fi = New FileInfo(d.fullPath)

                fullname = Path.Combine(fi.Directory.FullName, model.title + fi.Extension)

                If File.Exists(fullname) Then
                    Return BadRequest("Il nome del file è già presente.")
                End If

                Try
                    File.Move(fi.FullName, fullname)
                Catch ex As Exception
                    log.Error(ex.Message, ex)
                    Return BadRequest(ex.ToString)
                End Try
            End If

            Dim relPath As String = Left(d.relPath, d.relPath.LastIndexOf("/") + 1) & model.title & fi.Extension

            If Not IsNothing(d) Then
                With d
                    .Title = model.title
                    .fileName = model.title & fi.Extension
                    .fullPath = fullname
                    .relPath = relPath
                End With

                db.Details.AddOrUpdate(d)
                Await db.SaveChangesAsync()
                Return Ok()
            Else
                Return NotFound()
            End If


        End Function




        '''' <summary>
        '''' Controller per la registrazione dei dati utente (nuovi o presenti) 
        '''' </summary>
        '''' <param name="model"></param>
        '''' <returns></returns>
        '<Route("saveUser")>
        'Public Function PostSaveUser(model As UserInfo) As JRisposta
        '    Dim ut As New Users
        '    Dim r As New JRisposta
        '    If Not ModelState.IsValid Then
        '        r.stato = JRisposta.Stati.Errato
        '        r.add("ModelState", getModelStateMessages(ModelState))
        '        r.messaggio = "Errore nei dati trasmessi"
        '        Return r
        '    End If

        '    If model.userID = "0" Then
        '        '#### Inserire tutte le verifiche del caso
        '        With ut
        '            .userID = System.Guid.NewGuid.ToString()
        '            .userName = model.userName
        '            .displayName = model.displayName
        '            .email = model.email
        '            .password = cripta(model.password)
        '            .companyID = model.companyID
        '            .PasswordMustChange = True
        '            '.insertDate = Now
        '        End With

        '        Try
        '            db.Users.Add(ut)
        '            db.SaveChanges()
        '            r.stato = JRisposta.Stati.Corretto
        '            r.messaggio = "Utente registrato correttamete"
        '        Catch ex As Exception
        '            r.stato = JRisposta.Stati.Errato
        '            r.messaggio = ex.Message
        '        End Try

        '    Else
        '        r.stato = JRisposta.Stati.Errato
        '        r.messaggio = "Errore nei dati trasmessi"
        '    End If

        '    Dim ui As New UserInfo
        '    With ui
        '        .companyID = ut.companyID
        '        .displayName = ut.displayName
        '        .email = ut.email
        '        .password = ""
        '        .userID = ut.userID
        '        .userName = ut.userName
        '    End With

        '    r.add("userInfo", ui)
        '    Return r
        'End Function


        '<Route("saveCompany")>
        'Public Function PostSaveCompany(model As FabListDataBinding) As JRisposta
        '    Dim r As New JRisposta
        '    If Not ModelState.IsValid Then
        '        r.stato = JRisposta.Stati.Errato
        '        r.add("ModelState", getModelStateMessages(ModelState))
        '        r.messaggio = "Errore nei dati trasmessi"
        '        Return r
        '    End If
        '    Dim cr As CompanyRoles = (From nr In db.CompanyRoles Where nr.companyRoleID = model.companyRoleID Or nr.companyRoleName = model.companyRoleName).FirstOrDefault
        '    If IsNothing(cr) Then cr = New CompanyRoles
        '    cr.companyRoleName = model.companyRoleName
        '    db.CompanyRoles.AddOrUpdate(cr)
        '    db.SaveChanges()
        '    If model.companyID = "0" Then   'Nuova Azienda
        '        Dim nc As New Companies
        '        With nc
        '            .BusinessName = model.BusinessName
        '        End With
        '        Try
        '            db.Companies.Add(nc)
        '            db.SaveChanges()
        '            model.companyID = nc.companyID




        '            Dim ncd As New CompanyDetail
        '            With ncd
        '                .companyID = model.companyID
        '                .companyRoleID = cr.companyRoleID
        '                .country = model.country
        '                .SRN = model.SRN
        '            End With
        '            db.CompanyDetail.Add(ncd)
        '            db.SaveChanges()

        '        Catch ex As Exception
        '            r.stato = JRisposta.Stati.Errato
        '            r.messaggio = ex.Message
        '        End Try


        '    Else    ' Modifica Azienda
        '        Dim c As Companies = (From cp In db.Companies Where cp.companyID = model.companyID Select cp).FirstOrDefault
        '        If Not IsNothing(c) Then
        '            c.BusinessName = model.BusinessName
        '            Try
        '                db.Companies.AddOrUpdate(c)
        '                db.SaveChanges()

        '                Dim cd As CompanyDetail = (From d In db.CompanyDetail Where d.companyID = model.companyID Select d).FirstOrDefault
        '                If Not IsNothing(cd) Then
        '                    With cd
        '                        .companyRoleID = cr.companyRoleID
        '                        .country = model.country
        '                        .SRN = model.SRN
        '                    End With
        '                    db.Companies.AddOrUpdate(c)
        '                    db.SaveChanges()

        '                End If
        '            Catch ex As Exception
        '                r.stato = JRisposta.Stati.Errato
        '                r.messaggio = ex.Message
        '            End Try
        '        End If
        '    End If


        '    r.add("companyInfo", model)
        '    Return r

        'End Function


    End Class
End Namespace