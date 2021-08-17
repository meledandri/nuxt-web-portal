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

Public Class Companies
    <Key>
    Public Property companyID As Integer = 0
    Public Property BusinessName As String = ""

    Public Property isHidden As Boolean = False

    Public Property insertDate As Date = New Date()



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
    Public Property userID As String = System.Guid.NewGuid.ToString()
    Public Property UserName As String = ""
    Public Property DisplayName As String = ""

    Public Property password As String = ""
    Public Property email As String = ""
    <Required>
    Public Property companyID As Integer = -1

    Public Property TwoFactorEnabled As Boolean = False

    Public Property LockoutEndDateUtc As Nullable(Of Date) = Nothing
    Public Property LockoutEnabled As Boolean = False

    Public Property AccessFailedCount As Integer = 0


    Public Property isHidden As Boolean = False

    Public Property insertDate As Date = New Date()
    Public Property lastAccess As Nullable(Of Date) = Nothing

    Public Property LastPasswordChangedDate As Nullable(Of Date) = Nothing
    Public Property PasswordMustChange As Boolean = False
    Public Property Disabled As Boolean = False



End Class
