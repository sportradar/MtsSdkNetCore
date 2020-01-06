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
2020-01-06 1.0.0
Port of MTS SDK to .NET Standard 2.1
Replaced Metrics.NET with App.Metrics
Replaced Code.Contracts with Dawn.Guard conditions
Replaced Common.Logging with Microsoft.Extensions.Logging
Upgraded RabbitMQ.Client to v5.1.2
Upgraded Newtonsoft.Json to v12.0.3
Upgraded Unity to 5.11.3
Removed obsolete methods and properties