![Icon](https://raw.githubusercontent.com/adamralph/dude/master/img/dude.png)

# Dude

[![Build status](https://ci.appveyor.com/api/projects/status/acoj907kr0u9f2m7/branch/master?svg=true)](https://ci.appveyor.com/project/adamralph/dude/branch/master)

Have you ever wanted to get your hands on Microsoft's [C# REPL Command-Line Interface (csi.exe)](https://msdn.microsoft.com/en-us/magazine/mt614271.aspx) without having to install Visual Studio? Dude, look no further!

**Dude** is what you're looking for, and all in a single exe!

### Getting started

1. The Dude requires .NET Framework 4.6 or later.
2. Download [dude.exe](https://github.com/adamralph/dude/releases/download/stable/dude.exe).
3. (Optional) add the folder containing dude.exe to your PATH.

Dude is functionally identical to csi.exe. E.g. `dude.exe` starts a REPL session. `dude.exe hello.csx` runs a C# script. `dude.exe -?` shows help.

### How it works

Dude, [like csi.exe](https://github.com/dotnet/roslyn/blob/e045283767da82cfd276d020c8a798f78513a1ab/src/Interactive/CsiCore), is a tiny wrapper around various `Microsoft.CodeAnalysis` libraries. The only differences (apart from the name) are:

1. The invocation of the `Microsoft.CodeAnalysis` members that do all the the heavy lifting is via reflection. (Note that this should have negligible performance impact since the reflective calls only represent a small doorway into the code that does all the work.)
2. All the required assemblies are merged into a single exe using [ILRepack](https://github.com/gluck/il-repack).
3. The [default response file](https://github.com/dotnet/roslyn/blob/e045283767da82cfd276d020c8a798f78513a1ab/src/Interactive/csi/csi.rsp) is created alongside the exe the first time it is run.

---

<sub>[The Dude](https://thenounproject.com/term/the-dude/374112) by [Joan Andujar](https://thenounproject.com/joanandujar/) from [the Noun Project](https://thenounproject.com/).</sub>
