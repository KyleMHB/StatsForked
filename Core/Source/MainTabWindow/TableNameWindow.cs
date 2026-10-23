using System;
using UnityEngine;
using Verse;

namespace Stats;

internal static class TableName
{
    internal const int MaxLength = 120;

    internal static string Normalize(string? name)
    {
        string normalized = (name ?? "").Replace('\r', ' ').Replace('\n', ' ').Trim();
        return normalized.Length > MaxLength ? normalized.Substring(0, MaxLength) : normalized;
    }
}

internal sealed class TableNameWindow : Window
{
    private readonly Action<string> _onConfirm;
    private string _name;

    public override Vector2 InitialSize => new(480f, 170f);

    internal TableNameWindow(string initialName, Action<string> onConfirm)
    {
        _name = initialName;
        _onConfirm = onConfirm;
        doCloseX = true;
        draggable = true;
        closeOnClickedOutside = true;
        optionalTitle = Localization.Get(Localization.Rename);
    }

    public override void DoWindowContents(Rect rect)
    {
        _name = Widgets.TextField(new Rect(0f, 0f, rect.width, 30f), _name, TableName.MaxLength);
        Widgets.Label(new Rect(0f, 35f, rect.width, 30f), Localization.Get(Localization.ClearTabNameTip));
        if (Widgets.ButtonText(new Rect(0f, rect.height - 30f, 110f, 30f), Localization.Get(Localization.Save)))
        {
            _onConfirm(TableName.Normalize(_name));
            Close();
        }
        if (Widgets.ButtonText(new Rect(120f, rect.height - 30f, 110f, 30f), Localization.Get(Localization.Cancel)))
        {
            Close();
        }
    }
}
