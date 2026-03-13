using UnityEngine;
using Verse;

namespace Stats.Utils;

[StaticConstructorOnStartup]
public static class Assets
{
    public static readonly Texture2D ExpandWindowTex;
    public static readonly Texture2D FilterTex;

    static Assets()
    {
        ExpandWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ExpandWindow");
        FilterTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/Filter");
    }
}
