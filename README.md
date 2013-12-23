FFXIVLIB
========
FFXIVLIB is an open-source C# library allowing you to create easily 3rd party tools for FINAL FANTASY XIV MMORPG
Features
========
- Player support
- Party Members support
- Entity support (PC/NPC/Gathering)
- Target support (Current/Mouseover/Previous/Focus)
- Inventory support
- Chatlog support
- Signature scanner
- Resources parser (Mapping ID<>Name)

Getting Started
========
Add a reference to ffxivlib.dll in your application and include this snippet

```c#
using ffxivlib;

FFXIVLIB instance = new FFXIVLIB();
```

Check out the demo projects and the [documentation](http://sruon.github.io/ffxivlib/html/functions.html)

## Examples

Retrieve the current target and print its name and level
```c#
using ffxivlib;

FFXIVLIB instance = new FFXIVLIB();
Entity myTarget = instance.GetCurrentTarget();
Console.WriteLine("Name: {0} Level: {1}", e.Name, e.Level);
```

## Requests/Questions

Please feel free to create an issue and submit pull requests to discuss changes, bugs or new additions.

## Links
- [Documentation](http://sruon.github.io/ffxivlib/html/functions.html)
- [Support thread](http://www.ffevo.net/topic/3264-ffxivlib/)
- [Download](http://www.ffevo.net/files/file/242-ffxivlib/)
