﻿Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

''' <summary>
''' Tabella per l'Anagrafica dei Prodotti
''' </summary>
Public Class Products
    <Key>
    <Column(Order:=0)>
    Public Property productID As Integer

    <Required>
    <Column(Order:=1)>
    Public Property companyID As Integer

    <Required>
    <MaxLength>
    <Column(Order:=2)>
    Public Property productName As String = ""

    <Required>
    <Column(Order:=3)>
    Public Property mdClassID As Integer


End Class



''' <summary>
''' Tabella per l'elenco delle edizioni per ogni prodotto
''' </summary>
Public Class Editions
    <Key>
    <Column(Order:=0)>
    Public Property editionID As Integer

    <Column(Order:=1)>
    Public Property productID As Integer

    <Required>
    <MaxLength>
    <Column(Order:=2)>
    Public Property editionName As String = ""

    <Required>
    <StringLength(20)>
    <Column(Order:=3)>
    Public Property certificationPlan As String = ""

    <Required>
    <Column(Order:=4)>
    Public Property activityID As Integer

    <MaxLength>
    <Column(Order:=5)>
    Public Property editionNotes As String = ""

    <Required>
    <Column(Order:=6)>
    Public Property deadline As Nullable(Of Date)

    <Required>
    <Column(Order:=7)>
    Public Property StructureID As Integer

    <Required>
    <Column(Order:=8)>
    Public Property insertDate As Date = Now


End Class


''' <summary>
''' Tabella che definisce l'elenco delle strutture dati Master o da duplicare
''' </summary>
Public Class Structures
    <Key>
    <Column(Order:=0)>
    Public Property structureID As Integer

    <Required>
    <MaxLength>
    <Column(Order:=1)>
    Public Property structureName As String = ""

    <Required>
    <Column(Order:=2)>
    Public Property isMaster As Boolean = False

End Class


''' <summary>
''' Tabella che definisce i dettagli delle strutture Master o duplicate (rif. tbl_foldertaqble_ectd/tbl_ctd)
''' </summary>
Public Class StructureDetails
    <Key>
    <Column(Order:=0)>
    Public Property structureDetailID As Integer

    <Required>
    <Column(Order:=1)>
    Public Property structureID As Integer

    <Column(Order:=2)>
    Public Property editionID As Integer


    <Column(Order:=3)>
    Public Property productID As Integer


    <MaxLength>
    <Column(Order:=4)>
    Public Property Title As String = ""

    <Column(Order:=5)>
    Public Property idParent As Integer

    <Column(Order:=6)>
    Public Property documentID As Integer


End Class


''' <summary>
''' Tabella con i dettagli della struttura e dei file che compongono l'edizione
''' </summary>
Public Class Details
    <Key>
    <Column(Order:=0)>
    Public Property detyailID As Integer

    <Required>
    <Column(Order:=1)>
    Public Property structureDetailID As Integer

    '<Required>
    '<Column(Order:=0)>
    'Public Property structureID As Integer

    <Required>
    <Column(Order:=2)>
    Public Property editionID As Integer


    <Required>
    <Column(Order:=3)>
    Public Property productID As Integer


    <Required>
    <MaxLength>
    <Column(Order:=4)>
    Public Property Title As String = ""

    <Required>
    <Column(Order:=5)>
    Public Property idParent As Integer

    <Required>
    <Column(Order:=6)>
    Public Property documentID As Integer

    <Required>
    <StringLength(20)>
    <Column(Order:=7)>
    Public Property fileName As String = ""

    <Required>
    <Column(Order:=8)>
    Public Property addFolder As Integer = 0

    <Required>
    <Column(Order:=9)>
    Public Property addFile As Integer = 0

    <Required>
    <Column(Order:=10)>
    Public Property nLevels As Integer = 0


    <Required>
    <Column(Order:=11)>
    Public Property idVerDoc As Integer = 0


    <Required>
    <Column(Order:=12)>
    Public Property flagState As Integer = 0    'o = da caricare, 2 = completato

    <Required>
    <StringLength(20)>
    <Column(Order:=13)>
    Public Property fileExtension As String = ""

    <Required>
    <Column(Order:=14)>
    Public Property operatorID As Integer = 0

    <Required>
    <MaxLength>
    <Column(Order:=15)>
    Public Property MD5 As String = ""

    <Required>
    <Column(Order:=16)>
    Public Property swTarget As Integer = 0

    <Required>
    <MaxLength>
    <Column(Order:=17)>
    Public Property file_for_checklist As String = ""

    <Required>
    <MaxLength>
    <Column(Order:=18)>
    Public Property fullPath As String = ""


End Class


''' <summary>
''' Tabella che riporta tutti i cambiamenti di stato dell’edizione (dalla creazione all’approvazione/rifiuto)
''' </summary>
Public Class ActivityLog
    <Key>
    <Column(Order:=0)>
    Public Property activityID As Long

    <Required>
    <Column(Order:=1)>
    Public Property editionID As Integer

    <Required>
    <Column(Order:=2)>
    Public Property insertDate As Date = Now

    <Required>
    <Column(Order:=3)>
    Public Property activityStatusID As Integer = 0

    <Required>
    <Column(Order:=4)>
    Public Property userID As String = ""

    <Column(Order:=5)>
    Public Property startActiviyDate As Nullable(Of Date)
    <Column(Order:=6)>
    Public Property stopActiviyDate As Nullable(Of Date)


