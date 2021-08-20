Imports System.Data.Entity
Imports System.Data.Entity.ModelConfiguration.Conventions

Partial Public Class ApplicationDbContext
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=DefaultConnection")
    End Sub

    'Tabelle applicative
    Public Property AppConfig As System.Data.Entity.DbSet(Of AppConfig)
    Public Property AppMenu As System.Data.Entity.DbSet(Of AppMenu)

    'Tabelle per la gestione accessi
    Public Property Companies As System.Data.Entity.DbSet(Of Companies)
    Public Property Users As System.Data.Entity.DbSet(Of Users)
    Public Property UsersTokens As System.Data.Entity.DbSet(Of UsersTokens)


    'TechFile Online - Tabelle
    Public Property Products As System.Data.Entity.DbSet(Of Products)
    Public Property Editions As System.Data.Entity.DbSet(Of Editions)
    Public Property Structures As System.Data.Entity.DbSet(Of Structures)
    Public Property StructureDetails As System.Data.Entity.DbSet(Of StructureDetails)
    Public Property Details As System.Data.Entity.DbSet(Of Details)
    Public Property ActivityLog As System.Data.Entity.DbSet(Of ActivityLog)
    Public Property CompanyDetail As System.Data.Entity.DbSet(Of CompanyDetail)
    Public Property CompanyRoles As System.Data.Entity.DbSet(Of CompanyRoles)
    Public Property mdClass As System.Data.Entity.DbSet(Of mdClass)
    Public Property mdActivity As System.Data.Entity.DbSet(Of mdActivity)








    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        modelBuilder.Conventions.Remove(Of PluralizingTableNameConvention)()
    End Sub
End Class