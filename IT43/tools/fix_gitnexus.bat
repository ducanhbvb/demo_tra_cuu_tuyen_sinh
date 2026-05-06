@echo off
setlocal EnableDelayedExpansion
title Fix GitNexus - Duplicate Key Error

:: ============================================================
:: fix_gitnexus.bat
:: Fix loi "Found duplicated primary key value" cua GitNexus
:: bang cach xoa database KuZu cu va analyze lai tu dau.
::
:: Cach dung: click doi hoac chay tu bat ky terminal nao trong
::            thu muc project. Tu dong tim PROJECT_DIR.
:: ============================================================

:: Lay duong dan tuyet doi cua thu muc project (cap tren tools/)
pushd "%~dp0.."
set "PROJECT_DIR=%CD%"
popd

set "GN_DIR=%PROJECT_DIR%\.gitnexus"

echo ============================================================
echo   GitNexus Fix - Duplicate Primary Key Error
echo ============================================================
echo.
echo   Project : %PROJECT_DIR%
echo   GN Dir  : %GN_DIR%
echo.

:: ---- Kiem tra npx co san khong ----
where npx >nul 2>&1
if errorlevel 1 (
    echo [LOI] Khong tim thay npx! Hay cai Node.js truoc.
    echo       https://nodejs.org/
    pause
    exit /b 1
)

:: ---- Kiem tra folder .gitnexus ton tai ----
if not exist "%GN_DIR%" (
    echo [INFO] Folder .gitnexus khong ton tai. Chay analyze moi...
    goto :analyze
)

:: ---- Kiem tra embeddings truoc khi xoa ----
set "HAS_EMBEDDINGS=0"
if exist "%GN_DIR%\meta.json" (
    :: Tim xem embeddings co lon hon 0 khong
    for /f "usebackq tokens=*" %%L in ("%GN_DIR%\meta.json") do (
        echo %%L | findstr /C:"\"embeddings\": 0" >nul 2>&1
        if errorlevel 1 (
            echo %%L | findstr /C:"\"embeddings\"" >nul 2>&1
            if not errorlevel 1 (
                set "HAS_EMBEDDINGS=1"
            )
        )
    )
)

if "!HAS_EMBEDDINGS!"=="1" (
    echo [WARN] Index cu co embeddings. Se them --embeddings khi analyze lai.
    echo.
)

:: ---- Xoa folder .gitnexus ----
echo [1/2] Xoa folder .gitnexus ^(database KuZu bi corrupt^)...
rd /s /q "%GN_DIR%" 2>nul
if exist "%GN_DIR%" (
    echo.
    echo [LOI] Khong xoa duoc .gitnexus!
    echo       Co the file dang bi lock boi MCP server GitNexus.
    echo       Hay dong VS Code / Cursor roi chay lai file nay.
    pause
    exit /b 1
)
echo       Da xoa thanh cong.
echo.

:: ---- Chay analyze lai ----
:analyze
echo [2/2] Chay npx gitnexus analyze...
echo.

cd /d "%PROJECT_DIR%"

if "!HAS_EMBEDDINGS!"=="1" (
    echo       ^(Co embeddings - them flag --embeddings^)
    call npx gitnexus analyze --embeddings
) else (
    call npx gitnexus analyze
)

if errorlevel 1 (
    echo.
    echo [LOI] gitnexus analyze that bai! Xem log phia tren.
    pause
    exit /b 1
)

echo.
echo ============================================================
echo   THANH CONG! GitNexus da duoc rebuild sach se.
echo ============================================================
echo.

:: Hien thi thong tin index moi
if exist "%GN_DIR%\meta.json" (
    echo   Thong tin index moi:
    type "%GN_DIR%\meta.json"
    echo.
)

echo   Restart MCP server trong VS Code / Cursor de ap dung.
echo   (Command Palette ^> "MCP: Restart Server" ^> gitnexus)
echo.
pause
exit /b 0
