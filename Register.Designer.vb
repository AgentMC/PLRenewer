<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Register
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Register))
		Me.Panel1 = New System.Windows.Forms.Panel
		Me.Button2 = New System.Windows.Forms.Button
		Me.Button1 = New System.Windows.Forms.Button
		Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
		Me.CLB_in = New System.Windows.Forms.CheckedListBox
		Me.CLB_out = New System.Windows.Forms.CheckedListBox
		Me.Panel1.SuspendLayout()
		Me.SplitContainer1.Panel1.SuspendLayout()
		Me.SplitContainer1.Panel2.SuspendLayout()
		Me.SplitContainer1.SuspendLayout()
		Me.SuspendLayout()
		'
		'Panel1
		'
		Me.Panel1.Controls.Add(Me.Button2)
		Me.Panel1.Controls.Add(Me.Button1)
		Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.Panel1.Location = New System.Drawing.Point(0, 300)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(408, 28)
		Me.Panel1.TabIndex = 0
		'
		'Button2
		'
		Me.Button2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.Button2.Location = New System.Drawing.Point(321, 3)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(75, 23)
		Me.Button2.TabIndex = 1
		Me.Button2.Text = "Отмена"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'Button1
		'
		Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Button1.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.Button1.Location = New System.Drawing.Point(240, 3)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(75, 23)
		Me.Button1.TabIndex = 0
		Me.Button1.Text = "ОК"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'SplitContainer1
		'
		Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.SplitContainer1.IsSplitterFixed = True
		Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
		Me.SplitContainer1.Name = "SplitContainer1"
		Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
		'
		'SplitContainer1.Panel1
		'
		Me.SplitContainer1.Panel1.Controls.Add(Me.CLB_in)
		'
		'SplitContainer1.Panel2
		'
		Me.SplitContainer1.Panel2.Controls.Add(Me.CLB_out)
		Me.SplitContainer1.Size = New System.Drawing.Size(408, 300)
		Me.SplitContainer1.SplitterDistance = 148
		Me.SplitContainer1.TabIndex = 1
		'
		'CLB_in
		'
		Me.CLB_in.CheckOnClick = True
		Me.CLB_in.Dock = System.Windows.Forms.DockStyle.Fill
		Me.CLB_in.FormattingEnabled = True
		Me.CLB_in.IntegralHeight = False
		Me.CLB_in.Location = New System.Drawing.Point(0, 0)
		Me.CLB_in.Name = "CLB_in"
		Me.CLB_in.Size = New System.Drawing.Size(408, 148)
		Me.CLB_in.TabIndex = 0
		'
		'CLB_out
		'
		Me.CLB_out.CheckOnClick = True
		Me.CLB_out.Dock = System.Windows.Forms.DockStyle.Fill
		Me.CLB_out.FormattingEnabled = True
		Me.CLB_out.IntegralHeight = False
		Me.CLB_out.Location = New System.Drawing.Point(0, 0)
		Me.CLB_out.Name = "CLB_out"
		Me.CLB_out.Size = New System.Drawing.Size(408, 148)
		Me.CLB_out.TabIndex = 0
		'
		'Register
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(408, 328)
		Me.Controls.Add(Me.SplitContainer1)
		Me.Controls.Add(Me.Panel1)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Name = "Register"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "Выбор элементов списков"
		Me.Panel1.ResumeLayout(False)
		Me.SplitContainer1.Panel1.ResumeLayout(False)
		Me.SplitContainer1.Panel2.ResumeLayout(False)
		Me.SplitContainer1.ResumeLayout(False)
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
	Friend WithEvents CLB_in As System.Windows.Forms.CheckedListBox
	Friend WithEvents CLB_out As System.Windows.Forms.CheckedListBox
	Friend WithEvents Button2 As System.Windows.Forms.Button
	Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
