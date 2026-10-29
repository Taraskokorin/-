using HomeBudget.Storage;
using HomeBudget.Transactions;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var file = args.Length > 0 ? args[0] : "transactions.json";

if (!File.Exists(file))
{
    Console.WriteLine($"Файлу журналу немає: {file}");
    return 1;
}

var transactions = await new JsonTransactionStorage(file).LoadAsync();

Console.WriteLine($"Операцій: {transactions.Count}");

// Витрати за категоріями – від найбільшої суми.
var expenses = transactions.OfType<Expense>()
    .GroupBy(t => t.Category)
    .OrderByDescending(g => g.Sum(t => t.Amount));

foreach (var group in expenses)
    Console.WriteLine($"  {group.Key}: {group.Sum(t => t.Amount):0.00} грн");

var income = transactions.OfType<Income>().Sum(t => t.Amount);
Console.WriteLine($"Надходження: {income:0.00} грн");

return 0;
