namespace HomeBudget.Transactions;

/// <summary>Спільна основа всіх операцій: те, що є в кожної.</summary>
public abstract class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public string Category { get; set; } = "Інше";
    public DateTime Date { get; set; } = DateTime.Today;

    /// <summary>Сума операції, завжди додатна. Напрямок задає різновид.</summary>
    public decimal Amount { get; set; }

    protected Transaction() { }

    protected Transaction(int id, string description, decimal amount)
    {
        Id = id;
        Description = description;
        Amount = amount;
    }

    /// <summary>На скільки операція змінює баланс рахунку. Кожен різновид – по-своєму.</summary>
    public abstract decimal BalanceEffect(string account);

    /// <summary>Очікуваний вплив текстом – для показу людині.</summary>
    public abstract string ExpectedEffect { get; }

    /// <summary>Рахунки, яких стосується операція.</summary>
    public abstract IReadOnlyList<string> Accounts { get; }

    /// <summary>Підказка для введення. Різновид може змінити.</summary>
    public virtual string Prompt => "Рахунок";

    /// <summary>Позначка різновиду з атрибута [TransactionKind]: під нею операцію пишуть у файл.</summary>
    public virtual string Kind => TransactionKinds.Of(GetType());

    /// <summary>Записує власні дані різновиду до словника.</summary>
    public abstract void WriteData(IDictionary<string, string> data);

    /// <summary>Читає власні дані різновиду зі словника.</summary>
    public abstract void ReadData(IReadOnlyDictionary<string, string> data);

    /// <summary>Роздільник ";" не може трапитися всередині значення,
    /// інакше під час читання частин стане більше, ніж було.</summary>
    protected static string JoinParts(IEnumerable<string> parts)
    {
        foreach (var part in parts)
            if (part.Contains(';'))
                throw new InvalidOperationException($"Значення «{part}» містить ';'.");

        return string.Join(";", parts);
    }

    protected static string Read(IReadOnlyDictionary<string, string> data, string key) =>
        data.TryGetValue(key, out var value) ? value : "";

    protected static bool SameAccount(string given, string account) =>
        string.Equals(given.Trim(), account.Trim(),
            StringComparison.CurrentCultureIgnoreCase);

    public override string ToString() =>
        $"#{Id} {Date:dd.MM} [{Category}] {Description}: {Amount:0.00} грн";
}
