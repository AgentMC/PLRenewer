Imports System.IO
Public Class HdzFixedBordersFileStream
	Inherits Stream
	Dim _stream As FileStream
	Sub New(ByVal stream As FileStream)
		_stream = stream
		MarkOut = -1
	End Sub
	Public Property MarkOut As Long
	Public ReadOnly Property BaseStream As FileStream
		Get
			Return _stream
		End Get
	End Property
	Public Overrides Function Read(ByVal array() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
		If MarkOut = -1 Then
			Return _stream.Read(array, offset, count)
		ElseIf Position >= MarkOut Then
			Return 0
		Else
			Return _stream.Read(array, offset, Math.Min(count, CInt(MarkOut - Position)))
		End If
	End Function
	Public Function SeekToHdzContent(ByVal item As HdzHeaderItem) As Long
		MarkOut = item.ItemLocation + item.ItemLength
		Return Seek(item.ItemLocation, SeekOrigin.Begin)
	End Function
	Public Overrides ReadOnly Property CanRead As Boolean
		Get
			Return _stream.CanRead
		End Get
	End Property
	Public Overrides ReadOnly Property CanSeek As Boolean
		Get
			Return _stream.CanSeek
		End Get
	End Property
	Public Overrides ReadOnly Property CanWrite As Boolean
		Get
			Return _stream.CanWrite
		End Get
	End Property
	Public Overrides Sub Flush()
		_stream.Flush()
	End Sub
	Public Overrides ReadOnly Property Length As Long
		Get
			Return _stream.Length
		End Get
	End Property
	Public Overrides Property Position As Long
		Get
			Return _stream.Position
		End Get
		Set(ByVal value As Long)
			_stream.Position = value
		End Set
	End Property
	Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
		Return _stream.Seek(offset, origin)
	End Function
	Public Overrides Sub SetLength(ByVal value As Long)
		_stream.SetLength(value)
	End Sub
	Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
		_stream.Write(buffer, offset, count)
	End Sub
End Class
