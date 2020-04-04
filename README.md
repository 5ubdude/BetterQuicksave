# Better Quicksave
## Description
Normally, when you quicksave by pressing 'F5' or by using the 'Save' button in the pause menu the game overwrites your most recent manual save. This mod changes this behavior so the game will now save to a configurable number of dedicated quicksave files. The files will rotate with the oldest being overwritten when you reach the maximum number of quicksaves. By default that number is 3, but it can be easily changed in the `BetterQuicksaveConfig.xml` file located in the `ModuleData` folder.

## Installation
Place the  `BetterQuicksave` folder into your `Modules` folder that's in your game installation directory. Alternatively use Vortex. After that, simply activate the mod in the game launcher.

## Uninstallation
You can remove/disable the mod without issue. Your quicksave files will remain and be treated as normal manual saves by the game. If you reinstall/re-enable the mod it will recognize any previous saves made with the mod (or any save named like `quicksave_XXX`).

## Configuration
The configuration file is located here:

`[Game Install Location]/Modules/BetterQuicksave/ModuleData/BetterQuicksaveConfig.xml`

Change `<MaxQuicksaves>3</MaxQuicksaves>` to whatever number you'd like. Note that save files are ~50MB each and get bigger as the game progresses.

## Troubleshooting
### Game crashes on startup
- Go to `[Game Install Location]/Modules/BetterQuicksave/bin/Win64_Shipping_Client/` and right click each DLL file, click 'Properties', and then press the 'Unblock' button.

- If you are still having issues and are using multiple mods, make sure all of your installed mods `SubModule.xml` files have the following lines:
```
  <Official value="false"/>
  <DependedModules>
      <DependedModule Id="Native"/>
      <DependedModule Id="SandBoxCore"/>
      <DependedModule Id="Sandbox"/>
      <DependedModule Id="CustomBattle"/>
      <DependedModule Id="StoryMode" />
  </DependedModules>
```

## Compatibility
This mod uses [Harmony](https://harmony.pardeike.net/) to patch the game files and so should be compatible with pretty much everything, and will most likely be compatible with future game updates unless TaleWorlds makes changes to quicksaves.
