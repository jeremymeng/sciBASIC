﻿#Region "Microsoft.VisualBasic::870a3f9475cd1dd8bc184e3eabf7d2e8, ..\sciBASIC#\Data_science\Mathematical\ODE\ODESolvers\Gill.vb"

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

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO

Partial Module ODESolver

    ''' <summary>
    ''' 把根号2算出来，不在循环体内反复执行根号2，减少计算负担
    ''' </summary>
    ReadOnly sqr2# = Math.Sqrt(2.0)

    Public Sub Gill(ByRef ode As ODE, N As Integer, t0 As Double, tt As Double)
        Dim df As df = ode.df

        Call ode.Allocate(N, t0, tt)

        Dim y As Double = ode.y0, t As Double = t0    ' 积分初值
        Dim k1 As Double, k2 As Double, k3 As Double, k4 As Double
        Dim h# = (tt - t0) / N   ' 步长

        ' 因为已经设置了y0了，所以在这里都是从1开始的
        For i% = 1 To N
            k1 = df(y, t)
            k2 = df(y + 0.5 * h * k1, t + 0.5 * h)
            k3 = df(y + 0.5 * (sqr2 - 1.0) * h * k1 + 0.5 * (1 - sqr2) * h * k2, t + 0.5 * h)
            k4 = df(y - sqr2 * 0.5 * h * k2 + (1 + sqr2) * h * k3, t + h)

            y = y + (k1 + k2 * (2 - sqr2) + k3 * (2 + sqr2) + k4) * h / 6.0
            t = t + h
            ode.y(i) = y
            ode.x(i) = t
        Next
    End Sub
End Module
