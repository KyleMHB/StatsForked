using System.Collections.Generic;
using Stats.Extensions;
using Stats.GUIScopes;
using UnityEngine;
using Verse;

namespace Stats;

public sealed partial class MainTabWindow : RimWorld.MainTabWindow
{
    public override Vector2 RequestedTabSize => new(UI.screenWidth, base.RequestedTabSize.y);
    internal static readonly Color BorderColor = new(1f, 1f, 1f, 0.4f);

    protected override float Margin { get => 1f; }
    private bool IsExpanded;
    private const float _ToolbarWidth = 40f;
    private const float _IconPadding = 5f;
    //private static readonly TipSignal Manual =
    //    "- Hold (LMB) and move mouse cursor to scroll horizontally.\n" +
    //    "- Hold [Ctrl] and click on a column's name to pin/unpin it.\n" +
    //    "- Hold [Ctrl] and click on a row to pin/unpin it.\n" +
    //    "  - You can pin multiple rows.\n" +
    //    "  - Pinned rows are unaffected by filters.";
    private readonly List<TableRecord> _tables;
    private TableRecord? _activeTable;
    private readonly FloatMenu _tableDefsMenu;
    private static readonly TipSignal _openTableButtonTooltip = "Open table";

    public MainTabWindow()
    {
        List<TableDef> tableDefs = DefDatabase<TableDef>.AllDefsListForReading;
        int tableDefsCount = tableDefs.Count;
        List<FloatMenuOption> tableDefsMenuOptions = new(tableDefsCount);
        for (int i = 0; i < tableDefsCount; i++)
        {
            TableDef tableDef = tableDefs[i];
            FloatMenuOption menuOption = new(
                tableDef.LabelCap,
                () =>
                {
                    TableRecord tableRecord = new(tableDef);
                    _tables.Insert(0, tableRecord);
                    _activeTable = tableRecord;
                },
                tableDef.Icon,
                tableDef.iconColor
            );
            tableDefsMenuOptions.Add(menuOption);
        }
        tableDefsMenuOptions.SortBy(option => option.Label);

        _tableDefsMenu = new FloatMenu(tableDefsMenuOptions);
        _tables = new(tableDefsCount);
    }

    public override void DoWindowContents(Rect rect)
    {
        bool isRepaint = Event.current.type == EventType.Repaint;

        // TODO: Remove this after you'll explixitly set word wrap for every inner widget.
        bool wordWrap = Text.WordWrap;
        Text.WordWrap = false;

        #region Toolbar
        Rect toolbarRect = rect.CutByX(_ToolbarWidth);

        #region "Open table" button
        Rect openTableButtonRect = toolbarRect.CutByY(_ToolbarWidth);
        if (isRepaint)
        {
            Verse.Widgets.DrawLightHighlight(openTableButtonRect);
            Rect openTableButtonIconRect = openTableButtonRect.ContractedBy(_IconPadding);
            Verse.Widgets.DrawTextureFitted(openTableButtonIconRect, TexButton.Plus, 1f);
            TooltipHandler.TipRegion(openTableButtonRect, _openTableButtonTooltip);
        }

        if (Widgets.Draw.ButtonGhostly(openTableButtonRect))
        {
            Find.WindowStack.Add(_tableDefsMenu);
        }
        #endregion "Open table" button

        #region Table list
        int tablesCount = _tables.Count;
        for (int i = 0; i < tablesCount; i++)
        {
            TableRecord tableRecord = _tables[i];
            Rect tableRect = toolbarRect.CutByY(_ToolbarWidth);
            if (isRepaint)
            {
                if (_activeTable == tableRecord)
                {
                    Verse.Widgets.DrawHighlightSelected(tableRect);
                }

                using (new GUIColorScope(tableRecord.IconColor))
                {
                    Rect tableIconRect = tableRect.ContractedBy(_IconPadding);
                    Verse.Widgets.DrawTextureFitted(tableIconRect, tableRecord.Icon, tableRecord.IconScale);
                }

                TooltipHandler.TipRegion(tableRect, tableRecord.Tooltip);
            }

            if (Widgets.Draw.ButtonGhostly(tableRect))
            {
                _activeTable = tableRecord;
            }
        }
        #endregion Table list

        Widgets.Draw.VerticalLine(_ToolbarWidth - 1f, 0f, rect.height, BorderColor);
        #endregion Toolbar

        _activeTable?.TableWidget.Draw(rect);

        GUIDebugger.DrawDebugInfo(rect);

        Text.WordWrap = wordWrap;
    }

    private void ExpandOrResetWidow()
    {
        if (IsExpanded)
        {
            SetInitialSizeAndPosition();
        }
        else
        {
            windowRect.yMin = 0f;
        }

        IsExpanded = !IsExpanded;
    }

    public override void PreOpen()
    {
        base.PreOpen();

        if (IsExpanded)
        {
            windowRect.yMin = 0f;
        }
    }
}
