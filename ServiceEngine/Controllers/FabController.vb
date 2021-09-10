
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
    Public Class FabController
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


        <Route("fabList")>
        Public Function GetFabList() As JRisposta
            log.Info("[GET]" & vbTab & "api/fabList")

            Dim r As New JRisposta
            Dim hidden As Boolean = False
            Dim list As List(Of FabListDataBinding) = (From c In db.Companies
                                                       Join cd In db.CompanyDetail On c.companyID Equals cd.companyID
                                                       Join cr In db.CompanyRoles On cd.companyRoleID Equals cr.companyRoleID
                                                       Where c.isHidden = hidden
                                                       Select New FabListDataBinding With {.BusinessName = c.BusinessName, .companyID = c.companyID, .country = cd.country, .SRN = cd.SRN, .companyRoleID = cr.companyRoleID, .companyRoleName = cr.companyRoleName}).ToList

            For Each c As FabListDataBinding In list
                Dim d As New FabListDetailDataBinding

                Dim u As List(Of UserInfo) = (From ut In db.Users Where ut.companyID = c.companyID
                                              Select New UserInfo With {.userName = ut.userName, .displayName = ut.displayName, .email = ut.email, .userID = ut.userID, .password = "", .companyID = ut.companyID}).ToList
                d.users = u
                d.number_of_users = u.Count
                'Recupero le attività dell'azienda
                d.tasks = getTasksInfoList(c.companyID)
                d.number_of_tasks = d.tasks.Count
                c.details = d
            Next


            Dim isHidden As Integer = 0
            If Not hidden Then
                isHidden = (From c In db.Companies Where c.isHidden = True).Count
            End If


            Dim companyRoles As List(Of CompanyRoles) = (From cr In db.CompanyRoles Select cr Order By cr.companyRoleName).ToList
            r.add("roles", companyRoles)

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


        <HttpGet>
        <Route("fabEditData/{id}")>
        Public Function GetfabEditData(id As Integer) As JRisposta
            log.Info("[GET]" & vbTab & "api/fabEditData/" & id)

            Dim r As New JRisposta
            Dim cp As FabListDataBinding = (From c In db.Companies
                                            Join cd In db.CompanyDetail On c.companyID Equals cd.companyID
                                            Join cr In db.CompanyRoles On cd.companyRoleID Equals cr.companyRoleID
                                            Where c.companyID = id
                                            Select New FabListDataBinding With {.BusinessName = c.BusinessName, .companyID = c.companyID, .country = cd.country, .SRN = cd.SRN, .companyRoleID = cr.companyRoleID, .companyRoleName = cr.companyRoleName}).FirstOrDefault

            If Not IsNothing(cp) Then
                Dim d As New FabListDetailDataBinding

                Dim u As List(Of UserInfo) = (From ut In db.Users Where ut.companyID = cp.companyID
                                              Select New UserInfo With {.userName = ut.userName, .displayName = ut.displayName, .email = ut.email, .userID = ut.userID, .password = "", .companyID = ut.companyID}).ToList
                d.users = u
                'Recupero le attività dell'azienda
                d.tasks = getTasksInfoList(cp.companyID)

                d.number_of_users = u.Count
                d.number_of_tasks = d.tasks.Count
                cp.details = d

            End If



            Dim prd As List(Of Products) = (From p In db.Products Where p.companyID = id Select p Order By p.productName).ToList
            r.add("products", prd)

            Dim companyRoles As List(Of CompanyRoles) = (From cr In db.CompanyRoles Select cr Order By cr.companyRoleName).ToList
            r.add("roles", companyRoles)

            Dim mdc As List(Of mdClass) = (From c In db.mdClass Select c Order By c.mdClassName).ToList
            r.add("mdClass", mdc)

            Dim mda As List(Of mdActivity) = (From a In db.mdActivity Select a Order By a.mdActivityName).ToList
            r.add("mdActivity", mda)

            Dim mds As List(Of Structures) = (From s In db.Structures Select s Order By s.structureName).ToList
            r.add("Structures", mds)

            r.add("company", cp)
            Return r
        End Function








        ''' <summary>
        ''' Controller per la registrazione dei dati utente (nuovi o presenti) 
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Route("saveUser")>
        Public Function PostSaveUser(model As UserInfo) As JRisposta
            log.Info("[POST]" & vbTab & "api/saveUser")

            Dim ut As New Users
            Dim r As New JRisposta
            If Not ModelState.IsValid Then
                r.stato = JRisposta.Stati.Errato
                r.add("ModelState", getModelStateMessages(ModelState))
                r.messaggio = "Errore nei dati trasmessi"
                Return r
            End If

            If model.userID = "0" Then
                '#### Inserire tutte le verifiche del caso
                With ut
                    .userID = System.Guid.NewGuid.ToString()
                    .userName = model.userName
                    .displayName = model.displayName
                    .email = model.email
                    .password = cripta(model.password)
                    .companyID = model.companyID
                    .PasswordMustChange = True
                    '.insertDate = Now
                End With

                Try
                    db.Users.Add(ut)
                    db.SaveChanges()
                    r.stato = JRisposta.Stati.Corretto
                    r.messaggio = "Utente registrato correttamete"
                Catch ex As Exception
                    r.stato = JRisposta.Stati.Errato
                    r.messaggio = ex.Message
                End Try

            Else
                r.stato = JRisposta.Stati.Errato
                r.messaggio = "Errore nei dati trasmessi"
            End If

            Dim ui As New UserInfo
            With ui
                .companyID = ut.companyID
                .displayName = ut.displayName
                .email = ut.email
                .password = ""
                .userID = ut.userID
                .userName = ut.userName
            End With

            r.add("userInfo", ui)
            Return r
        End Function


        <Route("saveCompany")>
        Public Function PostSaveCompany(model As FabListDataBinding) As JRisposta
            log.Info("[POST]" & vbTab & "api/saveCompany")

            Dim r As New JRisposta
            If Not ModelState.IsValid Then
                r.stato = JRisposta.Stati.Errato
                r.add("ModelState", getModelStateMessages(ModelState))
                r.messaggio = "Errore nei dati trasmessi"
                Return r
            End If
            Dim cr As CompanyRoles = (From nr In db.CompanyRoles Where nr.companyRoleID = model.companyRoleID Or nr.companyRoleName = model.companyRoleName).FirstOrDefault
            If IsNothing(cr) Then cr = New CompanyRoles
            cr.companyRoleName = model.companyRoleName
            db.CompanyRoles.AddOrUpdate(cr)
            db.SaveChanges()
            If model.companyID = "0" Then   'Nuova Azienda
                Dim nc As New Companies
                With nc
                    .BusinessName = model.BusinessName
                End With
                Try
                    db.Companies.Add(nc)
                    db.SaveChanges()
                    model.companyID = nc.companyID




                    Dim ncd As New CompanyDetail
                    With ncd
                        .companyID = model.companyID
                        .companyRoleID = cr.companyRoleID
                        .country = model.country
                        .SRN = model.SRN
                    End With
                    db.CompanyDetail.Add(ncd)
                    db.SaveChanges()

                Catch ex As Exception
                    r.stato = JRisposta.Stati.Errato
                    r.messaggio = ex.Message
                End Try


            Else    ' Modifica Azienda
                Dim c As Companies = (From cp In db.Companies Where cp.companyID = model.companyID Select cp).FirstOrDefault
                If Not IsNothing(c) Then
                    c.BusinessName = model.BusinessName
                    Try
                        db.Companies.AddOrUpdate(c)
                        db.SaveChanges()

                        Dim cd As CompanyDetail = (From d In db.CompanyDetail Where d.companyID = model.companyID Select d).FirstOrDefault
                        If Not IsNothing(cd) Then
                            With cd
                                .companyRoleID = cr.companyRoleID
                                .country = model.country
                                .SRN = model.SRN
                            End With
                            db.Companies.AddOrUpdate(c)
                            db.SaveChanges()

                        End If
                    Catch ex As Exception
                        r.stato = JRisposta.Stati.Errato
                        r.messaggio = ex.Message
                    End Try
                End If
            End If


            r.add("companyInfo", model)
            Return r

        End Function


        <Route("tree/{editionID}")>
        Public Function GetTreeEdition(editionID As Integer) As JRisposta
            log.Info("[GET]" & vbTab & "api/tree/" & editionID)

            Dim r As New JRisposta
            '            Select Case Details.detailID, Structures.structureID, Structures.structureName, Editions.editionID, Editions.editionName, Editions.certificationPlan, Products.productID, Products.productName, Companies.companyID, 
            '                         Companies.BusinessName, mdClass.mdClassID, mdClass.mdClassName, Products.mdCode, Details.Title, Details.idParent, Details.documentID, Details.fileName, Details.addFolder, Details.addFile, Details.nLevels,
            '                         Details.idVerDoc, Details.flagState, Details.fileExtension, Details.operatorID, Details.MD5, Details.swTarget, Details.file_for_checklist, Details.fullPath, Details.flagContainer
            'From Products INNER Join
            '              Details On Products.productID = Details.productID INNER Join
            '              mdClass On Products.mdClassID = mdClass.mdClassID INNER Join
            '              Editions On Details.editionID = Editions.editionID INNER Join
            '              Structures On Details.structureID = Structures.structureID INNER Join
            '              Companies On Products.companyID = Companies.companyID

            'where Details.editionID = 2

            'Dim t As List(Of DetailsTreeModel) = New List(Of DetailsTreeModel)
            'Dim ed As Editions = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault
            'If Not IsNothing(ed) Then
            '    If ed.asZipFile Then
            '        Dim dt As DataTable = getCustomStructureDT(editionID)

            '        Try
            '            t = toEntityFramework(Of DetailsTreeModel)(dt)
            '        Catch ex As Exception
            '            log.Error("init_StructureDetails: " & ex.Message)
            '        End Try
            '    Else
            '        t = (From p In db.Products
            '             Join d In db.Details On p.productID Equals d.productID
            '             Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
            '             Join e In db.Editions On d.editionID Equals e.editionID
            '             Join str In db.Structures On str.structureID Equals e.structureID
            '             Join c In db.Companies On c.companyID Equals p.companyID
            '             Where e.editionID = editionID
            '             Select New DetailsTreeModel With {
            '                            .detailID = d.detailID,
            '                            .structureID = str.structureID,
            '                            .structureName = str.structureName,
            '                            .asZipFile = e.asZipFile,
            '                            .editionID = e.editionID,
            '                            .editionName = e.editionName,
            '                            .certificationPlan = e.certificationPlan,
            '                            .productID = p.productID,
            '                            .productName = p.productName,
            '                            .companyID = c.companyID,
            '                            .BusinessName = c.BusinessName,
            '                            .mdClassID = cls.mdClassID,
            '                            .mdClassName = cls.mdClassName,
            '                            .mdCode = p.mdCode,
            '                            .Title = d.Title,
            '                            .idParent = d.idParent,
            '                            .documentID = d.documentID,
            '                            .fileName = d.fileName,
            '                            .addFile = d.addFile,
            '                            .addFolder = d.addFolder,
            '                            .nLevels = d.nLevels,
            '                            .idVerDoc = d.idVerDoc,
            '                            .flagState = d.flagState,
            '                            .fileExtension = d.fileExtension,
            '                            .operatorID = d.operatorID,
            '                            .MD5 = d.MD5,
            '                            .swTarget = d.swTarget,
            '                            .file_for_checklist = d.file_for_checklist,
            '                            .fullPath = d.fullPath,
            '                            .flagContainer = d.flagContainer,
            '                            .fileStatus = e.fileStatus,
            '                            .checkListStatus = e.checkListStatus,
            '                            .productInfoStatus = e.productInfoStatus
            '                            }).ToList

            '    End If

            'End If

            Dim ed As Editions = (From e In db.Editions Where e.editionID = editionID Select e).FirstOrDefault
            Dim asFileZip As Boolean = False
            If Not IsNothing(ed) Then
                asFileZip = ed.asZipFile
            End If

            r.add("tree", getTreeList(editionID, asFileZip))

            Return r
        End Function

        <Route("task/save")>
        Public Function PostTaskSave(model As TaskInfoDataBindig) As JRisposta
            log.Info("[POST]" & vbTab & "api/task/save")

            Dim r As New JRisposta
            Dim ed As New Editions
            If Not ModelState.IsValid Then
                r.stato = JRisposta.Stati.Errato
                r.add("ModelState", getModelStateMessages(ModelState))
                r.messaggio = "Errore nei dati trasmessi"
                Return r
            End If

            'Classe Prodotto
            If (model.mdClassID = 0) Then
                Dim mdc As New mdClass
                mdc.mdClassName = model.mdClassName
                db.mdClass.Add(mdc)
                db.SaveChanges()
                model.mdClassID = mdc.mdClassID ' Reimposto il modello con la classe aggiornata
            End If


            'Prodotto
            If model.productID = 0 Then
                Dim p As New Products
                p.productName = model.productName
                p.companyID = model.companyID
                p.mdClassID = model.mdClassID
                p.mdCode = model.mdCode
                db.Products.Add(p)
                db.SaveChanges()
                model.productID = p.productID ' Reimposto il modello conil prodotto  aggiornato
            End If

            'Edizione
            If model.editionID <> 0 Then
                ed = (From e In db.Editions Where e.editionID = model.editionID).FirstOrDefault
            End If

            With ed
                .productID = model.productID
                .certificationPlan = model.certificationPlan
                .editionName = model.editionName
                .editionNotes = model.editionNotes
                .mdActivityID = model.mdActivityID
                '.mdTasksStatesID = model.certificationPlan
                .modifiedDate = Now
                .ownerID = model.ownerID
                .structureID = model.structureID
                If IsNothing(.deadline) Then .deadline = Now
                If Not IsNothing(model.deadline) Then .deadline = model.deadline
                .asZipFile = model.asZipFile
            End With

            Try
                db.Editions.AddOrUpdate(ed)
                db.SaveChanges()
                Try
                    db.Database.ExecuteSqlCommand("DELETE FROM Details WHERE editionID = " & ed.editionID)

                Catch ex As Exception
                    Dim m = ex.Message
                End Try


                createTemplateStructureDB(ed.editionID, model.ownerID)

            Catch ex As Exception
                r.messaggio = ex.Message
                r.stato = JRisposta.Stati.Errato

            End Try



            log.Info(String.Format("Upload file Archivio per l'edizione -{0}- conclusa.", ed.editionID))
            Return r
        End Function
    End Class
End Namespace