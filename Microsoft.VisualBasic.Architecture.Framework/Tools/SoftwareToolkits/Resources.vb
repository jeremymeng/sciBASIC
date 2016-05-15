﻿Imports System.ComponentModel.Composition
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices

Namespace SoftwareToolkits

    <Export(GetType(Global.System.Resources.ResourceManager))>
    Public Class Resources

        Public ReadOnly Property FileName As String
        Public ReadOnly Property Resources As Global.System.Resources.ResourceManager

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     The name parameter is null.
        '
        '   T:System.Resources.MissingManifestResourceException:
        '     No usable set of localized resources has been found, and there are no default
        '     culture resources. For information about how to handle this exception, see the
        '     "Handling MissingManifestResourceException and MissingSatelliteAssemblyException
        '     Exceptions" section in the System.Resources.ResourceManager class topic.
        '
        '   T:System.Resources.MissingSatelliteAssemblyException:
        '     The default culture's resources reside in a satellite assembly that could not
        '     be found. For information about how to handle this exception, see the "Handling
        '     MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions"
        '     section in the System.Resources.ResourceManager class topic.

        ''' <summary>
        ''' Returns the value of the specified non-string resource.
        ''' </summary>
        ''' <param name="name">The name of the resource to get.</param>
        ''' <returns>The value of the resource localized for the caller's current culture settings.
        ''' If an appropriate resource set exists but name cannot be found, the method returns
        ''' null.</returns>
        Public Overridable Function GetObject(name As String) As Object
            Return Resources.GetObject(name)
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     The name parameter is null.
        '
        '   T:System.Resources.MissingManifestResourceException:
        '     No usable set of resources have been found, and there are no default culture
        '     resources. For information about how to handle this exception, see the "Handling
        '     MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions"
        '     section in the System.Resources.ResourceManager class topic.
        '
        '   T:System.Resources.MissingSatelliteAssemblyException:
        '     The default culture's resources reside in a satellite assembly that could not
        '     be found. For information about how to handle this exception, see the "Handling
        '     MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions"
        '     section in the System.Resources.ResourceManager class topic.
        ''' <summary>
        ''' Gets the value of the specified non-string resource localized for the specified
        ''' culture.
        ''' </summary>
        ''' <param name="name">The name of the resource to get.</param>
        ''' <param name="culture">The culture for which the resource is localized. If the resource is not localized
        ''' for this culture, the resource manager uses fallback rules to locate an appropriate
        ''' resource.If this value is null, the System.Globalization.CultureInfo object is
        ''' obtained by using the System.Globalization.CultureInfo.CurrentUICulture property.</param>
        ''' <returns>The value of the resource, localized for the specified culture. If an appropriate
        ''' resource set exists but name cannot be found, the method returns null.</returns>
        Public Overridable Function GetObject(name As String, culture As CultureInfo) As Object
            Return Resources.GetObject(name, culture)
        End Function

        ' Exceptions:
        '   T:System.InvalidOperationException:
        '     The value of the specified resource is not a System.IO.MemoryStream object.
        '
        '   T:System.ArgumentNullException:
        '     name is null.
        '
        '   T:System.Resources.MissingManifestResourceException:
        '     No usable set of resources is found, and there are no default resources. For
        '     information about how to handle this exception, see the "Handling MissingManifestResourceException
        '     and MissingSatelliteAssemblyException Exceptions" section in the System.Resources.ResourceManager
        '     class topic.
        '
        '   T:System.Resources.MissingSatelliteAssemblyException:
        '     The default culture's resources reside in a satellite assembly that could not
        '     be found. For information about how to handle this exception, see the "Handling
        '     MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions"
        '     section in the System.Resources.ResourceManager class topic.
        ''' <summary>
        ''' Returns an unmanaged memory stream object from the specified resource.
        ''' </summary>
        ''' <param name="name">The name of a resource.</param>
        ''' <returns>An unmanaged memory stream object that represents a resource .</returns>
        <ComVisible(False)>
        Public Function GetStream(name As String) As UnmanagedMemoryStream
            Return Resources.GetStream(name)
        End Function

        '
        ' Exceptions:
        '   T:System.InvalidOperationException:
        '     The value of the specified resource is not a System.IO.MemoryStream object.
        '
        '   T:System.ArgumentNullException:
        '     name is null.
        '
        '   T:System.Resources.MissingManifestResourceException:
        '     No usable set of resources is found, and there are no default resources. For
        '     information about how to handle this exception, see the "Handling MissingManifestResourceException
        '     and MissingSatelliteAssemblyException Exceptions" section in the System.Resources.ResourceManager
        '     class topic.
        '
        '   T:System.Resources.MissingSatelliteAssemblyException:
        '     The default culture's resources reside in a satellite assembly that could not
        '     be found. For information about how to handle this exception, see the "Handling
        '     MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions"
        '     section in the System.Resources.ResourceManager class topic.
        ''' <summary>
        ''' Returns an unmanaged memory stream object from the specified resource, using
        ''' the specified culture.
        ''' </summary>
        ''' <param name="name">The name of a resource.</param>
        ''' <param name="culture">An object that specifies the culture to use for the resource lookup. If culture
        ''' is null, the culture for the current thread is used.</param>
        ''' <returns>An unmanaged memory stream object that represents a resource.</returns>
        <ComVisible(False)>
        Public Function GetStream(name As String, culture As CultureInfo) As UnmanagedMemoryStream
            Return Resources.GetStream(name, culture)
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     The name parameter is null.
        '
        '   T:System.InvalidOperationException:
        '     The value of the specified resource is not a string.
        '
        '   T:System.Resources.MissingManifestResourceException:
        '     No usable set of resources has been found, and there are no resources for the
        '     default culture. For information about how to handle this exception, see the
        '     "Handling MissingManifestResourceException and MissingSatelliteAssemblyException
        '     Exceptions" section in the System.Resources.ResourceManager class topic.
        '
        '   T:System.Resources.MissingSatelliteAssemblyException:
        '     The default culture's resources reside in a satellite assembly that could not
        '     be found. For information about how to handle this exception, see the "Handling
        '     MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions"
        '     section in the System.Resources.ResourceManager class topic.
        ''' <summary>
        ''' Returns the value of the specified string resource.
        ''' </summary>
        ''' <param name="name">The name of the resource to retrieve.</param>
        ''' <returns>The value of the resource localized for the caller's current UI culture, or null
        ''' if name cannot be found in a resource set.</returns>
        Public Overridable Function GetString(name As String) As String
            Return Resources.GetString(name)
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     The name parameter is null.
        '
        '   T:System.InvalidOperationException:
        '     The value of the specified resource is not a string.
        '
        '   T:System.Resources.MissingManifestResourceException:
        '     No usable set of resources has been found, and there are no resources for a default
        '     culture. For information about how to handle this exception, see the "Handling
        '     MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions"
        '     section in the System.Resources.ResourceManager class topic.
        '
        '   T:System.Resources.MissingSatelliteAssemblyException:
        '     The default culture's resources reside in a satellite assembly that could not
        '     be found. For information about how to handle this exception, see the "Handling
        '     MissingManifestResourceException and MissingSatelliteAssemblyException Exceptions"
        '     section in the System.Resources.ResourceManager class topic.
        ''' <summary>
        ''' Returns the value of the string resource localized for the specified culture.
        ''' </summary>
        ''' <param name="name">The name of the resource to retrieve.</param>
        ''' <param name="culture">An object that represents the culture for which the resource is localized.</param>
        ''' <returns>The value of the resource localized for the specified culture, or null if name
        ''' cannot be found in a resource set.</returns>
        Public Overridable Function GetString(name As String, culture As CultureInfo) As String
            Return Resources.GetString(name, culture)
        End Function

        Sub New()
            Call Me.New(Assembly.GetExecutingAssembly)
        End Sub

        Sub New(type As Type)
            Call Me.New(type.Assembly)
        End Sub

        ''' <summary>
        ''' 默认是<see cref="App.HOME"/>/Resources/assmFile
        ''' </summary>
        ''' <param name="assm"></param>
        Sub New(assm As Assembly)
            FileName = assm.Location.ParentPath & "/Resources/" & FileIO.FileSystem.GetFileInfo(assm.Location).Name
            Call __resParser()
        End Sub

        Sub New(dll As String)
            FileName = FileIO.FileSystem.GetFileInfo(dll).FullName
            Call __resParser()
        End Sub

        Private Sub __resParser()
            Call __load(Assembly.LoadFile(FileName))
        End Sub

        Private Sub __load(assm As Assembly)
