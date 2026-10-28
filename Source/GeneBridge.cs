using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

[assembly: AssemblyVersion("1.0.0.0")]

namespace SweetBriana.GeneBridge
{
    public sealed class GeneBridgeMod : Mod
    {
        public const string HarmonyId = "sweetbriana.genebridge";
        public GeneBridgeMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony(HarmonyId);
            var generator = AccessTools.Method("BigAndSmall.DefGeneratorPatches:GenerateXenotypeGenes");
            if (Supported(generator))
                harmony.Patch(generator, postfix: new HarmonyMethod(typeof(CollisionFix), nameof(CollisionFix.AfterGenerate)));
            else if (generator != null)
                Log.Warning("[Gene Bridge] B&S generator signature changed; name-collision repair was not applied.");
            harmony.Patch(AccessTools.Method(typeof(XenotypeDef), nameof(XenotypeDef.ResolveReferences)),
                prefix: new HarmonyMethod(typeof(FemaleReferences), nameof(FemaleReferences.BeforeXenotypeResolve)),
                postfix: new HarmonyMethod(typeof(FemaleReferences), nameof(FemaleReferences.AfterResolve)));
            harmony.Patch(AccessTools.Method(typeof(Def), nameof(Def.ResolveReferences)),
                postfix: new HarmonyMethod(typeof(FemaleReferences), nameof(FemaleReferences.AfterResolve)));
        }
        internal static bool Supported(MethodInfo method)
        {
            return method != null && method.IsStatic
                && method.ReturnType == typeof(List<GeneDef>)
                && method.GetParameters().Length == 0;
        }
    }
    public static class CollisionFix
    {
        public const string GeneratedPrefix = "SweetBriana_BS_";

        // Inspect the whole B&S batch before its caller registers implied defs.
        // Reserving all original names first prevents a renamed gene from taking
        // the normal name of a different gene later in the same batch.
        public static void AfterGenerate(List<GeneDef> __result)
        {
            if (__result == null || __result.Count == 0) return;
            var occupied = new HashSet<string>(StringComparer.Ordinal);
            foreach (var existing in DefDatabase<GeneDef>.AllDefsListForReading)
                if (existing != null && !string.IsNullOrEmpty(existing.defName)) occupied.Add(existing.defName);
            foreach (var gene in __result)
                if (gene != null && !string.IsNullOrEmpty(gene.defName)) occupied.Add(gene.defName);

            var seenNames = new HashSet<string>(StringComparer.Ordinal);
            var seenObjects = new HashSet<GeneDef>();
            foreach (var gene in __result)
            {
                if (gene == null || string.IsNullOrEmpty(gene.defName) || !seenObjects.Add(gene)) continue;
                var name = gene.defName;
                var existing = DefDatabase<GeneDef>.GetNamedSilentFail(name);
                var duplicateInBatch = !seenNames.Add(name);
                if (!duplicateInBatch && (existing == null || ReferenceEquals(existing, gene))) continue;

                // Never mutate another mod's registered definition in place.
                if (ReferenceEquals(existing, gene))
                {
                    Log.Warning("[Gene Bridge] Existing definition object returned twice: " + name);
                    continue;
                }
                var stem = GeneratedPrefix + name;
                var replacement = stem;
                for (var suffix = 2; occupied.Contains(replacement); suffix++)
                    replacement = stem + "_" + suffix.ToString(CultureInfo.InvariantCulture);
                gene.defName = replacement;
                occupied.Add(replacement);
                seenNames.Add(replacement);
                Log.Message("[Gene Bridge] Generated gene " + name
                    + " renamed to " + replacement + "; existing definitions retained.");
            }
        }
    }
    public static class FemaleReferences
    {
        public const string GeneName = "AG_Female";

        // XML patches run before inheritance. A child can acquire the same gene
        // twice, despite an XPath checking its own local gene list. Normalize the
        // resolved list before vanilla creates one description link per entry.
        public static void BeforeXenotypeResolve(XenotypeDef __instance)
        {
            var genes = __instance?.genes;
            if (genes == null || genes.Count == 0) return;
            var canonical = DefDatabase<GeneDef>.GetNamedSilentFail(GeneName);
            if (canonical == null) return;

            bool seen = false;
            List<GeneDef> corrected = null;
            for (int i = 0; i < genes.Count; i++)
            {
                var gene = genes[i];
                if (gene != null && gene.defName == GeneName)
                {
                    if (seen)
                    {
                        if (corrected == null) corrected = genes.GetRange(0, i);
                        continue;
                    }
                    seen = true;
                    if (!ReferenceEquals(gene, canonical) && corrected == null) corrected = genes.GetRange(0, i);
                    gene = canonical;
                }
                corrected?.Add(gene);
            }
            if (corrected != null)
            {
                __instance.genes = corrected;
                Log.Message("[Gene Bridge] Resolved one canonical female gene for " + __instance.defName + ".");
            }
        }

        // Run for base Defs, then again after XenotypeDef adds its automatic links.
        // Correct the data itself; normal configuration validation still runs.
        public static void AfterResolve(Def __instance)
        {
            var links = __instance?.descriptionHyperlinks;
            if (links == null || links.Count == 0) return;
            var canonical = DefDatabase<GeneDef>.GetNamedSilentFail(GeneName);
            if (canonical == null) return;

            bool seen = false;
            List<DefHyperlink> corrected = null;
            for (int i = 0; i < links.Count; i++)
            {
                var link = links[i];
                var gene = link?.def as GeneDef;
                if (gene != null && gene.defName == GeneName)
                {
                    if (seen)
                    {
                        if (corrected == null) corrected = links.GetRange(0, i);
                        continue;
                    }
                    seen = true;
                    if (!ReferenceEquals(gene, canonical))
                    {
                        if (corrected == null) corrected = links.GetRange(0, i);
                        link = new DefHyperlink(canonical) { faction = link.faction };
                    }
                }
                corrected?.Add(link);
            }
            if (corrected != null)
            {
                __instance.descriptionHyperlinks = corrected;
                Log.Message("[Gene Bridge] Resolved one canonical female-gene description link for " + __instance.defName + ".");
            }
        }
    }
}
