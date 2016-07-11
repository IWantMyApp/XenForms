# XenForms

XenForms is a Xamarin.Forms UI Designer. It allows you to open, visually manipulate, and save out XAML. There are two applications that collectively make up the product.

1. XenForms Toolbox
 
 The desktop application, containing the visual tree outline and property editors.
2. XenForms Designer

 The mobile application that plays the role of the design surface. The toolbox connects to the designer.
 
##Solutions

The Toolbox and Designer each have their respective Visual Studio/Xamarin Studio ```.sln``` file. Compiling and deploying locally should be straight-forward, as there are no custom build events.

First, start by opening ```designer.sln```, then compile and deploy ```Droid.csproj``` to your Android emulator or device.
Second, open ```toolbox.windows.sln```. Ensure that ```Toolbox.Windows.csproj``` is set as the **start-up project**. Now, compile and execute. There are two methods to connect to the design surface. If the Android device is not immediately displayed, click **File -> Connect** and type in the IP Address displayed on the design surface (mobile app).

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
 * Various others...

##Networking

The toolbox and designer communicate over a websocket, hence [websocket-sharp](https://github.com/sta/websocket-sharp). All messages derive from ```XenForms.Core.Messages.XenMessage```. A set of factory methods, such as ```XenForms.Core.Messages.XenMessage.Create<>()``` exits and should be used to instantiate a message object. Messages are actions, such as *get the visual tree*, *open a XAML file*, *return object X's properties*, *set a property value on object x*, and others.

When a message is received by the designer, it scans a list of registered ***reactions***. A reaction is any class that derives from the ```XenForms.Core.Designer.Reaction``` abstract base class. However, most reactions will derive from ```XenForms.Designer.XamarinForms.UI.Reactions.XamarinFormsReaction``` as it provides useful properties and methods to visually manipulate the design surface.

Reactions must be registered before a message can be dispatched to it. This is done in ```XenForms.Designer.XamarinForms.UI.DesignerAppEvents```.

**Example**

```cs
XamarinFormsReaction.Register<GetVisualTreeRequest, GetVisualTreeReaction<VisualElement>>(page);
```

##Roadmap and Contributing 
I have a [project board](https://trello.com/b/dAZJ4QkT/xenforms-beta-roadmap) on Trello that I use to maintain my backlog of work. Rewriting the XAML load & save features is crucial. The project started as a visual tree inspector, and saving XAML was *bolted* on in the last few cycles. You can see that by viewing the ```XamlPostProcessor`` type.

I happily accept pull requests. :P
