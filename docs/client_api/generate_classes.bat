echo off
cls
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:AccessTokenDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi /input:access-token-schema.json /output:ClientApi\AccessTokenDTO.cs
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:CcfResponseDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi /input:ccf-response-schema.json /output:ClientApi\CcfResponseDTO.cs
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:MaxStakeResponseDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi /input:max-stake-response-schema.json /output:ClientApi\MaxStakeResponseDTO.cs

echo.
echo Replacing 'double' to 'long' ...
..\..\build\tools\fart.exe -r *.cs  " double " " long "

echo Replacing 'double?' to 'long?' ...
..\..\build\tools\fart.exe -r *.cs  " double? " " long? "

echo.
echo Replacing 'new IEnumerable^<' to 'new Collection^<' ...
..\..\build\tools\fart.exe -r *.cs  "= new IEnumerable<" "= new Collection<"

set /p id=