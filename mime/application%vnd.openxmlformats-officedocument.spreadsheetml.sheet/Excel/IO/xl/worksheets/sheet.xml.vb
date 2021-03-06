﻿#Region "Microsoft.VisualBasic::40a0b1528f5404b4bc404a9da594b3b1, ..\sciBASIC#\mime\application%vnd.openxmlformats-officedocument.spreadsheetml.sheet\Excel\xl\worksheets\sheet.xml.vb"

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

Imports System.Xml.Serialization

Namespace xl.worksheets

    Public Class worksheet
        Public Property dimension As dimension

        Public Property cols As col()
        Public Property sheetData As row()
        Public Property phoneticPr As phoneticPr
        Public Property pageMargins As pageMargins
    End Class

    Public Structure dimension
        <XmlAttribute> Public Property ref As String
    End Structure

    Public Class col
        <XmlAttribute> Public Property min As String
        <XmlAttribute> Public Property max As String
        <XmlAttribute> Public Property width As String
        <XmlAttribute> Public Property style As String
        <XmlAttribute> Public Property bestFit As String
        <XmlAttribute> Public Property customWidth As String
    End Class

    Public Structure row
        <XmlAttribute> Public Property r As String
        <XmlAttribute> Public Property spans As String
        <XmlAttribute> Public Property ht As String
        <XmlAttribute> Public Property customHeight As String
        <XmlAttribute> Public Property customFormat As String
        <XmlElement("c")> Public Property columns As c()
    End Structure

    Public Structure c
        <XmlAttribute> Public Property r As String
        <XmlAttribute> Public Property s As String
        <XmlAttribute> Public Property t As String
        Public Property v As String
    End Structure

    Public Class pageMargins
        <XmlAttribute> Public Property left As Double
        <XmlAttribute> Public Property right As Double
        <XmlAttribute> Public Property top As Double
        <XmlAttribute> Public Property bottom As Double
        <XmlAttribute> Public Property header As Double
        <XmlAttribute> Public Property footer As Double
    End Class
End Namespace
