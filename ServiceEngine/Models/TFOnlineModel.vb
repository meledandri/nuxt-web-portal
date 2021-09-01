Imports System.ComponentModel.DataAnnotations
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
    Public Property mdClassID As mdClass_enum = mdClass_enum.none
    <Required>
    <StringLength(10)>
    <Column(Order:=4)>
    Public Property mdCode As String = ""


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
    Public Property mdActivityID As mdActivity_enum = mdActivity_enum.none

    <Required>
    <Column(Order:=5)>
    Public Property mdTasksStatesID As mdTaskStates_enum = mdTaskStates_enum.created

    <MaxLength>
    <Column(Order:=6)>
    Public Property editionNotes As String = ""

    <Required>
    <Column(Order:=7)>
    Public Property deadline As Nullable(Of Date)

    <Required>
    <Column(Order:=8)>
    Public Property StructureID As Integer

    <Required>
    <Column(Order:=9)>
    Public Property fileStatus As Integer
    <Required>
    <Column(Order:=10)>
    Public Property productInfoStatus As Integer
    <Required>
    <Column(Order:=11)>
    Public Property checkListStatus As Integer

    <Required>
    <Column(Order:=12)>
    Public Property insertDate As Date = Now

    <Required>
    <Column(Order:=13)>
    Public Property ownerID As String = ""

    <Required>
    <Column(Order:=14)>
    Public Property modifiedDate As Date = Now


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


    <Required>
    <MaxLength>
    <Column(Order:=2)>
    Public Property Title As String = ""

    <Required>
    <Column(Order:=3)>
    Public Property idParent As Integer

    <Required>
    <Column(Order:=4)>
    Public Property documentID As Integer

    <Required>
    <StringLength(250)>
    <Column(Order:=5)>
    Public Property fileName As String = ""

    <Required>
    <Column(Order:=6)>
    Public Property addFolder As Integer = 0

    <Required>
    <Column(Order:=7)>
    Public Property addFile As Integer = 0

    <Required>
    <Column(Order:=8)>
    Public Property nLevels As Integer = 0


    <Required>
    <Column(Order:=9)>
    Public Property idVerDoc As Integer = 0


    <Required>
    <Column(Order:=10)>
    Public Property flagState As Integer = 0    'o = da caricare, 2 = completato

    <StringLength(4)>
    <Column(Order:=11)>
    Public Property fileExtension As String = ""

    <Required>
    <Column(Order:=12)>
    Public Property operatorID As Integer = 0

    <MaxLength>
    <Column(Order:=13)>
    Public Property MD5 As String = ""

    <Required>
    <Column(Order:=14)>
    Public Property swTarget As Integer = 0

    <MaxLength>
    <Column(Order:=15)>
    Public Property file_for_checklist As String = ""

    <MaxLength>
    <Column(Order:=16)>
    Public Property fullPath As String = ""

    <Required>
    <Column(Order:=17)>
    Public Property flagContainer As Integer = 0

End Class


