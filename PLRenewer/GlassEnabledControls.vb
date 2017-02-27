Public Interface iGlassEnabledControl
	Property GlassMode() As Boolean
End Interface
Public Class GlassEnabledTextBox
	Inherits TextBox
	Implements iGlassEnabledControl

	Private _realForeColor As Color = SystemColors.WindowText
	Private WithEvents refreshTimer As Timer
	Private _cursorVisible As Boolean
	Private _timerEnabled As Boolean
	Private _glass As Boolean

	Public Sub New()
		_timerEnabled = False
	End Sub

	Public Property GlassMode() As Boolean Implements iGlassEnabledControl.GlassMode
		Get
			Return _glass
		End Get
		Set(ByVal value As Boolean)
			_glass = value
			SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint, _glass)
			If _glass Then
				If refreshTimer Is Nothing Then refreshTimer = New Timer() With {.Enabled = True, .Interval = GetCaretBlinkTime()} Else refreshTimer.Start()
			Else
				If refreshTimer IsNot Nothing Then refreshTimer.Stop()
			End If
		End Set
	End Property

	Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
		If _glass Then
			Dim r As New Rectangle(New Point(0, 0), Size)
			e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
			e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
			Dim sf As New StringFormat
			sf.LineAlignment = StringAlignment.Center
			sf.FormatFlags = StringFormatFlags.NoWrap
			e.Graphics.DrawString(Text, Font, New SolidBrush(_realForeColor), r, sf)
			If _cursorVisible Then
				Dim Cursor1, Cursor2 As Single
				Cursor1 = e.Graphics.MeasureString(Text.Substring(0, SelectionStart), Font, r.Width, sf).Width - 2
				If SelectionLength = 0 Then
					Cursor2 = Cursor1
					e.Graphics.DrawRectangle(New Pen(_realForeColor), New Rectangle(Math.Max(MainForm.CIntR(Cursor1), 0), 0, 1, r.Height))
				Else
					Cursor2 = e.Graphics.MeasureString(Text.Substring(0, SelectionStart + SelectionLength), Font, r.Width, sf).Width - 2
					e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(128, SystemColors.Highlight)), New Rectangle(MainForm.CIntR(Cursor1), 0, MainForm.CIntR(Cursor2 - Cursor1), r.Height))
				End If
			End If
			HideCaret(Handle)
		Else
			MyBase.OnPaint(e)
		End If
	End Sub
	Protected Overrides Sub OnPaintBackground(ByVal pevent As System.Windows.Forms.PaintEventArgs)
		If _glass Then
			pevent.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
			pevent.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
			pevent.Graphics.FillRectangle(New SolidBrush(BackColor), New Rectangle(0, 0, Width, Height))
		Else
			MyBase.OnPaintBackground(pevent)
		End If
	End Sub
	Protected Overrides Sub OnTextChanged(ByVal e As System.EventArgs)
		MyBase.OnTextChanged(e)
		If _glass Then
			_cursorVisible = True
			refreshTimer.Stop()
			refreshTimer.Start()
			Refresh()
		End If
	End Sub
	Protected Overrides Sub OnKeyDown(ByVal e As System.Windows.Forms.KeyEventArgs)
		MyBase.OnKeyDown(e)
		If _glass Then
			_cursorVisible = True
			If SelectionLength > 0 Then
				_timerEnabled = False
			Else
				_timerEnabled = True
			End If
			Refresh()
		End If
	End Sub
	Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
		If _glass Then
			If e.Button = Windows.Forms.MouseButtons.Left Then
				_cursorVisible = True
				Refresh()
			End If
			If SelectionLength > 0 Then
				_timerEnabled = False
			Else
				If Focused Then _timerEnabled = True
			End If
		Else
			MyBase.OnMouseMove(e)
		End If
	End Sub
	Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
		If _glass Then
			If e.Button = Windows.Forms.MouseButtons.Left Then
				_cursorVisible = True
				_timerEnabled = True
				refreshTimer.Stop()
				refreshTimer.Start()
				Refresh()
			End If
		Else
			MyBase.OnMouseDown(e)
		End If
	End Sub
	Protected Overrides Sub OnLostFocus(ByVal e As System.EventArgs)
		MyBase.OnLostFocus(e)
		If _glass Then
			_timerEnabled = False
			_cursorVisible = False
			Refresh()
		End If
	End Sub
	Protected Overrides Sub OnGotFocus(ByVal e As System.EventArgs)
		MyBase.OnGotFocus(e)
		If _glass Then
			_cursorVisible = True
			If SelectionLength = 0 Then _timerEnabled = True
			Refresh()
		End If
	End Sub
	Public Overrides Property ForeColor() As System.Drawing.Color
		Get
			If _glass Then Return _realForeColor Else Return MyBase.ForeColor
		End Get
		Set(ByVal value As System.Drawing.Color)
			If _glass Then _realForeColor = value Else MyBase.ForeColor = value
		End Set
	End Property
	Private Sub Ticker(ByVal sender As Object, ByVal e As System.EventArgs) Handles refreshTimer.Tick
		If _timerEnabled Then _cursorVisible = _cursorVisible Xor True
		Refresh()
	End Sub
