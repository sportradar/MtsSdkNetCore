A MTS SDK library (.NET Standard 2.1)

Notice: before starting DemoProject make sure to enter your bookmaker access token in app.config file 
    and restore nuget packages by right-clicking the solution item and selecting "Restore NuGet Packages".

The SDK is also available via NuGet package manager. Use the following command in the Package Manager Console to install it along with it's dependencies
    - Install-Package Sportradar.MTS.SDK
    
The SDK uses the following 3rd party libraries which must be added via the NuGet package manager
        - App.Metrics
        - Castle.Core
        - Dawn.Guard
        - Microsoft.Extensions.Logging
        - Newtonsoft.Json
        - RabbitMQ.Client
        - Unity

The package contains:
 - DemoProject: A Visual Studio 2019 solution containing a demo project showing the basic usage of the SDK
 - libs: DLL file composing the MTS SDK
 - MTS SDK Documentation.chm: A documentation file describing exposed entities
 - Resources containing the log4net configuration needed by the MTS SDK

CHANGE LOG:
2020-09-18 2.3.2
Added # to the User ID pattern
Fix: checking for invalid products

2020-04-07 2.3.1
Allow 0 cashout stake when building TicketCashout
Added bookmakerId to the client_properties
Added argument to rabbit queue declare: queue-master-locator
Updated rabbit client properties and consumerTag for Standard
Updated DemoProject to use logger factory
Examples updated to use UOF markets
Internalize internal classes and interfaces
Fix: XmlSerializer now properly initializes during DI container creation

2020-01-06 2.3.0
Port of MTS SDK to .NET Standard 2.1
Replaced Metrics.NET with App.Metrics
Replaced Code.Contracts with Dawn.Guard conditions
Replaced Common.Logging with Microsoft.Extensions.Logging
Upgraded RabbitMQ.Client to v5.1.2
Upgraded Newtonsoft.Json to v12.0.3
Upgraded Unity to 5.11.3
Removed obsolete methods and properties