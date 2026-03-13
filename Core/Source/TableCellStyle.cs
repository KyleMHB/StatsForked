using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats;

public static class TableCellStyle
{
    public static readonly GUIStyle String;
    public static readonly GUIStyle Number;
    public static readonly GUIStyle Boolean;
    public const float CellContentSpacing = GUIUtils.PadSm;

    internal const float CellPadHor = CellPadHorInt;
    internal const float CellPadVer = CellPadVerInt;
    internal const int CellPadHorInt = 12;
    internal const int CellPadVerInt = 4;

    static TableCellStyle()
    {
        GUIStyle baseStyle = new(Text.fontStyles[1])
        {
            wordWrap = false,
            padding = new RectOffset(CellPadHorInt, CellPadHorInt, CellPadVerInt, CellPadVerInt)
        };
        String = new GUIStyle(baseStyle)
        {
            alignment = (TextAnchor)TableCellStyleType.String
        };
        Number = new GUIStyle(baseStyle)
        {
            alignment = (TextAnchor)TableCellStyleType.Number
        };
        Boolean = new GUIStyle(baseStyle)
        {
            alignment = (TextAnchor)TableCellStyleType.Boolean
        };
    }
}
