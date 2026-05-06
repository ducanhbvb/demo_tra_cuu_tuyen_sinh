@echo off
title Publish - Tra Cuu Tuyen Sinh
chcp 65001 >nul 2>&1

:: Lay duong dan tuyet doi cua thu muc project (cap tren tools/)
pushd "%~dp0.."
set "PROJECT_DIR=%CD%"
popd

set "PUBLISH_DIR=%~dp0publish"

echo =======================================================
echo  PUBLISH PROJECT: Tra Cuu Tuyen Sinh
echo  Project : %PROJECT_DIR%
echo  Output  : %PUBLISH_DIR%
echo =======================================================
echo.

:: Tim MSBuild (VS 2022)
set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"

if not exist "%MSBUILD%" (
    echo [LOI] Khong tim thay MSBuild! Hay cai Visual Studio 2022.
    pause
    exit /b 1
)

echo [INFO] MSBuild: %MSBUILD%
echo.

:: Xoa thu muc publish cu neu co
if exist "%PUBLISH_DIR%" (
    echo [INFO] Xoa thu muc publish cu...
    rmdir /s /q "%PUBLISH_DIR%"
)

:: Build va Publish bang MSBuild
echo [INFO] Dang build va publish (Release)...
echo.

"%MSBUILD%" "%PROJECT_DIR%\TraCuuTuyenSinh.csproj" ^
    /p:Configuration=Release ^
    /p:DeployOnBuild=true ^
    /p:PublishProfile=FolderProfile ^
    /p:PublishUrl="%PUBLISH_DIR%" ^
    /p:WebPublishMethod=FileSystem ^
    /p:DeleteExistingFiles=true ^
    /p:ExcludeApp_Data=false ^
    /v:m /nologo /clp:Summary

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ========================================
    echo  [THAT BAI] Publish that bai!
    echo  Kiem tra loi o tren.
    echo ========================================
    pause
    exit /b 1
)

echo.
echo ========================================
echo  [OK] Publish thanh cong!
echo  Output: %PUBLISH_DIR%
echo ========================================
echo.

:: Verify published files
echo [INFO] Kiem tra thu muc publish (co ít nhat 100 file):
echo ----------------------------------------
dir "%PUBLISH_DIR%" /s /b | find /c /v ""
echo ----------------------------------------
echo.

echo [INFO] Kiem tra Resources folder:
if exist "%PUBLISH_DIR%\Resources" (
    echo [OK] Thu muc Resources ton tai.
    echo [INFO] Noi dung Resources:
    dir "%PUBLISH_DIR%\Resources" /b /s
) else (
    echo [LOI] Thu muc Resources KHONG ton tai trong publish!
    echo [LOI] Kiem tra lai .csproj co include Resources\**\*.* khong.
)
echo.

echo [INFO] Kiem tra Scripts folder:
if exist "%PUBLISH_DIR%\Scripts" (
    echo [OK] Thu muc Scripts ton tai.
    dir "%PUBLISH_DIR%\Scripts" /b
) else (
    echo [LOI] Thu muc Scripts KHONG ton tai trong publish!
)
echo.

echo [INFO] Kiem tra lib/bootstrap folder:
if exist "%PUBLISH_DIR%\lib\bootstrap" (
    echo [OK] Thu muc lib/bootstrap ton tai.
    dir "%PUBLISH_DIR%\lib\bootstrap" /b
) else (
    echo [LOI] Thu muc lib/bootstrap KHONG ton tai trong publish!
    echo [LOI] Kiem tra lai .csproj co include lib\**\*.* khong.
)
echo.

:: Hien thi thong tin thu muc
echo [INFO] Noi dung thu muc publish:
echo ----------------------------------------
dir "%PUBLISH_DIR%" /b
echo ----------------------------------------
echo.
echo Ban co the copy thu muc "%PUBLISH_DIR%"
echo sang VMware va deploy len IIS.
echo.
echo Luu y:
echo  - Cau hinh IIS Application Pool: .NET Framework v4.0, Integrated
echo  - Kiem tra connection string trong Web.config
echo  - Dam bao SQL Server da duoc cai dat va database da duoc restore
echo.

:: Run warm-up script after successful publish
echo.
echo ========================================
echo  [INFO] Running warm-up script...
echo ========================================
powershell -ExecutionPolicy Bypass -File "%PROJECT_DIR%\tools\warmup.ps1" -Url "http://localhost:63601"
echo.

pause
