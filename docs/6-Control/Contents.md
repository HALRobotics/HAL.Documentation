## 6. Control

[6.1. Configure a Virtual Controller](#6.1.-configure-a-virtual-controller)

[6.2. Export a Procedure](#6.2.-export-a-procedure)

[6.3. Upload a Procedure](#6.3.-upload-a-procedure)

[6.4. Monitor Robot State \[Coming Soon\]](#6.4.-monitor-robot-state)

[6.5. Stream Positions to a Robot \[Coming Soon\]](#6.5.-stream-positions-to-a-robot)

---
### 6.1. Configure a Virtual Controller

#### Objective:

In this tutorial we'll look at how you can configure a virtual [Controller](../7-Glossary/Contents.md#controller) to match your real [Controller](../7-Glossary/Contents.md#controller) using the HAL Robotics Framework for Grasshopper.

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

Industrial [Controllers](../7-Glossary/Contents.md#controller) are typically comprised of core functionality, such as the ability to run a program, extended through optional extras, like communication protocols or multi-[Robot](../7-Glossary/Contents.md#manipulator) support. To ensure that we only try and interact with your real [Controller](../7-Glossary/Contents.md#controller) in a way that is compatible, be it through a network or with exported code, we have added a means to configure your [Controller](../7-Glossary/Contents.md#controller). The constituent parts of this are:

a.  Controller - this is essentially a computer to which your [Robot](../7-Glossary/Contents.md#manipulator) and [Signals](../7-Glossary/Contents.md#signal) are connected.

b.  Capabilities - these are how we organize what a [Controller](../7-Glossary/Contents.md#controller) can do and draw parallels between different manufacturers' [Controllers](../7-Glossary/Contents.md#controller). _Capabilities_ are things like the ability to [Upload](../7-Glossary/Contents.md#upload) code to the [Controller](../7-Glossary/Contents.md#controller) from a PC or the ability to read the values of [Signals](../7-Glossary/Contents.md#signal) remotely.

c.  Subsystems - these are similar to the options you have in your [Controller](../7-Glossary/Contents.md#controller). They are the actual software modules that implement different _Capabilities_.

#### How to:

All of these different parts are best explored with concrete examples so let's create a [Controller](../7-Glossary/Contents.md#controller) and look at how we can configure it. We can start by navigating to the **HAL Robotics** tab, **Cell** panel and placing a **Controller**. As this is a windowed component, we can double-click to open the catalog and choose our [Controller](../7-Glossary/Contents.md#controller) preset. For this example, I'm going to use the `IRC5 Compact V2`. When we select a [Controller](../7-Glossary/Contents.md#controller) a configuration page will pop-up.

The first thing we'll see at the top is the system version. In the case of ABB this is the Robotware version but for KUKA this would be KUKA System Software or in Universal Robots it will be the Polyscope version. It's important to note that these are version ranges so don't expect to see every point release listed. By changing the version we'll change which **Subsystems** are available. If I switch down to `5.14`, `EGM` will disappear from the options below because it was only introduced in Robotware 6.

The rest of the window is split in two; on the left is **Subsystem** and **Capability** selection and on the right is parametrization. In the left-hand column we can see the **Capabilities** listed with **Subsystems** that implement that **Capability** in a drop-down alongside. Let's look specifically at `Upload`. By hovering over the name, we can see that the `Upload` **Capability** enables [Procedure](../7-Glossary/Contents.md#procedure) [Uploading](../7-Glossary/Contents.md#upload) to a remote [Controller](../7-Glossary/Contents.md#controller). We can also see that there are two subsystems that offer this **Capability**, `PCSDK` and `Robot Web Services (RWS)`. `RWS` is built in to the latest Robotware versions but to use the `PCSDK` we need the option "PC Interface" on our [Controller](../7-Glossary/Contents.md#controller). If you don't have that option you can change **Subsystem** to ensure we use a compatible method to [Upload](../7-Glossary/Contents.md#upload) [Procedures](../7-Glossary/Contents.md#procedure) to your [Controller](../7-Glossary/Contents.md#controller). There may also be circumstances where we don't have any of the options installed or don't want access to a **Capability** for security purposes. In that case we can deactivate the **Capability** using its toggle. On the right-hand side of the window, we have the inputs to configure our **Subsystems**. Only active **Subsystems** are listed so if we deactivate both `EGM` **Capabilities** the `EGM` parameters will disappear. Once we have changed the relevant properties we can select "Configure" to apply our changes. Closing the window without configuring will leave the [Controller](../7-Glossary/Contents.md#controller) in an invalid, unconfigured state.

In future tutorials we'll look at some specific uses of our **Capabilities** and **Subsystems** for [exporting](../6-Control/Contents.md#6.2.-export-a-procedure) and [uploading](../6-Control/Contents.md#6.3.-upload-a-procedure) code for a real [Controller](../7-Glossary/Contents.md#controller).

---
### 6.2. Export a Procedure

#### Objective:

In this tutorial we'll [Export](../7-Glossary/Contents.md#export) some robot code ready to be run on a real [Controller](../7-Glossary/Contents.md#controller) using the HAL Robotics Framework for Grasshopper.

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

For the most part, the **Programming** we are doing in the HAL Robotics Framework for Grasshopper doesn't require our PCs to be in the loop whilst running our [Controllers](../7-Glossary/Contents.md#controller). The major advantage of that is that we delegate all of the **Motion Control** to the **Industrial Controller** which has been built specifically to execute code and run our [Robots](../7-Glossary/Contents.md#manipulator), resulting in excellent predictability, reliability and accuracy. The actual code that a [Controller](../7-Glossary/Contents.md#controller) requires will depend on its manufacturer and configuration. For example, ABB IRC5 [Controllers](../7-Glossary/Contents.md#controller) require code in the programming language RAPID, KUKA KRC4s require code in KUKA Robot Language (KRL) and Staubli [Robots](../7-Glossary/Contents.md#manipulator) will require VAL+ or VAL3 depending on their generation. Fortunately, the HAL Robotics Framework handles all of this for you as long as you select the right [Controller](../7-Glossary/Contents.md#controller) configuration.

#### How to:

To prepare to export we must have a [Procedure](../7-Glossary/Contents.md#procedure) ready and a [Controller](../7-Glossary/Contents.md#controller) in our document. To properly configure our [Controller](../7-Glossary/Contents.md#controller), we need to return to its configuration screen. This is the page you will see immediately after selecting your [Controller](../7-Glossary/Contents.md#controller) or you can get back to it by simply double-clicking on your [Controller](../7-Glossary/Contents.md#controller) component. We will use an ABB IRC5 as an example but the same principles hold true for any [Controller](../7-Glossary/Contents.md#controller) preset. When we open the configuration window there are two pieces of information that we need to check. The first, and simplest, is the _Language_. If the [Controller](../7-Glossary/Contents.md#controller) _Version_ is correctly set then this should be compatible with your real [Controller](../7-Glossary/Contents.md#controller) but you can always export a slightly different _Language_ if you want.

The other element to verify is the **Export Settings**. These are listed on the right-hand side under the _Language_ version. You should have a list of all of your [Procedures](../7-Glossary/Contents.md#procedure). If you haven't given them identifiable _Aliases_ now would be a good time to do so. There are three scenarios that we need to discuss with these **Export Settings**:

a.  Single Robot - For a single [Robot](../7-Glossary/Contents.md#manipulator) setup you will just need to make sure that your path is correctly set. For ABB [Controllers](../7-Glossary/Contents.md#controller) the path is the Task name in your real [Controller](../7-Glossary/Contents.md#controller) but for KUKA this is an index which can remain "1". You also have the option of completely deactivating the export of a [Procedure](../7-Glossary/Contents.md#procedure) using its toggle, or exporting a [Procedure](../7-Glossary/Contents.md#procedure) as a library, which means it won't have a code entry point. This could be useful if you have pre-configured initialisation or termination sequences in your [Controller](../7-Glossary/Contents.md#controller).

b.  Multi-Robot - The only additional check you need to make when using a multi-[Robot](../7-Glossary/Contents.md#manipulator) configuration is that your paths are all correct. Again, that's Tasks for ABB or the equivalent for other manufacturers.

c.  External Axes - The final unique configuration is for [External Axes](../7-Glossary/Contents.md#positioner). In the HAL Robotics Framework, we require each [Mechanism](../7-Glossary/Contents.md#mechanism) to have its own [Procedure](../7-Glossary/Contents.md#procedure). With [External Axes](../7-Glossary/Contents.md#positioner) we actually want to merge a number of [Procedures](../7-Glossary/Contents.md#procedure) into one to [Export](../7-Glossary/Contents.md#export) correctly. We can do this by simply dragging the [External Axes'](../7-Glossary/Contents.md#positioner) [Procedure(s)](../7-Glossary/Contents.md#procedure) onto the main [Robot's](../7-Glossary/Contents.md#manipulator) [Procedure](../7-Glossary/Contents.md#procedure). This marks it as a child [Procedure](../7-Glossary/Contents.md#procedure) of the [Robot](../7-Glossary/Contents.md#manipulator) and they will be [Exported](../7-Glossary/Contents.md#export) together. When using this kind of configuration please make sure that you have also setup your **Joint Mappings** correctly for your [External Axes](../7-Glossary/Contents.md#positioner). This can be done during the [Joint](../7-Glossary/Contents.md#joint) creation when assembling a [Mechanism](../7-Glossary/Contents.md#mechanism) from scratch or using the _Mapping_ input on the [Positioner](../7-Glossary/Contents.md#positioner) component. **Mappings** are zero-based in the HAL Robotics Framework and will automatically be converted at [Export](../7-Glossary/Contents.md#export) to match the format of the real [Controller](../7-Glossary/Contents.md#controller).

Now that our [Controller](../7-Glossary/Contents.md#controller) is configured, we can place [Export](../7-Glossary/Contents.md#export) component from the **HAL Robotics** tab, **Control** panel. We can hook up our [Controller](../7-Glossary/Contents.md#controller), **Solution** and assign a path to the _Destination_. When we run the component by toggling _Export_ to `true` this will generate our code and give us the paths to all exported files as an output. In the second overload of this component there's one additional input worth discussing, _Mode_. `Inline` mode will create a dense code file with as little declarative code as possible. `Predeclaration` mode will do just the opposite, it will create variables wherever possible to make it easier to change things by hand should you want to. For most scenarios we recommend `Inline` as it produces shorter code and is much faster.

As a final note in this tutorial, we know that there are circumstances where you may need to add very specific lines of code to your [Exports](../7-Glossary/Contents.md#export). This could be to trigger a particular [Tool](../7-Glossary/Contents.md#end-effector), send a message or call another piece of code. You can do this using **Custom Actions**. These are found in the **HAL Robotics** tab, **Procedure** panel. You can add any text to the _Expression_ input in double quotes ("") and it will be [Exported](../7-Glossary/Contents.md#export) verbatim. If your **Custom Action** causes the [Robot](../7-Glossary/Contents.md#manipulator) to [Wait](../7-Glossary/Contents.md#wait-action) or some other **Simulatable** [Action](../7-Glossary/Contents.md#action) to occur you can add a [Procedure](../7-Glossary/Contents.md#procedure) to the _Simulation_ input. Just remember that regardless of what you add to the _Simulation_, only what you put in the _Expression_ will be [Exported](../7-Glossary/Contents.md#export).

---
### 6.3. Upload a Procedure
#### Coming Soon

[//]: # (Use RobotStudio as part of demo and upload to Virtual Controller - mention that it should work with other manufacturers' offline programming solutions)

---
### 6.4. Monitor Robot State \[C\# only\]
#### Coming Soon

---
### 6.5. Stream Positions to a Robot \[C\# Only\]
#### Coming Soon

---