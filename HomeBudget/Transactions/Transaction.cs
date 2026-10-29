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

    protected static bool SameAccount(string given, string account) =>
        string.Equals(given.Trim(), account.Trim(),
            StringComparison.CurrentCultureIgnoreCase);

    public override string ToString() =>
        $"#{Id} {Date:dd.MM} [{Category}] {Description}: {Amount:0.00} грн";
}
