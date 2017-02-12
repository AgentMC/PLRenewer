Imports System.Runtime.InteropServices
Module W7
	Friend GlassMode As Boolean
#Region "Taskbar"
	Function tb() As Boolean
		Return taskbar IsNot Nothing AndAlso Form1.CheckBox31.Checked
	End Function
	Friend taskbar As ITaskbarList3
#End Region
#Region "DWM"
	Friend Sub MakeGlass(ByVal hWnd As IntPtr, ByVal mrgs As MARGINS)
		Dim bbhOff As New DWM_BLURBEHIND
		bbhOff.dwFlags = DWM_BLURBEHIND.DWM_BB_ENABLE Or DWM_BLURBEHIND.DWM_BB_BLURREGION
		bbhOff.fEnable = False
		bbhOff.hRegionBlur = IntPtr.Zero
		DwmEnableBlurBehindWindow(hWnd, bbhOff)
		DwmExtendFrameIntoClientArea(hWnd, mrgs)
	End Sub
	Friend Sub MakeGlass(ByVal hWnd As IntPtr)
		If Form1 IsNot Nothing Then MakeGlass(hWnd, If(Form1.TabControl1.SelectedIndex = 4, New MARGINS(-1, 0, 0, 0), New MARGINS(5, Form1.ClientSize.Height - Form1.TabPage1Main.Height, 5, 5)))
	End Sub
#End Region
#Region "Common"
	'Can glass mode be enabled at all?
	Function w7glass() As Boolean
		If Form1 Is Nothing Then Return False
		Form1.LX("W7G: галочка в " & Form1.CheckBox30.Checked.ToString())
		Return w7() AndAlso Form1.CheckBox30.Checked AndAlso DwmIsCompositionEnabled()
	End Function
	'Is OS Win7 or higher
	Function w7() As Boolean
		Return Environment.OSVersion.Version.Major > 6 Or (Environment.OSVersion.Version.Major = 6 And Environment.OSVersion.Version.Minor > 0)
	End Function
#End Region
End Module