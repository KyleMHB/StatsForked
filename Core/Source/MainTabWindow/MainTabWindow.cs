using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.MainTabWindow;

namespace Stats;

public sealed partial class MainTabWindow : RimWorld.MainTabWindow
{
    public override Vector2 RequestedTabSize => new(UI.screenWidth, windowRect.height);

    protected override float Margin { get => 1f; }
    private readonly List<TableRecord> _tables;
    private TableRecord? _activeTable;
    private readonly FloatMenu _tableDefsMenu;
    private static readonly TipSignal _openTableButtonTooltip = "Open table";
    private static readonly TipSignal _expandButtonTooltip =
        "- Double click to Expand / Reset window\n" +
        "- Pull to change window height";
    private Vector2 _tableListScrollPosition;
    private readonly float _defaultHeight;
    private bool IsExpanded => windowRect.yMin == 0f;
    private bool _isResized;

    public MainTabWindow()
    {
        _defaultHeight = base.RequestedTabSize.y;
        windowRect.height = _defaultHeight;
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
        Event @event = Event.current;

        // TODO: Remove this after you'll explixitly set word wrap for every inner widget.
        bool wordWrap = Text.WordWrap;
        Text.WordWrap = false;

        // Layout
        rect
            .CutLeft(out Rect toolbarRect, ToolbarWidth)
            .TakeRest(out Rect tableRect);
        toolbarRect
            .CutTop(out Rect expandButtonRect, 20f)
            .CutTop(out Rect openTableButtonRect, ToolbarWidth)
            .TakeRest(out Rect tableListRect);

        // Border
        if (@event.type == EventType.Repaint)
        {
            toolbarRect.DrawBorderRight(BorderColor);
        }

        // Buttons
        DrawExpandButton(expandButtonRect);
        DrawOpenTableButton(openTableButtonRect);

        // Table list
        // TODO:
        // - Add culling.
        // - Add reordering.
        Rect tableListContentRect = new(0f, 0f, ToolbarWidth, _tables.Count * ToolbarWidth);
        using (new GUIScrollScope(tableListRect, ref _tableListScrollPosition, tableListContentRect, false))
        {
            Rect tableButtonRect = tableListContentRect with { height = ToolbarWidth };
            int tablesCount = _tables.Count;
            for (int i = 0; i < tablesCount; i++)
            {
                TableRecord tableRecord = _tables[i];
                tableRecord.Draw(tableButtonRect);
                tableButtonRect.y = tableButtonRect.yMax;
            }
        }

        // Table
        _activeTable?.TableWidget.Draw(tableRect);

        Text.WordWrap = wordWrap;
    }

    private void DrawOpenTableButton(Rect rect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            rect
                .HighlightLight()
                .DrawBorderBottom(BorderColor)
                .ContractedBy(IconPadding)
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
        Event @event = Event.current;

        if (@event.type == EventType.Repaint)
        {
            Texture2D texture = IsExpanded ? TexButton.ReorderDown : TexButton.ReorderUp;
            rect
                .HighlightLight()
                .DrawBorderBottom(BorderColor)
                .DrawTextureFitted(texture, 0.7f)
                .Tip(_expandButtonTooltip);
        }
        else if (@event.type == EventType.MouseDown && Mouse.IsOver(rect))
        {
            if (@event.clickCount > 1)
            {
                ExpandOrReset();
            }
            else
            {
                _isResized = true;
            }
        }
        else if (_isResized)
        {
            DoResize(rect.height);
        }

        // If the mouse cursor goes outside the window during MouseDrag/Up events (haven't tested the other ones),
        // the window stops recieving input events. Having a GUI control (which ButtonGhostly uses) drawn after
        // the code that checks for the aforementioned events, fixes the issue. And it just so happens that we need one here.
        rect.ButtonGhostly();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void DoResize(float rectHeight)
    {
        Event @event = Event.current;

        // Something eats the MouseUp event (but not the MouseDrag).
        // So we can use "type/rawType" for MouseDrag event.
        // But we must use "rawType" for MouseUp event.
        if (@event.type == EventType.MouseDrag)
        {
            windowRect.yMin = UI.GUIToScreenPoint(@event.mousePosition).y - rectHeight / 2f;
        }
        else if (@event.rawType == EventType.MouseUp)
        {
            _isResized = false;

            if (windowRect.yMin < 0f)
            {
                Expand();
            }

            GUIUtils.ReleaseMouseControl();
            @event.Use();
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

    private void ExpandOrReset()
    {
        if (IsExpanded)
        {
            Reset();
        }
        else
        {
            Expand();
        }
    }

    private void Expand()
    {
        windowRect.yMin = 0f;
    }

    private void Reset()
    {
        windowRect.height = _defaultHeight;
        SetInitialSizeAndPosition();
    }

    public override void PostClose()
    {
        _isResized = false;
        GUIUtils.ReleaseMouseControl();
        _activeTable?.TableWidget.NotifyParentWindowClosed();

        base.PostClose();
    }
}
