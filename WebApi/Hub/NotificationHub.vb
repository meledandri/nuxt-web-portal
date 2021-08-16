Imports System.Threading.Tasks
Imports Microsoft.AspNet.SignalR

Public Class NotificationHub
    ' Inherits Hub
    Public Sub ReceiveMessage(user As String, Message As String)
        'Me.Clients.All.AcceptMessage(user, Message)
    End Sub


    Public Async Function SendMessage(ByVal user As String, ByVal message As String) As Task
        'Await Clients.All.SendAsync("ReceiveMessage", user, message)
    End Function
End Class
