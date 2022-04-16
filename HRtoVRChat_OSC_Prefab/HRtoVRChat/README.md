# HRtoVRChat Prefab
Version 2

Created by 200Tigersbloxed

## Terminology

+ Avatar - The GameObject containing the VRC Avatar Descriptor Component
+ v1 - The old HRtoVRChat prefab
+ v2 - This new HRtoVRChat prefab
+ FX Layer - The Animator Controller being used as the FX Layer
+ TargetHRObject - The GameObject containing the TargetHRObject Component

## Getting Started

The HRtoVRChat Prefab v2 was designed to be as simple as possible with a quick setup.

### Preparing your Avatar

Unlike v1, v2 is designed to be snap-able, meaning you can put the prefab wherever you want, and it'll work.

> # ⚠️ Before continuing any further! ⚠️

**Please make a backup of your avatar!**

This system is **entirely automated** and may cause damage to your Avatar!

> *you have been warned...*

Inside of the `HRtoVRChat` folder, find the `HRContainer` (1) prefab.
Take this prefab and move it anywhere into your Avatar (2).
Select the HRContainer prefab in the hierarchy (3), and find the `TargetHRObject` component (4).

![Preparing your Avatar](https://i.imgur.com/Y0SGDAH.png)

### The TargetHRObject Component

The TargetHRObject component is used by the Editor to grab specific settings for the HRContainer being worked on.
This includes information like which materials to use for the number, references for number Transforms, etc.

All of the following fields will already be filled out for you:
+ `Ones Icon`
+ `Tens Icon`
+ `Hundreds Icon`
+ `Ones Materials`
+ `Tens Material`
+ `Hundreds Materials`

Please do not change the `Ones Icon`, `Tens Icon`, or `Hundreds Icon` fields, unless they're null, as they're specific to the GameObject.

You may change the Materials if you wish to.

For the `Avatar Root` field, set that to your Avatar.

For the `FX Controller` field, set that to your FX Layer.

![The TargetHRObject Component](https://i.imgur.com/uqP5ax2.png)

### Open the Editor Window

On your top bar, find the `Window` button, click it, then click the `HRtoVRChat_SDK` button, and the main window will open.

![Opening the Window](https://i.imgur.com/1OCuAlD.png)

You should now see a window popup containing all the functions required to setup your avatar.

### The Editor Window

The GitHub button will just take you to the GitHub Repository.

Underneath the `Discovered TargetHRObjects` header, you will see a list of buttons that correspond to every GameObject in the scene that has a TargetHRObject Component.
Click the button for the HRContainer you'll be working with.

You will then see a label Saying `SelectedHRTarget: [name]` (1), click that.

You may press the `Create Animations!` Button (3) whenever you're ready to continue.

#### Options (2)

+ friendlyName - An identifier to be used in the Layers corresponding to your HRContainer
  + This should be different for each HRContainer
+ useSmallerHR - Uses only one HR int parameter instead of the conventional three parameters
  + Note: This will lock your HR from 0-255, but to be fair, if your HR is over 255, you need to go to the ER.
+ deleteComponentOnDone - Will remove the TargetHRObject Component on the HRContainer when finished
+ overwriteLayers - Will overwrite all layers that have the same name
  + This will not overwrite any of your layers outside of the HR Layers.

![The Editor Window](https://i.imgur.com/6DCNRfa.png)

Once finished, you will see a DialogWindow saying `Completed Operation!`

If you get a Dialog Window saying 'Failed to Complete Operation!', then please check your Console window for errors.

### Finishing Up

Once the process is complete, make sure to Apply your FX Controller to your Avatar

Don't forget to add any parameters used to your Expression Parameters

**If you used a Smaller HR**:

Add the `HR` (int) Parameter

**If you did NOT use a Smaller HR**:

Add the following Parameters:
+ `onesHR` (int)
+ `tensHR` (int)
+ `hundredsHR` (int)

Now you're all set to publish!

## Source Code

All code is visible in the `Scripts` folder (under the respective LICENSE), but
is also mirrored at 
[this gist](https://gist.github.com/200Tigersbloxed/6c5798cb959b8073deaf5d6a3c17454f)