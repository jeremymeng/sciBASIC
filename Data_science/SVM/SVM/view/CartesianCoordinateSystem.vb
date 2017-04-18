﻿Imports System.Drawing
Imports Microsoft.VisualBasic.DataMining.SVM.Model
Imports Microsoft.VisualBasic.Imaging
Imports Canvas = System.Drawing.Graphics

''' <summary>
''' The view used for drawing the coordinate system.
''' 
''' @author Ralf Wondratschek
''' </summary>
Public Class CartesianCoordinateSystem

    Private Const STROKE_WIDTH As Integer = 4
    Private Const CIRCLE_RADIUS As Integer = 12

    Private mState As State
    Private mLineBuilder As LineBuilder

    Public Property Line As Line
        Get
            If mState.Line IsNot Nothing Then
                Return mState.Line
            ElseIf mLineBuilder IsNot Nothing Then
                Return mLineBuilder.Build()
            End If
            Return Nothing
        End Get
        Set(value As Line)
            mState.Line = value
            mLineBuilder = Nothing
        End Set
    End Property

    Public ReadOnly Property Points As List(Of LabeledPoint)
        Get
            Return mState.Points
        End Get
    End Property

    Public ReadOnly Property State As State
        Get
            Return New State(Me, mState.Points, mState.Line)
        End Get
    End Property

    Sub New(Optional points As IEnumerable(Of LabeledPoint) = Nothing)
        mState = New State(Me)

        If Not points Is Nothing Then
            For Each x In points
                Call mState.AddPoint(x)
            Next
        End If
    End Sub

    ''' <summary>
    ''' 可视化SVM结果
    ''' </summary>
    ''' <param name="canvas"></param>
    ''' <param name="width!"></param>
    ''' <param name="height!"></param>
    Sub Draw(canvas As Canvas, Optional width! = 1440, Optional height! = 900)
        Dim pen As Pen = Pens.Black

        canvas.DrawLine(pen, 0, 0, 0, height)
        canvas.DrawLine(pen, 0, height, width, height)

        Dim textLineWidth As Integer = 10
        Dim textPadding As Integer = textLineWidth + 6
        Dim f As New Font("Microsoft YaHei", 12)

        For Each text As String In {"0.25", "0.5", "0.75", "1.0"}
            With canvas.MeasureString(text, f)
                Dim y! = height * (1 - Val(text))
                Dim x! = width * Val(text)

                canvas.DrawString(text, f, Brushes.Black, textPadding, y - .Height() \ 2)         ' Y
                canvas.DrawString(text, f, Brushes.Black, x - .Width() \ 2, height - .Height - textPadding / 2) ' X
                canvas.DrawLine(pen, 0, y, textPadding, y)
                canvas.DrawLine(pen, x, height, x, height - textPadding)
            End With
        Next

        For Each p As LabeledPoint In mState.Points
            If p.ColorClass = ColorClass.RED Then
                canvas.DrawCircle(Pens.Red, CSng(p.X1) * width, CSng(1 - p.X2) * height, CIRCLE_RADIUS)
            Else
                canvas.DrawCircle(Pens.Blue, CSng(p.X1) * width, CSng(1 - p.X2) * height, CIRCLE_RADIUS)
            End If
        Next

        Dim lineText As Line = mState.Line
        If lineText Is Nothing AndAlso mLineBuilder IsNot Nothing Then lineText = mLineBuilder.Build()

        If lineText IsNot Nothing Then
            Dim text$ = "y = " & Math.Round(lineText.Increase * 100) / 100.0R & " * x + " & Math.Round(lineText.Offset * 100) / 100.0R
            With canvas.MeasureString(text, f)
                canvas.DrawString(text, f, Brushes.Black, width - textPadding - .Width(), textPadding + .Height())
            End With
        End If

        If mLineBuilder IsNot Nothing Then
            canvas.DrawLine(pen, mLineBuilder.StartX * width, (1 - mLineBuilder.StartY) * height, mLineBuilder.EndX * width, (1 - mLineBuilder.EndY) * height)
        ElseIf mState.Line IsNot Nothing Then
            Dim p1 As New Point(0, CSng(1 - mState.Line.CalcY(0)) * height)
            Dim p2 As New Point(width, CSng(1 - mState.Line.CalcY(1)) * height)
            canvas.DrawLine(pen, p1, p2)
        End If
    End Sub

    Public Sub AddPoint(point As LabeledPoint)
        Call mState.AddPoint(point)
    End Sub

    Public Sub ClearPoints()
        Call mState.ClearPoints()
    End Sub

    Public Sub RemovePoint(point As LabeledPoint)
        Call mState.RemovePoint(point)
    End Sub
End Class
