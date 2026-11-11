namespace Lec03LibN
{
    public interface IFactory
    {
        IBonus getA(            // тип вознаграждения A
            float cost1hour     // стоимость 1 часа
            );
        IBonus getB(            // тип вознаграждения B
            float cost1hour,    // стоимость 1 часа
            float x             // повышающий/понижающий коэффициент
        );
        IBonus getC(            // тип вознаграждения C
            float cost1hour,    // стоимость 1 часа
            float x,            // повышающий/понижающий коэффициент
            float y             // величина снижения/повышения
        );
    }
}
