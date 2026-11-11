namespace Lec03LibN
{
    internal class BonusA : IBonus
    {
        public float cost1hour { get; set; }
        public BonusA(float cost1hour)
        {
            this.cost1hour = cost1hour;
        }
        public float calc(float number_hours)
        {
            return number_hours * cost1hour;
        }
    }
}
