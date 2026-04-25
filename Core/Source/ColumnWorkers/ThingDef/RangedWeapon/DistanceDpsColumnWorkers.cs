using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;
using RimWorld;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class CloseDpsColumnWorker(ColumnDef columnDef) : DistanceDpsColumnWorker(columnDef, "AccuracyTouch");
public sealed class ShortDpsColumnWorker(ColumnDef columnDef) : DistanceDpsColumnWorker(columnDef, "AccuracyShort");
public sealed class MediumDpsColumnWorker(ColumnDef columnDef) : DistanceDpsColumnWorker(columnDef, "AccuracyMedium");
public sealed class LongDpsColumnWorker(ColumnDef columnDef) : DistanceDpsColumnWorker(columnDef, "AccuracyLong");

public abstract class DistanceDpsColumnWorker(ColumnDef columnDef, string accuracyStatName) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is not Verse.ThingDef thingDef)
        {
            return default;
        }

        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();
        if (verbProps is not { Ranged: true } || verbProps.defaultProjectile?.projectile?.damageDef?.harmsHealth != true)
        {
            return default;
        }

        StatDef? accuracyStat = DefDatabase<StatDef>.GetNamedSilentFail(accuracyStatName);
        StatDef? cooldownStat = DefDatabase<StatDef>.GetNamedSilentFail("RangedWeapon_Cooldown");
        if (accuracyStat == null || cooldownStat == null)
        {
            return default;
        }

        float accuracy = thingDef.GetStatValuePerceived(accuracyStat, @object.StuffDef, @object.Quality);
        if (accuracy <= 0f)
        {
            return default;
        }

        float projectileDamage = verbProps.defaultProjectile.projectile.GetDamageAmount(thingDef, null);
        int burstShotCount = verbProps.burstShotCount > 0 ? verbProps.burstShotCount : 1;
        float burstDuration = burstShotCount > 1
            ? (burstShotCount - 1) * verbProps.ticksBetweenBurstShots.TicksToSeconds()
            : 0f;
        float cycleSeconds =
            verbProps.warmupTime
            + thingDef.GetStatValuePerceived(cooldownStat, @object.StuffDef, @object.Quality)
            + burstDuration;
        if (cycleSeconds <= 0f)
        {
            return default;
        }

        decimal cellValue = (projectileDamage * burstShotCount * accuracy / cycleSeconds).ToDecimal(2);
        return new NumberCell(cellValue, "0.00");
    }
}
