Public Class Register
	Shadows Function ShowDialog() As DialogResult
		CLB_in.Items.Clear()
		CLB_out.Items.Clear()
		For i As Integer = 0 To Form1.ComboBox1.Items.Count - 1
			CLB_in.Items.Add(Form1.ComboBox1.Items(i).ToString)
			CLB_in.SetItemChecked(i, True)
		Next
		For i As Integer = 0 To Form1.ComboBox2.Items.Count - 1
			CLB_out.Items.Add(Form1.ComboBox2.Items(i).ToString)
			CLB_out.SetItemChecked(i, True)
		Next
		Return MyBase.ShowDialog
	End Function
	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		Dim k As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.CreateSubKey("Software\HACK-Design\PLReNewer\")
		k.SetValue("In_Count", CLB_in.CheckedIndices.Count)
		k.SetValue("Out_Count", CLB_out.CheckedIndices.Count)
		If CLB_in.CheckedIndices.Count > 0 Then
			Dim j As Integer = 0
			For i As Integer = 0 To CLB_in.Items.Count - 1
				If CLB_in.GetItemChecked(i) Then
					k.SetValue("In" & j.ToString, CLB_in.Items(i).ToString)
					j += 1
				End If
			Next
		End If
		If CLB_out.CheckedIndices.Count > 0 Then
			Dim j As Integer = 0
			For i As Integer = 0 To CLB_out.Items.Count - 1
				If CLB_out.GetItemChecked(i) Then
					k.SetValue("Out" & j.ToString, CLB_out.Items(i).ToString)
					j += 1
				End If
			Next
		End If
		k.Close()
	End Sub
End Class