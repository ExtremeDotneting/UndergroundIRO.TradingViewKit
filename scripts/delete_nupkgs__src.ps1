$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
.\delete_nupkgs.cmd "$PSScriptRoot\..\src" 0