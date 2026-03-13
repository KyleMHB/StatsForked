using System.Collections.Generic;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using UnityEngine;
using Verse;

namespace Stats;

public sealed partial class MainTabWindow : RimWorld.MainTabWindow
{
    public override Vector2 RequestedTabSize => new(UI.screenWidth, base.RequestedTabSize.y);
    internal static readonly Color BorderColor = new(1f, 1f, 1f, 0.4f);

    protected override float Margin { get => 1f; }
    private bool _isExpanded;
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
    private static readonly TipSignal _expandButtonTooltip = "Expand / Reset window";
    private Vector2 _tableListScrollPosition;

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
                    TableRecord tableRecord = new(tableDef, this);
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

        Rect toolbarRect = rect.CutByX(_ToolbarWidth);

        // Border
        if (isRepaint) toolbarRect.DrawBorderRight(BorderColor);

        // Expand window button
        Rect expandButtonRect = toolbarRect.CutByY(20f);
        DrawExpandButton(expandButtonRect);
        if (isRepaint) expandButtonRect.DrawBorderBottom(BorderColor);

        // "Open table" button
        Rect openTableButtonRect = toolbarRect.CutByY(_ToolbarWidth);
        DrawOpenTableButton(openTableButtonRect);
        if (isRepaint) openTableButtonRect.DrawBorderBottom(BorderColor);

        // Table list
        // TODO:
        // - Add culling.
        // - Add reordering.
        Rect tableListContentRect = new(0f, 0f, _ToolbarWidth, _tables.Count * _ToolbarWidth);
        using (new GUIScrollScope(toolbarRect, ref _tableListScrollPosition, tableListContentRect, false))
        {
            int tablesCount = _tables.Count;
            for (int i = 0; i < tablesCount; i++)
            {
                TableRecord tableRecord = _tables[i];
                Rect tableButtonRect = tableListContentRect.CutByY(_ToolbarWidth);
                tableRecord.Draw(tableButtonRect);
            }
        }

        // Table
        _activeTable?.TableWidget.Draw(rect);

        Text.WordWrap = wordWrap;
    }

    private void DrawOpenTableButton(Rect rect)
    {
        if (Event.current.IsRepaint())
        {
            rect
                .HighlightLight()
                .ContractedBy(_IconPadding)
                .DrawTextureFitted(TexButton.Plus)
                .Tip(_openTableButtonTooltip);
        }

        if (rect.ButtonGhostly())
        {
            _tableDefsMenu.Open();
        }
    }

    private void DrawExpandButton(Rect rect)
    {
        if (Event.current.IsRepaint())
        {
            Texture2D texture = _isExpanded ? TexButton.ReorderDown : TexButton.ReorderUp;
            rect
                .HighlightLight()
                .DrawTextureFitted(texture, 0.7f)
                .Tip(_expandButtonTooltip);
        }

        if (rect.ButtonGhostly())
        {
            ExpandOrResetWindow();
        }
    }

    private void RemoveTable(TableRecord table)
    {
        if (_activeTable == table)
        {
            int index = _tables.IndexOf(table);

            if (index > 0)
            {
                _activeTable = _tables[index - 1];
            }
            else if (_tables.Count > 1)
            {
                _activeTable = _tables[index + 1];
            }
            else
            {
                _activeTable = null;
            }
        }
        _tables.Remove(table);
    }

    private void ExpandOrResetWindow()
    {
        if (_isExpanded)
        {
            SetInitialSizeAndPosition();
        }
        else
        {
            windowRect.yMin = 0f;
        }

        _isExpanded = !_isExpanded;
    }

    public override void PreOpen()
    {
        base.PreOpen();

        if (_isExpanded)
        {
            windowRect.yMin = 0f;
        }
    }
}
