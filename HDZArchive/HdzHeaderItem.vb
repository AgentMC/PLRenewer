Imports System.IO

Public Class HdzHeaderItem
	Private _itemName As String
	Private _itemNameLength As UShort

	Sub New(ByVal Name As String)
		ItemName = Name
	End Sub
	Sub New(ByRef fs As FileStream)
		Dim itemNameLength_i = CUShort(fs.ReadByte * 256 + fs.ReadByte)
		If itemNameLength_i > 0 Then
			Dim encName(itemNameLength_i - 1) As Byte
			fs.Read(encName, 0, itemNameLength_i)
			ItemName = System.Text.Encoding.UTF8.GetString(encName)
			If _itemNameLength <> itemNameLength_i Then Throw New Exception("INL not synced!")
			_ItemAttributes = CByte(fs.ReadByte())
			_ItemLength = CUInt(fs.ReadByte * 256 ^ 3 + fs.ReadByte * 256 ^ 2 + fs.ReadByte * 256 + fs.ReadByte)
			_ItemLocation = CUInt(fs.ReadByte * 256 ^ 3 + fs.ReadByte * 256 ^ 2 + fs.ReadByte * 256 + fs.ReadByte)
		End If
	End Sub

	Property ItemName As String
		Get
			Return _itemName
		End Get
		Set(ByVal value As String)
			_itemName = value
			_itemNameLength = CUShort(System.Text.Encoding.UTF8.GetByteCount(_itemName))
		End Set
	End Property
	Property ItemAttributes As Byte
	Property ItemLength As UInteger
	Property ItemLocation As UInteger

	Function ItemNameLength() As UShort
		Return _itemNameLength
	End Function
	Function ItemTotalLength() As UInteger
		Return _itemNameLength + CUInt(11)
	End Function
	Function AttributesString() As String
		Dim chars(7) As Char
		If (_ItemAttributes And &H80) > 0 Then chars(0) = "R"c Else chars(0) = "_"c 'ReadOnly
		If (_ItemAttributes And &H40) > 0 Then chars(1) = "H"c Else chars(1) = "_"c 'Hidden
		If (_ItemAttributes And &H20) > 0 Then chars(2) = "S"c Else chars(2) = "_"c 'System
		If (_ItemAttributes And &H10) > 0 Then chars(3) = "A"c Else chars(3) = "_"c 'Archived
		If (_ItemAttributes And &H8) > 0 Then chars(4) = "L"c Else chars(4) = "_"c ' Link
		If (_ItemAttributes And &H4) > 0 Then chars(5) = "E"c Else chars(5) = "_"c ' Reserved
		If (_ItemAttributes And &H2) > 0 Then chars(6) = "E"c Else chars(6) = "_"c ' Reserved
		If (_ItemAttributes And &H1) > 0 Then chars(7) = "E"c Else chars(7) = "_"c ' Reserved
		Return New String(chars)
	End Function
	Function ToByteArray() As Byte()
		If _itemNameLength = 0 Then
			Return New Byte() {Byte.MinValue, Byte.MinValue}
		Else
			Dim hhiBytes(_itemNameLength + 10) As Byte
			hhiBytes(0) = CByte((_itemNameLength >> 8) And &HFF)
			hhiBytes(1) = CByte(_itemNameLength And &HFF)
			Array.Copy(System.Text.Encoding.UTF8.GetBytes(_itemName), 0, hhiBytes, 2, _itemNameLength)
			hhiBytes(_itemNameLength + 2) = _ItemAttributes
			hhiBytes(_itemNameLength + 3) = CByte((_ItemLength >> 24) And &HFF)
			hhiBytes(_itemNameLength + 4) = CByte((_ItemLength >> 16) And &HFF)
			hhiBytes(_itemNameLength + 5) = CByte((_ItemLength >> 8) And &HFF)
			hhiBytes(_itemNameLength + 6) = CByte(_ItemLength And &HFF)
			hhiBytes(_itemNameLength + 7) = CByte((_ItemLocation >> 24) And &HFF)
			hhiBytes(_itemNameLength + 8) = CByte((_ItemLocation >> 16) And &HFF)
			hhiBytes(_itemNameLength + 9) = CByte((_ItemLocation >> 8) And &HFF)
			hhiBytes(_itemNameLength + 10) = CByte(_ItemLocation And &HFF)
			Return hhiBytes
		End If
	End Function
End Class
