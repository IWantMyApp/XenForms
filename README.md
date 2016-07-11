# XenForms

XenForms is a Xamarin.Forms UI Designer. It allows you to open, visually manipulate, and save out XAML. There are two applications that collectively make up the product.

1. XenForms Toolbox
 
 The desktop application, containing the visual tree outline and property editors.
2. XenForms Designer

 The mobile application that plays the role of the design surface. The toolbox connects to the designer.
 
##Solutions

The Toolbox and Designer each have their respective Visual Studio/Xamarin Studio ```.sln``` file. Compiling and deploying locally should be straight-forward, as there are no custom build events.

First, start by opening ```designer.sln```, then compile and deploy ```Droid.csproj``` to your Android emulator or device.
Second, open ```toolbox.windows.sln```. Ensure that ```Toolbox.Windows.csproj``` is set as the **start-up project**. Now, compile and execute. There are two methods to connect to the design surface. If the Android device is not immediately displayed, click **File -> Connect** and type in the IP Address of the device.

1. toolbox.windows.sln (all csprojs)
  * Core
  * Core.Platform
  * Toolbox.UI
  * Toolbox.Windows
2. designer.sln (all csprojs)
 * Core (save as above)
 * Core.Platform (same as above)
 * Designer.Test
 * Designer.UI
 * Droid
 * iOS (not set to compile)

## Libraries and Frameworks
The major libraries and frameworks used are listed below:

1. Toolbox

 * Desktop UI: [Eto.Forms](https://github.com/picoe/Eto).
 * Installer: [Squirrel.Windows](https://github.com/Squirrel/Squirrel.Windows)
 * Android Communication: [SharpAdbClient](https://github.com/quamotion/madb)
 * XAML Open/Save: [Portable.Xaml](https://github.com/michaeled/Portable.Xaml)
 * Various others...

2. Designer

 * [Xamarin.Forms](https://www.xamarin.com/)
 * Communication: [websocket-sharp](https://github.com/sta/websocket-sharp)
