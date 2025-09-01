using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

// Note to myself: Don't remove stuff label. It's important because
// modded stuffs may have the same color as vanilla ones or other modded stuffs.
// Replacing label with icon won't do, because ex. all of the leathers have the same
// icon but of different color.
public sealed class Thing_LabelColumnWorker : ColumnWorker<AbstractThing>
{
    public Thing_LabelColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    public override ObjectTable.Cell GetCell(AbstractThing thing)
    {
        return new Cell(thing);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<AbstractThing> contextObjects)
    {
        yield return new(new Label("Label"), new StringFilter<Cell>(cell => cell.Text, this));

        var typeFilterOptions = contextObjects
            .Select(@object => @object.Def)
            .Distinct()
            .OrderBy(thingDef => thingDef.label)
            .Select<ThingDef, NTMFilterOption<ThingDef>>(
                thingDef => new(thingDef, thingDef.LabelCap, new ThingDefIcon(thingDef))
            );
        yield return new(new Label("Type"), new OTMFilter<Cell, ThingDef>(cell => cell.Def, typeFilterOptions, this));

        var filterWidget_Researched = new BooleanFilter<Cell>(
            thing => thing.Def.GetResearchProjectDef()?.All(researchProjectDef => researchProjectDef.IsFinished) is true or null,
            this
        );
        Globals.Events.OnResearchCompleted += () =>
        {
            if (filterWidget_Researched.IsActive)
            {
                filterWidget_Researched.NotifyChanged();
            }
        };
        yield return new(new Label("Researched"), filterWidget_Researched);

        if (contextObjects.Any(@object => @object.StuffDef != null))
        {
            yield return new(new Label("Distinct"), new StuffedVariantsDisplayModeToggleButton(this));

            var stuffFilterOptions = contextObjects
                .Select(@object => @object.StuffDef)
                .Distinct()
                .OrderBy(thingDef => thingDef?.label)
                .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
                    thingDef => thingDef == null
                        ? new()
                        : new(thingDef, thingDef.LabelCap, new ThingDefIcon(thingDef))
                );
            var stuffFilter = new OTMFilter<Cell, ThingDef?>(cell => cell.StuffDef, stuffFilterOptions, this);
            yield return new(new Label("Material"), stuffFilter);
        }
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public string Text { get; }
        public ThingDef Def { get; }
        public ThingDef? StuffDef { get; }
        public bool IsMadeFromDefaultStuff { get; }
        public Cell(AbstractThing thing)
        {
            Def = thing.Def;
            StuffDef = thing.StuffDef;
            IsMadeFromDefaultStuff = thing.StuffDef == thing.Def.GetDefaultStuff();

            var text = thing.StuffDef == null
                ? thing.Def.LabelCap.RawText
                : $"{thing.StuffDef.LabelAsStuff.CapitalizeFirst()} {thing.Def.label}";
            var widget = new HorizontalContainer([
                new ThingDefIcon(thing.Def, thing.StuffDef)
                .PaddingAbs(2f)
                .SizeAbs(Verse.Text.LineHeight + ObjectTable.CellPadVer * 2f)
                .ToButtonGhostly(() => Widgets.Draw.DefInfoDialog(thing.Def, thing.StuffDef)),
                new Label(text).PaddingAbs(0f, ObjectTable.CellPadVer),
            ], Globals.GUI.Pad)
            .PaddingAbs(ObjectTable.CellPadHor, 0f)
            .Tooltip(thing.Def.description);

            Widget = widget;
            Text = text;
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Text.CompareTo(((Cell)cell).Text);
        }
    }

    private sealed class StuffedVariantsDisplayModeToggleButton : FilterWidget
    {
        private bool _IsActive = false;
        public override bool IsActive => _IsActive;
        public override event Action<FilterWidget>? OnChange;
        private Texture2D Texture => _IsActive ? Verse.Widgets.CheckboxOnTex : Verse.Widgets.CheckboxOffTex;
        private static readonly TipSignal Manual =
            "Click to show only distinct item material variants.\n\n" +
            "Material for each distinct variant is chosen based on item's type definition.";
        private readonly Thing_LabelColumnWorker Column;
        public StuffedVariantsDisplayModeToggleButton(Thing_LabelColumnWorker column)
        {
            Column = column;
        }
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
        public override bool Eval(Dictionary<ColumnWorker, ObjectTable.Cell> cells)
        {
            if (_IsActive)
            {
                // Do not filter out stuffless things.
                // - You can filter them out with stuff filter.
                // - There are cases where one may want to compare
                //   things by stats unrelated to stuff. Ex. equipped
                //   stat offsets.
                return ((Cell)cells[Column]).IsMadeFromDefaultStuff;
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
