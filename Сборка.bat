@echo off
cls
echo ========================================
echo   Building Student Active System
echo ========================================
echo.
echo Starting build process...
echo.

cd StudentActiveSystem

echo [1/3] Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo.
    echo ERROR: Failed to restore packages!
    echo Make sure .NET 6.0 SDK is installed
    pause
    exit /b 1
)

echo.
echo [2/3] Building project (Release)...
dotnet build -c Release
if %errorlevel% neq 0 (
    echo.
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo [3/3] Publishing application...
dotnet publish -c Release -r win-x64 --self-contained false
if %errorlevel% neq 0 (
    echo.
    echo ERROR: Publish failed!
    pause
    exit /b 1
)

cd ..

echo.
echo ========================================
echo   BUILD COMPLETED SUCCESSFULLY!
echo ========================================
echo.
echo Files location:
echo StudentActiveSystem\bin\Release\net6.0-windows\
echo.
echo To run the app use: Zapusk.bat
echo.
pause
