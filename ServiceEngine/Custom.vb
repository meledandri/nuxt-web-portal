Imports System.Data.Entity
Imports System.IO
Imports System.Reflection
Imports System.Web.Http.ModelBinding
Imports log4net
Imports SevenZip

Public Class ClassiJSON
    'Private ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Public Function toJDate(ByVal Data As Date) As String
        Dim DateTimeOrigin As DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0)
        Dim diff As Double = DateDiff(DateInterval.Second, DateTimeOrigin, Data) * 1000
        Return diff.ToString
    End Function


    Public Overloads Function JData(ByVal dt As DataTable) As JRisposta
        Dim risposta As New JRisposta(True)
        risposta.stato = JRisposta.Stati.Neutro
        risposta.messaggio = ""
        Dim Colonne As New List(Of String)
        Dim Dati As New JRisposta(True)

        Dim errore As Boolean = False

        Dim id As Integer = 0
        Try
            Dim records As DataRowCollection = dt.Rows
            For c As Integer = 0 To dt.Columns.Count - 1
                Colonne.Add(dt.Columns(c).ColumnName)
            Next
            For Each record As DataRow In records
                Dim rec As New JRisposta(True)
                For c As Integer = 0 To dt.Columns.Count - 1
                    rec.add(dt.Columns(c).ColumnName, record(c))
                Next
                Dati.add(id, rec)
                id += 1
            Next
            risposta.stato = JRisposta.Stati.Corretto

        Catch ex As Exception
            risposta.stato = JRisposta.Stati.Errato
            risposta.messaggio = ex.Message
        End Try
        risposta.add("columns", Colonne)
        risposta.add("data", Dati)
        risposta.add("data_count", id)
        Return risposta

    End Function


End Class





<Serializable()>
Public Class classeErrore
    Private _Numero As Integer
    Private _Messaggio As String = ""
    Private _Sorgente As String = ""
    Private _Exeption As String = ""

    Public Property Numero() As Integer
        Get
            Return _Numero
        End Get
        Set(ByVal value As Integer)
            _Numero = value
        End Set
    End Property


    Public Property Messaggio() As String
        Get
            Return _Messaggio
        End Get
        Set(ByVal value As String)
            _Messaggio = value
        End Set
    End Property

    Public Property Sorgente() As String
        Get
            Return _Sorgente
        End Get
        Set(ByVal value As String)
            _Sorgente = value
        End Set
    End Property

    Public Property Exeption() As String
        Get
            Return _Exeption
        End Get
        Set(ByVal value As String)
            _Exeption = value
        End Set
    End Property
End Class


<Serializable()>
Public Class classeUtente
    Private _Utente As String
    Private _NomeCompleto As String
    Private _UltimoAccesso As Date = DateAdd("d", -2, Now)
    Private _Abilitato As Boolean = True


    Public Property NomeCompleto() As String
        Get
            Return _NomeCompleto
        End Get
        Set(ByVal value As String)
            _NomeCompleto = value
        End Set
    End Property

    Public Property Utente() As String
        Get
            Return _Utente
        End Get
        Set(ByVal value As String)
            _Utente = value
        End Set
    End Property

    Public Property UltimoAccesso() As Date
        Get
            Return _UltimoAccesso
        End Get
        Set(ByVal value As Date)
            _UltimoAccesso = value
        End Set
    End Property

    Public Property Abilitato() As Boolean
        Get
            Return _Abilitato
        End Get
        Set(ByVal value As Boolean)
            _Abilitato = value
        End Set
    End Property
End Class


<Serializable()>
Public Class classeOraServer
    Private _data As Date = Now()


    Public Property Data() As Date
        Get
            Return _data
        End Get
        Set(ByVal value As Date)
            _data = value
        End Set
    End Property

End Class


<Serializable()>
Public Class classeGenerica
    Private xdata As New Collection
    Public ReadOnly Property Data() As Collection
        Get
            Return xdata
        End Get
    End Property

    Public Sub Add(ByVal item As Object, ByVal key As String)
        Try
            xdata.Add(item, key)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub Remove(ByVal key As String)
        Try
            xdata.Remove(key)
        Catch ex As Exception

        End Try
    End Sub

End Class


<Serializable()>
Public Class classeCampo
    Private _Campo As String = ""
    Private _Valore As Object

    Public Sub New(ByVal NomeCampo As String, ByVal valore As Object)
        _Campo = NomeCampo
        _Valore = valore
    End Sub

    Public ReadOnly Property Campo As String
        Get
            Return _Campo
        End Get
    End Property

    Public ReadOnly Property Valore As Object
        Get
            Return _Valore
        End Get
    End Property
End Class


