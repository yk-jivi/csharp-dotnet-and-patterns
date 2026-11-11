//DP001_lib
//Class1.cs
using System;
using System.Collections.Generic;

namespace DP001_lib
{
    public partial class Calc
    {
        public static Builder getBuilder()
        {
            return new Builder();
        }
        public class Builder
        {
            private List<Func<int, int>> _operations = new List<Func<int, int>>();

            public Builder Sum(int value)
            {
                _operations.Add(x => x + value);
                return this;
            }

            public Builder Sub(int value)
            {
                _operations.Add(x => x - value);
                return this;
            }

            public Builder Mul(int value)
            {
                _operations.Add(x => x * value);
                return this;
            }

            public Builder Div(int value)
            {
                _operations.Add(x => x / value);
                return this;
            }

            public Builder Mod(int value)
            {
                _operations.Add(x => x % value);
                return this;
            }

            public ICalcResult Build()
            {
                return new CalcResult(_operations);
            }
        }
        public int result { get; private set; } = 0;
        public Calc(int result = 0)
        {
            this.result = result;
        }
        public int sum(int x, int y) { return x + y; }
        public int sub(int x, int y) { return x - y; }
        public int mul(int x, int y) { return x * y; }
        public int div(int x, int y) { return x / y; }
        public int mod(int x, int y) { return x % y; }
        public Calc sum(int y)
        {
            this.result = this.sum(this.result, y);
            return this;
        }
        public Calc sub(int y)
        {
            this.result = this.sub(this.result, y);
            return this;
        }
        public Calc mul(int y)
        {
            this.result = this.mul(this.result, y);
            return this;
        }
        public Calc div(int y)
        {
            this.result = this.div(this.result, y);
            return this;
        }
        public Calc mod(int y)
        {
            this.result = this.mod(this.result, y);
            return this;
        }
        public interface ICalcResult
        {
            int result(int y);
        }
        private class CalcResult : ICalcResult
        {
            private List<Func<int, int>> _operations;

            public CalcResult(List<Func<int, int>> operations)
            {
                _operations = operations;
            }

            public int result(int y)
            {
                int current = y;
                foreach (var operation in _operations)
                {
                    current = operation(current);
                }
                return current;
            }
        }
    }
}