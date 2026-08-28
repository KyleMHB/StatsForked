using Verse;

namespace Stats;

internal static class Localization
{
    internal const string OpenTable = "StatsForked_OpenTable";
    internal const string Remove = "StatsForked_Remove";
    internal const string Filters = "StatsForked_Filters";
    internal const string Columns = "StatsForked_Columns";
    internal const string Presets = "StatsForked_Presets";
    internal const string Variants = "StatsForked_Variants";
    internal const string Values = "StatsForked_Values";
    internal const string Quality = "StatsForked_Quality";
    internal const string SaveCurrent = "StatsForked_SaveCurrent";
    internal const string SavePreset = "StatsForked_SavePreset";
    internal const string Save = "StatsForked_Save";
    internal const string Cancel = "StatsForked_Cancel";
    internal const string Apply = "StatsForked_Apply";
    internal const string Overwrite = "StatsForked_Overwrite";
    internal const string Delete = "StatsForked_Delete";
    internal const string SetDefault = "StatsForked_SetDefault";
    internal const string ClearDefault = "StatsForked_ClearDefault";
    internal const string Default = "StatsForked_Default";
    internal const string Hidden = "StatsForked_Hidden";
    internal const string AddFilter = "StatsForked_AddFilter";
    internal const string SearchFilters = "StatsForked_SearchFilters";
    internal const string TableFilters = "StatsForked_TableFilters";
    internal const string VisibleColumns = "StatsForked_VisibleColumns";
    internal const string HiddenColumns = "StatsForked_HiddenColumns";
    internal const string HiddenColumn = "StatsForked_HiddenColumn";
    internal const string NoFiltersApplied = "StatsForked_NoFiltersApplied";
    internal const string NoFilterResults = "StatsForked_NoFilterResults";
    internal const string RemoveFilter = "StatsForked_RemoveFilter";
    internal const string ResetAll = "StatsForked_ResetAll";
    internal const string ValuesExpandTip = "StatsForked_ValuesExpandTip";
    internal const string ValuesCompactTip = "StatsForked_ValuesCompactTip";
    internal const string NoFilters = "StatsForked_NoFilters";
    internal const string Reset = "StatsForked_Reset";
    internal const string SortAscending = "StatsForked_SortAscending";
    internal const string SortDescending = "StatsForked_SortDescending";
    internal const string ResetWidth = "StatsForked_ResetWidth";
    internal const string Available = "StatsForked_Available";
    internal const string HasRecipe = "StatsForked_HasRecipe";
    internal const string Material = "StatsForked_Material";
    internal const string RecipeIngredients = "StatsForked_RecipeIngredients";
    internal const string RecipeBench = "StatsForked_RecipeBench";
    internal const string Amount = "StatsForked_Amount";
    internal const string Type = "StatsForked_Type";
    internal const string ResearchStatus = "StatsForked_ResearchStatus";
    internal const string Researched = "StatsForked_Researched";
    internal const string NotResearched = "StatsForked_NotResearched";
    internal const string NoResearchRequired = "StatsForked_NoResearchRequired";
    internal const string Expanded = "StatsForked_Expanded";
    internal const string Compact = "StatsForked_Compact";
    internal const string More = "StatsForked_More";
    internal const string ClearOptions = "StatsForked_ClearOptions";
    internal const string MultiSelectHint = "StatsForked_MultiSelectHint";
    internal const string Undefined = "StatsForked_Undefined";
    internal const string FilterContains = "StatsForked_FilterContains";
    internal const string FilterDoesNotContain = "StatsForked_FilterDoesNotContain";
    internal const string FilterIsOneOf = "StatsForked_FilterIsOneOf";
    internal const string FilterIsNotOneOf = "StatsForked_FilterIsNotOneOf";
    internal const string FilterIsEqualTo = "StatsForked_FilterIsEqualTo";
    internal const string FilterIsNotEqualTo = "StatsForked_FilterIsNotEqualTo";
    internal const string FilterContainsAtLeastOne = "StatsForked_FilterContainsAtLeastOne";
    internal const string FilterDoesNotContainAny = "StatsForked_FilterDoesNotContainAny";
    internal const string FilterIsSubsetOf = "StatsForked_FilterIsSubsetOf";
    internal const string FilterIsSupersetOf = "StatsForked_FilterIsSupersetOf";
    internal const string ManualScroll = "StatsForked_ManualScroll";
    internal const string ManualPinColumn = "StatsForked_ManualPinColumn";
    internal const string ManualPinRow = "StatsForked_ManualPinRow";
    internal const string ManualPinMultipleRows = "StatsForked_ManualPinMultipleRows";
    internal const string ManualPinnedRowsUnaffected = "StatsForked_ManualPinnedRowsUnaffected";
    internal const string ManualResize = "StatsForked_ManualResize";
    internal const string ManualResetResize = "StatsForked_ManualResetResize";

    internal static string Get(string key)
    {
        return key.Translate().ToString();
    }
}
