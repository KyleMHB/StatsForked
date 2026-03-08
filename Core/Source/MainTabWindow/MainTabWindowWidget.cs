using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.MainTabWindow;

public sealed class MainTabWindowWidget : RimWorld.MainTabWindow
{
    public override Vector2 RequestedTabSize => new(UI.screenWidth, base.RequestedTabSize.y);
    internal static readonly Color BorderLineColor = new(1f, 1f, 1f, 0.4f);

    protected override float Margin { get => 1f; }
    private bool IsExpanded;
    private readonly Widget TitleBarWidget;
    private readonly MainTabWindowTitleBar TitleBar;
    private readonly TableSelector TableSelector;
    private TableDef _CurTableDef;
    private TableDef CurTableDef
    {
        set
        {
            if (_CurTableDef == value)
            {
                return;
            }

            _CurTableDef = value;
            TableSelector.TableDef = value;
            TitleBar.TableWidget = value.Worker.TableWidget;
        }
    }

    public MainTabWindowWidget()
    {
        // TODO: All of this TableDef/ITableWidget juggling,
        // can probably be replaced with a single stream of TableDefs.
        _CurTableDef = TableDefOf.RangedWeapons;
        TableSelector = new TableSelector(_CurTableDef);
        TableSelector.OnTableSelect += tableDef => CurTableDef = tableDef;
        TitleBar = new MainTabWindowTitleBar(
            _CurTableDef.Worker.TableWidget,
            TableSelector,
            ExpandOrResetWidow,
            ResetCurrentTableFilters
        );
        TitleBarWidget = TitleBar.WidthRel(1f);
    }

    public override void DoWindowContents(Rect rect)
    {
        var rectSize = rect.size;
        var titleBarHeight = TitleBarWidget.GetSize(rectSize).y;

        bool wordWrap = Text.WordWrap;
        Text.WordWrap = false;

        TitleBarWidget.Draw(rect.TopPartPixels(titleBarHeight), rectSize);

        _CurTableDef.Worker.TableWidget.Draw(
            rect.BottomPartPixels(rect.height - titleBarHeight),
            TitleBar.ShowTableSettingsMenu
        );

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

    private void ResetCurrentTableFilters()
    {
        //_CurTableDef.Worker.TableWidget.ResetFilters();
    }
}
