using System.Collections.Generic;
using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;

namespace Stats;

public sealed partial class MainTabWindow
{
    private sealed class TableRecord
    {
        public readonly ObjectTable TableWidget;

        private readonly TipSignal _tooltip;
        private readonly Texture2D _icon;
        private readonly Color _iconColor;
        private readonly float _iconScale;
        private readonly MainTabWindow _parent;
        private readonly FloatMenu _menu;

        public TableRecord(TableDef tableDef, MainTabWindow parent)
        {
            TableWidget = tableDef.Worker.TableWidget;
            _tooltip = tableDef.LabelCap;
            if (tableDef.description?.Length > 0)
            {
                _tooltip += $"\n\n{tableDef.description}";
            }
            _icon = tableDef.Icon;
            _iconColor = tableDef.iconColor;
            _iconScale = tableDef.iconScale;
            _parent = parent;
            List<FloatMenuOption> menuOptions = [
                // TODO: Although this works (for some reason), we can't just go and modify the collection we are iterating.
                new FloatMenuOption("Remove", () => parent.RemoveTable(this))
            ];
            _menu = new FloatMenu(menuOptions);
        }

        public void Draw(Rect rect)
        {
            if (Event.current.IsRepaint())
            {
                if (_parent._activeTable == this)
                {
                    rect.HighlightSelected();
                }

                rect
                    .Tip(_tooltip)
                    .ContractedBy(GUIStyles.MainTabWindow.IconPadding)
                    .DrawTextureFitted(_icon, _iconColor, _iconScale);
            }

            if (rect.ButtonGhostly())
            {
                if (Event.current.IsLMB())
                {
                    _parent._activeTable = this;
                }
                else if (Event.current.IsRMB())
                {
                    _menu.Open();
                }
            }
        }
    }
}
