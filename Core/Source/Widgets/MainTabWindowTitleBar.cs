using System;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

[StaticConstructorOnStartup]
internal sealed class MainTabWindowTitleBar : WidgetWrapper
{
    protected override Widget Widget { get; }
    private static readonly Texture2D ExpandWindowTex;
    private static readonly Texture2D TableSettingsTex;
    private static readonly Texture2D FilterTex;
    private const string Manual =
        "- Hold (LMB) and move mouse cursor to scroll horizontally.\n" +
        "- Hold [Ctrl] and click on a column's name to pin/unpin it.\n" +
        "- Hold [Ctrl] and click on a row to pin/unpin it.\n" +
        "  - You can pin multiple rows.\n" +
        "  - Pinned rows are unaffected by filters.";
    internal const float Height = 30f;
    private const float IconPadding = Globals.GUI.PadXs;
    private readonly Label TableFilterModeLabelWidget;
    private ObjectTable _TableWidget;
    public ObjectTable TableWidget
    {
        set
        {
            if (value == _TableWidget)
            {
                return;
            }

            _TableWidget.OnFilterModeChange -= HandleTableFilterModeChange;
            _TableWidget = value;
            value.OnFilterModeChange += HandleTableFilterModeChange;
            TableFilterModeLabelWidget.Text = value.FilterMode.ToString();
        }
    }
    public bool ShowTableSettingsMenu { get; set; } = false;
    public MainTabWindowTitleBar(
        ObjectTable tableWidget,
        Widget tableSelector,
        Action expandOrResetWindow,
        Action resetTableFilters
    )
    {
        _TableWidget = tableWidget;
        tableWidget.OnFilterModeChange += HandleTableFilterModeChange;
        Widget = new HorizontalContainer([
            new HorizontalContainer([
                tableSelector,

                ToToolbarButton(
                    new Icon(TableSettingsTex),
                    new Label("Filters / columns"),
                    () => ShowTableSettingsMenu = !ShowTableSettingsMenu
                ),

                ToToolbarButton(
                    new Icon(FilterTex),
                    new Label(tableWidget.FilterMode.ToString())
                    .Ref(out TableFilterModeLabelWidget),
                    () => _TableWidget.ToggleFilterMode()
                )
                .Tooltip(
                    "Click to switch between filtering modes.\n\n" +
                    "This setting is individual for each table."
                ),

                ToToolbarButton(
                    new Icon(TexUI.RotRightTex),
                    new Label("Reset filters"),
                    resetTableFilters
                ),
            ], Globals.GUI.PadSm, true)
            .WidthRel(1f),

            ToToolbarButtonIcon(
                new Icon(TexButton.Info),
                Manual
            ),

            ToToolbarButtonIcon(
                new Icon(ExpandWindowTex),
                expandOrResetWindow,
                "Expand/Reset"
            ),
        ], Globals.GUI.PadSm, true)
        .Background(Verse.Widgets.LightHighlight)
        .BorderBottom(1f, MainTabWindow.BorderLineColor);
        Widget.Parent = this;
    }
    private static Widget ToToolbarButtonIcon(
        Widget widget,
        Action clickEventHandler,
        string tooltip
    )
    {
        return ToToolbarButtonIcon(widget, tooltip)
            .ToButtonGhostly(clickEventHandler);
    }
    private static Widget ToToolbarButtonIcon(
        Widget widget,
        string tooltip
    )
    {
        return widget
            .PaddingAbs(IconPadding)
            .SizeAbs(Height)
            .Tooltip(tooltip);
    }
    private static Widget ToToolbarButton(
        Widget iconWidget,
        Widget textWidget,
        Action clickEventHandler
    )
    {
        return new HorizontalContainer([
            iconWidget
            .PaddingAbs(IconPadding)
            .SizeAbs(Height),

            textWidget
            .HeightAbs(Height)
            .TextAnchor(TextAnchor.MiddleCenter)
        ], Globals.GUI.PadXs)
        .PaddingAbs(Globals.GUI.PadXs, 0f)
        .HeightAbs(Height)
        .ToButtonGhostly(clickEventHandler);
    }
    private void HandleTableFilterModeChange(ObjectTable.TableFilterMode filterMode)
    {
        TableFilterModeLabelWidget.Text = filterMode.ToString();
    }

    static MainTabWindowTitleBar()
    {
        ExpandWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ExpandWindow");
        TableSettingsTex = ContentFinder<Texture2D>.Get("UI/Icons/Options/OptionsGeneral");
        FilterTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/Filter");
    }
}
