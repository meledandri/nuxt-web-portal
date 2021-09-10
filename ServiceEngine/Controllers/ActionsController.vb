﻿Imports System.Data.Entity
Imports System.Data.Entity.Migrations
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Web
Imports System.Web.Hosting
Imports System.Web.Http
Imports log4net
Imports Microsoft.Owin.Host.SystemWeb
Imports SevenZipExtractor

Namespace Controllers
    <RoutePrefix("api/Actions")>
    Public Class ActionsController
        Inherits System.Web.Http.ApiController
        Private db As New ApplicationDbContext
        Private ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Private timeout_sec As Integer = 60


        <Route("upload/archive")>
        Public Async Function PostUpload() As Task(Of JRisposta)
            log.Info("[POST]" & vbTab & "api/Actions/upload/archive")

            Dim r As New JRisposta

            Dim editionID As Integer
            Dim userID As String = ""
            Dim ed As New Editions
            Dim startActiviyDate As Date = Now

            Dim al As New ActivityLog


            If Not Request.Content.IsMimeMultipartContent() Then
                Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
            End If


            Dim temp_dir As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_temp)
            Dim StoragePath As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_storage)
            If Not Directory.Exists(temp_dir) Then Directory.CreateDirectory(temp_dir)
            If Not Directory.Exists(StoragePath) Then Directory.CreateDirectory(StoragePath)
            Dim provider = New MultipartFormDataStreamProvider(temp_dir)
            Dim fullname As String = ""

            Dim tempEdition As String = ""
            Dim storeEdition As String = ""



            Try
                Await Request.Content.ReadAsMultipartAsync(provider)

                For Each fileData As MultipartFileData In provider.FileData

                    If provider.FormData.AllKeys.Contains("editionID") And provider.FormData.AllKeys.Contains("userID") Then   'Verifico che siano presenti i parametri obbligatori
                        If Not IsNumeric(editionID) OrElse Not Integer.TryParse(provider.FormData.GetValues("editionID")(0), editionID) Then
                            r.stato = JRisposta.Stati.Errato
                            r.messaggio = "Identificazione Edizione non corretta."
                        End If
                        userID = provider.FormData.GetValues("userID")(0)
                        If userID.Trim = "" Then
                            r.stato = JRisposta.Stati.Errato
                            r.messaggio = "Identificazione Utente non corretta."
                        End If

                        If r.stato = JRisposta.Stati.Errato Then
                            If File.Exists(fileData.LocalFileName) Then File.Delete(fileData.LocalFileName)
                            GoTo Fine
                        End If
                    Else
                        r.stato = JRisposta.Stati.Errato
                        r.messaggio = "La richiesta non è formattata propriamente."
                        GoTo Fine
                    End If

                    'For Each k As String In provider.FormData.AllKeys
                    '    Select Case k
                    '        Case "editionID"
                    '            editionID = provider.FormData.GetValues(k)(0)
                    '            ed = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault
                    '        Case "userID"
                    '            userID = provider.FormData.GetValues(k)(0)
                    '    End Select
                    'Next
                    ed = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault


                    If Not IsNothing(ed) Then

                        If String.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName) Then
                            r.stato = JRisposta.Stati.Errato
                            r.messaggio = "La richiesta non è formattata propriamente."
                            GoTo Fine
                        End If

                        'Dim fileName = fileData.Headers.ContentDisposition.FileName

                        'If fileName.StartsWith("""") AndAlso fileName.EndsWith("""") Then
                        '    fileName = fileName.Trim(""""c)
                        'End If

                        'If fileName.Contains("/") OrElse fileName.Contains("\") Then
                        '    fileName = Path.GetFileName(fileName)
                        'End If

                        tempEdition = Path.Combine(temp_dir, editionID)
                        If Not Directory.Exists(tempEdition) Then Directory.CreateDirectory(tempEdition)
                        fullname = Path.Combine(temp_dir, editionID & ".archive")

                        If File.Exists(fullname) Then File.Delete(fullname)
                        File.Move(fileData.LocalFileName, fullname)
                    End If
                Next



                'Return Request.CreateResponse(HttpStatusCode.OK)
            Catch e As System.Exception
                r.stato = JRisposta.Stati.Errato
                r.messaggio = "Errore: " & e.Message
                GoTo Fine
            End Try


            If Not IsNothing(ed) And r.stato <> JRisposta.Stati.Errato Then

                Try

                    UnpackArchive(fullname, tempEdition, True, True)


                Catch ex As Exception
                    r.stato = JRisposta.Stati.Errato
                    r.messaggio = ex.Message
                    log.Error(ex.Message, ex)
                End Try
                'File.Delete(fullname)

                Dim errors As List(Of String) = checkPathRules(tempEdition)
                If errors.Count > 0 Then
                    r.stato = JRisposta.Stati.Errato
                    r.messaggio = String.Join(vbCrLf, errors.ToArray)
                    If File.Exists(fullname) Then File.Delete(fullname)
                    If Directory.Exists(tempEdition) Then Directory.Delete(tempEdition)
                    GoTo Fine
                End If

                ed.mdTasksStatesID = mdTaskStates_enum.uploading_process
                ed.fileStatus = r.stato
                ed.ownerID = userID
                ed.modifiedDate = Now
                db.Editions.Attach(ed)
                db.Entry(ed).State = EntityState.Modified
                db.SaveChanges()

                log.Info(String.Format("Edizione {0} aggiornata con stato caricamento file -{1}-", editionID, r.stato))

                If Not createCustomStructureDT(tempEdition, ed.editionID, 0, 1000) Then
                    r.stato = JRisposta.Stati.Errato
                    r.messaggio = "Errore nella generazione della struttura dell'archivio"
                End If


            End If

