Imports System.Reflection
Imports System.Web.Http.ModelBinding

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
              VbStrConv.ProperCase)
            oData.Rows.Add(oRow)
        Next
        '-------------------------------------------------------------

        Return oData

    End Function


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