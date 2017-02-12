Partial Public Class MainForm
	'Процедуры в этом файле просто выдернуты из MainForm.vb для удобства отладки
	Friend WithEvents T1 As New Timer() With {.Enabled = False, .Interval = 500}
	Dim asm1, asm2 As Reflection.Assembly
	Private updater As Updater
	Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		If Processing2 Then
			MsgBox("Это окно не может быть закрыто до остановки копирования. Если вы хотите прервать копирование, то нажмите кнопку «Остановить», а затем закройте окно.", MsgBoxStyle.Critical, "Идёт копирование файлов...")
			e.Cancel = True
		Else
			LS("Form_Closing: Начало") : LogLevel += 1
			Dim k As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.CreateSubKey("Software\HACK-Design\PLReNewer\")
			LS("Ключ открыт, сохраняю...")
			k.SetValue("Formats", TextBox1.Text)
			k.SetValue("In_Count", ComboBox1.Items.Count)
			k.SetValue("Out_Count", ComboBox2.Items.Count)
			If ComboBox1.Items.Count > 0 Or ComboBox2.Items.Count > 0 Then
				For i As Integer = 0 To Math.Max(ComboBox1.Items.Count, ComboBox2.Items.Count) - 1
					If i < ComboBox1.Items.Count Then k.SetValue("In" & i.ToString, ComboBox1.Items(i).ToString)
					If i < ComboBox2.Items.Count Then k.SetValue("Out" & i.ToString, ComboBox2.Items(i).ToString)
				Next
			End If
			k.SetValue("UseLadder", If(CheckBox1.Checked, 1, 0))
			k.SetValue("UseDragAndDropFilter", If(CheckBox2.Checked, 1, 0))
			k.SetValue("TrackFreeSpace", If(CheckBox3.Checked, 1, 0))
			k.SetValue("UnCriticalLogging", If(CheckBox4.Checked, 1, 0))
			If ComboBox3.Items.Contains(ComboBox3.Text) Then k.SetValue("LadderLevel", ComboBox3.Items.IndexOf(ComboBox3.Text))
			If Me.WindowState = FormWindowState.Normal Then
				k.SetValue("Height", Me.Height)
				k.SetValue("Width", Me.Width)
				k.SetValue("Left", Me.Left)
				k.SetValue("Top", Me.Top)
				k.SetValue("TR2Val", TrackBar2.Value)
			Else
				LS("Windowstate некорректен: " & Me.WindowState.ToString)
			End If
			k.SetValue("LadderType", If(RadioButton5.Checked, 1, 0))
			k.SetValue("WildCard", TextBox2.Text)
			k.SetValue("CycleDebug", If(CheckBox6.Checked, 1, 0))
			k.SetValue("AltFM", TextBox4.Text)
			k.SetValue("AltFMEnabled", If(CheckBox7.Checked, 1, 0))
			k.SetValue("Aequillibrium", RB_Bal.Value)
			k.SetValue("VajtNois", RB_Vol.Value)
			k.SetValue("DragDropAutoSourceDetect", If(CheckBox8.Checked, 1, 0))
			k.SetValue("AutoPlayNextEnabled", If(CheckBox9.Checked, 1, 0))
			k.SetValue("CrossfaderEnabled", If(CheckBox10.Checked, 1, 0))
			k.SetValue("CrossfaderValue", NumericUpDown1.Value)
			k.SetValue("Repeat", If(CheckBox11.Checked, 1, 0))
			k.SetValue("ShuffleForward", If(CheckBox12.Checked, 1, 0))
			k.SetValue("ShuffleBackward", If(CheckBox13.Checked, 1, 0))
			k.SetValue("Replace1", If(CheckBox14.Checked, 1, 0))
			k.SetValue("Replace2", If(CheckBox15.Checked, 1, 0))
			k.SetValue("DefaultGenSize", NumericUpDown2.Value)
			k.SetValue("FaultIterations", NumericUpDown3.Value)
			k.SetValue("NoAutoScrollOnPlay", If(CheckBox16.Checked, 1, 0))
			k.SetValue("DmitrysPalette", If(CheckBox17.Checked, 1, 0))
			k.SetValue("EinstainsJoy", If(CheckBox18.Checked, 1, 0))
			k.SetValue("Valerianka", If(CheckBox19.Checked, 1, 0))
			k.SetValue("NoWayBack", If(CheckBox20.Checked, 1, 0))
			k.SetValue("IgnoreDriveDifference", If(CheckBox21.Checked, 1, 0))
			k.SetValue("NoManualDuplication", If(CheckBox22.Checked, 1, 0))
			k.SetValue("FolderSplitting", If(CheckBox23.Checked, 1, 0))
			k.SetValue("SplittingTreshold", NumericUpDown4.Value)
			k.SetValue("TimeDebug", If(CheckBox24.Checked, 1, 0))
			k.SetValue("FormatCheckEx", If(CheckBox25.Checked, 1, 0))
			k.SetValue("FormatCheckExCaseSens", If(CheckBox26.Checked, 1, 0))
			k.SetValue("CallExternalPlayer", If(CheckBox27.Checked, 1, 0))
			k.SetValue("EmptyFilterAction", ComboBox4.SelectedIndex)
			k.SetValue("IgnoreMSMDXMidiExs", If(CheckBox28.Checked, 1, 0))
			k.SetValue("PlayNextOnMSMDXError", If(CheckBox29.Checked, 1, 0))
			k.SetValue("UseGlassAtSe7en", If(CheckBox30.Checked, 1, 0))
			k.SetValue("UseSe7enTaskbar", If(CheckBox31.Checked, 1, 0))
			k.SetValue("UseGraphicsCompot", If(CheckBox32.Checked, 1, 0))
			k.SetValue("UseSizesAI", If(CheckBox33.Checked, 1, 0))
			k.SetValue("AchtungDebug", If(CheckBox34.Checked, 1, 0))
			k.SetValue("UseArialUniMS", If(CheckBox35.Checked, 1, 0))
			k.SetValue("ID3Level", ComboBox5.SelectedIndex)
			k.SetValue("TagEncoding", ComboBox7.SelectedIndex)
			k.SetValue("TagControlsBitmask", TagControlsBitmask(128))
			k.SetValue("GuessMP3", If(CheckBox43.Checked, 1, 0))
			k.Close()
			LS("Form_Closing: актуализируется лицензия")
			Try
				License.ActivUaLice()
			Catch ex As Exception
				MsgBox("Ошибка при записи файла лицензии", MsgBoxStyle.Critical, "Политика лицензирования нарушена")
			End Try
			LogLevel -= 1 : LS("Form_Closing: Конец")
		End If
	End Sub
	Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Text = My.Application.Info.ProductName & ", v." & My.Application.Info.Version.ToString
		Randomize()
		LogPanel = TabPage6Log
		TabControl1.Controls.Remove(TabPage6Log)
		Dim path1 As New Drawing2D.GraphicsPath()
		path1.AddEllipse(1, 1, Button18.Width - 3, Button18.Height - 3)
		Dim rgn As New Region(path1)
		rgn.Intersect(New Rectangle(1, CInt(Button18.Height / 5) + 1, Button18.Width - 3, CInt(Button18.Height / 5 * 3)))
		Button18.Region = rgn
		Button19.Region = rgn
		Button20.Region = rgn
		Button21.Region = rgn
		TagReader.host = Me
		TagWriter.host = Me
		tagGenre.Items.AddRange(TagReader.sGenreMatrix)
		SizeSuffix = " Mb"
		Dim k As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.CreateSubKey("Software\HACK-Design\PLReNewer\")
		TextBox1.Text = k.GetValue("Formats", "*.mp3;*.wma;*.ogg").ToString
		For i As Integer = 0 To CInt(k.GetValue("In_Count", 0)) - 1
			ComboBox1.Items.Add(k.GetValue("In" & i.ToString))
		Next
		For i As Integer = 0 To CInt(k.GetValue("Out_Count", 0)) - 1
			ComboBox2.Items.Add(k.GetValue("Out" & i.ToString))
		Next
		CheckBox1.Checked = If(CInt(k.GetValue("UseLadder", 0)) = 1, True, False)
		CheckBox2.Checked = If(CInt(k.GetValue("UseDragAndDropFilter", 1)) = 1, True, False)
		CheckBox3.Checked = If(CInt(k.GetValue("TrackFreeSpace", 1)) = 1, True, False)
		If CInt(k.GetValue("LadderLevel", 0)) <> -1 Then ComboBox3.SelectedIndex = CInt(k.GetValue("LadderLevel"))
		Me.Height = CInt(k.GetValue("Height", 514))
		Me.Width = CInt(k.GetValue("Width", 707))
		Me.Left = CInt(k.GetValue("Left", (Screen.PrimaryScreen.WorkingArea.Width - Me.Width) / 2))
		Me.Top = CInt(k.GetValue("Top", (Screen.PrimaryScreen.WorkingArea.Height - Me.Height) / 2))
		TrackBar2.Value = CInt(k.GetValue("TR2Val", 500))
		RadioButton5.Checked = If(CInt(k.GetValue("LadderType", 0)) = 1, True, False)
		TextBox2.Text = k.GetValue("WildCard", "\%1\(%3) %2\%6 %4.mp3").ToString
		TextBox4.Text = k.GetValue("AltFM", "").ToString
		CheckBox7.Checked = If(CInt(k.GetValue("AltFMEnabled", 0)) = 1, True, False)
		RB_Bal.Value = Math.Min(Math.Max(CInt(k.GetValue("Aequillibrium", 1000)), RB_Bal.Minimum), RB_Bal.Maximum)
		RB_Vol.Value = Math.Min(Math.Max(CInt(k.GetValue("VajtNois", 5000)), RB_Vol.Minimum), RB_Vol.Maximum)
		CheckBox8.Checked = If(CInt(k.GetValue("DragDropAutoSourceDetect", 0)) = 1, True, False)
		NumericUpDown1.Value = Math.Min(Math.Max(CInt(k.GetValue("CrossfaderValue", 250)), NumericUpDown1.Minimum), NumericUpDown1.Maximum)
		CheckBox10.Checked = If(CInt(k.GetValue("CrossfaderEnabled", 0)) = 1, True, False)
		CheckBox9.Checked = If(CInt(k.GetValue("AutoPlayNextEnabled", 0)) = 1, True, False)
		CheckBox11.Checked = If(CInt(k.GetValue("Repeat", 0)) = 1, True, False)
		CheckBox12.Checked = If(CInt(k.GetValue("ShuffleForward", 0)) = 1, True, False)
		CheckBox13.Checked = If(CInt(k.GetValue("ShuffleBackward", 0)) = 1, True, False)
		CheckBox14.Checked = If(CInt(k.GetValue("Replace1", 0)) = 1, True, False)
		CheckBox15.Checked = If(CInt(k.GetValue("Replace2", 0)) = 1, True, False)
		NumericUpDown2.Value = Math.Min(Math.Max(CInt(k.GetValue("DefaultGenSize", 4)), NumericUpDown2.Minimum), NumericUpDown2.Maximum)
		NumericUpDown3.Value = Math.Min(Math.Max(CInt(k.GetValue("FaultIterations", 7)), NumericUpDown3.Minimum), NumericUpDown3.Maximum)
		CheckBox16.Checked = If(CInt(k.GetValue("NoAutoScrollOnPlay", 0)) = 1, True, False)
		CheckBox17.Checked = If(CInt(k.GetValue("DmitrysPalette", 0)) = 1, True, False)
		CheckBox18.Checked = If(CInt(k.GetValue("EinstainsJoy", 0)) = 1, True, False)
		CheckBox19.Checked = If(CInt(k.GetValue("Valerianka", 0)) = 1, True, False)
		CheckBox20.Checked = If(CInt(k.GetValue("NoWayBack", 0)) = 1, True, False)
		CheckBox21.Checked = If(CInt(k.GetValue("IgnoreDriveDifference", 0)) = 1, True, False)
		CheckBox22.Checked = If(CInt(k.GetValue("NoManualDuplication", 0)) = 1, True, False)
		CheckBox23.Checked = If(CInt(k.GetValue("FolderSplitting", 0)) = 1, True, False)
		NumericUpDown4.Value = Math.Min(Math.Max(CInt(k.GetValue("SplittingTreshold", 30)), NumericUpDown4.Minimum), NumericUpDown4.Maximum)
		CheckBox25.Checked = If(CInt(k.GetValue("FormatCheckEx", 0)) = 1, True, False)
		CheckBox26.Checked = If(CInt(k.GetValue("FormatCheckExCaseSens", 1)) = 1, True, False)
		CheckBox27.Checked = If(CInt(k.GetValue("CallExternalPlayer", 0)) = 1, True, False)
		If CInt(k.GetValue("EmptyFilterAction", 0)) <> -1 Then ComboBox4.SelectedIndex = CInt(k.GetValue("EmptyFilterAction"))
		CheckBox28.Checked = If(CInt(k.GetValue("IgnoreMSMDXMidiExs", 1)) = 1, True, False)
		CheckBox29.Checked = If(CInt(k.GetValue("PlayNextOnMSMDXError", 0)) = 1, True, False)
		CheckBox32.Checked = If(CInt(k.GetValue("UseGraphicsCompot", 0)) = 1, True, False)
		CheckBox30.Checked = If(CInt(k.GetValue("UseGlassAtSe7en", 1)) = 1, True, False)
		CheckBox31.Checked = If(CInt(k.GetValue("UseSe7enTaskbar", 1)) = 1, True, False)
		CheckBox33.Checked = If(CInt(k.GetValue("UseSizesAI", 0)) = 1, True, False)
		CheckBox35.Checked = If(CInt(k.GetValue("UseArialUniMS", 0)) = 1, True, False)
		ComboBox5.SelectedIndex = CInt(k.GetValue("ID3Level", 5))
		ComboBox7.SelectedIndex = CInt(k.GetValue("TagEncoding", 3))
		TagControlsBitmask(CInt(k.GetValue("TagControlsBitmask", 0)))
		CheckBox43.Checked = If(CInt(k.GetValue("GuessMP3", 0)) = 1, True, False)
		'Debugging options are the last
		CheckBox24.Checked = If(CInt(k.GetValue("TimeDebug", 0)) = 1, True, False)
		CheckBox4.Checked = If(CInt(k.GetValue("UnCriticalLogging", 0)) = 1, True, False)
		CheckBox6.Checked = If(CInt(k.GetValue("CycleDebug", 0)) = 1, True, False)
		CheckBox34.Checked = If(CInt(k.GetValue("AchtungDebug", 1)) = 1, True, False)
		k.Close()
		Me.TabPage5Player.Controls.Add(RB_Bal.fKepper)
		Me.TabPage5Player.Controls.Add(RB_Pro.fKepper)
		Me.TabPage5Player.Controls.Add(RB_Vol.fKepper)
		For i As Integer = 0 To 9
			undoList(i) = New Collections.Generic.List(Of String)
		Next
		If Screen.GetWorkingArea(Me).Height <= 600 Or Screen.GetWorkingArea(Me).Width <= 800 Then Me.WindowState = FormWindowState.Maximized
		InvalidState = False
		init = False
		LogLevel = 0
		PlayIdx = -1
		LS(">>>>====-----Начало работы-----====<<<<")
		LS("Время: " & Now.TimeOfDay.ToString)
		LoadAsms()
		LS("Прокачка буфера обмена...")
		Try
			Clipboard.SetText("Error")
		Catch ex As Exception
			LS("Прокачка буфера обмена успешно завершилась исключением :)")
		End Try
		W7.GlassMode = False
		If W7.w7 Then
			CheckBox30.Enabled = True
			CheckBox31.Enabled = True
			If CheckBox31.Checked Then
				LS("Инициализация таскбара...")
				W7.taskbar = CType(New CTaskbarList, ITaskbarList3)
			Else
				LS("Инициализация таскбара пропущена")
			End If
			If W7.w7glass Then
				W7.GlassMode = True
				LS("Инициализация стекла...")
				Me.BackColor = Color.Black
				ListBox1.BackColor = Color.Black
				ListBox1.ForeColor = Color.White
				ListBox1.BorderStyle = BorderStyle.None
				Button24.ForeColor = Color.White
				Button22.ForeColor = Color.White
				W7Paint(TabPage5Player.Controls)
				AddHandler Button22.Paint, AddressOf PlayerControls_Paint
				AddHandler Button24.Paint, AddressOf PlayerControls_Paint
				LinkLabel1.LinkColor = Color.White
				W7.MakeGlass(Me.Handle)
			Else
				LS("Инициализация стекла пропущена")
			End If
			If Not IsAdmin Then
				Label17.Visible = True
				Button27.Enabled = False
				LS("UAC Active :(")
			Else
				LS("ЮАК отключен / Есть админ права")
			End If
		Else
			LS("Инициализация таскбара и стекла пропущена, версия ОС " & Environment.OSVersion.ToString)
		End If
		If Not W7.GlassMode And CheckBox32.Checked Then
			TabPage5Player.BackColor = Drawing.SystemColors.Control
			TabPage5Player.UseVisualStyleBackColor = True
			W7Paint(TabPage5Player.Controls)
		End If
		Compot = CheckBox32.Checked
		LS("Шрифт...")
		CheckBox35.Tag = 0
		If IO.File.Exists(Environment.GetEnvironmentVariable("windir") & "\fonts\arialuni.ttf") Then
			Try
				If CheckBox35.Checked Then
					Label13.Font = New Font("Arial Unicode MS", 7.5, FontStyle.Regular, GraphicsUnit.Point)
					Label15.Font = Label13.Font
					tagAlbum.Font = Label13.Font
					tagArtist.Font = Label13.Font
					tagComment.Font = Label13.Font
					tagGenre.Font = Label13.Font
					tagTrackName.Font = Label13.Font
					tagTrackNumber.Font = Label13.Font
					tagYear.Font = Label13.Font
					CheckBox35.Tag = 1
					LS("... применён :)")
				Else
					LS("... не применён, галка отключена")
				End If
			Catch ex As Exception
				Achtung("Не удалось применить шрифт АриалЮникод. Ошибка: " & ex.Message)
			End Try
		Else
			CheckBox35.Checked = False
			CheckBox35.Enabled = False
			ToolTip1.SetToolTip(CheckBox35, "Недоступно, т.к. не найден требуемый шрифт")
			LS("... не найден. Отключено, задизейблено, установлено")
		End If
		LS("Инициализация строк...")
		ComboBox2_TextChanged(Nothing, Nothing)
		LS("Init бонусный таймер...")
		T1.Start()
	End Sub
	Private Sub Form1_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
		Dim CP As Point = ListBox1.PointToScreen(New Point(CInt(ListBox1.Width / 2), CInt(ListBox1.Height / 2)))
		PLPleaseWait_Resize(CP.X - 240, CP.Y - CInt(PLPleaseWait_GetHeight() / 2), CInt(ListBox1.Width / 2) + 240)
		ShakeFocus()
	End Sub
	Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
		If Me.WindowState <> FormWindowState.Minimized Then
			TrackBar2.Maximum = ListBox1.Width - 13
			Form1_Move(sender, e)
		End If
	End Sub
	Sub ShakeFocus()
		If W7.GlassMode Then W7.MakeGlass(Me.Handle)
		ListBox1.Focus()
		CheckBox4.Focus()
		ListBox1.Focus()
	End Sub
	Sub W7Paint(ByRef Ctrls As Control.ControlCollection)
		LS("-> W7Paint @ " & If(W7.GlassMode, "glass", "non-glass") & ", control: " & Ctrls.Owner.Name)
		For i As Integer = 0 To Ctrls.Count - 1
			If W7.GlassMode Then
				If (TypeOf Ctrls(i) Is Button) OrElse (TypeOf Ctrls(i) Is RegressBar) Then
					Ctrls(i).ForeColor = Color.White
					Ctrls(i).BackColor = Color.Black
				ElseIf TypeOf Ctrls(i) Is iGlassEnabledControl Then
					CType(Ctrls(i), iGlassEnabledControl).GlassMode = True
				Else
					If (TypeOf Ctrls(i) Is CheckBox) Then
						Ctrls(i).ForeColor = Color.White
						Ctrls(i).BackColor = Color.Black
					End If
					AddHandler Ctrls(i).Paint, AddressOf PlayerControls_Paint
					AddHandler Ctrls(i).MouseEnter, AddressOf PlayerControls_Invalidater
					AddHandler Ctrls(i).MouseLeave, AddressOf PlayerControls_Invalidater
					AddHandler Ctrls(i).MouseUp, AddressOf PlayerControls_Invalidater
					AddHandler Ctrls(i).MouseDown, AddressOf PlayerControls_Invalidater
				End If
			Else
				If (TypeOf Ctrls(i) Is Button) OrElse (TypeOf Ctrls(i) Is Label) OrElse (TypeOf Ctrls(i) Is CheckBox) Then
					Ctrls(i).ForeColor = Drawing.SystemColors.ControlText
					Ctrls(i).BackColor = Color.Transparent
				ElseIf (TypeOf Ctrls(i) Is RegressBar) Then
					CType(Ctrls(i), RegressBar).ForeColor = Drawing.SystemColors.Highlight
					CType(Ctrls(i), RegressBar).BackColor = Drawing.SystemColors.Control
					CType(Ctrls(i), RegressBar).ForeColor2 = Drawing.SystemColors.Control
					CType(Ctrls(i), RegressBar).ForeColor2Active = Drawing.SystemColors.Highlight
					CType(Ctrls(i), RegressBar).VerticalDelta = 5
				End If
			End If
		Next
	End Sub
	Private Sub Timer1(ByVal sender As Object, ByVal e As System.EventArgs) Handles T1.Tick
		Dim t As Timer = CType(sender, Timer)
		t.Stop()
		If t.Tag Is Nothing Then
			LS("-> T1 ticked! Starting Auto-updater... Expect exception on disposing timer...")
			updater = New Updater(Me)
			ComboBox1.Focus()
			ListBox1.Focus()
		Else
			tagStatus.Font = Label26.Font
			tagStatus.Text = t.Tag.ToString
		End If
		t.Dispose()
	End Sub
	Sub LogLoaded()
		LX("Расположение реально загруженных:")
		LX(asm1.Location)
		LX(asm2.Location)
	End Sub
	Sub LoadAsms()
		LS("Загрузка библиотек MDX...")
		Dim loadMDX As Boolean = False
		Dim localEX As String = Nothing
		If IO.File.Exists(My.Application.Info.DirectoryPath & "\Microsoft.DirectX.dll") And IO.File.Exists(My.Application.Info.DirectoryPath & "\Microsoft.DirectX.AudioVideoPlayback.dll") Then
			Try
				asm1 = System.Reflection.Assembly.LoadFile(My.Application.Info.DirectoryPath & "\Microsoft.DirectX.dll")
				asm2 = System.Reflection.Assembly.LoadFile(My.Application.Info.DirectoryPath & "\Microsoft.DirectX.AudioVideoPlayback.dll")
				loadMDX = True
				LS("Библиотеки обнаружены в папке приложения. Загружены запросом по имени файла.")
				LogLoaded()
			Catch ex As Exception
				LS("Библиотеки в папке приложения не обнаружены!")
				localEX = ex.Message
			End Try
		End If
		If Not IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.System).ToLower.Replace("\system32", "") & "\Microsoft.NET\DirectX for Managed Code") Then
			NotifyIcon1.Visible = True
			NotifyIcon1.ShowBalloonTip(60000)
			LS("Показан балун об отсутствии MDX.")
		ElseIf Not loadMDX Then
			Try
				asm1 = System.Reflection.Assembly.Load("Microsoft.DirectX, Version=1.0.2902.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")
				asm2 = System.Reflection.Assembly.Load("Microsoft.DirectX.AudioVideoPlayback, Version=1.0.2902.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")
				loadMDX = True
				LS("Библиотеки загружены из GAC. Загружены запросом в полной форме.")
				LogLoaded()
			Catch ex As Exception
				If Not loadMDX Then MsgBox("Произошла ошибка загрузки MDX-библиотек из GAC: " & ex.Message & vbCrLf & If(localEX, "") & "Сообщите, пожалуйста, версию операционной системы, .Net Framework и PLRenewer'a разработчику (ссылка в окне ""О программе"")", MsgBoxStyle.Critical, "Ошибка загрузки MDX!")
			End Try
		End If
	End Sub
	Private Function TagControlsBitmask(ByVal bitmask As Integer) As Integer
		If (bitmask And 128) > 0 Then
			TagControlsBitmask = If(CheckBox36.Checked, 1, 0) Or If(CheckBox37.Checked, 2, 0) Or If(CheckBox38.Checked, 4, 0) Or If(CheckBox39.Checked, 8, 0) Or If(CheckBox40.Checked, 16, 0) Or If(CheckBox41.Checked, 32, 0) Or If(CheckBox42.Checked, 64, 0)
		Else
			CheckBox36.Checked = (bitmask And 1) > 0
			CheckBox37.Checked = (bitmask And 2) > 0
			CheckBox38.Checked = (bitmask And 4) > 0
			CheckBox39.Checked = (bitmask And 8) > 0
			CheckBox40.Checked = (bitmask And 16) > 0
			CheckBox41.Checked = (bitmask And 32) > 0
			CheckBox42.Checked = (bitmask And 64) > 0
			TagControlsBitmask = bitmask
		End If
	End Function
End Class