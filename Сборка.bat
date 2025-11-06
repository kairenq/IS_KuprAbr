@echo off
chcp 65001 >nul
cls
echo ╔════════════════════════════════════════════════════════╗
echo ║          СБОРКА ПРОЕКТА "СТУДЕНЧЕСКИЙ АКТИВ"          ║
echo ╚════════════════════════════════════════════════════════╝
echo.
echo Начинаем сборку проекта...
echo.

cd StudentActiveSystem

echo [1/3] Восстановление пакетов NuGet...
dotnet restore
if %errorlevel% neq 0 (
    echo.
    echo [ОШИБКА] Не удалось восстановить пакеты!
    echo Убедитесь, что установлен .NET 6.0 SDK
    pause
    exit /b 1
)

echo.
echo [2/3] Сборка проекта (Release)...
dotnet build -c Release
if %errorlevel% neq 0 (
    echo.
    echo [ОШИБКА] Сборка завершилась с ошибками!
    pause
    exit /b 1
)

echo.
echo [3/3] Публикация приложения...
dotnet publish -c Release -r win-x64 --self-contained false
if %errorlevel% neq 0 (
    echo.
    echo [ОШИБКА] Публикация завершилась с ошибками!
    pause
    exit /b 1
)

cd ..

echo.
echo ╔════════════════════════════════════════════════════════╗
echo ║              СБОРКА ЗАВЕРШЕНА УСПЕШНО!                ║
echo ╚════════════════════════════════════════════════════════╝
echo.
echo Файлы находятся в папке:
echo StudentActiveSystem\bin\Release\net6.0-windows\
echo.
echo Для запуска используйте файл Запуск.bat
echo.
pause
