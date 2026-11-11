namespace Lec03LibN
{
    internal class FactoryLevel2 : IFactory
    {
        private float a;
        public FactoryLevel2(float a) 
        {
            this.a = a;
        }

        internal class  BonusA2 : IBonus
        {
            public float cost1hour {  get; set; }
            private float a;
            
            public BonusA2(float cost1hour,  float a)
            {
                this.cost1hour = cost1hour;
                this.a = a;
            }
            public float calc(float numbers_hours)
            {
                return (numbers_hours + a) * cost1hour;
            }
        }

        internal class BonusB2 : IBonus
        {
            public float cost1hour { get; set; }
            private float a;
            private float x;

            public BonusB2(float cost1hour, float x, float a)
            {
                this.cost1hour = cost1hour;
                this.x = x;
                this.a = a;
            }
            public float calc(float numbers_hours)
            {
                return (numbers_hours + a) * cost1hour * x;
            }
        }

        internal class BonusC2 : IBonus
        {
            public float cost1hour { get; set; }
            private float a;
            private float x;
            private float y;

            public BonusC2(float cost1hour, float a, float x, float y)
            {
                this.cost1hour = cost1hour;
                this.a = a;
                this.x = x;
                this.y = y;
            }

            public float calc(float numbers_hours)
            {
                return (numbers_hours + a) * cost1hour * x + y;
            }
        }

        public IBonus getA(float cost1hour)
        {
            return new BonusA2(cost1hour, a);
        }
        public IBonus getB(float cost1hour, float x)
        {
            return new BonusB2(cost1hour, x, a);
        }
        public IBonus getC(float cost1hour, float x, float y)
        {
            return new BonusC2(cost1hour, x, y, a);
        }
    }
}
