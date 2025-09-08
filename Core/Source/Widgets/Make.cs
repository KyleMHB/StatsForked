namespace Stats.Widgets;

public static class Make
{
    public static Widget TextTableCellInnerWidget(string text)
    {
        return new Label(text).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);
    }
    public static ConstTableCell<decimal> ConstNumberTableCell(decimal value, string formatString)
    {
        return new(value, TextTableCellInnerWidget(value.ToString(formatString)));
    }
}