''' <summary>
''' Tabella con i dettagli della struttura e dei file che compongono l'edizione
''' </summary>
Public Class Details
    <Key>
    <Column(Order:=0)>
    Public Property detailID As Integer

    <Required>
    <Column(Order:=1)>
    Public Property productID As Integer
    '<Required>
    '<Column(Order:=0)>
    'Public Property structureID As Integer

    <Required>
    <Column(Order:=2)>
    Public Property editionID As Integer


    <Required>
    <Column(Order:=3)>
    Public Property structureID As Integer


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
    <StringLength(250)>
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

    <StringLength(20)>
    <Column(Order:=13)>
    Public Property fileExtension As String = ""

    <Column(Order:=14)>
    Public Property operatorID As String = ""

    <MaxLength>
    <Column(Order:=15)>
    Public Property MD5 As String = ""

    <Required>
    <Column(Order:=16)>
    Public Property swTarget As Integer = 0

    <MaxLength>
    <Column(Order:=17)>
    Public Property file_for_checklist As String = ""

    <MaxLength>
    <Column(Order:=18)>
    Public Property fullPath As String = ""

    <Required>
    <Column(Order:=19)>
    Public Property flagContainer As Integer = 0

End Class


Public Class DetailsModel
    '            Select Case Details.detailID,
    '            Structures.structureID,
    '            Structures.structureName,
    '            Editions.editionID,
    '            Editions.editionName,
    '            Editions.certificationPlan,
    '            Products.productID,
    '            Products.productName,
    '            Companies.companyID, 
    '            Companies.BusinessName,
    '            mdClass.mdClassID,
    '            mdClass.mdClassName,
    '            Products.mdCode,
    '            Details.Title,
    '            Details.idParent,
    '            Details.documentID,
    '            Details.fileName,
    '            Details.addFolder,
    '            Details.addFile,
    '            Details.nLevels,
    '            Details.idVerDoc,
    '            Details.flagState,
    '            Details.fileExtension,
    '            Details.operatorID,
    '            Details.MD5,
    '            Details.swTarget,
    '            Details.file_for_checklist,
    '            Details.fullPath,
    '            Details.flagContainer

    <Key>
    Public Property detailID As Integer

    <Required>
    Public Property productID As Integer

    <MaxLength>
    Public Property productName As String = ""


    <Required>
    Public Property editionID As Integer

    <MaxLength>
    Public Property editionName As String = ""

    <StringLength(20)>
    Public Property certificationPlan As String = ""


    <Required>
    Public Property structureID As Integer

    <MaxLength>
    Public Property structureName As String = ""

    Public Property companyID As Integer

    <StringLength(50)>
    Public Property BusinessName As String = ""

    Public Property mdClassID As mdClass_enum = mdClass_enum.none

    <StringLength(50)>
    Public Property mdClassName As String = ""

    <StringLength(10)>
    Public Property mdCode As String = ""


    <Required>
    <MaxLength>
    Public Property Title As String = ""

    <Required>
    Public Property idParent As Integer

    <Required>
    Public Property documentID As Integer

    <Required>
    <StringLength(250)>
    Public Property fileName As String = ""

    <Required>
    Public Property addFolder As Integer = 0

    <Required>
    Public Property addFile As Integer = 0

    <Required>
    Public Property nLevels As Integer = 0


    <Required>
    Public Property idVerDoc As Integer = 0


    <Required>
    Public Property flagState As Integer = 0    'o = da caricare, 2 = completato

    <StringLength(20)>
    Public Property fileExtension As String = ""

    Public Property operatorID As String = ""

    <MaxLength>
    Public Property MD5 As String = ""

    <Required>
    Public Property swTarget As Integer = 0

    <MaxLength>
    Public Property file_for_checklist As String = ""

    <MaxLength>
    Public Property fullPath As String = ""

    <Required>
    Public Property flagContainer As Integer = 0

    <Required>
    Public Property fileStatus As Integer
    <Required>
    Public Property productInfoStatus As Integer
    <Required>
    Public Property checkListStatus As Integer


End Class


Public Class DetailsTreeModel
    Inherits DetailsModel
    Public Property children As New List(Of DetailsTreeModel)
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
    Public Property mdTasksStatesID As mdTaskStates_enum = mdTaskStates_enum.created

    <Required>
    <Column(Order:=4)>
    Public Property userID As String = ""

    <Required>
    <Column(Order:=5)>
    Public Property resultID As Integer = 0

    <Required>
    <Column(Order:=6)>
    <MaxLength>
    Public Property resultMessage As String = ""

    <Column(Order:=7)>
    Public Property startActiviyDate As Nullable(Of Date)
    <Column(Order:=8)>
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
    <Key>
    <DatabaseGenerated(DatabaseGeneratedOption.None)>
    <Column(Order:=0)>
    Public Property mdClassID As Integer
    <StringLength(50)>
    <Required>
    <Column(Order:=1)>
    Public Property mdClassName As String = ""
End Class

Public Enum mdClass_enum
    none = 0
    Class_I = 10
    Class_II_and_IIb = 20
    Class_III = 30
End Enum


''' <summary>
''' Tabella che raccoglie tutte le tipologie di attività che possono essere eseguite sui TF
''' </summary>
Public Class mdActivity
    <Key>
    <DatabaseGenerated(DatabaseGeneratedOption.None)>
    <Column(Order:=0)>
    Public Property mdActivityID As Integer
    <StringLength(50)>
    <Required>
    <Column(Order:=1)>
    Public Property mdActivityName As String = ""
End Class

Public Enum mdActivity_enum
    none = 0
    product_registration = 10

End Enum

