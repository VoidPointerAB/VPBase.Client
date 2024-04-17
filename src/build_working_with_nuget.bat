@echo off

echo.
echo "Remove old dist folder"
rmdir /q /s "dist"

echo "Remove old nuget lib folder"
rmdir /q /s "nuget\netstandard1.2\lib"
rmdir /q /s "nuget\netstandard2.0\lib"

rmdir /q /s "VPBase.Client\bin"
rmdir /q /s "VPBase.Client\obj"

set tempdate=%DATE:/=%
set temptime=%TIME::=%
set temptime=%temptime: =0%

for /f %%i in ('git rev-parse --short HEAD') do set currgit=%%i

SET dirname="%tempdate:~0,4%%tempdate:~5,2%%tempdate:~8,2%.%temptime:~0,4%.%currgit%.contract"

mkdir dist
mkdir dist\\%dirname%

dotnet publish VPBase.Client\VPBase.Client.csproj /p:PublishProfile=Release /p:EnvironmentName=Production -o dist\\%dirname%

xcopy /Y "dist\%dirname%\VPBase.Client.dll" "nuget\lib\netstandard2.0\VPBase.Client.dll*"

pause




