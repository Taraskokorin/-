namespace HomeBudget.Transactions;

/// <summary>Надходження: гроші приходять на один рахунок.</summary>
[TransactionKind("income", "Надходження")]
public class Income : Transaction
{
    public string Account { get; set; } = "Картка";

    /// <summary>Від кого надійшли гроші.</summary>
    public string Source { get; set; } = "";

    public Income() { }

    public Income(int id, string description, decimal amount, string account)
        : base(id, description, amount)
    {
        Account = account;
    }

    public Income(int id, string description, decimal amount, string account,
                  string source)
        : this(id, description, amount, account)
    {
        Source = source;
    }

    public override decimal BalanceEffect(string account) =>
        SameAccount(account, Account) ? Amount : 0;

    public override string ExpectedEffect => $"+{Amount:0.00} грн на рахунок «{Account}»";

    public override IReadOnlyList<string> Accounts => new[] { Account };

    public override string Prompt => "На який рахунок надійшло";

    public override void WriteData(IDictionary<string, string> data)
    {
        data["account"] = Account;

        if (!string.IsNullOrEmpty(Source))
            data["source"] = Source;
    }

    public override void ReadData(IReadOnlyDictionary<string, string> data)
    {
        Account = Read(data, "account");
        Source = Read(data, "source");
    }
}
