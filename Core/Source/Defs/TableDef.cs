using System;
using System.Collections.Generic;
using Stats.TableWorkers;
using UnityEngine;
using Verse;

namespace Stats;

public class TableDef : Def
{
    public string? iconPath;
    public Texture2D Icon { get; private set; } = BaseContent.BadTex;
    public Color iconColor = Color.white;
    public float iconScale = 1f;
#pragma warning disable CS8618
    public List<ColumnDef> columns;
    public Type workerClass;
    public TableWorker Worker => field ??= (TableWorker)Activator.CreateInstance(workerClass, this);
#pragma warning restore CS8618
    public List<string> columnTags = [];

    public override void ResolveReferences()
    {
        base.ResolveReferences();

        LongEventHandler.ExecuteWhenFinished(ResolveIcon);
    }

    private void ResolveIcon()
    {
        if (iconPath?.Length > 0)
        {
            Icon = ContentFinder<Texture2D>.Get(iconPath);
        }
    }

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string item in base.ConfigErrors())
        {
            yield return item;
        }

        if (columnTags.Count == 0)
        {
            yield return "no column tags.";
        }
    }
}
