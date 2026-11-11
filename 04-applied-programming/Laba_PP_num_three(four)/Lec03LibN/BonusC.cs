namespace Lec03LibN
{
    internal class BonusC : IBonus
    {
        public float cost1hour { get; set; }
        private float x;
        private float y;
        public BonusC(float cost1hour, float x, float y)
        {
            this.cost1hour = cost1hour;
            this.x = x;
            this.y = y;
        }
        public float calc(float number_hours)
        {
            return number_hours * cost1hour * x + y;
        }
    }
}
