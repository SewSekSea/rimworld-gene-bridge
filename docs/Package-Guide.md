# Gene Bridge 1.0.0

RimWorld 1.7 compatibility mod by SweetBriana. Requires Harmony and Biotech. Big and Small, Alpha Genes and the affected xenotype mods remain separate optional subscriptions; their content is not bundled here.

Gene Bridge combines two repairs:

1. Big and Small's generated transformation genes receive unique names when their proposed name already belongs to another gene. The original `Imago_Metamorphosis` remains Imago's gene; the generated B&S gene uses the same `SweetBriana_BS_Imago_Metamorphosis` identifier as the earlier local compatibility patch.
2. After XML inheritance, repeated `AG_Female` gene entries collapse to one reference to Alpha Genes' registered definition. Existing female-gene description links point to that definition, once per description. Other genes, links and their ordering are preserved. The patch does not add the female gene where none was present, alter the original gene's behavior, or filter error messages.

## Affected mods and what is repaired

| Mod | Relevant repair |
| --- | --- |
| [Big and Small - Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=2925432336) | Hooks its generated xenotype-gene batch and prevents duplicate names. |
| [Big and Small - Genes & More](https://steamcommunity.com/sharedfiles/filedetails/?id=2920751126) | Supports the generated transformation genes used with this content. |
| [Big and Small - Races](https://steamcommunity.com/sharedfiles/filedetails/?id=2894397737) | Repairs inherited female-gene entries and links, reproduced on young, adult and old Troll xenotypes. |
| [Imago Xenotype](https://steamcommunity.com/sharedfiles/filedetails/?id=3816071711) | Preserves its original metamorphosis gene when B&S generates a gene with the same name. |
| [Alpha Genes](https://steamcommunity.com/sharedfiles/filedetails/?id=2891845502) | Supplies the canonical `AG_Female` gene to which female-gene links resolve. |
| [Vanilla Races Expanded - Android](https://steamcommunity.com/sharedfiles/filedetails/?id=2975771801) | Repairs duplicate female genes/links reproduced on Basic and Awakened Android xenotypes. |
| [Spawn Female Pawns Only 1.7](https://steamcommunity.com/sharedfiles/filedetails/?id=3822007133) | Repairs the inheritance overlap caused when female-gene XML additions reach both a parent and its children. Tested with Cinder's 1.7 continuation. |

The repairs are not restricted to those five affected xenotype names. Other mods using these same patterns can benefit. This is focused compatibility, not a guarantee that every gene pack works with every other mod. It does not repair duplicate mod package IDs or unrelated missing definitions.

## Installation

Extract the `GeneBridge` folder into RimWorld's normal `Mods` folder, then enable it. Load after Harmony and the affected mods you use. Disable the earlier standalone **Big and Small - Generated Gene Compatibility** and **Alpha Genes - AG_Female Link Compatibility** patches; Gene Bridge replaces both. Retain the actual content mods.

The supplied source and `Source/Build.ps1` rebuild the DLL using an installed RimWorld 1.7, Harmony and a .NET SDK with Roslyn. No game or third-party mod assemblies are redistributed in this package.

## Validation

43 isolated host assertions passed. A real RimWorld 1.7.4990 / Harmony 3 control startup reproduced all five female-link errors; the combined Gene Bridge startup removed all five and passed 13 native acceptance checks. The affected xenotypes' non-female genes and non-female links were identical between runs. The original female gene class, canonical references and Imago repair were preserved. Normal configuration and saves were unchanged during testing.

The installed-package startup additionally passed 16 native checks, including actual preview-image loading, with no logged errors. Duplicate mod package IDs remain outside this mod's scope and must be managed separately. Startup acceptance is not exhaustive gameplay metamorphosis, save-migration or developer hot-reload certification.

## Artwork and credits

The original Gene Bridge thumbnail was created with the built-in image-generation tool. `About/Preview.png` is its wide PNG export for RimWorld's uploader; the original artwork and prompt are retained in the project, outside the runtime package. The export is below Steam's documented [preview-image size limit](https://partner.steamgames.com/doc/api/ISteamUGC#SubmitItemUpdateResult_t).

Gene Bridge is original compatibility code. Big and Small, Imago, Alpha Genes, VRE Android and Spawn Female Pawns Only remain the work of their respective creators.
