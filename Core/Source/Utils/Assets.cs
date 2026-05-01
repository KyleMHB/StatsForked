using UnityEngine;
using Verse;

namespace Stats.Utils;

[StaticConstructorOnStartup]
public static class Assets
{
    internal static readonly Texture2D TableFiltersTabIcon;
    internal static readonly Texture2D TableColumnsMenuIcon;

    static Assets()
    {
        TableFiltersTabIcon = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/Filter");
        TableColumnsMenuIcon = ContentFinder<Texture2D>.Get("UI/Buttons/OpenSpecificTab");
    }
}