<Serializable()>
Public Class JRisposta
    Inherits Dictionary(Of String, Object)

    ''' <summary>
    ''' Crea un nuovo oggetto JRisposta
    ''' </summary>
    ''' <param name="vuoto">valore Boolean che indica se non devono essere presenti i valiri di default stato, messaggio, esegui</param>
    ''' <remarks></remarks>
    Sub New(Optional ByVal vuoto As Boolean = False)
        If Not vuoto Then
            MyBase.Add("stato", Stati.Corretto)
            MyBase.Add("messaggio", "")
        End If
    End Sub

    Public Function ToDataTable(Of T)(ByVal items As List(Of T)) As DataTable
        Dim dataTable As DataTable = New DataTable(GetType(T).Name)
        Dim Props As PropertyInfo() = GetType(T).GetProperties(BindingFlags.[Public] Or BindingFlags.Instance)

        For Each prop As PropertyInfo In Props
            dataTable.Columns.Add(prop.Name)
        Next

        For Each item As T In items
            Dim values = New Object(Props.Length - 1) {}

            For i As Integer = 0 To Props.Length - 1
                values(i) = Props(i).GetValue(item, Nothing)
            Next

            dataTable.Rows.Add(values)
        Next

        Return dataTable
    End Function

    Public Overloads Sub add(ByVal nome As String, ByVal valore As Object)
        If MyBase.ContainsKey(nome) Then
            MyBase.Remove(nome)
            MyBase.Add(nome, valore)
        Else
            MyBase.Add(nome, valore)
        End If
    End Sub

    Public Overloads Property stato As Stati
        Get
            Return MyBase.Item("stato")
        End Get
        Set(ByVal value As Stati)
            MyBase.Item("stato") = value
        End Set
    End Property

    Public Property messaggio As String
        Get
            Return MyBase.Item("messaggio")
        End Get
        Set(ByVal value As String)
            MyBase.Item("messaggio") = value
        End Set
    End Property

    Public Enum Stati
        Neutro = 0
        Attenzione = 2
        Corretto = 1
        Errato = -1
        noSession = -10
    End Enum

End Class

'Imports System.Runtime.CompilerServices

Module EnumerableExtensions
    Function Append(Of T)(ByVal source As IEnumerable(Of T), ParamArray tail As T()) As IEnumerable(Of T)
        Return source.Concat(tail)
    End Function
End Module


Module GeneralFunctions
    Private ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Function getModelStateMessages(msd As ModelStateDictionary) As List(Of String)
        Dim le As List(Of String) = New List(Of String)
        For Each ms As ModelState In msd.Values
            For Each err As ModelError In ms.Errors
                le.Add(err.ErrorMessage)
            Next
        Next
        Return le
    End Function

    Public Function EnumToDataTable(ByVal EnumObject As Type,
   ByVal KeyField As String, ByVal ValueField As String) As DataTable

        Dim oData As DataTable = Nothing
        Dim oRow As DataRow = Nothing
        Dim oColumn As DataColumn = Nothing

        '-------------------------------------------------------------
        ' Sanity check
        If KeyField.Trim() = String.Empty Then
            KeyField = "KEY"
        End If

        If ValueField.Trim() = String.Empty Then
            ValueField = "VALUE"
        End If
        '-------------------------------------------------------------

        '-------------------------------------------------------------
        ' Create the DataTable
        oData = New DataTable

        oColumn = New DataColumn(KeyField, GetType(System.Int32))
        oData.Columns.Add(KeyField)

        oColumn = New DataColumn(ValueField, GetType(System.String))
        oData.Columns.Add(ValueField)
        '-------------------------------------------------------------

        '-------------------------------------------------------------
        ' Add the enum items to the datatable
        For Each iEnumItem As Object In [Enum].GetValues(EnumObject)
            oRow = oData.NewRow()
            oRow(KeyField) = CType(iEnumItem, Int32)
            oRow(ValueField) = StrConv(Replace(iEnumItem.ToString(), "_", " "),
              VbStrConv.None)
            oData.Rows.Add(oRow)
        Next
        '-------------------------------------------------------------

        Return oData

    End Function






    ''' <summary>
    ''' Elenco delle attività per singola Azienda con data molde TaskInfoDataBindig
    ''' </summary>
    ''' <param name="companyID">Identificativo dell'Azienda</param>
    ''' <returns>Lista [TaskInfoDataBindig] di tutte le attività di un'Azienda</returns>
    Public Function getTasksInfoList(companyID As Integer) As List(Of TaskInfoDataBindig)
        Dim lst As New List(Of TaskInfoDataBindig)
        Using db As New ApplicationDbContext
            lst = (From p In db.Products
                   Join cp In db.Companies On p.companyID Equals cp.companyID
                   Join ed In db.Editions On ed.productID Equals p.productID
                   Join act In db.mdActivity On ed.mdActivityID Equals act.mdActivityID
                   Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                   Join tsks In db.mdTasksStates On ed.mdTasksStatesID Equals tsks.mdTasksStatesID
                   Join str In db.Structures On ed.structureID Equals str.structureID
                   Join u In db.Users On u.userID Equals ed.ownerID
                   Where cp.companyID = companyID
                   Select New TaskInfoDataBindig _
                                                               With {.companyID = cp.companyID,
                                                               .BusinessName = cp.BusinessName,
                                                               .productID = p.productID,
                                                               .productName = p.productName,
                                                               .mdClassID = cls.mdClassID,
                                                               .mdClassName = cls.mdClassName,
                                                               .mdCode = p.mdCode,
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
                                                               .email = u.email,
                                                               .fileStatus = ed.fileStatus,
                                                               .productInfoStatus = ed.productInfoStatus,
                                                               .checkListStatus = ed.checkListStatus
                                                               }).ToList
            For Each t As TaskInfoDataBindig In lst
                Try
                    t.appLogs = getLastAppLog(t.editionID, 3)

                Catch ex As Exception
                    log.Error(ex.Message, ex)
                End Try
            Next
        End Using
        Return lst
    End Function


    ''' <summary>
    ''' Dettaglio dell'attività di un'edizione specifica in datas model TaskInfoDataBindig
    ''' </summary>
    ''' <param name="editionID">Identificativo dell'Edizione</param>
    ''' <returns>Dettaglio dell'attività in formato TaskInfoDataBindig</returns>
    Public Function getTasksInfo(editionID As Integer) As TaskInfoDataBindig
        Dim t As New TaskInfoDataBindig
        Using db As New ApplicationDbContext
            t = (From p In db.Products
                 Join cp In db.Companies On p.companyID Equals cp.companyID
                 Join ed In db.Editions On ed.productID Equals p.productID
                 Join act In db.mdActivity On ed.mdActivityID Equals act.mdActivityID
                 Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                 Join tsks In db.mdTasksStates On ed.mdTasksStatesID Equals tsks.mdTasksStatesID
                 Join str In db.Structures On ed.structureID Equals str.structureID
                 Join u In db.Users On u.userID Equals ed.ownerID
                 Where ed.editionID = editionID
                 Select New TaskInfoDataBindig _
                                                               With {.companyID = cp.companyID,
                                                               .BusinessName = cp.BusinessName,
                                                               .productID = p.productID,
                                                               .productName = p.productName,
                                                               .mdClassID = cls.mdClassID,
                                                               .mdClassName = cls.mdClassName,
                                                               .mdCode = p.mdCode,
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
                                                               .email = u.email,
                                                               .fileStatus = ed.fileStatus,
                                                               .productInfoStatus = ed.productInfoStatus,
                                                               .checkListStatus = ed.checkListStatus
                                                               }).FirstOrDefault

        End Using
        Return t
    End Function


    Public Function deleteTaskFiles(editionID As Integer) As Boolean
        Dim r As Boolean = True
        Using db As New ApplicationDbContext
            Dim ed As Editions = (From e In db.Editions Where e.editionID = editionID Select e).FirstOrDefault
            If Not IsNothing(ed) Then
                Try
                    Dim temp_dir As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_temp)
                    Dim StoragePath As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_storage)
                    Dim tempEdition As String = Path.Combine(temp_dir, editionID)
                    Dim storageEdition As String = Path.Combine(StoragePath, editionID)
                    If Directory.Exists(tempEdition) Then Directory.Delete(tempEdition, True)
                    If Directory.Exists(storageEdition) Then Directory.Delete(storageEdition, True)
                    If File.Exists(tempEdition & ".xml") Then File.Delete(tempEdition & ".xml")
                    If File.Exists(tempEdition & ".archive") Then File.Delete(tempEdition & ".archive")
                    db.Database.ExecuteSqlCommand("Update Details set flagState = 0, fileExtension = '' Where editionID = " & editionID)
                Catch ex As Exception
                    log.Error("[deleteTaskFiles/" & editionID & "] : " & ex.Message, ex)
                    r = False
                End Try
            Else
                r = False
            End If

        End Using

        Return r
    End Function


#Region "Datatable ed Entity framework .."
    ''' <summary>
    ''' Converte un risultato Entity Framework in Datatable
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="items"></param>
    ''' <returns></returns>
    Public Function ToDataTable(Of T)(ByVal items As List(Of T)) As DataTable
        Dim dataTable As DataTable = New DataTable(GetType(T).Name)
        Dim Props As PropertyInfo() = GetType(T).GetProperties(BindingFlags.[Public] Or BindingFlags.Instance)

        For Each prop As PropertyInfo In Props
            dataTable.Columns.Add(prop.Name)
        Next

        For Each item As T In items
            Dim values = New Object(Props.Length - 1) {}

            For i As Integer = 0 To Props.Length - 1
                If Props(i).GetValue(item, Nothing).GetType.BaseType.Name = "Enum" Then
                    dataTable.Columns(i).DataType = System.Type.GetType(Props(i).GetValue(item, Nothing).GetType.FullName)
                    values(i) = CInt(Props(i).GetValue(item, Nothing))
                Else
                    values(i) = Props(i).GetValue(item, Nothing)
                End If
            Next

            dataTable.Rows.Add(values)
        Next

        Return dataTable
    End Function

    ''' <summary>
    ''' Funzione che converte un datatable in EF
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    Public Function toEntityFramework(Of T)(ByVal dt As DataTable) As List(Of T)
        Dim data As List(Of T) = New List(Of T)()

        For Each row As DataRow In dt.Rows
            Dim item As T = GetItem(Of T)(row)
            data.Add(item)
        Next

        Return data
    End Function

    ''' <summary>
    ''' Funzione utile a toEntityFramework per la conversione di un Datatable a EF
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="dr"></param>
    ''' <returns></returns>
    Private Function GetItem(Of T)(ByVal dr As DataRow) As T
        Dim temp As Type = GetType(T)
        Dim obj As T = Activator.CreateInstance(Of T)()

        For Each column As DataColumn In dr.Table.Columns

            For Each pro As PropertyInfo In temp.GetProperties()

                If pro.Name = column.ColumnName Then
                    Dim v = dr(column.ColumnName)
                    'Try
                    '    If Int32.Parse(v) Then v = Convert.ToInt32(v)
                    'Catch ex As Exception
                    'End Try

                    'Try
                    '    Dim flag As Boolean
                    '    If Boolean.TryParse(v, flag) Then v = flag
                    'Catch ex As Exception
                    'End Try
                    Dim exclude As List(Of String) = "List`1".Split(",").ToList
                    If Not exclude.Contains(pro.PropertyType.Name) Then
                        If pro.PropertyType.BaseType.Name = "Enum" Then
                            pro.SetValue(obj, [Enum].Parse(pro.PropertyType, v), Nothing)
                        Else
                            pro.SetValue(obj, Convert.ChangeType(v, pro.PropertyType), Nothing)

                        End If
                    End If
                    Exit For
                Else
                    Continue For
                End If
            Next
        Next

        Return obj
    End Function

#End Region


#Region "File system"


    Public Function FileSystemInfoList(p As String) As List(Of FileSystemInfo)
        Dim fileList As List(Of FileSystemInfo)
        Dim di As New DirectoryInfo(p)
        If Directory.Exists(di.FullName) Then '   Se la directori A esiste..
            'fileList = di.GetFiles("*.*", System.IO.SearchOption.AllDirectories).Where(AddressOf exclusions).ToList()  'Escludo il percorso di destinazione dal controllo
            fileList = di.GetFileSystemInfos("*.*", SearchOption.AllDirectories).Where(AddressOf exclusions).OrderBy(Function(fi) fi.FullName).ToList()  'Escludo il percorso di destinazione dal controllo
        End If
        Return fileList
    End Function

    Private Function exclusions(e As FileSystemInfo) As Boolean
        Dim r As Boolean = True ' Non escuso per default
        Return r
    End Function

    Private Function SortArray(ByVal x As FileSystemInfo, ByVal y As FileSystemInfo) As Integer
        Return x.FullName.CompareTo(y.FullName)
    End Function

    Public Function createCustomStructureDB(p As String, productID As Integer, editionID As Integer, idParent As Integer, ByRef progressiveID As Integer) As Boolean
        Dim r As Boolean = True
        Dim structureID As Integer = 1
        Dim di As New DirectoryInfo(p)
        If Directory.Exists(di.FullName) Then '   Se la directori A esiste..

            Dim db As New ApplicationDbContext
            If idParent = 0 Then
                Try
                    db.Database.ExecuteSqlCommand("DELETE FROM Details WHERE editionID = " & editionID)

                Catch ex As Exception
                    Dim m = ex.Message
                End Try
            End If

            Dim list As List(Of FileSystemInfo) = di.GetFileSystemInfos("*.*", SearchOption.TopDirectoryOnly).Where(AddressOf exclusions).OrderBy(Function(fi) fi.FullName).ToList()  'Escludo il percorso di destinazione dal controllo
            For Each fsi As FileSystemInfo In list
                progressiveID += 1
                Dim flagContainer As Integer = 1
                Try
                    If (fsi.Attributes And FileAttributes.Directory) = FileAttributes.Directory Then
                        flagContainer = 2
                    Else
                        flagContainer = 1
                    End If

                Catch ex As Exception
                    log.Error(ex.Message)
                End Try
                Dim d As New Details
                With d
                    .editionID = editionID
                    .documentID = progressiveID
                    .addFile = 0
                    .addFolder = 0
                    .nLevels = 0
                    .fileExtension = fsi.Extension.Replace(".", "")
                    .fileName = fsi.Name
                    .flagContainer = flagContainer
                    .flagState = 2
                    .relPath = Right(fsi.FullName, (fsi.FullName.Length - (di.FullName.Length + 1)))
                    .fullPath = fsi.FullName
                    .idParent = idParent
                    .idVerDoc = 0
                    .operatorID = 1
                    .productID = productID
                    .structureID = 1
                    .swTarget = 1
                    .Title = fsi.Name
                End With
                db.Details.Add(d)
                Try
                    db.SaveChanges()

                Catch ex As Exception
                    log.Error("createCustomStructure : " & ex.Message)
                End Try

                If flagContainer = 2 Then

                    createCustomStructureDB(fsi.FullName, productID, editionID, d.documentID, progressiveID)
                End If

            Next

        Else

        End If

        Return r

    End Function


    Public Function createCustomStructureDT(source_path As String, editionID As Integer, idParent As Integer, ByRef progressiveID As Integer) As Boolean
        Dim r As Boolean = True
        Dim di As New DirectoryInfo(source_path)
        If idParent = 0 Then log.Info(String.Format("Creazione della struttura dati dell'Edizione -{0}- tipo 'Details' dal percorso: {1}", editionID, source_path))
        If Directory.Exists(di.FullName) Then '   Se la directory esiste..

            Dim db As New ApplicationDbContext
            Dim ed As Editions = (From e In db.Editions Where e.editionID = editionID Select e).FirstOrDefault
            If Not IsNothing(ed) Then
                If idParent = 0 Then
                    Try
                        db.Database.ExecuteSqlCommand("DELETE FROM Details WHERE editionID = " & editionID)

                    Catch ex As Exception
                        Dim m = ex.Message
                    End Try
                End If

                Dim list As List(Of FileSystemInfo) = di.GetFileSystemInfos("*.*", SearchOption.TopDirectoryOnly).Where(AddressOf exclusions).OrderBy(Function(fi) fi.FullName).ToList()  'Escludo il percorso di destinazione dal controllo
                For Each fsi As FileSystemInfo In list
                    progressiveID += 1
                    Dim flagContainer As Integer = 1
                    Try
                        If (fsi.Attributes And FileAttributes.Directory) = FileAttributes.Directory Then
                            flagContainer = 2
                        Else
                            flagContainer = 1
                        End If

                    Catch ex As Exception
                        log.Error(ex.Message)
                    End Try
                    Dim d As New Details
                    With d
                        .editionID = editionID
                        .documentID = progressiveID
                        .addFile = 0
                        .addFolder = 0
                        .nLevels = 0
                        .fileExtension = fsi.Extension.Replace(".", "")
                        .fileName = fsi.Name
                        .flagContainer = flagContainer
                        .flagState = 2
                        .relPath = Right(fsi.FullName, (fsi.FullName.Length - (di.FullName.Length + 1)))
                        .fullPath = fsi.FullName
                        .idParent = idParent
                        .idVerDoc = 0
                        .operatorID = 1
                        .productID = ed.productID
                        .structureID = 1
                        .swTarget = 1
                        .Title = fsi.Name
                    End With
                    db.Details.Add(d)
                    Try
                        db.SaveChanges()

                    Catch ex As Exception
                        log.Error("createCustomStructure : " & ex.Message)
                    End Try

                    If flagContainer = 2 Then
                        createCustomStructureDT(fsi.FullName, editionID, d.documentID, progressiveID)
                    End If

                Next

                If idParent = 0 Then


                    'Dim listDetails As List(Of Details) = (From dx In db.Details Where dx.editionID = editionID Select dx).ToList()
                    'Dim dtDetails As DataTable = ToDataTable(Of Details)(listDetails)
                    'dtDetails.TableName = "Details"

                    log.Info(String.Format("Generazione del file indice '{0}' del percorso '{1}'", di.FullName & ".xml", source_path))

                    Dim DetailsTreeModel_list As List(Of DetailsTreeModel) = (From p In db.Products
                                                                              Join d In db.Details On p.productID Equals d.productID
                                                                              Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                                                                              Join e In db.Editions On d.editionID Equals e.editionID
                                                                              Join str In db.Structures On str.structureID Equals e.structureID
                                                                              Join c In db.Companies On c.companyID Equals p.companyID
                                                                              Where e.editionID = editionID
                                                                              Select New DetailsTreeModel With {
                                                                          .detailID = d.detailID,
                                                                          .structureID = str.structureID,
                                                                          .structureName = str.structureName,
                                                                           .asZipFile = e.asZipFile,
                                                                          .editionID = e.editionID,
                                                                          .editionName = e.editionName,
                                                                          .certificationPlan = e.certificationPlan,
                                                                          .productID = p.productID,
                                                                          .productName = p.productName,
                                                                          .companyID = c.companyID,
                                                                          .BusinessName = c.BusinessName,
                                                                          .mdClassID = cls.mdClassID,
                                                                          .mdClassName = cls.mdClassName,
                                                                          .mdCode = p.mdCode,
                                                                          .Title = d.Title,
                                                                          .idParent = d.idParent,
                                                                          .documentID = d.documentID,
                                                                          .fileName = d.fileName,
                                                                          .addFile = d.addFile,
                                                                          .addFolder = d.addFolder,
                                                                          .nLevels = d.nLevels,
                                                                          .idVerDoc = d.idVerDoc,
                                                                          .flagState = d.flagState,
                                                                          .fileExtension = d.fileExtension,
                                                                          .operatorID = d.operatorID,
                                                                          .MD5 = d.MD5,
                                                                          .swTarget = d.swTarget,
                                                                          .file_for_checklist = d.file_for_checklist,
                                                                          .fullPath = d.fullPath,
                                                                          .flagContainer = d.flagContainer,
                                                                          .fileStatus = e.fileStatus,
                                                                          .checkListStatus = e.checkListStatus,
                                                                          .productInfoStatus = e.productInfoStatus,
                                                                            .dataOrder = d.dataOrder
                                                                          }).ToList


                    Dim dtDetailsTreeModel As DataTable = ToDataTable(Of DetailsTreeModel)(DetailsTreeModel_list)
                    dtDetailsTreeModel.TableName = "DetailsTreeModel"

                    dtDetailsTreeModel.WriteXml(di.FullName & ".xml", XmlWriteMode.WriteSchema)
                    log.Info(String.Format("Generazione del file indice '{0}' conclusa", di.FullName & ".xml"))

                    If Not IsNothing(ed) Then

                        Try
                            db.Database.ExecuteSqlCommand("DELETE FROM Details WHERE editionID = " & editionID)

                        Catch ex As Exception
                            Dim m = ex.Message
                        End Try

                        createTemplateStructureDB(editionID, ed.ownerID)

                    End If

                End If

            Else
                r = False
            End If



        Else
            r = False
            log.Error("Archivio non presente, impossibile recuperare informazioni.")
        End If

        Return r

    End Function

    Public Function getCustomStructureDT(editionID As Integer) As DataTable
        Dim StoragePath As String = Path.Combine(My.Application.Info.DirectoryPath, My.Settings.path_temp)
        Dim fullpath As String = Path.Combine(StoragePath, editionID & ".xml")
        Dim dt As New DataTable
        If File.Exists(fullpath) Then
            dt.ReadXml(fullpath)
        End If
        Return dt
    End Function

    ''' <summary>
    ''' Alimenta la tabella Details con la struttura scelta e presente in StrucureDetails per l'edizione indicata
    ''' </summary>
    ''' <param name="editionID">Identificativo dell'Edizione</param>
    ''' <param name="userID">Identificativo del proprietario dell'attività</param>
    ''' <returns></returns>
    Public Function createTemplateStructureDB(editionID As Integer, userID As String) As Boolean
        Dim r As Boolean = True
        log.Info(String.Format("Generazione dati della tabella 'Details' per l'Edizione {0}-.", editionID))

        Using db As New ApplicationDbContext
            Dim ed As Editions = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault
            If Not IsNothing(ed) Then
                'Verifico che non ci siano file gestiti in precedenza per questa edizione..
                Dim n As Integer = (From d In db.Details Where d.editionID = editionID And d.flagState > 0).Count

                If n > 0 Then
                    r = False
                    log.Error("Edizione già presente")
                Else

                    'Elimino l'eventuale pregresso per reimpostare la nuova struttura
                    Try
                        db.Database.ExecuteSqlCommand("DELETE FROM Details WHERE editionID = " & editionID)

                    Catch ex As Exception
                        Dim m = ex.Message
                    End Try


                    Dim list As List(Of Details) = (From sd In db.StructureDetails Where sd.structureID = ed.structureID Select sd).ToList.Select(Function(sdx) New Details With {
                                                                                                                                      .addFile = sdx.addFile,
                                                                                                                                     .addFolder = sdx.addFolder,
                                                                                                                                     .documentID = sdx.documentID,
                                                                                                                                     .editionID = editionID,
                                                                                                                                     .fileExtension = sdx.fileExtension,
                                                                                                                                     .fileName = sdx.fileName,
                                                                                                                                     .file_for_checklist = sdx.file_for_checklist,
                                                                                                                                     .flagContainer = sdx.flagContainer,
                                                                                                                                     .flagState = sdx.flagState,
                                                                                                                                     .relPath = sdx.relPath,
                                                                                                                                     .fullPath = sdx.fullPath,
                                                                                                                                     .idParent = sdx.idParent,
                                                                                                                                     .idVerDoc = sdx.idVerDoc,
                                                                                                                                     .MD5 = sdx.MD5,
                                                                                                                                     .nLevels = sdx.nLevels,
                                                                                                                                     .operatorID = userID,
                                                                                                                                     .productID = ed.productID,
                                                                                                                                     .structureID = sdx.structureID,
                                                                                                                                     .swTarget = sdx.swTarget,
                                                                                                                                     .Title = sdx.Title,
                                                                                                                                     .dataOrder = sdx.dataOrder
                                                                                                                                      }).ToList





                    db.Configuration.AutoDetectChangesEnabled = False
                    db.Details.AddRange(list)
                    db.ChangeTracker.DetectChanges()
                    db.SaveChanges()

                    log.Info(String.Format("Generazione dati della tabella 'Details' per l'Edizione -{0}- completata.", editionID))
                End If
            Else
                log.Error("Edizione non presente")
                r = False
            End If
        End Using


        Return r
    End Function

    Public Function createFileSystemFromDB(editionID As Integer)
        Dim r As Boolean = True
        Dim db As New ApplicationDbContext
        Dim ed As Editions = (From e In db.Editions Where editionID = editionID).FirstOrDefault
        Dim storePath As String = Path.Combine(My.Application.Info.DirectoryPath, "store")
        storePath = Path.Combine(storePath, editionID)
        If Not Directory.Exists(storePath) Then Directory.CreateDirectory(storePath)

        Dim list As List(Of Details) = (From d In db.Details Where d.editionID = editionID Select d Order By d.idParent, d.Title, d.documentID).ToList
        For Each row As Details In list

        Next


        Return r
    End Function

    Public Function getTreeList(editionID As Integer, Optional ByVal fromFileSystem As Boolean = False) As List(Of DetailsTreeModel)
        Dim t As List(Of DetailsTreeModel) = New List(Of DetailsTreeModel)
        Using db As New ApplicationDbContext
            Dim ed As Editions = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault
            If Not IsNothing(ed) Then
                If ed.asZipFile And fromFileSystem Then
                    Dim dt As DataTable = getCustomStructureDT(editionID)
                    Try
                        t = toEntityFramework(Of DetailsTreeModel)(dt)
                    Catch ex As Exception
                        log.Error("init_StructureDetails: " & ex.Message)
                    End Try
                Else
                    t = (From p In db.Products
                         Join d In db.Details On p.productID Equals d.productID
                         Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                         Join e In db.Editions On d.editionID Equals e.editionID
                         Join str In db.Structures On str.structureID Equals e.structureID
                         Join c In db.Companies On c.companyID Equals p.companyID
                         Where e.editionID = editionID
                         Order By d.dataOrder
                         Select New DetailsTreeModel With {
                                        .detailID = d.detailID,
                                        .structureID = str.structureID,
                                        .structureName = str.structureName,
                                        .asZipFile = e.asZipFile,
                                        .editionID = e.editionID,
                                        .editionName = e.editionName,
                                        .certificationPlan = e.certificationPlan,
                                        .productID = p.productID,
                                        .productName = p.productName,
                                        .companyID = c.companyID,
                                        .BusinessName = c.BusinessName,
                                        .mdClassID = cls.mdClassID,
                                        .mdClassName = cls.mdClassName,
                                        .mdCode = p.mdCode,
                                        .Title = d.Title,
                                        .idParent = d.idParent,
                                        .documentID = d.documentID,
                                        .fileName = d.fileName,
                                        .addFile = d.addFile,
                                        .addFolder = d.addFolder,
                                        .nLevels = d.nLevels,
                                        .idVerDoc = d.idVerDoc,
                                        .flagState = d.flagState,
                                        .fileExtension = d.fileExtension,
                                        .operatorID = d.operatorID,
                                        .MD5 = d.MD5,
                                        .swTarget = d.swTarget,
                                        .file_for_checklist = d.file_for_checklist,
                                        .fullPath = d.fullPath,
                                        .flagContainer = d.flagContainer,
                                        .fileStatus = e.fileStatus,
                                        .checkListStatus = e.checkListStatus,
                                        .productInfoStatus = e.productInfoStatus,
                                         .dataOrder = d.dataOrder
                                        }).ToList

                End If

            End If


        End Using

        Return makeTreeList(t)
    End Function

    Public Function getDetailData(detailID As Long) As DetailsTreeModel
        Dim t As New DetailsTreeModel
        Using db As New ApplicationDbContext
            t = (From p In db.Products
                 Join d In db.Details On p.productID Equals d.productID
                 Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                 Join e In db.Editions On d.editionID Equals e.editionID
                 Join str In db.Structures On str.structureID Equals e.structureID
                 Join c In db.Companies On c.companyID Equals p.companyID
                 Where d.detailID = detailID
                 Select New DetailsTreeModel With {
                                        .detailID = d.detailID,
                                        .structureID = str.structureID,
                                        .structureName = str.structureName,
                                        .asZipFile = e.asZipFile,
                                        .editionID = e.editionID,
                                        .editionName = e.editionName,
                                        .certificationPlan = e.certificationPlan,
                                        .productID = p.productID,
                                        .productName = p.productName,
                                        .companyID = c.companyID,
                                        .BusinessName = c.BusinessName,
                                        .mdClassID = cls.mdClassID,
                                        .mdClassName = cls.mdClassName,
                                        .mdCode = p.mdCode,
                                        .Title = d.Title,
                                        .idParent = d.idParent,
                                        .documentID = d.documentID,
                                        .fileName = d.fileName,
                                        .addFile = d.addFile,
                                        .addFolder = d.addFolder,
                                        .nLevels = d.nLevels,
                                        .idVerDoc = d.idVerDoc,
                                        .flagState = d.flagState,
                                        .fileExtension = d.fileExtension,
                                        .operatorID = d.operatorID,
                                        .MD5 = d.MD5,
                                        .swTarget = d.swTarget,
                                        .file_for_checklist = d.file_for_checklist,
                                        .fullPath = d.fullPath,
                                        .flagContainer = d.flagContainer,
                                        .fileStatus = e.fileStatus,
                                        .checkListStatus = e.checkListStatus,
                                        .productInfoStatus = e.productInfoStatus,
                                        .dataOrder = d.dataOrder
                                        }).FirstOrDefault
        End Using

        Return t
    End Function

    ''' <summary>
    ''' Genera le informazioni utili al frontend per visualizzare informazioni con gerarchia per rappresentare una truttura ad albero del filesystem
    ''' </summary>
    ''' <param name="data">Lista estratta dalla query che contiene tutti i dati da rappresentare</param>
    ''' <param name="parentID">Indica gli elementi figlio da considerare riferiti ad un elemento pade</param>
    ''' <returns></returns>
    Public Function makeTreeList(ByRef data As List(Of DetailsTreeModel), Optional parentID As Integer = 0) As List(Of DetailsTreeModel)
        Dim items As New List(Of DetailsTreeModel)
        Try
            For Each r In (From row In data Select row Where row.idParent = parentID Order By row.dataOrder).ToList()
                Dim n As New DetailsTreeModel
                'n = CType(Convert.ChangeType(r, r.GetType), DetailsTreeModel)
                n = r
                n.children = makeTreeList(data, r.documentID)
                items.Add(r)
            Next

        Catch ex As Exception
            log.Error("makeTreeList : " & ex.Message)
        End Try
        Return items
    End Function

    'Public Sub Download(ByVal rel_path As String, idRichiestaAbilitazione As Integer)
    '    'Dim strFileName As String = String.Format("{0}.zip", id)
    '    'Dim strRootRelativePathName As String = String.Format("~/App_Data/Files/{0}", strFileName)
    '    Dim strPathName As String = Server.MapPath(rel_path)
    '    Dim fi As New FileInfo(strPathName)
    '    If System.IO.File.Exists(strPathName) = False Then
    '        Return
    '    End If

    '    Dim oStream As System.IO.Stream = Nothing

    '    Try
    '        oStream = New System.IO.FileStream(path:=strPathName, mode:=System.IO.FileMode.Open, share:=System.IO.FileShare.Read, access:=System.IO.FileAccess.Read)
    '        Response.Buffer = False
    '        'Response.ContentType = "application/octet-stream"
    '        Response.ContentType = "application/x-zip-compressed"
    '        Response.AddHeader("Content-Disposition", "attachment; filename=" & fi.Name)
    '        Dim lngFileLength As Long = oStream.Length
    '        Response.AddHeader("Content-Length", lngFileLength.ToString())
    '        Dim lngDataToRead As Long = lngFileLength

    '        While lngDataToRead > 0

    '            If Response.IsClientConnected Then
    '                Dim intBufferSize As Integer = 8 * 1024
    '                Dim bytBuffers As Byte() = New System.Byte(intBufferSize - 1) {}
    '                Dim intTheBytesThatReallyHasBeenReadFromTheStream As Integer = oStream.Read(buffer:=bytBuffers, offset:=0, count:=intBufferSize)
    '                Response.OutputStream.Write(buffer:=bytBuffers, offset:=0, count:=intTheBytesThatReallyHasBeenReadFromTheStream)
    '                Response.Flush()
    '                lngDataToRead = lngDataToRead - intTheBytesThatReallyHasBeenReadFromTheStream
    '            Else
    '                lngDataToRead = -1
    '            End If
    '        End While
    '        If idRichiestaAbilitazione <> 0 Then db.AggiornaRecord("tbl_Receivers", "StatoInstallazione", "1", "Where idRichiestaAbilitazione = " & idRichiestaAbilitazione)

    '    Catch
    '    Finally

    '        If oStream IsNot Nothing Then
    '            oStream.Close()
    '            oStream.Dispose()
    '            oStream = Nothing
    '        End If
    '        Response.Close()
    '    End Try
    'End Sub

    ''' <summary>
    ''' Funzione per decomprimere file Zip, Rar, 7zip, ed altri formati
    ''' </summary>
    ''' <param name="sourceFile">Percorso completo della posizione del file compresso.</param>
    ''' <param name="outputFolder">Percorso di destinazione del contenuto da decomprimere.</param>
    ''' <param name="overwrite">Flag di sovrascrittura del contenuto se già presente</param>
    ''' <param name="checkFileIntegrity">Flag di verifica integrità dell'archivio.</param>
    Public Function UnpackArchive(ByVal sourceFile As String, ByVal outputFolder As String, ByVal overwrite As Boolean, ByVal checkFileIntegrity As Boolean) As archiveInfo
        Dim ai As New archiveInfo
        log.Info(String.Format("Estrazione del file '{0}' in '{1}' in corso..", sourceFile, outputFolder))
        Using extracter As New SevenZipExtractor(sourceFile)
            If (checkFileIntegrity _
                    AndAlso Not extracter.Check) Then
                Throw New Exception(String.Format("Appears to be an invalid archive: {0}", sourceFile))
            End If
            ai.Format = extracter.Format.ToString
            ai.ArchiveFileNames = extracter.ArchiveFileNames.ToList

            If Not Directory.Exists(outputFolder) Then Directory.CreateDirectory(outputFolder) ' outputFolder.MakeSurePathExists
            'TODO: Warning!!!, inline IF is not supported ?
            extracter.ExtractFiles(outputFolder.ToString(), If(overwrite, extracter.ArchiveFileNames.ToArray(), extracter.ArchiveFileNames.Where(Function(x) Not File.Exists(Path.Combine(outputFolder, x))).ToArray()))

        End Using
        log.Info(String.Format("Fine estrazione del file '{0}'.", sourceFile))
        Return ai
    End Function

    Public Class archiveInfo
        Public Property Format As String = ""
        Public Property ArchiveFileNames As List(Of String) = New List(Of String)
    End Class

    ''' <summary>
    ''' Metodo per alimentare la Tabella ActivityLog con le operazioni richieste dagli utenti
    ''' </summary>
    ''' <param name="editionID">Identificativo dell'edizione del TechnicalFile</param>
    ''' <param name="mdTasksStatesID">Operazione svolta secondo la tabella mdTasksStates</param>
    ''' <param name="resultID">Stato finale dell'operazione</param>
    ''' <param name="resultMessage">Messaggio finale dell'operazione</param>
    ''' <param name="userID">Identificativo del proprietario dell'attività</param>
    ''' <param name="startActiviyDate">Quando è iniziata l'attività</param>
    ''' <param name="stopActiviyDate">Quando è terminata l'attività</param>
    Public Sub AppLog(editionID As Integer, mdTasksStatesID As mdTaskStates_enum, resultID As JRisposta.Stati, resultMessage As String, userID As String, Optional ByVal startActiviyDate As Nullable(Of Date) = Nothing, Optional ByVal stopActiviyDate As Nullable(Of Date) = Nothing)
        Dim al As New ActivityLog
        With al
            .editionID = editionID
            .mdTasksStatesID = mdTasksStatesID
            .resultID = resultID
            .resultMessage = resultMessage
            .startActiviyDate = IIf(IsNothing(startActiviyDate), Now, startActiviyDate)
            .stopActiviyDate = IIf(IsNothing(stopActiviyDate), Now, stopActiviyDate)
            .userID = userID
        End With
        Using db As New ApplicationDbContext
            db.ActivityLog.Add(al)
            Try
                db.SaveChanges()
                Select Case resultID
                    Case JRisposta.Stati.Errato
                        log.Error(resultMessage)
                    Case JRisposta.Stati.Corretto
                        log.Info(resultMessage)
                    Case JRisposta.Stati.Attenzione
                        log.Warn(resultMessage)
                End Select
            Catch ex As Exception
                log.Error(ex.Message, ex)
            End Try

        End Using

    End Sub

    Public Function getLastAppLog(editionID As Integer, Optional ByVal n As Integer = 1) As List(Of ActivityLogModel)
        Dim l As New List(Of ActivityLogModel)
        Using db As New ApplicationDbContext
            l = (From la In db.ActivityLog
                 Join ts In db.mdTasksStates On la.mdTasksStatesID Equals ts.mdTasksStatesID
                 Join u In db.Users On la.userID Equals u.userID
                 Where la.editionID = editionID Order By la.insertDate Descending Select New ActivityLogModel With {
                                                                           .activityID = la.activityID,
                                                                           .displayName = u.displayName,
                                                                           .editionID = la.editionID,
                                                                           .email = u.email,
                                                                           .insertDate = la.insertDate,
                                                                           .mdTasksStatesID = la.mdTasksStatesID,
                                                                           .mdTasksStatesName = ts.mdTasksStatesName,
                                                                           .resultID = la.resultID,
                                                                           .resultMessage = la.resultMessage,
                                                                           .startActiviyDate = la.startActiviyDate,
                                                                           .stopActiviyDate = la.stopActiviyDate,
                                                                           .userID = la.userID,
                                                                           .userName = u.userName
                                                                           }).Take(n).ToList()
        End Using
        Return l
    End Function

