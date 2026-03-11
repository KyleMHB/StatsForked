using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stats;

public sealed partial class MainTabWindow
{
    private sealed class TableRecord
    {
        public readonly ObjectTable TableWidget;
        public readonly TipSignal Tooltip;
        public readonly Texture2D Icon;
        public readonly Color IconColor;
        public readonly float IconScale;

        public TableRecord(TableDef tableDef)
        {
            TableWidget = tableDef.Worker.TableWidget;
            Tooltip = tableDef.LabelCap;
            if (tableDef.description?.Length > 0)
            {
                Tooltip += $"\n\n{tableDef.description}";
            }
            Icon = tableDef.Icon;
            IconColor = tableDef.iconColor;
            IconScale = tableDef.iconScale;
        }
    }
}
