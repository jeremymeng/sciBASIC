Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace Emit.Parameters

    ''' <summary>
    ''' Exception is a common issue in projects. To track this exception, we use error loggers 
    ''' which only log the exception detail and some other information if you want to. 
    ''' But hardly do we get any idea for which input set(parameters and its values) a 
    ''' particular method is throwing the error.
    ''' </summary>
    ''' <remarks>
    ''' https://www.codeproject.com/tips/795865/log-all-parameters-that-were-passed-to-some-method
    ''' </remarks>
    Public Module ParamLogUtility

        Public Function GetMyCaller() As MethodBase
            ' [2] a()
            ' [1]  ---> b()  ' my caller is a()
            ' [0]  ---> GetMyCaller  
            Return New StackTrace().GetFrame(2).GetMethod()
        End Function

        Public Function Acquire(ParamArray providedParameters As Expression(Of Func(Of Object))()) As Dictionary(Of Value)
            Dim currentMethod As MethodBase = New StackTrace().GetFrame(1).GetMethod()
            Return currentMethod.Acquire(providedParameters)
        End Function

        Public Function Acquire(Of T)(ParamArray providedParameters As Expression(Of Func(Of Object))()) As Dictionary(Of Value)
            Dim type As Type = GetType(T)
            Dim out As Dictionary(Of Value) = ParamLogUtility.Acquire(providedParameters)
            out -= out.Keys.Where(Function(k) Not out(k).Type.Equals(type))
            Return out
        End Function

        <Extension>
        Public Function Acquire(currentMethod As MethodBase, ParamArray providedParameters As Expression(Of Func(Of Object))()) As Dictionary(Of Value)
            Dim out As New Dictionary(Of Value)
            ' Set class and current method info
            Dim trace As New NamedValue(Of MethodBase) With {
                .Name = currentMethod.Name,
                .Value = currentMethod
            }

            ' Get current methods paramaters
            'Dim _methodParamaters As New Dictionary(Of String, Type)()
            'Call (From aParamater As ParameterInfo
            '      In currentMethod.GetParameters
            '      Select New With {
            '          .Name = aParamater.Name,
            '          .DataType = aParamater.ParameterType
            '}).ToList() _
            '  .ForEach(Sub(obj) _methodParamaters.Add(obj.Name, obj.DataType))

            ' Get provided methods paramaters
            For Each aExpression In providedParameters
                Dim bodyType As Expression = aExpression.Body

                If TypeOf bodyType Is MemberExpression Then
                    Call out.AddProvidedParamaterDetail(DirectCast(aExpression.Body, MemberExpression), trace)
                ElseIf TypeOf bodyType Is UnaryExpression Then
                    Dim unaryExpression As UnaryExpression = DirectCast(aExpression.Body, UnaryExpression)
                    Call out.AddProvidedParamaterDetail(DirectCast(unaryExpression.Operand, MemberExpression), trace)
                Else
                    Throw New Exception("Expression type unknown.")
                End If
            Next

            Return out
        End Function

        <Extension>
        Private Sub AddProvidedParamaterDetail(out As Dictionary(Of Value), memberExpression As MemberExpression, trace As NamedValue(Of MethodBase))
            Dim constantExpression As ConstantExpression = DirectCast(memberExpression.Expression, ConstantExpression)
            Dim name As String = memberExpression.Member.Name
            Dim value = DirectCast(memberExpression.Member, FieldInfo).GetValue(constantExpression.Value)
            Dim type As Type = value.[GetType]()

            name = name.Replace("$VB$Local_", "")
            out += New Value With {
                .Name = name,
                .Type = type,
                .value = value,
                .Trace = trace
            }
        End Sub

        <Extension>
        Public Function InitTable(caller As MethodBase, array As Expression(Of Func(Of Object()))) As Dictionary(Of Value)
            Dim unaryExpression As NewArrayExpression = DirectCast(array.Body, NewArrayExpression)
            Dim arrayData As UnaryExpression() = unaryExpression _
                .Expressions _
                .Select(Function(e) DirectCast(e, UnaryExpression)) _
                .ToArray
            Dim out As New Dictionary(Of Value)
            Dim trace As New NamedValue(Of MethodBase) With {
                .Name = caller.Name,
                .Value = caller
            }

            For Each expr As UnaryExpression In arrayData
                Dim member = DirectCast(expr.Operand, MemberExpression)
                Dim constantExpression As ConstantExpression = DirectCast(member.Expression, ConstantExpression)
                Dim name As String = member.Member.Name.Replace("$VB$Local_", "")
                Dim field As FieldInfo = DirectCast(member.Member, FieldInfo)
                Dim value As Object = field.GetValue(constantExpression.Value)

                out += New Value With {
                    .Name = name,
                    .Type = value.GetType,
                    .value = value,
                    .Trace = trace
                }
            Next

            Return out
        End Function
    End Module
End Namespace
