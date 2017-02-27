<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
		Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
		Me.tbtnHdzOpen = New System.Windows.Forms.ToolStripButton()
		Me.tbtnHdzNew = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
		Me.tbtnHdzAdd = New System.Windows.Forms.ToolStripButton()
		Me.tbtnHdzClone = New System.Windows.Forms.ToolStripButton()
		Me.tbtnHdzRemove = New System.Windows.Forms.ToolStripButton()
		Me.tbtnHdzRename = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
		Me.tbtnHdzSave = New System.Windows.Forms.ToolStripButton()
		Me.tbtnHdzSaveAs = New System.Windows.Forms.ToolStripButton()
		Me.tbtnHdzExtract = New System.Windows.Forms.ToolStripButton()
		Me.tbtnHdzExtractAs = New System.Windows.Forms.ToolStripButton()
		Me.tpbProgress = New System.Windows.Forms.ToolStripProgressBar()
		Me.tlName = New System.Windows.Forms.ToolStripLabel()
		Me._list = New System.Windows.Forms.ListView()
		Me.ItemName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.PhysName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.Attribs = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.Length = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.Loc = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
		Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
		Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
		Me.ToolStrip1.SuspendLayout()
		Me.SuspendLayout()
		'
		'ToolStrip1
		'
		Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tbtnHdzOpen, Me.tbtnHdzNew, Me.ToolStripSeparator2, Me.tbtnHdzAdd, Me.tbtnHdzClone, Me.tbtnHdzRemove, Me.tbtnHdzRename, Me.ToolStripSeparator1, Me.tbtnHdzSave, Me.tbtnHdzSaveAs, Me.tbtnHdzExtract, Me.tbtnHdzExtractAs, Me.tpbProgress, Me.tlName})
		Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Size = New System.Drawing.Size(628, 25)
		Me.ToolStrip1.TabIndex = 0
		Me.ToolStrip1.Text = "ToolStrip1"
		'
		'tbtnHdzOpen
		'
		Me.tbtnHdzOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzOpen.Image = CType(resources.GetObject("tbtnHdzOpen.Image"), System.Drawing.Image)
		Me.tbtnHdzOpen.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzOpen.Name = "tbtnHdzOpen"
		Me.tbtnHdzOpen.Size = New System.Drawing.Size(36, 22)
		Me.tbtnHdzOpen.Text = "|/[H]"
		Me.tbtnHdzOpen.ToolTipText = "Open HDZ"
		'
		'tbtnHdzNew
		'
		Me.tbtnHdzNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzNew.Image = CType(resources.GetObject("tbtnHdzNew.Image"), System.Drawing.Image)
		Me.tbtnHdzNew.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzNew.Name = "tbtnHdzNew"
		Me.tbtnHdzNew.Size = New System.Drawing.Size(33, 22)
		Me.tbtnHdzNew.Text = "*[H]"
		Me.tbtnHdzNew.ToolTipText = "New HDZ"
		'
		'ToolStripSeparator2
		'
		Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
		Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
		'
		'tbtnHdzAdd
		'
		Me.tbtnHdzAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzAdd.Image = CType(resources.GetObject("tbtnHdzAdd.Image"), System.Drawing.Image)
		Me.tbtnHdzAdd.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzAdd.Name = "tbtnHdzAdd"
		Me.tbtnHdzAdd.Size = New System.Drawing.Size(23, 22)
		Me.tbtnHdzAdd.Text = "+"
		Me.tbtnHdzAdd.ToolTipText = "Add item(s)"
		'
		'tbtnHdzClone
		'
		Me.tbtnHdzClone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzClone.Image = CType(resources.GetObject("tbtnHdzClone.Image"), System.Drawing.Image)
		Me.tbtnHdzClone.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzClone.Name = "tbtnHdzClone"
		Me.tbtnHdzClone.Size = New System.Drawing.Size(25, 22)
		Me.tbtnHdzClone.Text = ":][:"
		Me.tbtnHdzClone.ToolTipText = "Clone item(s)"
		'
		'tbtnHdzRemove
		'
		Me.tbtnHdzRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzRemove.Image = CType(resources.GetObject("tbtnHdzRemove.Image"), System.Drawing.Image)
		Me.tbtnHdzRemove.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzRemove.Name = "tbtnHdzRemove"
		Me.tbtnHdzRemove.Size = New System.Drawing.Size(23, 22)
		Me.tbtnHdzRemove.Text = "-"
		Me.tbtnHdzRemove.ToolTipText = "Delete item(s)"
		'
		'tbtnHdzRename
		'
		Me.tbtnHdzRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzRename.Image = CType(resources.GetObject("tbtnHdzRename.Image"), System.Drawing.Image)
		Me.tbtnHdzRename.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzRename.Name = "tbtnHdzRename"
		Me.tbtnHdzRename.Size = New System.Drawing.Size(23, 22)
		Me.tbtnHdzRename.Text = ">"
		Me.tbtnHdzRename.ToolTipText = "Move/Rename item(s)"
		'
		'ToolStripSeparator1
		'
		Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
		Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
		'
		'tbtnHdzSave
		'
		Me.tbtnHdzSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzSave.Image = CType(resources.GetObject("tbtnHdzSave.Image"), System.Drawing.Image)
		Me.tbtnHdzSave.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzSave.Name = "tbtnHdzSave"
		Me.tbtnHdzSave.Size = New System.Drawing.Size(23, 22)
		Me.tbtnHdzSave.Text = "v"
		Me.tbtnHdzSave.ToolTipText = "Save HDZ"
		'
		'tbtnHdzSaveAs
		'
		Me.tbtnHdzSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzSaveAs.Image = CType(resources.GetObject("tbtnHdzSaveAs.Image"), System.Drawing.Image)
		Me.tbtnHdzSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzSaveAs.Name = "tbtnHdzSaveAs"
		Me.tbtnHdzSaveAs.Size = New System.Drawing.Size(23, 22)
		Me.tbtnHdzSaveAs.Text = "*v"
		Me.tbtnHdzSaveAs.ToolTipText = "Save HDZ as..."
		'
		'tbtnHdzExtract
		'
		Me.tbtnHdzExtract.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzExtract.Image = CType(resources.GetObject("tbtnHdzExtract.Image"), System.Drawing.Image)
		Me.tbtnHdzExtract.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzExtract.Name = "tbtnHdzExtract"
		Me.tbtnHdzExtract.Size = New System.Drawing.Size(23, 22)
		Me.tbtnHdzExtract.Text = "^"
		Me.tbtnHdzExtract.ToolTipText = "Extract item(s)"
		'
		'tbtnHdzExtractAs
		'
		Me.tbtnHdzExtractAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tbtnHdzExtractAs.Image = CType(resources.GetObject("tbtnHdzExtractAs.Image"), System.Drawing.Image)
		Me.tbtnHdzExtractAs.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbtnHdzExtractAs.Name = "tbtnHdzExtractAs"
		Me.tbtnHdzExtractAs.Size = New System.Drawing.Size(24, 22)
		Me.tbtnHdzExtractAs.Text = "*^"
		Me.tbtnHdzExtractAs.ToolTipText = "Extract item as..."
		'
		'tpbProgress
		'
		Me.tpbProgress.Name = "tpbProgress"
		Me.tpbProgress.Size = New System.Drawing.Size(100, 22)
		'
		'tlName
		'
		Me.tlName.Name = "tlName"
		Me.tlName.Size = New System.Drawing.Size(136, 22)
		Me.tlName.Text = "Archive name: <empty>"
		Me.tlName.ToolTipText = "Archive file name"
		'
		'_list
		'
		Me._list.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ItemName, Me.PhysName, Me.Attribs, Me.Length, Me.Loc})
		Me._list.Dock = System.Windows.Forms.DockStyle.Fill
		Me._list.GridLines = True
		Me._list.Location = New System.Drawing.Point(0, 25)
		Me._list.Name = "_list"
		Me._list.ShowGroups = False
		Me._list.Size = New System.Drawing.Size(628, 422)
		Me._list.TabIndex = 1
		Me._list.UseCompatibleStateImageBehavior = False
		Me._list.View = System.Windows.Forms.View.Details
		'
		'ItemName
		'
		Me.ItemName.Text = "Item name"
		Me.ItemName.Width = 200
		'
		'PhysName
		'
		Me.PhysName.Text = "Physical path"
		Me.PhysName.Width = 200
		'
		'Attribs
		'
		Me.Attribs.Text = "Attributes"
		'
		'Length
		'
		Me.Length.Text = "Length"
		'
		'Loc
		'
		Me.Loc.Text = "Location"
		'
		'OpenFileDialog1
		'
		Me.OpenFileDialog1.FileName = "OpenFileDialog1"
		'
		'SaveFileDialog1
		'
		Me.SaveFileDialog1.DefaultExt = "hdz"
		Me.SaveFileDialog1.Filter = "HDZ Archive|*.hdz|All files|*.*"
		Me.SaveFileDialog1.Title = "Save HDZ as..."
		'
		'Form1
		'
		Me.AllowDrop = True
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(628, 447)
		Me.Controls.Add(Me._list)
		Me.Controls.Add(Me.ToolStrip1)
		Me.Name = "Form1"
		Me.Text = "HDZ Archiving tool"
		Me.ToolStrip1.ResumeLayout(False)
		Me.ToolStrip1.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
	Friend WithEvents tbtnHdzOpen As System.Windows.Forms.ToolStripButton
	Friend WithEvents tbtnHdzNew As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents tbtnHdzAdd As System.Windows.Forms.ToolStripButton
	Friend WithEvents tbtnHdzClone As System.Windows.Forms.ToolStripButton
	Friend WithEvents tbtnHdzRemove As System.Windows.Forms.ToolStripButton
	Friend WithEvents tbtnHdzRename As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents tbtnHdzSave As System.Windows.Forms.ToolStripButton
	Friend WithEvents tbtnHdzExtract As System.Windows.Forms.ToolStripButton
	Friend WithEvents tpbProgress As System.Windows.Forms.ToolStripProgressBar
	Friend WithEvents tlName As System.Windows.Forms.ToolStripLabel
	Friend WithEvents _list As System.Windows.Forms.ListView
	Friend WithEvents ItemName As System.Windows.Forms.ColumnHeader
	Friend WithEvents Attribs As System.Windows.Forms.ColumnHeader
	Friend WithEvents Length As System.Windows.Forms.ColumnHeader
	Friend WithEvents Loc As System.Windows.Forms.ColumnHeader
	Friend WithEvents PhysName As System.Windows.Forms.ColumnHeader
	Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
	Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
	Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
	Friend WithEvents tbtnHdzSaveAs As System.Windows.Forms.ToolStripButton
	Friend WithEvents tbtnHdzExtractAs As System.Windows.Forms.ToolStripButton

End Class
