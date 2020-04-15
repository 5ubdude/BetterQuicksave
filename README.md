# Better Quicksave
### Description

Normally, when you quicksave by pressing 'F5' or by using the 'Save' button in the pause menu the game overwrites your most recent manual save. This mod changes this behavior so the game will now save to a configurable number of dedicated quicksave files. The files will rotate with the oldest being overwritten when you reach the maximum number of quicksaves. By default that number is 3, but it can be easily changed in the `BetterQuicksaveConfig.xml` file located in the `ModuleData` folder. You can also quickload the latest quicksave with a bindable key (by default 'F10'). You are able to quickload from within combat/dialogue as well.

### Installation

Place the  `BetterQuicksave` folder into your `Modules` folder that's in your game installation directory. Alternatively use Vortex. After that, simply activate the mod in the game launcher.

### Uninstallation

You can remove/disable the mod without issue. Your quicksave files will remain and be treated as normal manual saves by the game. If you reinstall/re-enable the mod it will recognize any previous saves made with the mod (or any save named like `quicksave_XXX`).

### Configuration

The configuration file is located here:

`[Game Install Location]/Modules/BetterQuicksave/ModuleData/BetterQuicksaveConfig.xml`

##### `<MaxQuicksaves>`3`</MaxQuicksaves>`
Change to whatever number of quicksave slots you'd like. Note that save files are ~50MB each and get bigger as the game progresses.

##### `<QuicksavePrefix>`quicksave_`</QuicksavePrefix>`
Choose the name prefix used for quicksaves (e.g. `quicksave_` will result in names like `quicksave_001`). Some special characters aren't allowed and will be stripped out. If `MaxQuicksaves` is set to 1, this will just be used as the filename.

##### `<QuickloadKey>`68`</QuickloadKey>`
Choose which key you want bound to quickload. By default this is F10 (represented by 68). You can find a full list of key codes [here](https://gist.github.com/5ubdude/ef5b111d77231274572564888eaf6f25) (ignore the weird numbers to the right). Be careful with this as it will use that key regardless of what it is bound to in game. The quicksave key can be bound normally in the game options.

##### `<PerCharacterSaves>`true`</PerCharacterSaves>`
Choose whether each of your characters has their own quicksave slot(s). The character's name will be added to the save name (ex. `Name ClanName quicksave_001`).

### Troubleshooting

#### Game crashes on startup
- Go to `[Game Install Location]/Modules/BetterQuicksave/bin/Win64_Shipping_Client/` and right click each DLL file, click 'Properties', and then press the 'Unblock' button.

- If you are still having issues and are using multiple mods, make sure all of your installed mods' `SubModule.xml` files have the following lines:
```xml
<Official value="false"/>
<DependedModules>
  <DependedModule Id="Native"/>
  <DependedModule Id="SandBoxCore"/>
  <DependedModule Id="Sandbox"/>
  <DependedModule Id="CustomBattle"/>
  <DependedModule Id="StoryMode" />
</DependedModules>
```

#### Can't load saves
- This mod was designed with save compatibility in mind and so only touches the filename of quicksaves. It won't impact your ability to load saves. If you're having issues loading saves, it's very likely a result of another mod or a game patch. From my own experience, and from what other users have said about this issue, there are certain mods that make your saves depend on that mod. Check the mod pages of other mods you have installed to see if others are having an issue with saves.

### Compatibility

This mod uses [Harmony](https://harmony.pardeike.net/) to patch the game files and so should be compatible with pretty much everything, and will most likely be compatible with future game updates unless TaleWorlds makes changes to quicksaves.

### Building

To build, make sure your configuration is set to **Release** in Visual Studio and add references to the following DLLs from the `bin\Win64_Shipping_Client\` and `Modules\SandBox\bin\Win64_Shipping_Client` folders in your game's installation directory.
```
SandBox.dll
TaleWorlds.CampaignSystem.dll
TaleWorlds.Core.dll
TaleWorlds.DotNet.dll
TaleWorlds.Engine.dll
TaleWorlds.InputSystem.dll
TaleWorlds.Library.dll
TaleWorlds.Localization.dll
TaleWorlds.MountAndBlade.dll
TaleWorlds.SaveSystem.dll
```
The mod will be built to the `build` folder located in the project's root directory.
