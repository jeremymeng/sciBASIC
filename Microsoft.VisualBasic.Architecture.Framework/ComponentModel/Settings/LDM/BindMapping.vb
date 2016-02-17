﻿Imports System.Reflection

Namespace ComponentModel.Settings

    Public Class BindMapping : Inherits ProfileItem

        Dim _target As Object

        Public ReadOnly Property BindProperty As PropertyInfo

        Public Shared Function Initialize(attr As ProfileItem, prop As PropertyInfo, obj As Object) As BindMapping
            Dim maps As New BindMapping With {
                .Name = attr.Name,
                .Description = attr.Description,
                .Type = attr.Type,
                ._bindProperty = prop,
                ._target = obj
            }
            Return maps
        End Function

        Public ReadOnly Property Value As String
            Get
                Dim result As Object =
                    BindProperty.GetValue(_target)
                Return Scripting.ToString(result)
            End Get
        End Property

        Public Sub [Set](value As String)
            Dim obj As Object =
                Scripting.CTypeDynamic(value, _bindProperty.PropertyType)
            Call _bindProperty.SetValue(_target, obj)
        End Sub

        ''' <summary>
        ''' 打印在终端窗口上面的字符串
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AsOutString As String
            Get
                Return String.Format("{0} = ""{1}""", Name, Value)
            End Get
        End Property

        ''' <summary>
        ''' 这个方法只是针对<see cref="ValueTypes.File"/>和<see cref="ValueTypes.Directory"/>这两种类型而言才有效的
        ''' 对于其他的类型数据，都是返回False
        ''' </summary>
        ''' <returns></returns>
        Public Function IsFsysValid() As Boolean
            If Type = ValueTypes.Directory Then
                Return Value.DirectoryExists
            ElseIf Type = ValueTypes.File Then
                Return Value.FileExists
            Else
                Return False
            End If
        End Function
    End Class
End Namespace