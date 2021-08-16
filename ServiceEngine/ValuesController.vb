Imports System.Collections.Generic
Imports System.Web.Http

Namespace OwinSelfhost
    Public Class ValuesController
        Inherits ApiController

        Public Function [Get]() As IEnumerable(Of String)
            Return New String() {"value1", "value2"}
        End Function

        Public Function [Get](ByVal id As Integer) As String
            Return "value"
        End Function

        Public Sub Post(
<FromBody> ByVal value As String)
        End Sub

        Public Sub Put(ByVal id As Integer,
<FromBody> ByVal value As String)
        End Sub

        Public Sub Delete(ByVal id As Integer)
        End Sub
    End Class
End Namespace