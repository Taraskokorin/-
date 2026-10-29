using HomeBudget.Ledger;
using HomeBudget.Transactions;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Домашня бухгалтерія");

var transactions = new List<Transaction>
{
    new Expense(1, "Продукти в АТБ", 642.50m, "Картка",
        "Хліб", "Молоко", "Сир", "Яблука") { Category = "Продукти" },
    new Income(2, "Стипендія", 2000m, "Картка", "ЖДУ ім. І. Франка")
        { Category = "Дохід" },
    new Expense(3, "Проїзд у маршрутці", 15m, "Готівка")
        { Category = "Транспорт" },
    new Transfer(4, "Зняв готівку в банкоматі", 500m, "Картка", "Готівка", 5m)
        { Category = "Перекази" }
};

Console.WriteLine($"Операцій у журналі: {transactions.Count}.");

Console.Write("Початковий баланс, грн (Enter – 0): ");
var startText = Console.ReadLine() ?? "";
var start = 0m;

if (startText.Trim() != "" && !Money.TryParse(startText, out start))
    Console.WriteLine("Не вдалося прочитати суму, беру 0.");

var summary = new BalanceSummary(0, 0, 0, 0);

foreach (var transaction in transactions)
{
    Console.WriteLine();
    Console.WriteLine(transaction);

    if (transaction is Expense expense)
        for (var n = 1; n <= expense.Items.Count; n++)
            Console.WriteLine($"  {n}. {expense[n]}");

    Console.Write($"{transaction.Prompt}: ");
    var given = Console.ReadLine() ?? "";
    var effect = transaction.BalanceEffect(given);

    summary += effect;

    Console.WriteLine(effect != 0
        ? $"Баланс «{given.Trim()}» зміниться на {effect:+0.00;-0.00} грн."
        : $"Не стосується «{given.Trim()}». Очікувано: {transaction.ExpectedEffect}.");
}

Console.WriteLine();
Console.WriteLine(
    $"Підсумок: проведено {summary.Applied} з {summary.Count}, " +
    $"надійшло {summary.Income:0.00}, витрачено {summary.Expense:0.00} грн.");
Console.WriteLine(
    $"Баланс: {start:0.00} {summary.Net:+0.00;-0.00} = {start + summary.Net:0.00} грн.");
