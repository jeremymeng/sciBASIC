﻿#Region "Microsoft.VisualBasic::00ba1cbad729a3e36eb485042bfc7cd9, ..\visualbasic_App\Cli Tools\LicenseMgr\LicenseMgr\MainWindow.xaml.vb"

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

Imports FirstFloor.ModernUI.Windows.Controls
'Imports Microsoft.VisualBasic.Windows.Forms

''' <summary>
''' Interaction logic for MainWindow.xaml
''' </summary>
Partial Public Class MainWindow
    Inherits ModernWindow
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub ModernWindow_Initialized(sender As Object, e As EventArgs)
        '  If VistaSecurity.IsAdmin() Then
        '  Me.Title += " (Elevated)"
        '  End If
    End Sub
End Class
