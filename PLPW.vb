Public Class PLPW
	Structure Position
		Dim Top As Integer
		Dim Left As Integer
		Dim Width As Integer
		Dim Empty As Boolean
	End Structure
	Dim PT As String
	Dim SP As Boolean
	Dim PB2_Val, PB_Max, PB_Min, PB_Val, PB2_Max As Long
	Dim angle As Integer = 0
	Dim g As Graphics
	Friend NewInit As Boolean
	Friend pos As Position
	Private Sub InitInternal()
		If NewInit Then
			pos.Empty = True
			Timer1.Start()
			Me.Show(Form1)
			Form1.ListBox1.Items.Clear()
			Form1.MoveCallback()
			Form1.Refresh()
			Me.BringToFront()
			Application.DoEvents()
			Timer1_Tick(Nothing, Nothing)
			Application.DoEvents()
		End If
	End Sub
	Sub Init(ByVal Text2Paint As String)
		PT = Text2Paint
		SP = False
		InitInternal()
	End Sub
	Sub Init(ByVal Text2Paint As String, ByVal Minimum As Long, ByVal Maximum As Long, ByVal Value As Long)
		PT = Text2Paint
		SP = True
		PB_Min = Minimum
		PB_Max = Math.Max(Maximum, PB_Min + 1)
		PB_Val = Math.Min(Math.Max(Value, Minimum), Maximum)
		PB2_Val = 0
		PB2_Max = 1
		Form1.PLPBVal = PB_Val
		If W7.tb Then
			W7.taskbar.SetProgressState(Main.Form1Handle, TBPFLAG.TBPF_NORMAL)
			W7.taskbar.SetProgressValue(Main.Form1Handle, CType(Value - Minimum, ULong), CType(Maximum - Minimum, ULong))
		End If
		InitInternal()
	End Sub
	Sub Halt()
		Timer1.Stop()
		If W7.tb Then W7.taskbar.SetProgressState(Main.Form1Handle, TBPFLAG.TBPF_NOPROGRESS)
		If g IsNot Nothing Then g.Dispose()
		Me.Close()
	End Sub
	Sub ChangeText(ByVal Text2Paint As String, ByVal UpdateImmediately As Boolean)
		PT = Text2Paint
		If UpdateImmediately Then Refresh()
	End Sub
	Function GetLeftBytes() As Long
		Return PB_Max - PB_Val
	End Function
	Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
		If pos.Empty = False Then
			pos.Empty = True
			Me.SetBounds(pos.Left, pos.Top, pos.Width, Me.Height)
			Exit Sub
		End If
		angle += 5
		If angle > 360 Then angle = angle Mod 360
		If g Is Nothing Then g = Me.CreateGraphics
		Render(g)
		g.Dispose()
		g = Nothing
		Application.DoEvents() ' Если есть ещё события рисования, займёмся ими...
	End Sub
	Private Sub PLPW_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
		Render(e.Graphics)
	End Sub
	Private Sub Render(ByRef g As Graphics)
		Dim bg As BufferedGraphics = BufferedGraphicsManager.Current.Allocate(g, New Rectangle(0, 0, Me.Width - 10, Me.Height - 25))
		bg.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
		bg.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
		bg.Graphics.FillRectangle(New SolidBrush(Me.BackColor), bg.Graphics.ClipBounds)
		bg.Graphics.FillEllipse(New Drawing2D.LinearGradientBrush(New Rectangle(5, 5, 50, 50), Color.Blue, Color.Yellow, angle), 5, 5, 50, 50)
		bg.Graphics.FillEllipse(Drawing.SystemBrushes.Control, 15, 15, 30, 30)
		bg.Graphics.DrawLine(Drawing.SystemPens.Control, 30, 3, 30, 57)
		bg.Graphics.DrawLine(Drawing.SystemPens.Control, 31, 3, 31, 57)
		bg.Graphics.DrawLine(Drawing.SystemPens.Control, 3, 30, 57, 30)
		bg.Graphics.DrawLine(Drawing.SystemPens.Control, 3, 31, 57, 31)
		If W7.GlassMode Then
			bg.Graphics.DrawString(PT, TestLabel.Font, New SolidBrush(Color.FromArgb(128, Color.White)), 56, 36)
			bg.Graphics.DrawString(PT, TestLabel.Font, New SolidBrush(Color.FromArgb(128, Color.White)), 54, 34)
			bg.Graphics.DrawString(PT, TestLabel.Font, New SolidBrush(Color.FromArgb(192, Color.Black)), 55, 35)
		Else
			bg.Graphics.DrawString(PT, TestLabel.Font, Brushes.Black, 55, 35)
		End If
		If SP Then
			bg.Graphics.DrawRectangle(Pens.Black, 60, 10, 300, 20)
			bg.Graphics.FillRectangle(Drawing.SystemBrushes.Control, 64, 9, 297, 17)
			bg.Graphics.FillRectangle(Drawing.SystemBrushes.Control, 59, 14, 297, 17)
			Dim c1, c2 As Color
			If Main.F1Checkbox17 Then c1 = Color.Red Else c1 = Color.Lime
			If Main.F1Checkbox17 Then c2 = Color.Lime Else c2 = Color.Red
			bg.Graphics.FillRectangle(New Drawing2D.LinearGradientBrush(New Rectangle(61, 11, 300, 9), c1, c2, Drawing2D.LinearGradientMode.Horizontal), 61, 11, CSng(298 * ((PB_Val - PB_Min) / (PB_Max - PB_Min))), 9)
			bg.Graphics.FillRectangle(New Drawing2D.LinearGradientBrush(New Rectangle(61, 21, 300, 9), c1, c2, Drawing2D.LinearGradientMode.Horizontal), 61, 21, CSng(298 * (PB2_Val / PB2_Max)), 9)
		End If
		bg.Render()
		bg.Dispose()
	End Sub
	Private Sub PLPleaseWait_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
		Me.Region = New Region(New Rectangle(5, 20, Me.Width - 10, Me.Height - 25))
	End Sub
	Property Value1() As Long
		Get
			Return PB_Val
		End Get
		Set(ByVal value As Long)
			PB_Val = value
			If W7.tb Then W7.taskbar.SetProgressValue(Main.Form1Handle, CType(value, ULong), CType(PB_Max, ULong))
		End Set
	End Property
	Property Value2() As Long
		Get
			Return PB2_Val
		End Get
		Set(ByVal value As Long)
			PB2_Val = value
		End Set
	End Property
	Property Max2() As Long
		Get
			Return PB2_Max
		End Get
		Set(ByVal value As Long)
			PB2_Max = value
		End Set
	End Property
End Class