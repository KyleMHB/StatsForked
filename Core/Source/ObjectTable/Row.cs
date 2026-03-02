namespace Stats.ObjectTable;

public readonly struct Row<TObject>(int index, TObject @object)
{
    public readonly int Index = index;
    public readonly TObject Object = @object;
}
