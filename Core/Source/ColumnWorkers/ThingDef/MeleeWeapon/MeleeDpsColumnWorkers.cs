using System;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;
using RimWorld;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.MeleeWeapon;

public sealed class DpsArmorPenetrationColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is not Verse.ThingDef thingDef)
        {
            return default;
        }

        StatDef? dpsStat = DefDatabase<StatDef>.GetNamedSilentFail("MeleeWeapon_AverageDPS");
        StatDef? armorPenetrationStat = DefDatabase<StatDef>.GetNamedSilentFail("MeleeWeapon_AverageArmorPenetration");
        if (dpsStat == null || armorPenetrationStat == null)
        {
            return default;
        }

        float dps = thingDef.GetStatValuePerceived(dpsStat, @object.StuffDef, @object.Quality);
        if (dps <= 0f)
        {
            return default;
        }

        float armorPenetration = thingDef.GetStatValuePerceived(armorPenetrationStat, @object.StuffDef, @object.Quality);
        decimal cellValue = (dps * (1f + Math.Max(0f, armorPenetration))).ToDecimal(2);
        return new NumberCell(cellValue, "0.00");
    }
}

public sealed class BluntDpsColumnWorker(ColumnDef columnDef) : TypedDpsColumnWorker(columnDef)
{
    protected override bool MatchesDamageDef(DamageDef damageDef)
    {
        return damageDef == DamageDefOf.Blunt;
    }
}

public sealed class SharpDpsColumnWorker(ColumnDef columnDef) : TypedDpsColumnWorker(columnDef)
{
    protected override bool MatchesDamageDef(DamageDef damageDef)
    {
        return damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp;
    }
}

public abstract class TypedDpsColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected abstract bool MatchesDamageDef(DamageDef damageDef);

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is not Verse.ThingDef thingDef || thingDef.tools == null)
        {
            return default;
        }

        float weightedDps = 0f;
        float totalChanceFactor = 0f;
        float matchingChanceFactor = 0f;

        foreach (Tool tool in thingDef.tools)
        {
            if (tool == null || tool.cooldownTime <= 0f || tool.power <= 0f)
            {
                continue;
            }

            float chanceFactor = Math.Max(0f, tool.chanceFactor);
            if (chanceFactor <= 0f)
            {
                continue;
            }

            totalChanceFactor += chanceFactor;
            if (TryGetMatchingDamageDef(tool, out DamageDef? damageDef) == false || damageDef == null)
            {
                continue;
            }

            float cooldown = GetCooldown(tool, thingDef, @object);
            if (cooldown <= 0f)
            {
                continue;
            }

            float damage = GetDamage(tool, thingDef, @object, damageDef);
            if (damage <= 0f)
            {
                continue;
            }

            matchingChanceFactor += chanceFactor;
            weightedDps += damage / cooldown * chanceFactor;
        }

        if (matchingChanceFactor <= 0f || totalChanceFactor <= 0f)
        {
            return default;
        }

        decimal cellValue = (weightedDps / totalChanceFactor).ToDecimal(2);
        return new NumberCell(cellValue, "0.00");
    }

    private bool TryGetMatchingDamageDef(Tool tool, out DamageDef? damageDef)
    {
        foreach (ManeuverDef maneuverDef in tool.Maneuvers)
        {
            DamageDef? maneuverDamageDef = maneuverDef.verb?.meleeDamageDef;
            if (maneuverDamageDef != null && MatchesDamageDef(maneuverDamageDef))
            {
                damageDef = maneuverDamageDef;
                return true;
            }
        }

        damageDef = null;
        return false;
    }

    private static float GetDamage(Tool tool, Verse.ThingDef thingDef, DefBasedObject @object, DamageDef damageDef)
    {
        if (@object.Thing != null)
        {
            return tool.AdjustedBaseMeleeDamageAmount(@object.Thing, damageDef);
        }

        float damage = tool.AdjustedBaseMeleeDamageAmount(thingDef, thingDef.GetStatStuff(@object.StuffDef), damageDef);

        StatDef? damageMultiplierStat = DefDatabase<StatDef>.GetNamedSilentFail("MeleeWeapon_DamageMultiplier");
        if (damageMultiplierStat != null && @object.Quality != QualityCategory.Normal)
        {
            damage *= thingDef.GetStatValuePerceived(damageMultiplierStat, @object.StuffDef, @object.Quality);
        }

        return damage;
    }

    private static float GetCooldown(Tool tool, Verse.ThingDef thingDef, DefBasedObject @object)
    {
        return @object.Thing != null
            ? tool.AdjustedCooldown(@object.Thing)
            : tool.AdjustedCooldown(thingDef, thingDef.GetStatStuff(@object.StuffDef));
    }
}
