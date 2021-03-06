﻿#Region "Microsoft.VisualBasic::e8366e956915b17e58fc0b896323550b, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\Extensions\IO\PathSearchTool.vb"

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

Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Math
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

''' <summary>
''' Search the path from a specific keyword.(通过关键词来推测路径)
''' </summary>
''' <remarks></remarks>
<Package("Program.Path.Search",
                    Description:="A utility tools for searching a specific file of its path on the file system more easily.")>
Public Module ProgramPathSearchTool

    ''' <summary>
    ''' 函数返回文件的拓展名后缀，请注意，这里的返回值是不会带有小数点的
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ExtensionSuffix(path$) As String
        Return path.Split("."c).Last
    End Function

    ''' <summary>
    ''' Combine directory path.(这个主要是用于生成文件夹名称)
    ''' 
    ''' ###### Example usage
    ''' 
    ''' ```vbnet
    ''' Dim images As Dictionary(Of String, String) =
    '''     (ls - l - {"*.png", "*.jpg", "*.gif"} &lt;= PlatformEngine.wwwroot.DIR("images")) _
    '''     .ToDictionary(Function(file) file.StripAsId,
    '''                   AddressOf FileName)
    ''' ```
    ''' </summary>
    ''' <param name="d"></param>
    ''' <param name="name"></param>
    ''' <returns></returns>
    <Extension>
    Public Function DIR(d As IO.DirectoryInfo, name As String) As String
        Return $"{d.FullName}/{name}"
    End Function

    <Extension>
    Public Function UnixPath(path As String) As String
        Return FileIO.FileSystem.GetFileInfo(path).FullName.Replace("\", "/")
    End Function

    ''' <summary>
    ''' Make directory
    ''' </summary>
    ''' <param name="DIR"></param>
    <Extension> Public Sub MkDIR(DIR$)
        Try
            Call FileIO.FileSystem.CreateDirectory(DIR)
        Catch ex As Exception
            ex = New Exception("DIR value is: " & DIR, ex)
            Throw ex
        End Try
    End Sub

    <Extension>
    Public Function PathCombine(path As String, addTag As String) As String
        If path.DirectoryExists Then
            Return path.ParentPath & "/" & path.BaseName & addTag
        Else
            Return path.TrimSuffix & addTag
        End If
    End Function

    ''' <summary>
    ''' 使用<see cref="FileIO.FileSystem.GetFiles"/>函数枚举
    ''' **当前的**(不是递归的搜索所有的子文件夹)文件夹之中的
    ''' 所有的符合条件的文件路径
    ''' </summary>
    ''' <param name="DIR">文件夹路径</param>
    ''' <param name="keyword">文件名进行匹配的关键词</param>
    ''' <returns></returns>
    <Extension>
    Public Function EnumerateFiles(DIR$, ParamArray keyword$()) As IEnumerable(Of String)
        Dim files = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, keyword)
        Return files
    End Function

    ''' <summary>
    ''' ```
    ''' ls - l - r - pattern &lt;= DIR
    ''' ```
    ''' 
    ''' 的简化拓展函数模式
    ''' </summary>
    ''' <param name="DIR$"></param>
    ''' <param name="pattern$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ListFiles(DIR$, Optional pattern$ = "*.*") As IEnumerable(Of String)
        Return ls - l - r - pattern <= DIR
    End Function

    ''' <summary>
    ''' 这个函数是会直接枚举出所有的文件路径的
    ''' </summary>
    ''' <param name="DIR$"></param>
    ''' <param name="[option]"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function ReadDirectory(DIR$, Optional [option] As FileIO.SearchOption = FileIO.SearchOption.SearchTopLevelOnly) As IEnumerable(Of String)
        Dim current As New DirectoryInfo(DIR)

        For Each file In current.EnumerateFiles
            Yield file.FullName
        Next

        If [option] = FileIO.SearchOption.SearchAllSubDirectories Then
            For Each folder In current.EnumerateDirectories
                For Each path In folder.FullName.ReadDirectory([option])
                    Yield path
                Next
            Next
        End If
    End Function

    <Extension>
    Public Iterator Function ListDirectory(DIR$, Optional [option] As FileIO.SearchOption = FileIO.SearchOption.SearchTopLevelOnly) As IEnumerable(Of String)
        Dim current As New DirectoryInfo(DIR)

        For Each folder In current.EnumerateDirectories
            Yield folder.FullName

            If [option] = FileIO.SearchOption.SearchAllSubDirectories Then
                For Each path In folder.FullName.ListDirectory([option])
                    Yield path
                Next
            End If
        Next
    End Function

    ''' <summary>
    ''' 这个函数只会返回文件列表之中的第一个文件，故而需要提取某一个文件夹之中的某一个特定的文件，推荐使用这个函数
    ''' </summary>
    ''' <param name="DIR$"></param>
    ''' <param name="keyword$"></param>
    ''' <param name="opt"></param>
    ''' <returns>当查找不到目标文件或者文件夹不存在的时候会返回空值</returns>
    <Extension>
    Public Function TheFile(DIR$, keyword$, Optional opt As FileIO.SearchOption = FileIO.SearchOption.SearchTopLevelOnly) As String
        If Not DIR.DirectoryExists Then
            Return Nothing
        End If
        Return FileIO.FileSystem.GetFiles(DIR, opt, keyword).FirstOrDefault
    End Function

    ''' <summary>
    ''' Gets the URL type file path.(获取URL类型的文件路径)
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Path2Url", Info:="Gets the URL type file path.")>
    <Extension> Public Function ToFileURL(Path As String) As String
        If String.IsNullOrEmpty(Path) Then
            Return ""
        Else
            Path = FileIO.FileSystem.GetFileInfo(Path).FullName
            Return String.Format("file:///{0}", Path.Replace("\", "/"))
        End If
    End Function

    <ExportAPI("DIR2URL"), ExtensionAttribute>
    Public Function ToDIR_URL(DIR As String) As String
        If String.IsNullOrEmpty(DIR) Then
            Return ""
        Else
            DIR = FileIO.FileSystem.GetDirectoryInfo(DIR).FullName
            Return String.Format("file:///{0}", DIR.Replace("\", "/"))
        End If
    End Function

    ''' <summary>
    ''' 枚举所有非法的路径字符
    ''' </summary>
    ''' <remarks></remarks>
    Public Const ILLEGAL_PATH_CHARACTERS_ENUMERATION As String = ":*?""<>|"
    Public Const ILLEGAL_FILENAME_CHARACTERS As String = "\/" & ILLEGAL_PATH_CHARACTERS_ENUMERATION

    ''' <summary>
    ''' 将目标字符串之中的非法的字符替换为"_"符号以成为正确的文件名字符串
    ''' </summary>
    ''' <param name="str"></param>
    ''' <param name="OnlyASCII">当本参数为真的时候，仅26个字母，0-9数字和下划线_以及小数点可以被保留下来</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("NormalizePathString")>
    <Extension> Public Function NormalizePathString(str As String, Optional OnlyASCII As Boolean = True) As String
        Return NormalizePathString(str, "_", OnlyASCII)
    End Function

    <ExportAPI("NormalizePathString")>
    <Extension> Public Function NormalizePathString(str As String, normAs As String, Optional onlyASCII As Boolean = True) As String
        Dim sb As New StringBuilder(str)
        For Each ch As Char In ILLEGAL_FILENAME_CHARACTERS
            Call sb.Replace(ch, normAs)
        Next

        If onlyASCII Then
            For Each ch As Char In "()[]+-~!@#$%^&=;',"
                Call sb.Replace(ch, normAs)
            Next
        End If

        Return sb.ToString
    End Function

    Const PathTooLongException =
        "System.IO.PathTooLongException: The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters."

    ''' <summary>
    ''' 假设文件名过长发生在文件名和最后一个文件夹的路径之上
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' System.IO.PathTooLongException: The specified path, file name, or both are too long.
    ''' The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters.
    ''' </remarks>
    <Extension> Public Function Long2Short(path As String, <CallerMemberName> Optional caller As String = "") As String
        Dim parent As String = path.ParentPath
        Dim DIRTokens As String() = parent.Replace("\", "/").Split("/"c)
        Dim DIRname As String = DIRTokens.Last  ' 请注意，由于path参数可能是相对路径，所以在这里DIRname和name要分开讨论
        Dim name As String = path.Replace("\", "/").Split("/"c).Last  ' 因为相对路径最终会出现文件夹名称，但在path里面可能是使用小数点来表示的
        If parent.Length + name.Length >= 259 Then
            DIRname = Mid(DIRname, 1, 20) & "~"
            Dim ext As String = name.Split("."c).Last
            name = Mid(name, 1, 20) & "~." & ext
            parent = String.Join("/", DIRTokens.Take(DIRTokens.Length - 1).ToArray)
            parent &= "/" & DIRname
            parent &= "/" & name

            Dim ex As Exception = New PathTooLongException(PathTooLongException)
            ex = New Exception(path, ex)
            ex = New Exception("But the path was corrected as:   " & parent & "  to avoid the crashed problem.", ex)
            Call ex.PrintException
            Call App.LogException(ex, caller & " -> " & MethodBase.GetCurrentMethod.GetFullName)

            Return parent.Replace("\", "/")
        Else
            Return FileIO.FileSystem.GetFileInfo(path).FullName
        End If
    End Function

    ''' <summary>
    ''' File path illegal?
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <ExportAPI("Path.Illegal?")>
    <Extension> Public Function PathIllegal(path As String) As Boolean
        Dim tokens As String() = Strings.Split(path.Replace("\", "/"), ":/")

        If tokens.Length > 2 Then  ' 有多余一个的驱动器符，则肯定是非法的路径格式
            Return False
        ElseIf tokens.Length = 2 Then
            ' 完整路径
            ' 当有很多个驱动器的时候，这里会不止一个字母
            If Regex.Match(tokens(0), "[a-Z0-9]+", RegexICSng).Value <> tokens(0) Then
                ' 开头的驱动器的符号不正确
                Return False
            Else
                ' 驱动器的符号也正确
            End If
        Else
            ' 只有一个，则是相对路径
        End If

        Dim fileName As String = tokens.Last

        ' 由于这里是判断文件是否合法，所以之判断文件名就行了，即token列表的最后一个元素
        For Each ch As Char In ILLEGAL_PATH_CHARACTERS_ENUMERATION
            If fileName.IndexOf(ch) > -1 Then
                Return True
            End If
        Next

        Return False
    End Function

    ''' <summary>
    ''' Gets the file length, if the path is not exists, then returns -1.
    ''' (安全的获取文件的大小，如果目标文件不存在的话会返回-1)
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FileLength(path As String) As Long
        If Not path.FileExists Then
            Return -1&
        Else
            Return FileIO.FileSystem.GetFileInfo(path).Length
        End If
    End Function

    ''' <summary>
    ''' Safe file copy operation
    ''' </summary>
    ''' <param name="source$"></param>
    ''' <param name="copyTo$"></param>
    ''' <returns></returns>
    <Extension> Public Function FileCopy(source$, copyTo$) As Boolean
        Try
            If copyTo.FileExists Then
                Call FileIO.FileSystem.DeleteFile(copyTo)
            Else
                Call copyTo.ParentPath.MkDIR
            End If

            Call FileIO.FileSystem.CopyFile(source, copyTo)
        Catch ex As Exception
            ex = New Exception({source, copyTo}.GetJson, ex)
            App.LogException(ex)

            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' Check if the target file object is exists on your file system or not.
    ''' (这个函数也会自动检查目标<paramref name="path"/>参数是否为空)
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="ZERO_Nonexists">将0长度的文件也作为不存在</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
#If FRAMEWORD_CORE Then
    <ExportAPI("File.Exists", Info:="Check if the target file object is exists on your file system or not.")>
    <Extension> Public Function FileExists(path As String, Optional ZERO_Nonexists As Boolean = False) As Boolean
#Else
    <Extension> Public Function FileExists(path As String) As Boolean
#End If
        If path.IndexOf(ASCII.CR) > -1 OrElse path.IndexOf(ASCII.LF) > -1 Then
            Return False ' 包含有回车符或者换行符，则肯定不是文件路径了
        End If

        If Not String.IsNullOrEmpty(path) AndAlso
            FileIO.FileSystem.FileExists(path) Then  ' 文件存在

            If ZERO_Nonexists Then
                Return FileSystem.FileLen(path) > 0
            Else
                Return True
            End If
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Determine that the target directory is exists on the file system or not?(判断文件夹是否存在)
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <returns></returns>
    <ExportAPI("DIR.Exists", Info:="Determine that the target directory is exists on the file system or not?")>
    <Extension>
    Public Function DirectoryExists(DIR As String) As Boolean
        Return Not String.IsNullOrEmpty(DIR) AndAlso
            FileIO.FileSystem.DirectoryExists(DIR)
    End Function

    ''' <summary>
    ''' Get the directory its name of the target <paramref name="dir"/> directory
    ''' </summary>
    ''' <param name="dir$"></param>
    ''' <returns></returns>
    <Extension> Public Function DirectoryName(dir$) As String
        Return dir.TrimDIR _
            .Split("\"c).Last _
            .Split("/"c).Last
    End Function

    ''' <summary>
    ''' Check if the file is opened by other code?(检测文件是否已经被其他程序打开使用之中)
    ''' </summary>
    ''' <param name="FileName">目标文件</param>
    ''' <returns></returns>
    <ExportAPI("File.IsOpened", Info:="Detect while the target file is opened by someone process.")>
    <Extension> Public Function FileOpened(FileName As String) As Boolean
        Try
            Using FileOpenDetect As New FileStream(FileName,
                                                   IO.FileMode.Open,
                                                   IO.FileAccess.Read,
                                                   IO.FileShare.None)
                ' Just detects this file is occupied, no more things needs to do....
            End Using
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Gets the name of the target file or directory, if the target is a file, then the name without the extension name.
    ''' (获取目标文件夹的名称或者文件的不包含拓展名的名称)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' ###### 2017-2-14 
    ''' 原先的函数是依靠系统的API来工作的，故而只能够工作于已经存在的文件之上，
    ''' 所以在这里为了更加方便的兼容文件夹或者文件路径，在这使用字符串的方法来
    ''' 进行截取
    ''' </remarks>
    <ExportAPI(NameOf(BaseName), Info:="Gets the name of the target directory/file object.")>
    <Extension> Public Function BaseName(fsObj As String, Optional allowEmpty As Boolean = False) As String
        If fsObj.StringEmpty Then
            If allowEmpty Then
                Return ""
            Else
                Throw New NullReferenceException(NameOf(fsObj) & " file system object handle is null!")
            End If
        End If

        ' 前面的代码已经处理好了空字符串的情况了，在这里不会出现空字符串的错误
        Dim t$() = fsObj.Trim("\"c, "/"c).Replace("\", "/").Split("/"c)
        t = t.Last.Split("."c)
        If t.Length > 1 Then
            ' 文件名之中并没有包含有拓展名后缀，则数组长度为1，则不跳过了
            ' 有后缀拓展名，则split之后肯定会长度大于1的
            t = t.Take(t.Length - 1).ToArray
        End If

        Dim name = String.Join(".", t)
        Return name
    End Function

    ''' <summary>
    ''' <see cref="basename"/> shortcuts extension.
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <Extension> Public Function GetBaseName(path As String) As String
        Return BaseName(path)
    End Function

    ''' <summary>
    ''' Gets the name of the file's parent directory, returns value is a name, not path.
    ''' (获取目标文件的父文件夹的文件夹名称，是名称而非路径)
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <Extension> Public Function ParentDirName(file As String) As String
        Dim parentDir As String = FileIO.FileSystem.GetParentPath(file)
        Dim parDirInfo = FileIO.FileSystem.GetDirectoryInfo(parentDir)
        Return parDirInfo.Name
    End Function

    ''' <summary>
    ''' 这个函数是返回文件夹的路径而非名称，这个函数不依赖于系统的底层API，因为系统的底层API对于过长的文件名会出错
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <remarks>这个函数不依赖于系统的底层API，因为系统的底层API对于过长的文件名会出错</remarks>
    <ExportAPI(NameOf(ParentPath))>
    <Extension> Public Function ParentPath(file$, Optional full As Boolean = True) As String
        file = file.Replace("\", "/")

        Dim parent As String = ""
        Dim t As String() = file.Split("/"c)

        If full Then
            If InStr(file, "../") = 1 Then
                parent = FileIO.FileSystem.GetParentPath(App.CurrentDirectory)
                t = t.Skip(1).ToArray
                parent &= "/"
            ElseIf InStr(file, "./") = 1 Then
                parent = App.CurrentDirectory
                t = t.Skip(1).ToArray
                parent &= "/"
            Else

            End If

            If file.Last = "/"c Then ' 是一个文件夹
                parent &= String.Join("/", t.Take(t.Length - 2).ToArray)
            Else
                parent &= String.Join("/", t.Take(t.Length - 1).ToArray)
            End If

            If parent.StringEmpty Then
                ' 用户直接输入了一个文件名，没有包含文件夹部分，则默认是当前的文件夹
                parent = App.CurrentDirectory
            End If
        Else
            parent = String.Join("/", t.Take(t.Length - 1).ToArray)
        End If

        Return parent
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <param name="keyword"></param>
    ''' <param name="ext">元素的排布是有顺序的</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Get.File.Path")>
    <Extension> Public Function GetFile(DIR As String,
                                       <Parameter("Using.Keyword")> keyword As String,
                                       <Parameter("List.Ext")> ParamArray ext As String()) _
                                    As <FunctionReturns("A list of file path which match with the keyword and the file extension name.")> String()

        Dim Files As IEnumerable(Of String) = ls - l - wildcards(ext) <= DIR
        Dim matches = (From Path As String
                       In Files.AsParallel
                       Let NameID = basename(Path)
                       Where InStr(NameID, keyword, CompareMethod.Text) > 0
                       Let ExtValue = Path.Split("."c).Last
                       Select Path,
                           ExtValue)
        Dim LQuery =
            From extType As String
            In ext
            Select From path
                   In matches
                   Where InStr(extType, path.ExtValue, CompareMethod.Text) > 0
                   Select path.Path
        Return LQuery.IteratesALL.Distinct.ToArray
    End Function

    ''' <summary>
    ''' 这个方法没得卵用
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <returns></returns>
    <ExportAPI("Md5.Renamed")>
    Public Function BatchMd5Renamed(DIR As String) As Boolean
        DIR = FileIO.FileSystem.GetDirectoryInfo(DIR).FullName

        For Each path As String In FileIO.FileSystem.GetFiles(DIR)
            On Error Resume Next

            Dim Md5 As String = SecurityString.GetMd5Hash(path)
            Dim ext As String = IO.Path.GetExtension(path)
            Dim FileName As String = DIR & "/" & Md5 & ext

            Call IO.File.Move(path, FileName)
        Next

        Return True
    End Function

    ''' <summary>
    ''' [<see cref="FileIO.SearchOption.SearchAllSubDirectories"/>，这个函数会扫描目标文件夹下面的所有文件。]
    ''' 请注意，本方法是不能够产生具有相同的主文件名的数据的。假若目标GBK是使用本模块之中的方法保存或者导出来的，
    ''' 则可以使用本方法生成Entry列表；（在返回的结果之中，KEY为文件名，没有拓展名，VALUE为文件的路径）
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Load.ResourceEntry",
               Info:="Load the file from a specific directory from the source parameter as the resource entry list.")>
    <Extension>
    Public Function LoadSourceEntryList(<Parameter("Dir.Source", "The source directory which will be searchs for file.")> source As String,
                                        <Parameter("List.Ext", "The list of the file extension.")> ext As String(),
                                        Optional topLevel As Boolean = True) As Dictionary(Of String, String)

        If ext.IsNullOrEmpty Then
            ext = {"*.*"}
        End If

        Dim LQuery = (From path As String
                      In If(topLevel, ls - l, ls - l - r) - wildcards(ext) <= source
                      Select ID = BaseName(path),
                          path
                      Group By ID Into Group).ToArray

        ext = LinqAPI.Exec(Of String) <= From value As String
                                         In ext
                                         Select value.Split(CChar(".")).Last.ToLower

        Dim res As Dictionary(Of String, String) =
            LQuery.ToDictionary(Function(x) x.ID,
                                Function(x) LinqAPI.DefaultFirst(Of String) <= From path
                                                                               In x.Group
                                                                               Let extValue As String = path.path.Split("."c).Last.ToLower
                                                                               Where Array.IndexOf(ext, extValue) > -1
                                                                               Select path.path)
        res = (From entry
               In res
               Where Not String.IsNullOrEmpty(entry.Value)
               Select entry) _
                    .ToDictionary(Function(x) x.Key,
                                  Function(x) x.Value)

        Call $"{NameOf(ProgramPathSearchTool)} load {res.Count} source entry...".__DEBUG_ECHO

        Return res
    End Function

    ''' <summary>
    ''' 可以使用本方法生成Entry列表；（在返回的结果之中，KEY为文件名，没有拓展名，VALUE为文件的路径）
    ''' 请注意，这个函数会搜索目标文件夹下面的所有的文件夹的
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="ext">文件类型的拓展名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function LoadSourceEntryList(source As String, ParamArray ext As String()) As Dictionary(Of String, String)
        If Not FileIO.FileSystem.DirectoryExists(source) Then
            Return New Dictionary(Of String, String)
        End If

        Dim LQuery = From path As String
                     In FileIO.FileSystem.GetFiles(source, FileIO.SearchOption.SearchAllSubDirectories, ext)
                     Select ID = basename(path),
                          path
                     Group By ID Into Group
        Dim dict As Dictionary(Of String, String) =
            LQuery.ToDictionary(Function(x) x.ID,
                                Function(x) x.Group.First.path)
        Return dict
    End Function

    ''' <summary>
    ''' 允许有重复的数据
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <param name="exts"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Load.ResourceEntry")>
    <Extension> Public Function LoadEntryList(<Parameter("Dir.Source")> DIR As String, ParamArray exts As String()) As NamedValue(Of String)()
        Dim LQuery As NamedValue(Of String)() =
            LinqAPI.Exec(Of NamedValue(Of String)) <= From path As String
                                                      In ls - l - ShellSyntax.r - wildcards(exts) <= DIR
                                                      Select New NamedValue(Of String)(path.BaseName, path)
        Return LQuery
    End Function

    <ExportAPI("Load.ResourceEntry", Info:="Load the file from a specific directory from the source parameter as the resource entry list.")>
    <Extension>
    Public Function LoadSourceEntryList(source As IEnumerable(Of String)) As Dictionary(Of String, String)
        Dim LQuery = From path As String
                     In source
                     Select ID = basename(path),
                         path
                     Group By ID Into Group
        Dim res As Dictionary(Of String, String) =
            LQuery.ToDictionary(Function(x) x.ID,
                                Function(x) x.Group.First.path)
        Return res
    End Function

    ''' <summary>
    ''' 将不同来源<paramref name="source"></paramref>的文件复制到目标文件夹<paramref name="copyto"></paramref>之中
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="copyto"></param>
    ''' <returns>返回失败的文件列表</returns>
    ''' <remarks></remarks>
    <ExportAPI("Source.Copy",
               Info:="Copy the file in the source list into the copyto directory, function returns the failed operation list.")>
    Public Function SourceCopy(source As IEnumerable(Of String), CopyTo As String, Optional [Overrides] As Boolean = False) As String()
        Dim failedList As New List(Of String)

        For Each file As String In source
            Try
                Call FileIO.FileSystem.CopyFile(file, CopyTo & "/" & FileIO.FileSystem.GetFileInfo(file).Name, [Overrides])
            Catch ex As Exception
                Call failedList.Add(file)
                Call App.LogException(New Exception(file, ex))
            End Try
        Next

        Return failedList.ToArray
    End Function

    <ExportAPI("Get.FrequentPath",
               Info:="Gets a directory path which is most frequent appeared in the file list.")>
    Public Function GetMostAppreancePath(files As IEnumerable(Of String)) As String
        If files.IsNullOrEmpty Then
            Return ""
        End If

        Dim LQuery = From strPath As String
                     In files
                     Select FileIO.FileSystem.GetParentPath(strPath)
        Return LQuery _
            .CountTokens(IgnoreCase:=True) _
            .OrderByDescending(Function(x) x.Value) _
            .First.Key
    End Function

    ''' <summary>
    ''' Invoke the search session for the program file using a specific keyword string value.(使用某个关键词来搜索目标应用程序)
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <param name="Keyword"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("File.Search.Program",
               Info:="Invoke the search session for the program file using a specific keyword string value.")>
    Public Function SearchProgram(DIR As String, Keyword As String) As String()
        Dim ExeNameRule As String = String.Format("*{0}*.exe", Keyword)
        Dim DllNameRule As String = String.Format("*{0}*.dll", Keyword)

        Dim Files = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, ExeNameRule, DllNameRule)
        Dim binDIR As String = String.Format("{0}/bin/", DIR)
        Dim ProgramDIR As String = String.Format("{0}/Program", DIR)
        Dim buffer As New List(Of String)

        If FileIO.FileSystem.DirectoryExists(binDIR) Then
            buffer += FileIO.FileSystem.GetFiles(
                binDIR,
                FileIO.SearchOption.SearchTopLevelOnly,
                ExeNameRule, DllNameRule)
        End If
        If FileIO.FileSystem.DirectoryExists(ProgramDIR) Then
            buffer += FileIO.FileSystem.GetFiles(
                ProgramDIR,
                FileIO.SearchOption.SearchTopLevelOnly,
                ExeNameRule, DllNameRule)
        End If

        buffer += Files

        Return buffer.ToArray
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <param name="Keyword"></param>
    ''' <param name="withExtension">脚本文件的文件拓展名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Search.Scripts", Info:="Search for the path of a script file with a specific extension name.")>
    Public Function SearchScriptFile(DIR As String, Keyword As String, Optional withExtension As String = "") As String()
        Dim ScriptFileNameRule As String = String.Format("*{0}*{1}", Keyword, withExtension)
        Dim Files = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, ScriptFileNameRule)
        Dim binDIR As String = String.Format("{0}/bin/", DIR)
        Dim ProgramDIR As String = String.Format("{0}/Program", DIR)
        Dim ScriptsDIR As String = String.Format("{0}/scripts", DIR)
        Dim fileList As New List(Of String)

        If FileIO.FileSystem.DirectoryExists(binDIR) Then fileList += (ls - l - wildcards(ScriptFileNameRule) <= binDIR)
        If FileIO.FileSystem.DirectoryExists(ProgramDIR) Then fileList += (ls - l - wildcards(ScriptFileNameRule) <= ProgramDIR)
        If FileIO.FileSystem.DirectoryExists(ScriptsDIR) Then fileList += (ls - l - wildcards(ScriptFileNameRule) <= ScriptsDIR)

        Call fileList.AddRange(Files)

        If String.IsNullOrEmpty(withExtension) Then
            Return LinqAPI.Exec(Of String) <= From strPath As String
                                              In fileList
                                              Let ext As String = FileIO.FileSystem.GetFileInfo(strPath).Extension
                                              Where String.IsNullOrEmpty(ext)
                                              Select strPath
        Else
            Return fileList.ToArray
        End If
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="SpecificDrive">所制定进行搜索的驱动器，假若希望搜索整个硬盘，请留空字符串</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("DIR.Search.Program_Directory",
               Info:="Search for the directories which its name was matched the keyword pattern.")>
    Public Function SearchDirectory(Keyword As String, SpecificDrive As String) As String()
        Dim Drives As ReadOnlyCollection(Of DriveInfo) =
            If(String.IsNullOrEmpty(SpecificDrive),
               FileIO.FileSystem.Drives,
               New ReadOnlyCollection(Of IO.DriveInfo)(
                   {FileIO.FileSystem.GetDriveInfo(SpecificDrive)}))
        Dim DIRs As New List(Of String)

        For Each Drive As DriveInfo In Drives
            DIRs += SearchDrive(Drive, Keyword)
        Next

        Return DIRs.ToArray
    End Function

    <Extension>
    Private Function SearchDrive(Drive As DriveInfo, keyword As String) As String()
        If Not Drive.IsReady Then
            Return New String() {}
        End If

        Dim DriveRoot = FileIO.FileSystem.GetDirectories(Drive.RootDirectory.FullName, FileIO.SearchOption.SearchTopLevelOnly, keyword)
        Dim files As New List(Of String)

        Dim ProgramFiles As String = String.Format("{0}/Program Files", Drive.RootDirectory.FullName)
        If FileIO.FileSystem.DirectoryExists(ProgramFiles) Then
            Call files.AddRange(BranchRule(ProgramFiles, keyword))
        End If

        Dim ProgramFilesX86 = String.Format("{0}/Program Files(x86)", Drive.RootDirectory.FullName)
        If FileIO.FileSystem.DirectoryExists(ProgramFilesX86) Then
            Call files.AddRange(BranchRule(ProgramFilesX86, keyword))
        End If
        Call files.AddRange(DriveRoot)
        Call files.AddRange(DriveRoot.ToArray(Function(rootDir) BranchRule(rootDir, keyword)).Unlist)

        Return files.ToArray
    End Function

    ''' <summary>
    ''' 商标搜索规则
    ''' </summary>
    ''' <param name="ProgramFiles"></param>
    ''' <param name="Keyword"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BranchRule(ProgramFiles As String, Keyword As String) As String()
        Dim ProgramFiles_Directories = FileIO.FileSystem.GetDirectories(
            ProgramFiles,
            FileIO.SearchOption.SearchTopLevelOnly,
            Keyword)
        Dim fsObjs As New List(Of String)

        For Each Dir As String In ProgramFiles_Directories
            fsObjs += FileIO.FileSystem.GetDirectories(
                Dir, FileIO.SearchOption.SearchTopLevelOnly)
        Next
        Call fsObjs.Add(ProgramFiles_Directories.ToArray)

        If fsObjs.Count = 0 Then
            ' 这个应用程序的安装文件夹可能是带有版本号标记的
            Dim Dirs = FileIO.FileSystem.GetDirectories(ProgramFiles, FileIO.SearchOption.SearchTopLevelOnly)
            Dim version As String = Keyword & ProgramPathSearchTool.VERSION
            Dim Patterns As String() =
                LinqAPI.Exec(Of String) <= From DIR As String
                                           In Dirs
                                           Let name As String = FileIO.FileSystem.GetDirectoryInfo(DIR).Name
                                           Where Regex.Match(name, version, RegexOptions.IgnoreCase).Success
                                           Select DIR
            Call fsObjs.Add(Patterns)
        End If

        Return fsObjs.ToArray
    End Function

    Const VERSION As String = "[-_`~.]\d+(\.\d+)*"

    ''' <summary>
    ''' 获取相对于本应用程序的目标文件的相对路径(请注意，所生成的相对路径之中的字符串最后是没有文件夹的分隔符\或者/的)
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    '''
    <ExportAPI(NameOf(RelativePath),
               Info:="Get the specific file system object its relative path to the application base directory.")>
    Public Function RelativePath(path As String) As String
        Return RelativePath(App.HOME, path)
    End Function

    ''' <summary>
    ''' Gets the relative path of file system object <paramref name="pcTo"/> reference to the directory path <paramref name="pcFrom"/>.
    ''' (请注意，所生成的相对路径之中的字符串最后是没有文件夹的分隔符\或者/的)
    ''' </summary>
    ''' <param name="pcFrom">生成相对路径的参考文件夹</param>
    ''' <param name="pcTo">所需要生成相对路径的目标文件系统对象的绝对路径或者相对路径</param>
    ''' <param name="appendParent">是否将父目录的路径也添加进入相对路径之中？默认是</param>
    ''' <returns></returns>
    <ExportAPI(NameOf(RelativePath),
               Info:="Gets the relative path value of pcTo file system object relative to a reference directory pcFrom")>
    Public Function RelativePath(pcFrom$, pcTo$, Optional appendParent As Boolean = True) As <FunctionReturns("The relative path string of pcTo file object reference to directory pcFrom")> String
        Dim lcRelativePath As String = Nothing
        Dim lcFrom As String = (If(pcFrom Is Nothing, "", pcFrom.Trim()))
        Dim lcTo As String = (If(pcTo Is Nothing, "", pcTo.Trim()))

        If lcFrom.Length = 0 OrElse lcTo.Length = 0 Then
            Throw New InvalidDataException("One of the path string value is null!")
        End If
        If Not IO.Path.GetPathRoot(lcFrom.ToUpper()) _
            .Equals(IO.Path.GetPathRoot(lcTo.ToUpper())) Then
            Return pcTo
        End If

        ' 两个路径都有值并且都在相同的驱动器下才会进行计算

        Dim laDirSep As Char() = {"\"c}
        Dim lcPathFrom As String = (If(IO.Path.GetDirectoryName(lcFrom) Is Nothing, IO.Path.GetPathRoot(lcFrom.ToUpper()), IO.Path.GetDirectoryName(lcFrom)))
        Dim lcPathTo As String = (If(IO.Path.GetDirectoryName(lcTo) Is Nothing, IO.Path.GetPathRoot(lcTo.ToUpper()), IO.Path.GetDirectoryName(lcTo)))
        Dim lcFileTo As String = (If(IO.Path.GetFileName(lcTo) Is Nothing, "", IO.Path.GetFileName(lcTo)))
        Dim laFrom As String() = lcPathFrom.Split(laDirSep)
        Dim laTo As String() = lcPathTo.Split(laDirSep)
        Dim lnFromCnt As Integer = laFrom.Length
        Dim lnToCnt As Integer = laTo.Length
        Dim lnSame As Integer = 0
        Dim lnCount As Integer = 0

        While lnToCnt > 0 AndAlso lnSame < lnToCnt
            If lnCount < lnFromCnt Then
                If laFrom(lnCount).ToUpper().Equals(laTo(lnCount).ToUpper()) Then
                    lnSame += 1
                Else
                    Exit While
                End If
            Else
                Exit While
            End If
            lnCount += 1
        End While

        Dim lcEndPart As String = ""
        For lnEnd As Integer = lnSame To lnToCnt - 1
            If laTo(lnEnd).Length > 0 Then
                lcEndPart += laTo(lnEnd) & "\"
            Else
                Exit For
            End If
        Next

        Dim lnDiff As Integer = Abs(lnFromCnt - lnSame)
        If lnDiff > 0 AndAlso laFrom(lnFromCnt - 1).Length > 0 Then
            While lnDiff > 0
                lnDiff -= 1
                lcEndPart = "..\" & lcEndPart
            End While
        End If

        lcRelativePath = lcEndPart & lcFileTo

        If appendParent Then
            Return "..\" & lcRelativePath
        Else
            ' 2017-8-26
            ' 为Xlsx打包模块进行的修复
            Return lcRelativePath.Split("\"c).Skip(1).JoinBy("\")
        End If
    End Function

    ''' <summary>
    ''' Gets the full path of the specific file.(为了兼容Linux，这个函数会自动替换路径之中的\为/符号)
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("File.FullPath", Info:="Gets the full path of the file.")>
    <Extension> Public Function GetFullPath(file As String) As String
        Return FileIO.FileSystem.GetFileInfo(file).FullName.Replace("\", "/")
    End Function

    ''' <summary>
    ''' Gets the full path of the specific directory.
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("Dir.FullPath", Info:="Gets the full path of the directory.")>
    <Extension> Public Function GetDirectoryFullPath(dir$, <CallerMemberName> Optional stack$ = Nothing) As String
        Try
            Return FileIO.FileSystem _
                .GetDirectoryInfo(dir) _
                .FullName _
                .Replace("\", "/")
        Catch ex As Exception
            stack = stack & " --> " & NameOf(GetDirectoryFullPath)

            If dir = "/" AndAlso Not App.IsMicrosoftPlatform Then
                Return "/"  ' Linux上面已经是全路径了，root
            Else
                ex = New Exception(stack & ": " & dir, ex)
                Call App.LogException(ex)
                Call ex.PrintException
                Return dir
            End If
        End Try
    End Function

    ''' <summary>
    ''' Removes the file extension name from the file path.(去除掉文件的拓展名)
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("File.Ext.Trim")>
    <Extension> Public Function TrimSuffix(file As String) As String
        Try
            Dim path$ = file.FixPath.TrimEnd("/"c, "\"c)
            Dim fileInfo = FileIO.FileSystem.GetFileInfo(path$)
            Dim Name As String = BaseName(fileInfo.FullName)
            Return $"{fileInfo.Directory.FullName}/{Name}"
        Catch ex As Exception
            ex = New Exception($"{NameOf(file)} --> {file}", ex)
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Removes the last \ and / character in a directory path string.
    ''' (使用这个函数修剪文件夹路径之中的最后一个分隔符，以方便生成文件名)
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <returns></returns>
    <Extension>
    Public Function TrimDIR(DIR As String) As String
        Return DIR.TrimEnd("/"c, "\"c)
    End Function

    ''' <summary>
    ''' 返回``文件名称.拓展名``
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <ExportAPI("File.Name")>
    <Extension>
    Public Function FileName(path As String) As String
        Return FileIO.FileSystem.GetFileInfo(path).Name
    End Function

    ''' <summary>
    ''' 进行安全的复制，出现错误不会导致应用程序崩溃，大文件不推荐使用这个函数进行复制
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="copyTo"></param>
    ''' <returns></returns>
    <ExportAPI("SafeCopyTo")>
    Public Function SafeCopyTo(source As String, copyTo As String) As Boolean
        Try
            Dim buf As Byte() = IO.File.ReadAllBytes(source)
            Call buf.FlushStream(copyTo)
        Catch ex As Exception
            Dim pt As String = $"{source.ToFileURL} ===> {copyTo.ToFileURL}"
            Call App.LogException(New Exception(pt, ex))
            Return False
        End Try

        Return True
    End Function
End Module
