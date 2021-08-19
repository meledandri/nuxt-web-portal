﻿
Imports System.Data.Entity
Imports System.IO
Imports System.Net
Imports System.Web.Http
Imports Newtonsoft.Json
Imports System.Data.Entity.Migrations
Imports log4net

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
                                              Select New UserInfo With {.UserName = ut.UserName, .DisplayName = ut.DisplayName, .email = ut.email, .userID = ut.userID, .password = ""}).ToList
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


        <Route("fabdata")>
        Public Function PostFabData(model As LoginBindingModel) As JRisposta
            Dim r As New JRisposta
            If Not ModelState.IsValid Then
                r.stato = JRisposta.Stati.Errato
                r.add("ModelState", ModelState.Values)
                r.messaggio = "Errore nei dati trasmessi"
                Return r
            End If
            Dim pwd = cripta(model.password)
            Dim usr As Users = (From u In db.Users
                                Join c In db.Companies On u.companyID Equals c.companyID
                                Where u.UserName = model.UserName And u.password = pwd And u.Disabled = False And u.isHidden = False And c.isHidden = False
                                Select u).FirstOrDefault

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
                Dim cmp As Companies = (From c In db.Companies Where c.companyID = usr.companyID And c.isHidden = False Select c).FirstOrDefault
                If Not IsNothing(cmp) Then
                    ut.add("BusinessName", cmp.BusinessName)
                    ut.add("area", IIf(cmp.companyID = 1, "on", "fab"))
                End If

                usr.lastAccess = Now
                usr.AccessFailedCount = 0
                db.Users.Attach(usr)
                db.Entry(usr).State = EntityState.Modified
                db.SaveChanges()

                db.Database.ExecuteSqlCommand("DELETE FROM UsersTokens where userID = '" & usr.userID & "'")
                Dim tk As New UsersTokens
                tk.userID = usr.userID
                Dim dl = New DateTime(Now.Year, Now.Month, Now.Day).AddDays(1)
                tk.deadline = dl

                db.UsersTokens.Add(tk)
                db.SaveChanges()

                db.UsersTokens.Add(tk)

                ut.add("Token", tk.token)

                Dim mn As List(Of AppMenu) = (From m In db.AppMenu Where m.destination = "on" And m.flagVisible = True Select m Order By m.order, m.Name).ToList
                r.add("appMenu", mn)

                r.add("userInfo", ut)
            End If

            Return r

        End Function


    End Class
End Namespace