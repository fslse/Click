set WORKSPACE=..\..

set LUBAN_GEN=%WORKSPACE%\..\Luban-Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\AssetPackages\Config\DataTables

dotnet %LUBAN_GEN% ^
    -t client ^
    -c cs-newtonsoft-json ^
    -d json  ^
    --conf %CONF_ROOT%\config.json ^
    -x outputCodeDir=%WORKSPACE%\Scripts\HotFix\Config ^
    -x outputDataDir=%WORKSPACE%\AssetPackages\Config\Json
pause