using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

internal static class YayoAmmo
{
    // Yayo attaches the vanilla reloadable component after applying its ammo settings.
    internal static CompProperties_ApparelReloadable? GetProperties(Verse.ThingDef? def)
    {
        return def?.GetCompProperties<CompProperties_ApparelReloadable>();
    }
}

public sealed class YayoAmmosColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        CompProperties_ApparelReloadable? props = YayoAmmo.GetProperties(@object.Def as Verse.ThingDef);
        return props?.ammoDef != null ? new NumberCell(props.maxCharges) : default;
    }
}

public sealed class YayoReloadCostColumnWorker(ColumnDef columnDef) : ThingDefCountColumnWorker<DefBasedObject, ThingDefCountCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefCountCell MakeCell(DefBasedObject @object)
    {
        CompProperties_ApparelReloadable? props = YayoAmmo.GetProperties(@object.Def as Verse.ThingDef);
        return props?.ammoDef != null && props.ammoCountPerCharge > 0
            ? new ThingDefCountCell(props.ammoDef, props.ammoCountPerCharge)
            : default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return DefDatabase<Verse.ThingDef>.AllDefsListForReading
            .Select(def => YayoAmmo.GetProperties(def)?.ammoDef)
            .Distinct();
    }
}
