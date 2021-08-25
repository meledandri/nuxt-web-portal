
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
            Dim r As New JRisposta
            Dim list As List(Of TaskInfoDataBindig) = (From p In db.Products
                                                       Join cp In db.Companies On p.companyID Equals cp.companyID
                                                       Join ed In db.Editions On ed.productID Equals p.productID
                                                       Join act In db.mdActivity On ed.mdActivityID Equals act.mdActivityID
                                                       Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                                                       Join tsk In db.mdTasks On ed.editionID Equals tsk.editionID
                                                       Join tsks In db.mdTasksStates On tsk.mdTasksStatesID Equals tsks.mdTasksStatesID
                                                       Join str In db.Structures On ed.StructureID Equals str.structureID
                                                       Join u In db.Users On u.userID Equals tsk.ownerID
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
                                                           .StructureID = str.structureID,
                                                           .structureName = str.structureName,
                                                       .mdTaskID = tsk.mdTaskID,
                                                       .mdTaskStatesID = tsk.mdTasksStatesID,
                                                       .mdTaskStatesName = tsks.mdTasksStatesName,
                                                           .insertDate = tsk.insertDate,
                                                       .modDate = tsk.ModDate,
                                                       .ownerID = tsk.ownerID,
                                                       .UserName = u.UserName,
                                                       .DisplayName = u.DisplayName,
                                                       .email = u.email
                                                           }).ToList




            r.add("list", list)
            Return r
        End Function




    End Class
End Namespace