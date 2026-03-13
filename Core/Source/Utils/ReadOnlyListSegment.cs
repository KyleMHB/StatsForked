using System;
using System.Collections.Generic;

namespace Stats.Utils;

internal readonly struct ReadOnlyListSegment<T>
{
    public readonly int Start;
    public readonly int Length;

    private readonly List<T> _list;

    public ReadOnlyListSegment(List<T> list, int start, int length)
    {
        _list = list;
        Start = start;
        Length = length;
    }

    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return _list[index + Start];
        }
    }

    public ReadOnlyListSegment<T> Slice(int start, int length)
    {
        return new ReadOnlyListSegment<T>(_list, Start + start, length);
    }
}
