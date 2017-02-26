Partial Class MainForm
#Region "Definitions and implementation"
    Implements iLoggerHost
    'Objects
    Dim inFilesGlobal As New Collections.Generic.List(Of String)
    Dim inFiles As New Collections.Generic.List(Of String)
    Dim inFilesSizes As New Collections.Generic.List(Of Long)
    Dim outFiles As New Collections.Generic.List(Of String)
    Dim undoList(10) As Collections.Generic.List(Of String)
    Dim plFiles As New Collections.Generic.List(Of Integer)
    Dim LogPanel As TabPage
    'Private global vars
    Enum SearchVector
        Up = -1
        Down = 1
    End Enum
    Dim UndoListIndex As Integer = 0
    Dim Processing, Processing2, ProcessingScroll, LxB, InvalidState, Paused, DefSmooth, CurSmooth, Compot As Boolean
    Dim inFileCurrent, outFileCurrent, LastErrorText, LastText, SizeSuffix As String
    Dim PlayIdx, Ticks, ClipboardCheckResult, GFS_num, Ll As Integer
    Dim GetFullSize__va_cache As Long = -1
    'Multi-threaded GetFilesMy
    Dim mt_gf_Range() As String
    'Public global vars
    Public PLPBVal, PLPB2Val, PLPB2Max, ProcessingInternal As Long
    Public init As Boolean
    'Properties
    Public Property LogLevel As Integer Implements iLoggerHost.LogLevel
        Get
            Return Ll
        End Get
        Set(ByVal value As Integer)
            Ll = value
        End Set
    End Property
