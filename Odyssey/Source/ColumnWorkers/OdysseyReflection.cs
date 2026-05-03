using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace Stats.Compat.Odyssey;

internal static class OdysseyReflection
{
    private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly string[] GravshipCompTypeNames = ["CompProperties_GravshipFacility", "CompProperties_GravshipThruster"];

    public static bool HasComp(ThingDef thingDef, string compPropertiesTypeName)
    {
        return GetComp(thingDef, compPropertiesTypeName) != null;
    }

    public static bool HasCompClass(ThingDef thingDef, string compClassTypeName)
    {
        return thingDef.comps?.Any(comp => ValueToString(GetMemberValue(comp, "compClass")) == compClassTypeName) == true;
    }

    public static object? GetComp(ThingDef thingDef, string compPropertiesTypeName)
    {
        return thingDef.comps?.FirstOrDefault(comp => comp.GetType().Name == compPropertiesTypeName);
    }

    public static object? GetCompMemberValue(ThingDef thingDef, string compPropertiesTypeName, string memberName)
    {
        object? comp = GetComp(thingDef, compPropertiesTypeName);
        return comp == null ? null : GetMemberValue(comp, memberName);
    }

    public static IEnumerable<object> GetCompValues(ThingDef thingDef, string compPropertiesTypeName, string memberName)
    {
        object? value = GetCompMemberValue(thingDef, compPropertiesTypeName, memberName);
        return Enumerate(value);
    }

    public static object? GetGravshipCompMemberValue(ThingDef thingDef, string memberName)
    {
        foreach (string typeName in GravshipCompTypeNames)
        {
            object? value = GetCompMemberValue(thingDef, typeName, memberName);
            if (value != null)
            {
                return value;
            }
        }

        return null;
    }

    public static bool TryGetGravshipStatOffset(ThingDef thingDef, string statDefName, out decimal value)
    {
        foreach (string typeName in GravshipCompTypeNames)
        {
            object? comp = GetComp(thingDef, typeName);
            IEnumerable<object> statOffsets = GetEnumerableMemberValue(comp, "statOffsets") ?? [];
            foreach (object statOffset in statOffsets)
            {
                string? statName = ValueToString(GetMemberValue(statOffset, "stat"));
                if (statName == statDefName && TryGetDecimal(GetMemberValue(statOffset, "value"), out value))
                {
                    return true;
                }
            }
        }

        value = 0m;
        return false;
    }

    public static object? GetMemberValue(object? instance, string memberName)
    {
        if (instance == null)
        {
            return null;
        }

        Type type = instance.GetType();
        FieldInfo? field = type.GetField(memberName, MemberFlags);
        if (field != null)
        {
            return field.GetValue(instance);
        }

        PropertyInfo? property = type.GetProperty(memberName, MemberFlags);
        if (property != null && property.GetIndexParameters().Length == 0)
        {
            return property.GetValue(instance);
        }

        return null;
    }

    public static IEnumerable<object>? GetEnumerableMemberValue(object? instance, string memberName)
    {
        object? value = GetMemberValue(instance, memberName);
        if (value == null || value is string)
        {
            return null;
        }

        return value is IEnumerable enumerable ? enumerable.Cast<object>() : null;
    }

    public static bool TryGetDecimal(object? valueObject, out decimal value)
    {
        switch (valueObject)
        {
            case null:
                value = 0m;
                return false;
            case decimal decimalValue:
                value = decimalValue;
                return true;
            case float floatValue:
                value = (decimal)floatValue;
                return true;
            case double doubleValue:
                value = (decimal)doubleValue;
                return true;
            case int intValue:
                value = intValue;
                return true;
            case long longValue:
                value = longValue;
                return true;
            case short shortValue:
                value = shortValue;
                return true;
            case byte byteValue:
                value = byteValue;
                return true;
            default:
                return decimal.TryParse(valueObject.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }

    public static string? ValueToString(object? value)
    {
        return value switch
        {
            null => null,
            Def def => def.LabelCap.RawText.NullOrEmpty() ? def.defName : def.LabelCap.RawText,
            Type type => type.Name,
            TaggedString taggedString => taggedString.RawText,
            string text => text,
            _ => value.ToString(),
        };
    }

    private static IEnumerable<object> Enumerate(object? value)
    {
        if (value == null || value is string)
        {
            return [];
        }

        return value is IEnumerable enumerable ? enumerable.Cast<object>() : [value];
    }
}

