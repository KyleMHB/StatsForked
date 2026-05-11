using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.Filters;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using Stats.Utils.Widgets;
using UnityEngine;
using Verse;
using FilterLabelWidget = Stats.Utils.Widgets.Widget;
using LegacyThingDefIcon = Stats.Widgets_Legacy.ThingDefIcon;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    private const string AvailableFilterKey = "__table_filter_available";
    private const string HasRecipeFilterKey = "__table_filter_has_recipe";
    private const string MaterialFilterKey = "__table_filter_material";
    private const string RecipeIngredientsFilterKey = "__table_filter_recipe_ingredients";
    private const string RecipeBenchFilterKey = "__table_filter_recipe_bench";
    private readonly List<FilterEntry> _filters = [];
    private FiltersWindow? _filtersWindow;
    private Filter? _availableFilter;
    private static readonly HashSet<ThingDef?> _emptyThingDefSet = [];

    private void ToggleFiltersTab()
    {
        if (_filtersWindow != null)
        {
            _filtersWindow.Close();
            return;
        }

        _filtersWindow = new FiltersWindow(this);
        _filtersWindow.Open();
    }

    private void RegisterColumnFilters(Column column, ICollection<CellField> fields)
    {
        foreach (CellField field in fields)
        {
            field.FilterWidget.OnChange += ApplyFilters;
            string labelText = field.Label is Label label ? label.Text : field.Label.GetType().Name;
            _filters.Add(new FilterEntry(column.Def.defName, column, field.Label, labelText, field.FilterWidget));
        }
    }

    private void RegisterTableFilters()
    {
        if (typeof(TObject) != typeof(DefBasedObject))
        {
            return;
        }

        BooleanFilter availableFilter = new(IsAvailable);
        availableFilter.OnChange += ApplyFilters;
        _availableFilter = availableFilter;
        _filters.Add(new FilterEntry(AvailableFilterKey, null, new Label("available"), "available", availableFilter));

        if (SupportsEquipmentRecipeFilters())
        {
            RegisterEquipmentRecipeFilters();
        }
    }

    private void UnregisterColumnFilters(Column column)
    {
        for (int i = _filters.Count - 1; i >= 0; i--)
        {
            FilterEntry filter = _filters[i];
            if (filter.Column != column)
            {
                continue;
            }

            filter.Widget.OnChange -= ApplyFilters;
            _filters.RemoveAt(i);
        }
    }

    private void ApplyFilters()
    {
        List<Filter> activeFilters = _filters
            .Select(filter => filter.Widget)
            .Where(filter => filter.IsActive)
            .ToList();

        _rows.Clear();
        for (int i = 0; i < _topRowsCount; i++)
        {
            _rows.Add(_rowOrder[i]);
        }

        if (activeFilters.Count == 0)
        {
            for (int i = _topRowsCount; i < _rowOrder.Count; i++)
            {
                _rows.Add(_rowOrder[i]);
            }
        }
        else
        {
            for (int i = _topRowsCount; i < _rowOrder.Count; i++)
            {
                int row = _rowOrder[i];
                bool matches = true;
                for (int j = 0; j < activeFilters.Count; j++)
                {
                    try
                    {
                        if (activeFilters[j].Eval(row) == false)
                        {
                            matches = false;
                            break;
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"Failed to evaluate filter on row {row}: {exception}");
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    _rows.Add(row);
                }
            }
        }

        _scrollPosition.y = 0f;
    }

    private void ResetFilters()
    {
        foreach (FilterEntry filter in _filters)
        {
            if (filter.Widget.IsActive)
            {
                filter.Widget.Reset();
            }
        }
    }

    private List<FilterPresetState> CaptureFilterPresetStates()
    {
        List<FilterPresetState> states = [];
        foreach (FilterEntry filter in _filters)
        {
            if (filter.Widget is IPresettableFilter presettableFilter == false || filter.Widget.IsActive == false)
            {
                continue;
            }

            states.Add(new FilterPresetState
            {
                columnDefName = filter.Key,
                label = filter.LabelText,
                state = presettableFilter.SerializeState(),
            });
        }

        return states;
    }

    private void ApplyFilterPresetStates(List<FilterPresetState> states)
    {
        ResetFilters();

        foreach (FilterPresetState state in states)
        {
            FilterEntry? matchingFilter = _filters.FirstOrDefault(filter =>
                filter.Key == state.columnDefName
                && filter.LabelText == state.label);

            if (matchingFilter is { } filterEntry && filterEntry.Widget is IPresettableFilter presettableFilter)
            {
                try
                {
                    presettableFilter.DeserializeState(state.state);
                }
                catch (Exception exception)
                {
                    Log.Error($"Failed to restore filter preset for column \"{state.columnDefName}\" and label \"{state.label}\": {exception}");
                }
            }
        }
    }

    private bool HasActiveLiveTableFilter()
    {
        return _availableFilter?.IsActive == true;
    }

    private bool IsAvailable(int row)
    {
        if (_objects[row] is not DefBasedObject { Def: ThingDef thingDef })
        {
            return false;
        }

        return InventoryStateTracker.IsOwnedByPlayer(thingDef);
    }

    private bool SupportsEquipmentRecipeFilters()
    {
        List<string> columnTags = _tableWorker.Def.columnTags;
        return columnTags.Contains("Thing_IsApparel")
            || columnTags.Contains("Thing_IsRangedWeapon")
            || columnTags.Contains("Thing_IsMeleeWeapon");
    }

    private void RegisterEquipmentRecipeFilters()
    {
        BooleanFilter hasRecipeFilter = new(HasRecipe);
        hasRecipeFilter.OnChange += ApplyFilters;
        _filters.Add(new FilterEntry(HasRecipeFilterKey, null, new Label("has recipe"), "has recipe", hasRecipeFilter));

        Filter materialFilter = new MTMFilter<ThingDef?>(
            GetMaterialFilterValue,
            MakeThingDefFilterOptions(GetMaterialFilterOptions()));
        materialFilter.OnChange += ApplyFilters;
        _filters.Add(new FilterEntry(MaterialFilterKey, null, new Label("material"), "material", materialFilter));

        Filter recipeIngredientsFilter = new MTMFilter<ThingDef?>(
            GetRecipeIngredientsFilterValue,
            MakeThingDefFilterOptions(GetRecipeIngredientFilterOptions()));
        recipeIngredientsFilter.OnChange += ApplyFilters;
        _filters.Add(new FilterEntry(RecipeIngredientsFilterKey, null, new Label("recipe ingredients"), "recipe ingredients", recipeIngredientsFilter));

        Filter recipeBenchFilter = new MTMFilter<ThingDef?>(
            GetRecipeBenchFilterValue,
            MakeThingDefFilterOptions(GetRecipeBenchFilterOptions()));
        recipeBenchFilter.OnChange += ApplyFilters;
        _filters.Add(new FilterEntry(RecipeBenchFilterKey, null, new Label("recipe bench"), "recipe bench", recipeBenchFilter));
    }

    private bool HasRecipe(int row)
    {
        return _objects[row] is DefBasedObject { Def: ThingDef thingDef }
            && thingDef.GetRecipeDefs()?.Count > 0;
    }

    private IEnumerable<ThingDef?> GetMaterialFilterValue(int row)
    {
        if (_objects[row] is not DefBasedObject { Def: ThingDef thingDef } @object)
        {
            return _emptyThingDefSet;
        }

        if (@object.StuffDef != null)
        {
            return [@object.StuffDef];
        }

        return thingDef.GetAllowedStuffs()?.Cast<ThingDef?>() ?? _emptyThingDefSet;
    }

    private IEnumerable<ThingDef?> GetRecipeIngredientsFilterValue(int row)
    {
        if (_objects[row] is not DefBasedObject { Def: ThingDef thingDef } @object)
        {
            return _emptyThingDefSet;
        }

        return GetRecipeIngredients(thingDef, @object.StuffDef);
    }

    private IEnumerable<ThingDef?> GetRecipeBenchFilterValue(int row)
    {
        if (_objects[row] is not DefBasedObject { Def: ThingDef thingDef })
        {
            return _emptyThingDefSet;
        }

        HashSet<RecipeDef>? recipes = thingDef.GetRecipeDefs();
        if (recipes == null || recipes.Count == 0)
        {
            return _emptyThingDefSet;
        }

        HashSet<ThingDef?> benches = [];
        foreach (RecipeDef recipe in recipes)
        {
            HashSet<ThingDef>? recipeUsers = recipe.GetAllRecipeUsers();
            if (recipeUsers != null)
            {
                foreach (ThingDef recipeUser in recipeUsers)
                {
                    benches.Add(recipeUser);
                }
            }
        }

        return benches;
    }

    private IEnumerable<ThingDef?> GetMaterialFilterOptions()
    {
        return GetEquipmentThingDefs()
            .SelectMany(thingDef => thingDef.GetAllowedStuffs() ?? [])
            .Distinct();
    }

    private IEnumerable<ThingDef?> GetRecipeIngredientFilterOptions()
    {
        return GetEquipmentThingDefs()
            .SelectMany(thingDef => GetRecipeIngredients(thingDef, null))
            .Distinct();
    }

    private IEnumerable<ThingDef?> GetRecipeBenchFilterOptions()
    {
        return GetEquipmentThingDefs()
            .SelectMany(thingDef => thingDef.GetRecipeDefs() ?? [])
            .SelectMany(recipe => recipe.GetAllRecipeUsers() ?? [])
            .Distinct();
    }

    private IEnumerable<ThingDef> GetEquipmentThingDefs()
    {
        return _objects
            .OfType<DefBasedObject>()
            .Select(@object => @object.Def)
            .OfType<ThingDef>()
            .Distinct();
    }

    private static IEnumerable<ThingDef?> GetRecipeIngredients(ThingDef thingDef, ThingDef? stuffDef)
    {
        HashSet<RecipeDef>? recipes = thingDef.GetRecipeDefs();
        if (recipes == null || recipes.Count == 0)
        {
            return _emptyThingDefSet;
        }

        HashSet<ThingDef?> ingredients = [];
        foreach (RecipeDef recipe in recipes)
        {
            foreach (IngredientCount ingredient in recipe.ingredients)
            {
                if (stuffDef != null && ingredient.filter.Allows(stuffDef))
                {
                    ingredients.Add(stuffDef);
                    continue;
                }

                foreach (ThingDef ingredientThingDef in ingredient.filter.AllowedThingDefs)
                {
                    ingredients.Add(ingredientThingDef);
                }
            }
        }

        return ingredients;
    }

    private static IEnumerable<NTMFilterOption<ThingDef?>> MakeThingDefFilterOptions(IEnumerable<ThingDef?> thingDefs)
    {
        return thingDefs
            .OrderBy(thingDef => thingDef?.LabelCap.RawText)
            .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
                thingDef => thingDef == null
                    ? new NTMFilterOption<ThingDef?>()
                    : new NTMFilterOption<ThingDef?>(thingDef, thingDef.LabelCap, new LegacyThingDefIcon(thingDef)));
    }

    private readonly record struct FilterEntry(string Key, Column? Column, FilterLabelWidget Label, string LabelText, Filter Widget);

    private sealed class FiltersWindow : Window
    {
        protected override float Margin => GUIStyles.Global.Pad;

        private readonly ObjectTable<TObject> _parent;
        private Vector2 _scrollPosition;

        public FiltersWindow(ObjectTable<TObject> parent)
        {
            _parent = parent;
            closeOnClickedOutside = true;
            doCloseX = true;
            draggable = true;
            optionalTitle = "Filters";
        }

        public override void DoWindowContents(Rect rect)
        {
            rect
                .CutTop(out Rect controlsRect, 30f)
                .TakeRest(out Rect filtersRect);

            DrawControls(controlsRect);

            if (_parent._filters.Count == 0)
            {
                Widgets.Label(filtersRect, "No filters available.");
                return;
            }

            float labelWidth = _parent._filters.Max(filter => filter.Label.Size.x);
            float rowGap = GUIStyles.Global.PadSm;
            float contentHeight = 0f;
            float filterWidth = Mathf.Max(filtersRect.width - labelWidth - GUIStyles.Global.Pad - GenUI.ScrollBarWidth, 160f);
            Vector2 filterContainerSize = new(filterWidth, filtersRect.height);

            foreach (FilterEntry filter in _parent._filters)
            {
                contentHeight += Mathf.Max(filter.Label.Size.y, filter.Widget.GetSize(filterContainerSize).y) + rowGap;
            }

            Rect viewRect = new(0f, 0f, Mathf.Max(filtersRect.width - GenUI.ScrollBarWidth, 1f), Mathf.Max(filtersRect.height, contentHeight));
            using (new GUIScrollScope(filtersRect, ref _scrollPosition, viewRect))
            {
                Rect rowRect = new(0f, 0f, viewRect.width, 0f);
                foreach (FilterEntry filter in _parent._filters)
                {
                    rowRect.height = Mathf.Max(filter.Label.Size.y, filter.Widget.GetSize(filterContainerSize).y);
                    DrawFilterRow(rowRect, filter, labelWidth);
                    rowRect.y = rowRect.yMax + rowGap;
                }
            }
        }

        public override void PostClose()
        {
            _parent._filtersWindow = null;
            base.PostClose();
        }

        protected override void SetInitialSizeAndPosition()
        {
            Vector2 size = new(Mathf.Min(UI.screenWidth * 0.45f, 520f), Mathf.Min(UI.screenHeight * 0.6f, 420f));
            Vector2 position = UI.MousePositionOnUIInverted;

            if (position.x + size.x > UI.screenWidth)
            {
                position.x = UI.screenWidth - size.x - GUIStyles.Global.Pad;
            }

            if (position.y + size.y > UI.screenHeight)
            {
                position.y = UI.screenHeight - size.y - GUIStyles.Global.Pad;
            }

            position.x = Mathf.Max(position.x, GUIStyles.Global.Pad);
            position.y = Mathf.Max(position.y, GUIStyles.Global.Pad);
            windowRect = new Rect(position, size);
        }

        private void DrawControls(Rect rect)
        {
            rect.CutLeft(out Rect resetButtonRect, 90f);
            if (Widgets.ButtonText(resetButtonRect, "Reset"))
            {
                _parent.ResetFilters();
            }
        }

        private static void DrawFilterRow(Rect rect, FilterEntry filter, float labelWidth)
        {
            rect
                .CutLeft(out Rect labelRect, labelWidth)
                .CutLeft(GUIStyles.Global.Pad)
                .TakeRest(out Rect filterRect);

            labelRect.y += (labelRect.height - filter.Label.Size.y) / 2f;
            labelRect.height = filter.Label.Size.y;
            filter.Label.Draw(labelRect);

            Vector2 filterSize = filter.Widget.GetSize(filterRect.size);
            filterRect.y += (filterRect.height - filterSize.y) / 2f;
            filterRect.height = filterSize.y;
            filter.Widget.Draw(filterRect, filterRect.size);
        }
    }
}
