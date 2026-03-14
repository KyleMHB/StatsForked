using UnityEngine;
using Verse;

namespace Stats;

public static class GUIStyles
{
    internal static class Global
    {
        internal const float Pad = 10f;
        internal const float PadSm = 5f;
        internal const float PadXs = 3f;
        internal const float EstimatedInputFieldInnerPadding = 2f;
        internal const float ButtonSubtleContentHoverOffset = 2f;
    }

    internal static class MainTabWindow
    {
        internal const float ToolbarWidth = 40f;
        internal const float IconPadding = 5f;
        internal static readonly Color BorderColor = new(1f, 1f, 1f, 0.4f);
    }

    public static class Text
    {
        public static readonly Color ColorHighlight = new(1f, 0.98f, 0.62f);
        public static readonly Color ColorSecondary = Color.grey;
    }

    internal static class Table
    {
        internal static readonly Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
        internal static readonly Color PinnedRowsBGColor = Verse.Widgets.HighlightStrongBgColor.ToTransparent(0.1f);
    }

    public static class TableCell
    {
        public static readonly GUIStyle String;
        public static readonly GUIStyle StringNoPad;
        public static readonly GUIStyle Number;
        public static readonly GUIStyle NumberNoPad;
        public static readonly GUIStyle Boolean;
        public static readonly GUIStyle BooleanNoPad;
        public const float ContentSpacing = Global.PadSm;
        internal const float PadHor = _PadHor;
        internal const float PadVer = _PadVer;

        private const int _PadHor = 12;
        private const int _PadVer = 4;

        static TableCell()
        {
            GUIStyle baseStyle = new(Verse.Text.fontStyles[1]);
            baseStyle.wordWrap = false;
            RectOffset padding = new(_PadHor, _PadHor, _PadVer, _PadVer);

            StringNoPad = new GUIStyle(baseStyle);
            StringNoPad.alignment = TextAnchor.MiddleLeft;
            String = new GUIStyle(StringNoPad);
            String.padding = padding;

            NumberNoPad = new GUIStyle(baseStyle);
            NumberNoPad.alignment = TextAnchor.MiddleRight;
            Number = new GUIStyle(NumberNoPad);
            Number.padding = padding;

            BooleanNoPad = new GUIStyle(baseStyle);
            BooleanNoPad.alignment = TextAnchor.MiddleCenter;
            Boolean = new GUIStyle(BooleanNoPad);
            Boolean.padding = padding;
        }
    }
}
