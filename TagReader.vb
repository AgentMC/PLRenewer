Module TagReader
#Region "Основа"
	Interface iLoggerHost
		Sub LS(ByVal txt As String)
		Sub LX(ByVal txt As String)
		Sub Achtung(ByVal txt As String)
		Property LogLevel As Integer
	End Interface
	Public host As iLoggerHost
	Structure AudioTag
		Public Sub New(ByVal DefaultText As String)
			Artist = DefaultText
			Album = DefaultText
			Year = DefaultText
			Genre = DefaultText
			Comments = DefaultText
			Number = DefaultText
			Track = DefaultText
			Valid = False
			Empty = False
		End Sub
		Public Sub Normalize(ByVal ConsiderEmptyYearAsError As Boolean, Optional ByVal FixTagVisualization As Boolean = True)
			If Album = "" Or Artist = "" Or (Year = "" And ConsiderEmptyYearAsError) Or Number = "" Or Track = "" Then Valid = False

			If Year.Length = 3 Or Year.Length = 1 Then
				Valid = False
			ElseIf Year.Length = 2 Then
				If Char.IsNumber(Year(0)) And Char.IsNumber(Year(1)) Then
					Year = (CInt(Year) + If(CInt(Year) < Now.Year Mod 100, 1900, 2000)).ToString()
				Else
					Valid = False
				End If
			End If
			If Number.Length > 2 Then
				Dim nIdx As Integer = Number.IndexOf("/"c)
				If nIdx > -1 Then Number = Format(CInt(Mid(Number, 1, nIdx)), "00")
				nIdx = Number.IndexOf("\"c)
				If nIdx > -1 Then Number = Format(CInt(Mid(Number, 1, nIdx)), "00")
				nIdx = Number.IndexOf("_"c)
				If nIdx > -1 Then Number = Format(CInt(Mid(Number, 1, nIdx)), "00")
			ElseIf Number.Length = 1 Then
				If Char.IsNumber(Number(0)) Then Number = Format(CInt(Number), "00") Else Number = "00"
			End If
			Track = Track.Trim(New Char() {"	"c, " "c, "("c, ")"c, "["c, "]"c, "{"c, "}"c})

			If FixTagVisualization Then
				If Album = "" Then Album = "Неизвестный альбом"
				If Artist = "" Then Artist = "Неизвестный исполнитель"
				If Year = "" Then Year = "0000"
				If Genre = "" Then Genre = "Неизвестный жанр"
				If Comments = "" Then Comments = ""
				If Number = "" Then Number = "00"
				If Track = "" Then Track = IO.Path.GetFileNameWithoutExtension(rs.Name)
			End If
		End Sub
		Public Sub CopyTo(ByRef t As AudioTag)
			t.Album = Album
			t.Artist = Artist
			t.Comments = Comments
			t.Genre = Genre
			t.Number = Number
			t.Track = Track
			t.Valid = Valid
			t.Year = Year
			t.Empty = Empty
		End Sub
		Overrides Function ToString() As String
			If Empty Then
				Return "<empty>"
			ElseIf Valid = False Then
				Return "<invalid>: Ar[" & Artist & "]Al[" & Album & "]Yr[" & Year & "]Tr[" & Track & "]Gn[" & Genre & "]Cm[" & Comments & "]Nr[" & Number & "]"
			Else
				Return "Ar[" & Artist & "]Al[" & Album & "]Yr[" & Year & "]Tr[" & Track & "]Gn[" & Genre & "]Cm[" & Comments & "]Nr[" & Number & "]"
			End If
		End Function
		Dim Artist As String
		Dim Album As String
		Dim Year As String
		Dim Genre As String
		Dim Comments As String
		Dim Number As String
		Dim Track As String
		Dim Valid As Boolean
		Dim Empty As Boolean
	End Structure
	Public Enum ParseMode
		None = 0
		MP3 = 1
		WMA = 2
		OGG = 3
		FLAC = 4
	End Enum
	Enum ID3Mode
		Level1 = 1
		Level2 = 2
		Both = 3
		NoTpe1Marker = 4 'marker only. Do not use when calling GenTag()
		'Level1NoTpe1 = 5 'senseless item. Level1 doesn't contain named tags
		Level2NoTpe1 = 6
		BothNoTpe1 = 7
		NoTpe2Marker = 8 'marker only. Do not use when calling GenTag()
		'Level1NoTpe2 = 9 'senseless item. Level1 doesn't contain named tags
		Level2NoTpe2 = 10
		BothNoTpe2 = 11
		'Any other combinations are senseless and unusable
	End Enum
	Dim rs As IO.FileStream
	Dim br As IO.BinaryReader
	Private Function TestTagInternal(ByVal ID3Level As ID3Mode) As ParseMode
		host.LX("TestTAG__i: begin @ " & rs.Name & ", ID3Mode set to " & ID3Level)
		Dim str As String
		rs.Seek(-128, IO.SeekOrigin.End)
		str = Chr(rs.ReadByte) & Chr(rs.ReadByte) & Chr(rs.ReadByte)
		If str = "TAG" And ((ID3Level And ID3Mode.Level1) > 0) Then Return ParseMode.MP3
		host.LX("TestTAG__i: TAG subcription not found: " & str)
		rs.Seek(0, IO.SeekOrigin.Begin)
		str = Chr(rs.ReadByte) & Chr(rs.ReadByte) & Chr(rs.ReadByte)
		If str = "ID3" And ((ID3Level And ID3Mode.Level2) > 0) Then Return ParseMode.MP3
		host.LX("TestTAG__i: ID3 subcription not found: " & str)
		rs.Seek(0, IO.SeekOrigin.Begin)
		Dim g As Guid = readGUID() 'ВМА-файл начинается с ГУИД его, и если он не стандартный, то ... см. ниже
		If g = hdrGUID Then Return ParseMode.WMA
		host.LX("TestTAG__i: ASF Header GUID not found: " & g.ToString)
		rs.Seek(0, IO.SeekOrigin.Begin)
		str = Chr(rs.ReadByte) & Chr(rs.ReadByte) & Chr(rs.ReadByte)
		If str = "Ogg" Then Return ParseMode.OGG
		host.LX("TestTAG__i: Ogg subcription not found: " & str)
		rs.Seek(0, IO.SeekOrigin.Begin)
		str = Chr(rs.ReadByte) & Chr(rs.ReadByte) & Chr(rs.ReadByte) & Chr(rs.ReadByte)
		If str = "fLaC" Then Return ParseMode.FLAC
		host.LX("TestTAG__i: fLaC subcription not found: " & str)
		Return ParseMode.None
	End Function
	Public Function GenTag(ByVal Filename As String, ByVal ID3Level As ID3Mode, Optional ByVal ConsiderEmptyYearAsError As Boolean = False, Optional ByVal PerformVisualNormalization As Boolean = True) As AudioTag
		If host IsNot Nothing Then
			host.LS("GenTag: begin @" & Filename)
			host.LogLevel += 1
			GenTag = New AudioTag
			Try
				If rs Is Nothing Then rs = New IO.FileStream(Filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
				If br Is Nothing Then br = New IO.BinaryReader(rs, System.Text.Encoding.Default)
				Dim PM As ParseMode = TestTag(Filename, ID3Level)
				host.LS("GenTag: mode = " & PM.ToString)
				If PM = ParseMode.MP3 Then
					GenTag = ReadMP3Tag(ID3Level)
				ElseIf PM = ParseMode.WMA Then
					GenTag = ReadWMATag()
				ElseIf PM = ParseMode.OGG Then
					GenTag = ReadOGGTag()
				ElseIf PM = ParseMode.FLAC Then
					GenTag = ReadFLACTag()
				Else
					GenTag = New AudioTag("Tag not detected.")
					GenTag.Valid = False
					GenTag.Empty = True
				End If
				GenTag.Normalize(ConsiderEmptyYearAsError, PerformVisualNormalization)
				br.Close()
				rs.Dispose()
			Catch ex As Exception
				host.Achtung("Невозможно прочесть тег в " & Filename & ": " & ex.Message)
			End Try
			br = Nothing
			rs = Nothing
			host.LogLevel -= 1
			host.LS("GenTag returns «" & GenTag.ToString & "»")
		Else
			Throw New ArgumentException("Logging host not set! Please provide module with an iLoggerHost implementation (TagReader.host=<your logging host>) before calling GenTag().", "host")
		End If
	End Function
	'Warning!!!! Public TestTag and CloseFile are currently intended to be used as widening alternative for 
	'GenTag routine for MP3 files only!!!
	'Example:
	'
	'If TagReader.TestTag(Filename, Level) = TagReader.ParseMode.MP3 then
	'	tag = TagReader.ReadMP3Tag(Level)
	'	tag.Artist = "Me"
	'	TagReader.CloseFile()
	'	TagWriter.WriteMp3Tag(tag, Filename)
	'Else
	'	TagReader.CloseFile()
	'End If
	'
	'REMEMBER always to call CloseFile() after TestTag()!!!!
	Public Function TestTag(ByVal Filename As String, ByVal ID3Level As ID3Mode) As ParseMode
		If host IsNot Nothing Then
			host.LS("TestTag: begin @" & Filename)
			host.LogLevel += 1
			If rs Is Nothing Then rs = New IO.FileStream(Filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
			If br Is Nothing Then br = New IO.BinaryReader(rs, System.Text.Encoding.Default)
			Dim PM As ParseMode = TestTagInternal(ID3Level)
			host.LogLevel -= 1
			host.LS("TestTag: mode = " & PM.ToString)
			Return PM
		Else
			Throw New ArgumentException("Logging host not set! Please provide module with an iLoggerHost implementation (TagReader.host=<your logging host>) before calling GenTag().", "host")
		End If
	End Function
	Public Sub CloseFile()
		br.Close()
		rs.Dispose()
		br = Nothing
		rs = Nothing
	End Sub
#End Region
#Region "Общие функции"
	Friend Function Razor(ByVal text As String) As String
		host.LX("Razor: begin @ " & text)
		Razor = ""
		Dim i As Integer = text.Length
		While i > 0 AndAlso Asc(Mid(text, i, 1)) < 33
			i -= 1
		End While
		If i > 0 Then Razor = Mid(text, 1, i)
		host.LX("Razor: end @ " & Razor)
	End Function
	Private Function ByteString2UtfBytesArray(ByVal input As String, ByVal SrcEncoding As System.Text.Encoding) As Byte()
		Dim bytes(input.Length - 1) As Byte
		For i As Integer = 0 To input.Length - 1
			bytes(i) = CByte(Asc(input.Chars(i)))
		Next
		If SrcEncoding Is Nothing Then Return bytes
		Return System.Text.Encoding.Convert(SrcEncoding, System.Text.Encoding.Unicode, bytes)
	End Function
	Private Function DecodeUTF(ByVal bytes() As Byte) As String
		Dim sb As New System.Text.StringBuilder
		For i As Integer = 0 To bytes.Length - 2 Step 2
			sb.Append(ChrW(bytes(i + 1) * 256 + bytes(i)))
		Next
		Return sb.ToString
	End Function
	Private Function DecodeUTF(ByVal Input As String) As String
		Return DecodeUTF(ByteString2UtfBytesArray(Input, Nothing))
	End Function
	Private Function DecodeUTF8(ByVal bytes() As Byte) As OGGVarPair
		'Dim decodedBytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, bytes)
		'Dim DecUTF8 As String = ""
		'Dim chars(decodedBytes.Length - 1) As Char
		'For i As Integer = 0 To decodedBytes.Length - 1
		'	chars(i) = Chr(decodedBytes(i))
		'Next
		'DecUTF8 = New String(chars)
		'Return New OGGVarPair(DecUTF8.Split(New String() {"="}, 2, StringSplitOptions.None))
		Return New OGGVarPair(DecodeUTF(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Unicode, bytes)).Split(New String() {"="}, 2, StringSplitOptions.None))
	End Function
	Private Function DecodeUTF8(ByVal Input As String) As String
		Return DecodeUTF(ByteString2UtfBytesArray(Input, System.Text.Encoding.UTF8))
	End Function
	Friend Function DecodeUTFBE(ByVal bytes() As Byte) As String
		Return DecodeUTF(System.Text.Encoding.Convert(System.Text.Encoding.BigEndianUnicode, System.Text.Encoding.Unicode, bytes))
	End Function
	Private Function DecodeUTFBE(ByVal Input As String) As String
		Return DecodeUTF(ByteString2UtfBytesArray(Input, System.Text.Encoding.BigEndianUnicode))
	End Function
#End Region
#Region "MP3"
	Public Function ReadMP3Tag(ByVal il As ID3Mode) As AudioTag
		ReadMP3Tag = New AudioTag("")
		Dim Tag1 As New AudioTag("")
		Dim Tag2 As New AudioTag("")
		Dim Bytes128(127) As Byte
		host.LX("ReadMP3Tag: reading level1 tag...") : host.LogLevel += 1
		Dim Decoded As String = ""
		If (il And ID3Mode.Level1) > 0 Then
			rs.Seek(-128, IO.SeekOrigin.End)
			rs.Read(Bytes128, 0, 128)
			host.LX("ReadMP3Tag: working on level1 tag...")
			Decoded = System.Text.Encoding.Default.GetString(Bytes128)
			host.LX("ReadMP3Tag: level1 tag = «" & Decoded & "»")
			If Decoded.StartsWith("TAG") Then
				Tag1.Track = Razor(Mid(Decoded, 4, 30))
				Tag1.Artist = Razor(Mid(Decoded, 34, 30))
				Tag1.Album = Razor(Mid(Decoded, 64, 30))
				Tag1.Year = Razor(Mid(Decoded, 94, 4))
				If Bytes128(125) = 0 Then
					Tag1.Comments = Razor(Mid(Decoded, 98, 28))
					Tag1.Number = If(Bytes128(126) > 0, Format(Bytes128(126), "00"), "")
				Else
					Tag1.Comments = Razor(Mid(Decoded, 98, 30))
				End If
				Tag1.Valid = True
				If Bytes128(127) < sGenreMatrix.Length Then Tag1.Genre = sGenreMatrix(Bytes128(127))
			End If
			host.LX("ReadMP3Tag: level1 MP3tag = «" & Tag1.ToString & "»")
		Else
			host.LX("ReadMP3Tag: work over level1 tag skipped due to flag not set")
		End If
		host.LogLevel -= 1
		Decoded = ""
		host.LX("ReadMP3Tag: reading level2 tag header...")
		If (il And ID3Mode.Level2) > 0 Then
			Dim Id3Hdr(9) As Byte
			Dim Id3Sz As Integer
			rs.Seek(0, IO.SeekOrigin.Begin)
			rs.Read(Id3Hdr, 0, 10)
			Id3Sz = TagWriter.GetIntFromSyncSafeInt(Id3Hdr, 6)	'4 * %0xxxxxxx
			If Id3Sz > 0 And Id3Hdr(0) = &H49 And Id3Hdr(1) = &H44 And Id3Hdr(2) = &H33 Then '"ID3"
				host.LX("ReadMP3Tag: level2 tag version = " & Id3Hdr(3))
				If Id3Hdr(3) < 5 And Id3Hdr(3) > 1 Then
					host.LX("ReadMP3Tag: reading level2 tag, size = " & Id3Sz)
					Dim BytesTAG(Id3Sz - 1) As Byte
					rs.Read(BytesTAG, 0, Id3Sz)
					host.LX("ReadMP3Tag: working level2 tag...")
					host.LogLevel += 1
					Decoded = System.Text.Encoding.Default.GetString(BytesTAG)
					host.LX("ReadMP3Tag: level2 tag = «" & Decoded & "»")
					Dim NextFramePointer As Integer
					Dim FrameType As String
					Dim FrameSize As Integer
					Dim FrameContent As String
					Dim FrameStringFormat As ID3StringFmt
					If Id3Hdr(3) = 2 Then
						NextFramePointer = 0
						host.LogLevel += 1
						While NextFramePointer < Id3Sz - 1 AndAlso BytesTAG(NextFramePointer) <> 0
							host.LX("ReadMP3Tag: Innerloop: NFP = " & NextFramePointer)
							FrameType = Mid(Decoded, NextFramePointer + 1, 3)
							host.LX("ReadMP3Tag: Innerloop: FT = " & FrameType)
							FrameSize = CInt(BytesTAG(NextFramePointer + 3) * 256 ^ 2)
							FrameSize += BytesTAG(NextFramePointer + 4) * 256
							FrameSize += BytesTAG(NextFramePointer + 5)
							host.LX("ReadMP3Tag: Innerloop: FS = " & FrameSize)
							If FrameSize = 0 Or NextFramePointer + FrameSize + 5 >= Id3Sz Then Exit While
							If FrameSize > 1 And FrameType <> "PIC" Then
								FrameStringFormat = DirectCast(BytesTAG(NextFramePointer + 6), ID3StringFmt)
								host.LX("ReadMP3Tag: Innerloop: FSF = " & FrameStringFormat)
								If FrameType = "COM" Then
									Select Case FrameStringFormat
										Case ID3StringFmt.ISO8859
											FrameContent = Mid(Decoded, NextFramePointer + 11, FrameSize - 4)
										Case ID3StringFmt.Unicode
											FrameContent = DecodeUTF(Mid(Decoded, NextFramePointer + 13, FrameSize - 6))
										Case Else
											FrameContent = "<Unknown encoding>"
									End Select
									Dim CommentPair() As String = FrameContent.Split(New Char() {Chr(0)}, StringSplitOptions.RemoveEmptyEntries)
									If CommentPair.Length > 0 Then
										If CommentPair.Length = 1 Then
											FrameContent = Mid(CommentPair(0), If(FrameStringFormat = ID3StringFmt.Unicode, 2, 1))
										Else
											FrameContent = CommentPair(0) & " = " & Mid(CommentPair(1), If(FrameStringFormat = ID3StringFmt.Unicode, 2, 1))
										End If
									Else
										FrameContent = ""
									End If
								Else
									Select Case FrameStringFormat
										Case ID3StringFmt.ISO8859
											FrameContent = Mid(Decoded, NextFramePointer + 8, FrameSize - 1)
										Case ID3StringFmt.Unicode
											FrameContent = DecodeUTF(Mid(Decoded, NextFramePointer + 10, FrameSize - 3))
										Case Else
											FrameContent = "<Unknown encoding>"
									End Select
								End If
								FrameContent = Razor(FrameContent)
								If Mid(FrameContent, 2, 2) = "яю" Then FrameContent = Mid(FrameContent, 4)
								host.LX("ReadMP3Tag: Innerloop: FC = " & FrameContent)
								Select Case FrameType
									Case "TP2", "TP1"
										If (il And ID3Mode.NoTpe1Marker) > 0 Then
											If FrameType = "TP2" Then Tag2.Artist = FrameContent
										ElseIf (il And ID3Mode.NoTpe2Marker) > 0 Then
											If FrameType = "TP1" Then Tag2.Artist = FrameContent
										Else
											Tag2.Artist = If(Tag2.Artist.Length = 0 OrElse Tag2.Artist = FrameContent, "", Tag2.Artist & ", ") & FrameContent
										End If
									Case "TT2"
										Tag2.Track = FrameContent & Tag2.Track
									Case "TT1"
										Tag2.Track = Tag2.Track & " (" & FrameContent & ")"
									Case "TAL"
										Tag2.Album = FrameContent
									Case "TYE"
										Tag2.Year = FrameContent
									Case "TRK"
										Tag2.Number = FrameContent
									Case "COM"
										Tag2.Comments = FrameContent
									Case "TCO"
										Tag2.Genre = FrameContent
								End Select
							End If
							NextFramePointer += (FrameSize + 6)
						End While
					Else
						NextFramePointer = If((Id3Hdr(5) And &H40) > 0, TagWriter.GetIntFromSyncSafeInt(BytesTAG, 0), 0)
						Dim FrameFormatFlag As Byte
						host.LogLevel += 1
						While NextFramePointer < Id3Sz - 1 AndAlso BytesTAG(NextFramePointer) <> 0
							host.LX("ReadMP3Tag: Innerloop: NFP = " & NextFramePointer)
							FrameType = Mid(Decoded, NextFramePointer + 1, 4)
							host.LX("ReadMP3Tag: Innerloop: FT = " & FrameType)
							FrameSize = CInt(BytesTAG(NextFramePointer + 4) * 256 ^ 3)
							FrameSize += CInt(BytesTAG(NextFramePointer + 5) * 256 ^ 2)
							FrameSize += BytesTAG(NextFramePointer + 6) * 256
							FrameSize += BytesTAG(NextFramePointer + 7)
							host.LX("ReadMP3Tag: Innerloop: FS = " & FrameSize)
							If FrameSize = 0 Or NextFramePointer + FrameSize + 9 >= Id3Sz Then Exit While
							If FrameSize > 1 And FrameType <> "APIC" Then
								'FrameStatusFlag = BytesTAG(NextFramePointer + 8) - нас не интересует
								FrameFormatFlag = BytesTAG(NextFramePointer + 9)
								FrameStringFormat = CType(BytesTAG(NextFramePointer + 10), ID3StringFmt)
								host.LX("ReadMP3Tag: Innerloop: FFF = " & FrameFormatFlag & ", FSF = " & FrameStringFormat)
								If (FrameFormatFlag And &HC) > 0 Then
									FrameContent = "<Compressed/Encrypted>"
								Else
									If FrameType = "COMM" Then
										Select Case FrameStringFormat
											Case ID3StringFmt.ISO8859
												FrameContent = Mid(Decoded, NextFramePointer + 15, FrameSize - 4)
											Case ID3StringFmt.Unicode
												FrameContent = DecodeUTF(Mid(Decoded, NextFramePointer + 17, FrameSize - 6))
											Case ID3StringFmt.UTF16BE
												FrameContent = DecodeUTFBE(Mid(Decoded, NextFramePointer + 15, FrameSize - 4))
											Case ID3StringFmt.UTF8
												FrameContent = DecodeUTF8(Mid(Decoded, NextFramePointer + 15, FrameSize - 4))
											Case Else
												FrameContent = "<Unknown encoding>"
										End Select
										Dim CommentPair() As String = FrameContent.Split(New Char() {Chr(0)}, StringSplitOptions.RemoveEmptyEntries)
										If CommentPair.Length > 0 Then
											If CommentPair.Length = 1 Then
												FrameContent = Mid(CommentPair(0), If(FrameStringFormat = ID3StringFmt.Unicode, 2, 1))
											Else
												FrameContent = CommentPair(0) & " = " & Mid(CommentPair(1), If(FrameStringFormat = ID3StringFmt.Unicode, 2, 1))
											End If
										Else
											FrameContent = ""
										End If
									Else
										Select Case FrameStringFormat
											Case ID3StringFmt.ISO8859
												FrameContent = Mid(Decoded, NextFramePointer + 12, FrameSize - 1)
											Case ID3StringFmt.Unicode
												FrameContent = DecodeUTF(Mid(Decoded, NextFramePointer + 14, FrameSize - 3))
											Case ID3StringFmt.UTF16BE
												FrameContent = DecodeUTFBE(Mid(Decoded, NextFramePointer + 12, FrameSize - 1))
											Case ID3StringFmt.UTF8
												FrameContent = DecodeUTF8(Mid(Decoded, NextFramePointer + 12, FrameSize - 1))
											Case Else
												FrameContent = "<Unknown encoding>"
										End Select
									End If
									FrameContent = Razor(FrameContent)
									If Mid(FrameContent, 2, 2) = "яю" Then FrameContent = Mid(FrameContent, 4)
								End If
								host.LX("ReadMP3Tag: Innerloop: FC = " & FrameContent)
								Select Case FrameType
									Case "TPE2", "TPE1"
										If (il And ID3Mode.NoTpe1Marker) > 0 Then
											If FrameType = "TPE2" Then Tag2.Artist = FrameContent
										ElseIf (il And ID3Mode.NoTpe2Marker) > 0 Then
											If FrameType = "TPE1" Then Tag2.Artist = FrameContent
										Else
											Tag2.Artist = If(Tag2.Artist.Length = 0 OrElse Tag2.Artist = FrameContent, "", Tag2.Artist & ", ") & FrameContent
										End If
									Case "TIT2"
										Tag2.Track = FrameContent & Tag2.Track
									Case "TIT1"
										Tag2.Track = Tag2.Track & " (" & FrameContent & ")"
									Case "TALB"
										Tag2.Album = FrameContent
									Case "TRCK"
										Tag2.Number = FrameContent
									Case "COMM"
										Tag2.Comments = FrameContent
									Case "TCON"
										Tag2.Genre = FrameContent
									Case "TYER"
										Tag2.Year = FrameContent
									Case "TDRC"
										Try
											Tag2.Year = CDate(FrameContent).Year.ToString
										Catch ex As Exception
											host.LX("ReadMP3Tag: Innerloop: дата в TDRC не парсится!")
										End Try
								End Select
							End If
							NextFramePointer += (FrameSize + 10)
						End While
					End If
					Tag2.Valid = True
					host.LogLevel -= 1
					host.LX("ReadMP3Tag: level2 MP3tag = «" & Tag2.ToString & "»")
					host.LogLevel -= 1
				Else
					host.LX("ReadMP3Tag: unsupported ID3 version: " & Id3Hdr(3))
				End If
			Else
				host.LX("ReadMP3Tag: Skipping level2 tag reading due to " & If(Id3Sz = 0, "zero size", "missing header"))
			End If
		Else
			host.LX("ReadMP3Tag: work over level2 tag skipped due to flag not set")
		End If
		If Tag1.Valid And Tag2.Valid Then
			ReadMP3Tag.Valid = True
			ReadMP3Tag.Album = If(Tag1.Album.Length > Tag2.Album.Length, Tag1.Album, Tag2.Album)
			ReadMP3Tag.Artist = If(Tag1.Artist.Length > Tag2.Artist.Length, Tag1.Artist, Tag2.Artist)
			ReadMP3Tag.Comments = If(Tag1.Comments.Length > Tag2.Comments.Length, Tag1.Comments, Tag2.Comments)
			ReadMP3Tag.Genre = If(Tag1.Genre.Length > Tag2.Genre.Length, Tag1.Genre, Tag2.Genre)
			ReadMP3Tag.Number = If(Tag1.Number.Length > Tag2.Number.Length, Tag1.Number, Tag2.Number)
			ReadMP3Tag.Track = If(Tag1.Track.Length > Tag2.Track.Length, Tag1.Track, Tag2.Track)
			ReadMP3Tag.Year = If(Tag1.Year.Length > Tag2.Year.Length, Tag1.Year, Tag2.Year)
		ElseIf Tag1.Valid Then
			Tag1.CopyTo(ReadMP3Tag)
		ElseIf Tag2.Valid Then
			Tag2.CopyTo(ReadMP3Tag)
		Else
			ReadMP3Tag.Valid = False
		End If
	End Function
	Private Enum ID3StringFmt As Byte
		ISO8859 = 0
		Unicode = 1
		UTF16BE = 2
		UTF8 = 3
	End Enum
	Public ReadOnly sGenreMatrix() As String = ("Blues|Classic Rock|Country|Dance|Disco|Funk|Grunge|" & _
	  "Hip-Hop|Jazz|Metal|New Age|Oldies|Other|Pop|R&B|Rap|Reggae|Rock|Techno|" & _
	  "Industrial|Alternative|Ska|Death Metal|Pranks|Soundtrack|Euro-Techno|" & _
	  "Ambient|Trip Hop|Vocal|Jazz&Funk|Fusion|Trance|Classical|Instrumental|Acid|" & _
	  "House|Game|Sound Clip|Gospel|Noise|Alt. Rock|Bass|Soul|Punk|Space|Meditative|" & _
	  "Instrumental Pop|Instrumental Rock|Ethnic|Gothic|Darkwave|Techno-Industrial|Electronic|" & _
	  "Pop-Folk|Eurodance|Dream|Southern Rock|Comedy|Cult|Gangsta Rap|Top 40|Christian Rap|" & _
	  "Pop/Punk|Jungle|Native American|Cabaret|New Wave|Phychedelic|Rave|Showtunes|Trailer|" & _
	  "Lo-Fi|Tribal|Acid Punk|Acid Jazz|Polka|Retro|Musical|Rock & Roll|Hard Rock|Folk|" & _
	  "Folk/Rock|National Folk|Swing|Fast-Fusion|Bebob|Latin|Revival|Celtic|Blue Grass|" & _
	  "Avantegarde|Gothic Rock|Progressive Rock|Psychedelic Rock|Symphonic Rock|Slow Rock|" & _
	  "Big Band|Chorus|Easy Listening|Acoustic|Humour|Speech|Chanson|Opera|Chamber Music|" & _
	  "Sonata|Symphony|Booty Bass|Primus|Porn Groove|Satire|Slow Jam|Club|Tango|Samba|Folklore|" & _
	  "Ballad|Power Ballad|Rhythmic Soul|Freestyle|Duet|Punk Rock|Drum Solo|A Capella|Euro-House|" & _
	  "Dance Hall|Goa|Drum & Bass|Club-House|Hardcore|Terror|Indie|Brit Pop|Negerpunk|Polsk Punk|" & _
	  "Beat|Christian Gangsta Rap|Heavy Metal|Black Metal|Crossover|Comteporary Christian|" & _
	  "Christian Rock|Merengue|Salsa|Trash Metal|Anime|JPop|Synth Pop").Split(New Char() {"|"c})
#End Region
#Region "WMA"
	Private hdrGUID As Guid = New Guid("75B22630-668E-11CF-A6D9-00AA0062CE6C")
	Private contentGUID As Guid = New Guid("75B22633-668E-11CF-A6D9-00AA0062CE6C")
	Private extendedContentGUID As Guid = New Guid("D2D0A440-E307-11D2-97F0-00A0C95EA850")
	Private Function ReadWMATag() As AudioTag
		'Почитал спецификацию формата ASF и понял: какая хорошая была задумка... :(
		'На самом деле самое сложное в ней - понять, в каком объекте что засунто %)
		'Кому интересно - пункты 3.10 и 3.11 (ну кроме основ в начале :) )
		host.LX("ReadWMATag: Начало")
		ReadWMATag = New AudioTag("")
		br.ReadInt64() 'Нас не интересуют размер блока заголовка 
		Dim ObjCnt As Integer = br.ReadInt32 'А вот количество хедер-объектов запомним
		host.LX("ReadWMATag: Читаем 2е РП Хидера...")
		br.ReadByte() 'Первое резервное поле пропускаем
		If br.ReadByte = 2 Then	' А со вторым поступаем в соответствии со спецификацией - оно должно быть = 0х2
			Dim g As Guid
			Dim j As Integer = 0
			Dim ObjSz As Long
			host.LX("ReadWMATag: Успешно. Запускаю парсер объектов (всего = " & ObjCnt & ")...") : host.LogLevel += 1
			While j < ObjCnt
				g = readGUID()
				ObjSz = br.ReadInt64
				host.LX("ReadWMATag: Объект № " & j & ", размер " & ObjSz & ", GUID " & g.ToString)
				If g = contentGUID Then
					host.LX("ReadWMATag: Объект опознан как контент-объект. Читаю размеры полей...")
					Dim lTitle, lAuthor, lComment, lCopyright, lRating As Short
					lTitle = br.ReadInt16
					lAuthor = br.ReadInt16
					lCopyright = br.ReadInt16
					lComment = br.ReadInt16
					lRating = br.ReadInt16
					host.LX("ReadWMATag: Считываю поля...")
					If lTitle > 0 Then ReadWMATag.Track = Razor(DecodeUTF(br.ReadBytes(lTitle)))
					If lAuthor > 0 Then ReadWMATag.Artist = Razor(DecodeUTF(br.ReadBytes(lAuthor)))
					If lCopyright > 0 Then br.ReadBytes(lCopyright) ' Нас не волнует копирайт
					If lComment > 0 Then ReadWMATag.Comments = Razor(DecodeUTF(br.ReadBytes(lComment)))
					If lRating > 0 Then br.ReadBytes(lRating) ' Нас не волнует рейтинг
					host.LX("ReadWMATag: Контент-объект - парсинг завершён")
				ElseIf g = extendedContentGUID Then
					host.LX("ReadWMATag: Объект опознан как расширенный контент-объект") : host.LogLevel += 1
					Dim DataLen As Int16
					Dim AttribsCount As Integer = br.ReadInt16
					Dim Descriptor(AttribsCount - 1) As ExContDescrObjNrw
					host.LX("ReadWMATag: Создан массив дескрипторов: " & AttribsCount) : host.LogLevel += 1
					For k As Integer = 0 To AttribsCount - 1
						Descriptor(k).ObjectName = Razor(DecodeUTF(br.ReadBytes(br.ReadInt16)))
						Descriptor(k).ObjectDataType = CType(br.ReadInt16, ECDON_DT)
						DataLen = br.ReadInt16
						Select Case Descriptor(k).ObjectDataType
							Case ECDON_DT.Unicode
								Descriptor(k).ObjectData = Razor(DecodeUTF(br.ReadBytes(DataLen)))
							Case ECDON_DT.Ansi
								'Тип данных, выбираемый почему-то WMP для поля WM/MCDI, хотя его контент пишется в Юникоде
								'Но нас это поле не будет волновать до версии 2.5, поэтому пока все хаки только в проекте :)
								Descriptor(k).ObjectData = Razor(New String(br.ReadChars(DataLen)))
							Case ECDON_DT.Bool, ECDON_DT.DWord
								Descriptor(k).ObjectData = br.ReadInt32
							Case ECDON_DT.QWord
								Descriptor(k).ObjectData = br.ReadInt64
							Case ECDON_DT.Word
								Descriptor(k).ObjectData = br.ReadInt16
						End Select
						host.LX("ReadWMATag: Дескриптор: " & k + 1 & ",имя: " & Descriptor(k).ObjectName & ", тип: " & Descriptor(k).ObjectDataType.ToString & ", данные: " & Descriptor(k).ObjectData.ToString)
					Next
					host.LogLevel -= 1 : host.LX("ReadWMATag: Парсинг дескрипторов...")
					Dim s As String
					ReadWMATag.Album = GetObjData(Descriptor, "WM/AlbumTitle")
					ReadWMATag.Genre = GetObjData(Descriptor, "WM/Genre")
					ReadWMATag.Year = GetObjData(Descriptor, "WM/Year")
					s = GetObjData(Descriptor, "WM/TrackNumber")
					If s = "" Then
						s = GetObjData(Descriptor, "WM/Track")	'Это не дублирующее поле, тут выставляется индекс трека, а не номер
						If s <> "" Then
							If IsNumeric(s) Then
								ReadWMATag.Number = CStr(CInt(s) + 1)
							Else 'В формате <номер>/<всего>
								Dim i As Integer = s.IndexOf("/")
								ReadWMATag.Number = CStr(CInt(Mid(s, 0, i)) + 1) & Mid(s, i + 1)
							End If
						End If
					Else
						ReadWMATag.Number = s
					End If
					host.LogLevel -= 1 : host.LX("ReadWMATag: Расширенный контент-объект - парсинг завершён")
				Else
					host.LX("ReadWMATag: Объект не опознан")
					ObjSz -= 24	'Из всего размера блока вычитаем уже прочитанный ГУИД (16 байт) и размер блока (8 байт)...
					br.BaseStream.Seek(ObjSz, IO.SeekOrigin.Current) '...и сдвигаем курсор на остаток до след. объекта
				End If
				j += 1
			End While
			host.LogLevel -= 1 : host.LX("ReadWMATag: Тег сформирован: " & ReadWMATag.ToString)
			ReadWMATag.Valid = True
		Else
			host.LX("Второе резервное поле главного заголовка не сответствует стандарту. Прасинг прекращён.")
		End If
	End Function
	Private Function readGUID() As Guid
		Return New Guid(br.ReadInt32, br.ReadInt16, br.ReadInt16, br.ReadBytes(8))
	End Function
	Private Enum ECDON_DT
		Unicode = 0
		Ansi = 1
		Bool = 2
		DWord = 3
		QWord = 4
		Word = 5
	End Enum
	Private Structure ExContDescrObjNrw
		Dim ObjectName As String
		Dim ObjectDataType As ECDON_DT
		Dim ObjectData As Object
	End Structure
	Private Function GetObjData(ByVal Collection() As ExContDescrObjNrw, ByVal ObjName As String) As String
		host.LX("->GetObjectData: " & ObjName)
		GetObjData = ""
		For i As Integer = 0 To Collection.Length - 1
			If Collection(i).ObjectName = ObjName Then Return Collection(i).ObjectData.ToString
		Next
	End Function
#End Region
#Region "OGG, FLAC"
	Private Function ReadOGGTag() As AudioTag
		ReadOGGTag = New AudioTag("")
		Dim found As Boolean = False
		Dim i As Byte = br.ReadByte
		'Спецификацию я читал внимательно, но как определить начало битстрима иначе, чем
		'простым поиском подходящей строки, - осталось для меня тайной :)
		host.LX("ReadOGGTag: ищем...")
		While Not found
			'Пытаемся найти нужную строку перебором байтов в файле, начиная с байта со значением 3. Как только 
			'попадается байт, выбивающийся из общего шаблона (0x03,'vorbis'), мы возвращаемся к первому уровню
			'поиска до нахождения следующего байта со значением 3. При этом выбитый из шаблона байт не пропус-
			'кается, а анализируется на общих основаниях на первом уровне, т.к. он тоже может быть искомым.
			'Практика показывает, что искомая последовательность ВСЕГДА лежит в пределах 1 кб, поэтому поиск
			'не будет излишне долгим. Для ускорения процесса мы игнорируем требования спецификации о проверке
			'идентификационного заголовка на валидность.
			If i = 3 Then 'маркер заголовка-комментария
				i = br.ReadByte
				If i = &H76 Then 'v
					i = br.ReadByte
					If i = &H6F Then 'o
						i = br.ReadByte
						If i = &H72 Then 'r
							i = br.ReadByte
							If i = &H62 Then 'b
								i = br.ReadByte
								If i = &H69 Then 'i
									i = br.ReadByte
									If i = &H73 Then 's
										found = True '0x03,'vorbis' - Пакет комментария найден.
										host.LX("ReadOGGTag: Header found, position " & rs.Position) : host.LogLevel += 1
										ReadOGGTag = ParseVorbisComment()
										host.LogLevel -= 1 : If br.ReadByte = 0 Then host.LX("ReadOGGTag: до кучи - хедер-то кривой (framing bit not set)!")
									End If
								End If
							End If
						End If
					End If
				End If
			Else
				i = br.ReadByte
			End If
		End While
		host.LX("ReadOGGTag: Тег сформирован: " & ReadOGGTag.ToString)
	End Function
	Private Function ReadFLACTag() As AudioTag
		ReadFLACTag = New AudioTag("")
		Dim found As Boolean = False
		Dim MetadataBlockLastMarker As Boolean = False
		Dim MetadataBlockLength As Integer
		Dim i As Byte
		host.LX("ReadFLACTag: ищем...")
		While Not (found Or MetadataBlockLastMarker)
			i = br.ReadByte
			MetadataBlockLastMarker = (i And 128) > 0
			MetadataBlockLength = CInt(br.ReadByte * 256 ^ 2 + br.ReadByte * 256 + br.ReadByte)
			host.LX("ReadFLACTag: Метеданные, длина " & MetadataBlockLength & If(MetadataBlockLastMarker, ", последний", ", непоследний"))
			If (i And 127) = 4 Then	'METADATA_BLOCK_VORBIS_COMMENT
				host.LX("ReadFLACTag: тег найден, офсет " & br.BaseStream.Position) : host.LogLevel += 1
				ReadFLACTag = ParseVorbisComment()
				host.LogLevel -= 1 : found = True
			Else
				If (i And 127) = 127 Then host.LX("ReadFLACTag: до кучи, а хедер-то кривой (тип мета-блока установлен в 127)")
				host.LX("ReadFLACTag: пропускаем метаданные, тип мета-блока: " & (i And 127))
				br.ReadBytes(MetadataBlockLength)
			End If
		End While
		host.LX("ReadFLACTag: Тег сформирован: " & ReadFLACTag.ToString)
	End Function
	Private Function ParseVorbisComment() As AudioTag
		ParseVorbisComment = New AudioTag("")
		Dim VendorLength As UInteger = br.ReadUInt32
		br.ReadBytes(CInt(VendorLength)) 'Нас не интересует вендор кодера
		Dim CommentsCount As UInteger = br.ReadUInt32
		Dim CommentLength As UInteger
		Dim Comment As OGGVarPair
		For j As Integer = 1 To CInt(CommentsCount)
			CommentLength = br.ReadUInt32
			Comment = DecodeUTF8(br.ReadBytes(CInt(CommentLength)))
			host.LX("ParseVorbisComment: parsing comment, Name = " & Comment.VarName & ", Value = " & Comment.VarValue)
			Select Case Comment.VarName
				Case "ARTIST"
					ParseVorbisComment.Artist = Comment.VarValue
				Case "ALBUM"
					ParseVorbisComment.Album = Comment.VarValue
				Case "DATE"
					If Comment.VarValue.Length = 4 Then
						ParseVorbisComment.Year = Comment.VarValue
					Else
						'Допустим, что возможны случаи полной записи даты,
						'спецификации Vorbis это не противоречит...
						'ей по ходу вообще мало что может противоречить :(
						Try
							Dim d As Date = CDate(Comment.VarValue)
							ParseVorbisComment.Year = d.Year.ToString
						Catch ex As Exception
							host.LX("ParseVorbisComment: дата не парсится")
						End Try
					End If
				Case "GENRE"
					ParseVorbisComment.Genre = Comment.VarValue
				Case "COMMENT"
					ParseVorbisComment.Comments = Comment.VarValue
				Case "TRACKNUMBER"
					ParseVorbisComment.Number = Comment.VarValue
				Case "TITLE"
					ParseVorbisComment.Track = Comment.VarValue
			End Select
		Next
		ParseVorbisComment.Valid = True
	End Function
	Private Structure OGGVarPair
		Dim VarName As String
		Dim VarValue As String
		Sub New(ByVal Strings() As String)
			If Strings.Length < 2 Then
				If Strings.Length = 1 Then VarName = Strings(0) Else VarName = ""
				VarValue = ""
			Else
				VarName = Strings(0)
				VarValue = Strings(1)
			End If
		End Sub
	End Structure
#End Region
End Module