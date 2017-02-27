<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AutoCross
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
		Me.Label1 = New System.Windows.Forms.Label
		Me.Button1 = New System.Windows.Forms.Button
		Me.Button2 = New System.Windows.Forms.Button
		Me.Button3 = New System.Windows.Forms.Button
		Me.Button4 = New System.Windows.Forms.Button
		Me.Label2 = New System.Windows.Forms.Label
		Me.Label3 = New System.Windows.Forms.Label
		Me.Label4 = New System.Windows.Forms.Label
		Me.Label5 = New System.Windows.Forms.Label
		Me.SuspendLayout()
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New System.Drawing.Font("Times New Roman", 12.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(204, Byte))
		Me.Label1.Location = New System.Drawing.Point(12, 9)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(412, 19)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Пожалуйста подождите, выполняется тестирование..."
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'Button1
		'
		Me.Button1.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.Button1.Location = New System.Drawing.Point(28, 44)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(91, 23)
		Me.Button1.TabIndex = 1
		Me.Button1.Text = "Минимум"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'Button2
		'
		Me.Button2.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.Button2.Location = New System.Drawing.Point(125, 44)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(91, 23)
		Me.Button2.TabIndex = 2
		Me.Button2.Text = "Среднее"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'Button3
		'
		Me.Button3.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.Button3.Location = New System.Drawing.Point(222, 44)
		Me.Button3.Name = "Button3"
		Me.Button3.Size = New System.Drawing.Size(91, 23)
		Me.Button3.TabIndex = 3
		Me.Button3.Text = "Максимум"
		Me.Button3.UseVisualStyleBackColor = True
		'
		'Button4
		'
		Me.Button4.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.Button4.Location = New System.Drawing.Point(319, 44)
		Me.Button4.Name = "Button4"
		Me.Button4.Size = New System.Drawing.Size(91, 23)
		Me.Button4.TabIndex = 4
		Me.Button4.Text = "Самое частое"
		Me.Button4.UseVisualStyleBackColor = True
		'
		'Label2
		'
		Me.Label2.Location = New System.Drawing.Point(25, 70)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(94, 13)
		Me.Label2.TabIndex = 5
		Me.Label2.Text = "Label2"
		Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'Label3
		'
		Me.Label3.Location = New System.Drawing.Point(122, 70)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(94, 13)
		Me.Label3.TabIndex = 6
		Me.Label3.Text = "Label3"
		Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'Label4
		'
		Me.Label4.Location = New System.Drawing.Point(219, 70)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(94, 13)
		Me.Label4.TabIndex = 7
		Me.Label4.Text = "Label4"
		Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'Label5
		'
		Me.Label5.Location = New System.Drawing.Point(316, 70)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(94, 13)
		Me.Label5.TabIndex = 8
		Me.Label5.Text = "Label5"
		Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'AutoCross
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(435, 37)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.Button4)
		Me.Controls.Add(Me.Button3)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.Label1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
		Me.Name = "AutoCross"
		Me.ShowInTaskbar = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "AutoCross"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Friend WithEvents Button2 As System.Windows.Forms.Button
	Friend WithEvents Button3 As System.Windows.Forms.Button
	Friend WithEvents Button4 As System.Windows.Forms.Button
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents Label5 As System.Windows.Forms.Label
End Class
