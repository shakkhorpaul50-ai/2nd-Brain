@echo off
setlocal

rem Copy this file and Start-Local-Code-AI.ps1 to the root of the USB drive.
rem The PowerShell script starts llama-server and the Local Code AI application.
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0Start-Local-Code-AI.ps1"

if errorlevel 1 (
    echo.
    echo Local Code AI could not start. Read the message above and check the USB setup.
    pause
)
