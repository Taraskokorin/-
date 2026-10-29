using System.Globalization;
using HomeBudget.Transactions;

namespace HomeBudget.Transactions.Installment;

/// <summary>
/// Покупка частинами: повна ціна розбита на рівні щомісячні платежі,
/// і за місяць з рахунку знімають лише один платіж.
/// </summary>
[TransactionKind("installment", "Оплата частинами")]
public class InstallmentPurchase : Transaction
{
    public string Account { get; set; } = "Картка";

    /// <summary>На скільки місяців розбито покупку.</summary>
    public int Parts { get; set; } = 1;

    public InstallmentPurchase() { }

    public InstallmentPurchase(int id, string description, decimal amount,
                               string account, int parts)
        : base(id, description, amount)
    {
        Account = account;
        Parts = parts;
    }

    /// <summary>Щомісячний платіж, округлений до копійки.</summary>
    public decimal MonthlyPayment =>
        Math.Round(Amount / Math.Max(Parts, 1), 2, MidpointRounding.AwayFromZero);

    public override decimal BalanceEffect(string account) =>
        SameAccount(account, Account) ? -MonthlyPayment : 0;

    public override string ExpectedEffect =>
        $"-{MonthlyPayment:0.00} грн з рахунку «{Account}» (1 з {Parts} платежів)";

    public override IReadOnlyList<string> Accounts => new[] { Account };

    public override string Prompt => "З якого рахунку щомісячний платіж";

    public override void WriteData(IDictionary<string, string> data)
    {
        data["account"] = Account;
        data["parts"] = Parts.ToString(CultureInfo.InvariantCulture);
    }

    public override void ReadData(IReadOnlyDictionary<string, string> data)
    {
        Account = data.TryGetValue("account", out var account) ? account : "";
        Parts = data.TryGetValue("parts", out var text)
                && int.TryParse(text, NumberStyles.Integer,
                       CultureInfo.InvariantCulture, out var parts)
            ? parts
            : 1;
    }
}
