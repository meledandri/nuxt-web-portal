Imports Microsoft.Owin.Hosting

Imports System
Imports Microsoft.Owin
Imports Topshelf.Logging

Imports System.Net.Http

<Assembly: OwinStartup(GetType(SelfHostedServiceSignalR.Startup))>


Namespace SelfHostedServiceSignalR

    Public Class ServiceClass
        Public Shared ReadOnly Log As LogWriter = HostLogger.[Get](Of ServiceClass)()

        Public Sub StartService()
            WriteToEventLog("Service started")
            Log.InfoFormat("SignalRServiceChat: In OnStart")

            If runningMode = runningModes.console Then Console.WriteLine(String.Format("running on: {0}", DefaultUrl))
            If runningMode = runningModes.console Then Console.WriteLine(String.Format("running on: {0}", AlternariveUrl))
            Dim options As StartOptions = New StartOptions()
            options.Urls.Add(DefaultUrl)
            options.Urls.Add(AlternariveUrl)

            Try
                WebApp.Start(options)
                'browser = Process.Start(AlternariveUrl)
                browser = Process.Start("http://localhost:3000/")


            Catch ex As Exception
                Log.Fatal("StartService\WebApp.Start: " & ex.Message)
                If runningMode = runningModes.console Then Console.WriteLine(String.Format("##ERROR## - StartService\WebApp.Start: {0}", ex.Message))
                Me.StopService()
            End Try

            'Using WebApp.Start(Of Startup)(url:=baseAddress)
            '    Dim client As HttpClient = New HttpClient()
            '    Dim response = client.GetAsync(baseAddress & "/api/values").Result
            '    Console.WriteLine(response)
            '    Console.WriteLine(response.Content.ReadAsStringAsync().Result)
            '    Console.ReadLine()
            'End Using
        End Sub




        Public Sub StopService()
            WriteToEventLog("Service stopped")
            Log.InfoFormat("SignalRServiceChat: In OnStop")
        End Sub

        Private Sub WriteToEventLog(ByVal Message As String)
            Dim el As New EventLog("Application")
            el.Source = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            Try
                el.WriteEntry(Message, EventLogEntryType.Information)
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        End Sub

    End Class

    Partial Public Class SignalRServiceChat
        Implements IDisposable

        Public Shared ReadOnly Log As LogWriter = HostLogger.[Get](Of SignalRServiceChat)()

        Public Sub New()
        End Sub

        Public Sub OnStart(ByVal args As String())
            Log.InfoFormat("SignalRServiceChat: In OnStart")
            Dim url As String = "http://localhost:8091"
            WebApp.Start(url)
        End Sub

        Public Sub OnStop()
            Log.InfoFormat("SignalRServiceChat: In OnStop")
        End Sub

        Public Sub Dispose()
        End Sub

        Private Sub IDisposable_Dispose() Implements IDisposable.Dispose
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace


