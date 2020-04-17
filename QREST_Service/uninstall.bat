@ECHO OFF
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo Uninstalling QREST Service...
echo ---------------------------------------------------
InstallUtil /u /servicename="QRESTProdService" C:\QREST_ServiceRunFolder\QREST_Service.exe
echo ---------------------------------------------------
echo Done.
PAUSE