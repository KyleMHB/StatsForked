using System;
using UnityEngine;
using Verse;

namespace Stats.Filters;

public sealed class NumberFilter : FilterWithInputField<decimal, decimal>, IPresettableFilter
{
    public override bool IsActive => _TextFieldText.Length > 0 && InputIsValid;
    private decimal _Value = 0m;
    private decimal Value
    {
        get => _Value;
        set
        {
            // We don't check if value has changed here because:
            // - It will cause it to not update when it should.
            //   For example, when you input 0 into an empty
            //   input field.
            // - It is already checked in TextFieldText.

            _Value = value;
            OnChange?.Invoke();
        }
    }
    private RelOperator<decimal, decimal> _Operator = Operators.Default;
    protected override RelOperator<decimal, decimal> Operator
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
    public override event Action? OnChange;
    private readonly Func<int, decimal> CellValueFunc;
    private bool InputIsValid = true;
    private string _TextFieldText = "";
    private string TextFieldText
    {
        set
        {
            if (_TextFieldText == value)
            {
                return;
            }

            _TextFieldText = value.Trim();

            if (_TextFieldText.Length == 0)
            {
                InputIsValid = true;
                Value = 0m;
            }
            else
            {
                InputIsValid = decimal.TryParse(_TextFieldText, out var num);

                if (InputIsValid)
                {
                    Value = num;
                }
                else
                {
                    OnChange?.Invoke();
                }
            }

            Resize();
        }
    }
    protected override string InputFieldText => _TextFieldText;
    private static readonly Color ErrorColor = Color.red.ToTransparent(0.5f);
    public NumberFilter(Func<int, decimal> cellValueFunc, string? placeholder = null) : base([
        Operators.IsEqualTo.Instance,
        Operators.IsNotEqualTo.Instance,
        Operators.IsGreaterThan.Instance,
        Operators.IsLesserThan.Instance,
        Operators.IsGreaterThanOrEqualTo.Instance,
        Operators.IsLesserThanOrEqualTo.Instance,
    ], placeholder)
    {
        CellValueFunc = cellValueFunc;
    }
    protected override void DrawInputField(Rect rect)
    {
        if (InputIsValid == false)
        {
            Verse.Widgets.DrawBoxSolid(rect, ErrorColor);
        }

        TextFieldText = GUI.TextField(rect, _TextFieldText);
    }
    public override bool Eval(int row)
    {
        return Operator.Eval(CellValueFunc(row), Value);
    }
    public override void Reset()
    {
        _Operator = Operators.Default;
        ClearInputField();
    }
    protected override void ClearInputField()
    {
        _TextFieldText = "";
        InputIsValid = true;
        _Value = 0m;
        Resize();
        OnChange?.Invoke();
    }
    public override void NotifyChanged()
    {
        OnChange?.Invoke();
    }

    public string SerializeState()
    {
        return $"{Operator.Symbol}|{_TextFieldText}";
    }

    public void DeserializeState(string state)
    {
        string[] parts = state.Split('|');
        if (parts.Length > 0)
        {
            RelOperator<decimal, decimal>? @operator = parts[0] switch
            {
                "==" => Operators.IsEqualTo.Instance,
                "!=" => Operators.IsNotEqualTo.Instance,
                ">" => Operators.IsGreaterThan.Instance,
                "<" => Operators.IsLesserThan.Instance,
                ">=" => Operators.IsGreaterThanOrEqualTo.Instance,
                "<=" => Operators.IsLesserThanOrEqualTo.Instance,
                _ => null,
            };
            if (@operator != null)
            {
                Operator = @operator;
            }
        }

        TextFieldText = parts.Length > 1 ? parts[1] : "";
    }

    private static class Operators
    {
        public static RelOperator<decimal, decimal> Default = IsGreaterThan.Instance;

        public sealed class IsEqualTo : RelOperator<decimal, decimal>
        {
            private IsEqualTo() : base("==") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs == rhs;
            public static IsEqualTo Instance { get; } = new();
        }

        public sealed class IsNotEqualTo : RelOperator<decimal, decimal>
        {
            private IsNotEqualTo() : base("!=") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs != rhs;
            public static IsNotEqualTo Instance { get; } = new();
        }

        public sealed class IsGreaterThan : RelOperator<decimal, decimal>
        {
            private IsGreaterThan() : base(">") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs > rhs;
            public static IsGreaterThan Instance { get; } = new();
        }

        public sealed class IsLesserThan : RelOperator<decimal, decimal>
        {
            private IsLesserThan() : base("<") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs < rhs;
            public static IsLesserThan Instance { get; } = new();
        }

        public sealed class IsGreaterThanOrEqualTo : RelOperator<decimal, decimal>
        {
            private IsGreaterThanOrEqualTo() : base(">=") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs >= rhs;
            public static IsGreaterThanOrEqualTo Instance { get; } = new();
        }

        public sealed class IsLesserThanOrEqualTo : RelOperator<decimal, decimal>
        {
            private IsLesserThanOrEqualTo() : base("<=") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs <= rhs;
            public static IsLesserThanOrEqualTo Instance { get; } = new();
        }
    }
}
