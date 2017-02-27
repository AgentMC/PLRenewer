Public Class HDLTF
    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        License.SetLicense(New LicInfo(__IssuedTo.Text, __IssuedBy.Text, __ExtInfo.Text, CULng(__Activated.Text), CULng(__LastUsed.Text), CUShort(_Lasts.Value), False))
        Text = "HACK-Design Licensing Tool: лицензия license.hdk сохранена"
    End Sub
    Private Sub btnLoad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoad.Click
        Proso(License.GetLicense(False))
    End Sub
    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        __Activated.Text = Format(License.Now2ULong, "00000000")
    End Sub
    Private Sub LinkLabel2_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        __LastUsed.Text = Format(License.Now2ULong, "00000000")
    End Sub
    Private Sub LinkLabel3_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        __Activated.Text = "00000000"
    End Sub
    Private Sub LinkLabel4_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel4.LinkClicked
        __LastUsed.Text = "00000000"
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Randomize()
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Proso(License.GetLicense(True))
    End Sub
    Private Sub Proso(ByVal tLic As LicInfo)
        Dim tLic2 As License.HumanizedLicInfo = tLic.Humanize
        'Public
        __ExtInfo.Text = tLic2.ExtInfo
        __IssuedBy.Text = tLic2.IssuedBy
        __IssuedTo.Text = tLic2.UserName
        __hLasts.Text = tLic2.DaysValid
        __hActivated.Text = tLic2.ActivationDate
        __hLastUsed.Text = tLic2.LastUsageDate
        __hEndDate.Text = tLic2.EndDate
        __hDaysLeft.Text = tLic2.DaysLeft
        'Private
        __LastUsed.Text = Format(tLic.LastUsageDate, "00000000")
        __Activated.Text = Format(tLic.ActivationDate, "00000000")
        _Lasts.Value = tLic.DaysValid
        Text = "HACK-Design Licensing Tool: лицензия license.hdk"
        GPLCheckBox.Checked = tLic.GPL
    End Sub
End Class