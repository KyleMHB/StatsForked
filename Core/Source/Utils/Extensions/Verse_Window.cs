using Verse;

namespace Stats.Utils.Extensions;

internal static class Verse_Window
{
    public static void Open(this Window window)
    {
        Find.WindowStack.Add(window);
    }
}
