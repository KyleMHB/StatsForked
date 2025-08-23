using System.Collections.Generic;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.CE;

public sealed class Weapon_CaliberColumnWorker : ColumnWorker<ThingAlike, string?>
{
    public Weapon_CaliberColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    protected override DataCell GetCell(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var statRequest = StatRequest.For(thingDef, null);

        if (StatDefOf.Caliber.Worker.ShouldShowFor(statRequest))
        {
            var caliberName = StatDefOf.Caliber.Worker.GetStatDrawEntryLabel(
                StatDefOf.Caliber,
                StatDefOf.Caliber.Worker.GetValue(statRequest),
                ToStringNumberSense.Absolute,
                statRequest
            );

            if (caliberName?.Length > 0)
            {
                Widget widget = new Label(caliberName).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);
                var tooltip = StatDefOf.Caliber.Worker.GetExplanationFull(
                    statRequest,
                    ToStringNumberSense.Absolute,
                    StatDefOf.Caliber.Worker.GetValue(statRequest)
                );

                if (tooltip?.Length > 0)
                {
                    widget = widget.Tooltip(tooltip);
                }

                return new(widget, caliberName);
            }
        }

        return new(null, null);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
    {
        yield return new(ColumnDef.Title, Make.OTMFilter(thing => Cells[thing].Data, tableRecords));
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return Comparer<string?>.Default.Compare(Cells[thing1].Data, Cells[thing2].Data);
    }
}
