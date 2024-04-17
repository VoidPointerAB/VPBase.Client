
set /p AuthCode=Enter nuget server auth code: 

dotnet nuget push -s https://nuget2.voidpointer.se/v3/index.json -k %AuthCode% publish\VPBase.Client.1.0.1.nupkg

pause