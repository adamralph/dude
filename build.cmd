:: the windows shell, so amazing

:: options
@echo Off
cd %~dp0
setlocal

:: determine cache dir
set NUGET_CACHE_DIR=%LocalAppData%\NuGet





:: download nuget to cache dir
set NUGET_URL="https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
if not exist %NUGET_CACHE_DIR%\NuGet.exe (
  if not exist %NUGET_CACHE_DIR% md %NUGET_CACHE_DIR%
  echo Downloading latest version of NuGet.exe...
  @powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest '%NUGET_URL%' -OutFile '%NUGET_CACHE_DIR%\NuGet.exe'"
)

:: copy nuget locally
if not exist .nuget\NuGet.exe (
  if not exist .nuget md .nuget
  copy %NUGET_CACHE_DIR%\NuGet.exe .nuget\NuGet.exe > nul
)

:: restore packages
.nuget\NuGet.exe restore Dude.sln

:: build solution
if not exist artifacts mkdir artifacts
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" Dude.sln /property:Configuration=Release /nologo /maxcpucount /verbosity:minimal /fileLogger /fileloggerparameters:LogFile=artifacts\msbuild.log;Verbosity=normal;Summary /nodeReuse:false %*

:: run tests
packages\xunit.runner.console.2.1.0\tools\xunit.console.exe tests\DudeTests.Acceptance\bin\Release\DudeTests.Acceptance.dll -xml artifacts\DudeTests.Acceptance.xml -html artifacts\DudeTests.Acceptance.html
