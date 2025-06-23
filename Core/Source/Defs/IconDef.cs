using UnityEngine;
using Verse;

namespace Stats;

public class IconDef : Def
{
    public string path;
    public float scale = 1f;
    public Color? color;
    public Texture2D Texture { get; private set; }
    public override void PostLoad()
    {
        base.PostLoad();

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            Texture = ContentFinder<Texture2D>.Get(path);
        });
    }
}
