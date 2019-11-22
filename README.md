# WindowsService-AI-Dependency-Failure
Illustrates dependency load exception when AI TelemetryClient.StartOperation method is called from a .NET 4.7.2 Windows Service.

#### Problem:
[MS Visual Studio Intaller Project](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2017InstallerProjects&ssr=false) adds incorrect assemblies to the installer package.


#### Exception thrown when starting the installed windows service:
> The type initializer for 'PerTypeValues1' threw an exception.

> System.BadImageFormatException: Could not load file or assembly System.Runtime.CompilerServices.Unsafe, Version=4.0.4.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a or one of its dependencies.

> Reference assemblies should not be loaded for execution. They can only be loaded in the Reflection-only loader context. (Exception from HRESULT: 0x80131058)


#### Discussion:
https://github.com/microsoft/ApplicationInsights-Home/issues/429


#### Solution:
Add the 4 following assemblies manually from project bin folder to the Application Folder in addition to default  "Project Output" in installer project:
<ol>
<li>System.Buffers</li>
<li>System.Memory</li>
<li>System.Numeric.Vectors</li>
<li>System.Runtime.CompilerServices.Unsafe</li>
</ol>


#### Possibly Related:
https://github.com/dotnet/standard/issues/250
