Imports System.Net
Friend Class Updater
    Private log As ILoggerHost
    Private newVer As String
    Private dwnl, dwnl_ok As Boolean
    Private wc As WebClient
    Private dwnl_start As Date

    Sub New(ByVal host As ILoggerHost)
        log = host
        wc = New WebClient()
        log.LS("Updater: begin cross-thread async dl of description...")
        Threading.ThreadPool.QueueUserWorkItem(AddressOf WebInitializer, "http://plrenewer.googlecode.com/svn/trunk/My%20Project/AssemblyInfo.vb")
    End Sub
    Private Sub WebInitializer(ByVal uri As Object)
        CtLog("Updater: thread launched", False)
        Dim e As PlrDownloadStringCompletedEventArgs = Nothing
        Try
            e = New PlrDownloadStringCompletedEventArgs(wc.DownloadString(CType(uri, String)), Nothing, False, Nothing)
        Catch ex As Exception
            e = New PlrDownloadStringCompletedEventArgs("", ex, True, Nothing)
        End Try
        CtLog("Updater: invokation...", True)
        If CType(log, Form).InvokeRequired Then
            CType(log, Form).Invoke(New StrindDownDelegate(AddressOf StringDown), New Object() {wc, e})
        Else
            StringDown(wc, e)
        End If
    End Sub
    Private Sub StringDown(ByVal sender As Object, ByVal e As PlrDownloadStringCompletedEventArgs)
        If e.Error IsNot Nothing Then
            log.LS("Updater: descr. async download failed: " & e.Error.Message)
        Else
            log.LS("Updater: async download finished")
            Try
                Dim lines() As String = e.Result.Split(New String() {Chr(13) & Chr(10)}, StringSplitOptions.RemoveEmptyEntries)
                For Each line As String In lines
                    If line.StartsWith("<Assembly: AssemblyF") Then
                        newVer = line.Substring(32, 7)
                        log.LS("Updater: new version is " & newVer)
                        Dim ver As New Version(newVer)
                        log.LS("Updater: my version is: " & My.Application.Info.Version.ToString)
                        If ver > My.Application.Info.Version Then
                            log.LX("Updater: creating subkey")
                            Dim k As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.CreateSubKey("Software\HACK-Design\PLReNewer\")
                            Dim s As String = k.GetValue("IgnoredVersion", "0.0.0.0").ToString
                            k.Close()
                            log.LS("Updater: ignored version is " & s)
                            If s <> ver.ToString Then
                                log.LS("Updater: dialog will be shown")
                                InitializeComponent()
                                Label3.Text = My.Application.Info.Version.ToString & vbCrLf & newVer
                                TextBox1.Text = wc.DownloadString("http://plrenewer.googlecode.com/svn/trunk/log.txt")
                                log.LX("Updater: showing up...")
                                Me.ShowDialog(CType(log, IWin32Window))
                                Exit For
                            End If
                        End If
                        log.LX("Updater: exiting...")
                        Exit For
                    End If
                Next
            Catch ex As Exception
                log.LS("Updater: parsing failed: " & ex.Message)
            End Try
        End If
    End Sub
    Private Delegate Sub StrindDownDelegate(ByVal sender As Object, ByVal e As PlrDownloadStringCompletedEventArgs)
    Private Delegate Sub CtLogDelegate(ByVal text As String)
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim k As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.CreateSubKey("Software\HACK-Design\PLReNewer\")
        k.SetValue("IgnoredVersion", newVer)
        k.Close()
        Me.Close()
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        '		Button1.Enabled = False
        '		Button2.Enabled = False
        '		Button3.Enabled = False
        '		dwnl = True
        '		dwnl_ok = False
        '		Dim temp As String = Environment.GetEnvironmentVariable("temp")
        '		Dim dyn_hdzname As String = ""
        '		Try
        '			log.LS("Updater: begin async dl of descr...")
        '			Dim writer As New IO.FileStream(temp & "\PLU.exe", IO.FileMode.Create, IO.FileAccess.ReadWrite, IO.FileShare.ReadWrite)
        '			writer.Write(My.Resources.Plu, 0, My.Resources.Plu.Length)
        '			writer.Flush()
        '			writer.Close()
        '			writer.Dispose()
        '			writer = New IO.FileStream(temp & "\HDZArchive.dll", IO.FileMode.Create, IO.FileAccess.ReadWrite, IO.FileShare.ReadWrite)
        '			writer.Write(My.Resources.HDZArchive, 0, My.Resources.HDZArchive.Length)
        '			writer.Flush()
        '			writer.Close()
        '			writer.Dispose()
        '			dyn_hdzname = temp & "\" & Now.Ticks.ToString
        '			AddHandler wc.DownloadProgressChanged, AddressOf DwnlProgress
        '			AddHandler wc.DownloadFileCompleted, AddressOf DwnlEnd
        '			ProgressBar1.Visible = True
        '			Label5.Visible = True
        '			dwnl_start = Now
        '			wc.DownloadFileAsync(New Uri("http://plrenewer.googlecode.com/svn/trunk/bin/x86/Release/Update.hdz"), dyn_hdzname)
        '			While dwnl
        '				Threading.Thread.Sleep(10)
        '				Application.DoEvents()
        '			End While
        '			RemoveHandler wc.DownloadProgressChanged, AddressOf DwnlProgress
        '			RemoveHandler wc.DownloadFileCompleted, AddressOf DwnlEnd
        '			ProgressBar1.Visible = False
        '			Label5.Visible = False
        '		Catch ex As Exception
        '			dwnl_ok = False
        '			MsgBox(ex.Message, MsgBoxStyle.ApplicationModal Or MsgBoxStyle.Exclamation, "Ошибка при обновлении")
        '		End Try
        '		Try
        '			If dwnl_ok Then
        '				Dim psi As New ProcessStartInfo(temp & "\Plu.exe", "/hdz """ & dyn_hdzname & """ /path """ & Application.ExecutablePath & """ /exe PLReNewer.exe")
        '				If Not IsAdmin Then psi.Verb = "runas"
        '				psi.UseShellExecute = True
        '				Process.Start(psi)
        '				Me.Close()
        '				CType(log, Form).Close()
        '			Else
        '				Throw New Exception()
        '			End If
        '		Catch ex As Exception
        '			Button1.Enabled = True
        '			Button2.Enabled = True
        '			Button3.Enabled = True
        '		End Try
    End Sub
    Private Sub DwnlProgress(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        ProgressBar1.Value = e.ProgressPercentage
        Label5.Text = String.Format("Размер обновлений {0} КБ{1}Скорость {2} КБ/с", Format(e.BytesReceived / 1024.0, "0.00"), vbCrLf, Format((e.BytesReceived / 1024.0) / (Now - dwnl_start).TotalSeconds, "0.00"))
    End Sub
    Private Sub DwnlEnd(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        If e.Error Is Nothing Then
            dwnl_ok = True
        Else
            MsgBox(e.Error.Message, MsgBoxStyle.ApplicationModal Or MsgBoxStyle.Exclamation, "Ошибка при загрузке")
        End If
        dwnl = False
    End Sub
    Private Sub CtLog(ByVal text As String, ByVal isExtended As Boolean)
        If CType(log, Form).InvokeRequired Then
            If isExtended Then
                CType(log, Form).Invoke(New CtLogDelegate(AddressOf log.LX), New Object() {text})
            Else
                CType(log, Form).Invoke(New CtLogDelegate(AddressOf log.LS), New Object() {text})
            End If
        Else
            If isExtended Then
                log.LX(text)
            Else
                log.LS(text)
            End If
        End If
    End Sub
End Class
Friend Class PlrDownloadStringCompletedEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs
    Dim _result As String
    Public ReadOnly Property Result As String
        Get
            Return _result
        End Get
    End Property
    Sub New(ByVal result As String, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userToken As Object)
        MyBase.New(exception, cancelled, userToken)
        _result = result
    End Sub
End Class
