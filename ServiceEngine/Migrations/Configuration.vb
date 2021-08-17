Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Migrations
Imports System.Data.Entity.Migrations.Infrastructure
Imports System.Linq

Namespace Migrations

    Friend NotInheritable Class Configuration 
        Inherits DbMigrationsConfiguration(Of ApplicationDbContext)

        Public Sub New()
            AutomaticMigrationsEnabled = True
        End Sub

        Protected Overrides Sub Seed(context As ApplicationDbContext)
            '  This method will be called after migrating to the latest version.

            If My.Settings.MigrateDatabaseToLatestVersion Then
                Dim configuration As Configuration = New Configuration()
                configuration.ContextType = GetType(ApplicationDbContext)
                Dim migrator = New DbMigrator(configuration)
                Dim scriptor = New MigratorScriptingDecorator(migrator)
                Dim script As String = scriptor.ScriptUpdate(sourceMigration:=Nothing, targetMigration:=Nothing).ToString()
                Debug.Write(script)
                migrator.Update()

            End If
            '  You can use the DbSet(Of T).AddOrUpdate() helper extension method 
            '  to avoid creating duplicate seed data.
            Dim db As New ApplicationDbContext

            Dim ci As Integer = (From c In db.Companies Select c).Count
            If ci = 0 Then
                Dim cn As New Companies
                cn.BusinessName = "Gruppo SASI"
                cn.isHidden = False

                db.Companies.Add(cn)



            End If
        End Sub

    End Class

End Namespace
