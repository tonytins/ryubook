if($IsMacOS)
{
    Write-Output "Publishing macOS release."
    dotnet publish -c Release -r osx-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
}
elseif ($IsLinux)
{
    Write-Output "Publishing Linux release."
    dotnet publish -c Release -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
}
else
{
    Write-Output "Publishing Windows release."
    dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
}
