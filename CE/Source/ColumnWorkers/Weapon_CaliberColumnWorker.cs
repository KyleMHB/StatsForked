using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.Compat.CE;

public sealed class Weapon_CaliberColumnWorker(ColumnDef columnDef) : ColumnWorker<DefBasedObject, Weapon_CaliberColumnWorker.CaliberCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override CaliberCell MakeCell(DefBasedObject @object)
    {
        StatRequest statRequest = GetStatRequest(@object);
        string? caliberName = GetCaliberName(statRequest);
        if (string.IsNullOrEmpty(caliberName))
        {
            return default;
        }

        TipSignal? tooltip = null;
        string explanation = StatDefOf.Caliber.Worker.GetExplanationFull(
            statRequest,
            ToStringNumberSense.Absolute,
            StatDefOf.Caliber.Worker.GetValue(statRequest));
        if (explanation.Length > 0)
        {
            tooltip = explanation;
        }

        return new CaliberCell(caliberName!, tooltip);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<string?>> options = ((IRefRecordsProvider<ThingDef>)tableWorker).Records
            .Select(thingDef => GetCaliberName(GetStatRequest(thingDef)))
            .Distinct()
            .OrderBy(option => option)
            .Select<string?, NTMFilterOption<string?>>(option => option == null ? new() : new(option, option));
        Filter filter = new OTMFilter<string?>(row => this[row].Value, options);
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Value, this[row2].Value);
        return [new CellField(Def.TitleWidget, filter, Compare)];
    }

    private static StatRequest GetStatRequest(DefBasedObject @object)
    {
        ThingDef thingDef = GetGunDef((ThingDef)@object.Def);
        return StatRequest.For(thingDef, null, @object.Quality);
    }

    private static StatRequest GetStatRequest(ThingDef thingDef)
    {
        return StatRequest.For(GetGunDef(thingDef), null);
    }

    private static ThingDef GetGunDef(ThingDef thingDef)
    {
        return thingDef.building?.turretGunDef ?? thingDef;
    }

    private static string? GetCaliberName(StatRequest statRequest)
    {
        if (StatDefOf.Caliber.Worker.ShouldShowFor(statRequest) == false)
        {
            return null;
        }

        return StatDefOf.Caliber.Worker.GetStatDrawEntryLabel(
            StatDefOf.Caliber,
            StatDefOf.Caliber.Worker.GetValue(statRequest),
            ToStringNumberSense.Absolute,
            statRequest);
    }

    public readonly struct CaliberCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public string? Value { get; }

        private readonly TipSignal? _tooltip;

        public CaliberCell(string value, TipSignal? tooltip)
        {
            Value = value;
            _tooltip = tooltip;
            Width = Verse.Text.CalcSize(value).x;
        }

        public void Draw(Rect rect)
        {
            if (Value == null)
            {
                return;
            }

            if (_tooltip != null && Mouse.IsOver(rect))
            {
                rect.Tip(_tooltip.Value);
            }

            rect.Label(Value, GUIStyles.TableCell.String);
        }
    }
}
