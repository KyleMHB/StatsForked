using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class Thing_TechLevelColumnWorker : ColumnWorker<ThingAlike, TechLevel>
{
    public Thing_TechLevelColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    protected override DataCell GetCell(ThingAlike thing)
    {
        var techLevel = thing.Def.techLevel;

        if (thing.Def.techLevel != TechLevel.Undefined)
        {
            var text = techLevel.ToStringHuman().CapitalizeFirst();
            var widget = new Label(text).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            return new(widget, techLevel);
        }

        return new(null, TechLevel.Undefined);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
    {
        var options = tableRecords
            .Select(thing => thing.Def.techLevel)
            .Distinct()
            .OrderBy(techLevel => techLevel)
            .Select<TechLevel, NTMFilterOption<TechLevel>>(
                techLevel => new(techLevel, techLevel.ToStringHuman().CapitalizeFirst())
            );

        yield return new(ColumnDef.Title, Make.OTMFilter((ThingAlike thing) => thing.Def.techLevel, options));
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return thing1.Def.techLevel.CompareTo(thing2.Def.techLevel);
    }
}
