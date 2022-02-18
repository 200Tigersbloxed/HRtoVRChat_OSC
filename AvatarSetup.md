# HRtoVRChat_OSC Avatar Setup

The HRtoVRChat-Prefab (which can be found on the Latest Release Artifacts for HRtoVRChat_OSC) contains contents to get started with HRtoVRChat_OSC. Please read through this **whole document** for information on everything included.

**ATTENTION**

This guide assumes that you have *basic Animation/Animator Controller* knowledge in Unity. Please see the following guides for learning Animation and Animator Controllers in Unity:

https://learn.unity.com/course/introduction-to-3d-animation-systems

https://learn.unity.com/tutorial/animator-controllers-2019-3

https://learn.unity.com/tutorial/controlling-animation

## Video Tutorial

There's now a video tutorial which demonstrates how to setup the avatar with the [AV3Manager](https://github.com/VRLabs/Avatars-3.0-Manager) tool, which makes setup a thousand times easier! I'd *heavily* suggest following this video guide if you have troubles with SDK3.

https://vimeo.com/629586690

## HRContainer

The Example HRContainer for an avatar. This was made to work when put inside the avatar's chest bone, if you move it somewhere else, you may have to redo the animations provided. (30 animations/1 layers per 10 animations/3 layers in all)

Be sure that you are also using the provided FX Layer, or have implemented the Layers (excluding Base Layer) into your current FX Controller.

## hrEM

The Expression Menu for the avatar. This is not required, if you'd like to replace this, you can.

## hrEP

The Expression Parameters are required for the avatar. If you'd like to use your parameters, below is a table of the parameters and descriptions.

| Parameter Name | Parameter Type | Parameter Default | Parameter Saved | Description                                                                           |
|----------------|----------------|-------------------|-----------------|---------------------------------------------------------------------------------------|
| onesHR         | `int`          | 0                 | false           | Ones spot in the Heart Rate reading; 12**3** *(required)*                             |
| tensHR         | `int`          | 0                 | false           | Tens spot in the Heart Rate reading; 1**2**3 *(required)*                             |
| hundredsHR     | `int`          | 0                 | false           | Hundreds spot in the Heart Rate reading; **1**23 *(required)*                         |
| isHRConnected  | `bool`         | false             | false           | Returns whether the connection is valid or not                                        |
| isHRActive     | `bool`         | false             | false           | Returns whether the device's connection is valid or not                               |
| isHRBeat       | `bool`         | false             | false           | Estimation on when the heart is beating                                               |
| HRPercent      | `float`        | false             | false           | Range of HR between the MinHR and MaxHR config value                                  |
| HR             | `int`          | 0                 | false           | Returns the raw HR, ranged from 0 - 255. (Not Officially Supported; do what you want) |

## hr/HRTestingMaterial

Texture and Material for the background. Can be replaced.

## numbers

### anims

Includes all the animations for changing the numbers. You will need to redo all of these, or use [AV3Manager](https://github.com/VRLabs/VRChat-Avatars-3.0) to convert the animations to your avatar automatically.

Here's how you can redo them manually:
For each animation for all number spots (ones, tens, and hundreds) do the following:
1) Open the animation in the Animation window
2) Clear all the keys
3) Hit the record button
4) Replace the Material of the number spot with the number in the animation-name
  + For example: If the animation name is `ones-one`, you will set the material to the `one` material (found in the materials folder)
5) Stop recording
6) Copy the keyframe to 1 frame after
7) Repeat for all other animations

This also includes the FX Layer for setting the animations. There is 1 layer for each numbered spot (ones, tens, and hundreds). Here's how each layer works:

1) The layer starts with all the animations for the number spot.
2) The Any State is connected to all of the Animations
  + The Conditions for the Any State are `if (numberspot)HR Equals (animation number [example: ones-one == 1]) -> animation`
3) The Animations are connected to the Exit
  + The Conditions for the Exit are `if (numberspot)HR NotEqual (animation number [example: ones-one != 1]) -> exit`
4) Repeat for all other Layers

### materials

Contains the materials for the numbers. These can be changed to whatever shader you'd like but were tested with
PC: XSToon3
Quest: Toon Lit

### 1rowspritesheet

The texture/sprite sheet for the materials. Cropped with Tile Size and offset with Offset.

### number

An example number. You probably won't need to touch this.
