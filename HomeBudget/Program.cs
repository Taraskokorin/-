using HomeBudget.Ledger;
using HomeBudget.Order;
using HomeBudget.Storage;
using HomeBudget.Transactions;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Домашня бухгалтерія");

// Тут і тільки тут вирішують, з чого складається облік.
ITransactionStorage storage = new SampleTransactionStorage();
ITransactionOrder order = new ChronologicalOrder();

var session = new LedgerSession(storage, order);
var transactions = session.Transactions();

Console.WriteLine($"Операцій: {transactions.Count}, порядок: {session.OrderName}.");

Console.Write("Початковий баланс, грн (Enter – 0): ");
var startText = Console.ReadLine() ?? "";
var start = 0m;

if (startText.Trim() != "" && !Money.TryParse(startText, out start))
    Console.WriteLine("Не вдалося прочитати суму, беру 0.");

foreach (var transaction in transactions)
{
    Console.WriteLine();
    Console.WriteLine(transaction);

    if (transaction is Expense expense)
        for (var n = 1; n <= expense.Items.Count; n++)
            Console.WriteLine($"  {n}. {expense[n]}");

    Console.Write($"{transaction.Prompt}: ");
    var given = Console.ReadLine() ?? "";
    var effect = session.Apply(transaction, given);

    Console.WriteLine(effect != 0
        ? $"Баланс «{given.Trim()}» зміниться на {effect:+0.00;-0.00} грн."
        : $"Не стосується «{given.Trim()}». Очікувано: {transaction.ExpectedEffect}.");
}

var summary = session.Summary;

Console.WriteLine();
Console.WriteLine(
    $"Підсумок: проведено {summary.Applied} з {summary.Count}, " +
    $"надійшло {summary.Income:0.00}, витрачено {summary.Expense:0.00}, " +
    $"разом {summary.Net:+0.00;-0.00} грн.");
Console.WriteLine($"Баланс на кінець: {start + summary.Net:0.00} грн.");

if (summary.Skipped > 0)
{
    Console.WriteLine("Перевірити варто:");

    foreach (var transaction in session.NeedsAttention)
        Console.WriteLine($"  {transaction.Description} – {transaction.ExpectedEffect}");
}
