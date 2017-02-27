Partial Class Updater
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
		Me.Label1 = New System.Windows.Forms.Label()
		Me.TextBox1 = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.Button2 = New System.Windows.Forms.Button()
		Me.Button3 = New System.Windows.Forms.Button()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.SuspendLayout()
		'
		'Label1
		'
		Me.Label1.Font = New System.Drawing.Font("Segoe Print", 21.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
		Me.Label1.Location = New System.Drawing.Point(12, 1)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(507, 64)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Вышло обновление PLReNewer"
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'TextBox1
		'
		Me.TextBox1.Location = New System.Drawing.Point(12, 120)
		Me.TextBox1.Multiline = True
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.ReadOnly = True
		Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
		Me.TextBox1.Size = New System.Drawing.Size(507, 105)
		Me.TextBox1.TabIndex = 1
		Me.TextBox1.Text = "...пока что можно посмотреть онлайн в моём блоге по адресу" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "http://forum.kaspersk" & _
		 "yclub.ru/blog/agentmc/index.php?showentry=1269"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(12, 65)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(103, 52)
		Me.Label2.TabIndex = 2
		Me.Label2.Text = "Текущая версия" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Доступная версия" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Список изменений"
		'
		'Button1
		'
		Me.Button1.Font = New System.Drawing.Font("Segoe UI Symbol", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Button1.Location = New System.Drawing.Point(12, 231)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(210, 62)
		Me.Button1.TabIndex = 3
		Me.Button1.Text = "Обновиться"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'Button2
		'
		Me.Button2.Font = New System.Drawing.Font("Segoe UI Symbol", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Button2.Location = New System.Drawing.Point(231, 231)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(141, 62)
		Me.Button2.TabIndex = 4
		Me.Button2.Text = "Не сейчас"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'Button3
		'
		Me.Button3.Font = New System.Drawing.Font("Segoe UI Symbol", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Button3.Location = New System.Drawing.Point(378, 231)
		Me.Button3.Name = "Button3"
		Me.Button3.Size = New System.Drawing.Size(141, 62)
		Me.Button3.TabIndex = 5
		Me.Button3.Text = "Игнорировать эту версию"
		Me.Button3.UseVisualStyleBackColor = True
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(121, 65)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(101, 26)
		Me.Label3.TabIndex = 2
		Me.Label3.Text = "Текущая версия" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Доступная версия"
		'
		'ProgressBar1
		'
		Me.ProgressBar1.Location = New System.Drawing.Point(228, 65)
		Me.ProgressBar1.Name = "ProgressBar1"
		Me.ProgressBar1.Size = New System.Drawing.Size(291, 23)
		Me.ProgressBar1.TabIndex = 0
		Me.ProgressBar1.Visible = False
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(225, 91)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(135, 26)
		Me.Label5.TabIndex = 6
		Me.Label5.Text = "Размер обновлений 0 КБ" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Скорость 0 КБ/с"
		Me.Label5.Visible = False
		'
		'Updater
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(533, 305)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.ProgressBar1)
		Me.Controls.Add(Me.Button3)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.TextBox1)
		Me.Controls.Add(Me.Label1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "Updater"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Обновление"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Friend WithEvents Button2 As System.Windows.Forms.Button
	Friend WithEvents Button3 As System.Windows.Forms.Button
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
	Friend WithEvents Label5 As System.Windows.Forms.Label
End Class
