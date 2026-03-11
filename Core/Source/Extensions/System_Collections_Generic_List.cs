using System;
using System.Collections.Generic;

namespace Stats.Extensions;

public static class System_Collections_Generic_List
{
    public static void ReplaceWithLast<T>(this List<T> list, int index)
    {
        int lastItemlIndex = list.Count - 1;
        list[index] = list[lastItemlIndex];
        list.RemoveAt(lastItemlIndex);
    }

    internal static void CopyTo<T>(this List<T> list, Span<T> span, int start)
    {
        for (int i = 0; i < span.Length; i++)
        {
            span[i] = list[start + i];
        }
    }

    internal static void CopyTo<T>(this List<T> list, Span<T> span) => list.CopyTo(span, 0);
}
