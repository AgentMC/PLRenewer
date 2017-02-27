<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PLPW
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
		Me.components = New System.ComponentModel.Container
		Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
		Me.TestLabel = New System.Windows.Forms.Label
		Me.SuspendLayout()
		'
		'Timer1
		'
		Me.Timer1.Interval = 17
		'
		'TestLabel
		'
		Me.TestLabel.AutoSize = True
		Me.TestLabel.Font = New System.Drawing.Font("Times New Roman", 15.75!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
		Me.TestLabel.Location = New System.Drawing.Point(52, 23)
		Me.TestLabel.Name = "TestLabel"
		Me.TestLabel.Size = New System.Drawing.Size(70, 23)
		Me.TestLabel.TabIndex = 1
		Me.TestLabel.Text = "Label1"
		Me.TestLabel.Visible = False
		'
		'PLPleaseWait
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(523, 86)
		Me.Controls.Add(Me.TestLabel)
		Me.ForeColor = System.Drawing.Color.Lime
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.Name = "PLPleaseWait"
		Me.ShowInTaskbar = False
		Me.Text = "PLPleaseWait"
		Me.TransparencyKey = System.Drawing.SystemColors.Control
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Timer1 As System.Windows.Forms.Timer
	Friend WithEvents TestLabel As System.Windows.Forms.Label
End Class
