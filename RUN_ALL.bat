@echo off
title Let'sGoBiking - Full System Launcher

echo ============================================
echo   Starting Let'sGoBiking System...
echo ============================================
echo.

REM CHANGE WORKING DIRECTORY TO THE LOCATION OF THIS BAT FILE
cd /d "%~dp0"

REM ====== START PROXY CACHE SERVER ======
echo [1/4] Starting ProxyCacheServer...
start "ProxyCacheServer" "%~dp0ProxyCacheServer\bin\Debug\ProxyCacheServer.exe"
timeout /t 2 >nul

REM ====== START ROUTING SERVER ======
echo [2/4] Starting RoutingServer...
start "RoutingServer" "%~dp0RoutingServer\bin\Debug\RoutingServer.exe"
timeout /t 2 >nul

REM ====== START HEAVY CLIENT ======
echo [3/4] Starting Heavy Client...
start "HeavyClient" cmd /k "cd /d %~dp0heavyclient && mvn clean compile exec:java"
timeout /t 2 >nul

REM ====== START FRONTEND ======
echo [4/4] Opening Frontend...
start "" "%~dp0FrontendClient\index.html"

echo.
echo ============================================
echo   All components started successfully!
echo ============================================
pause
