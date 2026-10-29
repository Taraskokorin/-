namespace HomeBudget.Transactions;

/// <summary>Переказ між власними рахунками: з одного знімають, на інший зараховують.</summary>
public class Transfer : Transaction
{
    public string FromAccount { get; set; } = "";
    public string ToAccount { get; set; } = "";

    /// <summary>Комісія банку; її знімають з рахунку-джерела.</summary>
    public decimal Fee { get; set; }

    public Transfer() { }

    public Transfer(int id, string description, decimal amount,
                    string fromAccount, string toAccount)
        : base(id, description, amount)
    {
        FromAccount = fromAccount;
        ToAccount = toAccount;
    }

    public Transfer(int id, string description, decimal amount,
                    string fromAccount, string toAccount, decimal fee)
        : this(id, description, amount, fromAccount, toAccount)
    {
        Fee = fee;
    }

    public override decimal BalanceEffect(string account)
    {
        if (SameAccount(account, FromAccount))
            return -(Amount + Fee);

        if (SameAccount(account, ToAccount))
            return Amount;

        return 0;
    }

    public override string ExpectedEffect
    {
        get
        {
            var from = $"-{Amount + Fee:0.00} грн з «{FromAccount}»";
            var to = $"+{Amount:0.00} грн на «{ToAccount}»";

            return Fee > 0
                ? $"{from} (разом із комісією {Fee:0.00}), {to}"
                : $"{from}, {to}";
        }
    }

    public override IReadOnlyList<string> Accounts => new[] { FromAccount, ToAccount };

    public override string Prompt => "Рахунок (звідки чи куди)";
}
