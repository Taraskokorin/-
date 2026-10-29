using HomeBudget.Transactions;

namespace HomeBudget.Storage;

public class SampleTransactionStorage : ITransactionStorage
{
    public List<Transaction> Load() => Transactions();

    /// <summary>Зразкові операції. Статичний метод: вони знадобляться
    /// й тому, хто до сховища не звертається.</summary>
    public static List<Transaction> Transactions() => new()
    {
        new Expense(1, "Продукти в АТБ", 642.50m, "Картка",
            "Хліб", "Молоко", "Сир", "Яблука")
            { Category = "Продукти", Date = new DateTime(2026, 10, 3) },
        new Income(2, "Стипендія", 2000m, "Картка", "ЖДУ ім. І. Франка")
            { Category = "Дохід", Date = new DateTime(2026, 10, 1) },
        new Expense(3, "Проїзд у маршрутці", 15m, "Готівка")
            { Category = "Транспорт", Date = new DateTime(2026, 10, 6) },
        new Transfer(4, "Зняв готівку в банкоматі", 500m, "Картка", "Готівка", 5m)
            { Category = "Перекази", Date = new DateTime(2026, 10, 2) },
        new Expense(5, "Поповнення мобільного", 150m, "Картка")
            { Category = "Зв’язок", Date = new DateTime(2026, 10, 5) }
    };
}
