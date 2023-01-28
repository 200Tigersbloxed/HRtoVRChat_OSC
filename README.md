# HRtoVRChat_OSC

Stream your Heart Rate onto your VRChat avatar via. the use of OSC Avatar Parameters.

[![Discord](https://img.shields.io/discord/887157106472550422.svg?color=%237289da&label=discord&style=for-the-badge)](https://discord.gg/WF3B2r4xby) 
[![license](https://img.shields.io/github/license/200Tigersbloxed/HRtoVRChat_OSC?style=for-the-badge)](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/blob/main/LICENSE) 
![downloads](https://img.shields.io/github/downloads/200Tigersbloxed/HRtoVRChat_OSC/total?style=for-the-badge) 
[![lastcommit](https://img.shields.io/github/last-commit/200Tigersbloxed/HRtoVRChat_OSC?style=for-the-badge)](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/commits/main) 
[![issues](https://img.shields.io/github/issues/200Tigersbloxed/HRtoVRChat_OSC?style=for-the-badge)](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/issues)

## Using the Launcher

**The Launcher will manage your HRtoVRChat for you from a single executable!**

1) Download HRtoVRChatLauncher from the [Downloads Page](https://hrtovrchat.fortnite.lol/download#h.ha8hgsfz56g2)
2) Launch the Application
    + If you're on Linux, you may need to make the file executable first

## Installing the UI Helper

**It is always recommended to use the UI as it heavily simplifies the process of setting up the software! Use the Launcher to simplify the install proccess.**

1) Download HRtoVRChat from the [Downloads Page](https://hrtovrchat.fortnite.lol/download#h.ha8hgsfz56g2)
2) Extract the folder to a directory, and launch the application
3) Go to the Updates tab, and hit the Update Software
    + After the process completes, hit the Refresh button and make sure the Installed Version and Latest Version match
5) Return to the Program tab, and Start the app once, then wait for it to close
6) Go to the Config tab and configure all values you'd like to

## Installing the Console App

1) Download `HRtoVRChat_OSC` from the [Downloads Page](https://hrtovrchat.fortnite.lol/download#h.w34oohk7gucz)
2) Extract the folder to a directory
3) Run once, the app will auto close.
4) Open `config.cfg` (in the same directory) and change the config values to your likings.

## Config

After installing, you then have to tell HRtoVRChat_OSC which service you'll be using to get your heart rate. Below is a table of which Heart Rate monitors are supported. 
If you'd like to request support, please use the [Discussions Board](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/discussions) to do so.

Please also consider contributing to add more support with other Heart Rate Monitors.

| Device          | HRType          | Info                                                                                           |
|-----------------|-----------------|------------------------------------------------------------------------------------------------|
| FitbitHRtoWS    | `fitbithrtows`  | https://github.com/200Tigersbloxed/FitbitHRtoWS                                                |
| HRProxy         | `hrproxy`       | HRProxy Custom Reader                                                                          |
| HypeRate        | `hyperate`      | https://www.hyperate.io/                                                                       |
| Pulsoid         | `pulsoid`       | https://pulsoid.net/ https://www.stromno.com/                                                  |
| PulsoidSocket   | `pulsoidsocket` | https://github.com/200Tigersbloxed/HRtoVRChat_OSC/wiki/Upgrading-from-Pulsoid-to-PulsoidSocket |
| Stromno         | `stromno`       | https://www.stromno.com/                                                                       |
| TextFile        | `textfile`      | A .txt file containing only a number                                                           |
| Omnicept        | `omnicept`      | https://www.hp.com/us-en/vr/reverb-g2-vr-headset-omnicept-edition.html                         |
| SDK             | `sdk`           | https://github.com/200Tigersbloxed/HRtoVRChat_OSC/blob/main/SDK.md                             |

**[For a full list of all the supported devices, please click here](https://hrtovrchat.fortnite.lol/supported-devices)**

Take note of HRType, as you'll need to know which you you have to put in the `hrtype` config value.

### Extensions

Device support can be extended with our [SDK](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/blob/main/SDK.md). If you don't want to add support natively, for whatever reason that may be (such as complicated setup), then data can be passed through with our SDK. Feel free to create a PR if you've made an SDK you'd like to showoff.

| SDK             | Developer                                         | URL                                                          |
|-----------------|---------------------------------------------------|--------------------------------------------------------------|
| FitbitWebOSC    | [ButterScotchV](https://github.com/ButterscotchV) | https://github.com/ButterscotchV/FitbitWebOSC                |

**Note**: Extensions are not maintained by HRtoVRChat_OSC, they are maintained by its repositories' respective contributor(s). Please direct any issues with their software/SDK there, not here.

### Values

Below is a table of all the config values and a description. Please update the config accordingly.

| Config Value        | Default Value             | Description                                                                             |
|---------------------|---------------------------|-----------------------------------------------------------------------------------------|
| `ip`                | `(string)` `127.0.0.1`    | The IP to send OSC messages to.                                                         |
| `port`              | `(int)` 9000              | The Port to send OSC messages to.                                                       |
| `hrtype`            | `(string)` unknown        | The type of service where to get Heart Rate data from.                                  |
| `fitbiturl`         | `(string)` `String.Empty` | (FitbitHRtoWS Only) The WebSocket URL to connect to.                                    |
| `hrproxyId`         | `(string)` `String.Empty` | (HRProxy Only) The HRProxy Custom Reader Id.                                            |
| `hyperatesessionid` | `(string)` `String.Empty` | (HypeRate Only) The HypeRate SessionId to subscribe to.                                 |
| `pulsoidwidget`     | `(string)` `String.Empty` | (Pulsoid Only) The Widget's URL to GET from an API.                                     |
| `stromnowidget`     | `(string)` `String.Empty` | (Stromno Only) The Widget's URL to GET from an API.                                     |
| `pulsoidkey`        | `(string)` `String.Empty` | (PulsoidSocket Only) API Key for Pulsoid's Sockets.                                     |
| `textfilelocation`  | `(string)` `String.Empty` | (TextFile Only) Location of the text file where HR data should be read from             |
| `MaxHR`             | `(double)` 150            | Maximum range for the `HRPercent (float)` parameter                                     |
| `MinHR`             | `(double)` 0              | Minimum range for the `HRPercent (float)` parameter                                     |

## Launch Arguments

Arguments that can be run with the app

| Argument              | Description                                                                    |
|-----------------------|--------------------------------------------------------------------------------|
| `--auto-start`        | Starts and Stops depending on if VRChat is running or not.                     |
| `--skip-vrc-check`    | Ignores whether or not VRChat is open.                                         |
| `--neos-bridge`       | Initializes the bridge to send parameter data to Neos.                         |
| `--use-01-bool`       | Serializes bools as floats instead of as bools.                                |
| `--no-avatars-folder` | Will scan a userid directory for avatars files if no Avatars directory exists. |

## Console Commands

These commands can be input into the console window while running

| Command         | Parameters | Description                                        |
|-----------------|------------|----------------------------------------------------|
| `starthr`       | none       | Starts the HR Listener                             |
| `stophr`        | none       | Stops the HR Listener                              |
| `restarthr`     | none       | Restarts the HR Listener                           |
| `startbeat`     | none       | Starts HeartBeat if it isn't enabled already       |
| `stopbeat`      | none       | Stops the HeartBeat if it is already started       |
| `refreshconfig` | none       | Refreshes the config from file                     |
| `biassdk`       | SDK Name   | Sets a preferred SDK to pull data from             |
| `unbiassdk`     | none       | Removes a bias towards any SDK                     |
| `destroysdk`    | SDK Name   | Removes an SDK by name                             |
| `exit`          | none       | Exits the app properly. **NOT THE SAME AS CTRL+C** |

## Avatar Setup

Yes, avatar specific setup is required to use this properly. 
Please see the [Avatar Setup Guide](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/blob/main/AvatarSetup.md) (still a WIP) for more information.

## Demo
[![Click to see Video](https://i.imgur.com/KRRVyVK.png)](https://vimeo.com/678939624)
*This image link will take you to [vimeo](https://vimeo.com)*

## Wiki

See [the wiki](https://github.com/200Tigersbloxed/HRtoVRChat_OSC/wiki) for more information
