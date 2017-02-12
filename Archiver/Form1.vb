Imports System.IO

Public Class Form1
	Private _hdz As HdzArchive
	Private _isModified As Boolean

	Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		_hdz = New HdzArchive()
		If Command() <> "" Then
			Dim cmd As String = Command.Trim(New Char() {""""c}).Trim()
			If cmd.EndsWith(".hdz") Then
				LoadHdz(cmd)
			End If
		End If
	End Sub
	Private Sub Form1_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
		e.Cancel = Not (IsModValidator())
	End Sub
	Private Sub Form1_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragEnter
		If e.Data.GetDataPresent(DataFormats.FileDrop) Then
			e.Effect = DragDropEffects.Link
		End If
	End Sub
	Private Sub Form1_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragDrop
		Dim objects As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
		For Each o In objects
			If IO.File.Exists(o) Then
				AddItemInternal(o)
			ElseIf IO.Directory.Exists(o) Then
				Dim subfiles As String() = IO.Directory.GetFiles(o, "*.*", SearchOption.AllDirectories)
				For Each File In subfiles
					AddItemInternal(File)
				Next
			End If
		Next
	End Sub



	Private Sub tbtnHdzOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzOpen.Click
		If IsModValidator() Then
			OpenFileDialog1.FileName = ""
			OpenFileDialog1.Filter = "HDZ archives|*.hdz|Any file|*.*"
			OpenFileDialog1.Multiselect = False
			OpenFileDialog1.Title = "Open HDZ archive"
			If OpenFileDialog1.ShowDialog = DialogResult.OK Then
				tbtnHdzNew_Click(sender, e)
				LoadHdz(OpenFileDialog1.FileName)
			End If
		End If
	End Sub
	Private Sub tbtnHdzNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzNew.Click
		If IsModValidator() Then
			_hdz = New HdzArchive()
			_list.Items.Clear()
			tlName.Text = ""
		End If
	End Sub

	Private Sub tbtnHdzAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzAdd.Click
		OpenFileDialog1.FileName = ""
		OpenFileDialog1.Filter = "All files|*.*"
		OpenFileDialog1.Multiselect = True
		OpenFileDialog1.Title = "Add files to HDZ archive"
		If OpenFileDialog1.ShowDialog = DialogResult.OK Then
			For Each file In OpenFileDialog1.FileNames
				AddItemInternal(file)
			Next
		End If
	End Sub
	Private Sub tbtnHdzClone_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzClone.Click
		Dim ArcName, NewArcName, FileName As String
		For Each i As Integer In _list.SelectedIndices
			ArcName = _list.Items(i).Text
			NewArcName = ValidateArchiveItemName(ArcName & "_clone")
			FileName = _hdz.PathItems(ArcName)
			If String.IsNullOrEmpty(FileName) Then
				FileName = HdzArchive.GetTempStoragePathForItem(ArcName)
			End If
			_hdz.AddItem(FileName, New HdzHeaderItem(NewArcName))
			_list.Items.Add(New ListViewItem(New String() {NewArcName, FileName}))
			_isModified = True
		Next
	End Sub
	Private Sub tbtnHdzRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzRemove.Click
		For i As Integer = _list.SelectedIndices.Count - 1 To 0 Step -1
			Dim j As Integer = _list.SelectedIndices(i)
			_hdz.RemoveItem(_list.Items(j).Text)
			_list.Items.RemoveAt(j)
			_isModified = True
		Next
	End Sub
	Private Sub tbtnHdzRename_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzRename.Click
		If _list.SelectedIndices.Count > 0 Then
			If _list.SelectedIndices.Count > 1 Then
				MsgBox("Not Impl for many")
			Else
				Dim ArcName As String = _list.SelectedItems(0).Text
				Dim NewName As String = InputBox("Please enter a new archive name for a file «" & ArcName & "»", "Renaming", ArcName)
				If (Not String.IsNullOrEmpty(NewName) AndAlso NewName <> ArcName) Then
					_list.SelectedItems(0).Text = NewName
					_hdz.RenameItem(ArcName, NewName)
					_isModified = True
				End If
			End If
		Else
			MsgBox("Select something")
		End If
	End Sub

	Private Sub tbtnHdzSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzSave.Click
		SaveFunc()
	End Sub
	Private Sub tbtnHdzSaveAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzSaveAs.Click
		SaveAsFunc()
	End Sub
	Private Sub tbtnHdzExtract_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzExtract.Click
		Dim list As New List(Of String)
		If _list.SelectedItems.Count > 0 Then
			For Each item As ListViewItem In _list.SelectedItems
				list.Add(item.Text)
			Next
		Else
			For Each item As ListViewItem In _list.Items
				list.Add(item.Text)
			Next
		End If
		If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
			_hdz.ExtractItemsFromHdz(list, FolderBrowserDialog1.SelectedPath)
		End If
	End Sub
	Private Sub tbtnHdzExtractAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbtnHdzExtractAs.Click
		If _list.SelectedIndices.Count = 1 Then
			SaveFileDialog1.FileName = _list.SelectedItems(0).Text
			SaveFileDialog1.Filter = "All files|*.*"
			SaveFileDialog1.Title = "Extract file as..."
			If SaveFileDialog1.ShowDialog = DialogResult.OK Then
				_hdz.PathItems(_list.SelectedItems(0).Text) = SaveFileDialog1.FileName
				_hdz.ExtractItemsFromHdz(New List(Of String)(New String() {_list.SelectedItems(0).Text}))
			End If
		Else
			MsgBox("Multiple saving not impl")
		End If
	End Sub



	Private Function ValidateArchiveItemName(ByVal suggestion As String) As String
		Dim ArcItemName2 As String = suggestion
		Dim i As Integer = -1
		While _hdz.HdzItems.ContainsKey(ArcItemName2)
			i += 1
			ArcItemName2 = suggestion + String.Format("0000", i)
		End While
		Return ArcItemName2
	End Function
	Private Function SaveFunc() As Boolean
		If String.IsNullOrEmpty(_hdz.FileName) Then
			Return SaveAsFunc()
		Else
			_hdz.Save(HdzArchive.Versions.V1)
			_isModified = False
			Return True
		End If
	End Function
	Private Function SaveAsFunc() As Boolean
		SaveFileDialog1.FileName = Path.GetFileName(_hdz.FileName)
		SaveFileDialog1.Filter = "HDZ Archive|*.hdz|All files|*.*"
		SaveFileDialog1.Title = "Save HDZ as..."
		If SaveFileDialog1.ShowDialog = DialogResult.OK Then
			_hdz.Save(HdzArchive.Versions.V1, SaveFileDialog1.FileName)
			_isModified = False
			tlName.Text = SaveFileDialog1.FileName
			Return True
		End If
		Return False
	End Function
	Private Function IsModValidator() As Boolean
		If _isModified Then
			Dim dr As DialogResult = MessageBox.Show("HDZ was modified, would you like to save your changes?", "HDZ modification", MessageBoxButtons.YesNoCancel)
			If dr = Windows.Forms.DialogResult.Cancel Then Return False
			If dr = Windows.Forms.DialogResult.No Then Return True
			Return SaveFunc()
		Else
			Return True
		End If
	End Function

	Private Sub AddItemInternal(ByVal file As String)
		Dim validArcName As String = ValidateArchiveItemName(Path.GetFileName(file))
		_hdz.AddItem(file, New HdzHeaderItem(validArcName))
		_list.Items.Add(New ListViewItem(New String() {validArcName, file}))
		_isModified = True
	End Sub
	Private Sub LoadHdz(ByVal hdz As String)
		_hdz = New HdzArchive(hdz)
		For Each h In _hdz.HdzItems.Values
			_list.Items.Add(New ListViewItem(New String() {h.ItemName, "", h.AttributesString(), h.ItemLength.ToString(), h.ItemLocation.ToString()}))
		Next
		tlName.Text = hdz
	End Sub
End Class
