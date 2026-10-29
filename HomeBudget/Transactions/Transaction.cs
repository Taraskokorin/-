namespace HomeBudget.Transactions;

/// <summary>Одна фінансова операція: що, скільки й з якого рахунку.</summary>
public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public string Account { get; set; } = "Готівка";

    public Transaction() { }

    public Transaction(int id, string description, decimal amount)
    {
        Id = id;
        Description = description;
        Amount = amount;
    }

    public bool BelongsTo(string account) =>
        string.Equals(account.Trim(), Account.Trim(),
            StringComparison.CurrentCultureIgnoreCase);

    public override string ToString() => $"#{Id} [{Account}] {Description}: {Amount:0.00} грн";
}
