﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Mathematical.diffEq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module BootstrapEstimate

    ''' <summary>
    ''' Bootstrapping 参数估计分析，这个函数用于生成基本的采样数据
    ''' </summary>
    ''' <param name="vars">各个参数的变化范围</param>
    ''' <typeparam name="T">具体的求解方程组</typeparam>>
    ''' <param name="k">重复的次数</param>
    ''' <param name="yinit">``Y0``初值</param>
    ''' <returns></returns>
    Public Iterator Function Bootstrapping(Of T As ODEs)(
                                           vars As IEnumerable(Of NamedValue(Of PreciseRandom)),
                                          yinit As IEnumerable(Of NamedValue(Of PreciseRandom)),
                                              k As Long,
                                              n As Integer,
                                              a As Integer,
                                              b As Integer,
                                           Optional trimNaN As Boolean = True) As IEnumerable(Of ODEsOut)

        Dim params As NamedValue(Of PreciseRandom)() = vars.ToArray
        Dim y0 As NamedValue(Of PreciseRandom)() = yinit.ToArray
        Dim ps As Dictionary(Of String, Action(Of Object, Double)) =
            params _
            .Select(Function(x) x.Name) _
            .SetParameters(Of T)

        For Each x As ODEsOut In From it As Long ' 进行n次并行的采样计算
                                 In k.SeqIterator.AsParallel
                                 Let odes_Out = params.iterate(Of T)(y0, ps, n, a, b)
                                 Let isNaNResult As Boolean = odes_Out.HaveNaN
                                 Where If(trimNaN, Not isNaNResult, True) ' 假若不需要trim，则总是True，即返回所有数据
                                 Select odes_Out
            Yield x
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="TODEs"></typeparam>
    ''' <param name="vars"></param>
    ''' <param name="yinis"></param>
    ''' <param name="ps"></param>
    ''' <param name="n"></param>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    ''' <remarks>在Linux服务器上面有内存泄漏的危险</remarks>
    <Extension>
    Public Function iterate(Of TODEs As ODEs)(vars As NamedValue(Of PreciseRandom)(),
                                              yinis As NamedValue(Of PreciseRandom)(),
                                              ps As Dictionary(Of String, Action(Of Object, Double)),
                                              n As Integer,
                                              a As Integer,
                                              b As Integer) As ODEsOut

        Dim odes As TODEs = Activator.CreateInstance(Of TODEs)
        ' Dim debug As New List(Of NamedValue(Of Double))

        For Each x In vars
            Dim value As Double = x.x.NextNumber
            Call ps(x.Name)(odes, value)  ' 设置方程的参数的值

            'debug += New NamedValue(Of Double) With {
            '    .Name = x.Name,
            '    .x = value
            '}
        Next

        For Each y In yinis
            Dim value As Double = y.x.NextNumber
            odes(y.Name).value = value
            'debug += New NamedValue(Of Double) With {
            '    .Name = y.Name,
            '    .x = value
            '}
        Next

        ' Call debug.GetJson.__DEBUG_ECHO

        Return odes.Solve(n, a, b, incept:=True)
    End Function

    <Extension>
    Public Function SetParameters(Of T As ODEs)(vars As IEnumerable(Of String)) As Dictionary(Of String, Action(Of Object, Double))
        Dim type As Type = GetType(T)
        Dim ps As New Dictionary(Of String, Action(Of Object, Double))

        For Each var As String In vars
            ps(var) = type.FieldSet(Of Double)(var)
        Next

        Return ps
    End Function
End Module