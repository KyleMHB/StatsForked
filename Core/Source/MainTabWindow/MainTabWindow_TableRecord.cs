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
        public string DefName { get; }

        public string CustomName { get; private set; }
        public string Label => CustomName.Length > 0 ? CustomName : _defaultLabel;
        private readonly string _defaultLabel;
        private readonly string _description;
        private readonly Texture2D _icon;
        private readonly Color _iconColor;
        private readonly float _iconScale;
        private readonly MainTabWindow _parent;
        private readonly FloatMenu _menu;

        public TableRecord(TableDef tableDef, MainTabWindow parent, string? customName = null)
        {
            DefName = tableDef.defName;
            TableWidget = tableDef.Worker.TableWidget;
            TableWidget.ApplyDefaultPreset();
            _defaultLabel = tableDef.LabelCap;
            _description = tableDef.description ?? "";
            CustomName = TableName.Normalize(customName);
            _icon = tableDef.Icon;
            _iconColor = tableDef.iconColor;
            _iconScale = tableDef.iconScale;
            _parent = parent;
            List<FloatMenuOption> menuOptions = [
                new FloatMenuOption(Localization.Get(Localization.Rename), () =>
                    Find.WindowStack.Add(new TableNameWindow(CustomName, Rename))),
                // TODO: Although this works (for some reason), we can't just go and modify the collection we are iterating.
                new FloatMenuOption(Localization.Get(Localization.Remove), () => parent.RemoveTable(this))
            ];
            _menu = new FloatMenu(menuOptions);
        }

        private void Rename(string name)
        {
            CustomName = TableName.Normalize(name);
            _parent.SaveTableSession();
        }

        public void Draw(Rect rect)
        {
            Event @event = Event.current;

            if (@event.type == EventType.Repaint)
            {
                if (_parent._activeTable == this)
                {
                    rect.HighlightSelected();
                }

                rect
                    .Tip(_description.Length > 0 ? $"{Label}\n\n{_description}" : Label)
                    .ContractedBy(GUIStyles.MainTabWindow.IconPadding)
                    .DrawTextureFitted(_icon, _iconColor, _iconScale);
            }

            if (rect.ButtonGhostly() && @event.modifiers == EventModifiers.None)
            {
                if (@event.button == 0)
                {
                    _parent.SetActiveTable(this);
                }
                else if (@event.button == 1)
                {
                    _menu.Open();
                }
            }
        }
    }
}
