using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;
using UnityEngine;

namespace Stats.Tests;

[TestFixture]
public sealed class ColumnRowRebuildTests
{
    [TestCase(false)]
    [TestCase(true)]
    public void Quality_changes_replace_cells_in_reused_visible_and_filter_only_workers(bool hidden)
    {
        var worker = new QualityWorker();
        var names = hidden ? new string[0] : new[] { "quality" };
        var filters = hidden
            ? new[] { new TableFilterState("quality", "quality:0", "Quality", "active") }
            : new TableFilterState[0];
        using var session = new TableSession<DefBasedObject>(
            new TableConfiguration(names, filters, quality: (int)QualityCategory.Normal),
            new[] { new TableColumnDefinition<DefBasedObject>("quality", () => worker) });
        session.SetColumnWorkerRoles(names, hidden ? new[] { "quality" } : new string[0]);
        worker.RebuildRows(Rows(QualityCategory.Normal, 1));

        foreach (QualityCategory quality in new[] { QualityCategory.Legendary, QualityCategory.Awful, QualityCategory.Normal })
        {
            session.Queue(TableIntent.SetQuality((int)quality));
            var staged = session.StagePendingConfiguration(configuration =>
            {
                Assert.That(session.TryGetColumnWorker("quality", out var reused), Is.True);
                Assert.That(reused, Is.SameAs(worker));
                reused!.RebuildRows(Rows((QualityCategory)configuration.Quality, 1));
                return new TableConfigurationTransaction(configuration);
            });
            Assert.That(staged.Applied, Is.True);
            session.PrepareFrame(new TableLayoutMetrics(new[] { 0 }, new[] { 20f }));
            Assert.That(worker.Values(1), Is.EqualTo(new[] { quality }));
            Assert.That(worker.GetWidth(new List<int> { 0 }), Is.EqualTo((float)quality));
        }
    }

    [Test]
    public void Variant_growth_shrinkage_and_empty_rebuilds_replace_all_cached_cells()
    {
        var worker = new QualityWorker();
        worker.RebuildRows(Rows(QualityCategory.Normal, 1));
        worker.RebuildRows(Rows(QualityCategory.Legendary, 3));
        Assert.That(worker.Values(3), Is.EqualTo(Enumerable.Repeat(QualityCategory.Legendary, 3)));
        worker.RebuildRows(Rows(QualityCategory.Awful, 1));
        Assert.That(worker.Values(1), Is.EqualTo(new[] { QualityCategory.Awful }));
        worker.RebuildRows(Rows(QualityCategory.Normal, 0));
        worker.RebuildRows(Rows(QualityCategory.Excellent, 2));
        Assert.That(worker.Values(2), Is.EqualTo(Enumerable.Repeat(QualityCategory.Excellent, 2)));
        Assert.That(worker.IsRefreshable, Is.False);
    }

    private static List<DefBasedObject> Rows(QualityCategory quality, int count) =>
        Enumerable.Range(0, count).Select(_ => new DefBasedObject((Verse.Def)null!, quality: quality)).ToList();

    private sealed class QualityWorker : ColumnWorker<DefBasedObject, QualityCell>
    {
        public override ColumnDef Def => null!;
        public override ColumnType Type => ColumnType.Number;
        protected override QualityCell MakeCell(DefBasedObject row) => new(row.Quality);
        public override ICollection<CellField> GetCellFields(TableWorker tableWorker) => new List<CellField>();
        internal QualityCategory[] Values(int count) =>
            Enumerable.Range(0, count).Select(index => this[index].Quality).ToArray();
    }

    private readonly struct QualityCell(QualityCategory quality) : ICell
    {
        internal QualityCategory Quality => quality;
        public float Width => (float)quality;
        public bool IsRefreshable => false;
        public void Draw(Rect rect) { }
    }
}