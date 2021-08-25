Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Timers
Imports Topshelf
Imports log4net
Imports System.Net
Imports System.IO
Module Module1
    Public Enum runningModes
        service = 0
        console = 1
    End Enum

    Public runningMode As runningModes = runningModes.service
    Private ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    'Private db As New ApplicationDbContext
    Public DefaultUrl As String = My.Settings.DefaultUrl
    Public AlternariveUrl As String = My.Settings.AlternariveUrl
    Public browser As Process
    Private db As New ApplicationDbContext

    Sub Main()
        If Environment.UserInteractive Then runningMode = runningModes.console
        log4net.Config.XmlConfigurator.Configure()
        log.Info("Start console mode: " & runningMode.Equals(runningModes.console))

        '    HostFactory.Run(
        'Sub(serviceConfig)
        '    serviceConfig.Service(Of SelfHostedServiceSignalR.ServiceClass)(
        '        Sub(serviceInstance)
        '            serviceConfig.UseNLog()
        '            serviceInstance.ConstructUsing(Function(name) New ServiceClass())
        '            serviceInstance.WhenStarted(Sub(tc) tc.StartService())
        '            serviceInstance.WhenStopped(Sub(tc) tc.StopService())
        '        End Sub)
        '    Dim delay As TimeSpan = New TimeSpan(0, 0, 0, 60)
        '    serviceConfig.EnableServiceRecovery(Sub(recoveryOption)
        '                                            recoveryOption.RestartService(delay)
        '                                            recoveryOption.RestartService(delay)
        '                                            recoveryOption.RestartComputer(delay, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & " computer reboot")
        '                                        End Sub)
        '    serviceConfig.RunAsLocalSystem()
        '    serviceConfig.SetDescription(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & " as Background service of relative web Application.")
        '    serviceConfig.SetDisplayName(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
        '    serviceConfig.SetServiceName(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
        'End Sub)

        'HostFactory.Run(Function(serviceConfig)
        '                    serviceConfig.Service(Of ServiceClass)(Function(serviceInstance)
        '                                                               serviceConfig.UseNLog()
        '                                                               serviceInstance.ConstructUsing(Function() New ServiceClass())
        '                                                               serviceInstance.WhenStarted(Function(execute) execute.StartService(Nothing))
        '                                                               serviceInstance.WhenStopped(Function(execute) execute.StopService())
        '                                                           End Function)
        '                    Dim delay As TimeSpan = New TimeSpan(0, 0, 0, 60)
        '                    serviceConfig.EnableServiceRecovery(Function(recoveryOption)
        '                                                            recoveryOption.RestartService(delay)
        '                                                            recoveryOption.RestartService(delay)
        '                                                            recoveryOption.RestartComputer(delay, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & " computer reboot")
        '                                                        End Function)
        '                    serviceConfig.SetServiceName(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
        '                    serviceConfig.SetDisplayName(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
        '                    serviceConfig.SetDescription(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & " as Background service of relative web Application.")
        '                    serviceConfig.StartAutomatically()
        '                End Function)
        Try
            Dim rc = HostFactory.Run(Sub(serviceConfig)                                   '1
                                         serviceConfig.Service(Of SelfHostedServiceSignalR.ServiceClass)(Sub(serviceInstance)
                                                                                                             serviceConfig.UseNLog() '2
                                                                                                             serviceInstance.ConstructUsing(Function(name) New SelfHostedServiceSignalR.ServiceClass())                '3
                                                                                                             serviceInstance.WhenStarted(Sub(tc) tc.StartService())                         '4
                                                                                                             serviceInstance.WhenStopped(Sub(tc) tc.StopService())                          '5
                                                                                                         End Sub)
                                         serviceConfig.RunAsLocalSystem()                                       '6
                                         serviceConfig.StartAutomatically()
                                         serviceConfig.UseLog4Net

                                         Dim delay As TimeSpan = New TimeSpan(0, 0, 0, 60)
                                         serviceConfig.EnableServiceRecovery(Sub(recoveryOption)
                                                                                 recoveryOption.RestartService(delay)
                                                                                 recoveryOption.RestartService(delay)
                                                                                 recoveryOption.RestartComputer(delay, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & " computer reboot")
                                                                             End Sub)

                                         serviceConfig.SetDescription(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & " as Background service of relative web Application.")                   '7
                                         serviceConfig.SetDisplayName(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)                                  '8
                                         serviceConfig.SetServiceName(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)                                  '9
                                     End Sub)                                                             '10
            Dim exitCode = CInt(Convert.ChangeType(rc, rc.GetTypeCode()))  '11
            Environment.ExitCode = exitCode
            If Not IsNothing(browser) Then
                browser.Kill()
            End If
        Catch ex As Exception
            log.Fatal(ex.Message)
        End Try



    End Sub

    Public Class TownCrier
        Private ReadOnly _timer As Timer

        Public Sub New()
            _timer = New Timer(1000) With {
                .AutoReset = True
            }
            '_timer.Elapsed += Sub(sender, eventArgs) Console.WriteLine("It is {0} and all is well", Date.Now)
            AddHandler _timer.Elapsed, AddressOf OnTimedEvent
        End Sub

        Public Sub Start()
            _timer.Start()
        End Sub

        Public Sub [Stop]()
            _timer.[Stop]()
        End Sub

        Private Sub OnTimedEvent(source As Object, e As System.Timers.ElapsedEventArgs)
            Console.WriteLine("It is {0} and all is well", Date.Now)
        End Sub

    End Class




    Public Function getTasksInfoList(companyID As Integer) As List(Of TaskInfoDataBindig)
        Dim list As List(Of TaskInfoDataBindig) = (From p In db.Products
                                                   Join cp In db.Companies On p.companyID Equals cp.companyID
                                                   Join ed In db.Editions On ed.productID Equals p.productID
                                                   Join act In db.mdActivity On ed.mdActivityID Equals act.mdActivityID
                                                   Join cls In db.mdClass On p.mdClassID Equals cls.mdClassID
                                                   Join tsk In db.mdTasks On ed.editionID Equals tsk.editionID
                                                   Join tsks In db.mdTasksStates On tsk.mdTasksStatesID Equals tsks.mdTasksStatesID
                                                   Join str In db.Structures On ed.StructureID Equals str.structureID
                                                   Join u In db.Users On u.userID Equals tsk.ownerID
                                                   Where cp.companyID = companyID
                                                   Select New TaskInfoDataBindig _
                                                           With {.companyID = cp.companyID,
                                                           .BusinessName = cp.BusinessName,
                                                           .productID = p.productID,
                                                           .productName = p.productName,
                                                           .mdClassID = cls.mdClassID,
                                                           .mdClassName = cls.mdClassName,
                                                           .editionID = ed.editionID,
                                                           .editionName = ed.editionName,
                                                           .certificationPlan = ed.certificationPlan,
                                                           .mdActivityID = act.mdActivityID,
                                                           .mdActivityName = act.mdActivityName,
                                                           .editionNotes = ed.editionNotes,
                                                            .deadline = ed.deadline,
                                                           .StructureID = str.structureID,
                                                           .structureName = str.structureName,
                                                       .mdTaskID = tsk.mdTaskID,
                                                       .mdTaskStatesID = tsk.mdTasksStatesID,
                                                       .mdTaskStatesName = tsks.mdTasksStatesName,
                                                           .insertDate = tsk.insertDate,
                                                       .modDate = tsk.ModDate,
                                                       .ownerID = tsk.ownerID,
                                                       .UserName = u.UserName,
                                                       .DisplayName = u.DisplayName,
                                                       .email = u.email
                                                           }).ToList
        Return list
    End Function
End Module
