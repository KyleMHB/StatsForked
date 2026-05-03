using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.Compat.Odyssey;

public abstract class OdysseyThingTextColumnWorker(ColumnDef columnDef) : ColumnWorker<DefBasedObject, OdysseyThingTextColumnWorker.TextCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override TextCell MakeCell(DefBasedObject @object)
    {
        return @object.Def is ThingDef thingDef ? new TextCell(GetText(thingDef)) : default;
    }

    protected abstract string? GetText(ThingDef thingDef);

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter textFieldFilter = new StringFilter(row => this[row].Text ?? "");
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        return [new CellField(Def.TitleWidget, textFieldFilter, Compare)];
    }

    public readonly struct TextCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly string? Text;

        private readonly TipSignal _tooltip;

        public TextCell(string? text)
        {
            Text = text;
            string preview = text?.Truncate(80) ?? "";
            Width = Verse.Text.CalcSize(preview).x;
            _tooltip = text ?? "";
        }

        public void Draw(Rect rect)
        {
            if (Text != null)
            {
                rect
                    .Label(Text.Truncate(80), GUIStyles.TableCell.String)
                    .Tip(_tooltip);
            }
        }
    }
}

public abstract class OdysseyThingNumberColumnWorker(ColumnDef columnDef, string formatString = "") : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        return @object.Def is ThingDef thingDef && TryGetValue(thingDef, out decimal value)
            ? new NumberCell(value, formatString)
            : default;
    }

    protected abstract bool TryGetValue(ThingDef thingDef, out decimal value);
}

public sealed class OdysseyThingDescriptionColumnWorker(ColumnDef columnDef) : OdysseyThingTextColumnWorker(columnDef)
{
    protected override string? GetText(ThingDef thingDef)
    {
        return thingDef.description;
    }
}

public sealed class BookOutcomesColumnWorker(ColumnDef columnDef) : OdysseyThingTextColumnWorker(columnDef)
{
    protected override string? GetText(ThingDef thingDef)
    {
        IEnumerable<object> doers = OdysseyReflection.GetCompValues(thingDef, "CompProperties_Book", "doers");
        string[] labels = doers
            .Select(doer => OdysseyReflection.ValueToString(OdysseyReflection.GetMemberValue(doer, "label"))
                ?? OdysseyReflection.ValueToString(OdysseyReflection.GetMemberValue(doer, "outcomeDoer"))
                ?? doer.GetType().Name)
            .Where(label => label.NullOrEmpty() == false)
            .Distinct()
            .ToArray();

        return labels.Length == 0 ? null : string.Join(", ", labels);
    }
}

public sealed class FishCategoriesColumnWorker(ColumnDef columnDef) : OdysseyThingTextColumnWorker(columnDef)
{
    protected override string? GetText(ThingDef thingDef)
    {
        string[] categories = thingDef.thingCategories?
            .Select(category => category.LabelCap.RawText)
            .Where(label => label.NullOrEmpty() == false)
            .ToArray() ?? [];

        return categories.Length == 0 ? null : string.Join(", ", categories);
    }
}

public sealed class GravshipComponentTypeColumnWorker(ColumnDef columnDef) : OdysseyThingTextColumnWorker(columnDef)
{
    protected override string? GetText(ThingDef thingDef)
    {
        return OdysseyReflection.ValueToString(OdysseyReflection.GetGravshipCompMemberValue(thingDef, "componentTypeDef"));
    }
}

public sealed class GravshipRangeColumnWorker(ColumnDef columnDef) : OdysseyThingNumberColumnWorker(columnDef)
{
    protected override bool TryGetValue(ThingDef thingDef, out decimal value)
    {
        return OdysseyReflection.TryGetGravshipStatOffset(thingDef, "GravshipRange", out value);
    }
}

public sealed class GravshipSubstructureSupportColumnWorker(ColumnDef columnDef) : OdysseyThingNumberColumnWorker(columnDef)
{
    protected override bool TryGetValue(ThingDef thingDef, out decimal value)
    {
        return OdysseyReflection.TryGetGravshipStatOffset(thingDef, "SubstructureSupport", out value);
    }
}

