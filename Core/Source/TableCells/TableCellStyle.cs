using UnityEngine;
using Verse;

namespace Stats.TableCells;

public static class TableCellStyle
{
    public static readonly GUIStyle String;
    public static readonly GUIStyle Number;
    public static readonly GUIStyle Boolean;

    static TableCellStyle()
    {
        GUIStyle baseStyle = new(Text.fontStyles[1])
        {
            wordWrap = false,
            padding = new RectOffset(ObjectTable.CellPadHorInt, ObjectTable.CellPadHorInt, ObjectTable.CellPadVerInt, ObjectTable.CellPadVerInt)
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
