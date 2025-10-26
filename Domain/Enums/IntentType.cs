namespace Domain.Enums;

/// <summary>Намерение автора объявления по отношению к объекту недвижимости.</summary>
public enum IntentType
{
    Unknown = 0, // не удалось определить
    Sell = 1, // продам
    Buy = 2, // куплю
    RentOut = 3, // сдам
    RentWant = 4 // сниму/ищу аренду
}