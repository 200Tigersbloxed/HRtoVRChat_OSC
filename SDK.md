# HRtoVRChat_OSC_SDK

Created by 200Tigersbloxed

## Preface

The HRtoVRChat_OSC_SDK provides developers a way to send their HeartRate Data to HRtoVRChat_OSC to be forwarded to their avatar in VRChat. It is a super simple to use library written in C#, running on either [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48) or [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0), using [SuperSimpleTCP](https://github.com/jchristn/SuperSimpleTcp) for the protocol, and [protobuf-net](https://github.com/protobuf-net/protobuf-net) for serialization and deserialization.

## Terminology

+ HRSDK
  + The main abstract class in which an SDK will derive to be able to connect and send messages to the server
+ HRMessage
  + The message that will be serialized to be sent to the server and be de-serialized then processed
+ Solution
  + The .NET solution
+ Server
  + The HRtoVRChat_OSC application

> ___
> ## ⚠️ WARNING ⚠️
> This tutorial assumes you know how to use an IDE and develop either .NET Framework 4.8 apps or .NET6 apps. Please see [this guide](https://docs.microsoft.com/en-us/learn/modules/dotnet-introduction/) if you don't know how to develop with .NET
> ___

## Setting Up

Create your application solution in your IDE. Your application can either be a .NET Framework 4.8 application or a .NET6 application.

>___
> ### ℹ️ Which should I use? ℹ️
> .NET6 is a newer .NET version, which is supported more by Microsoft than .NET Framework. .NET6 also supports more platforms like macOS and Linux, so using .NET6 will reach a further audience. However, .NET Framework is generally easier to work with and more Windows users have installed it already.
> ___

Once you have your solution created, we'll need to import the library to be used. [Download the latest SDK](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest/download/HRtoVRChat_OSC_SDK.zip), and extract it to a directory. Inside the folder, you'll see two folders named `net48` and `net6.0`. Open the folder in which your solution is created.

> ___
> ### ℹ️ Which folder do I open? ℹ️
> If you're using .NET Framework 4.8, open the `net48` folder.
>
>If you're using .NET6, open the `net6.0` or `net48` folder. .NET6 supports importing .NET Framework libraries, however, it's recommended to use the .NET6 library for .NET6.
>___

Find the file named `HRtoVRChat_OSC_SDK.dll` (same for both folders). This is the library that we will import into our solution.

## Creating our SDK

Now that you have the libraries imported, import the SDK into the current file.

```csharp
using HRtoVRChat_OSC_SDK;
```

Then, create a new class. For example, I'll call the class `SDKInstance`. Make this either a public, private, or internal class depending on your use case.

```csharp
public class SDKInstance
{

}
```

Then, derive the class of HRSDK

```csharp
public class SDKInstance : HRSDK
{

}
```

Now you'll see some errors because we have to import our abstract properties. Create these three properties.

```csharp
public class SDKInstance : HRSDK
{
	public override int HR { get; set; }
	public override bool IsOpen { get; set; }
	public override bool IsActive { get; set; }
}
```

These three properties are the properties we will change to update our data.

## Starting the SDK

Now, back in our main class, let's set up the `SDKInstance` to be used. In this example, I'll run the `Main` method whenever I want to start the SDK

```csharp
public class SDKHandler
{
	private static SDKInstance sdk;

	public static void Main()
	{
		sdk = new SDKInstance();
	}
}
```

Then, to start the SDK, I'll run the following code wherever the entry point of the application is located.

```csharp
SDKHandler.Main();
```

This will start the SDK and attempt to open a connection to the server.

> ___
> ### ℹ️ What if  I don't want to start the SDK as soon as I instantiate the class? ℹ️
> `HRSDK` constructors have a bool parameter called `autoOpen`, which if set to `false`, will require you to call the `Open` method to start the SDK.
> ___

## Sending data

Now we'll update and send our data to the server. In our example, we'll edit `SDKHandler` to add a method to do this.

```csharp
public class SDKHandler
{
	private static SDKInstance sdk;

	public static void Main()
	{
		sdk = new SDKInstance();
	}

	public static void UpdateData(int HR, bool IsOpen, bool IsActive)
	{
		// Don't call this unless the SDK exists and is open
		if(sdk != null && sdk.isClientConnected)
		{
			// Set Data
			sdk.HR = HR;
			sdk.IsOpen = IsOpen;
			sdk.IsActive = IsActive;
			// Update
			sdk.Update();
		}
	}
}
```

Then, assuming our `HR` is `175`, our `IsOpen` is `true`, and our `IsActive` is `true`, this is how we'll update the data

```csharp
SDKHandler.UpdateData(175, true, true);
```

This will upload the data to the server, for it to be forwarded to our avatar! This was the simple setup for setting up the SDK, now all you'll need to do is rearrange everything to work with your HeartRate data source.

# HRSDK Documentation

## base constructors

Entire documentation of `HRSDK`'s constructors.

### HRSDK()

Just a normal constructor. Will open the socket automatically and connect to the server at `127.0.0.1:9000`.

### HRSDK(bool autoOpen)

Defines whether or not the socket will open automatically to connect to the server at `127.0.0.1:9000`.

### HRSDK(bool autoOpen, string ipPort)

Defines whether or not the socket will open automatically to connect to the server at the given `ipPort`

### Notes

Note that both `autoOpen` and `ipPort` are defined in the parameter, so you can also instantiate like

```csharp
new HRSDK(ipPort: "127.0.0.1:8000")
```

and `autoOpen` will default to `true`, while `ipPort` will be `127.0.0.1:8000`

## base properties

Entire documentation of `HRSDK`'s properties.

### isClientConnected (bool)

Returns whether or not the SDK is connected to the server.

### abstract HR (int)

The current Heart Rate.

### abstract IsOpen (bool)

Whether the data source connection is valid.

### abstract IsActive (bool)

Whether the data source has a device connected.

## base methods

Entire documentation of `HRSDK`'s methods.

### virtual OnSDKOpened()

A virtual method (one that can be overridden) that will be invoked when the SDK connects to the server.

### virtual OnSDKClosed()

A virtual method (one that can be overridden) that will be invoked when the SDK disconnects from the server.

### void Update()

Invoke when you wish to update the HeartRate data. Will send whatever the `HR`, `IsOpen`, and `IsActive` abstract properties are.

### void Open()

Invoke when you wish to open the connection to the SDK. This is invoked automatically if the constructor's `autoOpen` parameter is left at `true`.

### void Close()

Invoke when you wish to close the connection to the SDK.
