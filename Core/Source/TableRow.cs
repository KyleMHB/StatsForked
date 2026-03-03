namespace Stats;

public readonly struct TableRow<TObject>(int index, TObject @object)
{
    public readonly int Index = index;
    public readonly TObject Object = @object;
}
