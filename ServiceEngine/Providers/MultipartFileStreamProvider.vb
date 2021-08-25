Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks

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

Class CustomMultipartFileStreamProvider
    Inherits MultipartMemoryStreamProvider

    Public Property CustomData As List(Of MyCustomData)

    Public Sub New()
        CustomData = New List(Of MyCustomData)()
    End Sub

    Public Overrides Function ExecutePostProcessingAsync() As Task
        For Each file In Contents
            Dim parameters = file.Headers.ContentDisposition.Parameters
            Dim data As New MyCustomData
            Dim pName As String = parameters(0).Name
            data.add(GetNameHeaderValue(parameters, pName), GetNameHeaderValue(parameters, pName))
            'Dim data = New MyCustomData With {
            '    .Foo = Integer.Parse(GetNameHeaderValue(parameters, "Foo")),
            '    .Bar = GetNameHeaderValue(parameters, "Bar")
            '}
            CustomData.Add(data)
        Next

        Return MyBase.ExecutePostProcessingAsync()
    End Function

    Private Shared Function GetNameHeaderValue(ByVal headerValues As ICollection(Of NameValueHeaderValue), ByVal name As String) As String
        Dim nameValueHeader = headerValues.FirstOrDefault(Function(x) x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        Return If(nameValueHeader IsNot Nothing, nameValueHeader.Value, Nothing)
    End Function
End Class
