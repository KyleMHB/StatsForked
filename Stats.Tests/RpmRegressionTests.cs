using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Stats.ColumnWorkers.Cells;
using Stats.ColumnWorkers.ThingDef.RangedWeapon;
using Verse;

namespace Stats.Tests;

[TestFixture]
public sealed class RpmRegressionTests
{
    [TestCase(0)]
    [TestCase(-1)]
    public void Invalid_burst_intervals_leave_the_rpm_cell_empty(int ticks)
    {
        // Avoid the ThingDef constructor's Unity shader initialization.
        ThingDef weapon = (ThingDef)FormatterServices.GetUninitializedObject(typeof(ThingDef));
        typeof(ThingDef).GetField("verbs", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(weapon, new List<VerbProperties>
        {
            new() { verbClass = typeof(Verb_Shoot), isPrimary = true,
                showBurstShotStats = true, burstShotCount = 2, ticksBetweenBurstShots = ticks }
        });
        RPMColumnWorker worker = new(new ColumnDef());
        MethodInfo makeCell = typeof(RPMColumnWorker).GetMethod("MakeCell", BindingFlags.Instance | BindingFlags.NonPublic)!;
        NumberCell cell = default;
        Assert.DoesNotThrow(() => cell = (NumberCell)makeCell.Invoke(worker, new object[] { new DefBasedObject(weapon) })!);
        Assert.That(cell.Value, Is.Zero);
        Assert.That(cell.Width, Is.Zero);
    }
}
