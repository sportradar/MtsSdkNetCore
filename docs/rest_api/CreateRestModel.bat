@echo off

DEL RestMessages.cs

set DevEnvDir="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools"
call %DevEnvDir%\..\Tools\vsvars32.bat

REM REM REM KILL KILL KILL

@xsd.exe /c /l:C# /n:Sportradar.MTS.SDK.Entities.Internal.REST uf/UnifiedFeedDescriptions.xsd customBet/AvailableSelections.xsd customBet/Calculation.xsd customBet/CommonTypes.xsd customBet/ErrorResponse.xsd customBet/Selections.xsd

rename Selections.cs RestMessages.cs