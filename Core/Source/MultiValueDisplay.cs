using System;

namespace Stats;

internal static class MultiValueDisplay
{
    internal const int MaxLines = 10;

    [ThreadStatic]
    private static bool _isExpanded;

    internal static bool IsExpanded => _isExpanded;

    internal static Scope Enter(bool isExpanded)
    {
        return new Scope(isExpanded);
    }

    internal readonly struct Scope : IDisposable
    {
        private readonly bool _previous;

        public Scope(bool isExpanded)
        {
            _previous = _isExpanded;
            _isExpanded = isExpanded;
        }

        public void Dispose()
        {
            _isExpanded = _previous;
        }
    }
}
