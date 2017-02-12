Module License
	Dim bw As IO.FileStream
	Structure LicInfo
		Dim UserName As String
		Dim IssuedBy As String
		Dim ExtInfo As String
		Dim ActivationDate As ULong
		Dim LastUsageDate As ULong
		Dim DaysValid As UShort
		Dim GPL As Boolean
		Sub New(ByVal UserName As String, ByVal IssuedBy As String, ByVal ExtInfo As String, ByVal ActivationDate As ULong, ByVal LastUsageDate As ULong, ByVal DaysValid As UShort, ByVal IsGPL As Boolean)
			Me.UserName = UserName
			Me.IssuedBy = IssuedBy
			Me.ExtInfo = ExtInfo
			Me.ActivationDate = ActivationDate
			Me.LastUsageDate = LastUsageDate
			Me.DaysValid = DaysValid
			Me.GPL = IsGPL
		End Sub
		Function Humanize() As HumanizedLicInfo
			Return New HumanizedLicInfo(Me)
		End Function
	End Structure
	Structure HumanizedLicInfo
		Dim UserName As String
		Dim IssuedBy As String
		Dim ExtInfo As String
		Dim ActivationDate As String
		Dim LastUsageDate As String
		Dim DaysValid As String
		Dim EndDate As String
		Dim DaysLeft As String
		Sub New(ByVal SourceLic As LicInfo)
			UserName = SourceLic.UserName
			IssuedBy = SourceLic.IssuedBy
			ExtInfo = SourceLic.ExtInfo
			If SourceLic.ActivationDate = 0 Then
				ActivationDate = "не активирована"
			Else
				If ULong2Date(SourceLic.ActivationDate) = Now2Date() Then
					ActivationDate = "сегодня"
				Else
					ActivationDate = ULong2String(SourceLic.ActivationDate)
				End If
			End If
			If SourceLic.LastUsageDate = 0 Then
				LastUsageDate = "не известно"
			Else
				If ULong2Date(SourceLic.LastUsageDate) = Now2Date() Then
					LastUsageDate = "cегодня"
				Else
					LastUsageDate = ULong2String(SourceLic.LastUsageDate)
				End If
			End If
			If SourceLic.DaysValid < 65535 Then
				DaysValid = SourceLic.DaysValid.ToString & " дней"
				If SourceLic.ActivationDate = 0 Then
					EndDate = "не определена"
					DaysLeft = "не определено"
				Else
					Dim dEnd As Date = ULong2Date(SourceLic.ActivationDate).AddDays(SourceLic.DaysValid)
					EndDate = ULong2String(Date2ULong(dEnd))
					DaysLeft = (dEnd - Now2Date()).Days.ToString & " дней"
				End If
			Else
				DaysValid = "бесконечно"
				EndDate = "отсутствует"
				DaysLeft = "бесконечность"
			End If
		End Sub
	End Structure
	Function GetLicense(ByVal BypassDateTests As Boolean) As LicInfo
		If IO.File.Exists(My.Application.Info.DirectoryPath & "\License.hdk") Then
			bw = New IO.FileStream(My.Application.Info.DirectoryPath & "\License.hdk", IO.FileMode.Open, IO.FileAccess.Read)
			If bw.Length > 2 Then
				Dim lic(CInt(bw.Length - 1)) As Byte
				bw.Read(lic, 0, CInt(bw.Length))
				bw.Close()
				bw.Dispose()
				If lic(0) = Asc("H") And lic(1) = Asc("D") And lic(2) = Asc("K") Then
					Dim cs As Long = 0
					For i As Integer = 3 To lic.Length - 3 Step 4
						cs += lic(i)
					Next
					If (cs And &HFF) = lic(lic.Length - 2) And ((cs >> 8) And &HFF) = lic(lic.Length - 1) Then
						Dim k As Byte = lic(lic.Length - 3) Xor CByte(195)
						Dim ptr As Integer = 3
						Dim b As Byte = DecodeByte(lic, ptr, k)
						Dim col As New Collections.Generic.List(Of Char)
						b = DecodeByte(lic, ptr, k)
						While b <> 0
							col.Add(Chr(b))
							b = DecodeByte(lic, ptr, k)
						End While
						Dim CharArray(col.Count - 1) As Char
						col.CopyTo(CharArray)
						GetLicense.ExtInfo = New String(CharArray)
						col.Clear()
						b = DecodeByte(lic, ptr, k)
						While b <> 1
							col.Add(Chr(b))
							b = DecodeByte(lic, ptr, k)
						End While
						ReDim CharArray(col.Count - 1)
						col.CopyTo(CharArray)
						GetLicense.IssuedBy = New String(CharArray)
						col.Clear()
						b = DecodeByte(lic, ptr, k)
						While b <> 2
							col.Add(Chr(b))
							b = DecodeByte(lic, ptr, k)
						End While
						ReDim CharArray(col.Count - 1)
						col.CopyTo(CharArray)
						GetLicense.UserName = New String(CharArray)
						GetLicense.DaysValid = DecodeByte(lic, ptr, k) * CUShort(256) + DecodeByte(lic, ptr, k)
						For b = 0 To 7
							GetLicense.ActivationDate += DecodeByte(lic, ptr, k) * CULng(256 ^ b)
						Next
						For b = 0 To 7
							GetLicense.LastUsageDate += DecodeByte(lic, ptr, k) * CULng(256 ^ b)
						Next
						If Not BypassDateTests Then
							If GetLicense.ActivationDate = 0 Then
								ActivUaLice(GetLicense)
								Return License.GetLicense(BypassDateTests)
							End If
							Dim d1, d2 As Date
							d1 = ULong2Date(GetLicense.LastUsageDate)
							d2 = ULong2Date(GetLicense.ActivationDate)
							If d1 < d2 Then Return GPL(0, GetLicense.LastUsageDate, "Дата активации неверна!")
							d2 = d1
							d1 = Now2Date()
							If d1 < d2 Then Return GPL(0, GetLicense.LastUsageDate, "Системная дата неверна!")
							d2 = ULong2Date(GetLicense.ActivationDate)
							d2 = d2.AddDays(GetLicense.DaysValid)
							If d1 > d2 Then Return GPL(GetLicense.ActivationDate, GetLicense.LastUsageDate, "Ваша персональная лицензия истекла!")
						End If
						GetLicense.GPL = False
					Else
						Return GPL("Ошибка в файле лицензии!")
					End If
				Else
					Return GPL("Файл лицензии не опознан как таковой!")
				End If
			Else
				bw.Close()
				bw.Dispose()
				Return GPL("Файл лицензии, по-видимому, повреждён!")
			End If
		Else
			Return GPL("Файл персональной лицензии не найден!")
		End If
	End Function
	Sub SetLicense(ByVal License As LicInfo)
		bw = New IO.FileStream(My.Application.Info.DirectoryPath & "\License.hdk", IO.FileMode.Create, IO.FileAccess.ReadWrite, IO.FileShare.ReadWrite)
		bw.WriteByte(Asc("H"))
		bw.WriteByte(Asc("D"))
		bw.WriteByte(Asc("K"))
		Dim k As Byte = CByte(Rnd() * 255)
		Dim sk As Byte = CByte(Rnd() * 255)
		WriteByteEncoded(sk, k)
		For i As Integer = 0 To License.ExtInfo.Length - 1
			WriteByteEncoded(CByte(Asc(License.ExtInfo.Chars(i))), k)
		Next
		WriteByteEncoded(0, k)
		For i As Integer = 0 To License.IssuedBy.Length - 1
			WriteByteEncoded(CByte(Asc(License.IssuedBy.Chars(i))), k)
		Next
		WriteByteEncoded(1, k)
		For i As Integer = 0 To License.UserName.Length - 1
			WriteByteEncoded(CByte(Asc(License.UserName.Chars(i))), k)
		Next
		WriteByteEncoded(2, k)
		WriteByteEncoded(CByte(License.DaysValid >> 8), k)
		WriteByteEncoded(CByte(License.DaysValid And &HFF), k)
		For i As Integer = 0 To 7
			WriteByteEncoded(CByte((License.ActivationDate >> i * 8) And CULng(&HFF)), k)
		Next
		For i As Integer = 0 To 7
			WriteByteEncoded(CByte((License.LastUsageDate >> i * 8) And CULng(&HFF)), k)
		Next
		bw.WriteByte(CByte(k Xor 195)) '11000011
		'-----------------------------------------
		Dim ptr As Long = bw.Position
		bw.Seek(3, IO.SeekOrigin.Begin)
		Dim cs As Long = bw.ReadByte
		While bw.Position + 3 < ptr
			bw.Seek(3, IO.SeekOrigin.Current)
			cs += bw.ReadByte
		End While
		bw.Seek(ptr, IO.SeekOrigin.Begin)
		bw.WriteByte(CByte(cs And CLng(&HFF)))
		cs = cs >> 8
		bw.WriteByte(CByte(cs And CLng(&HFF)))
		bw.Flush()
		bw.Close()
	End Sub
	Sub ActivUaLice(ByVal LI As LicInfo)
		'Activate and|or actualise existing license file
		If IO.File.Exists(My.Application.Info.DirectoryPath & "\License.hdk") Then
			If LI.ActivationDate = 0 Then LI.ActivationDate = Now2ULong()
			LI.LastUsageDate = Now2ULong()
			SetLicense(LI)
		End If
	End Sub
	Sub ActivUaLice()
		ActivUaLice(GetLicense(False))
	End Sub
	Sub WriteByteEncoded(ByVal b As Byte, ByVal k As Byte)
		bw.WriteByte(b Xor k)
		b = (b Or CByte(8)) And CByte(15)
		For i As Byte = 0 To b
			bw.WriteByte(CByte(Rnd() * 255))
		Next
	End Sub
	Function DecodeByte(ByVal lic() As Byte, ByRef ptr As Integer, ByVal k As Byte) As Byte
		Dim b As Byte = lic(ptr) Xor k
		ptr += (((b Or 8) And 15) + 2)
		Return b
	End Function
	Function GPL(ByVal Activated As ULong, ByVal LastUsed As ULong, ByVal ExtInfo As String) As LicInfo
		Return New LicInfo("Общая публичная лицензия (GPL)", "HACK-Design Licensing Authority", ExtInfo, Activated, LastUsed, &HFFFF, True)
	End Function
	Function GPL(ByVal ExtInfo As String) As LicInfo
		Return GPL(0, 0, ExtInfo)
	End Function
	Function CutLevels(ByVal Number As ULong, ByVal Base As Byte, ByVal Top As Byte) As Integer
		If Base > Top Then
			MsgBox("CutLevels call error!", MsgBoxStyle.Critical)
			Return 0
		Else
			Return CInt(Int((Number - Int(Number / (10 ^ (9 - Base))) * (10 ^ (9 - Base))) / (10 ^ (8 - Top))))
		End If
	End Function
	Function Now2Date() As Date
		Return New Date(Now.Year, Now.Month, Now.Day)
	End Function
	Function Now2ULong() As ULong
		Return Date2ULong(Now)
	End Function
	Function Date2ULong(ByVal d As Date) As ULong
		Return CULng(d.Day * 1000000 + d.Month * 10000 + d.Year)
	End Function
	Function ULong2Date(ByVal uNumber As ULong) As Date
		Try
			Return New Date(CutLevels(uNumber, 5, 8), CutLevels(uNumber, 3, 4), CutLevels(uNumber, 1, 2))
		Catch ex As Exception
			Return Nothing
		End Try
	End Function
	Function ULong2String(ByVal uNumber As ULong) As String
		Return Format(CutLevels(uNumber, 1, 2), "00") & "." & Format(CutLevels(uNumber, 3, 4), "00") & "." & Format(CutLevels(uNumber, 5, 8), "0000")
	End Function
End Module