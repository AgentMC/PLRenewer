Public Module TagWriter
#Region "Общее"
    Public host As ILoggerHost
    Structure UnparsedTag
        Dim TID As String
        Dim BinaryRepresentation() As Byte
    End Structure
#End Region
#Region "MP3"
    Public Sub WriteMp3Tag(ByVal tag As AudioTag, ByVal Filename As String, Optional ByVal enc As System.Text.Encoding = Nothing)
        If host IsNot Nothing Then
            If enc Is Nothing Then enc = System.Text.Encoding.UTF8
            host.LS("WriteMp3Tag: begin @ " & Filename) : host.LogLevel += 1
            host.LX("Passed TAG is " & tag.ToString())
            '---------------------------MPEG stream
            Dim Fbegin, Fend As Integer
            Dim buffer(9) As Byte
            host.LS("Opening file for reading")
            Dim fs As New IO.FileStream(Filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
            Dim FullWrite As Boolean
            '------------------------------Prepareing Level2 and collecting unparseable for TAG structure frames
            host.LX("Prepareing L2 tag...") : host.LogLevel += 1
            fs.Read(buffer, 0, 10)
            If buffer(0) = Asc("I") And buffer(1) = Asc("D") And buffer(2) = Asc("3") Then
                Fbegin = CInt(If((buffer(5) And &H10) > 0, 20, 10) + GetIntFromSyncSafeInt(buffer, 6))
            Else
                Fbegin = 0
            End If
            host.LX("Fbegin=" & Fbegin)
            Dim unparseableFrames As New List(Of Byte)
            Dim subscriptions As New List(Of String)(New String() {"TPE2", "TPE1", "TIT2", "TALB", "TRCK", "TCON", "TYER", "COMM", "TENC", "ENDTAG"})
            Dim tbs() As String = New String() {tag.Artist, tag.Artist, tag.Track, tag.Album, tag.Number, tag.Genre, tag.Year, tag.Comments, "TagWriter module"}
            host.LS("Enumerating frames to find unparseables...") : host.LogLevel += 1
            If buffer(3) = 3 Or buffer(3) = 4 Then
                If Fbegin > 0 Then
                    If (buffer(5) And &H40) > 0 Then fs.Seek(2 + GetIntFromSyncSafeInt(New Byte() {CByte(fs.ReadByte), CByte(fs.ReadByte), CByte(fs.ReadByte), CByte(fs.ReadByte)}, 0), IO.SeekOrigin.Current)
                    Dim ut As UnparsedTag
                    While fs.Position < Fbegin
                        ut = GetNextFrame(fs, Fbegin)
                        host.LX("Found tag " & ut.TID)
                        If Not (subscriptions.Contains(ut.TID)) Then unparseableFrames.AddRange(ut.BinaryRepresentation)
                    End While
                Else
                    host.LX("Skipped due to Level 2 tag not present")
                End If
            ElseIf Fbegin = 0 Then
                host.LS("Level 2 tag notfound, skipping parsing of unparseables...")
            Else
                host.Achtung("Неподдерживаемая версия тега (" & buffer(3).ToString & ") в файле (" & Filename & "). Существующие элементы тега были пропущены при записи.")
            End If
            host.LogLevel -= 1 : host.LX("Finished enumerating frames, seeking to tag level1")
            '------------------------------Prepareing Level1
            fs.Seek(-128, IO.SeekOrigin.End)
            fs.Read(buffer, 0, 3)
            If buffer(0) = Asc("T") And buffer(1) = Asc("A") And buffer(2) = Asc("G") Then
                Fend = CInt(fs.Length - 129)
            Else
                Fend = CInt(fs.Length - 1)
            End If
            host.LX("Fend=" & Fend)
            Dim fileContents(Fend - Fbegin) As Byte
            fs.Seek(Fbegin, IO.SeekOrigin.Begin)
            '[%TAG2%][fileContens][%TAG1%]
            '-------------------------- TAG level 2
            host.LS("Forming Header and Frames...") : host.LogLevel += 1
            Dim tagFrames As New List(Of Byte)
            Dim encdgbuffer() As Byte
            For i As Integer = 0 To tbs.Length - 1
                If tbs(i) <> "" Then
                    host.LX("Generating binary tag with TagID = " & subscriptions(i))
                    encdgbuffer = enc.GetBytes(If(subscriptions(i) = "COMM", "ENG" & Chr(0), "") & tbs(i))
                    tagFrames.AddRange(System.Text.Encoding.Default.GetBytes(subscriptions(i)))
                    tagFrames.AddRange(GetSyncSafeInt(encdgbuffer.Length + 1)) ' +1 because we begin content with encoding bytecode (1 byte length)
                    tagFrames.AddRange(New Byte() {0, 0, GetEncodingByteCode(enc)})
                    tagFrames.AddRange(encdgbuffer)
                End If
            Next
            If buffer.Length + tagFrames.Count + unparseableFrames.Count <= Fbegin Then
                FullWrite = False
            Else
                FullWrite = True
                fs.Read(fileContents, 0, fileContents.Length)
            End If
            host.LS("Fullwrite is set to " & FullWrite)
            buffer(0) = Asc("I") : buffer(1) = Asc("D") : buffer(2) = Asc("3")
            buffer(3) = 4 ' version
            buffer(4) = 0
            buffer(5) = 0
            System.Array.Copy(GetSyncSafeInt(If(FullWrite, tagFrames.Count + unparseableFrames.Count, Fbegin - 10)), 0, buffer, 6, 4)
            host.LogLevel -= 1 : host.LX("Finished forming Header and Frames")
            '-------------------------- Selecting write method based on padding size
            host.LogLevel -= 1 : host.LX("Finished L2 tag, closing stream...")
            fs.Close()
            fs.Dispose()
            '[buffer][tagFrames.ToArray][unparseableFrames.ToArray][fileContens][%TAG1%]
            '-------------------------- Tag level 1
            host.LS("Begin L1 tag ")
            Dim tag1(127) As Byte
            tag1(0) = Asc("T") : tag1(1) = Asc("A") : tag1(2) = Asc("G")
            If tag.Track <> "" Then
                encdgbuffer = System.Text.Encoding.Default.GetBytes(tag.Track)
                System.Array.Copy(encdgbuffer, 0, tag1, 3, Math.Min(encdgbuffer.Length, 30))
            End If
            If tag.Artist <> "" Then
                encdgbuffer = System.Text.Encoding.Default.GetBytes(tag.Artist)
                System.Array.Copy(encdgbuffer, 0, tag1, 33, Math.Min(encdgbuffer.Length, 30))
            End If
            If tag.Album <> "" Then
                encdgbuffer = System.Text.Encoding.Default.GetBytes(tag.Album)
                System.Array.Copy(encdgbuffer, 0, tag1, 63, Math.Min(encdgbuffer.Length, 30))
            End If
            If tag.Year <> "" Then
                encdgbuffer = System.Text.Encoding.Default.GetBytes(tag.Year)
                System.Array.Copy(encdgbuffer, 0, tag1, 93, Math.Min(encdgbuffer.Length, 4))
            End If
            If tag.Comments <> "" Then
                encdgbuffer = System.Text.Encoding.Default.GetBytes(tag.Comments)
                System.Array.Copy(encdgbuffer, 0, tag1, 97, Math.Min(encdgbuffer.Length, If(tag.Number <> "", 28, 30)))
            End If
            If tag.Number <> "" Then '1.1
                tag1(125) = 0
                tag1(126) = CByte(tag.Number)
            End If
            Dim ix As Integer = -1
            If tag.Genre <> "" Then ix = New List(Of String)(TagReader.sGenreMatrix).IndexOf(tag.Genre)
            tag1(127) = CByte(If(ix > -1, ix, 255))
            '[buffer][tagFrames.ToArray][unparseableFrames.ToArray][fileContens][tag1]
            '-------------------------- Writing
            host.LS("Finished L1 tag, writing...")
            Try
                Dim fs2 As New IO.FileStream(Filename, If(FullWrite, System.IO.FileMode.Create, System.IO.FileMode.Open), IO.FileAccess.Write, IO.FileShare.ReadWrite)
                host.LogLevel += 1
                host.LX("<<buffer   , position " & fs2.Position & ", length " & buffer.Length)
                fs2.Write(buffer, 0, buffer.Length)
                host.LX("<<tagFrames, position " & fs2.Position & ", length " & tagFrames.Count)
                fs2.Write(tagFrames.ToArray, 0, tagFrames.Count)
                host.LX("<<unpFrames, position " & fs2.Position & ", length " & unparseableFrames.Count)
                fs2.Write(unparseableFrames.ToArray, 0, unparseableFrames.Count)
                If FullWrite Then
                    host.LX("<<FilContnt, position " & fs2.Position & ", length " & fileContents.Length)
                    fs2.Write(fileContents, 0, fileContents.Length)
                Else
                    Dim zeros(Fbegin - (buffer.Length + tagFrames.Count + unparseableFrames.Count + 1)) As Byte
                    host.LX("<<zeros    , position " & fs2.Position & ", length " & zeros.Length)
                    fs2.Write(zeros, 0, zeros.Length) ' filling padding
                    fs2.Seek(Fend + 1, IO.SeekOrigin.Begin)
                End If
                host.LX("<<tag1      , position " & fs2.Position & ", length " & tag1.Length)
                fs2.Write(tag1, 0, tag1.Length)
                host.LogLevel -= 1 : host.LS("Flush-Close-Dispose")
                fs2.Flush()
                fs2.Close()
                fs2.Dispose()
            Catch ex As Exception
                host.Achtung("Невозможно записать тег в " & Filename & ": " & ex.Message)
            End Try
            host.LogLevel -= 1 : host.LS("WriteMp3Tag: end")
        Else
            Throw New ArgumentException("Logging host not set! Please provide module with an iLoggerHost implementation (TagWriter.host=<your logging host>) before calling WriteTag().", "host")
        End If
    End Sub
    Public Function GetSyncSafeInt(ByVal i As Integer) As Byte()
        Dim ssi(3) As Byte
        For j = 3 To 0 Step -1
            ssi(j) = CByte(i And 127)
            i = i >> 7
        Next
        Return ssi
    End Function
    Public Function GetIntFromSyncSafeInt(ByVal arr() As Byte, ByVal minIdx As Integer) As Integer
        Return CInt((arr(minIdx) And 127) * 128 ^ 3 + (arr(minIdx + 1) And 127) * 128 ^ 2 + (arr(minIdx + 2) And 127) * 128 + (arr(minIdx + 3) And 127))
    End Function
    Private Function GetNextFrame(ByRef fs As IO.FileStream, ByVal TagSize As Integer) As UnparsedTag
        Dim t As New UnparsedTag
        t.TID = Chr(fs.ReadByte) + Chr(fs.ReadByte) + Chr(fs.ReadByte) + Chr(fs.ReadByte)
        Dim sz As Integer
        Dim szArr(3) As Byte
        fs.Read(szArr, 0, 4)
        sz = GetIntFromSyncSafeInt(szArr, 0)
        If sz = 0 Or sz + 2 + fs.Position > TagSize Then
            t.TID = "ENDTAG"
            fs.Seek(TagSize, IO.SeekOrigin.Begin)
        Else
            fs.Seek(-8, IO.SeekOrigin.Current)
            Dim bytes(sz + 9) As Byte
            fs.Read(bytes, 0, sz + 10)
            t.BinaryRepresentation = bytes
        End If
        Return t
    End Function
    Private Function GetEncodingByteCode(ByVal enc As System.Text.Encoding) As Byte
        If Object.Equals(enc, System.Text.Encoding.Default) Then
            Return CByte(0)
        ElseIf Object.Equals(enc, System.Text.Encoding.Unicode) Then
            Return CByte(1)
        ElseIf Object.Equals(enc, System.Text.Encoding.BigEndianUnicode) Then
            Return CByte(2)
        ElseIf Object.Equals(enc, System.Text.Encoding.UTF8) Then
            Return CByte(3)
        Else
            Throw New ArgumentException("Unsupported ID3 encoding! Use ANSI instead of ASCII, or Unicode(BE), or UTF-8", "enc")
        End If
    End Function
#End Region
End Module