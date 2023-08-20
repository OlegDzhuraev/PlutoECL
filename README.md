# PlutoECL
**Update:** Repository is archived. I decided that this architectural pattern is not very effective in a real development.

Pluto ECL is **experimental** implementation of Entity Component Logic architectural pattern in Unity. It is based on Ceres ECL ideas, but works fully on MonoBehaviours. Not so optimized as Ceres ECL, but much more comfortable to work with.

<p align="center">
    <img src="http://dzhuraev.com/CeresECL/PlutoECLScr1.png" width="400" height="430" alt="Pluto ECL">
</p>

### What is Entity Component Logic?
It is inspired by Entity Component System and Components over Inheritance approaches. 
Difference from ECS is that there no Systems, which handles a lot of objects, but there is Logics, which handle self objects like MonoBehaviours in Unity. 
But with ECL you obliged to separate logics from data.
I really don't know is there exist any pattern like this, so I described it by my own. :)

Problem of Unity-way scripting is that there only MonoBehaviour, which is not saves you from bad coding practices. Pluto ECL will keep you in track of following the pattern principles.

### Why I should use it instead of Entity Component System?
No reasons. ECS is cool. But if you're have problems with understanding ECS or it looks too complicated to you,
Pluto ECL is simplier and looks more like usual Unity-way scripting, so you can use it.

## Overview
### Entity
**Entity** is basic **MonoBehaviour** script (in this framework it is ExtendedBehaviour, but it is not necessary now). It marks your object as something dynamic and includes it in gameplay run cycle. Also **Entity** is container for **Tags, Events** and some **useful methods**, which is described below.

```csharp
var entity = Entity.Spawn(playerPrefab);

// Spawn empty game object as entity (no prefab needed).
var emptyEntity = Entity.Spawn();
```

### Component (Part)
Any derived from **Part** class is **Component**, if it contains **only data** of your **Entity**. For example, **MoveComponent**, which contains **Speed** and **Direction** of movement. 
There should be no game logics in this class. But you still can place there utility methods, which calculate something, but doesn't affect gameplay etc.

```csharp
using PlutoECL;

public class MoveComponent : Part
{
  public float Speed;
  public Vector3 Direction;
  
  // utility method example
  public Vector3 GetSpeedScaledDirection() => Direction * Speed;
}
```

### Logic
**Logic** is any script with game logic. Logic describes specific behaviour of the **Entity**. Logics should know nothing about anothers, it should work only with **Components**, **Events** and **Tags**.

For example, **MoveLogic** will move it using **MoveComponent** data. 
And **InputLogic** will fill **MoveComponent Direction** field with player input.

In actual framework version, to create logic, you should derive it from **ExtendedBehaviour**. Not the best solution, but for production it is best for me now.

```csharp
using PlutoECL;

public class MoveLogic : ExtendedBehaviour
{
  MoveComponent moveComponent;

  protected override void Init()
  {
    moveComponent = Get<MoveComponent>();

    moveComponent.Speed = 2f;
  }

  protected override void Run()
  {
    transform.position += moveComponent.Direction * (moveComponent.Speed * Time.deltaTime);
  }
}
```

There is **Init** method to implement initialization logic, similar to the **Awake** Unity method.

There also **Run** method to implement run logic, similar to the **Update** Unity method.

You also can use **PostInit**, it will be similar to the **Start** Unity method.

### Tags
If you need to create new component, which will be simple mark object state, use **Tags** instead. **Entity.Tags** contains all tags, added to the entity. 

**Tags** can be any **Enum**. You can use **TagsList.cs** from Example or create your own **enum Tag**.
To create new **Tag**, edit **enum Tag**:
```csharp
public enum Tag
{
    CustomTag = 0,
    // add your tags here
}
```

Adding tag to the entity:
```csharp
Entity.Tags.Add(Tag.CustomTag);
```

Note that tags are stacked, it means that you can add several same tags to the entity. It can used for some mechanics like block of some entity action from different Logics.

Check of tag on the entity:
```csharp
Entity.Tags.Have(Tag.CustomTag);
```

Removing tag (only one, if there stacked tags) from the entity:
```csharp
Entity.Tags.Remove(Tag.CustomTag);
```

New tags version is a simple integer in code, so if you want see your Tags names from enum in Entity debug, you need specify your enum type in **PlutoSettings** in ECL Launcher script:
```csharp
PlutoSettings.Instance.TagsEnum = typeof(Tag);
```
You can see it in **Example**.

Tags inspired by Pixeye Actors ECS tags. But possble that in my realisation this feature is not so cool. :D

### Events
**Events** - same to the components, but live only 1 frame. So if something happens in your game, you can send event from one of Logics and other will catch it. Since event same to the component, it can contain any data. To create **Event**, create new class, derived from **Event**:

```csharp
using PlutoECL;

class ColliderHitEvent : Event
{
    public Collider2D HitCollider;
}
```

To send **Event**, do it like this:
```csharp
 var hitEvent = new ColliderHitEvent
{
    HitCollider = other
};

Entity.Events.Add(hitEvent);
```

You can send events not from Logics and other MonoBehaviours too.

Unsubscribe is the same:
```csharp
Entity.Events.Unsubscribe<ColliderHitEvent>(OnHit);
```

Current Events version is not finished and can be unstable, but all tests passed fine, so I added it to the repo.

### Launcher
To make it all work, you need entry point, some classic **MonoBehaviour** script. To do this correct, create your new script, name it, for example, **MyLauncher**, and derive from Pluto **Launcher** class. Next, you need to override **StartAction** method and add there your init logic.

```csharp
using PlutoECL;

public class MyLauncher : Launcher
{
    protected override void StartAction()
    {
        Entity.Spawn();
    }
}
```

Base **Launcher** class handles all entities update, so there only 1 MonoBehaviour Update for **all** Entities Logics. For some unknown reasons, in Unity it increases game FPS. So do **not** make **Update** method in your Launcher, it can override Pluto one. And, yes, you don't need it in any case.

### Templates
You can create each of these classes using templates. Click **RMB** in **Project Window** and open submenu **Pluto ECL**. There will be all actual templates. 

It will generate script in root namespace, which you can change in **Editor Settings -> Editor -> C# Project Generation**. Otherwise it will be generated in **PlutoECL** namespace.

For template generator idea thanks to LeoECS, some used things came from its code.

## Debug
To check state of your **Entity**, select its **GameObject** on scene and you will see all Tags, Components and Logics, added to the entity with their parameters. In your Components you can see and edit all parameters.

You also can use **RuntimeOnly** and **ReadOnly** attributes on your fields, it help to keep these fields from unwanted editing.

## More FAQ
### Can I place public variables in Logics
No. It breaks whole pattern and returns you to basic Unity-way coding. You should remember, that this approach first of all should be in your head. There no framework needed to use it, Pluto framework just adds some useful functions to help with this pattern implementation.

### How I can use OnTriggerEnter, for example
Like in classic **MonoBehaviour**, just do it from **Logic** code. Also you can use interfaces like **IPointerClickHandler** with your **Logics** too.

### Is Pluto ECL production-ready
No, until there will be at least one release on GitHub. Currently it is fully experimental non-commercial project. But you can use it on your risk, all features already should work.

## Examples
Example will be added as separated repo soon.

Links to games examples on GitHub will be added when these examples will be created. :)

## Thanks to
Leopotam for LeoECS https://github.com/Leopotam/ecs

Pixeye for Actors https://github.com/PixeyeHQ/actors.unity

Inspired me to use ECS and think more about different coding architecture patterns. :)
