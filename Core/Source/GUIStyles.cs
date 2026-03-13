using UnityEngine;

namespace Stats;

public static class GUIStyles
{
    public static class Global
    {
        internal const float Pad = 10f;
        internal const float PadSm = 5f;
        internal const float PadXs = 3f;
        internal const float EstimatedInputFieldInnerPadding = 2f;
        internal const float ButtonSubtleContentHoverOffset = 2f;
    }

    public static class Text
    {
        public static readonly Color ColorHighlight = new(1f, 0.98f, 0.62f);
        public static readonly Color ColorSecondary = Color.grey;
    }

    public static class TableCell
    {
        public static readonly GUIStyle String;
        public static readonly GUIStyle Number;
        public static readonly GUIStyle Boolean;
        public const float ContentSpacing = Global.PadSm;
        internal const float PadHor = _PadHor;
        internal const float PadVer = _PadVer;

        private const int _PadHor = 12;
        private const int _PadVer = 4;

        static TableCell()
        {
            GUIStyle baseStyle = new(Verse.Text.fontStyles[1])
            {
                wordWrap = false,
                padding = new RectOffset(_PadHor, _PadHor, _PadVer, _PadVer)
            };
            String = new GUIStyle(baseStyle)
            {
                alignment = TextAnchor.MiddleLeft
            };
            Number = new GUIStyle(baseStyle)
            {
                alignment = TextAnchor.MiddleRight
            };
            Boolean = new GUIStyle(baseStyle)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}
