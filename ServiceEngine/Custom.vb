Imports System.Data.Entity
Imports System.IO
Imports System.Reflection
Imports System.Web.Http.ModelBinding
Imports log4net

Public Class ClassiJSON
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
                values(i) = Props(i).GetValue(item, Nothing)
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

                    pro.SetValue(obj, Convert.ChangeType(v, pro.PropertyType), Nothing)
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

    Public Function createTemplateStructureDB(editionID As Integer, userID As String) As Boolean
        Dim r As Boolean = True

        Using db As New ApplicationDbContext
            Dim ed As Editions = (From e In db.Editions Where e.editionID = editionID).FirstOrDefault
            If Not IsNothing(ed) Then
                Dim n As Integer = (From d In db.Details Where d.editionID = editionID).Count
                If n > 0 Then
                    r = False
                    log.Error("Edizione non presente")
                Else
                    'Dim sd = (From sdx In db.StructureDetails Where sdx.structureID = ed.structureID Select New With {
                    '                                                                                                                  .addFile = sdx.addFile,
                    '                                                                                                                 .addFolder = sdx.addFolder,
                    '                                                                                                                 .documentID = sdx.documentID,
                    '                                                                                                                 .editionID = editionID,
                    '                                                                                                                 .fileExtension = sdx.fileExtension,
                    '                                                                                                                 .fileName = sdx.fileName,
                    '                                                                                                                 .file_for_checklist = sdx.file_for_checklist,
                    '                                                                                                                 .flagContainer = sdx.flagContainer,
                    '                                                                                                                 .flagState = sdx.flagState,
                    '                                                                                                                 .fullPath = sdx.fullPath,
                    '                                                                                                                 .idParent = sdx.idParent,
                    '                                                                                                                 .idVerDoc = sdx.idVerDoc,
                    '                                                                                                                 .MD5 = sdx.MD5,
                    '                                                                                                                 .nLevels = sdx.nLevels,
                    '                                                                                                                 .operatorID = userID,
                    '                                                                                                                 .productID = ed.productID,
                    '                                                                                                                 .structureID = sdx.structureID,
                    '                                                                                                                 .swTarget = sdx.swTarget,
                    '                                                                                                                 .Title = sdx.Title
                    '                                                                                                                  }).ToList


                    'Dim list As List(Of Details) = sd.Select(Function(sdx) New Details With {
                    '                                                                                                                  .addFile = sdx.addFile,
                    '                                                                                                                 .addFolder = sdx.addFolder,
                    '                                                                                                                 .documentID = sdx.documentID,
                    '                                                                                                                 .editionID = editionID,
                    '                                                                                                                 .fileExtension = sdx.fileExtension,
                    '                                                                                                                 .fileName = sdx.fileName,
                    '                                                                                                                 .file_for_checklist = sdx.file_for_checklist,
                    '                                                                                                                 .flagContainer = sdx.flagContainer,
                    '                                                                                                                 .flagState = sdx.flagState,
                    '                                                                                                                 .fullPath = sdx.fullPath,
                    '                                                                                                                 .idParent = sdx.idParent,
                    '                                                                                                                 .idVerDoc = sdx.idVerDoc,
                    '                                                                                                                 .MD5 = sdx.MD5,
                    '                                                                                                                 .nLevels = sdx.nLevels,
                    '                                                                                                                 .operatorID = userID,
                    '                                                                                                                 .productID = ed.productID,
                    '                                                                                                                 .structureID = sdx.structureID,
                    '                                                                                                                 .swTarget = sdx.swTarget,
                    '                                                                                                                 .Title = sdx.Title
                    '                                                                                                                  }).ToList





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
                                                                                                                                     .fullPath = sdx.fullPath,
                                                                                                                                     .idParent = sdx.idParent,
                                                                                                                                     .idVerDoc = sdx.idVerDoc,
                                                                                                                                     .MD5 = sdx.MD5,
                                                                                                                                     .nLevels = sdx.nLevels,
                                                                                                                                     .operatorID = userID,
                                                                                                                                     .productID = ed.productID,
                                                                                                                                     .structureID = sdx.structureID,
                                                                                                                                     .swTarget = sdx.swTarget,
                                                                                                                                     .Title = sdx.Title
                                                                                                                                      }).ToList





                    db.Configuration.AutoDetectChangesEnabled = False
                    'db.Configuration.ValidateOnSaveEnabled = False
                    'For Each d As Details In list
                    '    Try
                    '        db.Details.Add(d)
                    '        db.Entry(d).State = EntityState.Modified
                    '        db.SaveChanges()

                    '    Catch ex As Exception
                    '        log.Error("createTemplateStructureDB : " & ex.Message)
                    '    End Try
                    'Next
                    db.Details.AddRange(list)
                    db.ChangeTracker.DetectChanges()
                    db.SaveChanges()
                    'db.Details.AddRange(list)
                    'db.SaveChanges()

                End If
            Else
                log.Error("Edizione non presente")
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



    Public Function makeTreeList(ByRef data As List(Of DetailsTreeModel), Optional parentID As Integer = 0) As List(Of DetailsTreeModel)
        Dim items As New List(Of DetailsTreeModel)
        Try
            For Each r In data.Where(Function(x) (x.idParent = parentID)).OrderBy(Function(y) y.flagContainer).ThenBy(Function(y) y.Title).ToList()
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