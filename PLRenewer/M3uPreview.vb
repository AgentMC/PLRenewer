Public Class M3uPreview
	Dim Encoding As Integer
	Dim iPath As String
	Structure iDialogResult
		Dim DialogResult As Windows.Forms.DialogResult
		Dim Encoding As System.Text.Encoding
	End Structure
	Public Overloads Function ShowDialog(ByVal Path As String) As iDialogResult
		Form1.LS("M3uPreview::ShowDialog: начало @ " & Path) : Form1.LogLevel += 1
		iPath = Path
		If RadioButton1.Checked Then
			Encoding = 1251
			LoadAtEnc()
		Else
			RadioButton1.Checked = True
		End If
		ShowDialog.DialogResult = Me.ShowDialog()
		ShowDialog.Encoding = System.Text.Encoding.GetEncoding(Encoding)
		Form1.LogLevel -= 1 : Form1.LS("M3uPreview::ShowDialog: конец @ " & Encoding)
	End Function
	Private Sub EncodingChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged, RadioButton2.CheckedChanged, RadioButton3.CheckedChanged, RadioButton4.CheckedChanged, RadioButton5.CheckedChanged, RadioButton6.CheckedChanged
		If CType(sender, RadioButton).Checked Then
			Encoding = CInt(CType(sender, RadioButton).Text.Split(New String() {" - "}, StringSplitOptions.RemoveEmptyEntries)(0))
			Form1.LS("M3uPreview::EncodingChanged @ " & Encoding)
			LoadAtEnc()
		End If
	End Sub
	Sub LoadAtEnc()
		Button2.Text = "Ok (cp" & Encoding & ")"
		Form1.LS("-> M3uPreview::LoadAtEnc @ " & Encoding)
		Try
			Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(Encoding)
			Form1.LS("M3uPreview::LoadAtEnc. Encoding acquired.")
			Try
				Dim rdr As New IO.StreamReader(iPath, enc, False)
				Form1.LS("M3uPreview::LoadAtEnc. Reader opened.")
				Dim ins As String = rdr.ReadToEnd
				Form1.LS("M3uPreview::LoadAtEnc. Reader done.")
				rdr.Close()
				rdr.Dispose()
				Form1.LS("M3uPreview::LoadAtEnc. Reader killed.")
				Dim i As Integer = InStr(ins, Chr(13))
				If Mid(ins, i + 1, 1) <> Chr(10) Then
					ins = ins.Replace(Chr(10), Chr(13) & Chr(10))
					Me.Text = "Выберите подходящую кодировку (ВНИМАНИЕ: *NIX-РЕЖИМ!!!)"
					Form1.LS("M3uPreview::LoadAtEnc. Unix mode activeted. Re-decoded for windows.")
				Else
					Me.Text = "Выберите подходящую кодировку плейлиста"
				End If
				ListBox1.Items.Clear()
				Application.DoEvents()
				Dim ins_str() As String = ins.Split(New String() {Chr(13) & Chr(10)}, StringSplitOptions.RemoveEmptyEntries)
				For j As Integer = 0 To ins_str.Length - 1
					ListBox1.Items.Add(ins_str(j))
					If j Mod 50 = 0 Then Application.DoEvents()
				Next
				'ListBox1.Items.AddRange(ins_str)
				Form1.LS("<- M3uPreview::LoadAtEnc: finished")
			Catch ex As Exception
				Form1.LS("<- M3uPreview::LoadAtEnc: ex2 = " & ex.Message)
				MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Ошибка открытия плейлиста!")
			End Try
		Catch ex As Exception
			Form1.LS("<- M3uPreview::LoadAtEnc: ex1 = " & ex.Message)
			MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Ошибка открытия кодировки!")
		End Try
	End Sub
	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		Form1.LS("M3uPreview::GUI. Button #1 pressed.")
		Encoding = CInt(TextBox2.Text)
		LoadAtEnc()
	End Sub
	Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
		Dim trys As Boolean = True
		If TextBox2.TextLength > 0 Then
			For i As Integer = 0 To TextBox2.TextLength - 1
				If Asc(TextBox2.Text.Chars(i)) > &H39 Or Asc(TextBox2.Text.Chars(i)) < &H30 Then trys = False
			Next
		Else
			trys = False
		End If
		Form1.LX("M3uPreview::GUI. TextBox2_tc @ " & TextBox2.Text & ", trys = " & trys.ToString)
		Button1.Enabled = trys
	End Sub
	Private Sub RadioButton7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton7.CheckedChanged
		Form1.LX("M3uPreview::GUI. RadioButton7_CheckedChanged @ " & RadioButton7.Checked.ToString)
		If RadioButton7.Checked Then
			TextBox2.Enabled = True
			TextBox2_TextChanged(Nothing, Nothing)
		Else
			Button1.Enabled = False
			TextBox2.Enabled = False
		End If
	End Sub
End Class