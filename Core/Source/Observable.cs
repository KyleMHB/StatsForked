using System;
using System.Collections.Generic;

namespace Stats;

internal class Observable<TValue>
{
    public TValue Value
    {
        get => field;
        set
        {
            if (EqualityComparer<TValue>.Default.Equals(value, field)) return;

            field = value;
            OnNext?.Invoke(value);
        }
    }
    public event Action<TValue>? OnNext;
    public Observable(TValue initialValue)
    {
        Value = initialValue;
    }
    public Observable<T> Map<T>(Func<TValue, T> map)
    {
        var observable = new Observable<T>(map(Value));
        OnNext += value => observable.Value = map(value);

        return observable;
    }
}
