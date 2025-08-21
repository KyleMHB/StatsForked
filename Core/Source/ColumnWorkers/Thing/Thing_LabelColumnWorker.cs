using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Thing_LabelColumnWorker : ColumnWorker<ThingAlike, string>
{
    public Thing_LabelColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    protected override Cell GetCell(ThingAlike thing)
    {
        // Note to myself: Don't remove stuff label. It's important because
        // modded stuffs may have the same color as vanilla ones or other modded stuffs.
        // Replacing label with icon won't do, because ex. all of the leathers have the same
        // icon but of different color.
        var text = thing.StuffDef == null
            ? thing.Def.LabelCap.RawText
            : $"{thing.StuffDef.LabelAsStuff.CapitalizeFirst()} {thing.Def.label}";
        var widget = new HorizontalContainer([
            new ThingIcon(thing.Def, thing.StuffDef)
            .SizeAbs(Text.LineHeight + (ObjectTable.CellPadVer - 2f) * 2f)
            .PaddingAbs(2f)
            .ToButtonGhostly(()=>Draw.DefInfoDialog(thing.Def, thing.StuffDef)),
            new Label(text).PaddingAbs(0f, ObjectTable.CellPadVer),
        ], Globals.GUI.Pad)
        .PaddingAbs(ObjectTable.CellPadHor, 0f)
        //.PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer)
        .Tooltip(thing.Def.description);

        return new(widget, text);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
    {
        yield return new(new Label("Label"), Make.StringFilter<ThingAlike>(thing => Cells[thing].Data));
        yield return new(new Label("Type"), Make.OTMThingDefFilter(thing => thing.Def, tableRecords));
        var filterWidget_Researched = Make.BooleanFilter<ThingAlike>(
            thing => thing.Def.GetResearchProjectDef()?.All(researchProjectDef => researchProjectDef.IsFinished) is true or null
        );
        Globals.Events.OnResearchCompleted += () =>
        {
            if (filterWidget_Researched.IsActive)
            {
                filterWidget_Researched.NotifyChanged();
            }
        };
        yield return new(new Label("Researched"), filterWidget_Researched);

        if (tableRecords.Any(record => record.StuffDef != null))
        {
            var stuffFilter = Make.OTMThingDefFilter(thing => thing.StuffDef, tableRecords);
            yield return new(new Label("Distinct"), new StuffedVariantsDisplayModeToggleButton());
            yield return new(new Label("Material"), stuffFilter);
        }
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return Cells[thing1].Data.CompareTo(Cells[thing2].Data);
    }

    private sealed class StuffedVariantsDisplayModeToggleButton : FilterWidget<ThingAlike>
    {
        private bool _IsActive = false;
        public override bool IsActive => _IsActive;
        public override event Action<FilterWidget<ThingAlike>>? OnChange;
        private Texture2D Texture => _IsActive ? Verse.Widgets.CheckboxOnTex : Verse.Widgets.CheckboxOffTex;
        private static readonly TipSignal Manual =
            "Click to show only distinct item material variants.\n\n" +
            "Material for each distinct variant is chosen based on item's type definition.";
        protected override Vector2 CalcSize()
        {
            return new Vector2(Text.LineHeight, Text.LineHeight);
        }
        public override void Draw(Rect rect, Vector2 _)
        {
            var origTextAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.LowerLeft;

            var origGUIColor = GUI.color;
            if (_IsActive == false)
            {
                GUI.color = Globals.GUI.TextColorSecondary;
            }

            if (Widgets.Draw.ButtonImageSubtle(rect, Texture))
            {
                _IsActive = !_IsActive;

                OnChange?.Invoke(this);
            }

            Text.Anchor = origTextAnchor;
            GUI.color = origGUIColor;

            TooltipHandler.TipRegion(rect, Manual);
        }
        public override bool Eval(ThingAlike thing)
        {
            if (_IsActive)
            {
                // Do not filter out stuffless things.
                // - You can filter them out with stuff filter.
                // - There are cases where one may want to compare
                //   things by stats unrelated to stuff. Ex. equipped
                //   stat offsets.
                return thing.StuffDef == thing.Def.GetDefaultStuff();
            }

            return true;
        }
        public override void Reset()
        {
        }
        public override void NotifyChanged()
        {
            OnChange?.Invoke(this);
        }
    }
}
