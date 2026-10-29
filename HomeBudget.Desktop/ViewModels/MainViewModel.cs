using System.Collections.ObjectModel;
using System.ComponentModel;
using HomeBudget.Ledger;
using HomeBudget.Order;
using HomeBudget.Plugins;
using HomeBudget.Storage;
using HomeBudget.Transactions;

namespace HomeBudget.Desktop.ViewModels;

public class MainViewModel : ObservableBase
{
    private readonly string dataFile;

    private LedgerSession? session;
    private IReadOnlyList<Transaction> transactions = Array.Empty<Transaction>();
    private IReadOnlyList<string> accounts = Array.Empty<string>();
    private int index = -1;

    private string status = "Читаю журнал…";
    private string description = "";
    private string details = "";
    private string feedback = "";
    private string summary = "";
    private bool hasTransaction;
    private bool answered;
    private AccountInput? input;
    private decimal? lastEffect;
    private int done;

    public MainViewModel(string dataFile)
    {
        this.dataFile = dataFile;

        ApplyCommand = new RelayCommand(Apply,
            () => HasTransaction && !Answered && Input is { IsReady: true });
        NextCommand = new RelayCommand(Next, () => HasTransaction && Answered);
    }

    public string Status { get => status; private set => Set(ref status, value); }

    public string Description { get => description; private set => Set(ref description, value); }

    /// <summary>Дата, сума й позиції чека – усе, що допомагає згадати операцію.</summary>
    public string Details { get => details; private set => Set(ref details, value); }

    public string Feedback { get => feedback; private set => Set(ref feedback, value); }

    public string Summary { get => summary; private set => Set(ref summary, value); }

    /// <summary>Проведені операції – перелік праворуч.</summary>
    public ObservableCollection<string> Journal { get; } = new();

    public bool HasTransaction
    {
        get => hasTransaction;
        private set { if (Set(ref hasTransaction, value)) RefreshCommands(); }
    }

    public bool Answered
    {
        get => answered;
        private set { if (Set(ref answered, value)) RefreshCommands(); }
    }

    public AccountInput? Input
    {
        get => input;
        private set
        {
            var old = input;

            if (!Set(ref input, value))
                return;

            if (old is not null)
                old.PropertyChanged -= OnInputChanged;

            if (value is not null)
                value.PropertyChanged += OnInputChanged;

            RefreshCommands();
        }
    }

    // Усі три ознаки обчислюються з lastEffect – сповіщаємо про кожну.
    public bool IsIncome => lastEffect > 0;
    public bool IsExpense => lastEffect < 0;
    public bool IsSkipped => lastEffect == 0;

    public int Total => transactions.Count;
    public int Done { get => done; private set => Set(ref done, value); }

    public RelayCommand ApplyCommand { get; }
    public RelayCommand NextCommand { get; }

    // Щось обрали чи набрали – «Провести» має перечитати CanExecute.
    private void OnInputChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AccountInput.IsReady))
            ApplyCommand.RaiseCanExecuteChanged();
    }

    private void RefreshCommands()
    {
        ApplyCommand.RaiseCanExecuteChanged();
        NextCommand.RaiseCanExecuteChanged();
    }

    private void SetResult(decimal? effect)
    {
        lastEffect = effect;
        Raise(nameof(IsIncome));
        Raise(nameof(IsExpense));
        Raise(nameof(IsSkipped));
    }

    // Читає журнал. Викликають після того, як вікно вже показано.
    public async Task LoadAsync()
    {
        try
        {
            PluginLoader.LoadInto(TransactionCatalog.Default);

            var storage = new JsonTransactionStorage(dataFile);

            // Ознака першого запуску – відсутній файл, а не порожній перелік.
            if (!File.Exists(dataFile))
                await storage.SaveAsync(SampleTransactionStorage.Transactions());

            session = new LedgerSession(storage, new ChronologicalOrder());
            transactions = await session.TransactionsAsync();
            accounts = transactions
                .SelectMany(t => t.Accounts)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .Order()
                .ToList();
            Raise(nameof(Total));

            Next();
        }
        catch (TransactionStorageException ex)
        {
            Fail(ex.Message, "Виправте або видаліть файл журналу.");
        }
        catch (Exception ex)
        {
            Fail("Не вдалося підготувати облік.", ex.Message);
        }
    }

    private void Apply()
    {
        if (session is null || Input is null)
            return;

        var transaction = transactions[index];
        var account = Input.Given;

        // Увесь облік – у ядрі. Тут лише запис результату до стану вікна.
        var effect = session.Apply(transaction, account);

        SetResult(effect);
        Done = index + 1;

        Feedback = effect != 0
            ? $"Баланс «{account}» зміниться на {effect:+0.00;-0.00} грн."
            : $"Не стосується «{account}». Очікувано: {transaction.ExpectedEffect}.";

        if (effect != 0)
            Journal.Add($"{transaction.Description}: {effect:+0.00;-0.00} ({account})");

        Answered = true;
    }

    private void Next()
    {
        index++;

        SetResult(null);
        Feedback = "";
        Answered = false;

        if (index >= transactions.Count)
        {
            Finish();
            return;
        }

        var transaction = transactions[index];

        HasTransaction = true;
        Status = $"Операція {index + 1} з {transactions.Count} · {transaction.Category}";
        Description = transaction.Description;
        Details = DetailsOf(transaction);
        Input = AccountInput.For(transaction, accounts);
    }

    private static string DetailsOf(Transaction transaction)
    {
        var text = $"{transaction.Date:dd.MM.yyyy} · {transaction.Amount:0.00} грн";

        if (transaction is Expense { Items.Count: > 0 } expense)
            text += " · " + string.Join(", ", expense.Items);

        return text;
    }

    private void Finish()
    {
        HasTransaction = false;
        Input = null;
        Status = "Облік завершено";

        if (session is null)
            return;

        var result = session.Summary;

        Summary = $"Проведено {result.Applied} з {result.Count}: " +
                  $"надійшло {result.Income:0.00}, витрачено {result.Expense:0.00}, " +
                  $"разом {result.Net:+0.00;-0.00} грн.";
    }

    private void Fail(string reason, string detail)
    {
        HasTransaction = false;
        Status = reason;
        Feedback = detail;
    }
}
