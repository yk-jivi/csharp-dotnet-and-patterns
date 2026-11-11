using System;

namespace BinaryCalculator
{
    public class Calculator
    {
        public delegate int BinaryOperation(int a, int b);
        public delegate int UnaryOperation(int a);

        public event BinaryOperation OnBinaryOperation;
        public event UnaryOperation OnUnaryOperation;

        public int ExecuteBinary(int a, int b)
        {
            return OnBinaryOperation?.Invoke(a, b) ?? 0;
        }

        public int ExecuteUnary(int a)
        {
            return OnUnaryOperation?.Invoke(a) ?? 0;
        }

        public int And(int a, int b) => a & b;
        public int Or(int a, int b) => a | b;
        public int Xor(int a, int b) => a ^ b;
        public int Not(int a) => ~a;
    }
}
