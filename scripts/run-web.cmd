@echo off
setlocal
cd /d "%~dp0..\frontend"

where node >nul 2>&1
if errorlevel 1 (
  echo Node.js was not found. Install it from https://nodejs.org/ then open a new terminal.
  exit /b 1
)

if not exist node_modules (
  echo Installing frontend dependencies...
  call npm install
  if errorlevel 1 exit /b 1
)

echo Starting Angular on http://localhost:4200 ...
call npm start
