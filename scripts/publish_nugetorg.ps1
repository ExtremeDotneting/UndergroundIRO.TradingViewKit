$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent

$ApiKey = Get-Content "$PSScriptRoot\localData\nuget_api_key.txt";
Write-Host "Your nuget.org api key is '$ApiKey'.`nIf you want to set new api key - please create file .\localData\nuget_api_key.txt where text - is your api key.";
Write-Host "Found files.";
Get-ChildItem "$PSScriptRoot\..\output\nuget" -Recurse -Filter "*.nupkg" | Foreach-Object { 
  $PackageName=$_.FullName;
  Write-Host "'$PackageName'";
}
Write-Host "Press enter to publish.";
pause;

Get-ChildItem "$PSScriptRoot\..\output\nuget" -Recurse -Filter "*.nupkg" | Foreach-Object { 
  $PackageName=$_.FullName;
  Write-Host "'$PackageName'";
  dotnet nuget push "$PackageName" --api-key $ApiKey -s https://api.nuget.org/v3/index.json
}
pause;