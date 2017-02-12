Public Class About
	Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
		ExternalCall("mailto:agentmc@mail.ru?subject=PLRenewer feedback")
	End Sub
	Private Sub LinkLabel2_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
		ExternalCall("http://plrenewer.googlecode.com/")
	End Sub
	Private Sub ExternalCall(ByVal objString As String)
		Try
			Process.Start(objString)
		Catch ex As Exception
			MsgBox(ex.Message, MsgBoxStyle.OkOnly, "PLReNewer: ошибка")
		End Try
	End Sub
End Class