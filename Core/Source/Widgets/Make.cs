namespace Stats.Widgets;

public static class Make
{
    //public static class CellWidget
    //{
    //    public static Widget String(string text)
    //    {
    //        if (text.Length == 0) return new EmptyWidget();

    //        return new Label(text)
    //            .PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);
    //    }
    //    public static Widget Number(decimal number, string formatString = "")
    //    {
    //        if (number == 0m) return new EmptyWidget();

    //        string text = number.ToString(formatString);

    //        return new Label(text)
    //            .PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);
    //    }
    //    public static Widget Boolean(bool value)
    //    {
    //        if (value == false) return new EmptyWidget();

    //        return new Icon(Verse.Widgets.CheckboxOnTex)
    //            .PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);
    //    }
    //    public static Widget ThingDefCount(ThingDef? thingDef, decimal count)
    //    {
    //        if (thingDef == null) return new EmptyWidget();

    //        Widget label = new Label(count.ToString())
    //            .PaddingRel(1f, 0f, 0f, 0f);
    //        Widget icon = new ThingDefIcon(thingDef)
    //            .ToButtonGhostly(() => Draw.DefInfoDialog(thingDef))
    //            .Tooltip(thingDef.LabelCap);

    //        return new HorizontalContainer([label, icon], Globals.GUI.PadSm, true)
    //            .PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);
    //    }
    //}
}
