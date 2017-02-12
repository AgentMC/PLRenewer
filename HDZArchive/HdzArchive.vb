Imports System.IO
Imports System.IO.Compression

Public Class HdzArchive
	Implements IList(Of KeyValuePair(Of String, HdzHeaderItem))
	Private Const MEGABYTE As Integer = 1024 * 1024	' buffer size
	Private _archivedPathsRealPaths As New ListedKeyDictionary(Of String, String)
	Private _archivedPathsHdzItems As New ListedKeyDictionary(Of String, HdzHeaderItem)

	Public Sub New(ByVal ExistingHdz As String)
		_FileName = ExistingHdz
		ReLoadFromHdz()
	End Sub
	Public Sub New()
		_FileName = ""
	End Sub

	Public Enum Versions As Integer
		V1 = 1
	End Enum

	Public Event CompressionProgressChanged As EventHandler(Of HdzStreamingEventArgs)
	Public Event DecompressionProgressChanged As EventHandler(Of HdzStreamingEventArgs)

#Region "HDZ file operations"
	Private Sub CompressFile(ByVal filename As String, ByRef hdz As FileStream)
		Using fs As New FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
			Using ds As New DeflateStream(hdz, CompressionMode.Compress, True)
				Dim temp(MEGABYTE - 1) As Byte
				Dim bytes_read As Integer
				Do
					bytes_read = fs.Read(temp, 0, MEGABYTE)
					If bytes_read > 0 Then
						ds.Write(temp, 0, bytes_read)
						RaiseEvent CompressionProgressChanged(Me, New HdzStreamingEventArgs(-1, ds.BaseStream.Position, fs.Length, fs.Position, filename))
					End If
				Loop While bytes_read > 0
			End Using
		End Using
	End Sub

	Private Sub DeCompressFile(ByVal filename As String, ByRef hdz As HdzFixedBordersFileStream, ByVal deflatedContentLength As UInteger)
		Using fs As New FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None)
			Using ds As New DeflateStream(hdz, CompressionMode.Decompress, True)
				Dim temp(MEGABYTE - 1) As Byte
				Dim bytes_read As Integer, bytes_read_totally As Integer = 0
				Do
					bytes_read = ds.Read(temp, 0, MEGABYTE)
					If bytes_read > 0 Then fs.Write(temp, 0, bytes_read)
					bytes_read_totally += bytes_read
					RaiseEvent DecompressionProgressChanged(Me, New HdzStreamingEventArgs(deflatedContentLength, bytes_read_totally, -1, fs.Length, filename))
				Loop While bytes_read > 0
			End Using
		End Using
	End Sub

	Private Sub WriteHDZv1()
		'Initialization
		Dim hdzStream As New FileStream(_FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None)
		hdzStream.Write(New Byte() {72, 68, 90, 1}, 0, 4) ' HDZ1
		'Step 1: calculating size of header
		Dim j As UInteger = 6
		For Each hdzItem In _archivedPathsHdzItems.Values
			j += hdzItem.ItemTotalLength()
		Next
		'Step 2: writing deflated content
		hdzStream.Seek(j, SeekOrigin.Begin)
		For Each hdzItem In _archivedPathsHdzItems.Values
			CompressFile(_archivedPathsRealPaths(hdzItem.ItemName), hdzStream)
			hdzItem.ItemLocation = j
			j = CUInt(hdzStream.Position)
			hdzItem.ItemLength = j - hdzItem.ItemLocation
			hdzItem.ItemAttributes = 0	'TODO: attributes
		Next
		'Step 3: writing header
		hdzStream.Seek(4, SeekOrigin.Begin)
		For Each hdzItem In _archivedPathsHdzItems.Values
			hdzStream.Write(hdzItem.ToByteArray, 0, CInt(hdzItem.ItemTotalLength))
		Next
		hdzStream.WriteByte(0)
		hdzStream.WriteByte(0)
		'Finalization
		hdzStream.Flush()
		hdzStream.Close()
		hdzStream.Dispose()
	End Sub

	Private Function GetHDZItems(ByVal HdzFileName As String) As List(Of HdzHeaderItem)
		Using fs = New FileStream(HdzFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
			GetHDZItems = GetHDZItems(fs)
		End Using
	End Function
	Private Function GetHDZItems(ByVal HdzFileName As String, ByRef HdzFile As FileStream) As List(Of HdzHeaderItem)
		HdzFile = New FileStream(HdzFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
		Return GetHDZItems(HdzFile)
	End Function
	Private Function GetHDZItems(ByVal HdzFile As FileStream) As List(Of HdzHeaderItem)
		Dim lst As New List(Of HdzHeaderItem)
		Dim hdr(3) As Byte
		HdzFile.Read(hdr, 0, 4)
		If hdr(0) = 72 And hdr(1) = 68 And hdr(2) = 90 Then	'HDZ
			If hdr(3) = 1 Then ' version
				Do
					lst.Add(New HdzHeaderItem(HdzFile))
				Loop Until lst(lst.Count - 1).ItemNameLength = 0
			Else
				Throw New InvalidDataException("Not a supported version: " & hdr(3))
			End If
		Else
			Throw New InvalidDataException("Not a HDZ archive: " & HdzFile.Name)
		End If
		lst.RemoveAt(lst.Count - 1)
		Return lst
	End Function

	Public Sub ReLoadFromHdz()
		Clear()
		Dim items As List(Of HdzHeaderItem) = GetHDZItems(_FileName)
		For Each h In items
			_archivedPathsRealPaths.Add(h.ItemName, "")
			_archivedPathsHdzItems.Add(h.ItemName, h)
		Next
	End Sub

	Public Sub Save(ByVal Ver As Versions)
		If String.IsNullOrEmpty(_FileName) Then Throw New InvalidOperationException("File name not set!")
		Dim Extractables As New List(Of String)
		For Each h In _archivedPathsHdzItems.Keys
			If String.IsNullOrEmpty(_archivedPathsRealPaths(h)) Then
				Extractables.Add(h)
				_archivedPathsRealPaths(h) = GetTempStoragePathForItem(h)
			End If
		Next
		If Extractables.Count > 0 Then ExtractItemsFromHdz(Extractables)
		Select Case Ver
			Case Versions.V1
				WriteHDZv1()
			Case Else
				Throw New NotImplementedException("Currently only writing of HDZ version 1 is supported!")
		End Select
		For Each File In Extractables
			IO.File.Delete(GetTempStoragePathForItem(File))
		Next
	End Sub
	Public Sub Save(ByVal Ver As Versions, ByVal FileName As String)
		Me.FileName = FileName
		Save(Ver)
	End Sub

	Public Sub ExtractItemsFromHdz()
		ExtractItemsFromHdz(New List(Of String)(_archivedPathsHdzItems.Keys), Nothing)
	End Sub
	Public Sub ExtractItemsFromHdz(ByVal Folder As String)
		ExtractItemsFromHdz(New List(Of String)(_archivedPathsHdzItems.Keys), Folder)
	End Sub
	Public Sub ExtractItemsFromHdz(ByVal ArchivedNames As List(Of String))
		ExtractItemsFromHdz(ArchivedNames, Nothing)
	End Sub
	Public Sub ExtractItemsFromHdz(ByVal ArchivedNames As List(Of String), ByVal Folder As String)
		Using HdzSubStream As FileStream = New FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
			Dim HdzStream As New HdzFixedBordersFileStream(HdzSubStream)
			For Each h In ArchivedNames
				HdzStream.SeekToHdzContent(_archivedPathsHdzItems(h))
				DeCompressFile(If(String.IsNullOrEmpty(Folder), If(String.IsNullOrEmpty(_archivedPathsRealPaths(h)), Path.GetDirectoryName(_FileName) & "\" & h, _archivedPathsRealPaths(h)), Folder.TrimEnd("\"c) & "\" & h), HdzStream, _archivedPathsHdzItems(h).ItemLength)
			Next
		End Using
	End Sub

	Public Shared Function GetTempStoragePathForItem(ByVal itemName As String) As String
		Return Environment.GetEnvironmentVariable("temp") & "\" & itemName
	End Function
#End Region

#Region "Implementation and other public interfaces"
	Public Property FileName As String
	Default Public Property Item(ByVal index As Integer) As KeyValuePair(Of String, HdzHeaderItem) _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).Item
		Get
			Dim k As String = _archivedPathsHdzItems.Key(index)
			Return New KeyValuePair(Of String, HdzHeaderItem)(_archivedPathsRealPaths(k), _archivedPathsHdzItems(k))
		End Get
		Set(ByVal value As KeyValuePair(Of String, HdzHeaderItem))
			Dim k As String = _archivedPathsHdzItems.Key(index)
			_archivedPathsRealPaths(k) = value.Key
			_archivedPathsHdzItems(k) = value.Value
		End Set
	End Property
	Public ReadOnly Property Count As Integer _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).Count
		Get
			Return _archivedPathsHdzItems.Count
		End Get
	End Property
	Public ReadOnly Property IsReadOnly As Boolean _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).IsReadOnly
		Get
			Return False
		End Get
	End Property
	Public Property PathItems As ListedKeyDictionary(Of String, String)
		Get
			Return _archivedPathsRealPaths
		End Get
		Set(ByVal value As ListedKeyDictionary(Of String, String))
			_archivedPathsRealPaths = value
		End Set
	End Property
	Public Property HdzItems As ListedKeyDictionary(Of String, HdzHeaderItem)
		Get
			Return _archivedPathsHdzItems
		End Get
		Set(ByVal value As ListedKeyDictionary(Of String, HdzHeaderItem))
			_archivedPathsHdzItems = value
		End Set
	End Property

	Public Sub AddItem(ByVal RealPath As String, ByVal ArchiveItem As HdzHeaderItem)
		_archivedPathsHdzItems.Add(ArchiveItem.ItemName, ArchiveItem)
		_archivedPathsRealPaths.Add(ArchiveItem.ItemName, RealPath)
	End Sub
	Public Sub AddItem(ByVal PathAndItem As KeyValuePair(Of String, HdzHeaderItem)) _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).Add
		AddItem(PathAndItem.Key, PathAndItem.Value)
	End Sub
	Public Sub AddItemsRange(ByVal Range As IEnumerable(Of KeyValuePair(Of String, HdzHeaderItem)))
		For Each RangeItem In Range
			AddItem(RangeItem.Key, RangeItem.Value)
		Next
	End Sub
	Public Sub RemoveItem(ByVal ArchiveItemName As String)
		_archivedPathsHdzItems.Remove(ArchiveItemName)
		_archivedPathsRealPaths.Remove(ArchiveItemName)
	End Sub
	Public Sub RemoveItem(ByVal index As Integer) _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).RemoveAt
		_archivedPathsHdzItems.RemoveAt(index)
		_archivedPathsRealPaths.RemoveAt(index)
	End Sub
	Public Sub RemoveItemsRange(ByVal ArchiveItemNames As IEnumerable(Of String))
		For Each ArchiveItem In ArchiveItemNames
			RemoveItem(ArchiveItem)
		Next
	End Sub
	Public Sub RenameItem(ByVal ArchiveItemName As String, ByVal NewArchiveItemName As String)
		Dim idx As Integer = _archivedPathsHdzItems.IndexOf(New KeyValuePair(Of String, HdzHeaderItem)(ArchiveItemName, Nothing))
		_archivedPathsHdzItems.ItemAt(idx) = New KeyValuePair(Of String, HdzHeaderItem)(NewArchiveItemName, _archivedPathsHdzItems.Value(idx))
		_archivedPathsRealPaths.ItemAt(idx) = New KeyValuePair(Of String, String)(NewArchiveItemName, _archivedPathsRealPaths.Value(idx))
	End Sub
	Public Sub Clear() Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).Clear
		_archivedPathsHdzItems.Clear()
		_archivedPathsRealPaths.Clear()
	End Sub
	Public Sub Insert(ByVal index As Integer, ByVal insertableItem As KeyValuePair(Of String, HdzHeaderItem)) _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).Insert
		_archivedPathsHdzItems.Insert(index, New KeyValuePair(Of String, HdzHeaderItem)(insertableItem.Value.ItemName, insertableItem.Value))
		_archivedPathsRealPaths.Insert(index, New KeyValuePair(Of String, String)(insertableItem.Value.ItemName, insertableItem.Key))
	End Sub
	Public Sub CopyTo(ByVal array() As KeyValuePair(Of String, HdzHeaderItem), ByVal arrayIndex As Integer) _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).CopyTo
		Dim i1 As IEnumerator(Of KeyValuePair(Of String, String)) = _archivedPathsRealPaths.GetEnumerator
		Dim i2 As IEnumerator(Of KeyValuePair(Of String, HdzHeaderItem)) = _archivedPathsHdzItems.GetEnumerator
		While arrayIndex < array.Length AndAlso (i1.MoveNext AndAlso i2.MoveNext)
			array(arrayIndex) = New KeyValuePair(Of String, HdzHeaderItem)(i1.Current.Value, i2.Current.Value)
		End While
	End Sub

	Public Function GetRealFileNameByIndex(ByVal index As Integer) As String
		Return _archivedPathsRealPaths.Value(index)
	End Function
	Public Function GetArchivedFileNameByIndex(ByVal index As Integer) As String
		Return _archivedPathsRealPaths.Key(index)
	End Function
	Public Function GetHdzItemByIngex(ByVal index As Integer) As HdzHeaderItem
		Return _archivedPathsHdzItems.Value(index)
	End Function
	Public Function GetEnumerator() As IEnumerator _
	 Implements IList(Of KeyValuePair(Of String, HdzHeaderItem)).GetEnumerator
		Return New HdzArchiveEnumerator(Me)
	End Function
	Public Function GetKvpEnumerator() As IEnumerator(Of KeyValuePair(Of String, HdzHeaderItem)) _
	 Implements IEnumerable(Of KeyValuePair(Of String, HdzHeaderItem)).GetEnumerator
		Return New HdzArchiveEnumerator(Me)
	End Function
	Public Function Contains(ByVal KeyValueOrBoth As KeyValuePair(Of String, HdzHeaderItem)) As Boolean _
	 Implements IList(Of KeyValuePair(Of String, HdzHeaderItem)).Contains
		If (KeyValueOrBoth.Key IsNot Nothing AndAlso _archivedPathsRealPaths.ContainsValue(KeyValueOrBoth.Key)) _
		 OrElse (KeyValueOrBoth.Value IsNot Nothing AndAlso _archivedPathsHdzItems.ContainsValue(KeyValueOrBoth.Value)) Then _
		 Return True
		Return False
	End Function
	Public Function IndexOf(ByVal OnlyItemNameMeans As KeyValuePair(Of String, HdzHeaderItem)) As Integer _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).IndexOf
		Return IndexOf(OnlyItemNameMeans.Value.ItemName)
	End Function
	Public Function IndexOf(ByVal ItemName As String) As Integer
		Return _archivedPathsRealPaths.IndexOf(New KeyValuePair(Of String, String)(ItemName, Nothing))
	End Function
	Public Function RemoveItem(ByVal Item As KeyValuePair(Of String, HdzHeaderItem)) As Boolean _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of String, HdzHeaderItem)).Remove
		RemoveItem(Item.Value.ItemName)
		Return True
	End Function
