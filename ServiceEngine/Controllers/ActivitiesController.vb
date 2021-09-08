
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
    Public Class ActivitiesController
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


        <Route("activitiesList")>
        Public Function GetactivitiesList() As JRisposta
            log.Info("[GET]" & vbTab & "api/activitiesList")

            Dim r As New JRisposta
            Dim list As List(Of TaskInfoDataBindig) = (From p In db.Products
                                                       Join cp In db.Companies On p.companyID Equals cp.companyID
                                                       Join ed In db.Editions On ed.productID Equals p.productID
                                                       Join act In db.mdActivity On ed.mdActivityID Equals act.mdActivityID
                                                       Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                                                       Join tsks In db.mdTasksStates On ed.mdTasksStatesID Equals tsks.mdTasksStatesID
                                                       Join str In db.Structures On ed.structureID Equals str.structureID
                                                       Join u In db.Users On u.userID Equals ed.ownerID
                                                       Where cp.isHidden = False
                                                       Order By cp.BusinessName, p.productName
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
                                                           .asZipFile = ed.asZipFile,
                                                       .mdTaskStatesID = ed.mdTasksStatesID,
                                                       .mdTaskStatesName = tsks.mdTasksStatesName,
                                                           .insertDate = ed.insertDate,
                                                       .modifiedDate = ed.modifiedDate,
                                                       .ownerID = ed.ownerID,
                                                       .userName = u.userName,
                                                       .displayName = u.displayName,
                                                       .email = u.email
                                                           }).ToList




            r.add("list", list)
            Return r
        End Function




    End Class
End Namespace