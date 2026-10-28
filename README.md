# Gene Bridge

Two focused compatibility repairs: unique Big and Small transformation-gene names and canonical inherited AG_Female references. Content mods remain separate subscriptions.

Requires Harmony and Biotech. Previous host/startup checks are documented in the retained package guide; gameplay metamorphosis and exhaustive save migration are not certified.

- Package ID: `sweetbriana.genebridge`
- Code license: MIT. See LICENSE.
- Steam Workshop: https://steamcommunity.com/sharedfiles/filedetails/?id=3828530773

## Source release

This is the selected production source and metadata snapshot, with game, Harmony and provider assemblies omitted. Original workspaces, installed mods, active lists and player saves are unchanged. This GitHub publication does not update Steam Workshop. See BUILD-STATUS.md for fresh compilation evidence and its limits.

## Build for 1.7

Use PowerShell 7 and an installed .NET SDK. Supply your licensed game and matching Harmony assembly:

```powershell
./Build17.ps1 -GameDir 'PATH/TO/RIMWORLD' -HarmonyDll 'PATH/TO/0Harmony.dll'
```

The portable helper compiles only Source/ into .build/1.7, without downloading dependencies, starting a game, copying a runtime or changing installed mods. Archived IDE project files may retain legacy references; the 1.7 helper is the supported build path for this snapshot. Legacy source, where included, is separate and has not been newly tested. Assets have separate terms in ASSETS.md.
