[CmdletBinding()]
Param(
    #[switch]$CustomParam,
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [int]$NotSilent,
	[int]$IsRelease,
	[string]$NugetsOutputDir
)

###########################################################################
# BASE FUNCTIONS
###########################################################################

function Write-Color([String[]]$Text, [ConsoleColor[]]$Color = "White", [int]$StartTab = 0, [int] $LinesBefore = 0,[int] $LinesAfter = 0, [string] $LogFile = "", $TimeFormat = "yyyy-MM-dd HH:mm:ss") {
    $DefaultColor = $Color[0]
    if ($LinesBefore -ne 0) {  for ($i = 0; $i -lt $LinesBefore; $i++) { Write-Host "`n" -NoNewline } } # Add empty line before
    if ($StartTab -ne 0) {  for ($i = 0; $i -lt $StartTab; $i++) { Write-Host "`t" -NoNewLine } }  # Add TABS before text
    if ($Color.Count -ge $Text.Count) {
        for ($i = 0; $i -lt $Text.Length; $i++) { Write-Host $Text[$i] -ForegroundColor $Color[$i] -NoNewLine } 
    } else {
        for ($i = 0; $i -lt $Color.Length ; $i++) { Write-Host $Text[$i] -ForegroundColor $Color[$i] -NoNewLine }
        for ($i = $Color.Length; $i -lt $Text.Length; $i++) { Write-Host $Text[$i] -ForegroundColor $DefaultColor -NoNewLine }
    }
    Write-Host
    if ($LinesAfter -ne 0) {  for ($i = 0; $i -lt $LinesAfter; $i++) { Write-Host "`n" } }  # Add empty line after
    if ($LogFile -ne "") {
        $TextToFile = ""
        for ($i = 0; $i -lt $Text.Length; $i++) {
            $TextToFile += $Text[$i]
        }
        Write-Output "[$([datetime]::Now.ToString($TimeFormat))]$TextToFile" | Out-File $LogFile -Encoding unicode -Append
    }
}

function SPause{
 if($NotSilent -eq 1){
   pause
 }
}

function CustomWrite([String[]]$Text, [ConsoleColor[]]$Color = "White"){
  $Text=">>>>> "+$Text;
  Write-Color -Text $Text -Color $Color;
}

function WriteOperationResultByExitCode($text, $exitCode)
{    
  if($exitCode)
  {
    $text=$text+" Failed";
    CustomWrite -Text $text -Color Red;
  }
  else
  {    
	$text=$text+" Successful";	
    CustomWrite -Text $text -Color Green;	
  }
  Write-Host "";
}

function ReadBool($hint)
{
  $hint=$hint+" y/n (y) ";
  if($Silent -eq 1)
  {
    Write-Host $hint;
    return 1;
  }
  $answer = Read-Host $hint
  if($answer -eq 'n'){
    return 0;
  }
  return 1;
}

###########################################################################
# CONFIGURATION
###########################################################################

$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
# Yours can be in another folder.
$MSBuildExe = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"

function DotnetBuildInfo()
{     
  # dotnet build $path --configuration $Configuration
  $exitCode=$lastexitcode;
  Write-Host "Builded in " + $Configuration + " mode.";
  WriteOperationResultByExitCode "Dotnet build status: " $exitCode;
  return $exitCode;
}

###########################################################################
# HANDLE STARTUP ARGUMENTS
###########################################################################

Write-Color -Text "Script path: $PSScriptRoot\build.ps1" -Color Green;

# Set configuration
if($IsRelease -eq 1)
{
  $IsRelease=1;
  $Configuration="Release";
}
else
{
  $IsRelease=0;
  $Configuration="Debug";
}
Write-Color -Text "Configuration: $Configuration" -Color Green;
  
# Set mode.
if($NotSilent -eq 1){
  $Silent=0;
  $NotSilent=1;
}
else{  
  $Silent=1;
  $NotSilent=0;
}
Write-Color -Text "Is silent: $Silent"  -Color Green;

if(-not $NugetsOutputDir)
{
  $NugetsOutputDir="$PSScriptRoot\output\nuget";
}
Write-Color -Text "Nugets output directory: $NugetsOutputDir" -Color Green;
SPause;

###########################################################################
# BUILD SUBMODULES
###########################################################################

# Executing same ps1 files with current parameters.
$FirstIter=$TRUE;

Get-ChildItem "$PSScriptRoot\submodules" -Recurse -Filter "build.ps1" | Foreach-Object { 
  if($FirstIter){
    Write-Color -Text "Building submodules: " -Green;
    $FirstIter=$FALSE;
  }
  $SubmoduleBuild=$_.FullName;
  & $SubmoduleBuild -NotSilent $NotSilent -IsRelease $IsRelease -NugetsOutputDir $NugetsOutputDir
}
if(-not $FirstIter){
  Write-Color -Text "Finished.";
  SPause;
}

###########################################################################
# EXECUTION
###########################################################################

# Remove default nuget output dir.
$DefaultNugetsOutputDir="$PSScriptRoot\output\nuget";
$WantClearOutputNuget = ReadBool "Want clear '$DefaultNugetsOutputDir' before build? "
if($WantClearOutputNuget){
  if(Test-Path -Path $DefaultNugetsOutputDir){
    rd $DefaultNugetsOutputDir -recurse;  	
  }  
  Write-Host "Removed."	  
}


# Remove old nupkg from src folder.
Write-Host "Will remove old nupkg from src folder.";
SPause;
& "$PSScriptRoot\scripts\delete_nupkgs.cmd" "$PSScriptRoot\src" 1
CustomWrite "Removed .nupkg files.`n" Green;
SPause;  

# Build
$BuildExitCode="";
Get-ChildItem "$PSScriptRoot" -Filter "*.sln" | Foreach-Object {    
  $SlnPath=$_.FullName;
  Write-Host "Building solution: " $SlnPath;
  & $MSBuildExe /t:restore $SlnPath /clp:ErrorsOnly -m
  & $MSBuildExe /t:build $SlnPath /p:Configuration=$Configuration /p:GenerateDocumentation=true /p:GeneratePackageOnBuild=true /clp:ErrorsOnly -m
  $BuildExitCode=$lastexitcode;
  WriteOperationResultByExitCode "Solution build status: " $lastexitcode
  SPause;
# Delete tests packages.
  Get-ChildItem "$PSScriptRoot\output\nuget" -Recurse -Filter "*IRO*Tests*" | 
  Foreach-Object {    
    Write-Host "Delete tests package: " $_.FullName;
    Remove-Item –path $_.FullName;
  }
  SPause;
}

# Tests
$WantSkipUnitTests = ReadBool "Want skip unit tests? "
if(-not $WantSkipUnitTests){
  $testRes=0;
  Get-ChildItem "$PSScriptRoot" -Recurse -Filter "*UnitTest*.csproj" | 
  Foreach-Object {    
    dotnet test $_.FullName --configuration $Configuration --verbosity  m
	if($lastexitcode){
	  $testRes=1;
	}
  }
  WriteOperationResultByExitCode "Tests execution status: " $testRes;
  SPause;
}

# Copy nugets to output/nuget
& "$PSScriptRoot\scripts\copy_nupkgs.cmd" "$PSScriptRoot\src\" $NugetsOutputDir $IsRelease 1
CustomWrite "Copied." Green
pause
exit $BuildExitCode;