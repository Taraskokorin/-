namespace HomeBudget.Transactions;

/// <summary>Витрата: гроші йдуть з одного рахунку.</summary>
[TransactionKind("expense", "Витрата")]
public class Expense : Transaction
{
    public string Account { get; set; } = "Готівка";

    /// <summary>Позиції чека – що саме купили.</summary>
    public List<string> Items { get; set; } = new();

    /// <summary>Позиція чека за номером, починаючи з одиниці.</summary>
    public string this[int number] => Items[number - 1];

    public Expense() { }

    public Expense(int id, string description, decimal amount, string account)
        : base(id, description, amount)
    {
        Account = account;
    }

    public Expense(int id, string description, decimal amount, string account,
                   params string[] items)
        : this(id, description, amount, account)
    {
        Items.AddRange(items);
    }

    public override decimal BalanceEffect(string account) =>
        SameAccount(account, Account) ? -Amount : 0;

    public override string ExpectedEffect => $"-{Amount:0.00} грн з рахунку «{Account}»";

    public override IReadOnlyList<string> Accounts => new[] { Account };

    public override string Prompt => "З якого рахунку сплачено";

    public override void WriteData(IDictionary<string, string> data)
    {
        data["account"] = Account;

        if (Items.Count > 0)
            data["items"] = JoinParts(Items);
    }

    public override void ReadData(IReadOnlyDictionary<string, string> data)
    {
        Account = Read(data, "account");

        Items.Clear();

        if (data.TryGetValue("items", out var items))
            Items.AddRange(items.Split(';', StringSplitOptions.RemoveEmptyEntries));
    }
}
