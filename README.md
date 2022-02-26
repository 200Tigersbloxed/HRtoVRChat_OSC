# HRtoVRChat_OSC

Stream your Heart Rate onto your VRChat avatar via. the use of OSC Avatar Parameters.

[![Discord](https://img.shields.io/discord/887157106472550422.svg?color=%237289da&label=discord&style=for-the-badge)](https://discord.gg/WF3B2r4xby) 
[![license](https://img.shields.io/github/license/200Tigersbloxed/HRtoVRChat_OSC?style=for-the-badge)](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/blob/main/LICENSE) 
![downloads](https://img.shields.io/github/downloads/200Tigersbloxed/HRtoVRChat_OSC/total?style=for-the-badge) 
[![lastcommit](https://img.shields.io/github/last-commit/200Tigersbloxed/HRtoVRChat_OSC?style=for-the-badge)](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/commits/main) 
[![issues](https://img.shields.io/github/issues/200Tigersbloxed/HRtoVRChat_OSC?style=for-the-badge)](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/issues)

## Installing

1) Download the `HRtoVRChat_OSC.zip` from the [Latest Release](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest)
2) Extract the folder to a directory
3) Run once, the app will auto close.
4) Open `config.cfg` (in the same directory) and change the config values to your likings.

## Config

After installing, you then have to tell HRtoVRChat_OSC which service you'll be using to get your heart rate. Below is a table of which Heart Rate monitors are supported. 
If you'd like to request support, please use the [Discussions Board](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/discussions) to do so.

Please also consider contributing to add more support with other Heart Rate Monitors.

| Device        | HRType          | Info                                                                     |
|---------------|-----------------|--------------------------------------------------------------------------|
| FitbitHRtoWS  | `fitbithrtows`  | https://github.com/200Tigersbloxed/FitbitHRtoWS                          |
| HypeRate      | `hyperate`      | https://www.hyperate.io/                                                 |
| Pulsoid       | `pulsoid`       | https://pulsoid.net/                                                     |
| PulsoidSocket | `pulsoidsocket` | https://github.com/pulsoid-oss/pulsoid-api#read_heart_rate_via_websocket |
| TextFile      | `textfile`      | A .txt file containing only a number                                     |

Take note of HRType, as you'll need to know which you you have to put in the `hrtype` config value.

### Values

Below is a table of all the config values and a description. Please update the config accordingly.

| Config Value        | Default Value             | Description                                                                             |
|---------------------|---------------------------|-----------------------------------------------------------------------------------------|
| `ip`                | `(string)` `127.0.0.1`    | The IP to send OSC messages to.                                                         |
| `port`              | `(int)` 9000              | The Port to send OSC messages to.                                                       |
| `hrtype`            | `(string)` unknown        | The type of service where to get Heart Rate data from.                                  |
| `fitbiturl`         | `(string)` `String.Empty` | (FitbitHRtoWS Only) The WebSocket URL to connect to.                                    |
| `hyperatesessionid` | `(string)` `String.Empty` | (HypeRate Only) The HypeRate SessionId to subscribe to.                                 |
| `pulsoidwidget`     | `(string)` `String.Empty` | (Pulsoid Only) The Widget's URL to GET from an API.                                     |
| `pulsoidkey`        | `(string)` `String.Empty` | (PulsoidSocket Only) API Key for Pulsoid's Sockets.                                     |
| `textfilelocation`  | `(string)` `String.Empty` | (TextFile Only) Location of the text file where HR data should be read from             |
| `MaxHR`             | `(double)` 150            | Maximum range for the `HRPercent (float)` parameter                                     |
| `MinHR`             | `(double)` 0              | Minimum range for the `HRPercent (float)` parameter                                     |

## Launch Arguments

Arguments that can be run with the app

| Argument           | Description                                                |
|--------------------|------------------------------------------------------------|
| `--auto-start`     | Starts and Stops depending on if VRChat is running or not. |
| `--skip-vrc-check` | Ignores whether or not VRChat is open.                     |

## Console Commands

These commands can be input into the console window while running

| Command     | Parameters | Description                                        |
|-------------|------------|----------------------------------------------------|
| `starthr`   | none       | Starts the HR Listener                             |
| `stophr`    | none       | Stops the HR Listener                              |
| `restarthr` | none       | Restarts the HR Listener                           |
| `startbeat` | none       | Starts HeartBeat if it isn't enabled already.      |
| `stopbeat`  | none       | Stops the HeartBeat if it is already started       |
| `exit`      | none       | Exits the app properly. **NOT THE SAME AS CTRL+C** |

## Avatar Setup

Yes, avatar specific setup is required to use this properly. 
Please see the [Avatar Setup Guide](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/blob/main/AvatarSetup.md) (still a WIP) for more information.

## Demo
[![Click to see Video](https://i.imgur.com/KRRVyVK.png)](https://vimeo.com/678939624)
*This image link will take you to [vimeo](https://vimeo.com)*

## FAQ

### Is this ToS friendly

Unlike HRtoVRChat, yes, **HRtoVRChat_OSC** is ToS friendly. It does not modify the VRChat client in *any* way, and uses VRChat's built-in OSC 
feature to work.

### There's references to old HRtoVRChat files/media. Should I be concerned?

No, since this is the exact same system, just not with modifications, this tool and all references on the GitHub page are safe to use.

### The app quits as soon as I open it!

Make sure you have set all values in your `config.cfg` file, and everything is correct. Read the Config section above for more information.

### How do I use launch arguments?

Open a cmd window, navigate to where your `HRtoVRChat_OSC.exe` is located with the [cd](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/cd) 
command, type `HRtoVRChat_OSC.exe ` following the launch argument(s) you want to use. (separated with a space)

### Pulsoid FAQs

While both HypeRate and Pulsoid were affected by their respective API changes, only Pulsoid's setup has changed. First, the old Pulsoid/API config value is **removed**. Now, there's two ways to migrate.

**Method 1** - Official Sockets *(Not Recommended)*

If you can figure out how to get an `access_token` from Pulsoid, then set the `pulsoidkey` config value to that token, and set `hrtype` to `pulsoidsocket`.
https://github.com/pulsoid-oss/pulsoid-api#read_heart_rate_via_websocket

**Method 2** - Third-Party Sockets

Set `hrtype` to `pulsoid` and set `pulsoidwidget` to your Pulsoid widgetId.

To get your widgetId, go to your Pulsoid dashboard, go to the Widgets tab, hit Configure on any widget (I recommend the default one), and copy the long string of characters after the last slash in the URL (red/highlighted in attached image)

![widgetId](https://cdn.discordapp.com/attachments/887159486677151814/937249892995326012/unknown.png)

### I found a bug/issue with the program

Report a bug/issue on the [issues page](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/issues) 

**DO NOT ASK FOR SUPPORT ON THE ISSUES PAGE**

### I need help/support

Use the [discussions board](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/discussions) or join the 
[discord server](https://discord.gg/WF3B2r4xby)
