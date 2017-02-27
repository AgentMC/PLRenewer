Public Class HdzArchiveEnumerator
	Implements IEnumerator, IEnumerator(Of KeyValuePair(Of String, HdzHeaderItem))

	Private _index As Integer
	Private _hdzArchive As HdzArchive

	Public Sub New(ByVal Arc As HdzArchive)
		_index = -1
		_hdzArchive = Arc
	End Sub

	Public ReadOnly Property Current As KeyValuePair(Of String, HdzHeaderItem) Implements IEnumerator(Of KeyValuePair(Of String, HdzHeaderItem)).Current
		Get
			Return New KeyValuePair(Of String, HdzHeaderItem)(_hdzArchive.GetRealFileNameByIndex(_index), _hdzArchive.GetHdzItemByIngex(_index))
		End Get
	End Property

	Public ReadOnly Property CurrentObject As Object Implements IEnumerator.Current
		Get
			Return Current
		End Get
	End Property

	Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
		_index += 1
		If _index < _hdzArchive.Count Then Return True
		Return False
	End Function

	Public Sub Reset() Implements IEnumerator.Reset
		_index = -1
	End Sub

#Region "IDisposable Support"
	Private disposedValue As Boolean ' To detect redundant calls

	' IDisposable
	Protected Overridable Sub Dispose(ByVal disposing As Boolean)
		If Not Me.disposedValue Then
			If disposing Then
				_hdzArchive = Nothing
				' TODO: dispose managed state (managed objects).
			End If

			' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
			' TODO: set large fields to null.
		End If
		Me.disposedValue = True
	End Sub

	' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
	'Protected Overrides Sub Finalize()
	'    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
	'    Dispose(False)
	'    MyBase.Finalize()
	'End Sub

	' This code added by Visual Basic to correctly implement the disposable pattern.
	Public Sub Dispose() Implements IDisposable.Dispose
		' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
		Dispose(True)
		GC.SuppressFinalize(Me)
	End Sub
#End Region

End Class
