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
        private readonly ToolbarButton _columnsMenuButton;
        private readonly ToolbarButton _columnPresetsButton;
        private ColumnsFloatMenu ColumnsMenu => field ??= MakeColumnsMenu();

        public Toolbar(ObjectTable<TObject> parent)
        {
            _parent = parent;
            _filtersButton = new ToolbarButton(Assets.TableFiltersTabIcon, "Filters", 0.7f);
            _columnsMenuButton = new ToolbarButton(Assets.TableColumnsMenuIcon, "Columns", 0.8f);
            // TODO: Do we really need this feature if plan to save opened tables?
            _columnPresetsButton = new ToolbarButton(Verse.TexButton.Paste, "Apply Preset", 0.8f);
        }

        public void NotifyColumnAdded(Column column)
        {
            ColumnsMenu.NotifyColumnAdded(column);
        }

        public void NotifyColumnRemoved(Column column)
        {
            ColumnsMenu.NotifyColumnRemoved(column);
        }

        public void Draw(Rect rect)
        {
            // Layout
            rect
                .CutLeft(out Rect filtersTabButtonRect, _filtersButton.Width)
                .SkipLeft(Style.Gap)
                .CutLeft(out Rect columnsMenuButtonRect, _columnsMenuButton.Width)
                .SkipLeft(Style.Gap)
                .CutLeft(out Rect columnPresetsButtonRect, _columnPresetsButton.Width);

            // Background
            if (Event.current.IsRepaint())
            {
                rect
                    .HighlightLight()
                    .DrawBorderBottom(GUIStyles.MainTabWindow.BorderColor);
            }

            // Buttons
            bool filtersTabButtonWasClicked = _filtersButton.Draw(filtersTabButtonRect);
            bool columnsMenuButtonWasClicked = _columnsMenuButton.Draw(columnsMenuButtonRect);
            _columnPresetsButton.Draw(columnPresetsButtonRect);

            // Events
            if (filtersTabButtonWasClicked)
            {
                _parent.ToggleFiltersTab();
            }
            else if (columnsMenuButtonWasClicked)
            {
                ColumnsMenu.Open();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private ColumnsFloatMenu MakeColumnsMenu()
        {
            List<ColumnDef> compatibleColumns = _parent._tableWorker.CompatibleColumns;
            int compatibleColumnsCount = compatibleColumns.Count;
            List<FloatMenuOption> columnsMenuOptions = new(compatibleColumnsCount);
            for (int i = 0; i < compatibleColumnsCount; i++)
            {
                ColumnDef columnDef = compatibleColumns[i];
                ColumnsFloatMenuOption menuOption = new(columnDef, _parent);
                columnsMenuOptions.Add(menuOption);
            }
            columnsMenuOptions.SortBy(option => option.Label);

            return new ColumnsFloatMenu(columnsMenuOptions);
        }

        private sealed class ColumnsFloatMenu : FloatMenu
        {
            public ColumnsFloatMenu(List<FloatMenuOption> options) : base(options) { }

            public void NotifyColumnAdded(Column column)
            {
                ColumnDef columnDef = column.Worker.Def;
                int optionsCount = options.Count;
                for (int i = 0; i < optionsCount; i++)
                {
                    ColumnsFloatMenuOption option = (ColumnsFloatMenuOption)options[i];
                    if (option.ColumnDef == columnDef)
                    {
                        option.Select();
                        break;
                    }
                }
            }

            public void NotifyColumnRemoved(Column column)
            {
                ColumnDef columnDef = column.Worker.Def;
                int optionsCount = options.Count;
                for (int i = 0; i < optionsCount; i++)
                {
                    ColumnsFloatMenuOption option = (ColumnsFloatMenuOption)options[i];
                    if (option.ColumnDef == columnDef)
                    {
                        option.Unselect();
                        break;
                    }
                }
            }
        }

        private sealed class ColumnsFloatMenuOption : FloatMenuOption
        {
            public readonly ColumnDef ColumnDef;

            public ColumnsFloatMenuOption(ColumnDef columnDef, ObjectTable<TObject> parent)
                : base(columnDef.LabelCap, null, Verse.Widgets.CheckboxOnTex, Color.white)
            {
                ColumnDef = columnDef;
                tooltip = columnDef.description;
                action = () =>
                {
                    if (parent._columns.Find(column => column.Worker.Def == columnDef) != null)
                    {
                        parent.RemoveColumn(columnDef);
                    }
                    else
                    {
                        parent.AddColumn(columnDef);
                    }
                };
                if (parent._columns.Find(column => column.Worker.Def == columnDef) == null)
                {
                    Unselect();
                }
                if (columnDef.title != null)
                {
                    extraPartOnGUI = rect =>
                    {
                        rect = rect.ContractedBy(0f, (rect.height - columnDef.TitleWidget.Size.y) / 2f);
                        columnDef.TitleWidget.Draw(rect);
                        return false;
                    };
                    extraPartWidth = columnDef.TitleWidget.Size.x + GUIStyles.Global.PadSm;
                    extraPartRightJustified = true;
                }
            }

            public void Select()
            {
                iconColor.a = 1f;
            }

            public void Unselect()
            {
                iconColor.a = 0f;
            }

            public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
            {
                bool wordWrap = Verse.Text.WordWrap;
                Verse.Text.WordWrap = false;

                base.DoGUI(rect, colonistOrdering, floatMenu);

                Verse.Text.WordWrap = wordWrap;

                return false;
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
                rect
                    .SkipLeft(ButtonStyle.PadHor)
                    .CutLeft(out Rect iconRect, ButtonStyle.IconWidth)
                    .TakeRest(out Rect labelRect);

                iconRect.DrawTextureFitted(_icon, _iconScale);
                labelRect.Label(_label, ButtonStyle.LabelStyle);
            }

            return rect.ButtonGhostly();
        }
    }
}
