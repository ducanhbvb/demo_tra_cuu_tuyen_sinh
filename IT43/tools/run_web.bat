@echo off
title Tra Cuu Tuyen Sinh - Local Server

:: Lay duong dan tuyet doi cua thu muc project (cap tren tools/)
pushd "%~dp0.."
set "PROJECT_DIR=%CD%"
popd

echo =======================================================
echo  TRA CUU TUYEN SINH - BUILD + CHAY WEB
echo  Thu muc: %PROJECT_DIR%
echo =======================================================

:: ---- BUOC 1: BUILD ----
echo.
echo [1/2] Dang build project...

set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"

if not exist "%MSBUILD%" (
    echo [LOI] Khong tim thay MSBuild! Hay cai Visual Studio 2022.
    pause
    exit /b 1
)

"%MSBUILD%" "%PROJECT_DIR%\TraCuuTuyenSinh.csproj" /p:Configuration=Debug /v:m /nologo

if not %ERRORLEVEL% == 0 (
    echo.
    echo [THAT BAI] Build that bai! Kiem tra loi o tren roi thu lai.
    pause
    exit /b 1
)

echo [OK] Build thanh cong!

:: ---- BUOC 2: CHAY IIS EXPRESS ----
echo.
echo [2/2] Dang khoi dong IIS Express...

set "IIS_PATH=%ProgramFiles%\IIS Express\iisexpress.exe"
if exist "%ProgramFiles(x86)%\IIS Express\iisexpress.exe" set "IIS_PATH=%ProgramFiles(x86)%\IIS Express\iisexpress.exe"

if not exist "%IIS_PATH%" (
    echo [LOI] Khong tim thay IIS Express!
    echo Hay cai IIS Express tai: https://www.microsoft.com/en-us/download/details.aspx?id=48264
    pause
    exit /b 1
)

echo Dang mo trinh duyet: http://localhost:63601/index.aspx
start http://localhost:63601/index.aspx

echo.
echo Server dang chay tai http://localhost:63601
echo   - Trang chu : http://localhost:63601/index.aspx
echo   - Admin     : http://localhost:63601/admin
echo De tat server, hay dong cua so nay.
echo =======================================================
"%IIS_PATH%" /path:"%PROJECT_DIR%" /port:63601 /clr:v4.0

pause
