@echo off
chcp 65001 >nul
cls
echo ╔════════════════════════════════════════════════════════╗
echo ║     ИНФОРМАЦИОННАЯ СИСТЕМА "СТУДЕНЧЕСКИЙ АКТИВ"       ║
echo ╚════════════════════════════════════════════════════════╝
echo.
echo Запуск приложения...
echo.

cd StudentActiveSystem\bin\Release\net6.0-windows

if exist StudentActiveSystem.exe (
    start StudentActiveSystem.exe
    echo Приложение успешно запущено!
) else (
    echo [ОШИБКА] Приложение не найдено!
    echo.
    echo Пожалуйста, сначала соберите проект используя файл Сборка.bat
    echo.
    pause
)
