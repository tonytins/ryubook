if($IsMacOS)
{
    echo "Publishing macOS release."
    dotnet publish -c Release -r osx-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
}
elseif ($IsLinux)
{
    echo "Publishing Linux release."
    dotnet publish -c Release -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
}
else
{
    echo "Publishing Windows release."
    dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
}