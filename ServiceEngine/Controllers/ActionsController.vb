Imports System.Data.Entity
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Web
Imports System.Web.Hosting
Imports System.Web.Http
Imports log4net
Imports Microsoft.Owin.Host.SystemWeb


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
            'If Not ModelState.IsValid Then
            '    r.stato = JRisposta.Stati.Errato
            '    r.add("ModelState", getModelStateMessages(ModelState))
            '    r.messaggio = "Errore nei dati trasmessi"
            '    Return r
            'End If

            Dim editionID As Integer
            Dim userID As String = ""
            Dim ed As Editions = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault
            Dim startActiviyDate As Date = Now


            Dim streamProvider = New CustomMultipartFileStreamProvider()
            Await Request.Content.ReadAsMultipartAsync(streamProvider)
            Dim fileStream = Await streamProvider.Contents(0).ReadAsStreamAsync()
            Dim customData = streamProvider.CustomData


            'If Not Request.Content.IsMimeMultipartContent() Then
            '    Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
            'End If

            'Dim provider = New MultipartFormDataStreamProvider(Path.Combine(My.Application.Info.DirectoryPath, "www\upload"))
            'Dim files = Await Request.Content.ReadAsMultipartAsync(provider)

            Dim al As New ActivityLog



            If Not Request.Content.IsMimeMultipartContent() Then
                Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
            End If


            '    Stream reqStream = Request.Content.ReadAsStreamAsync().Result;
            'MemoryStream tempStream = New MemoryStream();
            'reqStream.CopyTo(tempStream);



            'tempStream.Seek(0, SeekOrigin.End);
            'StreamWriter writer = New StreamWriter(tempStream);
            'writer.WriteLine();
            'writer.Flush();
            'tempStream.Position = 0;


            ' StreamContent StreamContent = New StreamContent(tempStream);
            ' foreach(var header In Request.Content.Headers)
            ' {
            '     StreamContent.Headers.Add(header.Key, header.Value);
            ' }

            '// Read the form data And return an async task.
            ' Await StreamContent.ReadAsMultipartAsync(provider);
            Dim root As String = Path.Combine(My.Application.Info.DirectoryPath, "www\upload")
            Dim provider = New MultipartFormDataStreamProvider(root)


            Dim reqStream As Stream = Request.Content.ReadAsStreamAsync().Result
            ' reqStream.Position = 0
            Dim tempStream As MemoryStream = New MemoryStream()
            reqStream.CopyTo(tempStream)
            tempStream.Position = 0
            tempStream.Seek(0, SeekOrigin.End)
            Dim writer As StreamWriter = New StreamWriter(tempStream)
            writer.WriteLine()
            writer.Flush()
            tempStream.Position = 0
            Dim StreamContent As StreamContent = New StreamContent(tempStream)
            For Each header In Request.Content.Headers
                StreamContent.Headers.Add(header.Key, header.Value)
            Next
            Await StreamContent.ReadAsMultipartAsync(provider)






            Try
                Await Request.Content.ReadAsMultipartAsync(provider)

                For Each file As MultipartFileData In provider.FileData
                    'Trace.WriteLine(file.Headers.ContentDisposition.FileName)
                    'Trace.WriteLine("Server file path: " & file.LocalFileName)
                    Dim errors As List(Of String) = checkFileRules(file.LocalFileName)
                    If errors.Count > 0 Then
                        r.stato = JRisposta.Stati.Errato
                        r.messaggio = String.Join(vbCrLf, errors.ToArray)
                        If System.IO.File.Exists(file.LocalFileName) Then
                            System.IO.File.Delete(file.LocalFileName)
                        End If
                    End If
                Next



                'Return Request.CreateResponse(HttpStatusCode.OK)
            Catch e As System.Exception
                r.stato = JRisposta.Stati.Errato
                r.messaggio = "Errore: " & e.Message
                Return r
            End Try



            With al
                .editionID = editionID
                .mdTasksStatesID = ed.mdTasksStatesID
                .resultID = r.stato
                .resultMessage = r.messaggio
                .startActiviyDate = startActiviyDate
                .stopActiviyDate = Now
                .userID = userID
            End With
            db.ActivityLog.Add(al)
            ed.fileStatus = r.stato
            ed.ownerID = userID
            ed.modifiedDate = Now
            db.Editions.Attach(ed)
            db.Entry(ed).State = EntityState.Modified
            db.SaveChanges()

            'Dim httpRequest = HttpContext.Current.Request
            ''Dim cd As New MyCustomData
            ''For Each p In customData
            ''    cd.add(p, httpRequest.Form(p))
            ''Next

            'Dim mdTaskID = 0 'httpRequest.Form("mdTaskID")
            'Dim userID = 0 ' httpRequest.Form("userID")

            'For n = 0 To customData.Count - 1
            '    Try
            '        mdTaskID = customData(n).Item("mdTaskID")

            '    Catch ex As Exception

            '    End Try
            '    Try
            '        userID = customData(n).Item("userID")

            '    Catch ex As Exception

            '    End Try

            'Next






            'Dim uploadDir As String = Path.Combine(My.Application.Info.DirectoryPath, "www\upload")
            'If Not Directory.Exists(uploadDir) Then Directory.CreateDirectory(uploadDir)

            'Dim flag_stato As Integer = 0
            'Dim fileName As String = ""
            'Dim fileName_ext As String = ""
            'Dim filepath As String = ""

            Dim docfiles = New List(Of String)()
            'If httpRequest.Files.Count > 0 Then
            '    For Each file As String In httpRequest.Files
            '        Dim postedFile = httpRequest.Files(file)
            '        Dim fi As New FileInfo(postedFile.FileName)
            '        fileName_ext = fi.Extension.Replace(".", "")

            '        fileName = mdTaskID & fileName_ext

            '        filepath = uploadDir & "\" & fileName
            '        postedFile.SaveAs(filepath)
            '        docfiles.Add(filepath)
            '    Next

            '    'result = Request.CreateResponse(HttpStatusCode.Created, docfiles)
            'Else
            '    'result = Request.CreateResponse(HttpStatusCode.BadRequest)
            'End If


            r.messaggio = String.Format("Sono stati caticati {0} files.", docfiles.Count)
            Return r
        End Function
        Private Function checkFileRules(file As String) As List(Of String)
            Return New List(Of String)
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