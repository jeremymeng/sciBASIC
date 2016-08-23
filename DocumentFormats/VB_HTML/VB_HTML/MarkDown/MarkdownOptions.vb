﻿#Region "Microsoft.VisualBasic::3e5b75f7df0e42a8c182236c4ea4063c, ..\visualbasic_App\DocumentFormats\VB_HTML\VB_HTML\MarkDown\MarkdownOptions.vb"

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

Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' The markdown document generate options.
''' </summary>
Public Class MarkdownOptions

    ''' <summary>
    ''' when true, text link may be empty
    ''' </summary>
    Public Property AllowEmptyLinkText() As Boolean

    ''' <summary>
    ''' when true, hr parser disabled
    ''' </summary>
    Public Property DisableHr() As Boolean

    ''' <summary>
    ''' when true, header parser disabled
    ''' </summary>
    Public Property DisableHeaders() As Boolean

    ''' <summary>
    ''' when true, image parser disabled
    ''' </summary>
    Public Property DisableImages() As Boolean

    ''' <summary>
    ''' when true, quote dont grab next lines
    ''' </summary>
    Public Property QuoteSingleLine() As Boolean

    ''' <summary>
    ''' when true, (most) bare plain URLs are auto-hyperlinked  
    ''' WARNING: this is a significant deviation from the markdown spec
    ''' </summary>
    Public Property AutoHyperlink() As Boolean

    ''' <summary>
    ''' when true, RETURN becomes a literal newline  
    ''' WARNING: this is a significant deviation from the markdown spec
    ''' </summary>
    Public Property AutoNewlines() As Boolean

    ''' <summary>
    ''' use ">" for HTML output, or " />" for XHTML output
    ''' </summary>
    Public Property EmptyElementSuffix() As String

    ''' <summary>
    ''' when false, email addresses will never be auto-linked  
    ''' WARNING: this is a significant deviation from the markdown spec
    ''' </summary>
    Public Property LinkEmails() As Boolean

    ''' <summary>
    ''' when true, bold and italic require non-word characters on either side  
    ''' WARNING: this is a significant deviation from the markdown spec
    ''' </summary>
    Public Property StrictBoldItalic() As Boolean

    ''' <summary>
    ''' when true, asterisks may be used for intraword emphasis
    ''' this does nothing if StrictBoldItalic is false
    ''' </summary>
    Public Property AsteriskIntraWordEmphasis() As Boolean

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
