Public Class AutoCross
	Dim result As Decimal
	Overloads Function ShowDialog(ByVal files() As String, ByVal owner As Form) As Decimal
		Form1.LS("AutoCross::ShowDialog: начало, файлов - " & files.Length) : Form1.LogLevel += 1
		Me.Show(owner)
		Me.Top = CInt(owner.Top + (owner.Height - Me.Height) / 2)
		Me.Left = CInt(owner.Left + (owner.Width - Me.Width) / 2)
		Dim t As Integer = My.Computer.Clock.TickCount
		While My.Computer.Clock.TickCount - t < 150
			Application.DoEvents()
		End While
		Form1.LS("Расположились...")
		Dim data(files.Length - 1) As Integer
		Dim tpla As Microsoft.DirectX.AudioVideoPlayback.Audio
		For i As Integer = 0 To files.Length - 1
			data(i) = My.Computer.Clock.TickCount
			Try
				tpla = New Microsoft.DirectX.AudioVideoPlayback.Audio(files(i), False)
			Catch ex As Exception
			End Try
			data(i) = My.Computer.Clock.TickCount - data(i)
			Form1.LX("iteration " & i & " file = " & files(i) & " value = " & data(i))
		Next
		Form1.LS("Тест завершён, начинается обработка результатов")
		'Min & Max
		Array.Sort(data)
		Label2.Text = data(0).ToString
		Label4.Text = data(data.Length - 1).ToString
		'Mid
		Dim j As Integer = 0
		For i As Integer = 0 To data.Length - 1
			j += data(i)
		Next
		Label3.Text = Math.Round(j / data.Length).ToString
		Form1.LS("Простой поиск завершён, начинается поиск повторов")
		'Pop
		j = 1
		Dim last As Integer = data(0)
		Dim k_max As Integer = 1
		Dim k_val As Object = Nothing
		For i As Integer = 1 To data.Length - 1
			If last = data(i) Then
				j += 1
				If j > k_max Then
					k_max = j
					k_val = last
				End If
			Else
				j = 1
				last = data(i)
			End If
		Next
		If k_val Is Nothing Then
			Label5.Text = "(повторов нет)"
			Button4.Enabled = False
		Else
			Label5.Text = k_val.ToString & ": " & k_max.ToString & IIf(k_max > 4, " случаев", " случая").ToString
			Button4.Enabled = True
		End If
		Form1.LS("Поиск повторов: " & Label5.Text)
		'Dialog
		Me.Hide()
		Me.Height = 92
		Label1.Text = "Выберите значение:"
		If Me.ShowDialog(owner) = Windows.Forms.DialogResult.Cancel Then result = Form1.NumericUpDown1.Value
		Me.Height = 38
		Label1.Text = "Пожалуйста подождите, выполняется тестирование..."
		Form1.LogLevel -= 1 : Form1.LS("AutoCross::ShowDialog: конец, результат: " & result)
		Return result
	End Function
	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		result = CDec(Label2.Text)
		Me.Close()
	End Sub
	Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
		result = CDec(Label3.Text)
		Me.Close()
	End Sub
	Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
		result = CDec(Label4.Text)
		Me.Close()
	End Sub
	Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
		Dim a() As String = Label5.Text.Split(New Char() {":"c})
		If a.Length = 1 Then
			MsgBox("Алярм!!!! Возникла ситуация, которая не могла возникнуть!!!")
			Button2_Click(sender, e)
		Else
			result = CDec(a(0))
			Me.Close()
		End If
	End Sub
End Class