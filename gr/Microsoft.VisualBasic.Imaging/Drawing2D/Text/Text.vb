﻿#Region "Microsoft.VisualBasic::5cd83470a4eac3ff800ea044f03a2416, ..\sciBASIC#\gr\Microsoft.VisualBasic.Imaging\Drawing2D\Text\Text.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML

Namespace Drawing2D.Vector.Text

    Public Class Text : Inherits VectorObject

        Public Property [Strings] As List(Of [String])

        Sub New(text As IEnumerable(Of [String]), rect As Rectangle)
            Call MyBase.New(rect)
            Strings = New List(Of [String])(text)
        End Sub

        Sub New(html As String, rect As Rectangle)
            Call Me.New(TextAPI.GetStrings(html), rect)
        End Sub

        ''' <summary>
        ''' Measures the specified string when drawn with the specified System.Drawing.Font.(最大的Rectangle)
        ''' </summary>
        ''' <returns>This method returns a System.Drawing.SizeF structure that represents the size,
        ''' in the units specified by the System.Drawing.Graphics.PageUnit property, of the
        ''' string specified by the text parameter as drawn with the font parameter.</returns>
        Public Function MeasureString(gdi As GDIPlusDeviceHandle) As SizeF
            Dim szs As SizeF() = Strings.ToArray(Function(x) x.MeasureString(gdi))
            Dim width As Integer = szs.Sum(Function(x) x.Width)
            Dim maxh As Integer = szs.Max(Function(x) x.Height)
            Return New SizeF(width, maxh)
        End Function

        Public Overrides Sub Draw(gdi As GDIPlusDeviceHandle, loci As Rectangle)
            Call Draw(gdi, loci.Location)
        End Sub

        Public Overloads Sub Draw(gdi As GDIPlusDeviceHandle, loci As Point)
            Call Strings.ToArray.DrawStrng(loci, gdi)
        End Sub

        Public Overrides Function ToString() As String
            Return String.Join("", Strings.ToArray(Function(x) x.Text))
        End Function
    End Class

    ''' <summary>
    ''' 基于HTML语法的字符串的绘制描述信息的解析
    ''' </summary>
    Public Module TextAPI

        ' html -->  <font face="Microsoft YaHei" size="1.5"><strong>text</strong><b><i>value</i></b></font> 
        ' 解析上述的表达式会产生一个栈，根据html标记来赋值字符串的gdi+属性

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="html">这里只是一个很小的html的片段，仅仅用来描述所需要进行绘制的字符串的gdi+属性</param>
        ''' <returns></returns>
        Public Function GetStrings(html As String,
                                   Optional defaulFont As Font = Nothing,
                                   Optional defaultColor As Color = Nothing) As [String]()
            Dim texts As TextString() = TryParse(html, defaulFont)

            If defaultColor.IsEmpty Then
                defaultColor = Color.Black
            End If

            Dim parserHelper As New __parserHelper With {
                .defaultColor = defaultColor
            }
            Dim models As [String]() =
                texts.ToArray(AddressOf parserHelper.GetString)
            Return models
        End Function

        Private Structure __parserHelper
            Dim defaultColor As Color

            Public Function GetString(x As TextString) As [String]
                Return New [String](x) With {
                    .Pen = New SolidBrush(defaultColor)
                }
            End Function
        End Structure

        Public Function GetText(html As String,
                                Optional defaulFont As Font = Nothing,
                                Optional defaultColor As Color = Nothing) As Text
            Return New Text(GetStrings(html, defaulFont, defaultColor), Nothing)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="texts">需要进行绘制的文本的集合</param>
        ''' <param name="loc">最开始的左上角的位置</param>
        ''' <param name="gdi"></param>
        <Extension>
        Public Sub DrawStrng(texts As [String](), loc As Point, gdi As GDIPlusDeviceHandle)
            Dim szs As SizeF() = texts.ToArray(Function(x) x.MeasureString(gdi))
            Dim maxH As Integer = szs.Select(Function(x) x.Height).Max
            Dim lowY As Integer = loc.Y + maxH
            Dim lx As Integer = loc.X

            For Each s As SeqValue(Of [String], SizeF) In texts.SeqIterator(Of SizeF)(szs)
                Dim y As Integer = lowY - s.Follow.Height
                Dim pos As New Point(lx.Move(s.Follow.Width), y)
                Dim rect As New Rectangle(pos, New Size(s.Follow.ToSize))

                Call New [String](s.obj, rect).Draw(gdi)
            Next
        End Sub
    End Module
End Namespace