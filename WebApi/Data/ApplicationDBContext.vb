Imports System.Data.Entity

'Namespace WeightTrackerOkta.Data
'    Public Class ApplicationDbContext
'        Inherits DbContext

'        Public Sub New()
'            MyBase.New("OktaConnectionString")
'        End Sub

'        Public Shared Function Create() As ApplicationDbContext
'            Return New ApplicationDbContext()
'        End Function

'        Public Property WeightMeasurements As DbSet(Of WeightMeasurement)
'    End Class
'End Namespace





Namespace Data

    Public Class ApplicationDbContext
        Inherits DbContext
        Public Sub New()
            MyBase.New("DefaultConnection")
        End Sub

        Public Shared Function Create() As ApplicationDbContext
            Return New ApplicationDbContext()
        End Function

        Public Overridable Property AspNetUsers As DbSet(Of AspNetUsers)
        Public Overridable Property AspNetRoles As DbSet(Of AspNetRoles)
        Public Overridable Property AspNetUserLogins As DbSet(Of AspNetUserLogins)
        Public Overridable Property AspNetUserClaims As DbSet(Of AspNetUserClaims)
        Public Overridable Property AppConfigs As DbSet(Of AppConfigs)
    End Class

End Namespace
