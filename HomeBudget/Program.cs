using System.Text;
using HomeBudget.Transactions;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Домашня бухгалтерія");

var transactions = new List<Transaction>
{
    new Transaction(1, "Продукти в АТБ", -642.50m)
        { Category = "Продукти", Account = "Картка" },
    new Transaction(2, "Стипендія", 2000m)
        { Category = "Дохід", Account = "Картка" },
    new Transaction(3, "Проїзд у маршрутці", -15m)
        { Category = "Транспорт", Account = "Готівка" },
    new Transaction(4, "Поповнення мобільного", -150m)
        { Category = "Зв’язок", Account = "Картка" }
};

Console.WriteLine($"Операцій у журналі: {transactions.Count}.");

// Перелік збирають у буфер і друкують одним викликом.
var list = new StringBuilder();

foreach (var transaction in transactions)
    list.AppendLine($"  {transaction}");

Console.Write(list.ToString());

foreach (var transaction in transactions)
{
    Console.WriteLine();
    Console.WriteLine(transaction.Description);
    Console.Write("Рахунок: ");
    var given = Console.ReadLine() ?? "";

    var effect = transaction.BalanceEffect(given);

    Console.WriteLine(effect != 0
        ? $"Баланс рахунку «{given.Trim()}» зміниться на {effect:+0.00;-0.00} грн."
        : $"Операція не стосується рахунку «{given.Trim()}». Її рахунок: {transaction.Account}.");
}