''' <summary>
''' Tabella che contiene gli stati del processo di acquisizione e valutazione
''' </summary>
Public Class mdTasksStates
    <Key>
    <DatabaseGenerated(DatabaseGeneratedOption.None)>
    <Column(Order:=0)>
    Public Property mdTasksStatesID As Integer
    <Required>
    <StringLength(50)>
    <Column(Order:=1)>
    Public Property mdTasksStatesName As String = ""

End Class



''' <summary>
''' Valori della tabella mdTasksStates Enumerati
''' </summary>
Public Enum mdTaskStates_enum
    created = 0
    uploading_process = 10
    uploading_process_error = 11
    ready_for_evaluation = 20
    preliminary_evaluation_process = 30
    preliminary_evaluation_rejected = 31
    evaluation_process = 40
    evaluation_rejected = 41
End Enum



'''' <summary>
'''' Tabella che elenca le attività di caricamento e valutazione svolte su una specifica Edizione
'''' </summary>
'Public Class mdTasks
'    <Key>
'    <Column(Order:=0)>
'    Public Property mdTaskID As Integer

'    <Required>
'    <Column(Order:=1)>
'    Public Property editionID As Integer

'    <Required>
'    <Column(Order:=2)>
'    Public Property mdTasksStatesID As Integer

'    <Required>
'    <Column(Order:=3)>
'    Public Property insertDate As Date = Now


'    <Required>
'    <Column(Order:=4)>
'    Public Property ownerID As String

'    <Required>
'    <Column(Order:=5)>
'    Public Property ModDate As Date = Now


'End Class




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
    Public Property tasks As List(Of TaskInfoDataBindig) = New List(Of TaskInfoDataBindig)

    Public number_of_users As Integer = 0
    Public number_of_tasks As Integer = 0


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


Public Class ProductInfoDataBinding
    <Required>
    Public Property companyID As Integer = 0
    <Required>
    <StringLength(50)>
    Public Property BusinessName As String = ""
    <Required>
    Public Property productID As Integer
    <Required>
    <MaxLength>
    Public Property productName As String = ""

    <Required>
    Public Property mdClassID As mdClass_enum = mdClass_enum.none
    <Required>
    <StringLength(50)>
    Public Property mdClassName As String = ""

    <Required>
    Public Property editionID As Integer
    <Required>
    <MaxLength>
    Public Property editionName As String = ""

    <StringLength(20)>
    Public Property certificationPlan As String = ""

    <Required>
    Public Property mdActivityID As mdActivity_enum = mdActivity_enum.none
    <Required>
    <StringLength(50)>
    Public Property mdActivityName As String = ""

    <MaxLength>
    Public Property editionNotes As String = ""

    Public Property deadline As Nullable(Of Date)

    <Required>
    Public Property StructureID As Integer
    <Required>
    <MaxLength>
    Public Property structureName As String = ""

    <Required>
    Public Property insertDate As Date = Now

End Class


Public Class TaskInfoDataBindig
    <Required>
    Public Property companyID As Integer = 0
    <Required>
    <StringLength(50)>
    Public Property BusinessName As String = ""
    <Required>
    Public Property productID As Integer
    <Required>
    <MaxLength>
    Public Property productName As String = ""

    <Required>
    Public Property mdClassID As mdClass_enum = mdClass_enum.none
    <Required>
    <StringLength(50)>
    Public Property mdClassName As String = ""

    <Required>
    Public Property editionID As Integer
    <Required>
    <MaxLength>
    Public Property editionName As String = ""

    <StringLength(20)>
    Public Property certificationPlan As String = ""

    <Required>
    Public Property mdActivityID As mdActivity_enum = mdActivity_enum.none
    <Required>
    <StringLength(50)>
    Public Property mdActivityName As String = ""

    <MaxLength>
    Public Property editionNotes As String = ""

    Public Property deadline As Nullable(Of Date)

    <Required>
    Public Property StructureID As Integer
    <Required>
    <MaxLength>
    Public Property structureName As String = ""

    '<Required>
    'Public Property mdTaskID As Integer
    <Required>
    Public Property mdTaskStatesID As Integer
    <Required>
    <MaxLength>
    Public Property mdTaskStatesName As String = ""

    <Required>
    Public Property insertDate As Date = Now

    <Required>
    Public Property modifiedDate As Date = Now

    Public Property ownerID As String = ""
    <StringLength(50)>
    Public Property UserName As String = ""

    <StringLength(20)>
    Public Property DisplayName As String = ""

    <StringLength(50)>
    Public Property email As String = ""

    <Required>
    Public Property fileStatus As Integer
    <Required>
    Public Property productInfoStatus As Integer
    <Required>
    Public Property checkListStatus As Integer


End Class