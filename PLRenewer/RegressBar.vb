Public Class RegressBar
	Inherits Control
	Dim Sticky As Boolean = False
	Dim Treshold As Integer = 120
	Public WithEvents fKepper As New TextBox With {.Cursor = Cursors.Hand}
	Dim Vall As Double = 10
	Dim Max As Double = 100
	Dim Min As Double = 0
	Dim VDta As Integer = 5
	Dim Colour As Color = Me.ForeColor
	Dim FontColour As Color = Me.ForeColor
	Dim Colour2 As Color = Me.ForeColor
	Dim FontColour2 As Color = Me.ForeColor
	Dim FontShadowColour As Color = Me.ForeColor
	Dim fmt As String = "00.0%"
	Dim atex As Boolean = True
	Dim g As Graphics
	Dim GS As String, GW As Integer
	Dim mouse_i As Boolean = False
	Public Event ValueChangedManually()
	Dim cur As Cursor
	Public Property Snapping() As Boolean
		Get
			Return Sticky
		End Get
		Set(ByVal value As Boolean)
			Sticky = value
		End Set
	End Property
	Public Property ScrollTheshold() As Integer
		Get
			Return Treshold
		End Get
		Set(ByVal value As Integer)
			Treshold = value
		End Set
	End Property
	Public Shadows Property Value() As Double
		Get
			Return Vall
		End Get
		Set(ByVal value As Double)
			Vall = Math.Max(Math.Min(value, Max), Min)
			If UpdateG() Then Refresh()
		End Set
	End Property
	Public Shadows Property Maximum() As Double
		Get
			Return Max
		End Get
		Set(ByVal value As Double)
			Max = value
			If Max < Min Then Max = Min
			If Vall > Max Then Vall = Max
			If UpdateG() Then Refresh()
		End Set
	End Property
	Public Shadows Property Minimum() As Double
		Get
			Return Min
		End Get
		Set(ByVal value As Double)
			Min = value
			If Min > Vall Then Vall = Min
			If Min > Max Then Max = Min
			If UpdateG() Then Refresh()
		End Set
	End Property
	Public Property VerticalDelta() As Integer
		Get
			Return VDta
		End Get
		Set(ByVal value As Integer)
			VDta = value
			Refresh()
		End Set
	End Property
	Public Property FontColor() As Color
		Get
			Return FontColour
		End Get
		Set(ByVal value As Color)
			FontColour = value
			Refresh()
		End Set
	End Property
	Public Property ForeColor2() As Color
		Get
			Return Colour
		End Get
		Set(ByVal value As Color)
			Colour = value
			Refresh()
		End Set
	End Property
	Public Property FontColorActive() As Color
		Get
			Return FontColour2
		End Get
		Set(ByVal value As Color)
			FontColour2 = value
			Refresh()
		End Set
	End Property
	Public Property ForeColor2Active() As Color
		Get
			Return Colour2
		End Get
		Set(ByVal value As Color)
			Colour2 = value
			Refresh()
		End Set
	End Property
	Public Property FontShadowColor() As Color
		Get
			Return FontShadowColour
		End Get
		Set(ByVal value As Color)
			FontShadowColour = value
		End Set
	End Property
	Public Property AutoText() As Boolean
		Get
			Return atex
		End Get
		Set(ByVal value As Boolean)
			atex = value
			If UpdateG() Then Refresh()
		End Set
	End Property
	Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
		If g Is Nothing Then g = Me.CreateGraphics
		Dim bg As BufferedGraphics = BufferedGraphicsManager.Current.Allocate(g, New Rectangle(0, 0, Me.Width, Me.Height))
		bg.Graphics.Clear(If(W7.GlassMode, Color.Transparent, Me.BackColor))
		If Me.Enabled Then
			If VDta > 1 And Not W7.GlassMode Then
				Dim rx As New Rectangle(0, 0, GW, VDta)
				Dim rx2 As New Rectangle(0, -1, GW, VDta + 1)
				Dim bx As New Drawing2D.LinearGradientBrush(rx2, BackColor, ForeColor, Drawing2D.LinearGradientMode.Vertical)
				bg.Graphics.FillRectangle(bx, rx)
			End If
			Dim r As New Rectangle(0, VDta, GW, Me.Height - VDta)
			Dim r2 As New Rectangle(0, VDta - 1, GW, Me.Height - VDta + 1)
			Dim b As New Drawing2D.LinearGradientBrush(r2, If(W7.GlassMode, Color.Transparent, ForeColor), If(mouse_i, Colour2, Colour), Drawing2D.LinearGradientMode.Vertical)
			bg.Graphics.FillRectangle(b, r)
			If W7.GlassMode Then
				bg.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
				bg.Graphics.SmoothingMode = Drawing2D.SmoothingMode.None
			End If
			bg.Graphics.DrawString(GS, Me.Font, New Drawing.SolidBrush(Me.FontShadowColour), 1, 1)
			bg.Graphics.DrawString(GS, Me.Font, New Drawing.SolidBrush(If(mouse_i, FontColour2, FontColour)), 0, 0)
		End If
		bg.Render()
	End Sub
	Private Sub MovefKeeper(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize, Me.Move
		fKepper.Left = Me.Left - 2
		fKepper.Top = Me.Top - 2
		fKepper.Width = Me.Width + 4
		fKepper.Height = Me.Height + 7
		Refresh()
	End Sub
	Private Sub iClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseDown, Me.MouseMove, fKepper.MouseWheel
		If e.Button = Windows.Forms.MouseButtons.Left Or e.Delta <> 0 Then
			Dim i As Double
			If e.Button = Windows.Forms.MouseButtons.Left Then i = e.X / Width Else i = (Vall - Min) / (Max - Min) + 0.1 * e.Delta / Treshold
			If Sticky Then
				If i < 0.1 Then i = 0
				If i > 0.9 Then i = 1
				If i > 0.44 And i < 0.56 Then i = 0.5
			End If
			i = Math.Max(Math.Min(Min + (Max - Min) * i, Max), Min)
			If i <> Vall Then
				Value = CInt(i)
				RaiseEvent ValueChangedManually()
			End If
		Else
			fKepper.Focus()
		End If
	End Sub
	Private Sub M_Enter(ByVal sender As Object, ByVal e As EventArgs) Handles Me.MouseEnter
		mouse_i = True
		Refresh()
	End Sub
	Private Sub M_Leave(ByVal sender As Object, ByVal e As EventArgs) Handles Me.MouseLeave
		mouse_i = False
		Refresh()
	End Sub
	Private Sub RegressBar_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.TextChanged
		If UpdateG() Then Refresh()
	End Sub
	Private Function UpdateG() As Boolean
		If Max > Min And Vall >= Min Then
			Dim dw As Integer = GW
			Dim ds As String = GS
			GW = Math.Max(CInt(Me.Width * ((Vall - Min) / (Max - Min))), 1)
			GS = If(atex, Format((Vall - Min) / (Max - Min), fmt), Me.Text)
			Return dw <> GW Or ds <> GS
		Else
			Return False
		End If
	End Function
	Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
		UpdateG()
		MyBase.OnResize(e)
	End Sub
End Class