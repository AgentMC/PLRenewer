Public Class ListedKeyDictionary(Of TKey, TValue)
	Inherits Dictionary(Of TKey, TValue)
	Implements IList(Of KeyValuePair(Of TKey, TValue))
	ReadOnly Property Key(ByVal index As Integer) As TKey
		Get
			If index < Count And index > -1 Then
				SyncLock Values
					Dim iterator As Dictionary(Of TKey, TValue).KeyCollection.Enumerator = Keys.GetEnumerator()
					While index >= 0
						index -= 1
						iterator.MoveNext()
					End While
					Return iterator.Current
				End SyncLock
			Else
				Throw New IndexOutOfRangeException()
			End If
		End Get
	End Property
	ReadOnly Property Value(ByVal index As Integer) As TValue
		Get
			If index < Count And index > -1 Then
				SyncLock Values
					Dim iterator As Dictionary(Of TKey, TValue).ValueCollection.Enumerator = Values.GetEnumerator()
					While index >= 0
						index -= 1
						iterator.MoveNext()
					End While
					Return iterator.Current
				End SyncLock
			Else
				Throw New IndexOutOfRangeException()
			End If
		End Get
	End Property
	Property ItemAt(ByVal index As Integer) As KeyValuePair(Of TKey, TValue) _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).Item
		Get
			Dim k As TKey = Key(index)
			Return New KeyValuePair(Of TKey, TValue)(k, Item(k))
		End Get
		Set(ByVal value As KeyValuePair(Of TKey, TValue))
			PerformReOrder(ListedKeyDictionary(Of TKey, TValue).ChaosType.Replace, index, value)
		End Set
	End Property
	Sub Insert(ByVal index As Integer, ByVal insertedItem As KeyValuePair(Of TKey, TValue)) _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).Insert
		PerformReOrder(ListedKeyDictionary(Of TKey, TValue).ChaosType.Insert, index, insertedItem)
	End Sub
	Sub RemoveAt(ByVal index As Integer) _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).RemoveAt
		PerformReOrder(ListedKeyDictionary(Of TKey, TValue).ChaosType.Remove, index, Nothing)
	End Sub
	Function IndexOf(ByVal KeyValueOrBoth As KeyValuePair(Of TKey, TValue)) As Integer _
	 Implements IList(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue)).IndexOf
		Dim j As Integer = 0
		If KeyValueOrBoth.Key IsNot Nothing Then
			For Each k As TKey In Keys
				If k.Equals(KeyValueOrBoth.Key) Then Return j Else j += 1
			Next
			Return -1
		Else
			For Each v As TValue In Values
				If v.Equals(KeyValueOrBoth.Value) Then Return j Else j += 1
			Next
			Return -1
		End If
	End Function
	Private Enum ChaosType
		Insert
		Remove
		Replace
	End Enum
	Private Sub PerformReOrder(ByVal type As ChaosType, ByVal index As Integer, ByVal kvp As KeyValuePair(Of TKey, TValue))
		Dim k As TKey = Key(index)
		Dim NewCol As New ListedKeyDictionary(Of TKey, TValue)
		Select Case type
			Case ChaosType.Insert
				For Each k2 As TKey In Keys
					If k.Equals(k2) Then NewCol.Add(kvp.Key, kvp.Value)
					NewCol.Add(k2, Item(k2))
				Next
			Case ChaosType.Remove
				For Each k2 As TKey In Keys
					If Not k.Equals(k2) Then NewCol.Add(k2, Item(k2))
				Next
			Case ChaosType.Replace
				For Each k2 As TKey In Keys
					If k.Equals(k2) Then
						NewCol.Add(kvp.Key, kvp.Value)
					Else
						NewCol.Add(k2, Item(k2))
					End If
				Next
			Case Else
				Throw New NotImplementedException("Unknown type of chaos!")
		End Select
		Clear()
		For Each K2 As TKey In NewCol.Keys
			Add(K2, NewCol(K2))
		Next
	End Sub
End Class
