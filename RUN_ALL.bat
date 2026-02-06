@echo off
title Let'sGoBiking - Full System Launcher

echo ============================================
echo   Starting Let'sGoBiking System...
echo ============================================
echo.

REM CHANGE WORKING DIRECTORY TO THE LOCATION OF THIS BAT FILE
cd /d "%~dp0"

REM ====== START PROXY CACHE SERVER ======
echo [1/5] Starting ProxyCacheServer...
start "ProxyCacheServer" "%~dp0ProxyCacheServer\bin\Debug\ProxyCacheServer.exe"
timeout /t 2 >nul

REM ====== START ROUTING SERVER ======
echo [2/5] Starting RoutingServer...
start "RoutingServer" "%~dp0RoutingServer\bin\Debug\RoutingServer.exe"
timeout /t 2 >nul

REM ====== START HEAVY CLIENT ======
echo [3/5] Starting Heavy Client...
start "HeavyClient" cmd /k "cd /d %~dp0heavyclient && mvn clean compile exec:java"
timeout /t 2 >nul

REM ====== START LLM SERVICE ======
echo [4/5] Starting LLM Service...
start "LLMService" cmd /k "cd /d %~dp0LLMService && python main.py"
timeout /t 3 >nul

REM ====== START FRONTEND ======
echo [5/5] Opening Frontend...
start "" "%~dp0FrontendClient\index.html"

echo.
echo ============================================
echo   All components started successfully!
echo ============================================
pause
