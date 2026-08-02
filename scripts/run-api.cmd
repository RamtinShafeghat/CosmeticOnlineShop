@echo off
setlocal
cd /d "%~dp0..\backend\CosmeticShop.Api"

where dotnet >nul 2>&1
if errorlevel 1 (
  echo .NET SDK was not found. Install .NET 8 from https://dotnet.microsoft.com/download/dotnet/8.0
  echo Then open a new terminal and try again.
  exit /b 1
)

echo Starting API on http://localhost:5041 ...
dotnet run --launch-profile http
