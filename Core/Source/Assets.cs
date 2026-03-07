using UnityEngine;
using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public static class Assets
{
    public static readonly Texture2D ExpandWindowTex;
    public static readonly Texture2D TableSettingsTex;
    public static readonly Texture2D FilterTex;

    static Assets()
    {
        ExpandWindowTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/ExpandWindow");
        TableSettingsTex = ContentFinder<Texture2D>.Get("UI/Icons/Options/OptionsGeneral");
        FilterTex = ContentFinder<Texture2D>.Get("StatsMod/UI/Icons/Filter");
    }
}
