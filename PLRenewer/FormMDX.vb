Public Class FormMDX
	Const mDX As String = "http://download.microsoft.com/download/8/4/A/84A35BF1-DAFE-4AE8-82AF-AD2AE20B6B14/directx_Jun2010_redist.exe"
	Const mDxDescr As String = "http://www.microsoft.com/downloads/details.aspx?displaylang=en&FamilyID=3b170b25-abab-4bc3-ae91-50ceb6d8fa8d"
	Private Sub LinkLabel3_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
		Clipboard.SetText(mDX)
	End Sub
	Private Sub LinkLabel1_LinkClicked_1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
		Process.Start(mDX)
	End Sub
	Private Sub LinkLabel2_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
		Process.Start(mDxDescr)
	End Sub
End Class