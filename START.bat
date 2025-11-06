@echo off
cd StudentActiveSystem\bin\Release\net6.0-windows
if exist StudentActiveSystem.exe (
    start StudentActiveSystem.exe
) else (
    echo ERROR: Application not found! Run Sborka.bat first.
    pause
)
