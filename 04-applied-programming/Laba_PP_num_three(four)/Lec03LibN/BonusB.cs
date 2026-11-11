namespace Lec03LibN
{
    internal class BonusB : IBonus
    {
        public float cost1hour { get; set; }
        private float x;
        public BonusB(float cost1hour, float x)
        {
            this.cost1hour = cost1hour;
            this.x = x;
        }
        public float calc(float number_hours)
        {
            return number_hours * cost1hour * x;
        }
    }
}
