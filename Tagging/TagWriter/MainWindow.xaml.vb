Class MainWindow 
    Implements ILoggerHost
	Private ll As Integer
    Public Property LogLevel As Integer Implements ILoggerHost.LogLevel
        Get
            Return ll
        End Get
        Set(ByVal value As Integer)
            ll = value
        End Set
    End Property
    Public Sub LS(ByVal txt As String) Implements ILoggerHost.LS, ILoggerHost.LX, ILoggerHost.Achtung
        LogBox.AppendText(vbCrLf & If(ll > 0, New String(vbTab, ll), "") & txt)
    End Sub
	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOpen.Click
		Dim ofd As New Microsoft.Win32.OpenFileDialog()
		If (ofd.ShowDialog()) Then
			LblFile.Text = ofd.FileName
			Dim tag As AudioTag = TagReader.GenTag(ofd.FileName, ID3Mode.Both)
			tbAlbum.Text = tag.Album
			tbArtist.Text = tag.Artist
			tbComments.Text = tag.Comments
			tbGenre.Text = tag.Genre
			tbTrackName.Text = tag.Track
			tbTrackNumber.Text = tag.Number
			tbYear.Text = tag.Year
			cbValid.IsChecked = tag.Valid
		End If
	End Sub
	Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
		TagReader.host = Me
        TagWriter.host = Me
	End Sub
	Private Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnSave.Click
		Dim aTag As New AudioTag("")
		aTag.Album = tbAlbum.Text
		aTag.Artist = tbArtist.Text
		aTag.Comments = tbComments.Text
		aTag.Genre = tbGenre.Text
		aTag.Number = tbTrackNumber.Text
		aTag.Track = tbTrackName.Text
		aTag.Year = tbYear.Text
		aTag.Valid = True
		aTag.Empty = False
        TagWriter.WriteMp3Tag(aTag, LblFile.Text, System.Text.Encoding.Unicode)
	End Sub
End Class
