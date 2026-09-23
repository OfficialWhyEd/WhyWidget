@echo off
rem Fa il programma: un file solo, WhyWidget.exe.
rem Non serve installare niente: il compilatore e' gia' dentro Windows.
rem Gli assembly di sistema li carica csc da solo: qui vanno indicati solo
rem quelli di WPF, che non stanno nella sua lista predefinita.
setlocal
cd /d "%~dp0"

set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
set GAC=%WINDIR%\Microsoft.NET\assembly

set R=-reference:"%GAC%\GAC_MSIL\WindowsBase\v4.0_4.0.0.0__31bf3856ad364e35\WindowsBase.dll"
set R=%R% -reference:"%GAC%\GAC_32\PresentationCore\v4.0_4.0.0.0__31bf3856ad364e35\PresentationCore.dll"
set R=%R% -reference:"%GAC%\GAC_MSIL\PresentationFramework\v4.0_4.0.0.0__31bf3856ad364e35\PresentationFramework.dll"
set R=%R% -reference:"%GAC%\GAC_MSIL\System.Xaml\v4.0_4.0.0.0__b77a5c561934e089\System.Xaml.dll"

set ICONA=
if exist WhyWidget.ico set ICONA=-win32icon:WhyWidget.ico

"%CSC%" -nologo -optimize+ -target:winexe -out:..\WhyWidget.exe %ICONA% %R% *.cs
if errorlevel 1 (
  echo.
  echo NON compilato.
  exit /b 1
)
echo.
echo Fatto: WhyWidget.exe
endlocal
