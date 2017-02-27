Public Class TheLine
	Inherits Control
	Dim FW As Integer = 1
	Public Property ForeWidth() As Integer
		Get
			Return FW
		End Get
		Set(ByVal value As Integer)
			FW = value
		End Set
	End Property
	Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
		e.Graphics.DrawRectangle(New Pen(Me.ForeColor, FW), New Rectangle(0, 0, Me.Width, Me.Height))
		'		MyBase.OnPaint(e)
	End Sub
End Class
