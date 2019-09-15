@echo off
set SearchDir=%1
set OutputDir=%2
rem Must be 1 or 0
set IsRelease=%3
set PausesDisabled=%4

echo Input params: {
echo   SearchDir: %SearchDir%
echo   OutputDir: %OutputDir%
echo   IsRelease: %IsRelease%
echo   PausesDisabled: %PausesDisabled%
echo }
echo Press to continue.
IF NOT "%PausesDisabled%"=="1" (    
  pause
)

echo Found files:
FORFILES /P %SearchDir% /M "*.nupkg" /S /C "cmd /c echo @path"
echo Will copy all .nupkg to output dir.
IF NOT "%PausesDisabled%"=="1" (    
  pause
)
IF "%IsRelease%"=="1" (
    echo "Release. Copy with replacement."
	FORFILES /P %SearchDir% /M "*.nupkg" /S /C "cmd /c robocopy @path/.. %OutputDir% @file /IS  /NJH /NJS /nc /ns /np"
) ELSE (
    echo "Debug. Copy without replacement."	
	FORFILES /P %SearchDir% /M "*.nupkg" /S /C "cmd /c robocopy @path/.. %OutputDir% @file /XF /XN /XO  /NJH /NJS /nc /ns /np"
)
echo Copied!