#End Region

#Region "Legacy"
	Private Sub CreateHDZv1(ByVal HdzFileName As String, ByVal ArchivedPathsRealPathsCollection As ListedKeyDictionary(Of String, String))
		'Initialization
		Dim items(ArchivedPathsRealPathsCollection.Count - 1) As HdzHeaderItem
		Dim hdzStream As New FileStream(HdzFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None)
		hdzStream.Write(New Byte() {72, 68, 90, 1}, 0, 4) ' HDZ1
		'Step 1: calculating size of header
		Dim j As UInteger = 6
		For i As Integer = 0 To items.Length - 1
			items(i) = New HdzHeaderItem(ArchivedPathsRealPathsCollection.Key(i))
			j += items(i).ItemTotalLength()
		Next
		'Step 2: writing deflated content
		hdzStream.Seek(j, SeekOrigin.Begin)
		For i As Integer = 0 To items.Length - 1
			CompressFile(ArchivedPathsRealPathsCollection.Value(i), hdzStream)
			items(i).ItemLocation = j
			j = CUInt(hdzStream.Position)
			items(i).ItemLength = j - items(i).ItemLocation
			items(i).ItemAttributes = 0	'TODO: attributes
		Next
		'Step 3: writing header
		hdzStream.Seek(4, SeekOrigin.Begin)
		For i As Integer = 0 To items.Length - 1
			hdzStream.Write(items(i).ToByteArray, 0, CInt(items(i).ItemTotalLength))
		Next
		hdzStream.WriteByte(0)
		hdzStream.WriteByte(0)
		'Finalization
		hdzStream.Flush()
		hdzStream.Close()
		hdzStream.Dispose()
	End Sub
	Private Sub ExtractFilesFromHdz(ByVal HdzFileName As String, ByVal IndicesFileNamesCollection As Dictionary(Of Integer, String))
		Dim HdzSubStream As FileStream = Nothing
		Dim items As List(Of HdzHeaderItem) = GetHDZItems(HdzFileName, HdzSubStream)
		Dim HdzStream As New HdzFixedBordersFileStream(HdzSubStream)
		For Each itemIndex As Integer In IndicesFileNamesCollection.Keys
			HdzStream.SeekToHdzContent(items(itemIndex))
			DeCompressFile(IndicesFileNamesCollection(itemIndex), HdzStream, items(itemIndex).ItemLength)
		Next
		HdzSubStream.Close()
		HdzSubStream.Dispose()
	End Sub
#End Region
End Class