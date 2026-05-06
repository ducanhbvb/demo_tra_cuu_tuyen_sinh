@echo off
setlocal EnableDelayedExpansion
title Tra Cuu Tuyen Sinh - Local Server

:: Lay duong dan tuyet doi cua thu muc project (cap tren tools/)
pushd "%~dp0.."
set "PROJECT_DIR=%CD%"
popd

:: ---- Profile Chrome rieng biet (tranh nhay profile loai) ----
:: Dung thu muc co dinh trong project, KHONG dung %TEMP%
set "CHROME_PROFILE=%PROJECT_DIR%\.dev-chrome-profile"

:: ---- Kiem tra dotnet ----
where dotnet >nul 2>&1
if errorlevel 1 (
    echo [LOI] Khong tim thay dotnet CLI! Hay cai .NET SDK.
    pause
    exit /b 1
)

:: ---- Tim Chrome ----
set "CHROME_PATH="
if exist "C:\Program Files\Google\Chrome\Application\chrome.exe" (
    set "CHROME_PATH=C:\Program Files\Google\Chrome\Application\chrome.exe"
)
if not defined CHROME_PATH if exist "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" (
    set "CHROME_PATH=C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
)
if not defined CHROME_PATH if exist "%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe" (
    set "CHROME_PATH=%LOCALAPPDATA%\Google\Chrome\Application\chrome.exe"
)

:: ---- Tim IIS Express ----
set "IIS_PATH="
if exist "C:\Program Files\IIS Express\iisexpress.exe" (
    set "IIS_PATH=C:\Program Files\IIS Express\iisexpress.exe"
)
if not defined IIS_PATH if exist "C:\Program Files (x86)\IIS Express\iisexpress.exe" (
    set "IIS_PATH=C:\Program Files (x86)\IIS Express\iisexpress.exe"
)

if not defined IIS_PATH (
    echo [LOI] Khong tim thay IIS Express!
    echo Hay cai IIS Express tai: https://www.microsoft.com/en-us/download/details.aspx?id=48264
    pause
    exit /b 1
)

:: ============================================================
::  RUN + BUILD  (goto tu :MENU_3 hoac khoi dong lan dau)
:: ============================================================
:RUN_BUILD
cls
echo =======================================================
echo  TRA CUU TUYEN SINH - BUILD + CHAY WEB
echo  Thu muc: %PROJECT_DIR%
echo =======================================================
echo.

echo [1/2] Dang build project...
dotnet build "%PROJECT_DIR%\TraCuuTuyenSinh.csproj" /p:Configuration=Debug /v:m /nologo
set BUILD_ERR=%ERRORLEVEL%

if not "%BUILD_ERR%"=="0" (
    echo.
    echo [THAT BAI] Build that bai! Kiem tra loi o tren.
    echo.
    echo   3 = Build lai        4 = Thoat
    echo.
    :ASK_BUILD_FAIL
    set "KEY="
    set /p "KEY=Lua chon (3/4): "
    if "!KEY!"=="3" goto RUN_BUILD
    if "!KEY!"=="4" goto EXIT
    goto ASK_BUILD_FAIL
)

echo [OK] Build thanh cong!
echo.
goto DO_RUN

:: ============================================================
::  RUN (KHONG BUILD)
:: ============================================================
:RUN_ONLY
cls
echo =======================================================
echo  TRA CUU TUYEN SINH - CHAY WEB (khong build)
echo  Thu muc: %PROJECT_DIR%
echo =======================================================
echo.

:: ============================================================
::  KHOI DONG IIS + CHROME
:: ============================================================
:DO_RUN
echo Dang tat IIS Express cu (neu co)...
taskkill /f /im iisexpress.exe >nul 2>&1

echo Dang khoi dong IIS Express...
start "IIS Express" /min "%IIS_PATH%" /path:"%PROJECT_DIR%" /port:9222 /clr:v4.0

echo Dang cho IIS Express khoi dong...
timeout /t 3 /nobreak >nul

echo.
echo  Server    : http://localhost:9222
echo  Trang chu : http://localhost:9222/index.aspx
echo  Admin     : http://localhost:9222/admin
echo  Profile   : %CHROME_PROFILE%
echo.

:: ---- Mo Chrome voi profile RIENG BIET va Remote Debugging ----
if defined CHROME_PATH (
    :: Kiem tra xem Chrome debug port 9222 co dang mo khong
    netstat -an 2>nul | findstr ":9222 " >nul 2>&1
    if !ERRORLEVEL! equ 0 (
        :: Port 9222 da mo: mo tab moi TRONG Chrome dev hien tai (dung debugger API)
        echo  [Chrome] Port 9222 da mo, mo tab moi trong Chrome dev...
        start "" "!CHROME_PATH!" ^
            --remote-debugging-port=9222 ^
            --user-data-dir="!CHROME_PROFILE!" ^
            --no-first-run ^
            --no-default-browser-check ^
            http://localhost:9222/index.aspx
    ) else (
        :: Khoi dong Chrome moi voi profile rieng + remote debug
        echo  [Chrome] Khoi dong Chrome dev profile moi...
        start "" "!CHROME_PATH!" ^
            --remote-debugging-port=9222 ^
            --user-data-dir="!CHROME_PROFILE!" ^
            --no-first-run ^
            --no-default-browser-check ^
            --disable-extensions ^
            http://localhost:9222/index.aspx
    )
    timeout /t 2 /nobreak >nul
    echo  [Chrome] DevTools: http://localhost:9222
    echo  [Chrome] Profile : !CHROME_PROFILE!
) else (
    echo  [INFO] Khong tim thay Chrome, mo browser mac dinh...
    start "" http://localhost:9222/index.aspx
)

echo.

:: ============================================================
::  MENU CHINH
:: ============================================================
:MENU
echo =======================================================
echo   1 = Dung IIS    2 = Run (k build)    3 = Run+Build
echo   4 = Reset profile Chrome              5 = Thoat
echo =======================================================
set "KEY="
set /p "KEY=Lua chon (1/2/3/4/5): "

if "!KEY!"=="1" goto STOP
if "!KEY!"=="2" goto RUN_ONLY
if "!KEY!"=="3" goto RUN_BUILD
if "!KEY!"=="4" goto RESET_PROFILE
if "!KEY!"=="5" goto EXIT
echo  Khong hop le, hay bam 1-5.
echo.
goto MENU

:STOP
echo.
echo Dang dung IIS Express...
taskkill /f /im iisexpress.exe >nul 2>&1
echo [OK] Da dung IIS Express. May chu da tat.
echo.
goto MENU

:RESET_PROFILE
echo.
echo Dang tat Chrome (neu dang mo)...
taskkill /f /im chrome.exe >nul 2>&1
timeout /t 1 /nobreak >nul
echo Dang xoa profile Chrome dev: %CHROME_PROFILE%
if exist "%CHROME_PROFILE%" (
    rmdir /s /q "%CHROME_PROFILE%"
    echo [OK] Da xoa profile cu.
) else (
    echo [INFO] Profile chua ton tai.
)
echo Profile se duoc tao lai lan chay tiep theo.
echo.
goto MENU

:EXIT
echo.
echo Dang dung IIS Express va thoat...
taskkill /f /im iisexpress.exe >nul 2>&1
echo [OK] Da thoat.
endlocal
exit /b 0
