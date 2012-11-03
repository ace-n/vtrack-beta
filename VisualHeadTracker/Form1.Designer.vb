<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class pbx2
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
        Me.worker = New System.ComponentModel.BackgroundWorker()
        Me.btnDraw = New System.Windows.Forms.Button()
        Me.cPnl = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.faceRectShape = New Microsoft.VisualBasic.PowerPacks.RectangleShape()
        Me.ShapeContainer2 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer()
        Me.xLine = New Microsoft.VisualBasic.PowerPacks.LineShape()
        Me.SuspendLayout()
        '
        'worker
        '
        Me.worker.WorkerReportsProgress = True
        '
        'btnDraw
        '
        Me.btnDraw.Location = New System.Drawing.Point(0, -1)
        Me.btnDraw.Name = "btnDraw"
        Me.btnDraw.Size = New System.Drawing.Size(75, 23)
        Me.btnDraw.TabIndex = 5
        Me.btnDraw.Text = "Draw"
        Me.btnDraw.UseVisualStyleBackColor = True
        '
        'cPnl
        '
        Me.cPnl.Location = New System.Drawing.Point(0, 28)
        Me.cPnl.Name = "cPnl"
        Me.cPnl.Size = New System.Drawing.Size(1199, 537)
        Me.cPnl.TabIndex = 6
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(82, -1)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 7
        Me.Button1.Text = "Calibrate"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'faceRectShape
        '
        Me.faceRectShape.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque
        Me.faceRectShape.BorderColor = System.Drawing.Color.Lime
        Me.faceRectShape.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dash
        Me.faceRectShape.BorderWidth = 2
        Me.faceRectShape.Location = New System.Drawing.Point(2, 566)
        Me.faceRectShape.Name = "faceRectShape"
        Me.faceRectShape.Size = New System.Drawing.Size(107, 104)
        '
        'ShapeContainer2
        '
        Me.ShapeContainer2.Location = New System.Drawing.Point(0, 0)
        Me.ShapeContainer2.Margin = New System.Windows.Forms.Padding(0)
        Me.ShapeContainer2.Name = "ShapeContainer2"
        Me.ShapeContainer2.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.xLine, Me.faceRectShape})
        Me.ShapeContainer2.Size = New System.Drawing.Size(1202, 671)
        Me.ShapeContainer2.TabIndex = 8
        Me.ShapeContainer2.TabStop = False
        '
        'xLine
        '
        Me.xLine.Name = "xLine"
        Me.xLine.X1 = 279
        Me.xLine.X2 = 279
        Me.xLine.Y1 = 565
        Me.xLine.Y2 = 670
        '
        'pbx2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1202, 671)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.cPnl)
        Me.Controls.Add(Me.btnDraw)
        Me.Controls.Add(Me.ShapeContainer2)
        Me.Name = "pbx2"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents worker As System.ComponentModel.BackgroundWorker
    Friend WithEvents btnDraw As System.Windows.Forms.Button
    Friend WithEvents cPnl As System.Windows.Forms.Panel
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents faceRectShape As Microsoft.VisualBasic.PowerPacks.RectangleShape
    Friend WithEvents ShapeContainer2 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
    Friend WithEvents xLine As Microsoft.VisualBasic.PowerPacks.LineShape

End Class
