using System;
using Stats.ObjectTable.Cells;
using UnityEngine;

namespace Stats.ObjectTable.FilterWidgets;

public sealed class StringFilter : FilterWidgetWithInputField<string, string>
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
            OnChange?.Invoke();
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
            OnChange?.Invoke();
        }
    }
    protected override string InputFieldText => Value;
    public override event Action? OnChange;
    private readonly Func<Cell, string> CellValueFunc;
    public StringFilter(Func<Cell, string> cellValueFunc, string? placeholder = null) : base([
        Operators.Contains.Instance,
        Operators.NotContains.Instance,
    ], placeholder)
    {
        CellValueFunc = cellValueFunc;
    }
    protected override void DrawInputField(Rect rect)
    {
        Value = GUI.TextField(rect, Value);
    }
    public override bool Eval(Cell cell)
    {
        return Operator.Eval(CellValueFunc(cell), Value);
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
        OnChange?.Invoke();
    }
    public override void NotifyChanged()
    {
        OnChange?.Invoke();
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