#End Region
#Region "Handlers"
    Private Sub Button1_InFolder(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        LS("ГУЙ: Нажата кнопка 1")
        Folder.Description = "Добавьте папку-источник..."
        If Folder.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim idx As Integer = ComboBox1.Items.IndexOf(Folder.SelectedPath)
            If idx = -1 Then
                ComboBox1.Items.Add(Folder.SelectedPath)
                ComboBox1.SelectedIndex = ComboBox1.Items.Count - 1
            Else
                If ComboBox1.SelectedIndex <> idx Then
                    ComboBox1.SelectedIndex = idx
                Else
                    ComboBox1_TextChanged(Nothing, Nothing)
                End If
            End If
            LS("Выбрана папка источника: " & Folder.SelectedPath)
        End If
    End Sub
    Private Sub Button2_OutFolder(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        LS("ГУЙ: Нажата кнопка 2")
        Folder.Description = "Добавьте папку-назначение..."
        If Folder.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim idx As Integer = ComboBox2.Items.IndexOf(Folder.SelectedPath)
            If idx = -1 Then
                ComboBox2.Items.Add(Folder.SelectedPath)
                ComboBox2.SelectedIndex = ComboBox2.Items.Count - 1
            Else
                If ComboBox2.SelectedIndex <> idx Then
                    ComboBox2.SelectedIndex = idx
                Else
                    ComboBox2_TextChanged(Nothing, Nothing)
                End If
            End If
            LS("Выбрана папка назначения: " & Folder.SelectedPath)
        End If
    End Sub
    Private Sub Button3_Generate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        LS("ГУЙ: Нажата кнопка 3")
        CM_Generate.Show(Button3, New Point(0, 0))
    End Sub
    Private Sub Button4_InPlaylist(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        LS("ГУЙ: Нажата кнопка 4")
        OFD.Title = "Добавьте плейлист-источник..."
        If OFD.ShowDialog = Windows.Forms.DialogResult.OK Then
            ComboBox1.Items.Add(OFD.FileName)
            ComboBox1.SelectedIndex = ComboBox1.Items.Count - 1
            LS("Выбран исходный плейлист: " & OFD.FileName)
        End If
    End Sub
    Private Sub Button5_OutPlaylist(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        LS("ГУЙ: Нажата кнопка 5")
        SFD.Title = "Добавьте плейлист-назначение..."
        If SFD.ShowDialog = Windows.Forms.DialogResult.OK Then
            ComboBox2.Items.Add(SFD.FileName)
            ComboBox2.SelectedIndex = ComboBox2.Items.Count - 1
            LS("Выбран целевой плейлист: " & SFD.FileName)
        End If
    End Sub
    Private Sub Button6_CopyFiles(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        LS("Ксерокс Старт/Стоп: Начало @" & Processing2.ToString)
        If Not Processing2 Then
            Processing2 = True
            Button6.Text = "Остановить"
        Else
            Processing2 = False
            PLPLeaseWait_ChangeText("Ожидаю завершение записи файла...")
            Button6.Text = "Остановка"
            Button6.Enabled = False
            LS("Ксерокс Старт/Стоп: Внутренний конец ")
            Exit Sub
        End If
        LogLevel += 1
        LS("Проверяем массивы на соответствие...")
        If inFiles.Count <> outFiles.Count Then NameGen()
        If inFiles.Count <> inFilesSizes.Count Then SizesUpdate()
        If PathExists(ComboBox2.Text) Then
            LS("Дизейблим контроллы...")
            Dim t0, t1, t2, t3, t4, t5 As TabPage
            t0 = TabPage2GenStgs
            t1 = TabPage3NmgStgs
            t2 = TabPage4SvgStgs
            t3 = TabPage5Player
            t4 = TabPage8TagEditor
            t5 = TabPage7ViewStgs
            TabControl1.TabPages.Remove(t0)
            TabControl1.TabPages.Remove(t1)
            TabControl1.TabPages.Remove(t2)
            TabControl1.TabPages.Remove(t3)
            TabControl1.TabPages.Remove(t4)
            TabControl1.TabPages.Remove(t5)
            ComboBox1.Enabled = False
            ComboBox2.Enabled = False
            GroupBox1.Enabled = False
            Button1.Enabled = False
            Button2.Enabled = False
            Button3.Enabled = False
            Button4.Enabled = False
            Button5.Enabled = False
            Button7.Enabled = False
            Button9.Enabled = False
            Button11.Enabled = False
            Button12.Enabled = False
            Button13.Enabled = False
            Button16.Enabled = False
            Button25.Enabled = False
            Button26.Enabled = False
            Button20.PerformClick()
            PLPleaseWait_Init("Идёт копирование...", 0, GetFullSize, 0)
            Ticks = 0
            LS("Поехали, файлов " & outFiles.Count)
            LogLevel += 1
            Dim phile As String, spd As Double
            For i As Integer = 0 To outFiles.Count - 1
                phile = "Файл: " & Strings.Mid(inFiles(i), inFiles(i).LastIndexOf("\") + 2)
                PLPLeaseWait_ChangeText(phile)
                inFileCurrent = inFiles(i)
                outFileCurrent = outFiles(i)
                Dim trd As New Threading.Thread(AddressOf CopyFile)
                ProcessingInternal = 0
                trd.Start()
                LS("Запущен поток №" & trd.ManagedThreadId & "; " & inFileCurrent & " -> " & outFileCurrent)
                While ProcessingInternal = 0
                    If isPL() Then
                        'Дешевле просто сделать проверку, чем строить прокси на каждый чих
                        PLPleaseWait.Value1 = PLPBVal
                        PLPleaseWait.Max2 = PLPB2Max
                        PLPleaseWait.Value2 = PLPB2Val
                        If Ticks > 0 AndAlso Button6.Text = "Остановить" Then
                            spd = PLPBVal / (Ticks * 1.24)
                            PLPLeaseWait_ChangeText(phile & Chr(13) & Chr(10) & "Скорость: " & Format(spd, "0.00") & " кб/с, осталось: " & CInt((PLPB2Max - PLPB2Val) / (spd * 1024)) & " с (всего: " & ParseTime(CInt(PLPleaseWait.GetLeftBytes / (spd * 1024))) & ")", False)
                        End If
                    End If
                    Application.DoEvents()
                    Threading.Thread.Sleep(15)
                End While
                If CheckBox3.Checked Then Button11_GetFreeSpace(Nothing, Nothing)
                If ProcessingInternal = 1 Then
                    LS("Скопировано успешно (код: 1)")
                Else
                    PLPBVal += inFilesSizes(i)
                    Achtung(inFileCurrent & " -> " & outFileCurrent & " : " & LastErrorText)
                    LS("Попытка удаления файла, скопированного с ошибкой...")
                    Try
                        IO.File.Delete(outFileCurrent)
                        LS("Успешно")
                    Catch ex As Exception
                        Achtung("Попытка удаления файла, скопированного с ошибкой, не увенчалась успехом :(")
                    End Try
                End If
                LS("Проверка на прерывание: " & (Not Processing2).ToString)
                If Not Processing2 Then Exit For
            Next
            LS("Инейблим контроллы...")
            TabControl1.TabPages.Insert(1, t0)
            TabControl1.TabPages.Insert(2, t1)
            TabControl1.TabPages.Insert(3, t2)
            TabControl1.TabPages.Insert(4, t3)
            TabControl1.TabPages.Insert(5, t4)
            TabControl1.TabPages.Insert(6, t5)
            GroupBox1.Enabled = True
            ComboBox1.Enabled = True
            ComboBox2.Enabled = True
            Button1.Enabled = True
            Button2.Enabled = True
            Button3.Enabled = True
            Button4.Enabled = True
            Button5.Enabled = True
            Button7.Enabled = True
            Button9.Enabled = True
            Button11.Enabled = True
            Button12.Enabled = True
            Button13.Enabled = True
            Button16.Enabled = True
            Button25.Enabled = True
            Button26.Enabled = True
            TrackBar1.Value = CInt(Math.Min(GetTrackBar1ValueMB(), TrackBar1.Maximum))
            LogLevel -= 1
            PLPleaseWait_Halt()
        Else
            If ComboBox2.Text.ToLower.EndsWith(".pla") Then
                '257 byte playlist test
                For i As Integer = 0 To inFiles.Count - 1
                    If inFiles(i).Length > 257 Then
                        MsgBox("Длина имени файла " & inFiles(i) & "превышает максимально допустимую для формата *.pla. Файл плейлиста не был сохранён.", MsgBoxStyle.Exclamation, "Проверка длины имён файлов")
                        Exit Sub
                    End If
                Next
                'Как и в случае с парсером, компоновщик придётся писать полностью отдельно от текстовых плейлистов
                LS("Открываю БинРайтер к " & ComboBox2.Text)
                Dim outstr As New IO.BinaryWriter(New IO.FileStream(ComboBox2.Text, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None))
                LS("Поехали, пишем текст плейлиста, файлов: " & inFiles.Count) : LogLevel += 1
                'Header
                Dim Bytes512(511) As Byte
                ClearBytes(Bytes512)
                'Конвертируем в BE вручную Int32
                Bytes512(0) = CByte((inFiles.Count And &HFF000000) >> 24)
                Bytes512(1) = CByte((inFiles.Count And &HFF0000) >> 16)
                Bytes512(2) = CByte((inFiles.Count And &HFF00) >> 8)
                Bytes512(3) = CByte(inFiles.Count And &HFF)
                System.Text.Encoding.BigEndianUnicode.GetBytes("iriver UMS PLA").CopyTo(Bytes512, 4)
                'Остальное нас не интересует
                outstr.Write(Bytes512)
                Dim iStr As String, iPtr As Integer
                For i As Integer = 0 To inFiles.Count - 1
                    ClearBytes(Bytes512)
                    iStr = Mid(inFiles(i), 3)   'убираем диск и двоеточие
                    iPtr = iStr.LastIndexOf("\") + 2  ' так нада ;)
                    Bytes512(0) = CByte((iPtr And &HFF00) >> 8)
                    Bytes512(1) = CByte(iPtr And &HFF)
                    System.Text.Encoding.BigEndianUnicode.GetBytes(iStr).CopyTo(Bytes512, 2)
                    outstr.Write(Bytes512)
                Next
                outstr.Flush()
                outstr.Close()
            Else
                LS("Открываю СтримРайтер к " & ComboBox2.Text)
                Dim outstr As New IO.StreamWriter(ComboBox2.Text, False, If(ComboBox2.Text.ToLower.EndsWith(".m3u8"), System.Text.Encoding.UTF8, If(ComboBox2.Text.ToLower.EndsWith(".plc"), System.Text.Encoding.Unicode, System.Text.Encoding.Default)))
                Dim plc As Boolean = ComboBox2.Text.ToLower.EndsWith(".plc")
                LS("Поехали, пишем текст плейлиста, файлов: " & inFiles.Count & ", режим относительности: " & CheckBox18.Checked) : LogLevel += 1
                If plc Then outstr.WriteLine("<Default:1>")
                For i As Integer = 0 To inFiles.Count - 1
                    If plc Then outstr.Write("1|")
                    If CheckBox18.Checked AndAlso (inFiles(i).StartsWith(ComboBox2.Text.Chars(0)) Or (CheckBox20.Checked And CheckBox21.Checked)) Then
                        'Относит. плейлист + Файл и плейлист на одном диске
                        '					или игнорим разницу в дисках и обрезаем до файлнеймов
                        If CheckBox20.Checked Then
                            outstr.Write(Mid(PathORezq(inFiles(i), 0), 2))
                        Else
                            Dim j As Byte = 0
                            Dim k As Integer = CountChars(inFiles(i), "\"c)
                            Dim s As String
                            While j < k
                                s = ComboBox2.Text.Substring(0, ComboBox2.Text.Length - PathORezq(ComboBox2.Text, j).Length)
                                If inFiles(i).Contains(s) Then
                                    If j > 0 Then
                                        For l As Integer = 0 To j - 1
                                            outstr.Write("..\")
                                        Next
                                    End If
                                    outstr.Write(Mid(inFiles(i), s.Length + 2))
                                    Exit While
                                End If
                                j += CByte(1)
                            End While
                        End If
                    Else 'Файл и плейлист на разных дисках | Плейлист абсолютный
                        outstr.Write(inFiles(i))
                    End If
                    outstr.WriteLine(If(plc, "||||||||||||", ""))
                Next
                If ComboBox2.Text.ToLower.EndsWith(".wmpl3") Then outstr.Write(":WINMPP:1")
                LS("флаш-клоуз-диспоуз :)")
                LogLevel -= 1
                outstr.Flush()
                outstr.Close()
                outstr.Dispose()
            End If
            MsgBox("Готово! Плейлист сохранён.", MsgBoxStyle.Information, "Выполнено")
            LS("Есть!")
        End If
        Processing2 = False
        Button6.Enabled = True
        Button6.Text = "Скопировать"
        LogLevel -= 1 : LS("Ксерокс Старт/Стоп: Конец")
    End Sub
    Private Sub Button7_ShowContextMenu2(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        LS("ГУЙ: Нажата кнопка 7")
        CM_Delete.Show(Button7, New Point(0, 0))
    End Sub
    Private Sub Button8_OpenSource_Button10_OpenDestination(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click, Button10.Click
        Dim textHolder As ComboBox = If(sender Is Button8, ComboBox1, ComboBox2)
        Dim command As String = If(TextBox4.Enabled AndAlso IO.File.Exists(TextBox4.Text), TextBox4.Text, "explorer")
        Dim arguments As String = """" & UIO.Directory.GetFileSystemOpenPath(textHolder.Text) & """"
        LS("ГУЙ: Нажата кнопка " & CType(sender, Control).Name & "; Комманда: " & command & " " & arguments)
        Process.Start(command, arguments)
    End Sub
    Private Sub Button9_ShowContextMenu(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        LS("ГУЙ: Нажата кнопка 9")
        CM_Edit.Show(Button9, New Point(0, 0))
    End Sub
    Private Sub Button11_GetFreeSpace(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        Dim b As Boolean = PathExists(ComboBox2.Text)
        LS("GetFreeSpace: начало @ " & If(sender Is Nothing, "галка; ", "кнопка; ") & b.ToString & "; " & RadioButton2.Checked.ToString) : LogLevel += 1
        If RadioButton2.Checked Then
            LX("Выполняется рекурсивный запрос...")
            RadioButton1.Checked = True
            Button11_GetFreeSpace(sender, e)
            RadioButton2.Checked = True
        Else
            If b Then
                LS("Проверка успешна, адрес: " & ComboBox2.Text)
                For Each drv In UIO.DriveInfo.GetDrives
                    If ComboBox2.Text.StartsWith(drv.Name) Then
                        TrackBar1.Value = 0
                        TrackBar1.Maximum = CInt(GetFullSizeMBStrict(drv.TotalFreeSpace))
                        TrackBar1.Value = TrackBar1.Maximum
                        LS("Диск " & drv.Name & ", свободно " & drv.TotalFreeSpace)
                        Exit For
                    End If
                Next
            Else
                LS("Проверка завершилась неудачей! Значение скролла: " & NumericUpDown2.Value)
                TrackBar1.Value = 1 : TrackBar1.Value = 0 'Для гарантированного вызова события авто-апдейта
                TrackBar1.Maximum = CInt(NumericUpDown2.Value * 1024)
                If IsPlaylist(ComboBox2.Text) Then TrackBar1.Value = TrackBar1.Maximum
            End If
        End If
        LogLevel -= 1 : LS("GetFreeSpace: конец ")
    End Sub
    Private Sub Button12_Sort(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        CM_Order.Show(Button12, New Point(0, 0))
    End Sub
    Private Sub Button13_EditLinks(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        LS("EditLinx: Начало") : LogLevel += 1
        If Register.ShowDialog = Windows.Forms.DialogResult.OK Then
            LS("<<ok")
            ComboBox1.Items.Clear()
            ComboBox2.Items.Clear()
            LS("очищено, вскрываю ключ...")
            Dim k As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.CreateSubKey("Software\HACK-Design\PLReNewer\")
            For i As Integer = 0 To CInt(k.GetValue("In_Count", 0)) - 1
                ComboBox1.Items.Add(k.GetValue("In" & i.ToString))
            Next
            For i As Integer = 0 To CInt(k.GetValue("Out_Count", 0)) - 1
                ComboBox2.Items.Add(k.GetValue("Out" & i.ToString))
            Next
            k.Close()
        End If
        LogLevel -= 1 : LS("EditLinx: Конец")
    End Sub
    Private Sub Button14_ClearLog(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        LS("ГУЙ: Нажата кнопка 14")
        Logbox.Items.Clear()
        TabPage6Log.Text = "Лог"
        If Not (CheckBox4.Checked Or CheckBox6.Checked) Then
            LogPanel = TabPage6Log
            TabControl1.Controls.Remove(TabPage6Log)
        End If
    End Sub
    Private Sub Button15_CopyLog2Clopboard(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        LS("CopyLog2Clipboard: Начало@" & Logbox.Items.Count & ", " & CheckBox6.Checked) : LogLevel += 1
        If Logbox.Items.Count > 0 Then
            If CheckBox6.Checked Then
                LS("Открываю файл " & MyEnvGetSpecFolder(EnvSpFldr.Desktop) & "\Log.txt")
                Dim sOut As New IO.StreamWriter(MyEnvGetSpecFolder(EnvSpFldr.Desktop) & "\Log.txt", False, System.Text.Encoding.UTF8)
                LogLevel += 1
                For i As Integer = 0 To Logbox.Items.Count - 1
                    sOut.Write(Logbox.Items(i).ToString & Chr(13) & Chr(10))
                    If i Mod 100 = 0 Then
                        Button15.Text = CLng(i * 10000 / Logbox.Items.Count) / 100 & "%"
                        Application.DoEvents()
                    End If
                Next
                LogLevel -= 1
                LS("флаш-клоуз-диспоуз")
                sOut.Flush()
                sOut.Close()
                sOut.Dispose()
                MsgBox("Лог, возможно, содержит нуль-символы и в буфер целиком не залезет. Файл лога сохранён на Рабочем Столе.", MsgBoxStyle.Information, "Лог был сохранён в файл log.txt")
            Else
                Dim txt As String = ""
                LS("Строка создана")
                LogLevel += 1
                For i As Integer = 0 To Logbox.Items.Count - 1
                    txt &= Logbox.Items(i).ToString & Chr(13) & Chr(10)
                    If i Mod 100 = 0 Then
                        Button15.Text = CLng(i * 10000 / Logbox.Items.Count) / 100 & "%"
                        Application.DoEvents()
                    End If
                Next
                LogLevel -= 1
                LS("Чищу клипборд")
                Clipboard.Clear()
                LS("Чищу клипборд")
                Clipboard.SetText(txt)
            End If
            Button15.Text = "Копировать"
        Else
            MsgBox("Лог пустой...", MsgBoxStyle.Exclamation, "Ошибка")
        End If
        LogLevel -= 1 : LS("CopyLog2Clipboard: Конец")
    End Sub
    Private Sub Button16_Undo(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click
        LS("Отменялка: начало") : LogLevel += 1
        Dim ts As String
        If ListBox1.SelectedIndex <> -1 Then ts = inFiles(ListBox1.SelectedIndex) Else ts = "::"
        If plFiles.Count > 0 Then
            LS("Сохраняем данные для отката сыгранных треков: " & plFiles.Count) : LogLevel += 1
            Dim tArr(plFiles.Count - 1) As String
            For i As Integer = 0 To plFiles.Count - 1
                tArr(i) = inFiles(plFiles(i))
                LX(tArr(i))
            Next
            LogLevel -= 1
            UndoInternals()
            plFiles.Clear()
            LS("Попытка восстановления сыграных...")
            Dim k As Integer
            For i As Integer = 0 To tArr.Length - 1
                k = inFiles.IndexOf(tArr(i))
                If k > -1 Then plFiles.Add(k)
                LX(k.ToString)
            Next
        Else
            UndoInternals()
        End If
        UndoListIndex -= 1
        LS("UndoListIndex = " & UndoListIndex)
        If UndoListIndex = 0 Then Button16.Enabled = False
        ReFreshTail()
        Dim j As Integer = inFiles.IndexOf(ts)
        LS("Активная строка: " & ts & ", индекс: " & j)
        If j <> 0 Then Scroll2(j)
        LogLevel -= 1 : LS("Отменялка: конец")
    End Sub
    Private Sub Button17_SelectAlternateFM(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click
        LS("ГУЙ: Нажата кнопка 16")
        If OFD2.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox4.Text = OFD2.FileName
            LS("Выбран альтернативный файл-менеджер: " & OFD2.FileName)
        End If
    End Sub
    Private Sub Button18_Previous(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click
        LS("Предыдущий: начало @ " & CheckBox11.Checked & "; " & CheckBox13.Checked) : LogLevel += 1
        Dim j As Integer = GetCurrentPlayingIndex()
        LS("Текущий индекс: " & j)
        Dim ti As Integer = ListBox1.SelectedIndex
        If CheckBox11.Checked Then
            ListBox1.SelectedIndex = j
        Else
            If CheckBox13.Checked Then
                If CheckBox16.Checked Then ListBox1.SelectedIndex = plFiles(plFiles.Count - 2) Else Scroll2(plFiles(plFiles.Count - 2))
                plFiles.RemoveAt(plFiles.Count - 1)
            Else
                ListBox1.SelectedIndex = If(j < 1, inFiles.Count - 1, j - 1)
            End If
        End If
        LS("Выбран индекс: " & ListBox1.SelectedIndex)
        Button20_Stop(Nothing, Nothing)
        If Not InvalidState Then Button19_PlayPause(Nothing, Nothing)
        If CheckBox13.Checked Then
            If plFiles.Count < 2 Then
                Button18.Enabled = False
                LS("Кнопка задизейблена")
            End If
        End If
        If CheckBox16.Checked Then Scroll2(ti)
        ListBox1.Focus()
        LogLevel -= 1 : LS("Предыдущий: конец")
    End Sub
    Private Sub Button19_PlayPause(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click
        LS("Плей/пауза: начало @ " & (Button19.Text = "4").ToString) : LogLevel += 1
        If Button19.Text = "4" Then
            Dim i As Integer = GetCurrentPlayingIndex()
            LS("Play: Текущий индекс: " & i & ", LB.SI: " & ListBox1.SelectedIndex) : LogLevel += 1
            If i = -1 And inFiles.Count > 0 Then
                If ListBox1.SelectedIndex = -1 Or ListBox1.SelectedIndex >= inFiles.Count Then ListBox1.SelectedIndex = 0
                SmallPlayStub()
            ElseIf i <> -1 And (ListBox1.SelectedIndex = i Or ListBox1.SelectedIndex = -1 Or Paused) And CheckBox11.Checked = False Then
                LS("type: 2, paused is" & Paused.ToString)
                If ListBox1.SelectedIndex = -1 Then ListBox1.SelectedIndex = i
                If Paused Then
                    If Player.Play Then Button19.Text = ";"
                Else
                    SmallPlayStub()
                End If
            ElseIf i <> -1 And (ListBox1.SelectedIndex <> i Or CheckBox11.Checked) Then
                LS("type: 3")
                SmallPlayStub()
            End If
            Paused = False
            LogLevel -= 1 : LS("Play: конец внутреннего раздела")
        Else
            Paused = True
            LS("Pause...")
            If Player.Pause Then Button19.Text = "4"
        End If
        If Not (CheckBox16.Checked) And sender IsNot Nothing Then Scroll2(ListBox1.SelectedIndex)
        ListBox1.Refresh()
        ListBox1.Focus()
        LogLevel -= 1 : LS("Плей/пауза: конец")
    End Sub
    Private Sub Button20_Stop(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click
        LS("Стоп: Начало") : LogLevel += 1
        If Button20.Enabled Then
            LS("Запрос остановки...")
            InvalidState = True
            If Player.PlayStop() Then
                LS("Корректно!")
                InvalidState = False
                Button19.Text = "4"
                If inFiles.Count = 0 Then
                    Button19.Enabled = False
                    LS("Кнопка Плей задизейблена")
                    Button20.Enabled = False
                    LS("Кнопка Stop задизейблена")
                End If
                Paused = False
                Button20.Enabled = False
            End If
            If sender IsNot Nothing Then ListBox1.Refresh()
            ListBox1.Focus()
        End If
        LogLevel -= 1 : LS("Стоп: Конец")
    End Sub
    Private Sub Button21_Next(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button21.Click
        LS("Следующий: начало @" & (sender Is Nothing).ToString) : LogLevel += 1
        Dim i As Integer = GenNextPlayedIndex()
        LS("Индекс следующего файла: " & i) : LogLevel += 1
        If i = -1 Then
            If sender IsNot Nothing Then
                LS("inFiles.Count = " & inFiles.Count & "; Cb12.checked = " & CheckBox12.Checked.ToString)
                If inFiles.Count > 0 Then
                    If CheckBox12.Checked Then
                        If MsgBox("Все треки из плейлиста уже воспроизведены. Начать воспроизведение заново?", MsgBoxStyle.YesNo, "Очистка списка воспроизведённых треков") = MsgBoxResult.Yes Then
                            LS("<<Ok")
                            plFiles.Clear()
                            Button21_Next(Nothing, Nothing)
                        End If
                    Else
                        i = GetCurrentPlayingIndex()
                        LS("Возвращено: " & i)
                        i = If(i = inFiles.Count - 1 Or i = -1, 0, i + 1)
                        PlayNextStub(i)
                    End If
                End If
            End If
            ListBox1.Focus()
        Else
            PlayNextStub(i)
        End If
        LogLevel -= 2 : LS("Следующий: конец")
    End Sub
    Private Sub Button22_ShowLog(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button22.Click
        LS("-> Гуй: кнопка 22")
        LR()
        TabControl1.SelectTab(TabPage6Log)
    End Sub
    Private Sub Button23_NameGenCaller(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button23.Click
        LS("-> Гуй: кнопка 23")
        NameGen()
        Button23.Enabled = False
    End Sub
    Private Sub Button24_About(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button24.Click
        LS("-> Гуй: кнопка 24")
        Dim tLic2 As License.HumanizedLicInfo = License.GetLicense(False).Humanize
        About.Show()
        AddOwnedForm(About)
        About.__ExtInfo.Text = tLic2.ExtInfo
        About.__IssuedBy.Text = tLic2.IssuedBy
        About.__IssuedTo.Text = tLic2.UserName
        About.__hLasts.Text = tLic2.DaysValid
        About.__hActivated.Text = tLic2.ActivationDate
        About.__hLastUsed.Text = tLic2.LastUsageDate
        About.__hEndDate.Text = tLic2.EndDate
        About.__hDaysLeft.Text = tLic2.DaysLeft
        About._PVer.Text = "PlayListReNewer, v." & Application.ProductVersion
    End Sub
    Private Sub Button25_26_ShowDisksMenu(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button25.Click, Button26.Click
        LS("->GUI: button25 click")
        CM_Disk.Tag = CType(sender, Button).Tag
        CM_Disk.Show(CType(sender, Button), New Point(0, 0))
    End Sub
    Private Sub Button27_TotalIntegration(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button27.Click
        Static hDesktop As IntPtr, at As Point
        If Button27.Tag.ToString = "0" Then
            Dim prc() As Process = Process.GetProcesses
            Dim tTotalCmdHandle As IntPtr = Nothing
            For i As Integer = 0 To prc.Length - 1
                If prc(i).ProcessName.ToLower = "totalcmd" Then
                    tTotalCmdHandle = prc(i).MainWindowHandle
                    Exit For
                End If
            Next
            If tTotalCmdHandle <> Nothing Then
                at = Me.Location
                Dim myListHandle As IntPtr = FindWindowEx(tTotalCmdHandle, Nothing, "TMyListBox", Nothing)
                myListHandle = FindWindowEx(tTotalCmdHandle, myListHandle, "TMyListBox", Nothing)
                hDesktop = SetParent(Me.Handle, myListHandle)
                Me.Left = 0
                Me.Top = 0
                Timer2.Start()
                Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
                Me.WindowState = FormWindowState.Maximized
                Button27.Text = "Извлечься из TC"
                Button27.Tag = "1"
            Else
                MsgBox("Процесс TotalCmd.exe не найден или его главное окно недоступно!", MsgBoxStyle.Exclamation, "PLReNewer")
            End If
        Else
            Button27.Tag = "0"
            Button27.Text = "Внедриться в TC"
            Me.WindowState = FormWindowState.Normal
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
            SetParent(Me.Handle, hDesktop)
            Me.SetDesktopLocation(at.X, at.Y)
            AppActivate(Process.GetCurrentProcess.Id)
            Timer2.Stop()
        End If
    End Sub
    Private Sub Button28_SaveTag(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button28.Click
        LS("SaveTag: validating...") : LogLevel += 1
        Dim valid As Integer = 0
        Try
            Dim i As Integer
            If tagYear.Text <> "" Then i = CInt(tagYear.Text)
            valid = 1
            If tagTrackNumber.Text <> "" Then i = CByte(tagTrackNumber.Text)
            valid = 2
        Catch ex As Exception
            MsgBox("Значение поля «" + If(valid = 0, "год", "номер трека") + "» неверно!", MsgBoxStyle.Exclamation, "PlReNewer")
        End Try
        If valid = 2 Then
            Dim t As AudioTag
            Dim pm As ParseMode
            LS("SaveTag: begin, count =  " & ListBox1.SelectedIndices.Count)
            Dim tagStatusLine As String = tagStatus.Text
            For i As Integer = 0 To ListBox1.SelectedIndices.Count - 1
                pm = TagReader.TestTag(inFiles(ListBox1.SelectedIndices(i)), GetUserId3RequestLevel)
                If pm = ParseMode.MP3 Or (pm = ParseMode.None And CheckBox43.Checked) Then
                    t = TagReader.ReadMP3Tag(GetUserId3RequestLevel)
                    t.Normalize(False, False)
                    TagReader.CloseFile()
                    If CheckBox36.Checked Or Not (CheckBox36.Enabled) Then t.Artist = tagArtist.Text
                    If CheckBox37.Checked Or Not (CheckBox37.Enabled) Then t.Album = tagAlbum.Text
                    If CheckBox38.Checked Or Not (CheckBox38.Enabled) Then t.Year = tagYear.Text
                    If CheckBox39.Checked Or Not (CheckBox39.Enabled) Then t.Track = tagTrackName.Text
                    If CheckBox40.Checked Or Not (CheckBox40.Enabled) Then t.Number = tagTrackNumber.Text
                    If CheckBox41.Checked Or Not (CheckBox41.Enabled) Then t.Genre = tagGenre.Text
                    If CheckBox42.Checked Or Not (CheckBox42.Enabled) Then t.Comments = tagComment.Text
                    t.Empty = False
                    t.Valid = True
                    TagWriter.WriteMp3Tag(t, inFiles(ListBox1.SelectedIndices(i)), GetUserId3Encoding())
                    If i Mod 10 = 0 Then
                        tagStatus.Text = "Пишется тег " & i + 1 & " из " & ListBox1.SelectedIndices.Count
                        Application.DoEvents()
                    End If
                Else
                    TagReader.CloseFile()
                    Achtung("Saving tag failed: " & inFiles(ListBox1.SelectedIndices(i)) & " is not an MP3 file!")
                End If
            Next
            tagStatus.Text = "Сохранено"
            tagStatus.Font = New Font(tagStatus.Font, FontStyle.Bold)
            Dim TempTimer2 As New Timer() With {.Enabled = True, .Interval = 1000, .Tag = tagStatusLine}
            AddHandler TempTimer2.Tick, AddressOf Timer1
        Else
            LS("SaveTag: validation failed @ " & valid)
        End If
        LogLevel -= 1 : LS("SaveTag: finish")
    End Sub
    Private Sub RB_Bal_ValueChangedManually() Handles RB_Bal.ValueChangedManually
        Dim d As Double = RB_Bal.Value - RB_Bal.Maximum / 2
        Dim i As Integer = CInt(d * Math.Abs(d) / 200)
        LX("Balance: " & RB_Bal.Value & ", send: " & i)
        Player.SetBal(i)
    End Sub
    Private Sub RB_Pro_ValueChangedManually() Handles RB_Pro.ValueChangedManually
        LX("Progress: " & RB_Pro.Value)
        Player.SetPro(Math.Floor(RB_Pro.Value / 10))
    End Sub
    Private Sub RB_Vol_ValueChangedManually() Handles RB_Vol.ValueChangedManually
        LX("Volume: " & RB_Vol.Value)
        Player.SetVol(CInt(If(RB_Vol.Value = 0.0#, -10000, RB_Vol.Value - 5000.0#)))
    End Sub
    Private Sub TrackBar1_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TrackBar1.ValueChanged
        LX("TB1.value = " & TrackBar1.Value)
        Label4.Text = TrackBar1.Value.ToString
        Label5.Refresh()
    End Sub
    Private Sub TrackBar2_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TrackBar2.MouseUp
        ListBox1.Refresh()
    End Sub
    Private Sub TrackBar2_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar2.Scroll
        Static LR As Long = -1
        LX("TB2: " & TrackBar2.Value)
        If LR = -1 OrElse (Now.Ticks - LR) > 417000 Then
            LR = Now.Ticks
            ListBox1.Refresh()
        End If
    End Sub
    Private Sub RadioButton1_MB2PcsAndBack(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        LX("RB1: " & RadioButton1.Checked.ToString)
        LogLevel += 1
        If RadioButton1.Checked Then
            TrackBar1.Maximum *= 5
            TrackBar1.Value *= 5
            TrackBar1.Minimum *= 5
            TrackBar1.TickFrequency *= 5
        Else
            TrackBar1.Minimum = CInt(TrackBar1.Minimum / 5)
            TrackBar1.Value = CInt(TrackBar1.Value / 5)
            TrackBar1.Maximum = CInt(TrackBar1.Maximum / 5)
            TrackBar1.TickFrequency = CInt(TrackBar1.TickFrequency / 5)
        End If
        LogLevel -= 1
    End Sub
    Private Sub Radiobutton3_InOrOutFilesShow(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton3.CheckedChanged
        LS("RadioButton3/RadioButton4.checked changed") : LogLevel += 1
        ReFreshPlayListAuto() : LogLevel -= 1
    End Sub
    Private Sub RadioButton5_6_CheckedEnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton5.CheckedChanged, RadioButton5.EnabledChanged, RadioButton6.EnabledChanged, RadioButton6.CheckedChanged
        LS("5,6.c,e: " & RadioButton5.Checked.ToString & RadioButton5.Enabled.ToString & RadioButton6.Checked.ToString & RadioButton6.Enabled.ToString)
        ComboBox3.Enabled = RadioButton5.Enabled And RadioButton5.Checked
        TextBox2.Enabled = RadioButton6.Enabled And RadioButton6.Checked
        CheckBox5.Enabled = RadioButton6.Enabled And RadioButton6.Checked
        CheckBox14.Enabled = RadioButton6.Enabled And RadioButton6.Checked
        CheckBox15.Enabled = RadioButton6.Enabled And RadioButton6.Checked
        CheckBox19.Enabled = RadioButton6.Enabled And RadioButton6.Checked
        If (ComboBox3.Enabled And sender.Equals(RadioButton5)) Or (TextBox2.Enabled And sender.Equals(RadioButton6)) Then TextBox2_TextChanged(Nothing, Nothing)
    End Sub
    Private Sub ListBox1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListBox1.DoubleClick
        LS("Listbox1: doubleclick @ " & ListBox1.SelectedIndex & ", cb27=" & CheckBox27.Checked.ToString) : LogLevel += 1
        If ListBox1.SelectedIndex > -1 Then
            If CheckBox27.Checked Then
                Button20_Stop(Me, Nothing)
                If Not InvalidState Then
                    Dim i As IntPtr = API.ShellExecute(Me.Handle, "open", inFiles(ListBox1.SelectedIndex), "", "", SW_Commands.SW_NORMAL)
                    LS("Listbox1: doubleclick : ShellExecuteW вернул " & CInt(i))
                End If
            Else
                Button20_Stop(Nothing, Nothing)
                If Not InvalidState Then Button19_PlayPause(Nothing, Nothing)
            End If
        End If
        LogLevel -= 1 : LS("Listbox1: doubleclick finished")
    End Sub
    Private Sub ListBox1_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListBox1.DragDrop
        LS("LB1_DragDrop: Начало") : LogLevel += 1
        SaveUndo()
        Dim ti As Integer = ListBox1.SelectedIndex
        Dim path() As String = DirectCast(e.Data.GetData(Windows.Forms.DataFormats.FileDrop), String())
        Dim Wildcards() As String = GetWildcards()
        LS("path-объектов: " & path.Length & ", шаблонов: " & Wildcards.Length & ", статус шаблонов: " & CheckBox2.Checked.ToString)
        If path.Length > 0 Then
            LogLevel += 1
            If CheckBox8.Checked Then
                LS("+источник:") : LogLevel += 1
                ComboBox1.Items.Add(Strings.Left(path(0), Len(path(0)) - PathORezq(path(0), 0).Length))
                ComboBox1.SelectedIndex = ComboBox1.Items.Count - 1
                LogLevel -= 1
            End If
            For i As Integer = 0 To path.Length - 1
                LX("объект: " & path(i)) : LogLevel += 1
                If PathExists(path(i)) Then
                    LX("Разбор как папки...")
                    If CheckBox22.Checked Then
                        Dim col As Collections.ObjectModel.ReadOnlyCollection(Of String) = GetFilesMy(path(i), If(CheckBox2.Checked, Wildcards, New String() {"*.*"}))
                        LX("Чекбокс 22 отмечен, поехали проверять " & col.Count & " объектов")
                        For j As Integer = 0 To col.Count - 1
                            If Not inFiles.Contains(col(j)) Then inFiles.Add(col(j))
                        Next
                    Else
                        inFiles.AddRange(GetFilesMy(path(i), If(CheckBox2.Checked, Wildcards, New String() {"*.*"})))
                    End If
                Else
                    For j As Integer = 0 To Wildcards.Length - 1
                        If CheckBox2.Checked = False OrElse path(i).ToLower Like Wildcards(j).ToLower Then
                            LX("Добавляю или проверяю объект")
                            If CheckBox22.Checked = False OrElse inFiles.Contains(path(i)) = False Then inFiles.Add(path(i))
                            Exit For
                        ElseIf IsPlaylist(path(i)) Then
                            LX("Разбор как плейлиста...")
                            ParsePlaylist(path(i), False)
                            Exit For
                        End If
                    Next
                End If
                LogLevel -= 1
            Next
            If inFiles.Count > 0 Then
                LS("обновка outFiles & sizes...")
                ReFreshTail()
                If PathExists(ComboBox2.Text) Or IsPlaylist(ComboBox2.Text) Then Button6.Enabled = True
                If ti >= 0 Then Scroll2(ti)
            End If
            LogLevel -= 1
        End If
        LogLevel -= 1 : LS("LB1_DragDrop: Конец")
    End Sub
    Private Sub ListBox1_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListBox1.DragEnter
        If e.Data.GetDataPresent(Windows.Forms.DataFormats.FileDrop) Then e.Effect = DragDropEffects.Copy Else e.Effect = DragDropEffects.None
    End Sub
    Private Sub ListBox1_DrawItem(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles ListBox1.DrawItem
        If ListBox1.ItemHeight <> CInt(13 * e.Graphics.DpiY / 96) Then ListBox1.ItemHeight = CInt(13 * e.Graphics.DpiY / 96)
        LX("Listbox item drawer called, idx = " & e.Index)
        If e.Index > -1 And Me.WindowState <> FormWindowState.Minimized And e.Bounds.Width > 0 And e.Bounds.Height > 0 Then
            Dim bg As BufferedGraphics = BufferedGraphicsManager.Current.Allocate(e.Graphics, e.Bounds)
            bg.Graphics.Clear(If(W7.GlassMode, Color.Transparent, e.BackColor))
            If W7.GlassMode Then
                bg.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                bg.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.SystemDefault
                If CBool(e.State And DrawItemState.Selected) Then
                    bg.Graphics.FillRectangle(New Drawing2D.LinearGradientBrush(New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), Color.FromArgb(0, e.BackColor), Color.FromArgb(192, Color.Blue), Drawing2D.LinearGradientMode.Vertical), New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height))
                Else
                    bg.Graphics.FillRectangle(New Drawing2D.LinearGradientBrush(New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), Color.FromArgb(0, e.BackColor), Color.FromArgb(128, e.BackColor), Drawing2D.LinearGradientMode.Vertical), New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height))
                End If
            End If
            If e.Index < inFiles.Count Then
                bg.Graphics.DrawString(Format(e.Index + 1, "0000 ") & ListBox1.Items(e.Index).ToString, If(e.Index = PlayIdx Or (PlayIdx = -1 And inFiles(e.Index) = Label15.Text), New Font(ListBox1.Font, If(Player.IsPlaying, FontStyle.Bold, FontStyle.Regular)), ListBox1.Font), New Drawing.SolidBrush(e.ForeColor), e.Bounds.Left, e.Bounds.Top)
            Else
                bg.Graphics.DrawString(Format(e.Index + 1, "0000 ") & ListBox1.Items(e.Index).ToString, ListBox1.Font, New Drawing.SolidBrush(e.ForeColor), e.Bounds.Left, e.Bounds.Top)
            End If
            If W7.GlassMode Then
                Dim colorX As Color = Color.FromArgb(192, ListBox1.BackColor)
                bg.Graphics.FillRectangle(New Drawing2D.LinearGradientBrush(New Rectangle(TrackBar2.Value - 100, e.Bounds.Top, 100, e.Bounds.Height), Color.FromArgb(0, colorX), colorX, Drawing2D.LinearGradientMode.Horizontal), New Rectangle(TrackBar2.Value - 100, e.Bounds.Top, 100, e.Bounds.Height))
                bg.Graphics.FillRectangle(New SolidBrush(colorX), New Rectangle(TrackBar2.Value, e.Bounds.Top, e.Bounds.Width - TrackBar2.Value, e.Bounds.Height))
            Else
                bg.Graphics.FillRectangle(New Drawing2D.LinearGradientBrush(New Rectangle(TrackBar2.Value - 50, e.Bounds.Top, 50, e.Bounds.Height), Color.FromArgb(0, e.BackColor), Color.FromArgb(255, ListBox1.BackColor), Drawing2D.LinearGradientMode.Horizontal), New Rectangle(TrackBar2.Value - 50, e.Bounds.Top, 50, e.Bounds.Height))
                bg.Graphics.FillRectangle(New SolidBrush(ListBox1.BackColor), New Rectangle(TrackBar2.Value, e.Bounds.Top, e.Bounds.Width - TrackBar2.Value, e.Bounds.Height))
            End If
            bg.Graphics.DrawString(GetFullSizeMBStrict(inFilesSizes(e.Index)) & " MB", ListBox1.Font, New Drawing.SolidBrush(ListBox1.ForeColor), TrackBar2.Value, e.Bounds.Top)
            bg.Render()
            bg.Dispose()
        ElseIf ListBox1.Items.Count = 0 AndAlso (isPL() = False And ListBox1.Width > 0 And ListBox1.Height > 0) Then
            Dim bg As BufferedGraphics = BufferedGraphicsManager.Current.Allocate(e.Graphics, New Rectangle(New Point(0, 0), ListBox1.Size))
            Dim sf2 As New StringFormat()
            sf2.Alignment = StringAlignment.Center
            sf2.LineAlignment = StringAlignment.Center
            bg.Graphics.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
            bg.Graphics.SmoothingMode = Drawing2D.SmoothingMode.Default
            bg.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            bg.Graphics.Clear(If(Not W7.GlassMode, e.BackColor, Color.Transparent))
            Dim ssss As String = "Можно перетаскивать нужные файлы и папки в это окно вручную!" & Chr(13) & Chr(10) & "Или выберите источник файлов (поле ""Из""), место назначения (поле ""В""), выставьте нужное количество/объём файлов (ползунок под полями) и нажмите ""Сгенерировать""."
            bg.Graphics.DrawString(ssss, New Drawing.Font("Verdana", CSng(ListBox1.Width / 100 * ListBox1.Height / 200 + 20), FontStyle.Italic, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(128, Color.White)), New RectangleF(e.Bounds.Location, ListBox1.Size), sf2)
            bg.Graphics.DrawString(ssss, New Drawing.Font("Verdana", CSng(ListBox1.Width / 100 * ListBox1.Height / 200 + 20), FontStyle.Italic, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(128, Color.Black)), New RectangleF(e.Bounds.X + 2, e.Bounds.Y + 2, ListBox1.Width, ListBox1.Height), sf2)
            bg.Render()
            bg.Dispose()
        End If
    End Sub
    Private Sub ListBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ListBox1.KeyDown
        LS("На плейлист упала кнопка, а именно " & e.KeyCode.ToString) : LogLevel += 1
        If Not (Processing2 Or Processing) Then
            If TSMI_1_1.Enabled AndAlso e.Control Then
                Dim s As Object
                Dim oi, ni As Integer
                If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
                    e.SuppressKeyPress = True
                    oi = ListBox1.SelectedIndex
                    If e.KeyCode = Keys.Up Then
                        ni = If(oi = 0, ListBox1.Items.Count - 1, oi - 1)
                    Else
                        ni = If(oi = ListBox1.Items.Count - 1, 0, oi + 1)
                    End If
                    LS("Меняем индексы: " & oi & "->" & ni)
                    s = inFiles(oi)
                    inFiles.RemoveAt(oi)
                    inFiles.Insert(ni, s.ToString)
                    s = inFilesSizes(oi)
                    inFilesSizes.RemoveAt(oi)
                    inFilesSizes.Insert(ni, CInt(s))
                    If outFiles.Count > oi Then
                        s = outFiles(oi)
                        outFiles.RemoveAt(oi)
                        outFiles.Insert(ni, s.ToString)
                    End If
                    If e.KeyCode = Keys.Up And oi = 0 Then
                        For i As Integer = 0 To plFiles.Count - 1
                            plFiles(i) -= 1
                        Next
                        PlayIdx -= 1
                    ElseIf e.KeyCode = Keys.Down And ni = 0 Then
                        For i As Integer = 0 To plFiles.Count - 1
                            plFiles(i) += 1
                        Next
                        PlayIdx += 1
                    Else
                        For i As Integer = 0 To plFiles.Count - 1
                            If plFiles(i) = ni Then
                                plFiles(i) = oi
                            ElseIf plFiles(i) = oi Then
                                plFiles(i) = ni
                            End If
                        Next
                        If PlayIdx = oi Then
                            PlayIdx = ni
                        ElseIf PlayIdx = ni Then
                            PlayIdx = oi
                        End If
                    End If
                    s = ListBox1.Items(oi)
                    ListBox1.Items.RemoveAt(oi)
                    ListBox1.Items.Insert(ni, s)
                    ListBox1.SelectedIndex = ni
                End If
                If e.KeyCode = Keys.C And ListBox1.SelectedIndex > -1 Then
                    e.SuppressKeyPress = True
                    Try
                        Clipboard.Clear()
                        If e.Alt Then
                            Clipboard.SetText(inFiles(ListBox1.SelectedIndex).ToString)
                            LS("Скопировано в буффер: " & inFiles(ListBox1.SelectedIndex).ToString)
                        Else
                            Clipboard.SetText(ListBox1.Items(ListBox1.SelectedIndex).ToString)
                            LS("Скопировано в буффер: " & ListBox1.Items(ListBox1.SelectedIndex).ToString)
                        End If
                    Catch
                        Dim sText As String = If(e.Alt, inFiles(ListBox1.SelectedIndex).ToString, ListBox1.Items(ListBox1.SelectedIndex).ToString)
                        ClipboardCheckResult = 0
                        LS("Операция с буффером не удалась, в нём должен находиться текст: " & sText)
                        Dim nt As New Threading.Thread(AddressOf ClipBoardCheck)
                        nt.Start(sText)
                        LS("Запущен поток")
                        While ClipboardCheckResult = 0
                            Threading.Thread.Sleep(100)
                        End While
                        If ClipboardCheckResult = 1 Then
                            LS("Скопировано в буффер: " & sText & ". Скопировано в буффер: операция завершена, однако система вернула ошибочный код :) Вероятно, это первая операция такого рода в текущем сеансе.")
                        Else
                            LS("Операция завершилась неудачей :(")
                            MsgBox("Ошибка при работе с буфером обмена! Попробуйте ещё, сейчас должно получиться :)", MsgBoxStyle.Exclamation, "Ошибка при работе с буфером обмена")
                        End If
                    End Try
                End If
            Else
                If e.KeyCode = Keys.Space And Button19.Enabled Then Button19_PlayPause(Nothing, Nothing)
                If e.KeyCode = Keys.OemOpenBrackets And Button18.Enabled Then Button18_Previous(Nothing, Nothing)
                If e.KeyCode = Keys.OemCloseBrackets And Button21.Enabled Then Button21_Next(Nothing, Nothing)
                If ListBox1.SelectedIndex > -1 Then
                    If e.KeyCode = Keys.Enter Then ListBox1_DoubleClick(sender, Nothing)
                    If e.KeyCode = Keys.Delete Then ContextMenu_Edit_ItemClicked(Nothing, New ToolStripItemClickedEventArgs(TSMI_1_1))
                    If e.KeyCode = Keys.Insert Then ContextMenu_Edit_ItemClicked(Nothing, New ToolStripItemClickedEventArgs(TSMI_1_2))
                End If
            End If
            If e.KeyCode = Keys.Home And e.Control And TSMI_5_1.Enabled Then TSMI_5_1_Click(Nothing, Nothing)
            If e.KeyCode = Keys.End And e.Control And TSMI_5_2.Enabled Then TSMI_5_2_Click(Nothing, Nothing)
            If e.KeyCode = Keys.Z And e.Control And Button16.Enabled Then Button16_Undo(Nothing, Nothing)
            LogLevel -= 1 : LS("Конец обработчика кнопок")
        End If
    End Sub
    Private Sub ListBox1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListBox1.MouseEnter
        ListBox1.Focus()
    End Sub
    Private Sub ListBox1_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseMove
        Static x, y As Integer
        If ListBox1.Items.Count = 0 Then
            If e.Button = Windows.Forms.MouseButtons.None Then
                x = e.X
                y = e.Y
            ElseIf e.Button = Windows.Forms.MouseButtons.Left Then
                Me.SetBounds(Me.Left + (e.X - x), Me.Top + (e.Y - y), 0, 0, BoundsSpecified.Location)
            End If
        End If
    End Sub
    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        If Not ProcessingScroll Then
            If ListBox1.SelectedIndex <> -1 Then
                TSMI_1_1.Enabled = True
                TSMI_1_2.Enabled = True
                If ListBox1.SelectedIndex < inFiles.Count Then LX("ListBox1_SelectedIndexChanged: " & ListBox1.SelectedIndex & ", value_iF: " & inFiles(ListBox1.SelectedIndex))
                'Button28.Enabled = If(plFiles.Count > 0, Not (ListBox1.SelectedIndices.Contains(plFiles(plFiles.Count - 1))), True)
                Button19.Enabled = True
                If Object.ReferenceEquals(TabControl1.SelectedTab, TabPage8TagEditor) Then FillTagFromSelection()
            Else
                TSMI_1_1.Enabled = False
                TSMI_1_2.Enabled = False
                Button28.Enabled = False
                LX("ListBox1_SelectedIndexChanged: -1")
            End If
            CheckBox36.Enabled = (ListBox1.SelectedIndices.Count > 1)
            CheckBox37.Enabled = (ListBox1.SelectedIndices.Count > 1)
            CheckBox38.Enabled = (ListBox1.SelectedIndices.Count > 1)
            CheckBox39.Enabled = (ListBox1.SelectedIndices.Count > 1)
            CheckBox40.Enabled = (ListBox1.SelectedIndices.Count > 1)
            CheckBox41.Enabled = (ListBox1.SelectedIndices.Count > 1)
            CheckBox42.Enabled = (ListBox1.SelectedIndices.Count > 1)
        End If
    End Sub
    Private Sub CheckBox1_CheckedEnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged, CheckBox1.EnabledChanged
        LS("CB1.c,e: " & CheckBox1.Checked.ToString & CheckBox1.Enabled.ToString) : LogLevel += 1
        RadioButton5.Enabled = CType(sender, CheckBox).Checked And CType(sender, CheckBox).Enabled
        RadioButton6.Enabled = CType(sender, CheckBox).Checked And CType(sender, CheckBox).Enabled
        If CType(sender, CheckBox).Checked = False Then TextBox2_TextChanged(Nothing, Nothing)
        LogLevel -= 1
    End Sub
    Private Sub CheckBox3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked Then
            Button11.Visible = False
            TrackBar1.Left = 6
            TrackBar1.Width = GroupBox2.Width - 111
        Else
            Button11.Visible = True
            TrackBar1.Left = 22
            TrackBar1.Width = GroupBox2.Width - 127
        End If
    End Sub
    Private Sub CheckBox4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked Then LR()
    End Sub
    Private Sub CheckBox5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox5.CheckedChanged
        If CheckBox5.Checked Then
            CM_Legend.Show(CheckBox5, New Point(CheckBox5.Width, 0))
        End If
    End Sub
    Private Sub CheckBox6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox6.CheckedChanged
        LxB = CheckBox6.Checked
        If LxB Then LR()
    End Sub
    Private Sub CheckBox7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox7.CheckedChanged
        TextBox4.Enabled = CheckBox7.Checked
        Button17.Enabled = CheckBox7.Checked
    End Sub
    Private Sub CheckBox9_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox9.CheckedChanged
        CheckBox10.Enabled = CheckBox9.Checked
        CheckBox29.Enabled = CheckBox9.Checked
    End Sub
    Private Sub CheckBox10_CheckedEnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox10.CheckedChanged, CheckBox10.EnabledChanged
        NumericUpDown1.Enabled = CheckBox10.Checked And CheckBox10.Enabled
    End Sub
    Private Sub CB10_29_EnblCh_PlayerChecksEnabledHandler(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBox10.EnabledChanged, CheckBox29.EnabledChanged
        If Not W7.GlassMode And Not Compot Then CType(sender, CheckBox).UseCompatibleTextRendering = Not CType(sender, CheckBox).Enabled
    End Sub
    Private Sub CheckBox13_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox13.CheckedChanged
        If (CheckBox13.Checked And plFiles.Count < 2) Or (Not (CheckBox13.Checked) And inFiles.Count = 0) Then
            Button18.Enabled = False
        Else
            Button18.Enabled = True
        End If
    End Sub
    Private Sub CheckBox18_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox18.CheckedChanged
        CheckBox20.Enabled = CheckBox18.Checked
    End Sub
    Private Sub CheckBox17_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox17.CheckedChanged
        Main.F1Checkbox17 = CheckBox17.Checked
    End Sub
    Private Sub CheckBox19_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox19.CheckedChanged
        If CheckBox19.Checked = False And Button23.Enabled = True Then Button23_NameGenCaller(sender, e)
    End Sub
    Private Sub CheckBox20_CheckedEnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox20.CheckedChanged, CheckBox20.EnabledChanged
        CheckBox21.Enabled = CheckBox20.Enabled And CheckBox20.Checked
    End Sub
    Private Sub CheckBox23_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox23.CheckedChanged
        NumericUpDown4.Enabled = CheckBox23.Checked
    End Sub
    Private Sub CheckBox25_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox25.CheckedChanged
        CheckBox26.Enabled = CheckBox25.Checked
    End Sub
    Private Sub CheckBox30_CheckedEnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox30.CheckedChanged, CheckBox30.EnabledChanged
        If CheckBox30.Enabled And CheckBox30.Checked Then
            CheckBox32.Checked = True
            CheckBox32.Enabled = False
        Else
            CheckBox32.Enabled = True
        End If
    End Sub
    Private Sub CheckBox33_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox33.CheckedChanged
        SizesUpdate()
    End Sub
    Private Sub CheckBox36_42_CheckedEnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox36.CheckedChanged, CheckBox37.CheckedChanged, CheckBox38.CheckedChanged, CheckBox39.CheckedChanged, CheckBox40.CheckedChanged, CheckBox41.CheckedChanged, CheckBox42.CheckedChanged, CheckBox36.EnabledChanged, CheckBox37.EnabledChanged, CheckBox38.EnabledChanged, CheckBox39.EnabledChanged, CheckBox40.EnabledChanged, CheckBox41.EnabledChanged, CheckBox42.EnabledChanged
        Dim textboxes() As Control = {tagArtist, tagAlbum, tagYear, tagTrackName, tagTrackNumber, tagGenre, tagComment}
        Dim sender_i As CheckBox = CType(sender, CheckBox)
        textboxes(CInt(sender_i.Tag)).Enabled = (Not sender_i.Enabled) OrElse sender_i.Checked
    End Sub
    Private Sub ComboBox1_2_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.GotFocus, ComboBox2.GotFocus
        CType(sender, ComboBox).Select(CType(sender, ComboBox).Text.Length, 0)
    End Sub
    Private Sub ComboBox1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.TextChanged
        inFilesGlobal.Clear()
        If PathExists(ComboBox1.Text) Then
            Button8.Enabled = True
        Else
            Button8.Enabled = False
        End If
        LS("В первом комбобоксе изменился текст, глобальный список очищен, проверка:" & Button8.Enabled.ToString)
    End Sub
    Private Sub ComboBox2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox2.TextChanged
        If Not init Then
            If sender IsNot Nothing Then LS("Combobox2: текст изменился (" & ComboBox2.Text & ")") Else LS("Обновление по CB2_TC")
            LogLevel += 1
            Button6.Enabled = False
            If PathExists(ComboBox2.Text) Then
                LS("Папка существует")
                Button7.Enabled = True
                If CheckBox1.Enabled Then TextBox2_TextChanged(Nothing, Nothing) Else CheckBox1.Enabled = True
                Button10.Enabled = True
                RadioButton4.Enabled = True
                If inFiles.Count > 0 Then Button6.Enabled = True
                Label7.ForeColor = Drawing.SystemColors.ControlText
                Button6.Text = "Скопировать"
            Else
                LS("Папка не существует")
                Button7.Enabled = False
                CheckBox1.Enabled = False
                Button10.Enabled = False
                RadioButton4.Enabled = False
                RadioButton3.Checked = True
                Label7.ForeColor = Color.Red
                If IsPlaylist(ComboBox2.Text) Then
                    If inFiles.Count > 0 Then Button6.Enabled = True
                    TextBox3.Text = "В качестве назначения указан плейлист"
                    Button6.Text = "Сохранить"
                Else
                    Button6.Text = "(запрещено)"
                    TextBox3.Text = "Указано несуществующее назначение"
                End If
            End If
            If CheckBox3.Checked Then Button11_GetFreeSpace(Nothing, Nothing)
            ComboBox2.Focus()
            LogLevel -= 1 : LS("Combobox2_TC: завершён")
        End If
    End Sub
    Private Sub ComboBox3_SelectedIndexChanged_checkboxes1415_checkedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox3.SelectedIndexChanged, CheckBox14.CheckedChanged, CheckBox15.CheckedChanged
        LS("-> Третий комбобокс и чекбоксы 14, 15-")
        NameGen()
    End Sub
    Private Sub ComboBox5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox5.SelectedIndexChanged
        TextBox2_TextChanged(Nothing, Nothing)
    End Sub
    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged, ComboBox4.SelectedIndexChanged
        LS("В поле фильтра / спика пустого д-я изменился текст, глобальный список очищен! " & TextBox1.Text & ", " & ComboBox4.Text)
        inFilesGlobal.Clear()
    End Sub
    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        If Not init And (sender Is Nothing Or TextBox2.Text <> LastText) Then
            If TextBox2.Text.Contains("*") Or TextBox2.Text.Contains("?") Then
                TextBox2.Text = LastText
            Else
                Dim i As Integer = ListBox1.SelectedIndex
                LS("TextBox2.Text=" & TextBox2.Text & If(sender Is Nothing, ", ex", ", in") & "ternal call") : LogLevel += 1
                TextBox3.Text = GetCombobox2Text(0) & ParseWildcard(TextBox2.Text, New String() {"Artist", "Album", "Year", "Track", "Comment", "Number", "Genre", "ext"})
                If Not CheckBox19.Checked Or sender Is Nothing Then NameGen() Else Button23.Enabled = True
                If i > -1 AndAlso i < ListBox1.Items.Count Then Scroll2(i)
                LastText = TextBox2.Text : LogLevel -= 1
            End If
        End If
    End Sub
    Private Sub TextBox5_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox5.KeyDown
        e.SuppressKeyPress = True
        Select Case e.KeyCode
            Case Keys.Up, Keys.Left
                If Not e.Alt Then
                    ListBox1.SelectedIndex = GetNextLine(If(ListBox1.SelectedIndex > 0, ListBox1.SelectedIndex - 1, ListBox1.Items.Count - 1), SearchVector.Up)
                ElseIf e.KeyCode = Keys.Left Then
                    TextBox5.SelectionStart = If(TextBox5.SelectionStart > 0, TextBox5.SelectionStart - 1, TextBox5.TextLength)
                End If
            Case Keys.Down, Keys.Right
                If Not e.Alt Then
                    ListBox1.SelectedIndex = GetNextLine(If(ListBox1.SelectedIndex < ListBox1.Items.Count - 1, ListBox1.SelectedIndex + 1, 0), SearchVector.Down)
                ElseIf e.KeyCode = Keys.Right Then
                    TextBox5.SelectionStart = If(TextBox5.SelectionStart < TextBox5.TextLength, TextBox5.SelectionStart + 1, 0)
                End If
            Case Keys.Enter
                ListBox1_DoubleClick(sender, Nothing)
            Case Else
                e.SuppressKeyPress = False
        End Select
    End Sub
    Private Sub TextBox5_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox5.TextChanged
        ListBox1.SelectedIndex = GetNextLine(0, SearchVector.Down)
    End Sub
    Private Sub ContextMenu_Edit_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles CM_Edit.ItemClicked
        SaveUndo()
        LS("CM_Edit_ICl: Начало@ " & CInt(e.ClickedItem.Tag)) : LogLevel += 1
        If CInt(e.ClickedItem.Tag) < 3 Then
            Dim i As Integer = ListBox1.SelectedIndex
            LS("Удаляется: " & inFiles(i))
            If i < outFiles.Count Then outFiles.RemoveAt(i)
            inFiles.RemoveAt(i)
            inFilesSizes.RemoveAt(i)
            If plFiles.Contains(i) Then plFiles.RemoveAt(plFiles.IndexOf(i))
            For k As Integer = 0 To plFiles.Count - 1
                If plFiles(k) > i Then plFiles(k) -= 1
            Next
            If PlayIdx = i Then
                PlayIdx = -1
            ElseIf PlayIdx > i Then
                PlayIdx -= 1
            End If
            If CInt(e.ClickedItem.Tag) = 2 Then
                If RadioButton2.Checked And inFilesGlobal.Count <= TrackBar1.Value Then
                    MsgBox("Замена невозможна: файлов на выбор меньше, чем запрашивается. Запись, тем не менее, была удалена. Для её восстановления в плейлисте нажмите ""Сгенерировать"".", MsgBoxStyle.Exclamation, "Ошибка")
                    LS("Возникла ситуёвина с кодом 1")
                Else
                    LS("Cитуёвина с кодом 1 не возникла")
                    GlobalSourceGen()
                    LongSourceGen(True, False)
                    NameGen()
                End If
            End If
            ReFreshPlayListAuto()
            Label5.Text = GetFullSizeMbWithSuffix()
            Dim j As Integer = If(i < ListBox1.Items.Count, i, ListBox1.Items.Count - 1)
            If j > -1 Then Scroll2(j)
        ElseIf CInt(e.ClickedItem.Tag) = 3 Then
            ClearLists(True)
        Else
            If MsgBox("Внимание! Эта функция удаляет ВСЕ файлы, находящиеся сейчас в треклисте, из их источников. Для очистки папки-назначения воспользуйтесь кнопкой ""Очистить цель"". Вы действительно хотите продолжить?", MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo, "Внимание!") = MsgBoxResult.Yes Then
                PLPleaseWait_Init("Удаляются файлы...", 0, inFiles.Count, 0)
                plFiles.Clear()
                LS("Начинаю удаление, файлов в inFiles: " & inFiles.Count)
                For i As Integer = inFiles.Count - 1 To 0 Step -1
                    Try
                        IO.File.Delete(inFiles(i))
                        inFiles.RemoveAt(i)
                        inFilesSizes.RemoveAt(i)
                        If i < outFiles.Count Then outFiles.RemoveAt(i)
                        LX("deleted ok: " & i)
                    Catch ex As Exception
                        Achtung("Не удалось удалить файл """ & inFiles(i) & """. " & ex.Message)
                    End Try
                    PLPleaseWait_ValuePlusPlus(True)
                Next
                PLPleaseWait_Halt()
            Else
                LS("<-answer:no")
            End If
        End If
        LogLevel -= 1 : LS("CM_Edit_ICl: Конец@ " & CInt(e.ClickedItem.Tag))
    End Sub
    Private Sub ContextMenu_Delete_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles CM_Delete.ItemClicked
        LS("CM_Delete_ICl: начало @" & CInt(e.ClickedItem.Tag)) : LogLevel += 1
        If MsgBox("Внимание: эта функция удаляет ВСЕ (а не только музыкальные) файлы и/или папки в объекте-приёмнике. Продолжить?", MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo, "Внимание!") = MsgBoxResult.No Then Exit Sub
        If CInt(e.ClickedItem.Tag) <> 1 Then
            LS("файлы...")
            Dim delFiles() As String = IO.Directory.GetFiles(ComboBox2.Text, "*.*", IO.SearchOption.TopDirectoryOnly)
            PLPB2Val = 0
            PLPB2Max = 2
            PLPleaseWait_Init("Удаляются файлы...", 0, delFiles.Length, 0)
            LS("всего " & delFiles.Length) : LogLevel += 1
            For i As Integer = 0 To delFiles.Length - 1
                Try
                    IO.File.Delete(delFiles(i))
                    LX("Файл " & delFiles(i) & " был удалён!")
                Catch ex As Exception
                    Achtung("Файл № " & i & ", «" & delFiles(i) & "», удалить не удалось: " & ex.Message)
                End Try
                PLPleaseWait_ValuePlusPlus(True)
                Application.DoEvents()
            Next
            LogLevel -= 1 : LS("...есть")
        End If
        PLPB2Val = 1
        If CInt(CInt(e.ClickedItem.Tag)) > 0 Then
            LS("папки...")
            Dim delFolders() As String = IO.Directory.GetDirectories(ComboBox2.Text, "*", IO.SearchOption.TopDirectoryOnly)
            PLPleaseWait_Init("Удаляются папки...", 0, delFolders.Length, 0)
            LS("всего " & delFolders.Length) : LogLevel += 1
            For i As Integer = 0 To delFolders.Length - 1
                Try
                    IO.Directory.Delete(delFolders(i), True)
                    LX("Папка " & delFolders(i) & " удалена!")
                Catch ex As Exception
                    Achtung("Папку № " & i & ", «" & delFolders(i) & "», удалить не удалось: " & ex.Message)
                End Try
                PLPleaseWait_ValuePlusPlus(True)
                Application.DoEvents()
            Next
            LogLevel -= 1 : LS("есть")
        End If
        PLPB2Val = 2
        Button11_GetFreeSpace(Nothing, Nothing)
        PLPleaseWait_Halt()
        LogLevel -= 1 : LS("CM_Delete_ICl: конец")
    End Sub
    Private Sub ContextMenu_Legend_Closed(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripDropDownClosedEventArgs) Handles CM_Legend.Closed
        CheckBox5.Checked = False
    End Sub
    Private Sub ContextMenu_Legend_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles CM_Legend.ItemClicked
        TextBox2.Text &= Mid(e.ClickedItem.Text, Len(e.ClickedItem.Text) - 2, 2)
    End Sub
    Private Sub ContextMenu_Generate_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles CM_Generate.ItemClicked
        LS("Генератор: начало@" & CInt(e.ClickedItem.Tag))
        If Processing Then Exit Sub
        CM_Generate.Hide()
        If Not (PathExists(ComboBox1.Text) Or IO.File.Exists(ComboBox1.Text)) Then
            MsgBox("Источник файлов, указанный в поле «Из», не существует!", MsgBoxStyle.Exclamation, "Ошибка")
            LS("Генератор: источник не существует: " & ComboBox1.Text)
            Exit Sub
        End If
        If CInt(e.ClickedItem.Tag) <> 3 AndAlso ((Not PathExists(ComboBox2.Text)) And Not IsPlaylist(ComboBox2.Text)) Then
            MsgBox("Плейлист или папка - цель для копирования файлов,- указанная в поле «В» не существует!", MsgBoxStyle.Exclamation, "Ошибка")
            LS("Генератор: назначения не существует: " & ComboBox2.Text)
            Exit Sub
        End If
        If CInt(e.ClickedItem.Tag) <> 3 AndAlso ((TrackBar1.Value < 5 And RadioButton1.Checked) Or (RadioButton2.Checked And TrackBar1.Value = 0)) AndAlso MsgBox("Задан очень маленький лимит генерации файлов или его вовсе не задано. Вы уверены, что нужно продолжать?", MsgBoxStyle.Information Or MsgBoxStyle.YesNo, "Проверьте лимит генерации!") = MsgBoxResult.No Then Exit Sub
        Processing = True
        MainGen(e)
    End Sub
    Private Sub ContextMenu_Disk_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles CM_Disk.ItemClicked
        LS("-> CM_D_IC @ tag=" & CM_Disk.Tag.ToString & ", e.ci.text=" & e.ClickedItem.Text)
        If CM_Disk.Tag.ToString = "1" Then
            ComboBox1.Text = e.ClickedItem.Text.Split(New String() {"} "}, StringSplitOptions.None)(1)
            If ComboBox1.Items.IndexOf(ComboBox1.Text) = -1 Then ComboBox1.Items.Add(ComboBox1.Text)
        Else
            ComboBox2.Text = e.ClickedItem.Text.Split(New String() {"} "}, StringSplitOptions.None)(1)
            If ComboBox2.Items.IndexOf(ComboBox2.Text) = -1 Then ComboBox2.Items.Add(ComboBox2.Text)
        End If
    End Sub
    Private Sub ContextMenu_Disk_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles CM_Disk.Opening
        LS("CM_Disk_Opening: begin") : LogLevel += 1
        CM_Disk.Items.Clear()
        Dim d As UIO.DriveInfo() = UIO.DriveInfo.GetDrives()
        LS("Disks: " & d.Length)
        For Each di In d
            LX("Drive: " & di.Name & ", type: " & di.DriveType & ", ready: " & di.IsReady)
            If (di.DriveType = IO.DriveType.Removable Or di.DriveType = IO.DriveType.Fixed) AndAlso di.IsReady Then
                CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name)
                If UIO.Directory.Exists(di.Name & "Audio") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Audio\")
                If UIO.Directory.Exists(di.Name & "Music") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Music\")
                If UIO.Directory.Exists(di.Name & "MP3") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "MP3\")
                If UIO.Directory.Exists(di.Name & "Sounds") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Sounds\")
                If UIO.Directory.Exists(di.Name & "Music\Audio") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Music\Audio\")
                If UIO.Directory.Exists(di.Name & "Music\MP3") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Music\MP3\")
                If UIO.Directory.Exists(di.Name & "Music\Sounds") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Music\Sounds\")
                If UIO.Directory.Exists(di.Name & "Audio\Music") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Audio\Music\")
                If UIO.Directory.Exists(di.Name & "Audio\MP3") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Audio\MP3\")
                If UIO.Directory.Exists(di.Name & "Audio\Sounds") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Audio\Sounds\")
                If UIO.Directory.Exists(di.Name & "MP3\Audio") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "MP3\Audio\")
                If UIO.Directory.Exists(di.Name & "MP3\Music") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "MP3\Music\")
                If UIO.Directory.Exists(di.Name & "MP3\Sounds") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "MP3\Sounds\")
                If UIO.Directory.Exists(di.Name & "Sounds\Audio") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Sounds\Audio\")
                If UIO.Directory.Exists(di.Name & "Sounds\MP3") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Sounds\MP3\")
                If UIO.Directory.Exists(di.Name & "Sounds\Music") Then CM_Disk.Items.Add("{" & di.VolumeLabel & "} " & di.Name & "Sounds\Music\")
            End If
        Next
        LS("Added " & CM_Disk.Items.Count & " items")
        LogLevel += 1
        For i As Integer = 0 To CM_Disk.Items.Count - 1
            LX(CM_Disk.Items(i).Text)
        Next
        LogLevel -= 1
        LX("Listing complete")
        e.Cancel = False
        LogLevel -= 1 : LS("CM_Disk_Opening: end")
    End Sub
    Private Sub TSMI_5_1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TSMI_5_1.Click
        LS("Sort: Начало @" & If(RadioButton3.Checked, "источник", "цель")) : LogLevel += 1
        If RadioButton3.Checked Then
            Dim ts As String
            If ListBox1.SelectedIndex = -1 Then ts = "::" Else ts = inFiles(ListBox1.SelectedIndex)
            PLPleaseWait_Init("Пожалуйста подождите...")
            SaveUndo()
            ListBox1.Sorted = True
            ListBox1.Sorted = False
            LS("plFiles.Count=" & plFiles.Count)
            If plFiles.Count > 0 Then
                LogLevel += 1
                Dim tArr(plFiles.Count - 1) As String
                LS("Первый цикл")
                LogLevel += 1
                For i As Integer = 0 To plFiles.Count - 1
                    tArr(i) = inFiles(plFiles(i))
                    LX(tArr(i))
                Next
                LogLevel -= 1
                inFiles.Sort()
                LS("Второй цикл")
                LogLevel += 1
                For i As Integer = 0 To plFiles.Count - 1
                    plFiles(i) = inFiles.IndexOf(tArr(i))
                    LX(plFiles(i).ToString)
                Next
                LogLevel -= 2
            Else
                inFiles.Sort()
            End If
            PlayIdx = inFiles.IndexOf(Label15.Text)
            ReFreshTail()
            If ts <> "::" Then Scroll2(inFiles.IndexOf(ts))
        Else
            If MsgBox("Сортировка в режиме отображения цели невозможна. Переключиться в режим отображения источника и отсортировать?", MsgBoxStyle.Information Or MsgBoxStyle.YesNo, "Отсортировать?") = MsgBoxResult.Yes Then
                RadioButton3.Checked = True
                TSMI_5_1_Click(sender, e)
            End If
        End If
        LogLevel -= 1 : LS("Sort: Конец")
    End Sub
    Private Sub TSMI_5_2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TSMI_5_2.Click
        LS("Shuffle: начало, файлов " & inFiles.Count) : LogLevel += 1
        Dim ts As String
        If ListBox1.SelectedIndex = -1 Then ts = "::" Else ts = inFiles(ListBox1.SelectedIndex)
        Dim ps(inFiles.Count - 1) As Integer ' Массив старых индексов по новым адресам
        Dim it, i As Integer, b As Boolean
        i = 0
        LS("Генерация общей карты...") : LogLevel += 1
        'Сначала сделаем общую карту - какой пункт куда должен упасть
        While i < inFiles.Count
            it = CInt(Rnd() * (inFiles.Count - 1)) ' i - новый номер файла № it
            b = False
            If i > 0 Then
                For j As Integer = 0 To i - 1
                    If ps(j) = it Then b = True 'Моя интерпретация функции contains()
                Next
            End If
            If Not b Then 'Т.е., если Not Contains, то добавляем
                ps(i) = it
                i += 1
                LX(i & "=" & Chr(9) & it)
            End If
            'В противном случае ищем новое число
        End While
        LogLevel -= 1
        SaveUndo()
        LS("Копировавние Исходных и Ихразмеров...") : LogLevel += 1
        Dim aInFiles(inFiles.Count - 1) As String, aInFilesSizes(inFiles.Count - 1) As Long
        'Массивы того, что имеет смысл переставлять вручную
        For j As Integer = 0 To inFiles.Count - 1
            aInFiles(j) = inFiles(ps(j))
            aInFilesSizes(j) = inFilesSizes(ps(j))
            i = plFiles.IndexOf(ps(j))
            If i <> -1 Then
                plFiles(i) = j
                LX("plFiles: индекс" & i & " установлен в " & it)
            End If
        Next
        LogLevel -= 1 : LS("Замена Исходных и Ихразмеров...")
        inFiles.Clear()
        inFilesSizes.Clear()
        inFiles.AddRange(aInFiles)
        inFilesSizes.AddRange(aInFilesSizes)
        PlayIdx = inFiles.IndexOf(Label15.Text)
        NameGen()
        If ts <> "::" Then Scroll2(inFiles.IndexOf(ts))
        LogLevel -= 1 : LS("Shuffle: конец")
    End Sub
    Private Sub Label12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label12.Click
        Scroll2(GetCurrentPlayingIndex())
    End Sub
    Private Sub Label12_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Label12.MouseMove
        If Label15.Text <> "" And GetCurrentPlayingIndex() > -1 Then Label12.Cursor = Cursors.Hand Else Label12.Cursor = Cursors.Arrow
    End Sub
    Private Sub Labels_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Label15.TextChanged, Label13.TextChanged
        ToolTip1.SetToolTip(CType(sender, Label), CType(sender, Label).Text)
    End Sub
    Private Sub Label5_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Label5.Paint
        Dim myRect As New Rectangle(0, 0, Label5.Width, Label5.Height)
        Dim mySize As Size = e.Graphics.MeasureString(Label5.Text, Label5.Font, Label5.Width).ToSize
        Dim myRect1 As New Rectangle(0, 0, myRect.Width, myRect.Height - mySize.Height)
        Dim myRect2 As New Rectangle(0, myRect.Height - mySize.Height, myRect.Width, mySize.Height)
        Dim baseColor As Color = If(GetFullSizeMBStrict() < GetTrackBar1ValueMB() Or GetFullSize() = 0, Color.Green, Color.Red)
        'If W7.w7 Then
        '	If W7.GlassMode Then e.Graphics.Clear(Color.Transparent)
        '	e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
        '	e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.None
        'Else
        'If Not W7.w7 Then e.Graphics.CompositingMode = Drawing2D.CompositingMode.SourceCopy
        e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
        'End If
        If GetFullSize() > 0 Then e.Graphics.FillRegion( _
         New Drawing2D.LinearGradientBrush(myRect2, If(W7.GlassMode, Color.Transparent, GetSuperParentBackColor(Label5)), If(W7.GlassMode, Color.FromArgb(192, baseColor), baseColor), Drawing2D.LinearGradientMode.Vertical), _
         New Region(RoundRectangle(If(baseColor = Color.Red, myRect2, New Rectangle(myRect2.X, myRect2.Y, CInt(GetFullSizeMBStrict() / GetTrackBar1ValueMB() * myRect2.Width), myRect2.Height)), 10)))
        e.Graphics.DrawString( _
         "Объём треклиста:", _
         New Font(Label5.Font.FontFamily, 7, FontStyle.Italic, GraphicsUnit.Point), _
         New SolidBrush(Label22.ForeColor), _
         0, _
         0)
        Dim MySF As New Drawing.StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Far}
        'myRect.Offset(New Point(If(W7.w7, Label5.Width Mod 2 - 1, If(e.Graphics.DpiY = 96, Label5.Width Mod 2 - 1, Label5.Width Mod 2 - 2)), If(e.Graphics.DpiY = 96, 1, 0)))
        e.Graphics.DrawString(Label5.Text, Label5.Font, New SolidBrush(Label22.ForeColor), myRect, MySF)
    End Sub
    Private Sub NumericUpDown2_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown2.ValueChanged
        Label16.Text = "ГБ (файлов: " & CInt(NumericUpDown2.Value * 1024 / 5) & ")"
        LX("NUD2 утсановлен в " & NumericUpDown2.Value)
    End Sub
    Private Sub NumericUpDown4_ValueEnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown4.ValueChanged, NumericUpDown4.EnabledChanged
        TextBox2_TextChanged(Nothing, Nothing)
    End Sub
    Private Sub NotifyIcon1_BalloonTipClosed(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.BalloonTipClosed
        NotifyIcon1.Visible = False
    End Sub
    Private Sub NotifyIcon1_BalloonTipclicked(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.BalloonTipClicked, NotifyIcon1.DoubleClick, NotifyIcon1.Click
        FormMDX.Show()
        NotifyIcon1.Visible = False
    End Sub
    Private Sub NumericUpDown1_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles NumericUpDown1.EnabledChanged
        LinkLabel1.Enabled = NumericUpDown1.Enabled
    End Sub
    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        LS("Авто-кроссфейдер: начало") : LogLevel += 1
        If MsgBox("Эта функция автоматически подбирает значения кроссфейдера в зависимости от производительности вашей системы. Необходимо будет выбрать 5 или больше файлов, которые способен воспроизвести PLRenewer. Типы файлов подставляются из поля ""Форматы"" закладки ""Генерация"". Продолжить?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Information, "Тестирование производительности") = MsgBoxResult.Ok Then
            Dim pars(1) As String
            pars(0) = OFD2.Title
            pars(1) = OFD2.Filter
            OFD2.Title = "Выберите 5 и более аудиофайлов"
            OFD2.Filter = GetWildcardsAsFilter()
            OFD2.Multiselect = True
            If OFD2.ShowDialog = Windows.Forms.DialogResult.OK Then
                If OFD2.FileNames.Length > 4 Then
                    NumericUpDown1.Value = Math.Max(Math.Min(CDec(AutoCross.ShowDialog(OFD2.FileNames, Me)), NumericUpDown1.Maximum), NumericUpDown1.Minimum)
                Else
                    MsgBox("Выбрано недостаточное количество файлов!", MsgBoxStyle.Exclamation, "Тестирование производительности")
                End If
            End If
            OFD2.Multiselect = False
            OFD2.Title = pars(0)
            OFD2.Filter = pars(1)
        End If
        LogLevel -= 1 : LS("Авто-кроссфейдер: конец")
    End Sub
    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged
        ShakeFocus()
        If Object.ReferenceEquals(TabControl1.SelectedTab, TabPage8TagEditor) Then
            FillTagFromSelection()
            ListBox1.SelectionMode = SelectionMode.MultiExtended
            If ListBox1.SelectedIndex > -1 Then Scroll2(ListBox1.SelectedIndex)
        Else
            If ListBox1.SelectionMode <> SelectionMode.One Then
                ListBox1.SuspendLayout()
                ListBox1.SelectionMode = SelectionMode.One
                If ListBox1.SelectedIndex > -1 Then Scroll2(ListBox1.SelectedIndex)
            End If
        End If
    End Sub
    Private Sub Buttons_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button18.MouseDown, Button19.MouseDown, Button20.MouseDown, Button21.MouseDown
        LX("МаусDown кЫнопочки: " & CType(sender, Button).Name)
        btns_shk(2, CType(sender, Button))
    End Sub
    Private Sub Buttons_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button18.MouseEnter, Button19.MouseEnter, Button20.MouseEnter, Button21.MouseEnter, Button18.MouseUp, Button19.MouseUp, Button20.MouseUp, Button21.MouseUp
        LX("МаусEnter кЫнопочки: " & CType(sender, Button).Name)
        If Compot And Not W7.GlassMode Then CType(sender, Button).ForeColor = Color.Yellow
        btns_shk(1, CType(sender, Button))
    End Sub
    Private Sub Buttons_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button18.MouseLeave, Button19.MouseLeave, Button20.MouseLeave, Button21.MouseLeave
        LX("МаусLeave кЫнопочки: " & CType(sender, Button).Name)
        If Compot And Not W7.GlassMode Then CType(sender, Button).ForeColor = Drawing.SystemColors.ControlText
        btns_shk(0, CType(sender, Button))
    End Sub
    Private Sub Buttons_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Button18.Paint, Button19.Paint, Button20.Paint, Button21.Paint
        If W7.GlassMode Or Not Compot Then
            Dim b As Button = CType(sender, Button)
            LX("Отрисовка кЫнопочки: " & b.Name)
            e.Graphics.Clear(If(W7.GlassMode, Color.Transparent, b.BackColor))
            If b.Enabled Then
                Dim c2, c3 As Color
                Select Case CInt(b.Tag)
                    Case 0
                        c2 = b.FlatAppearance.CheckedBackColor
                        c3 = b.FlatAppearance.MouseOverBackColor
                    Case 1
                        c2 = b.FlatAppearance.MouseOverBackColor
                        c3 = b.ForeColor
                    Case Else
                        c2 = b.FlatAppearance.MouseDownBackColor
                        c3 = b.FlatAppearance.CheckedBackColor
                End Select
                Dim r As New Rectangle(1, 1, b.Width - 3, b.Height - 3)
                Dim r2 As New Rectangle(0, CInt(b.Height / 5), b.Width, CInt(b.Height * 1.5 / 5))
                Dim r3 As New Rectangle(0, CInt(b.Height / 5 * 2.5), b.Width, CInt(b.Height * 1.5 / 5))
                Dim lg As New Drawing2D.LinearGradientBrush(r2, If(W7.GlassMode, Color.Transparent, b.BackColor), c2, Drawing2D.LinearGradientMode.Vertical)
                Dim lg2 As New Drawing2D.LinearGradientBrush(r3, c2, If(W7.GlassMode, Color.Transparent, b.BackColor), Drawing2D.LinearGradientMode.Vertical)
                Dim sf As New System.Drawing.StringFormat()
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center
                e.Graphics.FillRectangle(lg, r2)
                e.Graphics.FillRectangle(lg2, r3)
                e.Graphics.DrawString(b.Text, b.Font, New Drawing.SolidBrush(c3), r, sf)
            End If
        End If
    End Sub
    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        Me.Refresh()
    End Sub
    Private Sub PlayerControls_Invalidater(ByVal sender As Object, ByVal e As Object)
        LX("PC:I event, sender is " & CType(sender, Control).Name)
        CType(sender, Control).Invalidate()
    End Sub
    Private Sub PlayerControls_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        LX("PC:P event, sender is " & sender.GetType.ToString & " " & CType(sender, Control).Name)
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.Default
        e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
        Dim s As Control = CType(sender, Control)
        Dim r As RectangleF = New RectangleF(New Point(0, 0), s.Size)
        If TypeOf sender Is CheckBox Then
            Dim offsetX As Integer = CheckBoxRenderer.GetGlyphSize(e.Graphics, GetCheckBoxState(CType(s, CheckBox))).Width
            r.X = offsetX + 2
            r.Width -= (offsetX + 2)
        ElseIf TypeOf sender Is Panel Then
            e.Graphics.DrawString("T", New Font("Arial", 400), Brushes.Black, New Point(-102, -102))
            Exit Sub
        End If
        e.Graphics.Clear(If(W7.GlassMode, Color.Transparent, GetSuperParentBackColor(s)))
        Dim f As New System.Drawing.StringFormat(StringFormatFlags.NoClip Or StringFormatFlags.NoFontFallback)
        f.LineAlignment = If(TypeOf sender Is ButtonBase, StringAlignment.Center, StringAlignment.Near)
        f.Alignment = If(TypeOf sender Is ButtonBase, GetStringAlignmentFromContentAlignment(CType(s, ButtonBase).TextAlign), If(TypeOf sender Is Label, GetStringAlignmentFromContentAlignment(CType(s, Label).TextAlign), StringAlignment.Near))
        If s.Text <> "" Then
            Dim blend As New Drawing2D.Blend()
            blend.Factors = New Single() {0, 0.2, 0.5, 0.5, 0.2, 0}
            blend.Positions = New Single() {0, 0.2, 0.4, 0.6, 0.8, 1}
            Dim lgb As New Drawing2D.LinearGradientBrush(r, Color.Transparent, Color.White, Drawing2D.LinearGradientMode.Vertical)
            lgb.Blend = blend

            Dim sz As SizeF = e.Graphics.MeasureString(s.Text, s.Font, r.Size, f)
            Dim r_string As RectangleF
            If sz.Width + sz.Height > r.Width Then
                r_string = New RectangleF(CIntR(r.Height / 2 + r.X), 0, r.Width - r.Height, r.Height)
            Else
                r_string = New RectangleF(CIntR(If(f.Alignment = StringAlignment.Center, (r.Width - sz.Width) / 2, CIntR(r.Height / 2)) + 1 + r.X), 0, CInt(sz.Width - 2), r.Height)
            End If
            If r.X > 0 Then r_string.X -= CIntR(Math.Max(0, (r.Height - r.X) / 3))
            e.Graphics.FillRectangle(lgb, r_string)

            blend.Factors = New Single() {0, 0.2, 0.5}
            blend.Positions = New Single() {0, 0.4, 1}

            Dim r_leftPie As New RectangleF(r_string.X - CIntR(r_string.Height / 2), r_string.Y, r_string.Height, r_string.Height)
            FillHalfTransparentPie(r_leftPie, blend, 90, 180, e.Graphics)
            Dim r_rightPie As New RectangleF(r_string.X + r_string.Width - CIntR(r_string.Height / 2), r_string.Y, r_string.Height, r_string.Height)
            FillHalfTransparentPie(r_rightPie, blend, 270, 180, e.Graphics)
        End If
        If TypeOf s Is CheckBox Then CheckBoxRenderer.DrawCheckBox(e.Graphics, New Point(0, CIntR((r.Height - CheckBoxRenderer.GetGlyphSize(e.Graphics, GetCheckBoxState(CType(s, CheckBox))).Height) / 2)), GetCheckBoxState(CType(s, CheckBox)))
        Dim fnt As Font = s.Font
        If (TypeOf s Is LinkLabel Or TypeOf s Is Button) AndAlso IsHotTracked(s) Then fnt = New Font(fnt, FontStyle.Underline)
        e.Graphics.DrawString(s.Text, fnt, New SolidBrush(If(W7.GlassMode, If(s.Enabled, Color.Black, Color.Gray), If(s.Enabled, s.ForeColor, Color.FromArgb(CIntR((CInt(s.ForeColor.R) + s.BackColor.R) / 2), CIntR((CInt(s.ForeColor.G) + s.BackColor.G) / 2), CIntR((CInt(s.ForeColor.B) + s.BackColor.B) / 2))))), r, f)
    End Sub
#End Region
#Region "Subs"
    'Генерация и всё, что связано со спиками
    Sub MainGen(ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)
        LS("Генератор: продолжение") : LogLevel += 1
        PLPleaseWait_Init("Пожалуйста, подождите...")
        SaveUndo()
        Dim tbtval As Integer = TrackBar1.Value
        '
        'Чистим и подготавливаем массивы адресов + заполняем глобальный источник
        LS("Чистка: «" & ComboBox1.Text & "», «" & ComboBox2.Text & "»")
        Dim aTex As String = ComboBox1.Text
        aTex = aTex.TrimEnd(New Char() {"\"c})
        For i As Integer = 0 To ComboBox1.Items.Count - 1
            ComboBox1.Items(i) = ComboBox1.Items(i).ToString.TrimEnd(New Char() {"\"c})
        Next
        ComboBox1.Text = aTex
        aTex = ComboBox2.Text
        aTex = aTex.TrimEnd(New Char() {"\"c})
        For i As Integer = 0 To ComboBox2.Items.Count - 1
            ComboBox2.Items(i) = ComboBox2.Items(i).ToString.TrimEnd(New Char() {"\"c})
        Next
        ComboBox2.Text = aTex
        TrackBar1.Value = tbtval
        If CInt(e.ClickedItem.Tag) = 1 Or CInt(e.ClickedItem.Tag) = 3 Then ClearLists(False)
        PLPLeaseWait_ChangeText("Получаю список файлов в источнике...")
        Me.Refresh()
        Application.DoEvents()
        'OutFiles очищаются в процедуре NameGen()
        GlobalSourceGen()
        'Заполняем массив Исходных файлов и Их размера
        LS("Генерим inFiles и inFilesSizes") : LogLevel += 1
        If CInt(e.ClickedItem.Tag) = 3 Then
            ShortSourceGen(e)
        Else
            If RadioButton2.Checked And inFilesGlobal.Count <= TrackBar1.Value Then
                LS("требуемого количества достичь невозможно из-за недостатка файлов")
                ShortSourceGen(e)
            Else
                LongSourceGen(False, CInt(e.ClickedItem.Tag) = 2)
            End If
        End If
        LogLevel -= 1
        '
        'Генерируем выходные имена
        NameGen()
        '
        Label5.Text = GetFullSizeMbWithSuffix()
        If inFiles.Count > 0 AndAlso (PathExists(ComboBox2.Text) Or IsPlaylist(ComboBox2.Text)) Then Button6.Enabled = True
        Processing = False
        LogLevel -= 1 : LS("Генератор: конец")
    End Sub
    Sub GlobalSourceGen()
        If inFilesGlobal.Count = 0 Then
            LS("GlobalSourceGen: Начало") : LogLevel += 1
            LS("Глобальный список пуст. Наполняем...")
            If IO.File.Exists(ComboBox1.Text) AndAlso IsPlaylist(ComboBox1.Text) Then
                ParsePlaylist(ComboBox1.Text, True)
            Else
                inFilesGlobal.AddRange(GetFilesMy(ComboBox1.Text, GetWildcards))
            End If
            If inFilesGlobal.Count = 0 Then
                MsgBox("В источнике файлы указанных типов не найдены!", MsgBoxStyle.Exclamation, "Ошибка!")
                LS("В источнике файлы указанных типов не найдены!") : LogLevel -= 1
                Exit Sub
            End If
            LogLevel -= 1 : LS("GlobalSourceGen: Конец")
        Else
            LS("<- GlobalSourceGen: Глобальный список не пуст. Пропускаю процессинг...")
        End If
    End Sub
    Sub LongSourceGen(ByVal OneFile As Boolean, ByVal TwoFiles As Boolean)
        If inFilesGlobal.Count > 0 Then
            LS("LongSourceGen: Начало@" & OneFile) : LogLevel += 1
            Dim i, k As Integer
            Dim j As Long
            k = 0
            Dim inFilesGlobal2 As New Collections.Generic.List(Of String)(inFilesGlobal)
            If Not OneFile Then
                PLPLeaseWait_ChangeText("Генерирую список исходных файлов...")
                Application.DoEvents() 'Отрисуем графику
            End If
            Do
                If inFiles.Count Mod 100 = 0 Then
                    Application.DoEvents()
                End If
                i = CInt(Rnd() * (inFilesGlobal2.Count - 1))
                j = New IO.FileInfo(inFilesGlobal2(i)).Length
                If RadioButton1.Checked AndAlso GetFullSizeMBStrict(j + GetFullSize()) > TrackBar1.Value Then
                    'даём X попыток поиска файла, который уместился бы в остаточное пространство
                    k += 1
                    LS("звоночек №" & k & ", длина " & j & ", элемент №" & i)
                    If k = NumericUpDown3.Value Then Exit Do
                    'но если по требуемому объёму достигнут максимум, то выход
                Else
                    k = 0   'На каждое ост. пространство отводится X попыток поиска файла
                    If Not inFiles.Contains(inFilesGlobal2(i)) Then
                        'Только новые файлы
                        LX("Внесён файл " & inFilesGlobal2(i) & ", sizом " & j & ", rs: " & i)
                        inFiles.Add(inFilesGlobal2(i))
                        AddFullSize(CInt(j))
                        If OneFile Then Exit Do
                    End If
                    inFilesGlobal2.RemoveAt(i) 'Чтобы не искать в добавленных только что или раньше
                End If
                If inFilesGlobal2.Count = 0 Or (RadioButton2.Checked And inFiles.Count = TrackBar1.Value) Or (RadioButton1.Checked And inFiles.Count = inFilesGlobal.Count) Then
                    'Если требуемое кол-во достигнуто или требуемого объёма невозможно достичь из-за недостатка файлов - выход
                    LS("Достигнут максимум: iF=" & inFiles.Count & ", iFG=" & inFilesGlobal.Count)
                    Exit Do
                End If
            Loop
            LogLevel -= 1 : LS("LongSourceGen: Конец")
        Else
            LS("<- LongSourceGen @" & OneFile & ": Глобальный список пуст, пропускаю процессинг...")
        End If
    End Sub
    Sub ShortSourceGen(ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)
        inFiles.AddRange(inFilesGlobal)
        Dim filefound As Boolean = True
        PLPleaseWait_Init("Проверка файлов на диске...", 0, inFiles.Count - 1, 0)
        PLPleaseWait.Max2 = inFiles.Count
        For i As Integer = 0 To inFiles.Count - 1
            If i Mod 100 = 0 Then
                Application.DoEvents()
            End If
            If IO.File.Exists(inFiles(i)) Then
                AddFullSize(New IO.FileInfo(inFiles(i)).Length)
                PLPleaseWait_ValuePlusPlus(True)
                PLPleaseWait_ValuePlusPlus(False)
            Else
                filefound = False
                Exit For
            End If
        Next
        If Not filefound Then
            ClearLists(True)
            MainGen(e)
        End If
        LS("Добавлен диапазон имён и размеров: " & inFiles.Count)
    End Sub
    Sub NameGen()
        Static running As Boolean = False
        Static nbryak As Boolean = False
        If Not init Then
            If running Then
                nbryak = True
                Exit Sub
            End If
            running = True
            nbryak = False
            LS("NameGen: Начало") : LogLevel += 1
            outFiles.Clear()
            Dim b1, b2, b3, b4, b5, b6 As Boolean
            b1 = ComboBox1.Enabled : ComboBox1.Enabled = False
            b2 = ComboBox2.Enabled : ComboBox2.Enabled = False
            b3 = Button3.Enabled : Button3.Enabled = False
            b4 = Button6.Enabled : Button6.Enabled = False
            b5 = Button9.Enabled : Button9.Enabled = False
            b6 = Button12.Enabled : Button12.Enabled = False
            If PathExists(ComboBox2.Text) And inFiles.Count > 0 Then
                PLPleaseWait_Init("Генерирую имена целевых файлов...", 0, inFiles.Count, 0)
                PLPleaseWait.Max2 = inFiles.Count 'Здесь объект точно есть
                LS("папка существует, объектов " & inFiles.Count & ", сб1.ч: " & CheckBox1.Checked.ToString) : LogLevel += 1
                For i As Integer = 0 To inFiles.Count - 1
                    LX("NameGen: evaluating " & inFiles(i))
                    If CheckBox1.Checked Then
                        If RadioButton5.Checked Then
                            outFiles.Add(GetCombobox2Text(i) & PathORezq(inFiles(i), CByte(ComboBox3.Items.IndexOf(ComboBox3.Text) + 1)))
                        Else
                            Dim tag As AudioTag = GenTag(inFiles(i), GetUserId3RequestLevel)
                            If tag.Empty = False Then
                                outFiles.Add(GetCombobox2Text(i) & ParseWildcard(TextBox2.Text, New String() {tag.Artist, tag.Album, tag.Year, tag.Track, tag.Comments, tag.Number, tag.Genre, Mid(inFiles(i), inFiles(i).LastIndexOf(".") + 2).ToLower}))
                            Else
                                outFiles.Add(GetCombobox2Text(i) & PathORezq(inFiles(i), 0))
                            End If
                        End If
                    Else
                        outFiles.Add(GetCombobox2Text(i) & PathORezq(inFiles(i), 0))
                    End If
                    If nbryak Then Exit For
                    LX("NameGen: result is " & outFiles(i))
                    PLPleaseWait_ValuePlusPlus(True)
                    PLPleaseWait_ValuePlusPlus(False)
                    Application.DoEvents()
                Next
                LogLevel -= 1
            End If
            running = False
            If nbryak Then NameGen()
            ComboBox1.Enabled = b1
            ComboBox2.Enabled = b2
            Button3.Enabled = b3
            Button6.Enabled = b4
            Button9.Enabled = b5
            Button12.Enabled = b6
            PLPleaseWait_Halt()
            LogLevel -= 1 : LS("NameGen: Конец")
        End If
    End Sub
    Sub ReFreshPlayListAuto()
        If RadioButton3.Checked Then ReFreshPlayList(inFiles) Else ReFreshPlayList(outFiles)
    End Sub
    Sub ReFreshPlayList(ByVal col As Collections.Generic.List(Of String))
        LS("ReFreshPlayList: Начало") : LogLevel += 1
        ListBox1.Items.Clear()
        If col.Count > 0 Then
            LS("объектов: " & col.Count)
            ListBox1.Items.AddRange(col.ToArray)
        End If
        If inFiles.Count > 0 Then Button12.Enabled = True Else Button12.Enabled = False
        TSMI_1_3.Enabled = Button12.Enabled
        TSMI_1_4.Enabled = Button12.Enabled
        TSMI_1_1.Enabled = False
        TSMI_1_2.Enabled = False
        Button28.Enabled = False
        FillTagFromSelection()
        Button18.Enabled = Not ((CheckBox13.Checked And plFiles.Count < 2) Or (CheckBox13.Checked = False And inFiles.Count = 0))
        Button19.Enabled = Not (inFiles.Count = 0 And Button19.Text <> ";")
        If inFiles.Count = 0 And Button19.Text <> ";" Then Button20.Enabled = False
        If inFiles.Count = 0 Then Button21.Enabled = False
        LogLevel -= 1 : LS("ReFreshPlayList: Конец")
    End Sub
    Sub AddFullSize(ByVal Size2Add As Long)
        'Fast aka inFilesSizes.Add()+GetFullSize()
        inFilesSizes.Add(Size2Add)
        If GetFullSize__va_cache = -1 Or GFS_num <> inFilesSizes.Count - 1 Then
            GetFullSize()
        Else
            GetFullSize__va_cache += Size2Add
            GFS_num += 1
        End If
    End Sub
    Sub SizesUpdate()
        inFilesSizes.Clear()
        GetFullSize__va_cache = -1
        For i As Integer = inFiles.Count - 1 To 0 Step -1
            If IO.File.Exists(inFiles(i)) = False Then
                Achtung("Файл «" & inFiles(i) & "» не найден и был удалён из списка")
                inFiles.RemoveAt(i)
            End If
        Next
        For i As Integer = 0 To inFiles.Count - 1
            AddFullSize(New IO.FileInfo(inFiles(i)).Length)
        Next
        LS("<- SizesReGen: объёмы обновлены")
        Label5.Text = GetFullSizeMbWithSuffix()
        LS("<- SizesАпдейт: label обновлен")
    End Sub
    Sub ReFreshTail()
        LS("RefreshTail: Начало") : LogLevel += 1
        SizesUpdate()
        NameGen()
        LogLevel -= 1 : LS("RefreshTail: Конец")
    End Sub
    'Внутренние процедуры копирования.
    Sub CopyFile(ByVal fin As String, ByVal fout As String)
        Try
            Using instr As New UIO.FileStream(fin, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
                PLPB2Max = instr.Length
                PLPB2Val = 0
                UIO.Directory.CreateDirectory(UIO.Path.GetDirectoryName(fout))
                Using outstr As New UIO.FileStream(fout, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None, CULng(instr.Length))
                    Dim tick As Integer
                    Try
                        Dim arr(511999) As Byte
                        Dim tBuf As Integer
                        While instr.Position < instr.Length
                            tick = My.Computer.Clock.TickCount
                            instr.Read(arr, 0, 512000)
                            tBuf = If(instr.Position = instr.Length, CInt(instr.Length Mod 512000), 512000)
                            outstr.Write(arr, 0, tBuf)
                            PLPBVal += tBuf
                            PLPB2Val += tBuf
                            Ticks += My.Computer.Clock.TickCount - tick
                        End While
                        ProcessingInternal = 1
                    Catch ex As Exception
                        ProcessingInternal = -1
                        LastErrorText = ex.Message.TrimEnd(New Char() {Chr(13), Chr(10)})
                    End Try
                End Using
            End Using
        Catch ex As Exception
            ProcessingInternal = -1
            LastErrorText = ex.Message
        End Try
    End Sub
    Sub CopyFile()
        CopyFile(inFileCurrent, outFileCurrent)
    End Sub
    'Помощники воспроизведения
    Sub StopCaller4Redraw()
        LS("-> StopCaller4Redraw")
        Button20_Stop(Me, Nothing)
    End Sub
    Sub PlayNextCaller(ByVal Ord As Boolean)
        LS("PlayNextCaller: begin @" & Ord) : LogLevel += 1
        If Ord Then
            Button21_Next(Nothing, Nothing)
        Else
            Dim i As Integer = GenNextPlayedIndex()
            LS("Следующий воспроизводимый индекс: " & i)
            If i = -1 Or ListBox1.Items.Count < i - 1 Then
                Button20_Stop(Me, Nothing)
            Else
                Dim ti As Integer = ListBox1.SelectedIndex
                If CheckBox16.Checked Then ListBox1.SelectedIndex = i Else Scroll2(i)
                Button19.Text = "4"
                Button19_PlayPause(Nothing, Nothing)
                If CheckBox16.Checked Then Scroll2(ti)
            End If
        End If
        LogLevel -= 1 : LS("PlayNextCaller: end")
    End Sub
    Sub PlayNextStub(ByVal index As Integer)
        LS("PlayNextStub: begin @" & index) : LogLevel += 1
        Dim ti As Integer = ListBox1.SelectedIndex
        If CheckBox16.Checked Then ListBox1.SelectedIndex = index Else Scroll2(index)
        Button20_Stop(Nothing, Nothing)
        If Not InvalidState Then Button19_PlayPause(Nothing, Nothing)
        If CheckBox16.Checked Then Scroll2(ti)
        LogLevel -= 1 : LS("PlayNextStub: end")
    End Sub
    Sub AddPlayedIndex(ByVal index As Integer)
        LS("-> AddPlaylistIndex: " & index)
        If Not (plFiles.Count > 0 AndAlso plFiles(plFiles.Count - 1) = index) Then plFiles.Add(index) Else LS("...skiped")
        If (CheckBox13.Checked And plFiles.Count > 1) Or (CheckBox13.Checked = False And inFiles.Count > 0) Then Button18.Enabled = True
        Button21.Enabled = True
    End Sub
    Sub AddPlayedIndex()
        AddPlayedIndex(If(ListBox1.SelectedIndex = -1, 0, ListBox1.SelectedIndex))
    End Sub
    Sub SmallPlayStub()
        If Player.Init(inFiles(ListBox1.SelectedIndex)) Then
            Button19.Text = ";"
            PlayIdx = ListBox1.SelectedIndex
        End If
    End Sub
    'Логгирование
    Sub Achtung(ByVal txt As String) Implements iLoggerHost.Achtung
        If CheckBox34.Checked Then
            LR()
            TabPage6Log.Text = "Лог (ЕСТЬ ОШИБКИ!)"
        End If
        LA(If(CheckBox24.Checked, LT() & ": ", "") & "ОШИБКА!!! " & txt)
    End Sub
    Sub LS(ByVal txt As String) Implements iLoggerHost.LS
#If DEBUG Then
        Debug.Print(Now.ToLongTimeString() & "." & Now.Millisecond & "	" & txt)
#End If
        If CheckBox4.Checked Then LA(If(CheckBox24.Checked, LT() & " ", "") & StrDup(LogLevel, Chr(9)) & txt)
    End Sub
    Sub LX(ByVal txt As String) Implements iLoggerHost.LX
#If DEBUG Then
        Debug.Print(Now.ToLongTimeString() & "." & Now.Millisecond & "	" & txt)
#End If
        If LxB Then LA(If(CheckBox24.Checked, LT() & " ", "") & StrDup(LogLevel, Chr(9)) & txt)
    End Sub
    Sub LR()
        If LogPanel IsNot Nothing Then
            TabControl1.Controls.Add(LogPanel)
            LogPanel = Nothing
        End If
    End Sub
    Sub LA(ByVal Text As String)
        Logbox.Items.Add(If(Text.Length > 512, Text.Substring(0, 512), Text))
    End Sub
    Delegate Sub LD(ByVal txt As String)
    'Отменялка
    Sub SaveUndo()
        If inFiles.Count > 0 Then
            LS("SaveUndo: Начало") : LogLevel += 1
            If UndoListIndex = 10 Then
                LS("перемещение...")
                For i As Integer = 0 To 8
                    undoList(i).Clear()
                    undoList(i).AddRange(undoList(i + 1))
                Next
                UndoListIndex = 9
            End If
            undoList(UndoListIndex).Clear()
            LS("копирование...")
            undoList(UndoListIndex).AddRange(inFiles)
            UndoListIndex += 1
            Button16.Enabled = True
            LogLevel -= 1 : LS("SaveUndo: Конец")
        Else
            LS("<- SaveUndo: список исходных пуст")
        End If
    End Sub
    Sub UndoInternals()
        LS("-> UndoInternals")
        inFiles.Clear()
        LS("Записей: " & undoList(UndoListIndex - 1).Count)
        For i As Integer = 0 To undoList(UndoListIndex - 1).Count - 1
            inFiles.Add(undoList(UndoListIndex - 1)(i))
            LX(i & Chr(9) & "Добавлено: " & inFiles(i))
        Next
    End Sub
    'Помощники по плейлистам
    Sub ParsePlaylist(ByVal Playlist As String, ByVal addGlobally As Boolean)
        LS("ParsePlaylist: Начало @ " & Playlist & ", " & addGlobally) : LogLevel += 1
        If ParsePlaylistPlaTest(Playlist) Then
            LS("-> ParsePlaylistBinary") : LogLevel += 1
            Dim br As New IO.BinaryReader(New IO.FileStream(Playlist, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None))
            'Dim records As Integer = br.ReadByte * 256 ^ 3 + br.ReadByte * 256 ^ 2 + br.ReadByte * 256 + br.ReadByte
            Dim records As Integer = CInt(br.BaseStream.Length / 512 - 1) 'Так же проще, правда?
            LS("Записей в файле: " & records.ToString)
            Dim RelPath, AbsPath As String
            Dim Drvs As IO.DriveInfo() = IO.DriveInfo.GetDrives
            br.BaseStream.Seek(512, IO.SeekOrigin.Begin)
            LX("Начинаю внешний цикл") : LogLevel += 1
            For i As Integer = 1 To records
                br.ReadInt16()
                RelPath = Razor(DecodeUTFBE(br.ReadBytes(510)))
                LX("Относительный путь: " & RelPath) : LogLevel += 1
                For j As Integer = 0 To Drvs.Length - 1
                    If (Drvs(j).Name <> "A:\" And Drvs(j).Name <> "B:\") Then
                        AbsPath = Drvs(j).Name & Mid(RelPath, 2)
                        LX("Абсолютный путь: " & AbsPath)
                        If IO.File.Exists(AbsPath) Then
                            ParsePlaylistStrTest(AbsPath, addGlobally)
                            Exit For
                        End If
                    End If
                Next
                LogLevel -= 1
            Next
            LogLevel -= 1 : LS("Закрываю файл...")
            br.Close()
            LogLevel -= 1 : LS("ParsePlaylistBinary: конец")
        Else
            LS("-> ParsePlaylistTextual")
            'Парсинг плейлистов без разбора комментов и расширенной инфы
            Dim DR As M3uPreview.iDialogResult = M3uPreview.ShowDialog(Playlist)
            If DR.DialogResult = Windows.Forms.DialogResult.OK Then
                Dim instream As New IO.StreamReader(Playlist, DR.Encoding)
                LS("Открыт. Сплиттинг...")
                Dim lines() As String = instream.ReadToEnd.Replace(Chr(13), "").Split(New String() {Chr(10)}, StringSplitOptions.RemoveEmptyEntries)
                instream.Close()
                LS("Сплиттинг завершён, плейлист закрыт.")
                Dim curString As String
                Dim plc As Boolean = False
                LS("Ну, поехали. curstr: alloc ok, strings: " & lines.Length) : LogLevel += 1
                For i As Integer = 0 To lines.Length - 1
                    curString = ""
                    LX("Дано: " & lines(i))
                    If i = 0 AndAlso lines(0).StartsWith("<Default:") Then plc = True
                    If i <> 0 And plc Then lines(i) = lines(i).Split(New String() {"|"}, StringSplitOptions.RemoveEmptyEntries)(1)
                    'Поддержка формата plc - AIMP2
                    If Not ((lines(i).StartsWith("#")) Or lines(i).StartsWith(":WINMPP:")) Then
                        'Если это не комменты или extinfo в m3u(8) или wmpl3
                        If Mid(lines(i), 2, 2) = ":\" Then
                            'm3u(8) или wmpl3 - просто берём строку, если она соответствует шаблону
                            curString = lines(i)
                        ElseIf lines(i).StartsWith("..\") Then
                            'Поддержка относительных плейлистов - часть 1
                            Dim j As Byte = 0
                            While Mid(lines(i), j * 3 + 1, 3) = "..\"
                                j += CByte(1)
                            End While
                            Dim m As Integer = Len(Playlist) - PathORezq(Playlist, j).Length
                            curString = Strings.Left(Playlist, m) & Mid(lines(i), j * 3)
                        ElseIf lines(i).StartsWith("File") Then
                            'pls - ипём моск: только если это строка с адресом, то ищем начало этого адреса
                            Dim j As Integer = InStr(lines(i), ":\")
                            If j > 0 Then curString = Mid(lines(i), j - 1)
                        Else
                            'Поддержка относительных плейлистов - часть 2
                            'Применяется для плейлистов, расположенных в корне дерева папок и файлов с музыкой
                            Dim s1 As String = Mid(Playlist, 1, Len(Playlist) - Len(PathORezq(Playlist, 0)) + 1)
                            curString = s1 & lines(i)
                        End If
                    End If
                    If curString <> "" Then
                        If IO.File.Exists(curString) Then
                            ParsePlaylistStrTest(curString, addGlobally)
                        Else
                            Achtung("ParsePlaylistStrTest: файл «" & curString & "» не найден и не был добавлен!")
                        End If
                    Else
                        LS("<-Empty string")
                    End If
                Next
                LogLevel -= 1 : LS("ParsePlaylistTextual: Конец")
            Else
                LS("ParsePlaylistTextual: Отмена")
            End If
        End If
        LogLevel -= 1 : LS("ParsePlaylist: Конец")
    End Sub
    Sub ParsePlaylistStrTest(ByVal CurString As String, ByVal AddGlobally As Boolean)
        Dim WildCards() As String = GetWildcards()
        LS("-> ParsePlaylistStrTest: wildcards = " & WildCards.Length & ", CurString = " & CurString)
        If AddGlobally Then
            For j As Integer = 0 To WildCards.Length - 1
                If CurString.ToLower Like WildCards(j).ToLower Then
                    LX("ParsePlaylistStrTest: добавляю объект глобально " & CurString)
                    inFilesGlobal.Add(CurString)
                    Exit For
                End If
            Next
        Else
            If CheckBox22.Checked = False OrElse inFiles.Contains(CurString) = False Then
                For j As Integer = 0 To WildCards.Length - 1
                    If CheckBox2.Checked = False OrElse CurString.ToLower Like WildCards(j).ToLower Then
                        LX("ParsePlaylistStrTest: добавляю объект " & CurString)
                        inFiles.Add(CurString)
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub
    Sub ClearBytes(ByRef Bytes() As Byte)
        LS("-> ClearBytes")
        For i As Integer = 0 To Bytes.Length - 1
            Bytes(i) = 0
        Next
    End Sub
    'Sub WriteBytes(ByRef Bytes() As Byte, ByVal offset As Integer, ByVal NewBytes() As Byte)
    '	LS("-> WriteBytes @ " & offset.ToString & ":array of " & NewBytes.Length)
    '	Dim ptr As Integer = 0
    '	While offset < Bytes.Length - 1
    '		Bytes(offset) = NewBytes.CopyTo(  (ptr)
    '		offset += 1
    '		ptr += 1
    '		If ptr = bb.Length Then Exit While
    '	End While
    '	If ptr < bb.Length Then Achtung("WriteBytes failed, ptr = " & ptr)
    'End Sub
    Sub ClearLists(ByVal ClearGlobal As Boolean)
        LS("-> ClearLists @ " & ClearGlobal.ToString)
        If ClearGlobal Then inFilesGlobal.Clear()
        inFiles.Clear()
        inFilesSizes.Clear()
        outFiles.Clear()
        plFiles.Clear()
        PlayIdx = -1
        Label5.Text = "0 Mb"
        Button6.Enabled = False
        ReFreshPlayListAuto()
        ListBox1.Focus()
    End Sub
    'Разное
    Sub MoveCallback()
        Form1_Move(Nothing, Nothing)
    End Sub
    Sub Scroll2(ByVal index As Integer)
        If index > -1 And index < ListBox1.Items.Count Then
            ProcessingScroll = True
            Dim k As Integer = CInt(ListBox1.Height / (ListBox1.ItemHeight * 2))
            LS("-> Scroll2: j=" & index & ", k=" & k)
            If ListBox1.SelectionMode <> SelectionMode.One Then ListBox1.ClearSelected()
            ListBox1.SetSelected(If(index + k < ListBox1.Items.Count, index + k, ListBox1.Items.Count - 1), True)
            If ListBox1.SelectionMode <> SelectionMode.One Then ListBox1.SetSelected(If(index + k < ListBox1.Items.Count, index + k, ListBox1.Items.Count - 1), False)
            ListBox1.SetSelected(If(index - k > -1, index - k, 0), True)
            If ListBox1.SelectionMode <> SelectionMode.One Then ListBox1.SetSelected(If(index - k > -1, index - k, 0), False)
            ProcessingScroll = False
            ListBox1.SetSelected(index, True)
        End If
    End Sub
    Sub ClipBoardCheck(ByVal TestText As Object)
        Try
            If My.Computer.Clipboard.ContainsText Then
                If CStr(TestText) = My.Computer.Clipboard.GetText Then
                    ClipboardCheckResult = 1
                Else
                    Invoke(New LD(AddressOf LS), New String() {"В буфере " & My.Computer.Clipboard.GetText & Chr(13) & Chr(10) & "вместо " & TestText.ToString})
                    ClipboardCheckResult = -1
                End If
            Else
                Invoke(New LD(AddressOf LS), New String() {"В буфере пусто"})
                ClipboardCheckResult = -1
            End If
        Catch ex As Exception
            ClipboardCheckResult = -1
        End Try
    End Sub
    Sub btns_shk(ByVal i As Integer, ByVal b As Button)
        b.Tag = i
        b.Refresh()
        LS(b.Name & " tag = " & i)
    End Sub
    Sub FillTagFromSelection()
        tagAlbum.Text = ""
        tagArtist.Text = ""
        tagComment.Text = ""
        tagGenre.Text = ""
        tagTrackName.Text = ""
        tagTrackNumber.Text = ""
        tagYear.Text = ""
        If ListBox1.SelectedIndex > -1 AndAlso inFiles.Count > ListBox1.SelectedIndex Then
            Dim t As AudioTag = TagReader.GenTag(inFiles(ListBox1.SelectedIndex), GetUserId3RequestLevel(), , False)
            tagAlbum.Text = t.Album
            tagArtist.Text = t.Artist
            tagComment.Text = t.Comments
            tagGenre.Text = t.Genre
            tagTrackName.Text = t.Track
            tagTrackNumber.Text = t.Number
            tagYear.Text = t.Year
            If t.Valid Then
                If t.Empty = False Then
                    tagStatus.Text = "Тег в порядке"
                Else
                    tagStatus.Text = "Тег пуст"
                End If
            Else
                tagStatus.Text = "Тег неверен"
            End If
            Button28.Enabled = If(plFiles.Count > 0, Not TestPL(True) OrElse Not (ListBox1.SelectedIndices.Contains(plFiles(plFiles.Count - 1))), True)
        Else
            tagStatus.Text = "Тег недоступен"
            Button28.Enabled = False
        End If
    End Sub
    Sub FillHalfTransparentPie(ByVal pie As RectangleF, ByRef blend As Drawing2D.Blend, ByVal startAngle As Integer, ByVal endAngle As Integer, ByRef g As Graphics)
        Dim p As New Drawing2D.GraphicsPath()
        p.AddEllipse(pie)
        Dim pgb As New Drawing2D.PathGradientBrush(p)
        pgb.CenterColor = Color.White
        pgb.SurroundColors = New Color() {Color.Transparent}
        pgb.Blend = blend
        g.FillPie(pgb, New Rectangle(CInt(pie.X), CInt(pie.Y), CInt(pie.Width), CInt(pie.Height)), startAngle, endAngle)
    End Sub
#End Region
#Region "Functions"
    Shared Function CIntR(ByVal d As Double) As Integer
        ' Эта функция выполняет округление, привычное по школе, т.е. не соответствующее IETF: всё что меньше 0,5 округляется вниз, остальное — вверх
        ' Используется при точном позиционировании графических объектов
        If (CInt(d * 10)) Mod 10 < 5 Then Return CInt(Math.Floor(d)) Else Return CInt(Math.Ceiling(d))
    End Function
    Function CountChars(ByVal s As String, ByVal c As Char) As Integer
        CountChars = 0
        For i As Integer = 0 To s.Length - 1
            If s.Chars(i) = c Then CountChars += 1
        Next
        LS("<- CountChars: " & CountChars)
    End Function
    Function GetCheckBoxState(ByVal c As CheckBox) As System.Windows.Forms.VisualStyles.CheckBoxState
        Dim a As New CheckBox.CheckBoxAccessibleObject(c)
        Select Case c.CheckState
            Case CheckState.Checked
                If ((a.State And AccessibleStates.HotTracked) > 0) Or IsHotTracked(c) Then Return VisualStyles.CheckBoxState.CheckedHot
                If (a.State And AccessibleStates.Pressed) > 0 Then Return VisualStyles.CheckBoxState.CheckedPressed
                If c.Enabled = False Then Return VisualStyles.CheckBoxState.CheckedDisabled
                Return VisualStyles.CheckBoxState.CheckedNormal
            Case CheckState.Unchecked
                If ((a.State And AccessibleStates.HotTracked) > 0) Or IsHotTracked(c) Then Return VisualStyles.CheckBoxState.UncheckedHot
                If (a.State And AccessibleStates.Pressed) > 0 Then Return VisualStyles.CheckBoxState.UncheckedPressed
                If c.Enabled = False Then Return VisualStyles.CheckBoxState.UncheckedDisabled
                Return VisualStyles.CheckBoxState.UncheckedNormal
            Case Else
                If ((a.State And AccessibleStates.HotTracked) > 0) Or IsHotTracked(c) Then Return VisualStyles.CheckBoxState.MixedHot
                If (a.State And AccessibleStates.Pressed) > 0 Then Return VisualStyles.CheckBoxState.MixedPressed
                If c.Enabled = False Then Return VisualStyles.CheckBoxState.MixedDisabled
                Return VisualStyles.CheckBoxState.MixedNormal
        End Select
    End Function
    Function GetCombobox2Text(ByVal ItemIndex As Integer) As String
        LX("-> GetCB2Text@ " & ItemIndex)
        If CheckBox23.Checked Then
            If RadioButton6.Enabled AndAlso RadioButton6.Checked AndAlso Not (TextBox2.Text.StartsWith("\"c)) Then
                Return ComboBox2.Text.TrimEnd("\"c) & "\" & Format(ItemIndex \ CInt(NumericUpDown4.Value), "000\\")
            Else
                Return ComboBox2.Text.TrimEnd("\"c) & "\" & Format(ItemIndex \ CInt(NumericUpDown4.Value), "000")
            End If
        Else
            If RadioButton6.Enabled AndAlso RadioButton6.Checked AndAlso Not (TextBox2.Text.StartsWith("\"c)) Then
                Return ComboBox2.Text.TrimEnd("\"c) & "\"
            Else
                Return ComboBox2.Text.TrimEnd("\"c)
            End If
        End If
    End Function
    Function GetCurrentPlayingIndex() As Integer
        LS("-> Определение индекса текущего воспроизводимого файла...")
        If Label15.Text = "" Then
            Return -1
        ElseIf plFiles.Count > 0 AndAlso Label15.Text = inFiles(plFiles(plFiles.Count - 1)) Then
            Return plFiles(plFiles.Count - 1)
        Else
            Return inFiles.IndexOf(Label15.Text)
        End If
    End Function
    Private Function GetFilesMy(ByVal path As String, ByVal wildcards() As String) As Collections.ObjectModel.ReadOnlyCollection(Of String)
        LS("MyGetFiles: Начало@" & path & ", wc:" & wildcards.Length & ", FCEx=" & CheckBox25.Checked) : LogLevel += 1
        If PathExists(path) Then
            Dim res As New Collections.Generic.List(Of String)
            For i As Integer = 0 To wildcards.Length - 1
                Dim t As New Threading.Thread(AddressOf GetFilesMyInternal)
                t.Name = "MyGetFilesInternal"
                t.Start(New String() {path, wildcards(i)})
                While t.IsAlive
                    Threading.Thread.Sleep(10)
                    Application.DoEvents()
                End While
                If CheckBox25.Checked Then
                    For Each s As String In mt_gf_Range
                        If CheckBox26.Checked Then
                            If s.ToLower Like wildcards(i).ToLower Then res.Add(s)
                        Else
                            If s Like wildcards(i) Then res.Add(s)
                        End If
                        'Казалось бы, бред, но проблема в том, что .Net считает в запросе GetFiles() шаблоны "*.fla*" и "*.fla"
                        'эквивалентными. Указанная галка (№25) как раз и предназначена для точной сортировки в сложных случаях,
                        'поскольку оператор like таких ошибок не совершает. Равно как и не работает толерантно к регистру (галка №26)
                    Next
                Else
                    res.AddRange(mt_gf_Range)
                End If
                LS("wc: " & wildcards(i) & ", res: " & res.Count)
            Next
            GetFilesMy = New Collections.ObjectModel.ReadOnlyCollection(Of String)(res)
        Else
            Return New Collections.ObjectModel.ReadOnlyCollection(Of String)(New Collections.Generic.List(Of String)())
        End If
        LogLevel -= 1 : LS("MyGetFiles: Конец")
    End Function
    Private Sub GetFilesMyInternal(ByVal data As Object)
        Dim data_i As String() = CType(data, String())
        mt_gf_Range = IO.Directory.GetFiles(data_i(0), data_i(1), IO.SearchOption.AllDirectories)
    End Sub
    Function GetFullSize() As Long
        If GetFullSize__va_cache = -1 Or GFS_num <> inFilesSizes.Count Then
            GetFullSize = 0
            If inFilesSizes.Count > 0 Then
                For i As Integer = 0 To inFilesSizes.Count - 1
                    GetFullSize += inFilesSizes(i)
                Next
            End If
            GFS_num = inFilesSizes.Count
            GetFullSize__va_cache = GetFullSize
            LX("<- GetFullSize: " & GetFullSize)
        Else
            GetFullSize = GetFullSize__va_cache
            LX("<- GetFullSize (cached): " & GetFullSize)
        End If
    End Function
    Function GetFullSizeMB() As Single
        Return GetFullSizeMB(GetFullSize())
    End Function
    Function GetFullSizeMB(ByVal SizeInBytes As Long) As Single
        LX("->GetFullSizeMB @ " & SizeInBytes)
        If CheckBox33.Checked And SizeInBytes >= 1073741824 Then
            SizeSuffix = " Gb"
            Return CSng(CLng((SizeInBytes / (1024 * 1024 * 1024)) * 100) / 100)
        Else
            SizeSuffix = " Mb"
            Return GetFullSizeMBStrict(SizeInBytes)
        End If
    End Function
    Function GetFullSizeMbWithSuffix() As String
        Return GetFullSizeMB.ToString & SizeSuffix
    End Function
    Function GetFullSizeMBStrict() As Single
        Return GetFullSizeMBStrict(GetFullSize)
    End Function
    Function GetFullSizeMBStrict(ByVal SizeInBytes As Long) As Single
        Return CSng(CLng((SizeInBytes / (1024 * 1024)) * 100) / 100)
    End Function
    Function GetNextLine(ByVal StartIdx As Integer, ByVal Vector As SearchVector) As Integer
        GetNextLine = -1
        Dim i As Integer = 0
        While i < ListBox1.Items.Count
            If ListBox1.Items(StartIdx).ToString.ToLower.Contains(TextBox5.Text.ToLower) Then
                GetNextLine = StartIdx
                Exit While
            Else
                i += 1
                StartIdx += Vector
                If StartIdx = ListBox1.Items.Count Then StartIdx = 0
                If StartIdx = -1 Then StartIdx += ListBox1.Items.Count
            End If
        End While
        LS("<- GetNextLine = " & GetNextLine)
    End Function
    Function GenNextPlayedIndex() As Integer
        LS("-> Определение индекса следующего воспроизводимого файла...")
        If ListBox1.Items.Count > 0 Then
            Dim j As Integer = GetCurrentPlayingIndex()
            If CheckBox11.Checked Then
                Return j
            Else
                Dim b As Boolean = True
                For i As Integer = 0 To inFiles.Count - 1
                    If Not plFiles.Contains(i) Then b = False
                Next
                If b Then
                    Return -1
                Else
                    If CheckBox12.Checked Then
                        Do
                            j = CInt(Rnd() * (inFiles.Count - 1))
                            If Not plFiles.Contains(j) Then Return j
                        Loop
                    Else
                        Return If(j = inFiles.Count - 1, 0, j + 1)
                    End If
                End If
            End If
        Else
            LS("<- -1: треклист пуст")
            Return -1
        End If
    End Function
    Function GetStringAlignmentFromContentAlignment(ByVal a As System.Drawing.ContentAlignment) As StringAlignment
        If a = ContentAlignment.BottomCenter Or a = ContentAlignment.MiddleCenter Or a = ContentAlignment.TopCenter Then Return StringAlignment.Center
        If a = ContentAlignment.BottomLeft Or a = ContentAlignment.MiddleLeft Or a = ContentAlignment.TopLeft Then Return StringAlignment.Near
        Return StringAlignment.Far
    End Function
    Function GetSuperParentBackColor(ByVal c As Control) As Color
        While c.Parent IsNot Nothing
            If c.Parent.BackColor <> Color.Transparent Then Return c.Parent.BackColor Else c = c.Parent
        End While
        Return c.BackColor
    End Function
    Function GetTrackBar1ValueMB() As Single
        Static cache As Single
        If Not (Button6.Text.StartsWith("Останов")) Then
            cache = If(RadioButton1.Checked, TrackBar1.Value, TrackBar1.Value * 5)
        End If
        Return cache
    End Function
    'ID3Level enum is supposed Friend by default (vb.net rule), so we need strict Private modifier here
    '
    'Here's an explanation of how it works
    '
    '0001 1  l1
    '0010 2  l2
    '0011 3  b
    '0100 4  n_marker 	#directly_unusable
    '0101 5  l1n 		#unusable
    '0110 6  l2n
    '0111 7  bn
    '1000 8  n2_marker 	#directly_unusable
    '1001 9  l1n2		#unusable
    '1010 10 l2n2
    '1011 11 bn2
    '...				#unusable

    'v	si	descr
    '1	0	Уровень 1
    '2	1	Уровень 2
    '3	2	Оба уровня
    '6	3	Ур. 2 без TPE1
    '7	4	Оба, без TPE1
    '10	5	Ур. 2 без TPE2
    '11	6	Оба, без TPE2
    Private Function GetUserId3RequestLevel() As ID3Mode
        Return CType(ComboBox5.SelectedIndex + 1 + If(ComboBox5.SelectedIndex > 2, If(ComboBox5.SelectedIndex > 4, 4, 2), 0), ID3Mode)
    End Function
    Private Function GetUserId3Encoding() As System.Text.Encoding
        Select Case ComboBox7.SelectedIndex
            Case 0
                Return System.Text.Encoding.Default
            Case 1
                Return System.Text.Encoding.Unicode
            Case 2
                Return System.Text.Encoding.BigEndianUnicode
            Case Else
                Return System.Text.Encoding.UTF8
        End Select
    End Function
    Function GetWildcards() As String()
        LS("-> GetWildcards")
        If TextBox1.Text <> "" Then
            Return TextBox1.Text.Split(New Char() {";"c})
        ElseIf ComboBox4.SelectedIndex = 0 Then
            Return New String() {""}
        ElseIf ComboBox4.SelectedIndex = 1 Then
            Return New String() {"*.*"}
        Else
            Return New String() {"*.mp3", "*.ogg", "*.wma", "*.fla*", "*.ape", "*.m4a", "*.amr", "*.rm", "*.ra", "*.mid*", "*.wav", "*.aac"}
        End If
    End Function
    Function GetWildcardsAsFilter() As String
        LS("-> GetWildcardsAsFilter")
        Dim ws() As String = GetWildcards()
        GetWildcardsAsFilter = ""
        If ws.Length > 1 Then
            For i As Integer = 0 To ws.Length - 1
                GetWildcardsAsFilter &= ws(i) & "|" & ws(i) & "|"
            Next
            GetWildcardsAsFilter &= "Все типы|"
            For i As Integer = 0 To ws.Length - 2
                GetWildcardsAsFilter &= ws(i) & ";"
            Next
            GetWildcardsAsFilter &= ws(ws.Length - 1) & "|"
        End If
        GetWildcardsAsFilter &= "Все файлы|*.*"
        LS("<- GetWildcardsAsFilter : " & GetWildcardsAsFilter)
    End Function
    Function IsHotTracked(ByVal sender As Control) As Boolean
        If Control.MouseButtons = MouseButtons.None Then
            Dim p As Point = sender.PointToClient(Control.MousePosition)
            If p.X >= 0 And p.X < sender.Width And p.Y >= 0 And p.Y < sender.Height Then Return True
        End If
        Return False
    End Function
    Function IsPlaylist(ByVal Path As String) As Boolean
        LS("-> IsPlaylist, Path = " & Path)
        Path = Path.ToLower
        If Path.ToLower.EndsWith(".m3u") Then Return True
        If Path.ToLower.EndsWith(".m3u8") Then Return True
        If Path.ToLower.EndsWith(".pla") Then Return True
        If Path.ToLower.EndsWith(".plc") Then Return True
        If Path.ToLower.EndsWith(".pls") Then Return True
        If Path.ToLower.EndsWith(".wmpl3") Then Return True
        Return False
    End Function
    Function LT() As String
        If My.Computer.Clock.TickCount > 9999999 Then
            Return My.Computer.Clock.TickCount.ToString
        Else
            Dim tLT As String = My.Computer.Clock.TickCount.ToString()
            Return StrDup(Math.Max(8 - tLT.Length, 0), "0") & tLT
        End If
    End Function
    Function ParseWildcard(ByVal Wildcard As String, ByVal Params() As String, Optional ByVal ReplaceFilePathIncompatibleSymbols As Boolean = True) As String
        LX("-> ParseWildcard: begin @" & Wildcard)
        ParseWildcard = ""
        Dim i As Integer = 0
        While i < Wildcard.Length
            If Wildcard.Chars(i) <> "%" Then
                ParseWildcard &= Wildcard.Chars(i)
            Else
                i += 1
                If i < Wildcard.Length AndAlso Asc(Wildcard.Chars(i)) > &H30 AndAlso Asc(Wildcard.Chars(i)) < &H30 + Params.Length + 1 Then
                    ParseWildcard &= Params(Asc(Wildcard.Chars(i)) - &H31).Replace("\", "_")
                Else
                    ParseWildcard &= "%"
                    i -= 1
                End If
            End If
            i += 1
        End While
        If ReplaceFilePathIncompatibleSymbols Then
            ParseWildcard = ParseWildcard.Replace("?", "_")
            ParseWildcard = ParseWildcard.Replace("*", "_")
            ParseWildcard = ParseWildcard.Replace("/", "_")
            ParseWildcard = ParseWildcard.Replace(":", "_")
            ParseWildcard = ParseWildcard.Replace("""", "_")
            ParseWildcard = ParseWildcard.Replace(">", "_")
            ParseWildcard = ParseWildcard.Replace("<", "_")
            If CheckBox14.Checked Then ParseWildcard = ParseWildcard.Replace(" ", "_")
            If CheckBox15.Checked Then ParseWildcard = ParseWildcard.Replace("_", " ")
        End If
        LX("<- ParseWildcard: " & ParseWildcard)
    End Function
    Function ParseTime(ByVal seconds As Integer) As String
        Static minSecs As Integer = -1
        Static minCache As String = ""
        seconds = seconds + 5 - (seconds Mod 5)
        If minSecs = -1 OrElse seconds - minSecs > 20 Or seconds < minSecs Then
            minSecs = seconds
            Dim Hr, Mn As Integer
            Hr = seconds \ 3600
            seconds -= Hr * 3600
            Mn = seconds \ 60
            seconds -= Mn * 60
            ParseTime = Hr & If(Mn < 10, ":0", ":") & Mn & If(seconds < 10, ":0", ":") & seconds
            minCache = ParseTime
            LX("<- ParseTime: " & ParseTime)
        Else
            ParseTime = minCache
            LX("<- ParseTime (cached): " & ParseTime)
        End If
    End Function
    Function ParsePlaylistPlaTest(ByVal Path As String) As Boolean
        LS("-> ParsePlaylistPlaTest @ " & Path)
        Dim rdr As New IO.StreamReader(Path, System.Text.Encoding.Default)
        rdr.BaseStream.Seek(4, IO.SeekOrigin.Begin)
        Dim buffer(13) As Char
        rdr.Read(buffer, 0, 14)
        rdr.Close()
        rdr.Dispose()
        If New String(buffer) = "iriver UMS PLA" Then Return True
        Return False
    End Function
    Function PathORezq(ByVal Path As String, ByVal Level As Byte) As String
        LX("-> Path-O-RezQ @" & Path & ", lvl:" & Level)
        Dim i, j, k As Integer
        j = 0 : k = 0
        For i = Path.Length - 1 To 0 Step -1
            If Path.Chars(i) = "\" Then
                j += 1
                k = i
                If j > Level Then Return Mid(Path, i + 1)
            End If
        Next
        Achtung("В строке «" & Path & "» обратного слеша нужного уровня (" & Level & ") не обнаружено! ПутеРезка вернула " & If(j > 0, "подстроку максимально достигнутого уровня (" & j & ")!", "исходную подстроку!"))
        Return Mid(Path, k + 1)
    End Function
    Function PathExists(ByVal Path As String) As Boolean
        LX("-> PathExists: question @ " & Path)
        Return UIO.Directory.Exists(Path)
    End Function
    Function RoundRectangle(ByVal rect As Rectangle, ByVal rFractor As Integer) As Drawing2D.GraphicsPath
        Dim path As New Drawing2D.GraphicsPath()
        If rect.Width < rFractor / 2 Then rect.Width = CInt(rFractor / 2)
        If rect.Height < rFractor / 2 Then rect.Height = CInt(rFractor / 2)
        path.AddLine(rect.X + rFractor, rect.Y, rect.X + rect.Width - rFractor, rect.Y)
        path.AddArc(rect.X + rect.Width - rFractor, rect.Y, rFractor, rFractor, 270, 90)
        path.AddLine(rect.X + rect.Width, rect.Y + rFractor, rect.X + rect.Width, rect.Y + rect.Height - rFractor)
        path.AddArc(rect.X + rect.Width - rFractor, rect.Y + rect.Height - rFractor, rFractor, rFractor, 0, 90)
        path.AddLine(rect.X + rect.Width - rFractor, rect.Y + rect.Height, rect.X + rFractor, rect.Y + rect.Height)
        path.AddArc(rect.X, rect.Y + rect.Height - rFractor, rFractor, rFractor, 90, 90)
        path.AddLine(rect.X, rect.Y + rect.Height - rFractor, rect.X, rect.Y + rFractor)
        path.AddArc(rect.X, rect.Y, rFractor, rFractor, 180, 90)
        Return path
    End Function
#End Region
End Class