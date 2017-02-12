Module Player
	Private WithEvents Tima As New Timer With {.Interval = 25}
	Dim pl, pl2 As Microsoft.DirectX.AudioVideoPlayback.Audio
	Dim awaiting, firstPlayed As Boolean
	Dim tagString As String = ""
	Function Init(ByVal Path As String) As Boolean
        Try
            Form1.LS("Плеер: инит @ " & Path) : Form1.LogLevel += 1
            Tima.Stop()
            Form1.Label15.Text = Path
            Form1.LS("Плеер: получаю тег")
			Dim tag As AudioTag = GenTag(Path, ID3Mode.Both)
            If tag.Empty = False Then
				tagString = Form1.ParseWildcard("%1 / (%3) [%2] —" & If(CInt(Form1.CheckBox35.Tag) = 0, " ", vbCrLf) & "%5 %4", New String() {tag.Artist, tag.Album, tag.Year, tag.Track, tag.Number}, False)
            Else
                tagString = "Тэг не обнаружен."
            End If
            Form1.LS("Плеер: конструктор MsftDXAVPb... ахтунг! :)")
			If TestPL(True) Then pl.Dispose()
			UpdateLabel13(True)
			pl = New Microsoft.DirectX.AudioVideoPlayback.Audio(Path, False)
			firstPlayed = True
            Form1.LS("Плеер: громкость, баланс, запуск")
            pl.Volume = CInt(Form1.RB_Vol.Value) - 5000
            SetBal(CInt((Form1.RB_Bal.Value - Form1.RB_Bal.Maximum / 2) * Math.Abs(Form1.RB_Bal.Value - Form1.RB_Bal.Maximum / 2) / 200))
            pl.Play()
            Form1.LS("Плеер: регрессбары и прочее")
            Form1.RB_Pro.Maximum = CInt(pl.Duration * 10)
            Form1.RB_Pro.Value = 0
            Form1.Button20.Enabled = True
			Form1.RB_Pro.Enabled = True
			Form1.RB_Bal.Enabled = True
			Form1.AddPlayedIndex()
            Tima.Start()
			Form1.LogLevel -= 1 : Form1.LS("Плеер: готово")
            Return True
        Catch ex As Exception
            Alarma(ex, Path)
            Form1.LogLevel -= 1
            Return False
        End Try
	End Function
	Function Play() As Boolean
        Form1.LS("Плеер: воспроизведение")
        Try
            If TestPL(True) Then
                pl.Play()
                Tima.Start()
                Return True
            End If
			Throw New InvalidOperationException("Невозможно запустить воспроизведение: объект плеера не обнаружен [TestPL failed]")
        Catch ex As Exception
            Tima.Stop()
            Alarma(ex)
            Return False
        End Try
	End Function
	Function PlayStop() As Boolean
        Form1.LS("Плеер: стоп")
        PlayStop = True
		If TestPL(True) Then
			Try
				Dim b As Boolean = False
				If pl.Playing Then
					pl.Pause()
					b = True
				End If
				pl.CurrentPosition = 0
				If pl.Paused Then pl.Stop()
				If b Then Wait4Tima()
				UpdateLabel13(False)
				pl.Dispose()
				pl = Nothing
			Catch ex As Exception
				Alarma(ex)
				Return False
			End Try
		End If
	End Function
    Function Pause() As Boolean
        Form1.LS("Плеер: пауза")
        Try
            If TestPL(True) Then
                pl.Pause()
                Wait4Tima()
                Return True
            End If
			Throw New InvalidOperationException("Невозможно приостановить воспроизведение: объект плеера не обнаружен [TestPL failed]")
        Catch ex As Exception
            Alarma(ex)
            Return False
        End Try
    End Function
	Function SetBal(ByVal value As Integer) As Boolean
		Try
			If TestPL(True) Then
				pl.Balance = value
				Return True
			End If
			Throw New InvalidOperationException("Невозможно установить баланс: объект плеера не обнаружен [TestPL failed]")
		Catch ex As Exception
			If Not (Form1.Label15.Text.EndsWith(".mid") Or Form1.Label15.Text.EndsWith(".midi")) AndAlso Form1.CheckBox28.Checked = False Then
				Alarma(ex)
				Return False
			End If
		End Try
	End Function
	Function SetVol(ByVal value As Integer) As Boolean
		Try
			If TestPL(True) Then
				pl.Volume = value
				Return True
			End If
			If firstPlayed Then Throw New InvalidOperationException("Невозможно установить громкость: объект плеера не обнаружен [TestPL failed]")
		Catch ex As Exception
			Alarma(ex)
			Return False
		End Try
	End Function
	Function SetPro(ByVal value As Double) As Boolean
		Try
			If TestPL(True) Then
				pl.CurrentPosition = value
				Return True
			End If
			Throw New InvalidOperationException("Невозможно установить позицию: объект плеера не обнаружен [TestPL failed]")
		Catch ex As Exception
			Alarma(ex)
			Return False
		End Try
	End Function
	Sub Alarma(ByVal ex As Exception)
		Alarma(ex, "")
	End Sub
	Sub Alarma(ByVal ex As Exception, ByVal AddInfo As String)
		Form1.Achtung("Плеер: ошибка «" & ex.Message & "» " & ex.StackTrace)
		If AddInfo <> "" Then Form1.Achtung("Плеер: расширенная информация «" & AddInfo & "» ")
		Form1.Button20.Enabled = False
		Form1.Button19.Text = "4"
		Form1.AddPlayedIndex()
		If Form1.CheckBox29.Checked Then Form1.PlayNextCaller(False) Else Form1.ListBox1.Refresh()
	End Sub
	Sub Boom(ByVal sender As Object, ByVal e As EventArgs) Handles Tima.Tick
		If TestPL(True) Then
			If Form1.RB_Pro.Maximum < CInt(pl.CurrentPosition * 10) Then Form1.RB_Pro.Maximum = CInt(pl.CurrentPosition * 10)
			Form1.RB_Pro.Value = CInt(pl.CurrentPosition * 10)
			Dim s As String
			If Form1.RB_Bal.Value = Form1.RB_Bal.Minimum + (Form1.RB_Bal.Maximum - Form1.RB_Bal.Minimum) / 2 Then
				s = "центр"
			Else
				s = If(Form1.RB_Bal.Value < Form1.RB_Bal.Minimum + (Form1.RB_Bal.Maximum - Form1.RB_Bal.Minimum) / 2, "влево ", "вправо ")
			End If
			UpdateLabel13(True)
			Form1.RB_Bal.Text = s & If(s = "центр", "", Math.Abs(Math.Floor((Form1.RB_Bal.Value - 1000) * 100 / 1000)).ToString & "%")
			If pl.Playing Then Form1.NumericUpDown1.Enabled = False Else Form1.NumericUpDown1.Enabled = True
			If Form1.CheckBox9.Checked Then
				If Form1.CheckBox10.Checked Then
                    If pl.CurrentPosition >= pl.Duration - Form1.NumericUpDown1.Value / 1000 Then
                        Form1.LS("Плеер: пора играть новый трек. Проверка определением индекса...")
                        If Form1.GenNextPlayedIndex <> -1 Then
                            pl2 = pl
                            pl = Nothing
                            Form1.LS("Плеер: индекс не -1")
                        End If
                        Form1.PlayNextCaller(False)
                    End If
					If TestPL(False) AndAlso pl2.CurrentPosition = pl2.Duration Then pl2.Dispose()
				Else
					If pl.CurrentPosition = pl.Duration Then
						Form1.LS("Плеер: пора играть новый трек. Вызов напрямую...")
						Form1.PlayNextCaller(True)
					End If
				End If
			Else
				If pl.CurrentPosition = pl.Duration Then Form1.StopCaller4Redraw()
			End If
		End If
		awaiting = False
	End Sub
	Sub Wait4Tima()
		awaiting = True
		While awaiting
			Threading.Thread.Sleep(50)
			Application.DoEvents()
		End While
		Tima.Stop()
	End Sub
	Sub UpdateLabel13(ByVal PlusTag As Boolean)
		Form1.Label13.Text = If(TestPL(True), ParseStateRuRu(pl.State) & If(PlusTag, ", " & ParseTime(pl.CurrentPosition) & " / " & ParseTime(pl.Duration) & vbCrLf & tagString, ""), "Не воспроизводится")
		If Not PlusTag Then Form1.Label15.Text = ""
	End Sub
	Function ParseTime(ByVal d As Double) As String
		Dim i As Integer = CInt(Math.Floor(d / 60))
		d -= i * 60
		ParseTime = Format(i, "00") & ":" & Format(CInt(d), "00")
	End Function
	Function ParseStateRuRu(ByVal State As Integer) As String
		If State = 0 Or State = 1 Then Return "Остановлен"
		'  If State = 1 Then Return "Приостановлен"
		Return "Воспроизведение"
	End Function
	Function IsPlaying() As Boolean
		Try
			If TestPL(True) Then
				Return pl.Playing
			ElseIf TestPL(False) Then
				Return pl2.Playing
			Else
				Return False
			End If
		Catch ex As Exception
			Return False
		End Try
	End Function
	Function TestPL(ByVal pl1 As Boolean) As Boolean
		If pl1 Then
			Return pl IsNot Nothing AndAlso pl.Disposed = False
		Else
			Return pl2 IsNot Nothing AndAlso pl2.Disposed = False
		End If
	End Function
End Module