Fine:
            AppLog(editionID, mdTaskStates_enum.uploading_process, r.stato, "[ActionsController\UPLOAD] " & r.messaggio, userID, startActiviyDate)
            r.add("task", getTasksInfo(editionID))
            Return r
        End Function





        <Route("upload/file/{detailID}")>
        Public Async Function PostUpload(detailID As Integer) As Task(Of JRisposta)
            log.Info("[POST]" & vbTab & "api/Actions/upload/file")

            Dim r As New JRisposta

            Dim d As Details = (From dt In db.Details Where dt.detailID = detailID).FirstOrDefault
            If IsNothing(d) Then    ' Verifico se ho trovato il record
                r.stato = JRisposta.Stati.Errato
                r.messaggio = "Record non presente."
                GoTo Fine
            End If

            If Not d.flagContainer = 1 Then ' Verifico che stia caricando un file.
                r.stato = JRisposta.Stati.Errato
                r.messaggio = "Non è posibile caricare un file su una destinazione differente."
                GoTo Fine
            End If


            Dim editionID As Integer
            Dim userID As String = ""
            Dim ed As New Editions
            Dim startActiviyDate As Date = Now

            Dim al As New ActivityLog


            If Not Request.Content.IsMimeMultipartContent() Then
                Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
            End If


            Dim temp_dir As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_temp)
            Dim StoragePath As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_storage)
            If Not Directory.Exists(temp_dir) Then Directory.CreateDirectory(temp_dir)
            If Not Directory.Exists(StoragePath) Then Directory.CreateDirectory(StoragePath)
            Dim provider = New MultipartFormDataStreamProvider(temp_dir)
            Dim fullname As String = ""

            Dim tempEdition As String = ""
            Dim storeEdition As String = ""



            Try
                Await Request.Content.ReadAsMultipartAsync(provider)

                For Each fileData As MultipartFileData In provider.FileData

                    If provider.FormData.AllKeys.Contains("editionID") And provider.FormData.AllKeys.Contains("userID") Then   'Verifico che siano presenti i parametri obbligatori
                        If Not IsNumeric(editionID) OrElse Not Integer.TryParse(provider.FormData.GetValues("editionID")(0), editionID) Then
                            r.stato = JRisposta.Stati.Errato
                            r.messaggio = "Identificazione Edizione non corretta."
                        End If
                        userID = provider.FormData.GetValues("userID")(0)
                        If userID.Trim = "" Then
                            r.stato = JRisposta.Stati.Errato
                            r.messaggio = "Identificazione Utente non corretta."
                        End If

                        If r.stato = JRisposta.Stati.Errato Then
                            If File.Exists(fileData.LocalFileName) Then File.Delete(fileData.LocalFileName)
                            GoTo Fine
                        End If
                    Else
                        r.stato = JRisposta.Stati.Errato
                        r.messaggio = "La richiesta non è formattata propriamente."
                        GoTo Fine
                    End If

                    ed = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault

                    If Not IsNothing(ed) Then
                        Dim fileName = fileData.Headers.ContentDisposition.FileName

                        If fileName.StartsWith("""") AndAlso fileName.EndsWith("""") Then
                            fileName = fileName.Trim(""""c)
                        End If

                        If fileName.Contains("/") OrElse fileName.Contains("\") Then
                            fileName = Path.GetFileName(fileName)
                        End If

                        Dim fi As New FileInfo(fileName)
                        tempEdition = Path.Combine(temp_dir, editionID)
                        If Not Directory.Exists(tempEdition) Then Directory.CreateDirectory(tempEdition)
                        storeEdition = Path.Combine(StoragePath, editionID)
                        If Not Directory.Exists(storeEdition) Then Directory.CreateDirectory(storeEdition)

                        fullname = Path.Combine(tempEdition, detailID & fi.Extension)

                        If File.Exists(fullname) Then File.Delete(fullname)
                        File.Move(fileData.LocalFileName, fullname)

                        If checkFileRules(fullname).Count = 0 Then  'Verifico se il file è idoneo (es. dimenzioni, formato, ecc)
                            Dim fn As String = Path.Combine(storeEdition, d.fullPath.Replace("/", "\"))
                            fi = New FileInfo(fn)
                            If Not Directory.Exists(fi.Directory.FullName) Then Directory.CreateDirectory(fi.Directory.FullName)
                            If File.Exists(fn) Then File.Delete(fn)
                            File.Move(fullname, fn)

                            d.flagState = 2
                            d.fileExtension = fi.Extension.Replace(".", "")
                            d.operatorID = userID

                            db.Details.AddOrUpdate(d)
                            db.SaveChanges()

                        Else
                            r.stato = JRisposta.Stati.Errato
                            r.messaggio = "File non idoneo"
                            GoTo Fine
                        End If



                    End If
                Next



                'Return Request.CreateResponse(HttpStatusCode.OK)
            Catch e As System.Exception
                r.stato = JRisposta.Stati.Errato
                r.messaggio = "Errore: " & e.Message
                GoTo Fine
            End Try


            If Not IsNothing(ed) And r.stato <> JRisposta.Stati.Errato Then



                'Dim errors As List(Of String) = checkPathRules(tempEdition)
                'If errors.Count > 0 Then
                '    r.stato = JRisposta.Stati.Errato
                '    r.messaggio = String.Join(vbCrLf, errors.ToArray)
                '    'If File.Exists(fullname) Then File.Delete(fullname)
                '    'If Directory.Exists(tempEdition) Then Directory.Delete(tempEdition)
                '    GoTo Fine
                'End If

                ed.mdTasksStatesID = mdTaskStates_enum.uploading_process
                ed.fileStatus = r.stato
                ed.ownerID = userID
                ed.modifiedDate = Now
                db.Editions.Attach(ed)
                db.Entry(ed).State = EntityState.Modified
                db.SaveChanges()

                log.Info(String.Format("Edizione {0} aggiornata con stato caricamento file -{1}-", editionID, r.stato))

            End If

Fine:
            AppLog(editionID, mdTaskStates_enum.uploading_process, r.stato, "[ActionsController\UPLOAD] " & r.messaggio, userID, startActiviyDate)
            r.add("detail", getDetailData(d.detailID))
            Return r
        End Function
















        Private Function checkFileRules(full_path As String) As List(Of String)
            Dim errors As New List(Of String)
            log.Info(String.Format("Verifica del file '{0}' in cornso..", full_path))

            If errors.Count > 0 Then
                log.Error(String.Format("Il file '{0}' presenta -{1}- errori.", full_path, errors.Count))
            Else
                log.Info(String.Format("Il file '{0}' non presenta errori.", full_path))
            End If
            Return errors
        End Function


        Private Function checkPathRules(full_path As String) As List(Of String)
            Dim errors As New List(Of String)
            log.Info(String.Format("Verifica del percorso '{0}' in cornso..", full_path))

            If errors.Count > 0 Then
                log.Error(String.Format("Il percorso '{0}' presenta -{1}- errori.", full_path, errors.Count))
            Else
                log.Info(String.Format("Il percorso '{0}' non presenta errori.", full_path))
            End If
            Return errors
        End Function



        <Route("task/clearArchive/{editionID}")>
        Public Function GetClearArchive(editionID As Integer) As JRisposta
            log.Info("[GET]" & vbTab & "api/Actions/clearArchive/" & editionID)
            Dim startActivity As Date = Now
            Dim r As New JRisposta
            Dim ed As Editions = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault
            If Not IsNothing(ed) Then
                If ed.asZipFile Then    ' Si tratta di un file Zip
                    If ed.fileStatus <> 0 Then
                        If deleteTaskFiles(editionID) Then
                            With ed
                                .fileStatus = 0
                                .modifiedDate = Now
                            End With
                            db.Editions.Attach(ed)
                            db.Entry(ed).State = EntityState.Modified
                            db.SaveChanges()

                            r.messaggio = "OK"
                        Else
                            r.stato = JRisposta.Stati.Errato
                            r.messaggio = "Cancellazione non effettuata"
                        End If
                    Else
                        r.stato = JRisposta.Stati.Errato
                        r.messaggio = "Archivio non presente."
                    End If

                Else    ' Si tratta di un TechFile light
                    If ed.fileStatus <> 2 Then
                        If deleteTaskFiles(editionID) Then
                            With ed
                                .fileStatus = 0
                                .modifiedDate = Now
                            End With
                            db.Editions.Attach(ed)
                            db.Entry(ed).State = EntityState.Modified
                            db.SaveChanges()

                            r.messaggio = "OK"
                        Else
                            r.stato = JRisposta.Stati.Errato
                            r.messaggio = "Cancellazione non effettuata"
                        End If

                    Else
                        r.stato = JRisposta.Stati.Corretto
                        r.messaggio = "Nessun file da cancellare"

                    End If
                End If
            Else
                r.stato = JRisposta.Stati.Errato
                r.messaggio = "Edizione selezionata non presente."
            End If

            r.add("task", getTasksInfo(editionID))
            AppLog(editionID, mdTaskStates_enum.evaluation_process, r.stato, "[ClearArchive] " & r.messaggio, ed.ownerID, startActivity)

            Return r
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