#End Region

#Region "Backup e Ripristino"
    Dim backupPath As String = Path.Combine(My.Application.Info.DirectoryPath, "BackupData")

    Public Function saveAllData()
        Dim dtIndex As DataTable = newBackupIndex()
        If Not Directory.Exists(backupPath) Then Directory.CreateDirectory(backupPath)
        Dim backupFile As String = Path.Combine(backupPath, "index.xml")
        If File.Exists(backupFile) Then dtIndex.ReadXml(backupFile)

        Dim ds As New DataSet("Backup")
        Using db As New ApplicationDbContext

        End Using


    End Function

    Private Function newBackupIndex() As DataTable
        Dim table As New DataTable("Index")
        table.Columns.Add("ID", GetType(Integer))
        table.Columns.Add("relPath", GetType(String))
        table.Columns.Add("Date", GetType(DateTime))
        table.Columns("ID").AutoIncrement = True
        table.Columns("ID").AutoIncrementSeed = 1
        table.Columns("ID").AutoIncrementSeed = 1

        Return table
    End Function

#End Region

    Function cripta(strTesto, Optional intKey = 5) As String
        Dim ctInd
        Dim chrAnalisi
        Dim strTesto2 As String = ""
        For ctInd = 1 To Len(strTesto)
            chrAnalisi = Mid(strTesto, ctInd, 1)
            chrAnalisi = Asc(chrAnalisi) + intKey
            chrAnalisi = chrAnalisi Mod 256
            strTesto2 = strTesto2 & Chr(chrAnalisi)
        Next
        cripta = strTesto2
    End Function

    Public Function decripta(strTesto, Optional intKey = 5)
        Dim ctInd
        Dim chrAnalisi
        Dim IntValore
        Dim intResto
        Dim strTesto2 As String = ""
        For ctInd = 1 To Len(strTesto)
            chrAnalisi = Mid(strTesto, ctInd, 1)
            IntValore = Asc(chrAnalisi)
            intResto = (intKey + IntValore) Mod 256
            If (IntValore + intKey < 256) Then
                strTesto2 = strTesto2 & Chr(IntValore - intKey)
            Else
                strTesto2 = strTesto2 & Chr(256 - intKey + intResto)
            End If
        Next
        decripta = strTesto2
    End Function

End Module