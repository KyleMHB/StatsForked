using System;
using System.Linq;
using Stats.Widgets;
using Stats.Widgets.Extensions;
using UnityEngine;
using Verse;

namespace Stats.MainTabWindow;

internal sealed class TableSelector : Widget
{
    private readonly Widget Widget;
    public TableDef TableDef
    {
        set
        {
            IconWidget.Texture = value.Icon;
            IconColorExtension.Color = value.IconColor;
            LabelWidget.Text = value.LabelCap;
        }
    }
    private readonly FloatMenu Menu;
    private readonly Icon IconWidget;
    private readonly ColorWidgetExtension IconColorExtension;
    private readonly Label LabelWidget;
    public event Action<TableDef>? OnTableSelect;
    public TableSelector(TableDef tableDef)
    {
        Widget = new HorizontalContainer(
            [
                new Icon(tableDef.Icon)
                    .Ref(out IconWidget)
                    .PaddingAbs(Globals.GUI.PadXs)
                    .SizeAbs(MainTabWindowTitleBar.Height)
                    .Color(tableDef.IconColor)
                    .Ref(out IconColorExtension),
                new Label(tableDef.LabelCap)
                    .Ref(out LabelWidget)
                    .HeightAbs(MainTabWindowTitleBar.Height)
                    .TextAnchor(TextAnchor.MiddleLeft),
            ],
            Globals.GUI.Pad
        )
        .PaddingAbs(Globals.GUI.Pad, 0f)
        .Background(Verse.Widgets.LightHighlight, TexUI.HighlightTex)
        .OnClick(ShowMenu);
        Widget.Parent = this;

        var menuOptions =
            DefDatabase<TableDef>
            .AllDefs
            .Select(tableDef => new FloatMenuOption(
                tableDef.LabelCap,
                () => OnTableSelect?.Invoke(tableDef),
                tableDef.Icon,
                tableDef.IconColor
            ))
            .OrderBy(menuOption => menuOption.Label)
            .ToList();
        Menu = new(menuOptions);
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);
    }
    private void ShowMenu()
    {
        Find.WindowStack.Add(Menu);
    }
}
