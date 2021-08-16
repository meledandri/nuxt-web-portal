Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class AppConfig
    <Key>
    <Column(Order:=0)>
    <StringLength(20)>
    Public Property App As String

    <Required>
    <Key>
    <Column(Order:=1)>
    <StringLength(20)>
    Public Property Parameter As String

    <Required>
    <MaxLength>
    Public Property Value As String


End Class
