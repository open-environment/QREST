@ECHO OFF
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo Installing QREST Service...
echo ---------------------------------------------------
InstallUtil C:\QREST_ServiceRunFolder\QREST_Service.exe
echo ---------------------------------------------------
PAUSE
echo Done.
