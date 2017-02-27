<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class About
	Inherits System.Windows.Forms.Form

	'Форма переопределяет dispose для очистки списка компонентов.
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

	'Является обязательной для конструктора форм Windows Forms
	Private components As System.ComponentModel.IContainer

	'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
	'Для ее изменения используйте конструктор форм Windows Form.  
	'Не изменяйте ее в редакторе исходного кода.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(About))
		Me.__IssuedTo = New System.Windows.Forms.TextBox()
		Me.__ExtInfo = New System.Windows.Forms.TextBox()
		Me.__IssuedBy = New System.Windows.Forms.TextBox()
		Me.__hLasts = New System.Windows.Forms.TextBox()
		Me.__hActivated = New System.Windows.Forms.TextBox()
		Me.__hEndDate = New System.Windows.Forms.TextBox()
		Me.__hLastUsed = New System.Windows.Forms.TextBox()
		Me.__hDaysLeft = New System.Windows.Forms.TextBox()
		Me.GroupBox1 = New System.Windows.Forms.GroupBox()
		Me.LinkLabel2 = New System.Windows.Forms.LinkLabel()
		Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
		Me.Label9 = New System.Windows.Forms.Label()
		Me._PVer = New System.Windows.Forms.Label()
		Me.GroupBox2 = New System.Windows.Forms.GroupBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.PictureBox1 = New System.Windows.Forms.PictureBox()
		Me.GroupBox1.SuspendLayout()
		Me.GroupBox2.SuspendLayout()
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'__IssuedTo
		'
		Me.__IssuedTo.Location = New System.Drawing.Point(6, 32)
		Me.__IssuedTo.Multiline = True
		Me.__IssuedTo.Name = "__IssuedTo"
		Me.__IssuedTo.ReadOnly = True
		Me.__IssuedTo.Size = New System.Drawing.Size(236, 88)
		Me.__IssuedTo.TabIndex = 0
		'
		'__ExtInfo
		'
		Me.__ExtInfo.Location = New System.Drawing.Point(248, 32)
		Me.__ExtInfo.Multiline = True
		Me.__ExtInfo.Name = "__ExtInfo"
		Me.__ExtInfo.ReadOnly = True
		Me.__ExtInfo.Size = New System.Drawing.Size(244, 88)
		Me.__ExtInfo.TabIndex = 1
		'
		'__IssuedBy
		'
		Me.__IssuedBy.Location = New System.Drawing.Point(6, 139)
		Me.__IssuedBy.Name = "__IssuedBy"
		Me.__IssuedBy.ReadOnly = True
		Me.__IssuedBy.Size = New System.Drawing.Size(236, 20)
		Me.__IssuedBy.TabIndex = 2
		'
		'__hLasts
		'
		Me.__hLasts.Location = New System.Drawing.Point(248, 139)
		Me.__hLasts.Name = "__hLasts"
		Me.__hLasts.ReadOnly = True
		Me.__hLasts.Size = New System.Drawing.Size(244, 20)
		Me.__hLasts.TabIndex = 3
		'
		'__hActivated
		'
		Me.__hActivated.Location = New System.Drawing.Point(6, 178)
		Me.__hActivated.Name = "__hActivated"
		Me.__hActivated.ReadOnly = True
		Me.__hActivated.Size = New System.Drawing.Size(236, 20)
		Me.__hActivated.TabIndex = 4
		'
		'__hEndDate
		'
		Me.__hEndDate.Location = New System.Drawing.Point(248, 178)
		Me.__hEndDate.Name = "__hEndDate"
		Me.__hEndDate.ReadOnly = True
		Me.__hEndDate.Size = New System.Drawing.Size(244, 20)
		Me.__hEndDate.TabIndex = 5
		'
		'__hLastUsed
		'
		Me.__hLastUsed.Location = New System.Drawing.Point(6, 217)
		Me.__hLastUsed.Name = "__hLastUsed"
		Me.__hLastUsed.ReadOnly = True
		Me.__hLastUsed.Size = New System.Drawing.Size(236, 20)
		Me.__hLastUsed.TabIndex = 6
		'
		'__hDaysLeft
		'
		Me.__hDaysLeft.Location = New System.Drawing.Point(248, 217)
		Me.__hDaysLeft.Name = "__hDaysLeft"
		Me.__hDaysLeft.ReadOnly = True
		Me.__hDaysLeft.Size = New System.Drawing.Size(246, 20)
		Me.__hDaysLeft.TabIndex = 7
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.LinkLabel2)
		Me.GroupBox1.Controls.Add(Me.LinkLabel1)
		Me.GroupBox1.Controls.Add(Me.Label9)
		Me.GroupBox1.Controls.Add(Me._PVer)
		Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(500, 209)
		Me.GroupBox1.TabIndex = 8
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Программа"
		'
		'LinkLabel2
		'
		Me.LinkLabel2.AutoSize = True
		Me.LinkLabel2.Location = New System.Drawing.Point(181, 193)
		Me.LinkLabel2.Name = "LinkLabel2"
		Me.LinkLabel2.Size = New System.Drawing.Size(196, 13)
		Me.LinkLabel2.TabIndex = 5
		Me.LinkLabel2.TabStop = True
		Me.LinkLabel2.Text = "Сайт программы на Code.Google.com"
		'
		'LinkLabel1
		'
		Me.LinkLabel1.AutoSize = True
		Me.LinkLabel1.Location = New System.Drawing.Point(6, 193)
		Me.LinkLabel1.Name = "LinkLabel1"
		Me.LinkLabel1.Size = New System.Drawing.Size(169, 13)
		Me.LinkLabel1.TabIndex = 4
		Me.LinkLabel1.TabStop = True
		Me.LinkLabel1.Text = "Написать письмо разработчику"
		'
		'Label9
		'
		Me.Label9.Location = New System.Drawing.Point(6, 90)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(488, 103)
		Me.Label9.TabIndex = 1
		Me.Label9.Text = resources.GetString("Label9.Text")
		'
		'_PVer
		'
		Me._PVer.AutoSize = True
		Me._PVer.Location = New System.Drawing.Point(6, 16)
		Me._PVer.Name = "_PVer"
		Me._PVer.Size = New System.Drawing.Size(43, 13)
		Me._PVer.TabIndex = 0
		Me._PVer.Text = "PLRv..."
		'
		'GroupBox2
		'
		Me.GroupBox2.Controls.Add(Me.Label8)
		Me.GroupBox2.Controls.Add(Me.Label7)
		Me.GroupBox2.Controls.Add(Me.__hDaysLeft)
		Me.GroupBox2.Controls.Add(Me.Label6)
		Me.GroupBox2.Controls.Add(Me.__hLastUsed)
		Me.GroupBox2.Controls.Add(Me.Label5)
		Me.GroupBox2.Controls.Add(Me.__hEndDate)
		Me.GroupBox2.Controls.Add(Me.Label4)
		Me.GroupBox2.Controls.Add(Me.__hActivated)
		Me.GroupBox2.Controls.Add(Me.Label3)
		Me.GroupBox2.Controls.Add(Me.__hLasts)
		Me.GroupBox2.Controls.Add(Me.Label2)
		Me.GroupBox2.Controls.Add(Me.__IssuedBy)
		Me.GroupBox2.Controls.Add(Me.Label1)
		Me.GroupBox2.Controls.Add(Me.__ExtInfo)
		Me.GroupBox2.Controls.Add(Me.__IssuedTo)
		Me.GroupBox2.Location = New System.Drawing.Point(12, 227)
		Me.GroupBox2.Name = "GroupBox2"
		Me.GroupBox2.Size = New System.Drawing.Size(500, 246)
		Me.GroupBox2.TabIndex = 9
		Me.GroupBox2.TabStop = False
		Me.GroupBox2.Text = "Лицензия"
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(245, 201)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(56, 13)
		Me.Label8.TabIndex = 8
		Me.Label8.Text = "Осталось"
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(3, 201)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(198, 13)
		Me.Label7.TabIndex = 7
		Me.Label7.Text = "Последнее использование перс. лиц."
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(245, 162)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(89, 13)
		Me.Label6.TabIndex = 6
		Me.Label6.Text = "Дата окончания"
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(3, 162)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(215, 13)
		Me.Label5.TabIndex = 5
		Me.Label5.Text = "Дата активации персональной лицензии"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(245, 123)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(86, 13)
		Me.Label4.TabIndex = 4
		Me.Label4.Text = "Действительна"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(3, 123)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(60, 13)
		Me.Label3.TabIndex = 3
		Me.Label3.Text = "Выпущена"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(245, 16)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(123, 13)
		Me.Label2.TabIndex = 2
		Me.Label2.Text = "Подробнее о лицензии"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(3, 16)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(76, 13)
		Me.Label1.TabIndex = 1
		Me.Label1.Text = "Принадлежит"
		'
		'PictureBox1
		'
		Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New System.Drawing.Size(161, 119)
		Me.PictureBox1.TabIndex = 10
		Me.PictureBox1.TabStop = False
		'
		'About
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(524, 485)
		Me.Controls.Add(Me.GroupBox2)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me.PictureBox1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "About"
		Me.Text = "О программе PLReNewer"
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.GroupBox2.ResumeLayout(False)
		Me.GroupBox2.PerformLayout()
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents __IssuedTo As System.Windows.Forms.TextBox
	Friend WithEvents __ExtInfo As System.Windows.Forms.TextBox
	Friend WithEvents __IssuedBy As System.Windows.Forms.TextBox
	Friend WithEvents __hLasts As System.Windows.Forms.TextBox
	Friend WithEvents __hActivated As System.Windows.Forms.TextBox
	Friend WithEvents __hEndDate As System.Windows.Forms.TextBox
	Friend WithEvents __hLastUsed As System.Windows.Forms.TextBox
	Friend WithEvents __hDaysLeft As System.Windows.Forms.TextBox
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Friend WithEvents Label8 As System.Windows.Forms.Label
	Friend WithEvents Label7 As System.Windows.Forms.Label
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents Label5 As System.Windows.Forms.Label
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
	Friend WithEvents LinkLabel2 As System.Windows.Forms.LinkLabel
	Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
	Friend WithEvents Label9 As System.Windows.Forms.Label
	Friend WithEvents _PVer As System.Windows.Forms.Label
End Class
