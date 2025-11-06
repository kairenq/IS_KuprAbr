@echo off
cls
echo Starting Student Active System...
echo.

cd StudentActiveSystem\bin\Release\net6.0-windows

if exist StudentActiveSystem.exe (
    start StudentActiveSystem.exe
    echo Application started successfully!
) else (
    echo ERROR: Application not found!
    echo.
    echo Please build the project first using Sborka.bat
    echo.
    pause
)
