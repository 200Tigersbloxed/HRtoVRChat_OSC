# HRtoVRChat_OSC_SDK Docs

Created by 200Tigersbloxed

## Preface

The HRtoVRChat_OSC_SDK provides developers with an SDK to implement their own devices into the HRtoVRChat application. It is a simple library written in C# with either a [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48) or [.NET Core 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0). It is up to the developer to decide whether to use the framework or core libraries, as both are supported, however *it is recommended to use the core libraries where possible*.

The library has two ways of communication with the application.

1) Headless via. [SuperSimpleTCP](https://github.com/jchristn/SuperSimpleTcp) and packaging with [protobuf-net](https://github.com/protobuf-net/protobuf-net) libraries.

2) With [Reflection](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection) by loading a Class Library from the SDKs folder that HRtoVRChat_OSC generates.

## Prerequisites

+ A .NET Compatible IDE

  + Such as:

    + [Visual Studio](https://visualstudio.microsoft.com/)
    + [Rider](https://www.jetbrains.com/rider/)

+ [The SDK Files](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest/download/HRtoVRChat_OSC_SDK.zip)

> ___
> ## ⚠️ WARNING ⚠️
> This guide assumes you know how to use an IDE and develop either .NET Framework 4.8 apps or .NET6 apps. Please see [this guide](https://docs.microsoft.com/en-us/learn/modules/dotnet-introduction/) if you don't know how to develop with .NET
> ___

## Creating the SDK Library

First, start with a new C# Class Library for .NET Core or Framework (see Preface for info on which one you should choose). We will call this our **HRExtension**, and that's how it will be referred to in the rest of this guide.

Inside of our HRExtension's Solution, import the Library of choice, and then create a new Class file if you don't have one already, and make sure to name the class accordingly. After you've done so, be sure to include the `HRtoVRChat_OSC_SDK` namespace in the Class file.

```cs
using HRtoVRChat_OSC_SDK;
```

Now, derive the class of the HRSDK abstract class

```cs
public class HRExtension : HRSDK
{
    public override HRSDKOptions Options = new HRSDKOptions("HRExtension");
    public override int HR { get; set; }
    public override bool IsOpen { get; set; }
    public override bool IsActive { get; set; }
}
```

Next, we need to start collecting the data when the SDK is ready, so we'll add an override given to use that will invoke once the SDK makes a connection with the OSC Software.

```cs
public override void OnSDKOpened()
{
    // Put your code to start getting the HR Data here (if needed)
}
```

Now, if still needed, you may need to use an Update frame to update data, there's an override provided for that too.

```cs
public override void OnSDKUpdate()
{
    /*
    * This method is invoked every 10ms if loaded with reflection, ~1000ms if over network
    *
    * It would be better to use the IsReflected property and make your own thread if the
    * library is not loaded over reflection if you need a high update rate for updating
    * the HR data.
    *
    * If this is the case, update HR data in a separate Thread, and Push data in the 
    * OnSDKUpdate override
    */
}
```

Then, when you're ready to send your data off to the server, invoke the `PushData` method

```cs
public void SendCurrentData() =>/*This will send the data to the Server -> */ PushData();
```

Sometimes, you may want to check if the data on the server matches yours, you can do so with the `PullData` and the `OnSDKData(HRMessage)` methods

```cs
// Example Usage; usage may vary depending on your need
public override void OnSDKData(Messages.HRMessage message)
{
    bool isThisSDK = message.SDKName == Options.SDKName;
    if(isThisSDK)
        Log(LogLevel.Debug, "Currently using this SDK!")
    else
        Log(LogLevel.Warn, "Not using this SDK!")
}

public void GetServerData() =>/*This will get the server's data -> */ PullData();
```

As demonstrated above, sometimes you may want to log your own data to the client's log. The included `Log(LogLevel, object, ...)` method provides a way for the developer to log.

> ___
> ## ℹ️ Note ℹ️
> The Log method currently does not write logs to the OSC Software unless it is being reflected. This will change in the future to expand support for logging to the Network, but for now, it's recommended to implement your own logging solution if IsReflected is false.
> ___

Now you may want to close your SDK if you don't want it to be used anymore. There are two ways to do this, and both are acceptable

1) Close via. the Network
    + ```cs
      public void CloseSDK() =>/*This will only work if IsReflected is false*/ Close();
      ```
2) Close by setting IsOpen and IsActive to false
  + ```cs
    public void CloseSDK()
    {
        IsOpen = false;
        IsActive = false;
    }
    ```

Or, you could even combine the two

```cs
public void CloseSDK()
{
    if(IsReflected)
        Close();
    else
    {
        IsOpen = false;
        IsActive = false;
    }
}
```

Finally, you may need to dispose or reset some things when the SDK does close, which is what the `OnSDKClosed` method is for

```cs
public override void OnSDKClosed()
{
    // Make sure everything is default
    HR = 0;
    IsOpen = false;
    IsActive = false;
}
```

And now you're done with the library portion. If you'd like to test it, you can build and load the artifact as a reflected SDK and test! Here's what our finished code looked like:

> HRExtension / HRExtension.cs
```cs
using HRtoVRChat_OSC_SDK;

namespace HRExtension;

public class HRExtensionMain : HRSDK
{
    public override HRSDKOptions Options = new HRSDKOptions("HRExtension");
    public override int HR { get; set; }
    public override bool IsOpen { get; set; }
    public override bool IsActive { get; set; }

    public override void OnSDKOpened()
    {
        // Put your code to start getting the HR Data here (if needed)
    }

    public override void OnSDKUpdate()
    {
        /*
        * This method is invoked every 10ms if loaded with reflection, ~1000ms if over network
        *
        * It would be better to use the IsReflected property and make your own thread if the
        * library is not loaded over reflection if you need a high update rate for updating
        * the HR data.
        *
        * If this is the case, update HR data in a separate Thread, and Push data in the 
        * OnSDKUpdate override
        */
    }

    public void SendCurrentData() =>/*This will send the data to the Server -> */ PushData();

    // Example Usage; usage may vary depending on your need
    public override void OnSDKData(Messages.HRMessage message)
    {
        bool isThisSDK = message.SDKName == Options.SDKName;
        if(isThisSDK)
            Log(LogLevel.Debug, "Currently using this SDK!")
        else
            Log(LogLevel.Warn, "Not using this SDK!")
    }

    public void GetServerData() =>/*This will get the server's data -> */ PullData();

    public void CloseSDK()
    {
        if(IsReflected)
            Close();
        else
        {
            IsOpen = false;
            IsActive = false;
        }
    }

    public override void OnSDKClosed()
    {
        // Make sure everything is default
        HR = 0;
        IsOpen = false;
        IsActive = false;
    }
}
```

> ___
> ## ℹ️ Note ℹ️
> If you use any external libraries that HRtoVRChat_OSC does not utilize, then you will need to merge libraries with a tool like [ILRepack](https://github.com/gluck/il-repack). HRtoVRChat_OSC will not load libraries dropped in the SDKs folder!
> ___

## Creating your Application to run a Headless SDK

This portion of the docs are for taking your previous HRExtension library and using it with your own software to be connected via. Network. For this example, I'll be creating a Console Application, as it's easier to setup, however if you wish to have a User Interface, I'd look into solutions like [Avalonia](https://github.com/AvaloniaUI/Avalonia)

Underneath the same HRExtension solution, create a new project. Select the Console Application for either Net Framework or NET Core, and create it. I'm going to call it HRExtensionCLI. Inside of this project, we're going to reference our HRExtension project and the HRtoVRChat_OSC_SDK again.

Now that we have all of our references, look into the project's files and you should see a Program.cs, open that and find the `static void Main(string[] args)` method it created. This is our entry-point for the program.

Inside of this void, we're going to initialize our HRExtension class and start it whenever it opens.

> ___
> ## ⚠️ WARNING ⚠️
> When opening a network SDK, you must make sure that the HRtoVRChat_OSC software is running and accepting connections from the SDK!
> ___

```cs
using HRExtension;
using HRtoVRChat_OSC_SDK;

namespace HRExtensionCLI;

class Program
{
    private static HRExtensionMain? SDK;

    static void Main(string[] args)
    {
        SDK = new();
        SDK.Open();
        Console.ReadKey(true);
    }
}
```

And from here, you can get fancy and decide how you want to handle when the user wishes to close the SDK. Maybe something like using [Console.ReadLine()](https://docs.microsoft.com/en-us/dotnet/api/system.console.readline?view=net-6.0) to handle commands?

## HRSDK Docs

### Abstract Properties

+ HRSDKOptions Options {get}
    + Required for SDK Information for the OSC App
+ int HR {get set}
    + The current HeartRate that the SDK reads
+ bool IsOpen {get set}
    + If the device that transmits HR data to the SDK is connected
    + If this doesn't matter, then make it the same as IsActive
+ bool IsActive {get set}
    + If the SDK has an active connection to it's source
    + Most of the time, this will be true as long as it can get HR data

### Properties

+ bool IsSDKConnected {get}
    + Whether the SDK is connected to the Server
    + This will always be true if the SDK IsReflected
+ bool IsReflected {get}
    + Whether or not the SDK was loaded with Reflection

### Override Methods

+ void OnSDKOpened()
    + Invoked when the SDK is opened
+ void OnSDKUpdate()
    + Invoked when the SDK is updated
    + Preferred to only be used when IsReflected is true
      + If it isn't true, then only use it for PushData()
+ void OnSDKData(Messages.HRMessage message)
    + Callback for when the SDK receives data from the server
+ void OnSDKClosed()
    + Invoked when the SDK is closed

## Methods

+ void PushData()
    + Sends the current values of `HR`, `IsOpen`, and `IsActive` to the server.
+ void PullData()
    + Requests the current server data
    + Callback is at `OnSDKData`
+ void Open()
    + Opens a TCP Connection to the server
    + Network only! Will not do anything if IsReflected is true
+ void Close()
    + Closes an active TCP Connection to the server
    + Network only! Will not do anything if IsReflected is true
+ void Log(LogLevel logLevel, object msg, ConsoleColor color = ConsoleColor.White, Exception? e = null)
    + LogHelper duplication for HRSDK
    + Highly recommended to be used if IsReflected is true, as logs will output to the log folder when sent through here
