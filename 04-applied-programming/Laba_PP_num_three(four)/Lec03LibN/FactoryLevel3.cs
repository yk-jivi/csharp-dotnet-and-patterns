namespace Lec03LibN
{
    internal class FactoryLevel3 : IFactory
    {
        private float a;
        private float b;
        public FactoryLevel3(float a, float b)
        {
            this.a = a;
            this.b = b;
        }
        internal class BonusA3 : IBonus
        {
            public float cost1hour { get; set; }
            private float a;
            private float b;

            public BonusA3(float cost1hour, float a, float b)
            {
                this.cost1hour = cost1hour;
                this.a = a;
                this.b = b;
            }

            public float calc(float number_hours)
            {
                return (number_hours + a) * (cost1hour + b);
            }
        }

        internal class BonusB3 : IBonus
        {
            public float cost1hour { get; set; }
            private float x;
            private float a;
            private float b;

            public BonusB3(float cost1hour, float x, float a, float b)
            {
                this.cost1hour = cost1hour;
                this.x = x;
                this.a = a;
                this.b = b;
            }

            public float calc(float number_hours)
            {
                return (number_hours + a) * (cost1hour + b) * x;
            }
        }

        internal class BonusC3 : IBonus
        {
            public float cost1hour { get; set; }
            private float x;
            private float y;
            private float a;
            private float b;

            public BonusC3(float cost1hour, float x, float y, float a, float b)
            {
                this.cost1hour = cost1hour;
                this.x = x;
                this.y = y;
                this.a = a;
                this.b = b;
            }
            public float calc(float number_hours)
            {
                return (number_hours + a) * (cost1hour + b) * x + y;
            }
        }
        public IBonus getA(float cost1hour)
        {
            return new BonusA3(cost1hour, a, b);
        }

        public IBonus getB(float cost1hour, float x)
        {
            return new BonusB3(cost1hour, x, a, b);
        }

        public IBonus getC(float cost1hour, float x, float y)
        {
            return new BonusC3(cost1hour, x, y, a, b);
        }
    }
}
