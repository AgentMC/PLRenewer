<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HDLTF
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label
        Me.__IssuedTo = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.__IssuedBy = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.__ExtInfo = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me._Lasts = New System.Windows.Forms.NumericUpDown
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.btnLoad = New System.Windows.Forms.Button
        Me.btnSave = New System.Windows.Forms.Button
        Me.__Activated = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.__LastUsed = New System.Windows.Forms.TextBox
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
        Me.LinkLabel2 = New System.Windows.Forms.LinkLabel
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.__hLasts = New System.Windows.Forms.TextBox
        Me.GPLCheckBox = New System.Windows.Forms.CheckBox
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.__hDaysLeft = New System.Windows.Forms.TextBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.__hEndDate = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.LinkLabel4 = New System.Windows.Forms.LinkLabel
        Me.LinkLabel3 = New System.Windows.Forms.LinkLabel
        Me.__hLastUsed = New System.Windows.Forms.TextBox
        Me.__hActivated = New System.Windows.Forms.TextBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        CType(Me._Lasts, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Пользователь"
        '
        '__IssuedTo
        '
        Me.__IssuedTo.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.__IssuedTo.Location = New System.Drawing.Point(9, 32)
        Me.__IssuedTo.Multiline = True
        Me.__IssuedTo.Name = "__IssuedTo"
        Me.__IssuedTo.Size = New System.Drawing.Size(211, 112)
        Me.__IssuedTo.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 147)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(46, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Выдана"
        '
        '__IssuedBy
        '
        Me.__IssuedBy.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.__IssuedBy.Location = New System.Drawing.Point(9, 163)
        Me.__IssuedBy.Name = "__IssuedBy"
        Me.__IssuedBy.Size = New System.Drawing.Size(211, 20)
        Me.__IssuedBy.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(223, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(73, 13)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Информация"
        '
        '__ExtInfo
        '
        Me.__ExtInfo.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.__ExtInfo.Location = New System.Drawing.Point(226, 32)
        Me.__ExtInfo.Multiline = True
        Me.__ExtInfo.Name = "__ExtInfo"
        Me.__ExtInfo.Size = New System.Drawing.Size(211, 112)
        Me.__ExtInfo.TabIndex = 3
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 16)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(73, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Активирован"
        '
        '_Lasts
        '
        Me._Lasts.Location = New System.Drawing.Point(226, 163)
        Me._Lasts.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
        Me._Lasts.Name = "_Lasts"
        Me._Lasts.Size = New System.Drawing.Size(72, 20)
        Me._Lasts.TabIndex = 8
        Me._Lasts.Value = New Decimal(New Integer() {30, 0, 0, 0})
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(223, 147)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(86, 13)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "Действительна"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(304, 166)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(31, 13)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "дней"
        '
        'btnLoad
        '
        Me.btnLoad.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnLoad.Location = New System.Drawing.Point(12, 326)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(75, 23)
        Me.btnLoad.TabIndex = 9
        Me.btnLoad.Text = "Загрузить"
        Me.btnLoad.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnSave.Location = New System.Drawing.Point(275, 326)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 10
        Me.btnSave.Text = "Сохранить"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        '__Activated
        '
        Me.__Activated.Location = New System.Drawing.Point(85, 13)
        Me.__Activated.MaxLength = 8
        Me.__Activated.Name = "__Activated"
        Me.__Activated.Size = New System.Drawing.Size(58, 20)
        Me.__Activated.TabIndex = 4
        Me.__Activated.Text = "00000000"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(223, 16)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(75, 13)
        Me.Label7.TabIndex = 11
        Me.Label7.Text = "Использован"
        '
        '__LastUsed
        '
        Me.__LastUsed.Location = New System.Drawing.Point(302, 13)
        Me.__LastUsed.MaxLength = 8
        Me.__LastUsed.Name = "__LastUsed"
        Me.__LastUsed.Size = New System.Drawing.Size(58, 20)
        Me.__LastUsed.TabIndex = 6
        Me.__LastUsed.Text = "00000000"
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(149, 16)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(27, 13)
        Me.LinkLabel1.TabIndex = 5
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "now"
        '
        'LinkLabel2
        '
        Me.LinkLabel2.AutoSize = True
        Me.LinkLabel2.Location = New System.Drawing.Point(366, 16)
        Me.LinkLabel2.Name = "LinkLabel2"
        Me.LinkLabel2.Size = New System.Drawing.Size(27, 13)
        Me.LinkLabel2.TabIndex = 7
        Me.LinkLabel2.TabStop = True
        Me.LinkLabel2.Text = "now"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.__hLasts)
        Me.GroupBox1.Controls.Add(Me.GPLCheckBox)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.__IssuedTo)
        Me.GroupBox1.Controls.Add(Me._Lasts)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.__IssuedBy)
        Me.GroupBox1.Controls.Add(Me.__ExtInfo)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(456, 194)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Лицензия "
        '
        '__hLasts
        '
        Me.__hLasts.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.__hLasts.Location = New System.Drawing.Point(341, 162)
        Me.__hLasts.Name = "__hLasts"
        Me.__hLasts.ReadOnly = True
        Me.__hLasts.Size = New System.Drawing.Size(96, 20)
        Me.__hLasts.TabIndex = 10
        '
        'GPLCheckBox
        '
        Me.GPLCheckBox.AutoSize = True
        Me.GPLCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.GPLCheckBox.Enabled = False
        Me.GPLCheckBox.Location = New System.Drawing.Point(409, 2)
        Me.GPLCheckBox.Name = "GPLCheckBox"
        Me.GPLCheckBox.Size = New System.Drawing.Size(47, 17)
        Me.GPLCheckBox.TabIndex = 9
        Me.GPLCheckBox.Text = "GPL"
        Me.GPLCheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.__hDaysLeft)
        Me.GroupBox2.Controls.Add(Me.Label9)
        Me.GroupBox2.Controls.Add(Me.__hEndDate)
        Me.GroupBox2.Controls.Add(Me.Label8)
        Me.GroupBox2.Controls.Add(Me.LinkLabel4)
        Me.GroupBox2.Controls.Add(Me.LinkLabel3)
        Me.GroupBox2.Controls.Add(Me.__hLastUsed)
        Me.GroupBox2.Controls.Add(Me.__hActivated)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.LinkLabel2)
        Me.GroupBox2.Controls.Add(Me.__Activated)
        Me.GroupBox2.Controls.Add(Me.__LastUsed)
        Me.GroupBox2.Controls.Add(Me.LinkLabel1)
        Me.GroupBox2.Controls.Add(Me.Label7)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 212)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(456, 108)
        Me.GroupBox2.TabIndex = 13
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Файл License.hdk"
        '
        '__hDaysLeft
        '
        Me.__hDaysLeft.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.__hDaysLeft.Location = New System.Drawing.Point(226, 78)
        Me.__hDaysLeft.Name = "__hDaysLeft"
        Me.__hDaysLeft.ReadOnly = True
        Me.__hDaysLeft.Size = New System.Drawing.Size(134, 20)
        Me.__hDaysLeft.TabIndex = 18
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(223, 62)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(56, 13)
        Me.Label9.TabIndex = 17
        Me.Label9.Text = "Осталось"
        '
        '__hEndDate
        '
        Me.__hEndDate.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.__hEndDate.Location = New System.Drawing.Point(9, 78)
        Me.__hEndDate.Name = "__hEndDate"
        Me.__hEndDate.ReadOnly = True
        Me.__hEndDate.Size = New System.Drawing.Size(134, 20)
        Me.__hEndDate.TabIndex = 16
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(6, 62)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(81, 13)
        Me.Label8.TabIndex = 15
        Me.Label8.Text = "Конечная дата"
        '
        'LinkLabel4
        '
        Me.LinkLabel4.AutoSize = True
        Me.LinkLabel4.Location = New System.Drawing.Point(399, 16)
        Me.LinkLabel4.Name = "LinkLabel4"
        Me.LinkLabel4.Size = New System.Drawing.Size(30, 13)
        Me.LinkLabel4.TabIndex = 14
        Me.LinkLabel4.TabStop = True
        Me.LinkLabel4.Text = "clear"
        '
        'LinkLabel3
        '
        Me.LinkLabel3.AutoSize = True
        Me.LinkLabel3.Location = New System.Drawing.Point(182, 16)
        Me.LinkLabel3.Name = "LinkLabel3"
        Me.LinkLabel3.Size = New System.Drawing.Size(30, 13)
        Me.LinkLabel3.TabIndex = 13
        Me.LinkLabel3.TabStop = True
        Me.LinkLabel3.Text = "clear"
        '
        '__hLastUsed
        '
        Me.__hLastUsed.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.__hLastUsed.Location = New System.Drawing.Point(226, 39)
        Me.__hLastUsed.Name = "__hLastUsed"
        Me.__hLastUsed.ReadOnly = True
        Me.__hLastUsed.Size = New System.Drawing.Size(134, 20)
        Me.__hLastUsed.TabIndex = 12
        '
        '__hActivated
        '
        Me.__hActivated.BackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.__hActivated.Location = New System.Drawing.Point(9, 39)
        Me.__hActivated.Name = "__hActivated"
        Me.__hActivated.ReadOnly = True
        Me.__hActivated.Size = New System.Drawing.Size(134, 20)
        Me.__hActivated.TabIndex = 6
        '
        'Button1
        '
        Me.Button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Button1.Location = New System.Drawing.Point(93, 326)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(139, 23)
        Me.Button1.TabIndex = 14
        Me.Button1.Text = "Загрузить без тестов"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Button2.Location = New System.Drawing.Point(393, 326)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 15
        Me.Button2.Text = "Закрыть"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(470, 362)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnLoad)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.Text = "HACK-Design Licensing Tool"
        CType(Me._Lasts, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents __IssuedTo As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents __IssuedBy As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents __ExtInfo As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents _Lasts As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btnLoad As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents __Activated As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents __LastUsed As System.Windows.Forms.TextBox
    Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
    Friend WithEvents LinkLabel2 As System.Windows.Forms.LinkLabel
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents __hLastUsed As System.Windows.Forms.TextBox
    Friend WithEvents __hActivated As System.Windows.Forms.TextBox
    Friend WithEvents GPLCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents __hLasts As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents LinkLabel4 As System.Windows.Forms.LinkLabel
    Friend WithEvents LinkLabel3 As System.Windows.Forms.LinkLabel
    Friend WithEvents __hDaysLeft As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents __hEndDate As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label

End Class
