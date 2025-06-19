using System.Linq;

namespace Stats.Compat.CE;

public sealed class Thing_BinaryStatColumnWorker_Left : Thing_StatColumnWorker
{
    private readonly char Separator;
    public Thing_BinaryStatColumnWorker_Left(BinaryStatColumnDef columnDef) : base(columnDef)
    {
        Separator = columnDef.statValueSeparator[0];
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var label = base.GetStatDrawEntryLabel(thing);

        if (label.Length > 0)
        {
            label = label.Split(Separator).First();

            if (Utils.NonZeroNumberRegex.IsMatch(label))
            {
                return label.TrimEnd();
            }
        }

        return "";
    }
}

public sealed class Thing_BinaryStatColumnWorker_Right : Thing_StatColumnWorker
{
    private readonly char Separator;
    public Thing_BinaryStatColumnWorker_Right(BinaryStatColumnDef columnDef) : base(columnDef)
    {
        Separator = columnDef.statValueSeparator[0];
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var label = base.GetStatDrawEntryLabel(thing);

        if (label.Length > 0)
        {
            label = label.Split(Separator).Last();

            if (Utils.NonZeroNumberRegex.IsMatch(label))
            {
                return label.TrimStart();
            }
        }

        return "";
    }
}
