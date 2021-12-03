# Introduction to Web Templates

Project templates produce ready-to-run projects that make it easy for users to start with a working set of code. .NET includes a few project templates such as a console application or a class library. 

## How to setup a project template via dotnet core cli

1. create a project
2. add file .template.config->template.json into the project
3. repeate 1 & 2 steps to add more templates as needed
4. add all of them into the template project
5. test
6. pack it as nuget package
7. install the package



## Introduce templates into dotnet CLI Tools

###### Overview

DNV.SolutionPackage.WebTemplate.Base



DNV.SolutionPackage.WebTemplate.Base.plus



DNV.SolutionPackage.WebTemplate.Mgmt





###### Install the template package

There are two methods to install SolutionPackage project templates. One is to install it directly from Nuget package source. Other is to install from local file.

```bash
dotnet new -i DNV.SolutionPackage.ProjectTemplates::2.0.26 --nuget-source "https://dnvgl-one.pkgs.visualstudio.com/_packaging/AssuranceApplicationsChina%40Local/nuget/v3/index.json"

dotnet new -i C:\Temp\DNV.SolutionPackage.ProjectTemplates.2.0.26.nupkg
```

After the template package installation,  DNV.SolutionPackages in the templates list. You also can use `dotnet new --list` command to list the detail which templates have installed.

```bash
Template Name                                 Short Name              Language    Tags
--------------------------------------------  ----------------------  ----------  ----------------------
Console Application                           console                 [C#],F#,VB  Common/Console
Class library                                 classlib                [C#],F#,VB  Common/Library
WPF Application                               wpf                     [C#],VB     Common/WPF
WPF Class library                             wpflib                  [C#],VB     Common/WPF
WPF Custom Control Library                    wpfcustomcontrollib     [C#],VB     Common/WPF
WPF User Control Library                      wpfusercontrollib       [C#],VB     Common/WPF
Windows Forms App                             winforms                [C#],VB     Common/WinForms
Windows Forms Control Library                 winformscontrollib      [C#],VB     Common/WinForms
Windows Forms Class Library                   winformslib             [C#],VB     Common/WinForms
Worker Service                                worker                  [C#],F#     Common/Worker/Web
MSTest Test Project                           mstest                  [C#],F#,VB  Test/MSTest
NUnit 3 Test Project                          nunit                   [C#],F#,VB  Test/NUnit
NUnit 3 Test Item                             nunit-test              [C#],F#,VB  Test/NUnit
xUnit Test Project                            xunit                   [C#],F#,VB  Test/xUnit
Razor Component                               razorcomponent          [C#]        Web/ASP.NET
Razor Page                                    page                    [C#]        Web/ASP.NET
MVC ViewImports                               viewimports             [C#]        Web/ASP.NET
MVC ViewStart                                 viewstart               [C#]        Web/ASP.NET
Blazor Server App                             blazorserver            [C#]        Web/Blazor
Blazor WebAssembly App                        blazorwasm              [C#]        Web/Blazor/WebAssembly
ASP.NET Core Empty                            web                     [C#],F#     Web/Empty
ASP.NET Core Web App (Model-View-Controller)  mvc                     [C#],F#     Web/MVC
ASP.NET Core Web App                          webapp                  [C#]        Web/MVC/Razor Pages
ASP.NET Core with Angular                     angular                 [C#]        Web/MVC/SPA
ASP.NET Core with React.js                    react                   [C#]        Web/MVC/SPA
ASP.NET Core with React.js and Redux          reactredux              [C#]        Web/MVC/SPA
Razor Class Library                           razorclasslib           [C#]        Web/Razor/Library
ASP.NET Core Web API                          webapi                  [C#],F#     Web/WebAPI
ASP.NET Core gRPC Service                     grpc                    [C#]        Web/gRPC
DNV.SolutionPacakge.WebApi                    DNVWebApi                           Common
DNV.SolutionPacakge.WebApp                    DNVWebApp                           Common
dotnet gitignore file                         gitignore                           Config
global.json file                              globaljson                          Config
NuGet Config                                  nugetconfig                         Config
Dotnet local tool manifest file               tool-manifest                       Config
Web Config                                    webconfig                           Config
DNV.SolutionPacakge.Solution1                 DNVSolution1                        DNV
DNV.SolutionPacakge.WebTemplate.Base.plus     DNVWebTemplateBasePlus              DNV
DNV.SolutionPacakge.WebTemplate.Base          DNVWebTemplateBase                  DNV
DNV.SolutionPacakge.WebTemplate.Mgmt          DNVWebTemplateMgmt                  DNV
Solution File                                 sln                                 Solution
Protocol Buffer File                          proto                               Web/gRPC
```



###### List the templates

```bash
dotnet new --list
```

There are some built-in template packages, also you see our Solution Package project templates.

###### Uninstall the template package

```bash
dotnet new --uninstall

dotnet new -u DNV.SolutionPackage.ProjectTemplates
```



### Create project via Solution Package project templates

The `dotnet new` command create project from DNV.SolutionPackage.WebTemplate.Base template

```bash
dotnet new DNVWebTemplateBase --name Deme.Website
```

```bash
C:\temp\work>dotnet new DNVWebTemplateBase --name Deme.Website
The template "DNV.SolutionPacakge.WebTemplate.Base" was created successfully.

Processing post-creation actions...
No Primary Outputs to restore.
```

![](C:\workspace\repos\DNVGL.SolutionPackage\docfx_project\images\WebTemplates\Demo.Website.PNG)
