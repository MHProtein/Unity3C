@ECHO OFF
ECHO Have you updated the package version by a notch? Confirm with enter
PAUSE >NUL
DEL *.nupkg
CD ..
DEL /Q bin\release\*
C:\WINDOWS\MICROSOFT.NET\Framework\v4.0.30319\MSBuild.exe Monocle.net35.csproj /p:Configuration=Release
MKDIR nu\lib\net35
DEL /Q nu\lib\net35\*
COPY bin\Release\Monocle.dll nu\lib\net35
C:\WINDOWS\MICROSOFT.NET\Framework\v4.0.30319\MSBuild.exe Monocle.net40.csproj /p:Configuration=Release
MKDIR nu\lib\net40
DEL /Q nu\lib\net40\*
COPY bin\Release\Monocle.dll nu\lib\net40
CD nu
Nuget.exe Pack Monocle.nuspec
ECHO Now write: Nuget push Monocle-version.nupkg
PAUSE >NUL
