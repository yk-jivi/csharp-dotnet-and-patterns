namespace Lec03LibN
{
    static public partial class Lec03BLib
    {
        static public IFactory getL1()
        {
            return new FactoryLevel1();
        }
        static public IFactory getL2(float a)
        {
            return new FactoryLevel2(a);
        }
        static public IFactory getL3(float a, float b)
        {
            return new FactoryLevel3(a, b);
        }
    }
}
