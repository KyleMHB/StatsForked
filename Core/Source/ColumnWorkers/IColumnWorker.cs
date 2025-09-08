using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;

namespace Stats;

public interface IColumnWorker
{
    //public ColumnDef Def { get; }
    public CellStyleType CellStyle { get; }

    public enum CellStyleType
    {
        Number = TextAnchor.LowerRight,
        String = TextAnchor.LowerLeft,
        Boolean = TextAnchor.LowerCenter,
    }
}

public interface IColumnWorker<TObject> : IColumnWorker
{
    public ObjectTable.Cell GetCell(TObject @object);
    public IEnumerable<ObjectTable.ObjectProp> GetObjectProps();
}
