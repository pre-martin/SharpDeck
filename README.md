# Fork

This is a fork of SharpDeck, since SharpDeck has not been maintained since December 2022. I am continuing to develop this fork to meet the needs of my [Stream Deck plugin](https://github.com/pre-martin/StreamDeckSimHubPlugin).

Below is the original README:

# SharpDeck

A comprehensive .NET wrapper for creating Stream Deck plugins, using the official Stream Deck SDK documentation. SharpDeck takes the hassle out of communicating with the Stream Deck SDK, and handles action caching and event delegation.

## 📦 Examples

* [Counter](/examples/Counter) - Single action counter plugin.
* [Shared Counter](/examples/SharedCounter) - Multiple action plugin with a shared counter and reset.

## ✏️ Creating a plugin

SharpDeck enables console applications to easily communicate with Stream Deck; run `dotnet new console` to create a .NET console application, and follow these five steps to create your first Stream Deck plugin.

### 🔗 1. Program.cs

```csharp
public static void Main(string[] args)
{
#if DEBUG
    System.Diagnostics.Debugger.Launch();
#endif

    // Connect to Stream Deck.
    SharpDeck.StreamDeckPlugin.Run();
}
```

### ⚡ 2. Create an action
```csharp
using SharpDeck;
using SharpDeck.Events.Received;

[StreamDeckAction("com.geekyeggo.exampleplugin.firstaction")]
public class MyFirstAction : StreamDeckAction
{
    // Methods can be overriden to intercept events received from the Stream Deck.
    protected override Task OnKeyDown(ActionEventArgs<KeyPayload> args) => ...;
    protected override Task OnKeyUp(ActionEventArgs<KeyPayload> args) => ...;
}
```

### 📄 3. Create a manifest.json file

> For more information about creating a manifest file, please refer to the [SDK documentation](https://developer.elgato.com/documentation/stream-deck/sdk/manifest/).

### ⚙️ 4. Configure .csproj

```xml
<!-- Install the plugin after each build - change authorName and pluginName accordingly. -->
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
    <OutputPath>$(APPDATA)\Elgato\StreamDeck\Plugins\com.{{authorName}}.{{pluginName}}.sdPlugin\</OutputPath>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
</PropertyGroup>

<ItemGroup>
    <!-- The manifest file is required by Stream Deck and provides important information. -->
    <None Update="manifest.json">
        <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
</ItemGroup>

<!-- Kill the Stream Deck process before each build; this allows the copy to occur. -->
<Target Name="PreBuild" BeforeTargets="PreBuildEvent" Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
    <Exec Command="taskkill -f -t -im StreamDeck.exe -fi &quot;status eq running&quot;" />
</Target>
```

### 🚧 5. Debug profile

> Located at **Properties/launchSettings.json**; this debug profile will automatically launch StreamDeck.exe when debugging starts.
```json
{
  "profiles": {
    "DebugWin": {
      "commandName": "Executable",
      "executablePath": "c:\\windows\\system32\\cmd.exe",
      "commandLineArgs": "/S /C \"start \"title\" /B \"%ProgramW6432%\\Elgato\\StreamDeck\\StreamDeck exe\" \""
    }
  }
}
```

## 🏃 Running

When debugging starts (default F5):
1. The plugin will be built and installed.
1. Stream Deck will then launch.
1. You will be prompted to attach your plugin to a process; this allows for debugging.

When your plugin is ready, consider [packaging](https://developer.elgato.com/documentation/stream-deck/sdk/packaging/) and [distributing](https://developer.elgato.com/documentation/stream-deck/sdk/distribution/) it on the Stream Deck store.

## 📃 License

[The MIT License (MIT)](LICENSE.md)
Stream Deck is a trademark or registered trademark of Elgato Systems.
