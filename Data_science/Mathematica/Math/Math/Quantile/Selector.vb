﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace Quantile

    ''' <summary>
    ''' String expression for select the sequence members by its numeric values, 
    ''' selector expression provider for CLI programming.
    ''' (对输入的目标序列进行数量上面的选择的表达式)
    ''' </summary>
    ''' <remarks>
    ''' |name             |syntax          |description                                                                                              |
    ''' |-----------------|----------------|---------------------------------------------------------------------------------------------------------|
    ''' |quantile selector|``quantile:p%`` |``quantile:75%`` means selector object if its value is greater than quantile 75%.                        |
    ''' |quartile selector|``Q1|2|3``      |quartile selector only have 3 enumerate values: ``Q1``, ``Q2``, ``Q3``, all of the values are UPPER_CASE.|
    ''' |rank selector    |``desc:n|asc:n``|selector using object its rank order, desc:n for top n and asc:n for first n.                            |
    ''' </remarks>
    Public Module Selector

        Public Structure Provider(Of T)

            Dim source As IEnumerable(Of T)
            ''' <summary>
            ''' 描述了如何从具体的对象之中解析出数值信息
            ''' </summary>
            Dim getValue As Func(Of T, Double)

            Public Function CreateArray() As IEnumerable(Of (x#, obj As T))
                Dim readData = getValue
                Return source.Select(Function(x) (readData(x), x))
            End Function
        End Structure

        <Extension>
        Public Function ApplySelector(Of T)(source As IEnumerable(Of T), getValue As Func(Of T, Double), exp$) As IEnumerable(Of T)
            Return New Provider(Of T) With {
                .getValue = getValue,
                .source = source
            }.ApplySelector(exp)
        End Function

        <Extension>
        Public Function ApplySelector(Of T)(source As Provider(Of T), exp$) As IEnumerable(Of T)
            If InStr(exp, "quantile:", CompareMethod.Text) > 0 Then
                Dim q#

                With exp.Split(":"c).Last.Trim
                    If .IsPattern("\d+(\.\d+)?[%]") Then
                        q = Val(.ref) / 100
                    Else
                        q = Val(.ref)
                    End If
                End With

                Return source.SelectByQuantile(q)
            ElseIf exp.IsPattern("\s*Q[123]\s*") Then
                Dim level As Quartile.Levels = [Enum].Parse(GetType(Quartile.Levels), exp.Trim)
                Return source.SelectByQuartile(name:=level)
            ElseIf exp.IsPattern("((desc)|(asc))[:]\d+") Then
                Dim arg = exp.GetTagValue(":", trim:=True)
                Return source.SelectByRankAsc(Val(arg.Value), arg.Name.TextEquals("desc"))
            Else
                Throw New NotImplementedException(exp)
            End If
        End Function

        ''' <summary>
        ''' 使用数量百分比来进行选择
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="q#"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SelectByQuantile(Of T)(source As Provider(Of T), q#) As IEnumerable(Of T)
            Dim array = source.CreateArray.ToArray
            Dim quantile = array.Select(Function(o) o.x).GKQuantile
            Dim threshold# = quantile.Query(q)

            Call $"quantile {q * 100}% => {threshold}".__INFO_ECHO

            Return array _
                .Where(Function(o) o.x >= threshold) _
                .Select(Function(x) x.obj)
        End Function

        <Extension>
        Public Function SelectByQuartile(Of T)(source As Provider(Of T), name As Quartile.Levels) As IEnumerable(Of T)
            Dim array = source.CreateArray.ToArray
            Dim quartile = array.Select(Function(o) o.x).Quartile
            Dim q#

            Select Case name
                Case Levels.Q1 : q# = quartile.Q1
                Case Levels.Q2 : q# = quartile.Q2
                Case Levels.Q3 : q# = quartile.Q3
                Case Else
                    Throw New NotSupportedException("???" & name.ToString)
            End Select

            Call $"quartile {name.ToString} => {q}".__INFO_ECHO

            Return array _
                .Where(Function(o) o.x >= q#) _
                .Select(Function(x) x.obj)
        End Function

        <Extension>
        Public Function SelectByRankAsc(Of T)(source As Provider(Of T), n%, desc As Boolean) As IEnumerable(Of T)
            Return source _
                .CreateArray _
                .Sort(Function(o) o.x, desc) _
                .Take(n) _
                .Select(Function(x) x.obj)
        End Function
    End Module
End Namespace