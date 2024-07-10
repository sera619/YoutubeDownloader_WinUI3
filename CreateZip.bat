@echo off
setlocal enabledelayedexpansion

echo Generate zip Archive...

rem Set paths
set "sourcePath=%~dp0bin\win10-x64\publish"
set "zipFileName=YTDownloader_Win_x64.zip"
set "zipFilePath=%sourcePath%\%zipFileName%"
set "targetPath=%~dp0bin"

rem Attempt to delete old ZIP files
for %%F in ("%sourcePath%\YTDownloader_Win_x64.zip") do (
    del "%%F" 2>nul
)

rem Create archive with retry attempts
set "attempts=0"
:retry
set /a attempts+=1
echo Attempt %attempts% of 5...

powershell -Command "& {Compress-Archive -Path '%sourcePath%\*' -DestinationPath '%zipFilePath%' -Force}"

if %errorlevel% neq 0 (
    if %attempts% lss 5 (
        echo Error creating archive. Waiting 5 seconds and retrying...
        timeout /t 5 >nul
        goto retry
    ) else (
        echo All attempts failed. Archive could not be created.
        exit /b 1
    )
) else (
    echo Archive completed: %zipFilePath%
    
    rem Move the ZIP file to the target directory
    move "%zipFilePath%" "%targetPath%"
    if errorlevel 1 (
        echo Error moving ZIP file to %targetPath%.
        exit /b 1
    ) else (
        echo Archive successfully moved to %targetPath%.
    )
)

if not defined CI pause
