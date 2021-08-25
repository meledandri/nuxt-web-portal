
Imports System.Data.Entity
Imports System.IO
Imports System.Net
Imports System.Web.Http
Imports Newtonsoft.Json
Imports System.Data.Entity.Migrations
Imports log4net
Imports System.Web.Http.ModelBinding
Imports System.Net.Http
Imports System.Web



Namespace Controllers
    <RoutePrefix("api/Actions")>
    Public Class ActionsController
        Inherits System.Web.Http.ApiController
        Private db As New ApplicationDbContext
        Private ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Private timeout_sec As Integer = 60


        <Route("upload")>
        Public Async Function PostUpload() As Task(Of JRisposta)
            Dim r As New JRisposta
            If Not ModelState.IsValid Then
                r.stato = JRisposta.Stati.Errato
                r.add("ModelState", getModelStateMessages(ModelState))
                r.messaggio = "Errore nei dati trasmessi"
                Return r
            End If

            Dim streamProvider = New CustomMultipartFileStreamProvider()
            Await Request.Content.ReadAsMultipartAsync(streamProvider)
            Dim fileStream = Await streamProvider.Contents(0).ReadAsStreamAsync()
            Dim customData = streamProvider.CustomData

            Dim httpRequest = HttpContext.Current.Request
            'Dim cd As New MyCustomData
            'For Each p In customData
            '    cd.add(p, httpRequest.Form(p))
            'Next

            Dim mdTaskID = 0 'httpRequest.Form("mdTaskID")
            Dim userID = 0 ' httpRequest.Form("userID")

            For n = 0 To customData.Count - 1
                Try
                    mdTaskID = customData(n).Item("mdTaskID")

                Catch ex As Exception

                End Try
                Try
                    userID = customData(n).Item("userID")

                Catch ex As Exception

                End Try

            Next






            Dim uploadDir As String = Path.Combine(My.Application.Info.DirectoryPath, "www\upload")
            If Not Directory.Exists(uploadDir) Then Directory.CreateDirectory(uploadDir)

            Dim flag_stato As Integer = 0
            Dim fileName As String = ""
            Dim fileName_ext As String = ""
            Dim filepath As String = ""

            Dim docfiles = New List(Of String)()
            If httpRequest.Files.Count > 0 Then
                For Each file As String In httpRequest.Files
                    Dim postedFile = httpRequest.Files(file)
                    Dim fi As New FileInfo(postedFile.FileName)
                    fileName_ext = fi.Extension.Replace(".", "")

                    fileName = mdTaskID & fileName_ext

                    filepath = uploadDir & "\" & fileName
                    postedFile.SaveAs(filepath)
                    docfiles.Add(filepath)
                Next

                'result = Request.CreateResponse(HttpStatusCode.Created, docfiles)
            Else
                'result = Request.CreateResponse(HttpStatusCode.BadRequest)
            End If


            r.messaggio = String.Format("Sono stati caticati {0} files.", docfiles.Count)
            Return r
        End Function
    End Class


    Class MyCustomData
        Inherits Dictionary(Of String, Object)
        Sub New()
        End Sub

        Public Overloads Sub add(ByVal nome As String, ByVal valore As Object)
            If MyBase.ContainsKey(nome) Then
                MyBase.Remove(nome)
                MyBase.Add(nome, valore)
            Else
                MyBase.Add(nome, valore)
            End If
        End Sub
    End Class
End Namespace