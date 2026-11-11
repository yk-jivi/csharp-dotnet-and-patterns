namespace Lec03LibN
{
    public interface IBonus              // вознаграждение
    {
        float cost1hour { get; set; }    // стоимость одного часа
        float calc(                      // вычисление вознаграждения
            float number_hours           // - количество отработанных часов
            );   
    }
}
