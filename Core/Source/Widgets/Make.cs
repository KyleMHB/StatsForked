using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.Widgets;

public static class Make
{
    #region Basic filters
    public static FilterWidget<TObject> StringFilter<TObject>(Func<TObject, string> lhs, string? placeholder = null)
    {
        return new StringFilter<TObject>(lhs, placeholder);
    }
    public static FilterWidget<TObject> NumberFilter<TObject>(Func<TObject, decimal> lhs, string? placeholder = null)
    {
        return new NumberFilter<TObject>(lhs, placeholder);
    }
    public static FilterWidget<TObject> BooleanFilter<TObject>(Func<TObject, bool> lhs)
    {
        return new BooleanFilter<TObject>(lhs);
    }
    #endregion

    #region One-to-many filters
    public static FilterWidget<TObject> OTMFilter<TObject, TOption>(
        Func<TObject, TOption> lhs,
        IEnumerable<NTMFilterOption<TOption>> options,
        string? label = null
    )
    {
        return new OTMFilter<TObject, TOption>(lhs, options, label);
    }
    public static FilterWidget<TObject> OTMFilter<TObject, TOption>(
        Func<TObject, TOption> lhs,
        IEnumerable<TObject> objects,
        string? label = null
    )
    {
        var options = objects
            .Select(lhs)
            .Distinct()
            .OrderBy(option => option)
            .Select<TOption, NTMFilterOption<TOption>>(
                option => option == null ? new() : new(option, option.ToString())
            );

        return OTMFilter(lhs, options, label);
    }
    public static FilterWidget<TObject> OTMThingDefFilter<TObject, TOption>(
        Func<TObject, TOption> lhs,
        IEnumerable<TObject> objects,
        string? label = null
    ) where TOption : ThingDef?
    {
        var options = objects
            .Select(lhs)
            .Distinct()
            .OrderBy(thingDef => thingDef?.label)
            .Select<TOption, NTMFilterOption<TOption>>(
                thingDef => thingDef == null
                    ? new()
                    : new(thingDef, thingDef.LabelCap, new ThingIcon(thingDef))
            );

        return OTMFilter(lhs, options, label);
    }
    public static FilterWidget<TObject> OTMDefFilter<TObject, TOption>(
        Func<TObject, TOption> lhs,
        IEnumerable<TObject> objects,
        string? label = null
    ) where TOption : Def?
    {
        var options = objects
            .Select(lhs)
            .Distinct()
            .OrderBy(def => def?.label)
            .Select<TOption, NTMFilterOption<TOption>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );

        return OTMFilter(lhs, options, label);
    }
    #endregion

    #region Many-to-many filters
    public static FilterWidget<TObject> MTMFilter<TObject, TOption>(
        Func<TObject, HashSet<TOption>> lhs,
        IEnumerable<NTMFilterOption<TOption>> options,
        string? label = null
    )
    {
        return new MTMFilter<TObject, TOption>(lhs, options, label);
    }
    public static FilterWidget<TObject> MTMThingDefFilter<TObject, TOption>(
        Func<TObject, HashSet<TOption>> lhs,
        IEnumerable<TObject> objects,
        string? label = null
    ) where TOption : ThingDef
    {
        var options = objects
            .SelectMany(lhs)
            .Distinct()
            .OrderBy(thingDef => thingDef.label)
            .Select<TOption, NTMFilterOption<TOption>>(
                thingDef => thingDef == null
                    ? new()
                    : new(thingDef, thingDef.LabelCap, new ThingIcon(thingDef))
            );

        return MTMFilter(lhs, options, label);
    }
    public static FilterWidget<TObject> MTMDefFilter<TObject, TOption>(
        Func<TObject, HashSet<TOption>> lhs,
        IEnumerable<TObject> objects,
        string? label = null
    ) where TOption : Def
    {
        var options = objects
            .SelectMany(lhs)
            .Distinct()
            .OrderBy(def => def.label)
            .Select<TOption, NTMFilterOption<TOption>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );

        return MTMFilter(lhs, options, label);
    }
    #endregion
}
