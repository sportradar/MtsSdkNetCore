A MTS SDK library

Notice: before starting DemoProject make sure to enter your bookmaker access token in app.config file 
    and restore nuget packages by right-clicking the solution item and selecting "Restore NuGet Packages".

The SDK is also available via NuGet package manager. Use the following command in the Package Manager Console to install it along with it's dependencies
    - Install-Package Sportradar.MTS.SDK
    
The SDK uses the following 3rd party libraries which must be added via the NuGet package manager
        - App.Metrics
        - log4net
        - RabbitMQ.Client
        - Microsoft.Practices.Unity
    - Newtonsoft.Json

The package contains:
 - DemoProject: A Visual Studio 2019 solution containing a demo project showing the basic usage of the SDK
 - libs: DLL file composing the MTS SDK
 - MTS SDK Documentation.chm: A documentation file describing exposed entities
 - Resources containing the log4net configuration needed by the MTS SDK

CHANGE LOG:
2019-12-12 2.3.0