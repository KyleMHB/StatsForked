using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats.Widgets;

public sealed class StringFilter<TCell> : FilterWidgetWithInputField<string, string> where TCell : ObjectTable.Cell
{
    public override bool IsActive => Value.Length > 0;
    private string _Value = "";
    private string Value
    {
        get => _Value;
        set
        {
            if (_Value == value)
            {
                return;
            }

            _Value = value;
            Resize();
            OnChange?.Invoke(this);
        }
    }
    private RelOperator<string, string> _Operator = Operators.Default;
    protected override RelOperator<string, string> Operator
    {
        get => _Operator;
        set
        {
            if (_Operator == value)
            {
                return;
            }

            _Operator = value;
            Resize();
            OnChange?.Invoke(this);
        }
    }
    protected override string InputFieldText => Value;
    public override event Action<FilterWidget>? OnChange;
    private readonly Func<TCell, string> ObjectValueFunc;
    private readonly ColumnWorker Column;
    public StringFilter(Func<TCell, string> objectValueFunc, ColumnWorker column, string? placeholder = null) : base([
        Operators.Contains.Instance,
        Operators.NotContains.Instance,
    ], placeholder)
    {
        ObjectValueFunc = objectValueFunc;
        Column = column;
    }
    protected override void DrawInputField(Rect rect)
    {
        Value = GUI.TextField(rect, Value);
    }
    public override bool Eval(Dictionary<ColumnWorker, ObjectTable.Cell> cells)
    {
        return Operator.Eval(ObjectValueFunc((TCell)cells[Column]), Value);
    }
    public override void Reset()
    {
        _Operator = Operators.Default;
        ClearInputField();
    }
    protected override void ClearInputField()
    {
        _Value = "";
        Resize();
        OnChange?.Invoke(this);
    }
    public override void NotifyChanged()
    {
        OnChange?.Invoke(this);
    }

    private static class Operators
    {
        public static RelOperator<string, string> Default = Contains.Instance;

        public sealed class Contains : RelOperator<string, string>
        {
            public Contains() : base("~=", "Contains") { }
            public override bool Eval(string lhs, string rhs) =>
                lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase);
            public static Contains Instance { get; } = new();
        }

        public sealed class NotContains : RelOperator<string, string>
        {
            public NotContains() : base("!~=", "Does not contains") { }
            public override bool Eval(string lhs, string rhs) =>
                lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase) == false;
            public static NotContains Instance { get; } = new();
        }
    }
}
