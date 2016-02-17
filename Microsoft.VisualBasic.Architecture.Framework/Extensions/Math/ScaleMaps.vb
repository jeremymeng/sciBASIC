﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

<PackageNamespace("ScaleMaps",
                  Category:=APICategories.UtilityTools,
                  Publisher:="xie.guigang@live.com")>
Public Module ScaleMaps

    <Extension> Public Function TrimRanges(Dbl As Double(), min As Double, max As Double) As Double()
        If Dbl.IsNullOrEmpty Then
            Return New Double() {}
        End If

        For i As Integer = 0 To Dbl.Length - 1
            Dim n As Double = Dbl(i)

            If n < min Then
                n = min
            ElseIf n > max
                n = max
            End If

            Dbl(i) = n
        Next

        Return Dbl
    End Function

    <ExportAPI("Ranks.Mapping")>
    <Extension> Public Function GenerateMapping(Of T As sIdEnumerable)(data As IEnumerable(Of T),
                                                                       getSample As Func(Of T, Double),
                                                                       Optional Level As Integer = 10) As Dictionary(Of String, Integer)
        Dim samples As Double() = data.ToArray(Function(x) getSample(x))
        Dim levels As Integer() = samples.GenerateMapping(Level)
        Dim hash = data.ToArray(Function(x, i) New KeyValuePair(Of String, Integer)(x.Identifier, levels(i)))
        Return hash.ToDictionary
    End Function

    ''' <summary>
    ''' 如果每一个数值之间都是相同的大小，则返回原始数据，因为最大值与最小值的差为0，无法进行映射的创建（会出现除0的错误）
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks>为了要保持顺序，不能够使用并行拓展</remarks>
    ''' 
    <ExportAPI("Ranks.Mapping")>
    <Extension> Public Function GenerateMapping(data As IEnumerable(Of Double), Optional Level As Integer = 10) As Integer()
        Dim MinValue As Double = data.Min
        Dim MaxValue As Double = data.Max
        Dim d As Double = (MaxValue - MinValue) / Level

        If d = 0R Then
            Return (From n As Double In data Select 1).ToArray  '所有的值都是一样的，则都是同等级的
        End If

        Dim chunkBuf As Integer() = New Integer(data.Count - 1) {}
        Dim i As Integer = 0

        Level -= 1

        For Each x As Double In data
            Dim lv As Integer = CInt((x - MinValue) / d + 1)
            If lv > Level Then
                lv = Level
            End If
            chunkBuf(i) = lv
            i += 1
        Next

        Return chunkBuf
    End Function

    <ExportAPI("Ranks.Log2")>
    <Extension> Public Function Log2Ranks(data As Generic.IEnumerable(Of Double), Optional Level As Integer = 100) As Long()
        Dim priMaps As Double() = data.GenerateMapping(Level).ToArray(Function(d) d / Level)
        Dim log2Data As Double() = Level.ToArray(Function(n) Math.Log(n + 1, 2))
        ' 将等级映射到log2的y轴上面
        Dim lgMax As Double = log2Data.Max
        priMaps = priMaps.ToArray(Function(d) lgMax * d) ' lgMax * %
        Dim Maps = priMaps.ToArray(Function(d) CLng(2 ^ d))  ' 找到x坐标的位置 0-100
        Return Maps
    End Function

    'Public Function SquareMaps(data As Generic.IEnumerable(Of Double), Optional level As Integer = 100) As Integer()
    '    Dim priMaps As Double() = data.GenerateMapping(level)  ' 得到y轴的数据

    'End Function

    <ExportAPI("Ranks.Log2")>
    <Extension> Public Function Log2Ranks(data As Generic.IEnumerable(Of Integer), Optional Level As Integer = 10) As Long()
        Return data.ToArray(Function(d) CDbl(d)).Log2Ranks
    End Function

    <ExportAPI("Ranks.Log2")>
    <Extension> Public Function Log2Ranks(data As Generic.IEnumerable(Of Long), Optional Level As Integer = 10) As Long()
        Return data.ToArray(Function(d) CDbl(d)).Log2Ranks
    End Function

    ''' <summary>
    ''' 如果每一个数值之间都是相同的大小，则返回原始数据，因为最大值与最小值的差为0，无法进行映射的创建（会出现除0的错误）
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks>为了要保持顺序，不能够使用并行拓展</remarks>
    ''' 
    <ExportAPI("Ranks.Mapping")>
    <Extension> Public Function GenerateMapping(data As IEnumerable(Of Integer), Optional Level As Integer = 10) As Integer()
        Return GenerateMapping((From n In data Select CDbl(n)).ToArray, Level)
    End Function

    <ExportAPI("Ranks.Mapping")>
    <Extension> Public Function GenerateMapping(data As IEnumerable(Of Long), Optional Level As Integer = 10) As Integer()
        Return GenerateMapping((From n In data Select CDbl(n)).ToArray, Level)
    End Function

    ''' <summary>
    ''' Function centers and/or scales the columns of a numeric matrix.
    ''' </summary>
    ''' <param name="data">numeric matrix</param>
    ''' <param name="center">either a logical value or a numeric vector of length equal to the number of columns of x</param>
    ''' <param name="isScale">either a logical value or a numeric vector of length equal to the number of columns of x</param>
    ''' <returns></returns>
    <ExportAPI("Scale", Info:="function centers and/or scales the columns of a numeric matrix.")>
    Public Function Scale(<Parameter("x", "numeric matrix")> data As Generic.IEnumerable(Of Double),
                          <Parameter("center", "either a logical value or a numeric vector of length equal to the number of columns of x")>
                          Optional center As Boolean = True,
                          <Parameter("scale", "either a logical value or a numeric vector of length equal to the number of columns of x")>
                          Optional isScale As Boolean = True) As Double()
        Dim avg As Double = data.Average
        Dim rms As Double = VBMathExtensions.RMS(data)

        If center Then
            data = (From n In data Select n - avg).ToArray
        End If

        If isScale Then
            data = (From n In data Select n / rms).ToArray
        End If

        Return data.ToArray
    End Function
End Module
