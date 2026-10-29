namespace HomeBudget.Transactions;

/// <summary>Одна фінансова операція: опис, сума, категорія й рахунок.</summary>
public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = "";

    /// <summary>Сума зі знаком: від’ємна – витрата, додатна – надходження.</summary>
    public decimal Amount { get; set; }

    public string Category { get; set; } = "Інше";
    public string Account { get; set; } = "Готівка";

    public Transaction() { }

    public Transaction(int id, string description, decimal amount)
    {
        Id = id;
        Description = description;
        Amount = amount;
    }

    /// <summary>На скільки операція змінює баланс названого рахунку.</summary>
    public decimal BalanceEffect(string account) =>
        string.Equals(account.Trim(), Account.Trim(),
            StringComparison.CurrentCultureIgnoreCase)
            ? Amount
            : 0;

    public override string ToString() =>
        $"#{Id} [{Category}] {Description}: {Amount:+0.00;-0.00} грн ({Account})";
}
