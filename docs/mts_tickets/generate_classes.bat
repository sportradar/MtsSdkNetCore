echo off
cls
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket /input:ticket-schema.json /output:Ticket\TicketDTO.cs
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketAckDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketAck /input:ticket-ack-schema.json /output:TicketAck\TicketAckDTO.cs
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketCancelDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancel /input:ticket-cancel-schema.json /output:TicketCancel\TicketCancelDTO.cs
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketCancelAckDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck /input:ticket-cancel-ack-schema.json /output:TicketCancelAck\TicketCancelAckDTO.cs
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketResponseDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse /input:ticket-response-schema.json /output:TicketResponse\TicketResponseDTO.cs
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketCancelResponseDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelResponse /input:ticket-cancel-response-schema.json /output:TicketCancelResponse\TicketCancelResponseDTO.cs

..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketReofferCancelDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketReofferCancel /input:ticket-reoffer-cancel-schema.json /output:TicketReofferCancel\TicketReofferCancelDTO.cs

..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketCashoutDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashout /input:ticket-cashout-schema.json /output:TicketCashout\TicketCashoutDTO.cs
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketCashoutResponseDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashoutResponse /input:ticket-cashout-response-schema.json /output:TicketCashoutResponse\TicketCashoutResponseDTO.cs

..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketNonSrSettleDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettle /input:ticket-non-sr-settle-schema.json /output:TicketNonSrSettle\TicketNonSrSettleDTO.cs
..\..\build\tools\nswag\nswag.exe jsonschema2csclient /arraytype:IEnumerable /name:TicketNonSrSettleResponseDTO /namespace:Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettleResponse /input:ticket-non-sr-settle-response-schema.json /output:TicketNonSrSettleResponse\TicketNonSrSettleResponseDTO.cs

echo.
echo Replacing 'double' to 'long' ...
..\..\build\tools\fart.exe -r *.cs  " double " " long "

echo Replacing 'double?' to 'long?' ...
..\..\build\tools\fart.exe -r *.cs  " double? " " long? "

echo.
echo Replacing 'new IEnumerable^<' to 'new Collection^<' ...
..\..\build\tools\fart.exe -r *.cs  "= new IEnumerable<" "= new Collection<"

echo.
echo.
echo TODO: Add the following into TicketDTO.cs, TicketCancelDTO.cs, TicketResponseDTO.cs and TicketCashoutDTO.cs:
echo using System.Collections.Generic;
echo using System.Collections.ObjectModel;

echo.
echo.
echo TODO: Add the following into TicketDTO.cs:
echo [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
echo.
echo Bonus.BonusType, Bonus.BonusMode, Stake.StakeType, OddsChange

set /p id=