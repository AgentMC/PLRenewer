Imports System.Runtime.InteropServices
Module API
#Region "WinAll"
	Declare Auto Function ShellExecute Lib "shell32.dll" Alias "ShellExecuteW" (ByVal Handle As IntPtr, ByVal Operation As String, ByVal Path As String, ByVal Parameters As String, ByVal Directory As String, ByVal Command As SW_Commands) As IntPtr
	Enum SW_Commands As Integer
		SW_HIDE = 0
		SW_SHOWNORMAL = 1
		SW_NORMAL = 1
		SW_SHOWMINIMIZED = 2
		SW_SHOWMAXIMIZED = 3
		SW_MAXIMIZE = 3
		SW_SHOWNOACTIVATE = 4
		SW_SHOW = 5
		SW_MINIMIZE = 6
		SW_SHOWMINNOACTIVE = 7
		SW_SHOWNA = 8
		SW_RESTORE = 9
		SW_SHOWDEFAULT = 10
		SW_FORCEMINIMIZE = 11
		SW_MAX = 11
	End Enum
	Declare Ansi Function SetParent Lib "user32.dll" (ByVal frmForm As IntPtr, ByVal newParent As IntPtr) As IntPtr
	Declare Auto Function FindWindowEx Lib "user32.dll" (ByVal hwndParent As IntPtr, ByVal hwndChildAfter As IntPtr, ByVal lpszClass As String, ByVal lpszWindow As String) As IntPtr
	Declare Auto Function GetWindowRect Lib "user32.dll" (ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
	<StructLayout(LayoutKind.Sequential)> Structure RECT
		Public left As Integer
		Public top As Integer
		Public right As Integer
		Public bottom As Integer
		Sub New(ByVal left As Integer, ByVal top As Integer, ByVal right As Integer, ByVal bottom As Integer)
			left = left
			top = top
			right = right
			bottom = bottom
		End Sub
	End Structure
	Enum EnvSpFldr
		AppData = 0
		Cache = 1
		CD_Burning = 2
		Cookies = 4
		Desktop = 8
		Favorites = 16
		Fonts = 32
		History = 64
		Local_AppData = 128
		Local_Settings = 256
		My_Music = 512
		My_Pictures = 1024
		My_Video = 2048
		NetHood = 4096
		Personal = 8192
		PrintHood = 16384
		Programs = 32768
		Recent = 65536
		SendTo = 131072
		Start_Menu = 262144
		Startup = 524288
		Templates = 1048576
	End Enum
	Function MyEnvGetSpecFolder(ByVal f As EnvSpFldr) As String
		Return Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", [Enum].GetName(GetType(EnvSpFldr), f), "").ToString
	End Function
	Declare Function HideCaret Lib "user32.dll" (ByVal hWnd As IntPtr) As Boolean
	Declare Function GetCaretBlinkTime Lib "user32.dll" () As Integer
#End Region
#Region "Win7"
	'
	'iTaskBar3
	'
	<GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")> _
	  <ClassInterfaceAttribute(ClassInterfaceType.None)> _
	  <ComImportAttribute()> _
	  Class CTaskbarList
	End Class
	<ComImportAttribute()> _
	<GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")> _
	<InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)> _
	Interface ITaskbarList3
		' ITaskbarList
		<PreserveSig()> Sub HrInit()
		<PreserveSig()> Sub AddTab(ByVal hwnd As IntPtr)
		<PreserveSig()> Sub DeleteTab(ByVal hwnd As IntPtr)
		<PreserveSig()> Sub ActivateTab(ByVal hwnd As IntPtr)
		<PreserveSig()> Sub SetActiveAlt(ByVal hwnd As IntPtr)

		' ITaskbarList2
		<PreserveSig()> Sub MarkFullscreenWindow(ByVal hwnd As IntPtr, ByVal hwndTab As IntPtr, <MarshalAs(UnmanagedType.Bool)> ByVal fFullscreen As Boolean)

		' ITaskbarList3
		Sub SetProgressValue(ByVal hwnd As IntPtr, ByVal ullCompleted As UInt64, ByVal ullTotal As UInt64)
		Sub SetProgressState(ByVal hwnd As IntPtr, ByVal tbpFlags As TBPFLAG)
		Sub RegisterTab(ByVal hwndTab As IntPtr, ByVal hwndMDI As IntPtr)
		Sub UnregisterTab(ByVal hwndTab As IntPtr)
		Sub SetTabOrder(ByVal hwndTab As IntPtr, ByVal hwndInsertBefore As IntPtr)
		Sub SetTabActive(ByVal hwndTab As IntPtr, ByVal hwndMDI As IntPtr, ByVal tbatFlags As TBATFLAG)
		Sub ThumbBarAddButtons(ByVal hwnd As IntPtr, ByVal cButtons As UInteger, ByVal hwndTab As IntPtr, <MarshalAs(UnmanagedType.LPArray)> ByVal pButtons As THUMBBUTTON())
		Sub ThumbBarUpdateButtons(ByVal hwnd As IntPtr, ByVal cButtons As UInteger, ByVal hwndTab As IntPtr, <MarshalAs(UnmanagedType.LPArray)> ByVal pButtons As THUMBBUTTON())
		Sub ThumbBarSetImageList(ByVal hwnd As IntPtr, ByVal himl As IntPtr)
		Sub SetOverlayIcon(ByVal hwnd As IntPtr, ByVal hIcon As IntPtr, <MarshalAs(UnmanagedType.LPWStr)> ByVal pszDescription As String)
		Sub SetThumbnailTooltip(ByVal hwnd As IntPtr, <MarshalAs(UnmanagedType.LPWStr)> ByVal pszTip As String)
		Sub SetThumbnailClip(ByVal hwnd As IntPtr, ByRef prcClip As RECT)
	End Interface
	<StructLayout(LayoutKind.Sequential, Pack:=4, CharSet:=CharSet.Auto)> Structure THUMBBUTTON
		<MarshalAs(UnmanagedType.U4)> Public dwMask As THBMASK
		Public iId As UInteger
		Public iBitmap As UInteger
		Public hIcon As IntPtr
		<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)> Public szTip As String
		<MarshalAs(UnmanagedType.U4)> Public dwFlags As THBFLAGS
	End Structure
	Enum THBMASK
		THB_BITMAP = 1
		THB_ICON = 2
		THB_TOOLTIP = 4
		THB_FLAGS = 8
	End Enum
	Enum THBFLAGS
		THBF_ENABLED = 0
		THBF_DISABLED = 1
		THBF_DISMISSONCLICK = 2
		THBF_NOBACKGROUND = 4
		THBF_HIDDEN = 8
	End Enum
	Enum TBPFLAG
		TBPF_NOPROGRESS = 0
		TBPF_INDETERMINATE = 1
		TBPF_NORMAL = 2
		TBPF_ERROR = 4
		TBPF_PAUSED = 8

	End Enum
	Enum TBATFLAG
		TBATF_USEMDITHUMBNAIL = 1
		TBATF_USEMDILIVEPREVIEW = 2

	End Enum
	'
	'DWM API
	'
	Declare Sub DwmEnableBlurBehindWindow Lib "dwmapi.dll" (ByVal hWnd As IntPtr, ByVal pBlurBehind As DWM_BLURBEHIND)
	Declare Sub DwmExtendFrameIntoClientArea Lib "dwmapi.dll" (ByVal hWnd As IntPtr, ByVal pMargins As MARGINS)
	<DllImport("dwmapi.dll", PreserveSig:=False)> Public Function DwmIsCompositionEnabled() As Boolean
	End Function
	'Declare Function DwmIsCompositionEnabled Lib "dwmapi.dll" () As Boolean
	<StructLayout(LayoutKind.Sequential)> Public Class MARGINS
		Public cxLeftWidth, cxRightWidth, cyTopHeight, cyBottomHeight As Integer
		Sub New(ByVal left As Integer, ByVal top As Integer, ByVal right As Integer, ByVal bottom As Integer)
			cxLeftWidth = left : cyTopHeight = top
			cxRightWidth = right : cyBottomHeight = bottom
		End Sub
	End Class
	<StructLayout(LayoutKind.Sequential)> Public Class DWM_BLURBEHIND
		Public dwFlags As UInteger
		<MarshalAs(UnmanagedType.Bool)> Public fEnable As Boolean
		Public hRegionBlur As IntPtr
		<MarshalAs(UnmanagedType.Bool)> Public fTransitionOnMaximized As Boolean
		Public Const DWM_BB_ENABLE As UInteger = 1
		Public Const DWM_BB_BLURREGION As UInteger = 2
		Public Const DWM_BB_TRANSITIONONMAXIMIZED As UInteger = 4
	End Class
#End Region
End Module