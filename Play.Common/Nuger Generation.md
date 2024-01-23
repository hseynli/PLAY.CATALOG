## For generating nuget file use this code:
dotnet pack -o ..\..\..\packages

dotnet pack -p:PackageVersion=1.0.2 -o ..\..\..\packages

## Add nuget source
dotnet nuget add source D:packages -n PlayEconomy