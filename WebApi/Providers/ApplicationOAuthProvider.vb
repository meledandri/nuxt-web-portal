Imports System.Data.Entity
Imports System.Security.Claims
Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.OAuth

Public Class ApplicationOAuthProvider
    Inherits OAuthAuthorizationServerProvider
    Private ReadOnly _publicClientId As String

    Public Sub New(publicClientId As String)
        If publicClientId Is Nothing Then
            Throw New ArgumentNullException("publicClientId")
        End If

        _publicClientId = publicClientId
    End Sub

    Public Overrides Async Function GrantResourceOwnerCredentials(context As OAuthGrantResourceOwnerCredentialsContext) As Task
        'context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", {"*"})
        HttpContext.Current.Request.Headers.Add("Access-Control-Allow-Origin", "*")
        HttpContext.Current.Request.Headers.Add("Access-Control-Allow-Methods", "*")
        HttpContext.Current.Request.Headers.Add("Access-Control-Allow-Headers", "*")
        Dim userManager As ApplicationUserManager = context.OwinContext.GetUserManager(Of ApplicationUserManager)()
        Dim user As ApplicationUser = Await userManager.FindAsync(context.UserName, context.Password)
        Dim max As Integer = IIf(IsNothing(AppParameter("AccessFailedMaxCount", "webui")), 5, AppParameter("AccessFailedMaxCount", "webui"))
        Dim addTime As Integer = IIf(IsNothing(AppParameter("AccessFailedBlockMinutes", "webui")), 5, AppParameter("AccessFailedBlockMinutes", "webui"))

        'Se non ho trovoato l'utente
        If user Is Nothing Then
            Dim u As ApplicationUser = userManager.FindByName(context.UserName)
            Dim sDate = New DateTime(Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, Now.Second).AddMinutes(addTime)
            If Not IsNothing(u) Then
                If u.LockoutEnabled Then
                    With u
                        .AccessFailedCount += 1
                        .LockoutEndDateUtc = sDate

                    End With
                    userManager.Update(u)
                End If
            End If

            context.SetError("invalid_grant", My.Resources.PasswordONomeUtenteErrato)
            Return
        Else
            'Se utente e password sono corrette verifico prima che l'utente non sia bloccato
            If user.Disabled Then
                context.SetError("invalid_grant", WebApi.My.Resources.ApplicationOAuthProvider_GrantResourceOwnerCredentials_UtenteDisabilitato)
                Return
            ElseIf user.LockoutEnabled Then
                If user.AccessFailedCount >= max Then
                    If DateDiff(DateInterval.Minute, CDate(user.LockoutEndDateUtc), Date.Now) <= addTime Then
                        context.SetError("invalid_grant", WebApi.My.Resources.ApplicationOAuthProvider_GrantResourceOwnerCredentials_UtenteBloccato)
                        Return
                    End If
                End If
            End If
        End If


        Dim sessionID As String = System.Guid.NewGuid.ToString

        Dim oAuthIdentity As ClaimsIdentity = Await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType)
        Dim cookiesIdentity As ClaimsIdentity = Await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType)
        oAuthIdentity.AddClaim(New Claim("session_id", sessionID))
        Dim properties As AuthenticationProperties = CreateProperties(user.UserName, sessionID)
        Dim ticket As New AuthenticationTicket(oAuthIdentity, properties)

        context.Validated(ticket)


        context.Request.Context.Authentication.SignIn(cookiesIdentity)

        'Using db As New applicationDBEntities
        'Dim usr As ApplicationUser = (From q In userManager.Users Where q.UserName = context.UserName Select q).FirstOrDefault()
        'usr.sessionID = sessionID

        'mdb.SqlString = "update AspNetUsers set sessionID = '" & sessionID & "' WHERE UserName = '" & context.UserName & "'"
        'mdb.AggiornaCampi()
        'Dim usr As AspNetUsers = (From u In db.AspNetUsers Where u.Id = user.Id).FirstOrDefault
        'If Not IsNothing(usr) Then
        '    Dim Now = DateTime.Now
        '    Dim myDate = New DateTime(Now.Year, Now.Month, Now.Day,
        '            Now.Hour, Now.Minute, Now.Second)
        '    With usr
        '        .sessionID = sessionID
        '        .lastAccess = myDate
        '        .sessionDate = myDate
        '    End With
        '    db.AspNetUsers.Attach(usr)
        '    db.Entry(usr).State = EntityState.Modified
        '    Dim x = db.SaveChanges()

        '    Debug.WriteLine(x)

        'End If
        With user
            .sessionID = sessionID
            .lastAccess = Date.Now
            .sessionDate = Date.Now
            .AccessFailedCount = 0
        End With
        userManager.Update(user)

        'End Using

    End Function

    Public Overrides Function TokenEndpoint(context As OAuthTokenEndpointContext) As Task
        For Each [property] As KeyValuePair(Of String, String) In context.Properties.Dictionary
            context.AdditionalResponseParameters.Add([property].Key, [property].Value)
        Next

        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Overrides Function ValidateClientAuthentication(context As OAuthValidateClientAuthenticationContext) As Task
        ' ID client non specificato nelle credenziali del proprietario della risorsa.
        If context.ClientId Is Nothing Then
            context.Validated()
        End If

        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Overrides Function ValidateClientRedirectUri(context As OAuthValidateClientRedirectUriContext) As Task
        If context.ClientId = _publicClientId Then
            Dim expectedRootUri As New Uri(context.Request.Uri, "/")

            If expectedRootUri.AbsoluteUri = context.RedirectUri Then
                context.Validated()
            End If
        End If

        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Shared Function CreateProperties(userName As String, ByVal Optional session_id As String = "") As AuthenticationProperties
        Dim data As IDictionary(Of String, String) = New Dictionary(Of String, String)() From {
            {"userName", userName},
            {"session_id", session_id}
        }
        Return New AuthenticationProperties(data)
    End Function
End Class
