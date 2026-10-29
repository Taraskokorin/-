using HomeBudget.Ledger;
using HomeBudget.Order;
using HomeBudget.Storage;
using HomeBudget.Transactions;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Домашня бухгалтерія");

// Path.Combine складає шлях із правильним роздільником:
// \ у Windows, / у Linux і macOS.
var file = Path.Combine(AppContext.BaseDirectory, "data", "transactions.json");
var storage = new JsonTransactionStorage(file);

try
{
    // Ознака першого запуску – відсутній файл, а не порожній перелік:
    // порожнім він буває й тоді, коли в журналі лише невідомі різновиди.
    if (!File.Exists(storage.Path))
    {
        await storage.SaveAsync(SampleTransactionStorage.Transactions());
        Console.WriteLine($"Створено зразковий журнал: {storage.Path}");
    }

    var session = new LedgerSession(storage, new ChronologicalOrder());
    var transactions = await session.TransactionsAsync();

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
}
catch (TransactionStorageException ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine("Виправте або видаліть файл журналу й запустіть ще раз.");
}
