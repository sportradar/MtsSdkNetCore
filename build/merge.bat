@echo off

set bat_dir=%~dp0
set relative_path=..\src\Sportradar.MTS.SDK\bin\Release\netstandard2.1

pushd "%bat_dir%"

REM echo Merging 4.0
REM IF NOT EXIST "%relative_path%\net40" mkdir "%relative_path%\net40"

REM "%bat_dir%tools\ILMerge.exe" /ver:%1 /internalize:"DoNotInternalize.txt" /ndebug /copyattrs /targetplatform:v4,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /out:"%relative_path%\net40\Sportradar.MTS.SDK.dll" "%relative_path%\Sportradar.MTS.SDK.dll" "%relative_path%\Sportradar.MTS.SDK.API.dll" "%relative_path%\Sportradar.MTS.SDK.Common.dll" "%relative_path%\Sportradar.MTS.SDK.Entities.dll" "%relative_path%\RabbitMQ.Client.dll" "%relative_path%\Microsoft.Practices.Unity.dll" "%relative_path%\Microsoft.Practices.ServiceLocation.dll" "%relative_path%\log4net.dll" "%relative_path%\Metrics.dll" "%relative_path%\Newtonsoft.Json.dll"

REM echo ILmerge 4.0 successfuly executed.

REM echo.
REM copy "%relative_path%\net40\Sportradar.MTS.SDK.dll" "lib\net40\Sportradar.MTS.SDK.dll"
REM echo.

echo Merging 4.5
REM IF NOT EXIST "%relative_path%\net45" mkdir "%relative_path%\net45"

"%bat_dir%tools\ILMerge.exe" /ver:%1 /internalize:"DoNotInternalize.txt" /ndebug /copyattrs /targetplatform:v4,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /out:"%relative_path%\Sportradar.MTS.SDK.dll" "%relative_path%\Sportradar.MTS.SDK.dll" "%relative_path%\RabbitMQ.Client.dll" "%relative_path%\Microsoft.Practices.Unity.dll" "%relative_path%\Microsoft.Practices.ServiceLocation.dll" "%relative_path%\log4net.dll" "%relative_path%\Metrics.dll" "%relative_path%\Newtonsoft.Json.dll"

echo ILmerge 4.5 successfuly executed.

echo.
copy "%relative_path%\Sportradar.MTS.SDK.dll" "lib\netstandard2.1\Sportradar.MTS.SDK.dll"
echo.

popd