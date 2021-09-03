Imports System.Data.Entity
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Web
Imports System.Web.Hosting
Imports System.Web.Http
Imports Ionic.Zip
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


            'Dim streamProvider = New CustomMultipartFileStreamProvider()
            'Await Request.Content.ReadAsMultipartAsync(streamProvider)
            'Dim fileStream = Await streamProvider.Contents(0).ReadAsStreamAsync()
            'Dim customData = streamProvider.CustomData


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
            Dim temp_dir As String = Path.Combine(My.Application.Info.DirectoryPath, "www\temp")
            Dim StoragePath As String = Path.Combine(My.Application.Info.DirectoryPath, "www\files")
            If Not Directory.Exists(temp_dir) Then Directory.CreateDirectory(temp_dir)
            If Not Directory.Exists(StoragePath) Then Directory.CreateDirectory(StoragePath)
            Dim provider = New MultipartFormDataStreamProvider(temp_dir)
            Dim fullname As String = ""

            Dim mpfd As MultipartFileData

            Try
                Await Request.Content.ReadAsMultipartAsync(provider)

                For Each fileData As MultipartFileData In provider.FileData
                    mpfd = fileData
                    For Each k As String In provider.FormData.AllKeys
                        Select Case k
                            Case "editionID"
                                editionID = provider.FormData.GetValues(k)(0)
                                ed = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault
                            Case "userID"
                                userID = provider.FormData.GetValues(k)(0)
                        End Select
                    Next


                    Dim errors As List(Of String) = checkFileRules(fileData.LocalFileName)
                    If errors.Count > 0 Then
                        r.stato = JRisposta.Stati.Errato
                        r.messaggio = String.Join(vbCrLf, errors.ToArray)
                        If File.Exists(fileData.LocalFileName) Then
                            File.Delete(fileData.LocalFileName)
                        End If
                        GoTo Fine
                    Else


                        If String.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName) Then
                            r.stato = JRisposta.Stati.Errato
                            r.messaggio = "La richiesta non è formattata propriamente."
                            GoTo Fine
                        End If

                        Dim fileName = fileData.Headers.ContentDisposition.FileName

                        If fileName.StartsWith("""") AndAlso fileName.EndsWith("""") Then
                            fileName = fileName.Trim(""""c)
                        End If

                        If fileName.Contains("/") OrElse fileName.Contains("\") Then
                            fileName = Path.GetFileName(fileName)
                        End If

                        fullname = Path.Combine(StoragePath, fileName)

                        If File.Exists(fullname) Then
                            File.Delete(fullname)
                        End If

                        File.Move(fileData.LocalFileName, fullname)



                    End If
                Next



                'Return Request.CreateResponse(HttpStatusCode.OK)
            Catch e As System.Exception
                r.stato = JRisposta.Stati.Errato
                r.messaggio = "Errore: " & e.Message
                GoTo Fine
            End Try


            If Not IsNothing(ed) Then
                With al
                    .editionID = editionID
                    .mdTasksStatesID = mdTaskStates_enum.uploading_process
                    .resultID = r.stato
                    .resultMessage = IIf(r.messaggio = "", mpfd.LocalFileName & " (OK)", r.messaggio)
                    .startActiviyDate = startActiviyDate
                    .stopActiviyDate = Now
                    .userID = userID
                End With
                db.ActivityLog.Add(al)
                Try
                    db.SaveChanges()

                Catch ex As Exception
                    r.stato = JRisposta.Stati.Errato
                    r.messaggio = ex.Message
                    GoTo Fine
                End Try
                ed.fileStatus = r.stato
                ed.ownerID = userID
                ed.modifiedDate = Now
                db.Editions.Attach(ed)
                db.Entry(ed).State = EntityState.Modified
                db.SaveChanges()

                Try
                    Dim storeEdition As String = Path.Combine(StoragePath, editionID)
                    Using zip As New ZipFile(fullname)
                        If Directory.Exists(storeEdition) Then Directory.Delete(storeEdition, True)
                        zip.ExtractAll(storeEdition)
                        'Dim e As ZipEntry
                        'For Each e In zip
                        '    If (e.UsesEncryption) Then
                        '        e.ExtractWithPassword("Secret!")
                        '    Else
                        '        e.Extract()
                        '    End If
                        'Next
                    End Using
                    File.Delete(fullname)

                    createCustomStructureDB(storeEdition, ed.productID, ed.editionID, 0, 1000)

                Catch ex As Exception
                    r.stato = JRisposta.Stati.Errato
                    r.messaggio = ex.Message
                End Try

            End If

Fine:

            Return r
        End Function
        Private Function checkFileRules(file As String) As List(Of String)
            Return New List(Of String)
        End Function
    End Class


    'Class MyCustomData
    '    Inherits Dictionary(Of String, Object)
    '    Sub New()
    '    End Sub

    '    Public Overloads Sub add(ByVal nome As String, ByVal valore As Object)
    '        If MyBase.ContainsKey(nome) Then
    '            MyBase.Remove(nome)
    '            MyBase.Add(nome, valore)
    '        Else
    '            MyBase.Add(nome, valore)
    '        End If
    '    End Sub
    'End Class
End Namespace