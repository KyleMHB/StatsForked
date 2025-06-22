using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public static class ColumnLabelFormat
{
    private static readonly Texture2D ArmorIcon;
    private static readonly Texture2D DamageToIcon;
    private static readonly Texture2D NutritionIcon;
    private static readonly Texture2D IntervalIcon;
    private static readonly Texture2D TimeIcon;
    private static readonly Texture2D ResistanceIcon;
    static ColumnLabelFormat()
    {
        ArmorIcon = ContentFinder<Texture2D>.Get("Things/Pawn/Humanlike/Apparel/FlakVest/FlakVest");
        DamageToIcon = ContentFinder<Texture2D>.Get("UI/Commands/FireAtWill");
        NutritionIcon = ContentFinder<Texture2D>.Get("Things/Mote/ThoughtSymbol/Food");
        IntervalIcon = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/Interval");
        TimeIcon = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/Time");
        ResistanceIcon = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/Resistance");
    }
    public static Widget LabelOnly(ColumnDef columnDef, ColumnCellStyle cellStyle)
    {
        return new Label(columnDef.LabelShort);
    }
    public static Widget IconOnly(ColumnDef columnDef, ColumnCellStyle cellStyle)
    {
        var icon = new InlineTexture(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor);

        return cellStyle switch
        {
            ColumnCellStyle.Number => new SingleElementContainer(icon.PaddingRel(1f, 0f, 0f, 0f)),
            ColumnCellStyle.Boolean => new SingleElementContainer(icon.PaddingRel(0.5f, 0f)),
            _ => icon
        };
    }
    public static Widget LabelWithIcon(ColumnDef columnDef, ColumnCellStyle cellStyle)
    {
        var label = new Label(columnDef.LabelShort);
        var icon = new InlineTexture(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor);

        return cellStyle switch
        {
            ColumnCellStyle.Number => new HorizontalContainer(
                [icon.PaddingRel(1f, 0f, 0f, 0f), label],
                Globals.GUI.PadXs,
                true
            ),
            ColumnCellStyle.Boolean => new SingleElementContainer(
                new HorizontalContainer([icon, label], Globals.GUI.PadXs).PaddingRel(0.5f, 0f)
            ),
            _ => new HorizontalContainer([icon, label], Globals.GUI.PadXs),
        };
    }
    public static Widget ArmorRating(ColumnDef columnDef, ColumnCellStyle _)
    {
        return new HorizontalContainer(
            [
                new InlineTexture(ArmorIcon).Color(new(0.4f,0.4f,0.4f)).PaddingRel(1f, 0f, 0f, 0f),
                new InlineTexture(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor)
            ],
            Globals.GUI.PadXs,
            true
        );
    }
    public static Widget DamageFactorTo(ColumnDef columnDef, ColumnCellStyle _)
    {
        return new HorizontalContainer(
            [
                new InlineTexture(DamageToIcon, 1.2f).PaddingRel(1f, 0f, 0f, 0f),
                new Label("%"),
                new InlineTexture(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor)
            ],
            Globals.GUI.PadXs,
            true
        );
    }
    public static Widget NutritionAmount(ColumnDef columnDef, ColumnCellStyle _)
    {
        var widgets = new List<Widget>([
            new InlineTexture(columnDef.Icon!, columnDef.IconScale)
                .Color(columnDef.IconColor)
                .PaddingRel(1f, 0f, 0f, 0f),
            new InlineTexture(NutritionIcon)
        ]);

        return new HorizontalContainer(widgets, Globals.GUI.PadXs, true);
    }
    public static Widget NutritionAmountPerDay(ColumnDef columnDef, ColumnCellStyle _)
    {
        var widgets = new List<Widget>([
            new InlineTexture(columnDef.Icon!, columnDef.IconScale)
                .Color(columnDef.IconColor)
                .PaddingRel(1f, 0f, 0f, 0f),
            new InlineTexture(NutritionIcon),
            new Label("/d"),
        ]);

        return new HorizontalContainer(widgets, Globals.GUI.PadXs, true);
    }
    public static Widget Interval(ColumnDef columnDef, ColumnCellStyle _)
    {
        var widgets = new List<Widget>([
            new InlineTexture(columnDef.Icon!, columnDef.IconScale)
                .Color(columnDef.IconColor)
                .PaddingRel(1f, 0f, 0f, 0f),
            new InlineTexture(IntervalIcon),
        ]);

        return new HorizontalContainer(widgets, Globals.GUI.PadXs, true);
    }
    public static Widget Time(ColumnDef columnDef, ColumnCellStyle _)
    {
        var widgets = new List<Widget>([
            new Label(columnDef.labelShort).PaddingRel(1f, 0f, 0f, 0f),
            new InlineTexture(TimeIcon),
        ]);

        return new HorizontalContainer(widgets, Globals.GUI.PadXs, true);
    }
    public static Widget Resistance(ColumnDef columnDef, ColumnCellStyle _)
    {
        return new HorizontalContainer(
            [
                new InlineTexture(columnDef.Icon!, columnDef.IconScale)
                    .Color(columnDef.IconColor)
                    .PaddingRel(1f, 0f, 0f, 0f),
                new InlineTexture(ResistanceIcon)
            ],
            Globals.GUI.PadXs,
            true
        );
    }
}
