using HomeBudget.Transactions;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Домашня бухгалтерія");

var transactions = new List<Transaction>
{
    new Transaction(1, "Продукти в АТБ", 642.50m) { Account = "Картка" },
    new Transaction(2, "Стипендія", 2000m) { Account = "Картка" },
    new Transaction(3, "Проїзд у маршрутці", 15m) { Account = "Готівка" },
    new Transaction(4, "Поповнення мобільного", 150m) { Account = "Картка" }
};

Console.WriteLine($"У журналі {transactions.Count} операції.");

while (true)
{
    Console.WriteLine();
    Console.Write("Назва рахунку (порожній рядок — вихід): ");
    var given = Console.ReadLine() ?? "";
    if (given.Trim() == "")
        break;

    var found = transactions.Where(t => t.BelongsTo(given)).ToList();
    Console.WriteLine(found.Count > 0
        ? $"Операцій за рахунком: {found.Count}"
        : $"Рахунку «{given}» у журналі немає.");
    foreach (var transaction in found)
        Console.WriteLine(transaction);
}
