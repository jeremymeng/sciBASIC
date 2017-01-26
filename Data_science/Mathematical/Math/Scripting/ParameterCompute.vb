﻿Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Emit.Parameters
Imports Microsoft.VisualBasic.Language

Namespace Scripting

    ''' <summary>
    ''' 在vb之中由于可选参数的值只能够是常量，假若变量之间还存在关联，则必须要用表达式，
    ''' 但是表达式不是常量，所以使用这个模块之中的代码来模拟R语言之中的可选参数表达式
    ''' </summary>
    Public Module ParameterExpression

        Public Function Demo(c#,
                             Optional x$ = "c*33+5!",
                             Optional y$ = "log(x)+sin(9)",
                             Optional title$ = "This is a title string, not numeric expression") As Double()

            Dim parameters As Dictionary(Of String, Double) = Evaluate(Function() {c, x, y})

            Return {
                c,
                parameters(NameOf(x)),
                parameters(NameOf(y))
            }
        End Function

        '<Extension>
        'Public Function Evaluate(params As IEnumerable(Of Object)) As Dictionary(Of String, Double)
        '    Dim expressions As Expression(Of Func(Of Object))() = params _
        '        .Select(Function(value) DirectCast(value, Expression(Of Func(Of Object)))) _
        '        .ToArray
        '    Return GetMyCaller.Acquire(expressions).Evaluate
        'End Function

        ''' <summary>
        ''' 进行参数计算的时候只会接受数值类型以及字符串类型的参数
        ''' </summary>
        ''' <param name="params">假若参数是不需要进行计算的，则在生成字典的时候不放进去就行了</param>
        ''' <returns></returns>
        <Extension>
        Public Function Evaluate(params As Expression(Of Func(Of Object()))) As Dictionary(Of String, Double)
            Dim caller As MethodBase = GetMyCaller()
            Return caller.InitTable(params).Evaluate(caller)
        End Function

        ''' <summary>
        ''' 在计算参数表达式的同时，也将计算之后的值更新回原来的参数变量之上
        ''' </summary>
        Public Sub Apply(array As Expression(Of Func(Of Object())))
            Dim caller As MethodBase = GetMyCaller()
            Dim values As Dictionary(Of String, Double) = caller.InitTable(array).Evaluate(caller)
            Dim unaryExpression As NewArrayExpression = DirectCast(array.Body, NewArrayExpression)
            Dim arrayData As UnaryExpression() = unaryExpression _
                .Expressions _
                .Select(Function(e) DirectCast(e, UnaryExpression)) _
                .ToArray

            For Each expr As UnaryExpression In arrayData
                Dim member = DirectCast(expr.Operand, MemberExpression)
                Dim constantExpression As ConstantExpression = DirectCast(member.Expression, ConstantExpression)
                Dim name As String = member.Member.Name.Replace("$VB$Local_", "")
                Dim field As FieldInfo = DirectCast(member.Member, FieldInfo)
                Dim value As Object = values(name)
                Dim target As Object = constantExpression.Value

                Select Case field.FieldType
                    Case GetType(String)
                        value = CStr(value)
                    Case GetType(Integer)
                        value = CInt(value)
                    Case GetType(Long)
                        value = CLng(value)
                    Case GetType(Byte)
                        value = CByte(value)
                    Case GetType(Single)
                        value = CSng(value)
                    Case GetType(Decimal)
                        value = CDec(value)
                    Case GetType(Short)
                        value = CShort(value)
                End Select

                Call field.SetValue(target, value)
            Next
        End Sub

        ''' <summary>
        ''' 进行参数计算的时候只会接受数值类型以及字符串类型的参数
        ''' </summary>
        ''' <param name="params">假若参数是不需要进行计算的，则在生成字典的时候不放进去就行了</param>
        ''' <returns></returns>
        <Extension>
        Private Function Evaluate(params As Dictionary(Of Value), caller As MethodBase) As Dictionary(Of String, Double)
            Dim callerParameters As ParameterInfo() = caller _
                .GetParameters _
                .Where(Function(n) params.ContainsKey(n.Name)) _
                .ToArray   ' 按顺序计算
            Dim out As New List(Of String)
            Dim expression As New Expression

            For Each name As ParameterInfo In callerParameters
                Dim value As Value = params(name.Name)

                If value.IsNumeric Then
                    Call expression.SetVariable(name.Name, CDbl(value.value))
                ElseIf value.IsString Then
                    Call expression.SetVariable(name.Name, CStr(value.value))
                Else
                    ' 忽略掉其他的类型
                    Continue For
                End If

                out += name.Name
            Next

            Dim values As Dictionary(Of String, Double) = out _
                .ToDictionary(Function(name) name,
                              Function(name) expression(name))
            Return values
        End Function
    End Module
End Namespace