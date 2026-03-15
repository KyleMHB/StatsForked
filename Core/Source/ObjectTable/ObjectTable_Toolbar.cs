using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    private sealed class Toolbar
    {
        private readonly ObjectTable<TObject> _parent;
        private readonly ToolbarButton _filtersButton;
        private readonly ToolbarButton _addColumnButton;
        private readonly ToolbarButton _columnPresetButton;

        public Toolbar(ObjectTable<TObject> parent)
        {
            _parent = parent;
            _filtersButton = new ToolbarButton(Assets.FilterTex, "Filters", 0.7f);
            _addColumnButton = new ToolbarButton(Verse.TexButton.Add, "Add Column");
            _columnPresetButton = new ToolbarButton(Verse.TexButton.Paste, "Apply Preset", 0.8f);
        }

        public void Draw(Rect rect)
        {
            if (Event.current.IsRepaint())
            {
                rect
                    .HighlightLight()
                    .DrawBorderBottom(GUIStyles.MainTabWindow.BorderColor);
            }

            _filtersButton.Draw(rect.CutByX(_filtersButton.Width));

            rect.xMin += GUIStyles.TableToolbar.Gap;

            _addColumnButton.Draw(rect.CutByX(_addColumnButton.Width));

            rect.xMin += GUIStyles.TableToolbar.Gap;

            _columnPresetButton.Draw(rect.CutByX(_columnPresetButton.Width));
        }
    }

    private sealed class ToolbarButton
    {
        public readonly float Width;

        private readonly Texture2D _icon;
        private readonly float _iconScale;
        private readonly string _label;

        public ToolbarButton(Texture2D icon, string label, float iconScale = 1f)
        {
            _icon = icon;
            _iconScale = iconScale;
            _label = label;
            float labelWidth = label.CalcSize(GUIStyles.TableToolbarButton.LabelStyle).x;
            Width = GUIStyles.TableToolbarButton.PadHor + GUIStyles.TableToolbarButton.IconWidth + labelWidth;
        }

        public bool Draw(Rect rect)
        {
            if (Event.current.IsRepaint())
            {
                Rect contentRect = rect;

                contentRect.xMin += GUIStyles.TableToolbarButton.PadHor;

                contentRect
                    .CutByX(GUIStyles.TableToolbarButton.IconWidth)
                    .DrawTextureFitted(_icon, _iconScale);

                contentRect.Label(_label, GUIStyles.TableToolbarButton.LabelStyle);
            }

            return rect.ButtonGhostly();
        }
    }

    //private sealed class ToggleFiltersTabToolBarButton
    //{
    //    public void Draw(Rect rect)
    //    {

    //    }
    //}
}
