Public Class HdzStreamingEventArgs
	Inherits EventArgs
	Private _compressedTotalBytes, _compressedDoneBytes, _decompressedTotalBytes, _decompressedDoneBytes As Long
	Private _fileName As String
	Public Sub New(ByVal CompressedTotal As Long, ByVal CompressedDone As Long, ByVal DecompressedTotal As Long, ByVal DecompressedDone As Long, ByVal Filename As String)
		_compressedDoneBytes = CompressedDone
		_compressedTotalBytes = CompressedTotal
		_decompressedDoneBytes = DecompressedDone
		_decompressedTotalBytes = DecompressedTotal
		_fileName = Filename
	End Sub
	Public ReadOnly Property CompressedDoneBytes As Long
		Get
			Return _compressedDoneBytes
		End Get
	End Property
	Public ReadOnly Property CompressedTotalBytes As Long
		Get
			Return _compressedTotalBytes
		End Get
	End Property
	Public ReadOnly Property DecompressedDoneBytes As Long
		Get
			Return _decompressedDoneBytes
		End Get
	End Property
	Public ReadOnly Property DecompressedTotalBytes As Long
		Get
			Return _decompressedTotalBytes
		End Get
	End Property
	Public ReadOnly Property FileName As String
		Get
			Return _fileName
		End Get
	End Property
End Class
