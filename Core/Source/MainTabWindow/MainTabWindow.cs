using System.Collections.Generic;
using RimWorld;
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
    private Vector2 _tableListScrollPosition;
    private readonly float _defaultHeight;
    private bool _isResized;
    private float _resizeYOffset;
    private float _yMax;

    public MainTabWindow()
    {
        _defaultHeight = base.RequestedTabSize.y;
        windowRect.height = _defaultHeight;
        List<TableDef> tableDefs = DefDatabase<TableDef>.AllDefsListForReading;
        int tableDefsCount = tableDefs.Count;
        _tables = new(tableDefsCount);
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
            .CutTop(out Rect openTableButtonRect, ToolbarWidth)
            .TakeRest(out Rect tableListRect);
        tableRect.CutTop(out Rect expandButtonRect, GUIStyles.TableToolbar.Height);

        // Border
        if (@event.type == EventType.Repaint)
        {
            toolbarRect.DrawBorderRight(BorderColor);
        }

        // Buttons
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

        DoResizeControl(expandButtonRect);
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

    private void DoResizeControl(Rect rect)
    {
        Event @event = Event.current;

        if (@event is { type: EventType.MouseDown, button: 0, modifiers: EventModifiers.None } && Mouse.IsOver(rect))
        {
            if (@event.clickCount > 1)
            {
                ResetSize();
            }
            else
            {
                _isResized = true;
                _resizeYOffset = @event.mousePosition.y;
            }
        }
        else if (_isResized)
        {
            if (OriginalEventUtility.EventType == EventType.MouseDrag)
            {
                float y = UI.GUIToScreenPoint(@event.mousePosition).y - _resizeYOffset;
                windowRect.yMin = Mathf.Clamp(y, 0f, _yMax);
                @event.Use();
            }
            else if (@event.rawType == EventType.MouseUp)
            {
                _isResized = false;
                GUIUtils.ReleaseMouseControl();
                @event.Use();
            }
        }

        GUI.Button(rect, GUIContent.none, GUIStyle.none);
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

    private void ResetSize()
    {
        windowRect.height = _defaultHeight;
        SetInitialSizeAndPosition();
    }

    public override void PostOpen()
    {
        _yMax = UI.screenHeight - MainButtonDef.ButtonHeight - GUIStyles.TableToolbar.Height;
        base.PostOpen();
    }

    public override void PostClose()
    {
        _isResized = false;
        _activeTable?.TableWidget.NotifyParentWindowClosed();

        base.PostClose();
    }
}
