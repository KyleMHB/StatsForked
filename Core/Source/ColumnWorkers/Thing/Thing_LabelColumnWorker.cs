using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Thing_LabelColumnWorker : ColumnWorker<ThingAlike>
{
    public Thing_LabelColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    private static readonly Func<ThingAlike, string> GetThingLabel =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        // Note to myself: Don't remove stuff label. It's important because
        // modded stuffs may have the same color as vanilla ones or other modded stuffs.
        // Replacing label with icon won't do, because ex. all of the leathers have the same
        // icon but of different color.
        return thing.StuffDef == null
            ? thing.Def.LabelCap.RawText
            : $"{thing.StuffDef.LabelAsStuff.CapitalizeFirst()} {thing.Def.label}";
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        void openDefInfoDialog()
        {
            Draw.DefInfoDialog(thing.Def, thing.StuffDef);
        }

        return new HorizontalContainer(
            [
                new ThingIcon(thing.Def, thing.StuffDef).ToButtonGhostly(openDefInfoDialog),
                new Label(GetThingLabel(thing)),
            ],
            Globals.GUI.Pad
        )
        .Tooltip(thing.Def.description);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var anyThingIsMadeFromStuff = tableRecords.Any(record => record.StuffDef != null);
        var labelFilter = Make.StringFilter(GetThingLabel, "Label");

        if (anyThingIsMadeFromStuff)
        {
            var stuffFilter = Make.OTMThingDefFilter(thing => thing.StuffDef, tableRecords, "Material");
            var baseTypeFilter = Make.OTMThingDefFilter(thing => thing.Def, tableRecords, "Type");

            return Make.CompositeFilter<ThingAlike>([
                labelFilter,
                baseTypeFilter,
                new StuffedVariantsDisplayModeToggleButton(),
                stuffFilter,
            ]);
        }

        return labelFilter;
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetThingLabel(thing1).CompareTo(GetThingLabel(thing2));
    }

    private sealed class StuffedVariantsDisplayModeToggleButton : FilterWidget<ThingAlike>
    {
        private bool _IsActive = false;
        public override bool IsActive => _IsActive;
        public override event Action<FilterWidget<ThingAlike>>? OnChange;
        private const string ButtonText = "Distinct";
        private const float ButtonPaddingHor = Globals.GUI.PadSm;
        private static readonly TipSignal Manual =
            "Show only distinct item material variants.\n\n" +
            "Material for each distinct variant is chosen based on item's type definition.";
        protected override Vector2 CalcSize()
        {
            var size = Text.CalcSize(ButtonText);
            size.x += ButtonPaddingHor * 2f;

            return size;
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

            if (Widgets.Draw.ButtonTextSubtle(rect, ButtonText, GUI.color, ButtonPaddingHor))
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
    }
}
