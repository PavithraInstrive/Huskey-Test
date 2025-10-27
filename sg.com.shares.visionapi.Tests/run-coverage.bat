@echo off
powershell.exe -ExecutionPolicy Bypass -File "%~dp0RunTestsWithCoverage.ps1" -AllTests
pause