#If NET_40 = 0 Then
            Dim __resEXPORT As Type = GetType(ExportAttribute)
            Dim typeDef As Type = (From type As Type
                                   In assm.GetTypes
                                   Let exp As ExportAttribute = type.GetCustomAttribute(__resEXPORT)
                                   Where Not exp Is Nothing AndAlso
                                       exp.ContractType.Equals(GetType(Global.System.Resources.ResourceManager))
                                   Select type).FirstOrDefault
            If Not typeDef Is Nothing Then
                Dim myRes As PropertyInfo = (From prop As PropertyInfo
                                             In typeDef.GetProperties(BindingFlags.Public Or BindingFlags.Static)
                                             Let exp As ExportAttribute = prop.GetCustomAttribute(__resEXPORT)
                                             Where prop.CanRead AndAlso
                                                 Not exp Is Nothing AndAlso
                                                 exp.ContractType.Equals(GetType(Global.System.Resources.ResourceManager))
                                             Select prop).FirstOrDefault
                If Not myRes Is Nothing Then
                    Dim value As Object = myRes.GetValue(Nothing, Nothing)
                    _Resources = DirectCast(value, Global.System.Resources.ResourceManager)
                End If
            End If
#End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="my">null</param>
        ''' <param name="assm"></param>
        Private Sub New(my As Type, assm As Assembly)
            Call __load(assm)
        End Sub

        ''' <summary>
        ''' 从自身的程序集之中加载数据
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function LoadMy() As Resources
            Return New Resources(Nothing, Assembly.GetExecutingAssembly)
        End Function

        Public Shared Function DirectLoadFrom(assm As Assembly) As Resources
            Return New Resources(Nothing, assm)
        End Function

        ''' <summary>
        ''' Returns the cached ResourceManager instance used by this class.
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Export(GetType(Global.System.Resources.ResourceManager))>
        Public Shared ReadOnly Property MyResource As Global.System.Resources.ResourceManager
            Get
                Return My.Resources.ResourceManager
            End Get
        End Property
    End Class
End Namespace