public sealed class GravshipFuelSavingsColumnWorker(ColumnDef columnDef) : OdysseyThingNumberColumnWorker(columnDef, "0.#%")
{
    protected override bool TryGetValue(ThingDef thingDef, out decimal value)
    {
        if (OdysseyReflection.TryGetDecimal(OdysseyReflection.GetGravshipCompMemberValue(thingDef, "fuelSavingsPercent"), out value))
        {
            return true;
        }

        value = 0m;
        return false;
    }
}

public sealed class GravshipMaxSimultaneousColumnWorker(ColumnDef columnDef) : OdysseyThingNumberColumnWorker(columnDef)
{
    protected override bool TryGetValue(ThingDef thingDef, out decimal value)
    {
        return OdysseyReflection.TryGetDecimal(OdysseyReflection.GetGravshipCompMemberValue(thingDef, "maxSimultaneous"), out value);
    }
}

public sealed class GravshipMaxDistanceColumnWorker(ColumnDef columnDef) : OdysseyThingNumberColumnWorker(columnDef)
{
    protected override bool TryGetValue(ThingDef thingDef, out decimal value)
    {
        return OdysseyReflection.TryGetDecimal(OdysseyReflection.GetGravshipCompMemberValue(thingDef, "maxDistance"), out value);
    }
}

public sealed class GravshipDirectionInfluenceColumnWorker(ColumnDef columnDef) : OdysseyThingNumberColumnWorker(columnDef)
{
    protected override bool TryGetValue(ThingDef thingDef, out decimal value)
    {
        return OdysseyReflection.TryGetDecimal(OdysseyReflection.GetGravshipCompMemberValue(thingDef, "directionInfluence"), out value);
    }
}

public sealed class OrbitalInfrastructureFunctionColumnWorker(ColumnDef columnDef) : OdysseyThingTextColumnWorker(columnDef)
{
    protected override string? GetText(ThingDef thingDef)
    {
        List<string> functions = [];
        if (OdysseyReflection.HasCompClass(thingDef, "CompOrbitalScanner"))
        {
            functions.Add("Orbital scanner");
        }
        if (OdysseyReflection.HasComp(thingDef, "CompProperties_OxygenPusher"))
        {
            functions.Add("Oxygen");
        }
        if (thingDef.thingClass?.Name == "Building_VacBarrier")
        {
            functions.Add("Vac barrier");
        }

        return functions.Count == 0 ? null : string.Join(", ", functions);
    }
}

public sealed class OrbitalInfrastructureLowPowerFactorColumnWorker(ColumnDef columnDef) : OdysseyThingNumberColumnWorker(columnDef, "0.#%")
{
    protected override bool TryGetValue(ThingDef thingDef, out decimal value)
    {
        return OdysseyReflection.TryGetDecimal(OdysseyReflection.GetCompMemberValue(thingDef, "CompProperties_LowPowerUnlessVacuum", "lowPowerConsumptionFactor"), out value);
    }
}

public sealed class OrbitalInfrastructureAirPerSecondColumnWorker(ColumnDef columnDef) : OdysseyThingNumberColumnWorker(columnDef, "0.####")
{
    protected override bool TryGetValue(ThingDef thingDef, out decimal value)
    {
        return OdysseyReflection.TryGetDecimal(OdysseyReflection.GetCompMemberValue(thingDef, "CompProperties_OxygenPusher", "airPerSecondPerHundredCells"), out value);
    }
}

public sealed class UniqueWeaponTraitsColumnWorker(ColumnDef columnDef) : OdysseyThingTextColumnWorker(columnDef)
{
    protected override string? GetText(ThingDef thingDef)
    {
        object? comp = OdysseyReflection.GetComp(thingDef, "CompProperties_UniqueWeapon");
        if (comp == null)
        {
            return null;
        }

        IEnumerable<object> traits = OdysseyReflection.GetEnumerableMemberValue(comp, "traits")
            ?? OdysseyReflection.GetEnumerableMemberValue(comp, "weaponTraits")
            ?? [];

        string[] labels = traits
            .Select(OdysseyReflection.ValueToString)
            .Where(label => label.NullOrEmpty() == false)
            .ToArray()!;

        return labels.Length == 0 ? null : string.Join(", ", labels);
    }
}