End Class



''' <summary>
''' Tabella contenente i dettagli sull'Azienda Cliente
''' </summary>
Public Class CompanyDetail
    <Key>
    <Required>
    <Column(Order:=0)>
    Public Property CompanyDetailID As Integer = 0

    <Required>
    <Column(Order:=1)>
    Public Property companyID As Integer = 0

    <Required>
    <Column(Order:=2)>
    Public Property companyRoleID As Integer = 0
    <StringLength(20)>
    <Required>
    <Column(Order:=3)>
    Public Property SRN As String = ""

    <StringLength(20)>
    <Required>
    <Column(Order:=4)>
    Public Property country As String = ""

End Class


''' <summary>
''' Tabella che raccoglie tutte le tipologie di Clienti
''' </summary>
Public Class CompanyRoles
    <Required>
    <Key>
    <Column(Order:=0)>
    Public Property companyRoleID As Integer
    <StringLength(50)>
    <Required>
    <Column(Order:=1)>
    Public Property companyRoleName As String = ""
End Class

''' <summary>
''' Tabella che raccoglie tutte le tipologie di Classi dei MD
''' </summary>
Public Class mdClass
    <Required>
    <Key>
    <Column(Order:=0)>
    Public Property mdClassID As Integer
    <StringLength(50)>
    <Required>
    <Column(Order:=1)>
    Public Property mdClassName As String = ""
End Class




''' <summary>
''' Tabella che raccoglie tutte le tipologie di attività che possono essere eseguite sui TF
''' </summary>
Public Class mdActivity
    <Required>
    <Key>
    <Column(Order:=0)>
    Public Property mdActivityID As Integer
    <StringLength(50)>
    <Required>
    <Column(Order:=1)>
    Public Property mdActivityName As String = ""
End Class





Public Class FabListDataBinding
    <Required>
    Public Property companyID As Integer = 0
    <Required>
    Public Property BusinessName As String = ""

    Public Property companyRoleID As Nullable(Of Integer)

    <StringLength(20)>
    <Required>
    Public Property companyRoleName As String = ""

    <StringLength(20)>
    <Required>
    <Column(Order:=1)>
    Public Property SRN As String = ""

    <StringLength(20)>
    <Required>
    <Column(Order:=1)>
    Public Property country As String = ""

    Public Property details As New FabListDetailDataBinding

End Class

Public Class FabListDetailDataBinding
    Public Property users As List(Of UserInfo) = New List(Of UserInfo)

End Class

Public Class UserInfo
    <Required>
    Public Property userID As String = ""

    <Required>
    Public Property companyID As Integer = 0

    <Required>
    <StringLength(50)>
    Public Property UserName As String = ""

    <Required>
    <StringLength(20)>
    Public Property password As String = ""

    <Required>
    Public Property DisplayName As String = ""

    <StringLength(50)>
    Public Property email As String = ""

End Class


