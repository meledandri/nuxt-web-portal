
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
                                              Select New UserInfo With {.UserName = ut.UserName, .DisplayName = ut.DisplayName, .email = ut.email, .userID = ut.userID, .password = "", .companyID = ut.companyID}).ToList
                d.users = u
                c.details = d
            Next


            Dim isHidden As Integer = 0
            If Not hidden Then
                isHidden = (From c In db.Companies Where c.isHidden = True).Count
            End If


            Dim companyRoles As List(Of CompanyRoles) = (From cr In db.CompanyRoles Select cr Order By cr.companyRoleName).ToList
            r.add("roles", companyRoles)
            r.add("list", list)
            r.add("nHidden", isHidden)
            Return r
        End Function




        ''' <summary>
        ''' Controller per la registrazione dei dati utente (nuovi o presenti) 
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Route("saveUser")>
        Public Function PostSaveUser(model As UserInfo) As JRisposta
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
                    .UserName = model.UserName
                    .DisplayName = model.DisplayName
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
                .DisplayName = ut.DisplayName
                .email = ut.email
                .password = ""
                .userID = ut.userID
                .UserName = ut.UserName
            End With

            r.add("userInfo", ui)
            Return r
        End Function


        <Route("saveCompany")>
        Public Function PostSaveCompany(model As FabListDataBinding) As JRisposta
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


    End Class
End Namespace