## 3. Motion

[3.1. Create a Target](#3.1.-create-a-target)

[3.2. Modify a Target](#3.2.-modify-a-target)

[3.3. Change Motion Settings](#3.3.-change-motion-settings)

[3.4. Combine Procedures](#3.4.-combine-procedures-and-the-procedure-browser)

[3.5. Synchronize Motion](#3.5.-synchronize-motion)

[3.6. Resolve Targets](#3.6.-coupled-motion-and-resolving-targets)

[3.7. Add Target Constraints \[Coming Soon\]](#3.7.-add-target-constraints)

[3.8. Add Mechanism Constraints \[Coming Soon\]](#3.8.-add-mechanism-constraints)

---
### 3.1. Create a Target

#### Objective:

In this tutorial we'll look at the different ways that we can create [Targets](../7-Glossary/Contents.md#target) in the HAL Robotics Framework for Grasshopper.

#### Demo Files:

[3.1 - Create a Target.gh](../ExampleFiles/Tutorials/3.1%20-%20Create%20a%20Target.gh)

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

[Targets](../7-Glossary/Contents.md#target) are the way we define to where a [Robot](../7-Glossary/Contents.md#manipulator) should move. They do not, in and of themselves, define how a [Robot](../7-Glossary/Contents.md#manipulator) should move so we can mix and match [Target](../7-Glossary/Contents.md#target) creation techniques with different types of [Move](../7-Glossary/Contents.md#motion-action).

#### How to:

There are 2 main ways of creating [Targets](../7-Glossary/Contents.md#target), both of which are available from **Target** component in the **HAL Robotics** tab, **Motion** panel.

The first way of creating a [Target](../7-Glossary/Contents.md#target) is a [Cartesian](../7-Glossary/Contents.md#cartesian-space) [Target](../7-Glossary/Contents.md#target) from a **Frame**. When a [Robot](../7-Glossary/Contents.md#manipulator) moves to a [Cartesian](../7-Glossary/Contents.md#cartesian-space) [Target](../7-Glossary/Contents.md#target) the active [TCP](../7-Glossary/Contents.md#endpoint) of our [Robot](../7-Glossary/Contents.md#manipulator) will align with the [Target's](../7-Glossary/Contents.md#target) position and orientation. If we create a **Frame** in Grasshopper, such as by selecting a point and making it the origin of an XY Plane. When we assign this to our [Target](../7-Glossary/Contents.md#target) component and hide the previous components, we can see our [Target](../7-Glossary/Contents.md#target) axes centred on the point we had selected. Of course, because this is Grasshopper, if we move that point our [Target](../7-Glossary/Contents.md#target) moves with it. We can also set the [Reference](../7-Glossary/Contents.md#reference) here. Please take a look at the [References tutorial](../2-Cell/Contents.md#2.2.-create-a-reference) to see how those work.

The Z axis of this [Target](../7-Glossary/Contents.md#target) is pointing up. In our [Tool creation tutorial](../2-Cell/Contents.md#2.2.-create-a-tool), we recommended that the Z axis of [Tool](../7-Glossary/Contents.md#end-effector) [TCPs](../7-Glossary/Contents.md#endpoint) point out of the [Tool](../7-Glossary/Contents.md#end-effector), following the co-ordinate system flow of the Robot itself. That means that when our Robot reaches the [Target](../7-Glossary/Contents.md#target) we've just created, the [Tool](../7-Glossary/Contents.md#end-effector) will also be pointing up. That may be desirable but remember that setting the orientation of your [Targets](../7-Glossary/Contents.md#target) is just as important as their positions and therefore creating the correct **Frame** is critical. We have found a number of cases where creating [Targets](../7-Glossary/Contents.md#target) facing directly downwards, with their X axes towards world -X is a useful default and have added a shortcut to create those by-passing points directly to a [Target](../7-Glossary/Contents.md#target) parameter.

The other primary way of creating a [Target](../7-Glossary/Contents.md#target) is in [Joint space](../7-Glossary/Contents.md#joint-space), that is by defining the desired position of each active [Joint](../7-Glossary/Contents.md#joint) of your [Robot](../7-Glossary/Contents.md#manipulator). We can do this by changing the template of our [Target](../7-Glossary/Contents.md#target) component by right-clicking and selecting **From Joint Positions**. The inputs are now slightly different. We need to pass a [Mechanism](../7-Glossary/Contents.md#mechanism) into the component to visualize the [Robot](../7-Glossary/Contents.md#manipulator) in its final position and ensure that the _Positions_ we give are valid. The other required input is a list of the _Positions_ of each active [Robot](../7-Glossary/Contents.md#manipulator) [Joint](../7-Glossary/Contents.md#joint). It's important to note that unlike many other inputs in the HAL Robotics Framework these _Positions_ must be defined in SI units (metres and radians) because they can legitimately contain both lengths and angles. If we create a few sliders, six for a six-axis [Robot](../7-Glossary/Contents.md#manipulator), merge into a single list and ensure that we're in SI units we can visualize the final position of our [Robot](../7-Glossary/Contents.md#manipulator) at these [Joint](../7-Glossary/Contents.md#joint) positions.

Using these two [Target](../7-Glossary/Contents.md#target) creation methods we can get our [Robots](../7-Glossary/Contents.md#manipulator) to perform any motion we require. That being said, particularly in a Grasshopper-centric workflow, we often want to follow a Curve as we did in the [Getting Started](../1-Getting-Started/Contents.md#1.-getting-started) tutorial. To facilitate that we have included a **From Curve** template in the **Target** component. This variation of the component takes a _Curve_ and creates [Targets](../7-Glossary/Contents.md#target) to approximate the _Curve_. We do this by subdividing (or discretizing) it. The _Discretization_ can be controlled using the input and changed between straight line segments only or line segments and arcs. The accuracy of the approximation can be controlled using this _Tolerance_ input. The distance given here is the maximum allowed deviation from the input _Curve_.

All of these [Target](../7-Glossary/Contents.md#target) creation options give exactly the same types of [Target](../7-Glossary/Contents.md#target) so can be used interchangeably in your [Move](../7-Glossary/Contents.md#motion-action) components.

---
### 3.2. Modify a Target

#### Objective:

In this tutorial we'll look at the different utilities to modify [Targets](../7-Glossary/Contents.md#target) built in to the HAL Robotics Framework for Grasshopper.

#### Demo Files:

[3.2 - Modify a Target.gh](../ExampleFiles/Tutorials/3.2%20-%20Modify%20a%20Target.gh)

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

[Targets](../7-Glossary/Contents.md#target) are the way we define to where a [Robot](../7-Glossary/Contents.md#manipulator) should move and defining them correctly is a fundamental step in the programming of a [Procedure](../7-Glossary/Contents.md#procedure). To facilitate certain recurring cases, we provide inbuilt [Target](../7-Glossary/Contents.md#target) modifiers.

#### How to:

The **Transform Target** component from the **HAL Robotics** tab, **Motion** panel offers several different ways of realigning your [Targets](../7-Glossary/Contents.md#target) to face vectors, curve tangents, mesh or surface normal, or any other direction you choose with a **Free Transformation**. To start let's stick with the **Parallel to Vector** template and try and get all of our [Targets](../7-Glossary/Contents.md#target) to face the base of our [Robot](../7-Glossary/Contents.md#manipulator) which happens to be at the world origin. This is often useful if your [Tool](../7-Glossary/Contents.md#end-effector) is free to rotate around its Z axis and can help to avoid reachability issues. We can use the **Target Properties** component to get the location of our [Targets](../7-Glossary/Contents.md#target), create an XY plane to represent the [Robot](../7-Glossary/Contents.md#manipulator) base frame and **Vector from 2 Points** component to find the vector between our [Targets](../7-Glossary/Contents.md#target) and the origin. We can use our new vector as the _Direction_ input in our [Target](../7-Glossary/Contents.md#target) modifier and pass our [Targets](../7-Glossary/Contents.md#target) to their input. Ensure the original [Targets](../7-Glossary/Contents.md#target) are hidden to make it easier to see our results. We should see that our [Targets](../7-Glossary/Contents.md#target) are all pointing towards the origin but not necessarily in the way we were expecting. That is because the _Axis_ defaults to `Z`. If we change this to `X` then we should see something closer to what we want. We can also _Flip_ the vectors so that our [Targets](../7-Glossary/Contents.md#target) face the opposite direction. If we don't want our [Targets](../7-Glossary/Contents.md#target) to all be facing down towards the origin as they are now, the we can discard the Z component of our input vector to keep our [Targets](../7-Glossary/Contents.md#target) horizontal.

Most variations of the **Transform Target** component work in a similar way so please play with those to discover what they can do. The one exception is the **Free Transform** template. This allows us to apply any transformation we want to our [Targets](../7-Glossary/Contents.md#target) by simply specifying translations and reorientations. The default _Reference_ for this transformation is the [Target](../7-Glossary/Contents.md#target) itself but we can specify a Plane as the _Reference_ to change the way our [Targets](../7-Glossary/Contents.md#target) are transformed.

The last [Target](../7-Glossary/Contents.md#target) modifier we're going to look at in this tutorial is the **Target Filter** component from the **HAL Robotics** tab, **Motion** panel. To demonstrate this functionality, we can divide a curve into a large number of [Targets](../7-Glossary/Contents.md#target). After a certain point adding more [Targets](../7-Glossary/Contents.md#target) is unnecessary, slows down code export and, in some circumstances, code execution. The **Target Filter** component takes our [Targets](../7-Glossary/Contents.md#target) and splits them into two lists, those that meet the _Position_ and _Orientation_ tolerances (_Remaining_) and those that don't (_Discarded_). It is therefore useful to hide the component output and display only the _Remaining_ [Targets](../7-Glossary/Contents.md#target). If we filter the targets to the nearest centimeter, we should see that far fewer remain and by changing the _Position_ tolerance the number of _Remaining_ [Targets](../7-Glossary/Contents.md#target) will vary accordingly.

---
### 3.3. Change Motion Settings

#### Objective:

In this tutorial we'll look at how to change the way in which a [Robot](../7-Glossary/Contents.md#manipulator) moves to you [Targets](../7-Glossary/Contents.md#target) using the HAL Robotics Framework for Grasshopper.

#### Demo Files:

[3.3 - Change Motion Settings.gh](../ExampleFiles/Tutorials/3.3%20-%20Change%20Motion%20Settings.gh)

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

**Motion Settings** control the way in which a [Robot](../7-Glossary/Contents.md#manipulator) moves between [Targets](../7-Glossary/Contents.md#target). They combine settings for the **Space**, **Speeds**, **Accelerations**, **Blends** and a number of other parameters to control how a [Robot](../7-Glossary/Contents.md#manipulator) gets to its destination.

#### How to:

The **Motion Settings** component can be found in the **HAL Robotics** tab, **Motion** panel and can be directly passed in to the **Move** component. The four settings mentioned previously are the first inputs on this component.

_Space_ controls which path the [Robot](../7-Glossary/Contents.md#manipulator) takes to a [Target](../7-Glossary/Contents.md#target). In `Cartesian` mode the [TCP](../7-Glossary/Contents.md#endpoint) moves in a very controlled manner along a straight line or arc. This is probably the easier motion type to visualize but can cause problems when moving between configurations or when trying to optimise cycle times. Moving in [Joint space](../7-Glossary/Contents.md#joint-space) means that each [Joint](../7-Glossary/Contents.md#joint) will move from one position to the next without consideration for the position of the [TCP](../7-Glossary/Contents.md#endpoint). [Joint space](../7-Glossary/Contents.md#joint-space) [Moves](../7-Glossary/Contents.md#motion-action) always end in the same configuration and are not liable to [Singularities](../7-Glossary/Contents.md#(kinematic)-singularity). It's often useful to start your [Procedures](../7-Glossary/Contents.md#procedure) with a [Motion](../7-Glossary/Contents.md#motion-action) in [Joint space](../7-Glossary/Contents.md#joint-space) to ensure your [Robot](../7-Glossary/Contents.md#manipulator) is always initialized to a known position and configuration. It's worth noting that when using [Joint space](../7-Glossary/Contents.md#joint-space) [Motions](../7-Glossary/Contents.md#motion-action) your [Toolpath](../7-Glossary/Contents.md#toolpath) will be dotted until the [Procedure](../7-Glossary/Contents.md#procedure) is [Solved](../7-Glossary/Contents.md#solving) because we can't know ahead of time exactly where the [TCP](../7-Glossary/Contents.md#endpoint) will go during that [Motion](../7-Glossary/Contents.md#motion-action). Once [Solved](../7-Glossary/Contents.md#solving), you will see the path your [TCP](../7-Glossary/Contents.md#endpoint) will actually take in space.

_Speed_ settings, as the name implies, constrain the speed of your [Robot](../7-Glossary/Contents.md#manipulator). They can be declared in [Cartesian space](../7-Glossary/Contents.md#cartesian-space) to directly limit the position or orientation _Speed_ of the [TCP](../7-Glossary/Contents.md#endpoint). You can also constrain the _Speeds_ of your [Robot's](../7-Glossary/Contents.md#manipulator) [Joints](../7-Glossary/Contents.md#joint) using the second overload or combine the two using the third overload. Please note that not all [Robot](../7-Glossary/Contents.md#manipulator) manufacturers support programmable [Joint](../7-Glossary/Contents.md#endpoint) speed constraints so there may be variations between your simulation and real [Robot](../7-Glossary/Contents.md#manipulator) when they are used.

_Acceleration_ settings constrain the acceleration of your [Robot](../7-Glossary/Contents.md#manipulator). They function in exactly the same way as the _Speeds_, constraining [Cartesian](../7-Glossary/Contents.md#cartesian-space) acceleration, [Joint](../7-Glossary/Contents.md#joint-space) acceleration or both.

_[Blends](../7-Glossary/Contents.md#blend)_ sometimes called zones or approximations change how close the [Robot](../7-Glossary/Contents.md#manipulator) needs to get to a [Target](../7-Glossary/Contents.md#target) before moving on to the next. It's useful to consider your _[Blends](../7-Glossary/Contents.md#blend)_ carefully because increasing their size can drastically improve cycle time by allowing the [Robot](../7-Glossary/Contents.md#manipulator) to maintain speed instead of coming to a stop at each [Target](../7-Glossary/Contents.md#target). _[Blends](../7-Glossary/Contents.md#blend)_ are most easily visualized in _Position_. If we set a 100 mm radius [Blend](../7-Glossary/Contents.md#blend), we can see circles appear around each [Target](../7-Glossary/Contents.md#target). These indicate that the [Robot](../7-Glossary/Contents.md#manipulator) will exactly follow our [Toolpath](../7-Glossary/Contents.md#toolpath) until it gets within 100 mm of the [Target](../7-Glossary/Contents.md#target), at which point it will start to deviate within that circle to keep its speed up and head towards the subsequent [Target](../7-Glossary/Contents.md#target). It will exactly follow our [Toolpath](../7-Glossary/Contents.md#toolpath) again when it leaves the circle. When we solve our [Procedure](../7-Glossary/Contents.md#procedure), we can see the path our [TCP](../7-Glossary/Contents.md#endpoint) will actually take getting close but not actually to all of our [Targets](../7-Glossary/Contents.md#target).

_Kinematic_ settings are a more advanced topic and will be discussed in future tutorials ([1](../3-Motion/Contents.md#3.6.-coupled-motion-and-resolving-targets),[2](../3-Motion/Contents.md#3.8.-add-target-constraints),[3](../3-Motion/Contents.md#3.9.-add-mechanism-constraints)).

---
### 3.4. Combine Procedures and the Procedure Browser

#### Objective:

In this tutorial we'll see how to combine different [Procedures](../7-Glossary/Contents.md#procedure) to chain sequences using the HAL Robotics Framework for Grasshopper.

#### Demo Files:

[3.4 - Combine Procedures and the Procedure Browser.gh](../ExampleFiles/Tutorials/3.4%20-%20Combine%20Procedures%20and%20the%20Procedure%20Browser.gh)

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

[Procedures](../7-Glossary/Contents.md#procedure) are sequences of atomic [Actions](../7-Glossary/Contents.md#action), such as [Move](../7-Glossary/Contents.md#motion-action), [Wait](../7-Glossary/Contents.md#wait-action) or [Change Signal State](../7-Glossary/Contents.md#signal-action). Each of these are created individually but need to be combined to be executed one after the other by our [Robot](../7-Glossary/Contents.md#manipulator). [Procedures](../7-Glossary/Contents.md#procedure) can also be created as reusable sequences of [Actions](../7-Glossary/Contents.md#action) for example moving to a home position and opening a gripper.

#### How to:

To combine multiple [Procedures](../7-Glossary/Contents.md#procedure), we can use the **Combine Actions** component from the **HAL Robotics** tab, **Procedure** panel. This component allows us to name our new [Procedure](../7-Glossary/Contents.md#procedure) with the _Alias_ input. This is extremely useful for identifying your [Procedure](../7-Glossary/Contents.md#procedure) later, particularly when using it more than once. The only mandatory input for this component is the list of [Procedures](../7-Glossary/Contents.md#procedure) and [Actions](../7-Glossary/Contents.md#action) to be _Combined_. In Grasshopper we can pass any number of wires into the same input and the **Combine Actions** component will create a [Procedure](../7-Glossary/Contents.md#procedure) for each branch of items it gets. However, to ensure that we keep a clean document and an easy means of changing the order of [Procedures](../7-Glossary/Contents.md#procedure) it is recommended to use something like a **Merge** component and flattening all the inputs. Once those are _Combined_, we will have single [Procedure](../7-Glossary/Contents.md#procedure) that executes each of our sub-[Procedures](../7-Glossary/Contents.md#procedure) one after the other.

Once a [Procedure](../7-Glossary/Contents.md#procedure) has been assigned to a [Controller](../7-Glossary/Contents.md#controller) and [Solved](../7-Glossary/Contents.md#solving) it is useful to see how a [Simulation](../7-Glossary/Contents.md#7.3.-simulation) is progressing through that [Procedure](../7-Glossary/Contents.md#procedure) so we can see where any issues may lie or which phases might be taking longer than we expect. We can do that using the **Procedure Browser**. To access the **Procedure Browser**, we need to ensure that we have an **Execution Control** connected to a complete **Execute** component. Once that's in place we can double-click on the **Execution Control** to open the **Procedure Browser**. In this window we can see our execution controls, reset, play/pause, next, previous and loop as well as all of our actions. Alongside that we have a time slider that allows you to speed up or slow down the [Simulation](../7-Glossary/Contents.md#7.3.-simulation) of your [Procedures](../7-Glossary/Contents.md#procedure) without affecting your program itself. The rest of the **Procedure Browser** window shows the [Procedure](../7-Glossary/Contents.md#procedure) that you are executing and the progress of each [Action](../7-Glossary/Contents.md#action) within it. This **Procedure Browser** view also serves to demonstrate the purpose of the _Compact_ input on our **Combine Procedure** component. By default, _Compact_ is set to `true`. This compacts all of the incoming [Procedures](../7-Glossary/Contents.md#procedure) and creates a single, flat list of [Actions](../7-Glossary/Contents.md#action). If, however, we toggle _Compact_ to `false` we see that all of our previous [Procedures](../7-Glossary/Contents.md#procedure) are maintained in the hierarchy and can be collapsed or expanded to view their contents. The hierarchical, un-compacted mode can be particularly useful if you reuse sub-[Procedures](../7-Glossary/Contents.md#procedure).

---
### 3.5. Synchronize Motion

#### Objective:

In this tutorial we'll see how to synchronize the motion of multiple [Mechanisms](../7-Glossary/Contents.md#mechanism) using the HAL Robotics Framework for Grasshopper.

#### Demo Files:

[3.5 - Synchronize Motion.gh](../ExampleFiles/Tutorials/3.5%20-%20Synchronize%20Motion.gh)

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

When we have multiple [Robots](../7-Glossary/Contents.md#manipulator) or [Mechanisms](../7-Glossary/Contents.md#mechanism), such as [Positioners](../7-Glossary/Contents.md#positioner), in a [Cell](../7-Glossary/Contents.md#cell) it may be necessary for them to execute [Motion](../7-Glossary/Contents.md#motion-action) synchronously. This could be in scenarios such as two [Robots](../7-Glossary/Contents.md#manipulator) sharing a load between them or a [Positioner](../7-Glossary/Contents.md#positioner) reorientating a [Part](../7-Glossary/Contents.md#part) whilst a [Robot](../7-Glossary/Contents.md#manipulator) works on it.

#### How to:

In order to **Synchronize** [Motions](../7-Glossary/Contents.md#motion-action), we need to ensure we have multiple [Procedures](../7-Glossary/Contents.md#procedure) to work with and we always use one [Procedure](../7-Glossary/Contents.md#procedure) per [Mechanism](../7-Glossary/Contents.md#mechanism) we want to program, whether it's a [Robot](../7-Glossary/Contents.md#manipulator), **Track** or [Positioner](../7-Glossary/Contents.md#positioner). A setup for this could be as simple as having two [Robots](../7-Glossary/Contents.md#manipulator) each moving to a single [Target](../7-Glossary/Contents.md#target) in [Joint space](../7-Glossary/Contents.md#joint-space). To make this a little more demonstrative it would be preferable if the [Motions](../7-Glossary/Contents.md#motion-action) are dissimilar, for example one being long and the short or one fast and the other slow. To **Synchronize** the [Motions](../7-Glossary/Contents.md#motion-action), we need to assign them **Sync Settings**. The **Sync Settings** component can be found in the **HAL Robotics** tab, **Motion** panel. We should assign a unique name, using the _Alias_ input, to the **Sync Settings** to ensure that they are easily identifiable later. Once those **Sync Settings** have been created, they need to be assigned to both of our [Moves](../7-Glossary/Contents.md#motion-action). It is important to note that it must be the exact same **Sync Settings** passed to both. Your **Sync Settings** must only be used for one synchronous sequence of [Motions](../7-Glossary/Contents.md#motion-action) per [Procedure](../7-Glossary/Contents.md#procedure), and synchronous sequences must contain the same number of [Actions](../7-Glossary/Contents.md#action) in each [Procedure](../7-Glossary/Contents.md#procedure) in which they're used. We can now **Solve** and see that the duration of our [Moves](../7-Glossary/Contents.md#motion-action) has been adjusted so that they both take the same amount of time. Also critically important in **Synchronization**, is the fact that all the motions start at the same time. We can test this out be adding a [Move](../7-Glossary/Contents.md#motion-action) to one of the [Procedures](../7-Glossary/Contents.md#procedure) prior to the **Synchronous** [Moves](../7-Glossary/Contents.md#motion-action). When this is re-**Solved,** we can see that the second [Robot](../7-Glossary/Contents.md#manipulator) implicitly waits for the first [Robot's](../7-Glossary/Contents.md#manipulator) [Move](../7-Glossary/Contents.md#motion-action) to finish before they both start their **Synchronous** [Moves](../7-Glossary/Contents.md#motion-action).

---
### 3.6. Coupled Motion and Resolving Targets

#### Objective:

In this tutorial we'll see how to simplify the programming of multi-[Mechanism](../7-Glossary/Contents.md#mechanism) setups using **Target Resolvers** in the HAL Robotics Framework for Grasshopper.

#### Demo Files:

[3.6 - Resolve Targets.gh](../ExampleFiles/Tutorials/3.6%20-%20Resolve%20Targets.gh)

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

When we have multiple [Robots](../7-Glossary/Contents.md#manipulator) or [Mechanisms](../7-Glossary/Contents.md#mechanism), such as [Positioners](../7-Glossary/Contents.md#positioner), in a [Cell](../7-Glossary/Contents.md#cell) programming can become increasingly complex and, in many scenarios, we only really have one set of [Targets](../7-Glossary/Contents.md#target) that we care about which a [Positioner](../7-Glossary/Contents.md#positioner) should be relocating to facilitate access by a [Manipulator](../7-Glossary/Contents.md#manipulator). We refer to this configuration, where one [Mechanism](../7-Glossary/Contents.md#mechanism) displaces the [Targets](../7-Glossary/Contents.md#target) of another as **Coupled** motion.

#### How to:

As an example of this we can have a [Robot](../7-Glossary/Contents.md#manipulator) drawing a straight line between two [Targets](../7-Glossary/Contents.md#target) that are referenced on a rotary [Positioner](../7-Glossary/Contents.md#positioner). In order to setup **Coupled** motion we need to add some settings to all the **Motion Settings** that will be used in that **Coupled** configuration. We can create **Kinematic Settings** from the **HAL Robotics** tab, **Motion** panel and because we're dealing with multiple [Mechanisms](../7-Glossary/Contents.md#mechanism) we are going to change to the **Composite Kinematic Settings** template. We are now asked to input settings for the _Primary_ and _Secondary_ [Mechanisms](../7-Glossary/Contents.md#mechanism). The former is typically the [Mechanism](../7-Glossary/Contents.md#mechanism) that is moving the [Targets](../7-Glossary/Contents.md#target) around, historically termed "Master", and the latter is the [Mechanism](../7-Glossary/Contents.md#mechanism) moving to those [Targets](../7-Glossary/Contents.md#target), historically termed "Slave". In our case the [Targets](../7-Glossary/Contents.md#target) are referenced to the [Positioner](../7-Glossary/Contents.md#positioner) and the [Robot](../7-Glossary/Contents.md#manipulator) is following those [Targets](../7-Glossary/Contents.md#target) around. We don't need to assign any additional **Kinematic Settings** to our [Mechanisms](../7-Glossary/Contents.md#mechanism) so we can simply chain our [Mechanisms](../7-Glossary/Contents.md#mechanism) into simple Kinematic Settings and into their positions in the **Composite Kinematic Settings**. These **Composite Kinematic Settings** can now be added to our **Motion Settings** for the **Coupled** [Motions](../7-Glossary/Contents.md#motion-action). We have two separate sets of **Motion Settings** here; one is for the **Coupled** [Motion](../7-Glossary/Contents.md#motion-action) and the other is for an asynchronous initialization [Motion](../7-Glossary/Contents.md#motion-action) for each of the [Mechanisms](../7-Glossary/Contents.md#mechanism).

With our settings in place we can now look at programming the [Positioner](../7-Glossary/Contents.md#positioner). We could calculate the [Targets](../7-Glossary/Contents.md#target) for the [Positioner](../7-Glossary/Contents.md#positioner) and set them explicitly but when we're in a scenario like this welding example we can set some rules for the [Positioner](../7-Glossary/Contents.md#positioner) to follow. We do this using the **Target Resolvers** from the **HAL Robotics** tab, **Motion** panel. There are a few different templates to explore but, in our case, the first is the one we want. The **Vector Aligned** **Target Resolver** tells the positioner to point the given _Axis_ of our [Targets](../7-Glossary/Contents.md#target) towards a particular direction. If we can it's normally preferable to weld with Gravity so we're going to ask the [Positioner](../7-Glossary/Contents.md#positioner) to point the Z axis of our [Targets](../7-Glossary/Contents.md#target) straight down. The **Target Resolver** can be used in a **Move** just like a [Target](../7-Glossary/Contents.md#target) provided it is duplicated to match the number of "secondary" [Targets](../7-Glossary/Contents.md#target). To make that task easier we have included a template in **Move** called **Synchronized** which takes in a [Procedure](../7-Glossary/Contents.md#procedure) and a [Target](../7-Glossary/Contents.md#target), or **Target Resolver**, and will create all of the necessary [Moves](../7-Glossary/Contents.md#motion-action) for you with the correct synchronization settings to match the input _Procedure_. **Synchronized** [Move](../7-Glossary/Contents.md#motion-action) creates a [Procedure](../7-Glossary/Contents.md#procedure) as an output like any other **Move** and so it can be merged and **Combined** as we would normally with any other **Move**. With both of our [Procedures](../7-Glossary/Contents.md#procedure) now complete we can **Solve** and **Simulate** to see our [Positioner](../7-Glossary/Contents.md#positioner) aligning itself automatically to best present the [Targets](../7-Glossary/Contents.md#target) to the [Robot](../7-Glossary/Contents.md#manipulator).

---
### 3.7. Change a Tool at Runtime

#### Objective:

In this tutorial we'll see how to change the active [Tool](../7-Glossary/Contents.md#end-effector) of a [Mechanism](../7-Glossary/Contents.md#mechanism) during the execution of a [Procedure](../7-Glossary/Contents.md#procedure) in the HAL Robotics Framework for Grasshopper.

#### Demo Files:

[3.7 - Change a Tool at Runtime.gh](../ExampleFiles/Tutorials/3.7%20-%20Change%20a%20Tool%20at%20Runtime.gh)

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

There are scenarios in which a single [Robot](../7-Glossary/Contents.md#manipulator) may have access to multiple [Tools](../7-Glossary/Contents.md#end-effector) and the ability to change which [Tool](../7-Glossary/Contents.md#end-effector) is in use at runtime. This could be because, either, the [Tool](../7-Glossary/Contents.md#end-effector) itself has multiple [Endpoints](../7-Glossary/Contents.md#endpoint) or because automatic [Tool](../7-Glossary/Contents.md#end-effector) changing equipment is available in the [Cell](../7-Glossary/Contents.md#cell).

#### How to:

In preparation for this tutorial a number of things have been put in place. Firstly, three [Tools](../7-Glossary/Contents.md#end-effector) have been created; a simple cone, an interchange plate and a double cone with 2 distinct [Endpoints](../7-Glossary/Contents.md#endpoint). Secondly, these [Tools](../7-Glossary/Contents.md#end-effector) have each been positioned in front of the [Robot](../7-Glossary/Contents.md#manipulator) in a known location. And finally, a [Toolpath](../7-Glossary/Contents.md#toolpath) has been created to go to each of the [Tool](../7-Glossary/Contents.md#end-effector) picking positions with a jump in between. I have also prepared the standard **Combine**, **Solve**, and **Execute** flow.

The focus of this tutorial will be on the **Change Tool** component which can be found in the **HAL Robotics** tab under **Procedure**. Each version of this component will give us a [Procedure](../7-Glossary/Contents.md#procedure) that we can Combine with [Moves](../7-Glossary/Contents.md#motion-action) and [Waits](../7-Glossary/Contents.md#wait-action) as we have done countless times before. The **Change Tool** component has 3 templates and we'll cover them all, starting with **Detach Tool**. We're going to use this to remove the `Cone` [Tool](../7-Glossary/Contents.md#end-effector) that we've initially got attached to the [Robot](../7-Glossary/Contents.md#manipulator). We want to ensure the _Mechanism_ is the combined [Mechanism](../7-Glossary/Contents.md#mechanism) and we can specify the _Tool_ as the `Cone` [Tool](../7-Glossary/Contents.md#end-effector). The _Tool_ input is actually optional and the currently active [Tool](../7-Glossary/Contents.md#end-effector) will be removed if none is specified. We can weave this into our main [Procedure](../7-Glossary/Contents.md#procedure) and if you Solve and Execute, you'll see the Cone disappear when we hit the right [Target](../7-Glossary/Contents.md#target). The `Cone` is actually in the exact position we left it but it is no longer displayed because it's not part of our [Robot](../7-Glossary/Contents.md#manipulator). We can use the _Environment_ input on **Execute** to force the display of mobile, non-mechanism [Parts](../7-Glossary/Contents.md#part). If we now **Execute**, we should see the `Cone` hang in space where we detached it.

From here we're going to attach two [Tools](../7-Glossary/Contents.md#end-effector) to the [Robot](../7-Glossary/Contents.md#manipulator) in succession. The first is going to be the `Interface` which acts as something of a Tool Changer. Using the **Change Tool** component and the **Attach Tool** template we can set the combined [Mechanism](../7-Glossary/Contents.md#mechanism) as the _Mechanism_ again and the `Interface` as the _Tool_. Merging this into our [Procedure](../7-Glossary/Contents.md#procedure) will attach the `Interface` to our [Robot](../7-Glossary/Contents.md#manipulator) and we can visualize the [Parts](../7-Glossary/Contents.md#part) before they are attached using the same technique as for the `Cone`, passing the [Parts](../7-Glossary/Contents.md#part) into _Environment_ on **Execute**. If we repeat this process and attach the `MultiTool` this time, you should see that the `MultiTool` gets connected to the [Active Endpoint](../7-Glossary/Contents.md#endpoint) of the [Robot](../7-Glossary/Contents.md#manipulator), which in this case is the end of the `Interface`. This behavior may not always be desirable e.g. stationary tools, and can be modified in the overload of **Attach Tool**.

In this final combination of [Tools](../7-Glossary/Contents.md#end-effector) attached to the [Robot](../7-Glossary/Contents.md#manipulator) we have two distinct potential [Endpoints](../7-Glossary/Contents.md#endpoint). The final template of the **Change Tool** component allows us to set which [Endpoint](../7-Glossary/Contents.md#endpoint), or [Tool](../7-Glossary/Contents.md#end-effector) if you have multiple distinct [Tools](../7-Glossary/Contents.md#end-effector) attached, is currently Active. We do this by specifying, once again, the combined [Mechanism](../7-Glossary/Contents.md#mechanism), and the [Connection](../7-Glossary/Contents.md#connection) that we want to use as the [Active Endpoint](../7-Glossary/Contents.md#endpoint). To ensure consistent and deterministic output, I would recommend doing this immediately after attaching the `MultiTool` as well as when you may wish to switch between the two [Endpoints](../7-Glossary/Contents.md#endpoint). With that merged and our [Tool](../7-Glossary/Contents.md#end-effector) [Parts](../7-Glossary/Contents.md#part) in the _Environment_ we can see everything run.

---
### 3.8. Add Target Constraints
#### Coming Soon

---
### 3.9. Add Mechanism Constraints
#### Coming Soon

---