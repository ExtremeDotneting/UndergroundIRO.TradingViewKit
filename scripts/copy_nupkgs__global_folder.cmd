@echo off
set /p NugetDirPath=<localData\local_nuget_path.txt
echo Path of yor local nuget dir: '%NugetDirPath%'
echo Warning! 
echo Don`t continue if it`s empty. 
echo You must create file 'local_nuget_path.txt' and paste path to nuget there.
pause
call copy_nupkgs ..\output\nuget %NugetDirPath% 1 0
pause