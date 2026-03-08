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
        String = new GUIStyle(Text.fontStyles[1]);
        String.alignment = (TextAnchor)TableCellStyleType.String;
        String.wordWrap = false;
        Number = new GUIStyle(Text.fontStyles[1]);
        Number.alignment = (TextAnchor)TableCellStyleType.Number;
        Number.wordWrap = false;
        Boolean = new GUIStyle(Text.fontStyles[1]);
        Boolean.alignment = (TextAnchor)TableCellStyleType.Boolean;
        Boolean.wordWrap = false;
    }
}