End Class
Public Class GlassEnabledNumericUpDown
	Inherits NumericUpDown
	Implements iGlassEnabledControl

	Protected WithEvents _topLevelTextBox As GlassEnabledTextBox
	Private _lastText As String = "0"
	Private _initDone As Boolean = False

	Public Sub New()
		MyBase.New()
		_topLevelTextBox = New GlassEnabledTextBox()
		Controls.Add(_topLevelTextBox)
		_topLevelTextBox.Visible = True
		_topLevelTextBox.Left = 0
		_topLevelTextBox.Top = 0
		_topLevelTextBox.BringToFront()
		_topLevelTextBox.Text = Text
		AddHandler _topLevelTextBox.TextChanged, AddressOf topTextBoxTextChanged
		_initDone = True
	End Sub

	Public Property GlassMode() As Boolean Implements iGlassEnabledControl.GlassMode
		Get
			If _initDone Then Return _topLevelTextBox.GlassMode Else Return False
		End Get
		Set(ByVal value As Boolean)
			If _initDone Then _topLevelTextBox.GlassMode = value
		End Set
	End Property

	Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
		MyBase.OnResize(e)
		If _initDone Then _topLevelTextBox.Width = Width - Height + 3
	End Sub
	Public Overrides Property ForeColor() As System.Drawing.Color
		Get
			Return MyBase.ForeColor
		End Get
		Set(ByVal value As System.Drawing.Color)
			MyBase.ForeColor = value
			If _initDone Then _topLevelTextBox.ForeColor = value
		End Set
	End Property
	Public Overrides Property BackColor() As System.Drawing.Color
		Get
			Return MyBase.BackColor
		End Get
		Set(ByVal value As System.Drawing.Color)
			MyBase.BackColor = value
			If _initDone Then _topLevelTextBox.BackColor = value
		End Set
	End Property
	Public Overrides Property Font() As System.Drawing.Font
		Get
			Return MyBase.Font
		End Get
		Set(ByVal value As System.Drawing.Font)
			MyBase.Font = value
			If _initDone Then _topLevelTextBox.Font = value
		End Set
	End Property

	Protected Overrides Sub OnTextBoxTextChanged(ByVal source As Object, ByVal e As System.EventArgs)
		If _initDone AndAlso _topLevelTextBox.Text <> Text Then _topLevelTextBox.Text = Text
		MyBase.OnTextBoxTextChanged(source, e)
	End Sub

	Private Sub topTextBoxTextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
		Dim d As New Decimal
		Try
			d = CDec(_topLevelTextBox.Text)
			_lastText = _topLevelTextBox.Text
		Catch ex As Exception
			_topLevelTextBox.Text = _lastText
			Exit Sub
		End Try
		If d <> Value Then Text = _topLevelTextBox.Text
	End Sub


End Class