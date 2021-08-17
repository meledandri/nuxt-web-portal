Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class LoginBindingModel
    <Required>
    Public Property UserName As String = ""
    <Required>
    Public Property password As String = ""

End Class


Public Class UsersTokens
    Public Property userID As String = ""
    Public Property token As String = (New Guid).ToString
    Public Property insertDate As Date = New Date()
    Public Property deadline As Nullable(Of Date) = Nothing

End Class
