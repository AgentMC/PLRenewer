<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormMDX
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMDX))
		Me.LinkLabel2 = New System.Windows.Forms.LinkLabel
		Me.Panel1 = New System.Windows.Forms.Panel
		Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
		Me.LinkLabel3 = New System.Windows.Forms.LinkLabel
		Me.TextBox1 = New System.Windows.Forms.TextBox
		Me.Panel1.SuspendLayout()
		Me.SuspendLayout()
		'
		'LinkLabel2
		'
		Me.LinkLabel2.AutoSize = True
		Me.LinkLabel2.Location = New System.Drawing.Point(84, 3)
		Me.LinkLabel2.Name = "LinkLabel2"
		Me.LinkLabel2.Size = New System.Drawing.Size(239, 13)
		Me.LinkLabel2.TabIndex = 2
		Me.LinkLabel2.TabStop = True
		Me.LinkLabel2.Text = "Перейти к описанию DirectX за ферваль 2010"
		'
		'Panel1
		'
		Me.Panel1.Controls.Add(Me.LinkLabel1)
		Me.Panel1.Controls.Add(Me.LinkLabel3)
		Me.Panel1.Controls.Add(Me.LinkLabel2)
		Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.Panel1.Location = New System.Drawing.Point(0, 263)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(574, 23)
		Me.Panel1.TabIndex = 3
		'
		'LinkLabel1
		'
		Me.LinkLabel1.AutoSize = True
		Me.LinkLabel1.Location = New System.Drawing.Point(3, 3)
		Me.LinkLabel1.Name = "LinkLabel1"
		Me.LinkLabel1.Size = New System.Drawing.Size(75, 13)
		Me.LinkLabel1.TabIndex = 4
		Me.LinkLabel1.TabStop = True
		Me.LinkLabel1.Text = "Скачать MDX"
		'
		'LinkLabel3
		'
		Me.LinkLabel3.AutoSize = True
		Me.LinkLabel3.Location = New System.Drawing.Point(329, 3)
		Me.LinkLabel3.Name = "LinkLabel3"
		Me.LinkLabel3.Size = New System.Drawing.Size(240, 13)
		Me.LinkLabel3.TabIndex = 3
		Me.LinkLabel3.TabStop = True
		Me.LinkLabel3.Text = "Поместить ссылку на DirectX в буфер обмена"
		'
		'TextBox1
		'
		Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.TextBox1.Location = New System.Drawing.Point(0, 0)
		Me.TextBox1.Multiline = True
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.ReadOnly = True
		Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
		Me.TextBox1.Size = New System.Drawing.Size(574, 263)
		Me.TextBox1.TabIndex = 4
		Me.TextBox1.Text = resources.GetString("TextBox1.Text")
		'
		'FormMDX
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(574, 286)
		Me.Controls.Add(Me.TextBox1)
		Me.Controls.Add(Me.Panel1)
		Me.Name = "FormMDX"
		Me.Text = "Управляемые библиотеки DX - инфо"
		Me.Panel1.ResumeLayout(False)
		Me.Panel1.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents LinkLabel2 As System.Windows.Forms.LinkLabel
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
	Friend WithEvents LinkLabel3 As System.Windows.Forms.LinkLabel
	Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
End Class
