﻿Imports Microsoft.Expression.Encoder
Imports Microsoft.Expression.Encoder.Devices
Imports Microsoft.Expression.Encoder.Live
Imports System.Runtime.InteropServices
Imports System.IO

Public Class pbx2

    Declare Function Sleep Lib "kernel32" (ByVal dwMilliseconds As Integer) As Integer

    ' Starting point
    Dim StartPt As New Point

    ' List of video devices
    Dim VDeviceList As New List(Of EncoderDevice)

    ' Preview window
    Dim PWin As PreviewWindow

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' Code (largely) copy-pasted from: http://www.codeproject.com/Articles/202464/How-to-use-a-WebCam-in-C-with-the-NET-Framework-4

        ' List devices
        For Each Device In EncoderDevices.FindDevices(EncoderDeviceType.Video)
            'lbSources.Items.Add(Device.Name)
            VDeviceList.Add(Device)
        Next

        ' Capture an image
        Dim VJob As New LiveJob
        Dim VDevSrc As LiveDeviceSource = VJob.AddDeviceSource(VDeviceList.Item(1), EncoderDevices.FindDevices(EncoderDeviceType.Audio).FirstOrDefault) ' We don't care about audio
        VJob.ActivateSource(VDevSrc)

        ' Print the image to the main form
        PWin = New PreviewWindow(New HandleRef(cPnl, cPnl.Handle))
        VDevSrc.PreviewWindow = PWin

    End Sub

    Dim PastMousePtList As New List(Of Point)
    Dim ImgBytes As Byte()
    Dim LipAvg As New Point
    Dim IsCalibrated As Boolean = False
    Private Sub Redraw() Handles worker.ProgressChanged

        ' This is the definition of Kludge

        ' Capture from screen
        Dim Bmp As New Bitmap(cPnl.Width, cPnl.Height)
        Dim Gr As Graphics = Graphics.FromImage(Bmp)
        Gr.CopyFromScreen(cPnl.Location + Me.Location + New Point(9, 42), New Point(), cPnl.Size)

        ' --- PERFORM ANALYTICS ---

        ' Decompile (from http://social.msdn.microsoft.com/Forums/en-US/vbgeneral/thread/72bff6ea-b7ca-476d-8f6a-faa28b1b4a8c/)
        Dim ImgStream As New MemoryStream
        Bmp.Save(ImgStream, System.Drawing.Imaging.ImageFormat.Bmp) ' 32 bit format
        ImgBytes = ImgStream.GetBuffer()

        ' Averaging
        Dim XTotal As Long = 0
        Dim YTotal As Long = 0
        Dim Count As Integer = 0

        ' Find average
        Dim FaceRect As Rectangle = FindFacialRect(ImgBytes)
        Dim XYAvg As Point = FindAverage(ImgBytes, FaceRect, 5)

        Dim Size As Integer = 120
        If True Then
            XYAvg = FindAverage(ImgBytes, New Rectangle(XYAvg - New Point(Size / 2, Size / 2), New Size(Size, Size)), 3)
        End If

        ' Draw bounding box
        'For i = -5 To 5
        '    SetPixel(ImgBytes, XTotal + i, YTotal, 255, 0, 0)
        '    SetPixel(ImgBytes, XTotal, YTotal + i, 255, 0, 0)
        'Next

        ' Average the XYAvg with the past XYAvg's to reduce random error
        Dim PastPtCount As Integer = 30
        PastMousePtList.Add(XYAvg)
        If PastMousePtList.Count >= 3 Then

            ' Remove final point of PastMousePtList (PMPtList is a first-in last-out setup)
            While PastMousePtList.Count > PastPtCount
                PastMousePtList.RemoveAt(PastMousePtList.Count - 1)
            End While

            ' Use historical data
            XTotal = 0
            YTotal = 0
            For Each P As Point In PastMousePtList
                XTotal += P.X
                YTotal += P.Y
            Next

            XTotal = XTotal / PastMousePtList.Count
            YTotal = YTotal / PastMousePtList.Count

        Else

            ' No historical data available, so use current data
            XTotal = XYAvg.X
            YTotal = XYAvg.Y

        End If

        ' Skew X/Y totals to the calibration rectangle
        If False And IsCalibrated Then ' Calibration is broken
            XTotal = ((XTotal - CalibRect.Left) / Math.Abs(CalibRect.Width)) * Screen.PrimaryScreen.Bounds.Width
            YTotal = YTotal - CalibRect.Top
        Else
            XTotal = (XTotal - FaceRect.Left) ' / FaceRect.Width * Screen.PrimaryScreen.Bounds.Width
            YTotal = YTotal - FaceRect.Top
        End If

        ' Update lip average
        LipAvg = New Point(XTotal, YTotal)

        ' Recompile image
        'Bmp = Drawing.Image.FromStream(New MemoryStream(ImgBytes))

        ' Move cursor
        Dim Bds As Size = Screen.PrimaryScreen.Bounds.Size

        '   Smoothing
        Dim MouseSpeed = 5

        Dim XScl As Integer = 2
        Dim TargetPt As Point = New Point((XTotal / FaceRect.Width) * Bds.Width, (YTotal / FaceRect.Height) * Bds.Height)

        Dim DirVec As Point = (TargetPt - Cursor.Position)
        Dim DirLen As Double = Math.Max(Math.Sqrt(DirVec.X ^ 2 + DirVec.Y ^ 2), 1)

        ' Normalize cursor movement vector
        Dim DirNorm As New PointF(DirVec.X / DirLen, DirVec.Y / DirLen)

        ' Rescale normalized vector
        DirVec.X = Math.Round(DirNorm.X * Math.Sqrt(DirLen ^ 2 / MouseSpeed))
        DirVec.Y = Math.Round(DirNorm.Y * Math.Sqrt(DirLen ^ 2 / MouseSpeed))

        ' Move cursor
        Cursor.Position = Cursor.Position + DirVec

        ' Draw facial rectangle
        faceRectShape.Location = New Point(FaceRect.Location.X, faceRectShape.Location.Y)
        faceRectShape.Size = New Size(FaceRect.Size.Width, faceRectShape.Size.Height)
        xLine.X1 = XTotal + FaceRect.Location.X
        xLine.X2 = XTotal + FaceRect.Location.X

        ' Update picture box
        'pbox2.Image = Bmp

    End Sub

    ' Find facial rectangle
    Public Function FindFacialRect(ByRef ImgBytes As Byte()) As Rectangle

        Dim Threshold As Integer = 60
        Dim FaceRect As New Rectangle(New Point, cPnl.Size)

        ' Get left
        FaceRect.Location = New Point(FFRGetEdge(ImgBytes, 25, cPnl.Width - 6, 5), 4)

        ' Get right
        FaceRect.Size = New Size(FFRGetEdge(ImgBytes, cPnl.Width - 26, 5, 5) - FaceRect.Left, FaceRect.Size.Height - 10)

        ' Return
        Return FaceRect

    End Function
    Public Function FFRGetEdge(ByRef ImgBytes As Byte(), ByVal Start As Integer, ByVal EndN As Integer, ByVal StepSize As Integer) As Integer

        Dim Threshold As Integer = 115 ' # of valid pixels that must occur in a line for it to be marked

        For x = Start To EndN Step Math.Sign(EndN - Start) * StepSize

            ' Scan vertically
            Dim CurrentValidPxls As Integer = 0
            For y = 0 To cPnl.Height - 1

                Dim HSV As Integer() = RGB2HSV(GetPixel(ImgBytes, x, y))

                ' Detect face
                If ((HSV.GetValue(0) > 10 AndAlso HSV.GetValue(0) < 30) OrElse HSV.GetValue(0) > 330) AndAlso HSV.GetValue(1) > 25 AndAlso HSV.GetValue(2) > 30 Then
                    CurrentValidPxls += 1
                End If

                ' If face detected, report it as such
                If CurrentValidPxls > Threshold Then
                    Return x
                End If

            Next

        Next

        ' Return starting value if nothing was found
        Return Start

    End Function

    ' Find average of valid pixels
    Public Function FindAverage(ByRef ImgBytes As Byte(), ByVal Rect As Rectangle, ByVal StepInt As Integer)

        ' Values
        Dim XTotal As Long = 0
        Dim YTotal As Long = 0
        Dim Count As Integer = 1

        ' Return null if rect is invalid
        If Rect.Top < 0 OrElse Rect.Left < 0 Then
            Return New Point
        End If

        ' Image parsing
        For y As Integer = Rect.Top To Rect.Bottom - 1 Step StepInt
            For x As Integer = Rect.Left To Rect.Right - 1 Step StepInt

                ' Find lips (current tracking point)
                Dim HSV As Integer() = RGB2HSV(GetPixel(ImgBytes, x, y))

                ' Include valid points in the average
                'If ((HSV.GetValue(0) > 10 AndAlso HSV.GetValue(0) < 30) OrElse HSV.GetValue(0) > 330) AndAlso HSV.GetValue(1) > 25 AndAlso HSV.GetValue(2) > 30 Then
                If (HSV.GetValue(0) > 355) AndAlso HSV.GetValue(1) > 50 AndAlso HSV.GetValue(2) > 50 Then ' Lips

                    'SetPixel(ImgBytes, x, y, 0, 255, 0)

                    ' Determine weight of pixel
                    Dim PixWeight As Integer = (HSV.GetValue(2) - 50) ^ 2

                    ' Tally pixel
                    XTotal += x * PixWeight
                    YTotal += y * PixWeight
                    Count += PixWeight

                End If

            Next
        Next

        ' Compute/return average
        XTotal = XTotal / Count
        YTotal = YTotal / Count

        ' Return
        Return New Point(XTotal, YTotal)

    End Function

    ' Get-set pixel
    Public Function GetPixel(ByRef ImgBytes As Byte(), ByVal X As Integer, ByVal Y As Integer) As Integer()

        Dim Offset As Integer = (X * 4) + (Y * (cPnl.Width * 4)) + 3

        ' Exception handler
        If Offset >= ImgBytes.Length Then
            Return {0, 0, 0}
        End If

        Dim R As Integer = CInt(ImgBytes.GetValue(Offset + 1))
        Dim G As Integer = CInt(ImgBytes.GetValue(Offset))
        Dim B As Integer = CInt(ImgBytes.GetValue(Offset - 1))

        Return {R, G, B}

    End Function
    Public Sub SetPixel(ByRef ImgBytes As Byte(), ByVal X As Integer, ByVal Y As Integer, ByVal R As Integer, ByVal G As Integer, ByVal B As Integer)

        Dim Offset As Integer = (X * 4) + (Y * (cPnl.Width * 4)) + 3

        ' Exception handler
        If Offset >= ImgBytes.Length Then
            Return
        End If

        ImgBytes.SetValue(CByte(R), Offset + 1)
        ImgBytes.SetValue(CByte(G), Offset)
        ImgBytes.SetValue(CByte(B), Offset - 1)

    End Sub

    ' Redraw triggering worker
    Private Sub WorkerWork() Handles worker.DoWork

        Dim I As Integer = 0
        While True

            I = Math.Abs(I - 1)
            worker.ReportProgress(I + 1)

            Sleep(300)

        End While

    End Sub

    ' Recycled from Blackjack
    ' See here: http://www.cs.rit.edu/~ncs/color/t_convert.html
    Private Function RGB2HSV(ByVal RGB As Integer()) As Integer()

        Dim R As Integer = RGB.GetValue(0)
        Dim G As Integer = RGB.GetValue(1)
        Dim B As Integer = RGB.GetValue(2)

        Dim H, S, V As Double

        Dim max As Integer = Math.Max(R, Math.Max(G, B))
        Dim min As Integer = Math.Min(R, Math.Min(G, B))
        V = max

        If min = max Then
            Return {CInt(H), CInt(max / 2.55), 0}
        End If

        Dim delta As Integer = max - min

        If max <> 0 Then
            S = 100 * delta / max
        Else
            S = 0
            H = -1
        End If

        If R = max Then
            H = ((G - B) / delta) * 60
        ElseIf G = max Then
            H = (2 + (B - R) / delta) * 60
        Else
            H = (4 + (R - G) / delta) * 60
        End If

        If H < 0 Then
            H += 360
        End If

        Return {CInt(H), CInt(S), CInt(V)}

    End Function

    Private Sub btnDraw_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDraw.Click

        ' Calibrate
        'Calib1()

        worker.RunWorkerAsync()

    End Sub

    Public CalibRect As Rectangle
    Private Sub Calib1()

        IsCalibrated = False

        ' -- Left/top --
        ' Move dialog
        CalibDlg.GoToLoc = Screen.PrimaryScreen.Bounds.Location
        CalibDlg.ShowDialog()

        ' Take snapshot
        Redraw()
        CalibRect.Location = LipAvg

        ' -- Right/Bottom --
        ' Move dialog
        CalibDlg.GoToLoc = Screen.PrimaryScreen.Bounds.Size - CalibDlg.Size
        CalibDlg.Hide()
        CalibDlg.ShowDialog()

        ' Take snapshot
        Redraw()
        CalibRect.Size = LipAvg - CalibRect.Location

        ' Trigger draw procedure
        Me.Show()
        Sleep(500)
        'IsCalibrated = True
        Sleep(100)
        worker.RunWorkerAsync()

    End Sub

End Class

Public Class RecursiveBlobDect

    Public Sub RecursionHandler(ByRef BinaryImg As Boolean()(), ByVal Start As Point)

        ' Set up recursion


        ' Start recursing
        'Recursor()

    End Sub

    Public Sub Recursor(ByVal Pt As Point, ByVal VertDir As Integer)


        ' Recurse into nearby points


    End Sub


End Class