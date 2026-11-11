namespace Laba_Four;

abstract partial class EarthPart
{
    //Переопределение ToString()
    public override string ToString()
    {
        return "Тип: " + GetType().Name + "Название: " + Name;
    }
}