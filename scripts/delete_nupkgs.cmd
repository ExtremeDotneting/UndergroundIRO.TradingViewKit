@echo off
set SearchDir=%1
set PausesDisabled=%2

echo Input params: {
echo   SearchDir: %SearchDir%
echo   PausesDisabled: %PausesDisabled%
echo }
echo Press to continue.
IF NOT "%PausesDisabled%"=="1" (    
  pause
)

echo Found files:
FORFILES /P %SearchDir% /M "*.nupkg" /S /C "cmd /c echo @path"
echo Will delete.
IF NOT "%PausesDisabled%"=="1" (    
  pause
)
FORFILES /P %SearchDir% /M "*.nupkg" /S /C "cmd /c del @path "
echo Deleted!

