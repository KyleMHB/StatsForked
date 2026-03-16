using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;
using ButtonStyle = Stats.GUIStyles.TableToolbarButton;
using Style = Stats.GUIStyles.TableToolbar;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    private sealed class Toolbar
    {
        private readonly ObjectTable<TObject> _parent;
        private readonly ToolbarButton _filtersButton;
        private readonly ToolbarButton _addColumnButton;
        private readonly ToolbarButton _columnPresetButton;
        private FloatMenu ColumnsMenu => field ??= MakeColumnsMenu();

        public Toolbar(ObjectTable<TObject> parent)
        {
            _parent = parent;
            _filtersButton = new ToolbarButton(Assets.FilterTex, "Filters", 0.7f);
            _addColumnButton = new ToolbarButton(Verse.TexButton.Add, "Add Column");
            // TODO: Do we really need this feature if plan to save opened tables?
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

            bool filterButtonWasClicked = _filtersButton.Draw(rect.CutByX(_filtersButton.Width));
            if (filterButtonWasClicked) _parent.ToggleFiltersTab();

            rect.xMin += Style.Gap;

            bool addColumnButtonWasClicked = _addColumnButton.Draw(rect.CutByX(_addColumnButton.Width));
            if (addColumnButtonWasClicked) ColumnsMenu.Open();

            rect.xMin += Style.Gap;

            _columnPresetButton.Draw(rect.CutByX(_columnPresetButton.Width));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private FloatMenu MakeColumnsMenu()
        {
            List<ColumnDef> compatibleColumns = _parent._tableWorker.CompatibleColumns;
            int compatibleColumnsCount = compatibleColumns.Count;
            List<FloatMenuOption> columnsMenuOptions = new(compatibleColumnsCount);
            for (int i = 0; i < compatibleColumnsCount; i++)
            {
                ColumnDef columnDef = compatibleColumns[i];
                MyFloatMenuOption menuOption = new(
                    columnDef.LabelCap,
                    null,
                    Verse.Widgets.CheckboxOnTex,
                    _parent._columns.Find(column => column.Worker.Def == columnDef) == null ? Color.white with { a = 0f } : Color.white
                );
                menuOption.tooltip = columnDef.description;
                menuOption.action = () =>
                {
                    if (_parent._columns.Find(column => column.Worker.Def == columnDef) != null)
                    {
                        _parent.RemoveColumn(columnDef);
                        menuOption.iconColor.a = 0f;
                    }
                    else
                    {
                        _parent.AddColumn(columnDef);
                        menuOption.iconColor.a = 1f;
                    }
                };
                if (columnDef.title != null)
                {
                    menuOption.extraPartOnGUI = rect =>
                    {
                        rect = rect.ContractedBy(0f, (rect.height - columnDef.TitleWidget.Size.y) / 2f);
                        columnDef.TitleWidget.Draw(rect);
                        return false;
                    };
                    menuOption.extraPartWidth = columnDef.TitleWidget.Size.x + GUIStyles.Global.PadSm;
                    menuOption.extraPartRightJustified = true;
                }
                columnsMenuOptions.Add(menuOption);
            }
            columnsMenuOptions.SortBy(option => option.Label);

            return new FloatMenu(columnsMenuOptions);
        }

        private sealed class MyFloatMenuOption : FloatMenuOption
        {
            public MyFloatMenuOption(string label, Action? action, Texture2D? iconTex, Color iconColor)
                : base(label, action, iconTex, iconColor) { }

            public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
            {
                bool wordWrap = Verse.Text.WordWrap;
                Verse.Text.WordWrap = false;

                bool flag = base.DoGUI(rect, colonistOrdering, floatMenu);

                Verse.Text.WordWrap = wordWrap;

                return flag;
            }
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
            float labelWidth = label.CalcSize(ButtonStyle.LabelStyle).x;
            Width = ButtonStyle.PadHor + ButtonStyle.IconWidth + labelWidth;
        }

        public bool Draw(Rect rect)
        {
            if (Event.current.IsRepaint())
            {
                Rect contentRect = rect;

                contentRect.xMin += ButtonStyle.PadHor;

                contentRect
                    .CutByX(ButtonStyle.IconWidth)
                    .DrawTextureFitted(_icon, _iconScale);

                contentRect.Label(_label, ButtonStyle.LabelStyle);
            }

            return rect.ButtonGhostly();
        }
    }
}
