using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ContentSourceColumnWorker<TObject> : ColumnWorker<TObject, ModContentPack?>
{
    protected ContentSourceColumnWorker(ColumnDef columndef) : base(columndef, ColumnCellStyle.String)
    {
    }
    protected abstract ModContentPack? GetModContentPack(TObject @object);
    protected sealed override DataCell GetCell(TObject @object)
    {
        var mod = GetModContentPack(@object);

        if (mod != null)
        {
            var widget = new Label(mod.Name)
            .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer)
            .Tooltip(mod.PackageIdPlayerFacing);

            return new(widget, mod);
        }

        return new(null, null);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> tableRecords)
    {
        var filterOptions = tableRecords
            .Select(record => Cells[record].Data)
            .Distinct()
            .OrderBy(mod => mod?.Name)
            .Select<ModContentPack?, NTMFilterOption<ModContentPack?>>(
                mod => mod == null ? new() : new(mod, mod.Name, null, mod.PackageIdPlayerFacing)
            );

        yield return new(ColumnDef.Title, Make.OTMFilter<TObject, ModContentPack?>(@object => Cells[@object].Data, filterOptions));
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        var modName1 = Cells[object1].Data?.Name;
        var modName2 = Cells[object1].Data?.Name;

        return Comparer.Default.Compare(modName1, modName2);
    }
}
