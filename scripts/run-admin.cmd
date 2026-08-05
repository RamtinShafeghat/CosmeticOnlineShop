@echo off
setlocal
cd /d "%~dp0..\admin"

where node >nul 2>&1
if errorlevel 1 (
  echo Node.js was not found. Install it from https://nodejs.org/ then open a new terminal.
  exit /b 1
)

if not exist node_modules (
  echo Installing admin dependencies...
  call npm install
  if errorlevel 1 exit /b 1
)

echo Starting Admin on http://localhost:4300 ...
call npm start
