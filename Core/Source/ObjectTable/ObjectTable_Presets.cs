using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    private List<string> CaptureVisibleColumnDefNames()
    {
        return _columns.Select(column => column.Def.defName).ToList();
    }

    private void ApplyVisibleColumns(List<string> visibleColumnDefNames)
    {
        string? sortColumnDefName = _sortColumn?.Def.defName;
        int sortDirection = _sortDirection;

        ResetColumns(visibleColumnDefNames);
        RestoreSort(sortColumnDefName, sortDirection);
        SortRows();
        ApplyFilters();
    }

    private void RestoreSort(string? sortColumnDefName, int sortDirection)
    {
        if (sortColumnDefName?.Length > 0)
        {
            _sortColumn = _columns.FirstOrDefault(column => column.Def.defName == sortColumnDefName) ?? _sortColumn;
            _sortDirection = sortDirection;
        }
    }

    private void SavePreset(string presetName)
    {
        if (presetName.NullOrEmpty())
        {
            return;
        }

        StatsSettings settings = StatsMod.Instance.Settings;
        TablePreset preset = settings.presets.FirstOrDefault(existingPreset =>
            existingPreset.tableDefName == _tableWorker.Def.defName
            && existingPreset.name.Equals(presetName, StringComparison.CurrentCultureIgnoreCase));

        if (preset == null)
        {
            preset = new TablePreset();
            settings.presets.Add(preset);
        }

        preset.tableDefName = _tableWorker.Def.defName;
        preset.name = presetName;
        preset.showVariants = _showVariants;
        preset.visibleColumnDefNames = CaptureVisibleColumnDefNames();
        preset.filterStates = CaptureFilterPresetStates();
        StatsMod.Instance.WriteSettings();
    }

    private void ApplyPreset(TablePreset preset)
    {
        if (SupportsVariants && _showVariants != preset.showVariants)
        {
            string? sortColumnDefName = _sortColumn?.Def.defName;
            int sortDirection = _sortDirection;

            _showVariants = preset.showVariants;
            ResetRows(GetCurrentObjects());
            ResetColumns(preset.visibleColumnDefNames);
            RestoreSort(sortColumnDefName, sortDirection);
            SortRows();
            ApplyFilters();
        }
        else
        {
            ApplyVisibleColumns(preset.visibleColumnDefNames);
        }

        ApplyFilterPresetStates(preset.filterStates);
    }

    private void DeletePreset(TablePreset preset)
    {
        StatsMod.Instance.Settings.presets.Remove(preset);
        StatsMod.Instance.WriteSettings();
    }

    private IEnumerable<TablePreset> GetPresets()
    {
        return StatsMod.Instance.Settings.presets
            .Where(preset => preset.tableDefName == _tableWorker.Def.defName)
            .OrderBy(preset => preset.name);
    }

    private sealed class PresetNameWindow : Window
    {
        protected override float Margin => GUIStyles.Global.Pad;

        private readonly Action<string> _onConfirm;
        private string _name;

        public PresetNameWindow(string initialName, Action<string> onConfirm)
        {
            _name = initialName;
            _onConfirm = onConfirm;
            doCloseX = true;
            draggable = true;
            closeOnClickedOutside = true;
            optionalTitle = "Save Preset";
        }

        public override void DoWindowContents(Rect rect)
        {
            rect.CutTop(out Rect textFieldRect, Text.LineHeight + GUIStyles.Global.PadSm);
            _name = Widgets.TextField(textFieldRect, _name);

            rect.yMin = textFieldRect.yMax + GUIStyles.Global.Pad;
            rect.CutLeft(out Rect saveRect, 110f).CutLeft(GUIStyles.Global.PadSm);
            Rect cancelRect = new(saveRect.xMax + GUIStyles.Global.PadSm, saveRect.y, 110f, saveRect.height);

            if (Widgets.ButtonText(saveRect, "Save"))
            {
                _onConfirm(_name.Trim());
                Close();
            }

            if (Widgets.ButtonText(cancelRect, "Cancel"))
            {
                Close();
            }
        }

        protected override void SetInitialSizeAndPosition()
        {
            Vector2 size = new(260f, 100f);
            Vector2 position = UI.MousePositionOnUIInverted;
            if (position.x + size.x > UI.screenWidth)
            {
                position.x = UI.screenWidth - size.x - GUIStyles.Global.Pad;
            }

            if (position.y + size.y > UI.screenHeight)
            {
                position.y = UI.screenHeight - size.y - GUIStyles.Global.Pad;
            }

            windowRect = new Rect(position, size);
        }
    }
}
