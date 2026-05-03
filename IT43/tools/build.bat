@echo off
title Build - Tra Cuu Tuyen Sinh

:: Lay duong dan tuyet doi cua thu muc project (cap tren tools/)
pushd "%~dp0.."
set "PROJECT_DIR=%CD%"
popd

echo =======================================================
echo  BUILD PROJECT: Tra Cuu Tuyen Sinh
echo  Thu muc: %PROJECT_DIR%
echo =======================================================

:: Tim MSBuild (VS 2022)
set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"

if not exist "%MSBUILD%" (
    echo [LOI] Khong tim thay MSBuild! Hay cai Visual Studio 2022.
    pause
    exit /b 1
)

echo [INFO] Dang build...
"%MSBUILD%" "%PROJECT_DIR%\TraCuuTuyenSinh.csproj" /p:Configuration=Debug /v:m /nologo

if %ERRORLEVEL% == 0 (
    echo.
    echo [OK] Build thanh cong!
) else (
    echo.
    echo [THAT BAI] Build that bai, kiem tra loi o tren.
)

pause
