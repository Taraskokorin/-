using HomeBudget.Ledger;
using HomeBudget.Order;
using HomeBudget.Platform;
using HomeBudget.Storage;
using HomeBudget.Transactions;
using Spectre.Console;

Console.OutputEncoding = System.Text.Encoding.UTF8;

if (args.Contains("--info"))
{
    // Text друкує рядок як є: дужки в шляхах не сприймуться як розмітка.
    AnsiConsole.Write(new Panel(new Text(PlatformInfo.Describe()))
    {
        Header = new PanelHeader("Відомості про систему"),
        Border = BoxBorder.Rounded
    });

    return 0;
}

AnsiConsole.Write(new Rule(
    $"[bold]Домашня бухгалтерія[/] · {Markup.Escape(PlatformInfo.OsName)}")
{
    Justification = Justify.Left
});

var storage = new JsonTransactionStorage(BudgetLocation.DataFile);

try
{
    // Ознака першого запуску – відсутній файл, а не порожній перелік (ЛР 5).
    if (!File.Exists(storage.Path))
    {
        await storage.SaveAsync(SampleTransactionStorage.Transactions());
        AnsiConsole.MarkupLineInterpolated(
            $"[grey]Створено зразковий журнал: {storage.Path}[/]");
    }

    var session = new LedgerSession(storage, new ChronologicalOrder());
    var ordered = await session.TransactionsAsync();

    // Усі рахунки журналу – з них обирають стрілками.
    var accounts = ordered
        .SelectMany(t => t.Accounts)
        .Distinct(StringComparer.CurrentCultureIgnoreCase)
        .Order()
        .ToList();

    AnsiConsole.MarkupLineInterpolated(
        $"Операцій: [bold]{ordered.Count}[/], порядок: {session.OrderName}.");

    var start = AskStartBalance();

    for (var i = 0; i < ordered.Count; i++)
    {
        var transaction = ordered[i];

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Panel(new Text(Describe(transaction)))
        {
            Header = new PanelHeader(
                $"{i + 1} з {ordered.Count} · {Markup.Escape(transaction.Category)}"),
            Border = BoxBorder.Rounded,
            Expand = true
        });

        var given = Ask(transaction, accounts);

        // Вплив рахує ядро. Оболонка лише обирає колір.
        var effect = session.Apply(transaction, given);

        if (effect > 0)
            AnsiConsole.MarkupLineInterpolated($"[green]▲ {effect:+0.00} грн[/] на «{given.Trim()}»");
        else if (effect < 0)
            AnsiConsole.MarkupLineInterpolated($"[red]▼ {effect:0.00} грн[/] з «{given.Trim()}»");
        else
            AnsiConsole.MarkupLineInterpolated(
                $"[yellow]✗ Не стосується «{given.Trim()}».[/] Очікувано: [bold]{transaction.ExpectedEffect}[/]");
    }

    ShowSummary(session, start);
    return 0;
}
catch (TransactionStorageException ex)
{
    AnsiConsole.MarkupLineInterpolated($"[red]{ex.Message}[/]");
    AnsiConsole.MarkupLine("Виправте або видаліть файл журналу.");
    return 1;
}

// Опис операції для рамки; для витрати – ще й позиції чека.
string Describe(Transaction transaction)
{
    var text = $"{transaction.Date:dd.MM.yyyy}  {transaction.Description}: {transaction.Amount:0.00} грн";

    if (transaction is Expense expense)
        for (var n = 1; n <= expense.Items.Count; n++)
            text += $"{Environment.NewLine}  {n}. {expense[n]}";

    return text;
}

bool IsInteractive() =>
    AnsiConsole.Profile.Capabilities.Interactive && !Console.IsInputRedirected;

decimal AskStartBalance()
{
    const string question = "Початковий баланс, грн (Enter – 0)";
    string text;

    if (IsInteractive())
        text = AnsiConsole.Prompt(new TextPrompt<string>($"{question}:").AllowEmpty());
    else
    {
        AnsiConsole.Markup($"{question}: ");
        text = Console.ReadLine() ?? "";
    }

    if (text.Trim() == "")
        return 0;

    if (Money.TryParse(text, out var value))
        return value;

    AnsiConsole.MarkupLine("[yellow]Не вдалося прочитати суму, беру 0.[/]");
    return 0;
}

// Вибір рахунку стрілками – лише в інтерактивному терміналі з ANSI.
// Інакше друкуємо перелік і читаємо рядок.
string Ask(Transaction transaction, IReadOnlyList<string> accounts)
{
    var canSelect = IsInteractive() && AnsiConsole.Profile.Capabilities.Ansi;

    if (canSelect && accounts.Count > 0)
    {
        var account = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Markup.Escape(transaction.Prompt))
                .AddChoices(accounts)
                .UseConverter(a => Markup.Escape(a)));

        // Меню після вибору зникає – лишаємо слід, що саме обрано.
        AnsiConsole.MarkupLineInterpolated($"{transaction.Prompt}: [blue]{account}[/]");
        return account;
    }

    AnsiConsole.MarkupLineInterpolated($"[grey]Рахунки: {string.Join(", ", accounts)}[/]");

    if (!IsInteractive())
    {
        AnsiConsole.Markup($"{Markup.Escape(transaction.Prompt)}: ");
        return Console.ReadLine() ?? "";
    }

    var prompt = new TextPrompt<string>($"{Markup.Escape(transaction.Prompt)}:");
    return AnsiConsole.Prompt(prompt.AllowEmpty());
}

void ShowSummary(LedgerSession session, decimal start)
{
    var result = session.Summary;

    AnsiConsole.WriteLine();
    AnsiConsole.Write(new Rule("[bold]Підсумок[/]")
    {
        Justification = Justify.Left
    });

    AnsiConsole.MarkupLineInterpolated(
        $"Проведено {result.Applied} з {result.Count}: [green]+{result.Income:0.00}[/] / [red]-{result.Expense:0.00}[/] грн, баланс [bold]{start:0.00} → {start + result.Net:0.00}[/] грн");

    if (result.Skipped == 0)
        return;

    var attention = new Table { Border = TableBorder.Rounded };
    attention.AddColumn("Перевірити варто");
    attention.AddColumn("Очікуваний вплив");

    foreach (var transaction in session.NeedsAttention)
        attention.AddRow(
            new Text(transaction.Description),
            new Text(transaction.ExpectedEffect));

    AnsiConsole.WriteLine();
    AnsiConsole.Write(attention);
}
