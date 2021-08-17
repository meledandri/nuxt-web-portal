Imports System.Data.Entity

Partial Public Class ApplicationDbContext
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=DefaultConnection")
    End Sub

    Public Property AppConfig As System.Data.Entity.DbSet(Of AppConfig)
    Public Property Companies As System.Data.Entity.DbSet(Of Companies)
    Public Property Users As System.Data.Entity.DbSet(Of Users)
    'Public Property Contacts As System.Data.Entity.DbSet(Of Contacts)

End Class