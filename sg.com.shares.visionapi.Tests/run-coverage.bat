@echo off
echo Running coverage from: %CD%
cd /d "%~dp0"
echo Now in directory: %CD%
powershell.exe -ExecutionPolicy Bypass -File "%~dp0RunTestsWithCoverage.ps1" -AllTests
echo Script finished with exit code: %ERRORLEVEL%
exit /b %ERRORLEVEL%
