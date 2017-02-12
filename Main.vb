Module Main
	Public IsAdmin As Boolean

	'-----------Mainform

	Public Form1 As MainForm
	Friend Form1Handle As IntPtr
	Sub Main()
		IsAdmin = New System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator)
		System.Windows.Forms.Application.EnableVisualStyles()
		System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(False)
		Form1 = New MainForm()
		Form1Handle = Form1.Handle
		System.Windows.Forms.Application.Run(Form1)
	End Sub

	'-----------PLPleaseWait

	Public PLPleaseWait As PLPW
	Friend F1Checkbox17 As Boolean
	Sub PLPleaseWait_Init(ByVal Text2Paint As String, ByVal Minimum As Long, ByVal Maximum As Long, ByVal Value As Long)
		PLPleaseWait_Init(Text2Paint, Minimum, Maximum, Value, True)
	End Sub
	Sub PLPleaseWait_Init(ByVal Text2Paint As String)
		PLPleaseWait_Init(Text2Paint, 0, 0, 0, False)
	End Sub
	Private Sub PLPleaseWait_Init(ByVal Text2Paint As String, ByVal Minimum As Long, ByVal Maximum As Long, ByVal Value As Long, ByVal OptionsValid As Boolean)
		If Not isPL() Then
			PLPleaseWait = New PLPW()
			PLPleaseWait.NewInit = True
		Else
			PLPleaseWait.NewInit = False
		End If
		If OptionsValid Then
			PLPleaseWait.Init(Text2Paint, Minimum, Maximum, Value)
		Else
			PLPleaseWait.Init(Text2Paint)
		End If
	End Sub
	Sub PLPleaseWait_Halt()
		If isPL() Then
			PLPleaseWait.Halt()
			PLPleaseWait = Nothing
		End If
		Form1.ReFreshPlayListAuto()
		Form1.Focus()
	End Sub
	Sub PLPleaseWait_Resize(ByVal iLeft As Integer, ByVal iTop As Integer, ByVal iWidth As Integer)
		If isPL() Then
			PLPleaseWait.pos.Left = iLeft
			PLPleaseWait.pos.Top = iTop
			PLPleaseWait.pos.Width = iWidth
			PLPleaseWait.pos.Empty = False
		End If
	End Sub
	Sub PLPLeaseWait_ChangeText(ByVal Text2Paint As String, Optional ByVal UpdateImmediately As Boolean = True)
		If isPL() Then
			PLPleaseWait.ChangeText(Text2Paint, UpdateImmediately)
		End If
	End Sub
	Sub PLPleaseWait_ValuePlusPlus(ByVal Value1 As Boolean)
		If isPL() Then
			If Value1 Then
				PLPleaseWait.Value1 += 1
			Else
				PLPleaseWait.Value2 += 1
			End If
		End If
	End Sub
	Function PLPleaseWait_GetHeight() As Integer
		If isPL() Then
			Return PLPleaseWait.Height
		Else
			Return 0
		End If
	End Function
	Function isPL() As Boolean
		Return Not (PLPleaseWait Is Nothing OrElse PLPleaseWait.IsDisposed)
	End Function
End Module
