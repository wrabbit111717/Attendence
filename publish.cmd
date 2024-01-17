@echo off
git fetch origin
git pull
rename "C:\inetpub\attendance\app__offline.htm" app_offline.htm
if not exist "C:\inetpub\attendance\app_offline.htm" (
                echo Cannot initiate file app_offline.htm!
                goto :error
)
%windir%\system32\inetsrv\appcmd recycle apppool /apppool.name:"attendance"
dotnet publish Attendance -c Release -r win81-x64 --self-contained false -f netcoreapp3.1 --force -o C:\inetpub\attendance
rename "C:\inetpub\attendance\app_offline.htm" app__offline.htm
if %errorlevel% neq 0 (
                echo Cannot rename file app_offline.htm!
)

:; exit 0
pause
exit /b 0

:error
pause
exit /b %errorlevel%
