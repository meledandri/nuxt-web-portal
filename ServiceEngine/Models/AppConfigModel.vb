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
    <Column(Order:=2)>
    Public Property Value As String


End Class

Public Class Companies
    <Key>
    <Column(Order:=0)>
    Public Property companyID As Integer = 0
    <Required>
    <Column(Order:=1)>
    <StringLength(50)>
    Public Property BusinessName As String = ""

    <Column(Order:=2)>
    Public Property isHidden As Boolean = False

    <Column(Order:=3)>
    Public Property insertDate As Date = Now



End Class



Public Class Users

    '    Id
    'DisplayName
    'PasswordHash
    'TwoFactorEnabled
    'LockoutEndDateUtc
    'LockoutEnabled
    'AccessFailedCount
    'UserName
    'Disabled
    'lastAccess
    'LastPasswordChangedDate
    'PasswordMustChange




    <Key>
    <Column(Order:=0)>
    Public Property userID As String = System.Guid.NewGuid.ToString()
    <Column(Order:=1)>
    <StringLength(50)>
    Public Property UserName As String = ""
    <Column(Order:=2)>
    <StringLength(20)>
    Public Property DisplayName As String = ""

    <Column(Order:=3)>
    <StringLength(20)>
    Public Property password As String = ""
    <Column(Order:=4)>
    <StringLength(50)>
    Public Property email As String = ""
    <Required>
    <Column(Order:=5)>
    Public Property companyID As Integer = -1

    <Column(Order:=6)>
    Public Property TwoFactorEnabled As Boolean = False

    <Column(Order:=7)>
    Public Property LockoutEndDateUtc As Nullable(Of Date) = Nothing
    <Column(Order:=8)>
    Public Property LockoutEnabled As Boolean = False

    <Column(Order:=9)>
    Public Property AccessFailedCount As Integer = 0


    <Column(Order:=10)>
    Public Property isHidden As Boolean = False

    <Column(Order:=11)>
    Public Property insertDate As Date = Now
    <Column(Order:=12)>
    Public Property lastAccess As Nullable(Of Date) = Nothing

    <Column(Order:=13)>
    Public Property LastPasswordChangedDate As Nullable(Of Date) = Nothing
    <Column(Order:=14)>
    Public Property PasswordMustChange As Boolean = False
    <Column(Order:=15)>
    Public Property Disabled As Boolean = False



End Class


Public Class AppMenu
    <Key>
    <Column(Order:=0)>
    Public Property menuID As Integer
    <Required>
    <Column(Order:=1)>
    <StringLength(20)>
    Public Property Name As String = ""
    <Required>
    <Column(Order:=2)>
    <StringLength(20)>
    Public Property Link As String = ""
    <Column(Order:=3)>
    Public Property parentID As Integer
    <Column(Order:=4)>
    Public Property icon As String = ""
    <Column(Order:=5)>
    Public Property Permissions As String = ""
    <Column(Order:=6)>
    Public Property flagVisible As Boolean = True
    <Column(Order:=7)>
    Public Property order As Integer = 0
    <Column(Order:=8)>
    <StringLength(5)>
    Public Property destination As String = "on"

End Class
