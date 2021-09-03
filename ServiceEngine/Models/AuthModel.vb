Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class LoginBindingModel
    <Required>
    Public Property userName As String = ""
    <Required>
    Public Property password As String = ""

End Class


Public Class UsersTokens
    <Key>
    Public Property userID As String = ""
    Public Property token As String = System.Guid.NewGuid.ToString()
    Public Property insertDate As Date = Now
    Public Property deadline As Nullable(Of Date) = Nothing

End Class
