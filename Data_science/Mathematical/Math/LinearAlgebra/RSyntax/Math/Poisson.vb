﻿#Region "Microsoft.VisualBasic::a45c58314177ea0130d3898ae33781b5, ..\sciBASIC#\Data_science\Mathematical\Math\LinearAlgebra\RSyntax\Math\Poisson.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Mathematical.SyntaxAPI.Vectors
Imports Microsoft.VisualBasic.Mathematical.LinearAlgebra
Imports Microsoft.VisualBasic.Linq

Namespace SyntaxAPI.MathExtension

    ''' <summary>
    ''' Density, distribution function, quantile function and random generation for the Poisson distribution with parameter lambda.
    ''' </summary>
    ''' <remarks></remarks>
    <PackageNamespace("RBase.Math.Poisson", Description:="Density, distribution function, quantile function and random generation for the Poisson distribution with parameter lambda.")>
    Public Module Poisson

        ''' <summary>
        ''' Density, distribution function, quantile function and random generation for the Poisson distribution with parameter lambda.
        ''' </summary>
        ''' <param name="n">number of random values to return.</param>
        ''' <param name="lambda">vector of (non-negative) means.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("rPois")>
        Public Function rPois(n As Integer, lambda As Vector) As Vector
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Density, distribution function, quantile function and random generation for the Poisson distribution with parameter lambda.
        ''' </summary>
        ''' <param name="x">vector of (non-negative integer) quantiles.</param>
        ''' <param name="lambda">vector of (non-negative) means.</param>
        ''' <param name="log">logical; if TRUE, probabilities p are given as log(p).</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Dpois")>
        Public Function Dpois(x As Vector, lambda As Vector, Optional log As Boolean = False) As Vector
            Throw New NotImplementedException
        End Function

        <ExportAPI("qpois")>
        Public Function qpois(p As Vector, lambda As Vector, Optional lowertail As Boolean = True, Optional logp As Boolean = False) As Vector
            Throw New NotImplementedException
        End Function
    End Module

    ''' <summary>
    ''' The Normal Distribution
    ''' Density, distribution function, quantile function and random generation for the normal distribution with mean equal to mean and standard deviation equal to sd.
    ''' </summary>
    ''' <remarks></remarks>
    <NamespaceRenamed("RBase.Math.Normal", Description:="Density, distribution function, quantile function and random generation for the normal distribution with mean equal to mean and standard deviation equal to sd.")>
    Public Module Normal

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p">vector of probabilities.</param>
        ''' <param name="mean">vector of means.</param>
        ''' <param name="sd">vector of standard deviations.</param>
        ''' <param name="lowertail">logical; if TRUE (default), probabilities are P[X ≤ x] otherwise, P[X > x].</param>
        ''' <param name="logp">logical; if TRUE, probabilities p are given as log(p).</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("qnorm")>
        Public Function qnorm(p As Vector, Optional mean As Vector = NULL, Optional sd As Integer = 1, Optional lowertail As Boolean = True, Optional logp As Boolean = False) As Vector
            If Missing(mean) Then
                mean = Vector.Zero
            End If

            Throw New NotImplementedException
        End Function

        <ExportAPI("pnorm")>
        Public Function pnorm(x As Vector) As Vector
            Throw New NotImplementedException
        End Function

        <ExportAPI("dnorm")>
        Public Function dnorm(x As Vector) As Vector
            Throw New NotImplementedException
        End Function

        <ExportAPI("rnorm")>
        Public Function rnorm(x As Vector, m As Double, sd As Double) As Vector
            Return New Vector(x.Select(Function(n) Bootstraping.ProbabilityDensity(n, m, sd)))
        End Function

        ''' <summary>
        ''' Returns a vector with specific parameters normal distribution of length n elements.
        ''' Distribution data was generated by the n length random seeds as the normal distribution 
        ''' inputs.
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="m"></param>
        ''' <param name="sd"></param>
        ''' <returns></returns>
        Public Function rnorm(n As Integer, m As Double, sd As Double) As Vector
            Dim rand As New Random
            Return New Vector(
                n.Sequence.Select(
                Function(x) rand.NextGaussian(m, sd)))  ' 不清楚在R之中是否是这样子来实现的
        End Function
    End Module
End Namespace
