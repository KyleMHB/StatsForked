using System.Collections.Generic;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Gene_LabelColumnWorker(ColumnDef columnDef) : ColumnWorker<GeneDef, Gene_LabelColumnWorker.GeneLabelCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;
    public override bool ShouldDrawCellsNow => Event.current.type == EventType.Repaint || Event.current.IsLeftMouseInteraction();

    protected override GeneLabelCell MakeCell(GeneDef geneDef)
    {
        return new GeneLabelCell(geneDef);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter textFieldFilter = new StringFilter((int row) => this[row].Text ?? "");
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField textField = new(Def.TitleWidget, textFieldFilter, Compare);

        return [textField];
    }

    public readonly struct GeneLabelCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public string Text { get; }

        private readonly GeneDef _geneDef;
        private readonly Texture2D _icon;
        private readonly float _iconWidth;

        public GeneLabelCell(GeneDef geneDef)
        {
            _geneDef = geneDef;
            Text = geneDef.LabelCap.RawText;
            _icon = geneDef.Icon ?? BaseContent.BadTex;
            _iconWidth = Verse.Text.LineHeight;
            Width = _iconWidth + GUIStyles.TableCell.ContentSpacing + Text.CalcSize(GUIStyles.TableCell.StringNoPad).x;
        }

        public void Draw(Rect rect)
        {
            rect
                .ContractedByObjectTableCellPadding()
                .CutLeft(out Rect iconRect, _iconWidth)
                .CutLeft(GUIStyles.TableCell.ContentSpacing)
                .TakeRest(out Rect labelRect);

            if (Event.current.type == EventType.Repaint)
            {
                iconRect.DrawTextureFitted(_icon);
                labelRect.Label(Text, GUIStyles.TableCell.String);
                iconRect.Tip(_geneDef.description);
            }

            if (iconRect.ButtonGhostly())
            {
                _geneDef.OpenInfoDialog();
            }
        }
    }
}
