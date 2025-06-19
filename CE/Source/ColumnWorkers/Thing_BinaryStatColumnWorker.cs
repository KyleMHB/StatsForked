using System.Linq;
using RimWorld;
using Verse;

namespace Stats.Compat.CE;

public abstract class Thing_BinaryStatColumnWorker : Thing_StatColumnWorker
{
    protected readonly char Separator;
    protected Thing_BinaryStatColumnWorker(BinaryStatColumnDef columnDef) : base(columnDef)
    {
        Separator = columnDef.statValueSeparator[0];
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var worker = Stat.Worker;
        var statRequest = StatRequest.For(thing.Def, thing.StuffDef);

        if (worker.ShouldShowFor(statRequest))
        {
            var statValue = worker.GetValue(statRequest);
            var label = worker.GetStatDrawEntryLabel(Stat, statValue, ToStringNumberSense.Absolute, statRequest);

            // We do not check whether statValue is 0 here,
            // because since this type of stat is often times used
            // only for display purposes it can have a value of 0,
            // but display non-zero value.

            if (label?.Length > 0)
            {
                return ParsePartFromLabel(label);
            }
        }

        return "";
    }
    protected abstract string ParsePartFromLabel(string label);
}

public sealed class Thing_BinaryStatColumnWorker_Left : Thing_BinaryStatColumnWorker
{
    public Thing_BinaryStatColumnWorker_Left(BinaryStatColumnDef columnDef) : base(columnDef)
    {
    }
    protected override string ParsePartFromLabel(string label)
    {
        label = label.Split(Separator).First();

        if (Utils.NonZeroNumberRegex.IsMatch(label))
        {
            return label.TrimEnd();
        }

        return "";
    }
}

public sealed class Thing_BinaryStatColumnWorker_Right : Thing_BinaryStatColumnWorker
{
    public Thing_BinaryStatColumnWorker_Right(BinaryStatColumnDef columnDef) : base(columnDef)
    {
    }
    protected override string ParsePartFromLabel(string label)
    {
        label = label.Split(Separator).Last();

        if (Utils.NonZeroNumberRegex.IsMatch(label))
        {
            return label.TrimStart();
        }

        return "";
    }
}
