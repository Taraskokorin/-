using System.Globalization;
using HomeBudget.Transactions;

namespace HomeBudget.Storage;

public static class TransactionFactory
{
    public static Transaction? Create(string kind) => kind switch
    {
        "expense" => new Expense(),
        "income" => new Income(),
        "transfer" => new Transfer(),
        _ => null
    };

    public static Transaction? FromRecord(TransactionRecord record)
    {
        var transaction = Create(record.Kind);

        if (transaction is null)
            return null;

        transaction.Id = record.Id;
        transaction.Description = record.Description;
        transaction.Category = record.Category;
        transaction.Amount = record.Amount;
        transaction.Date = DateTime.TryParse(record.Date, CultureInfo.InvariantCulture,
                               DateTimeStyles.RoundtripKind, out var date)
            ? date
            : DateTime.Today;
        transaction.ReadData(record.Data);

        return transaction;
    }

    public static TransactionRecord ToRecord(Transaction transaction)
    {
        var record = new TransactionRecord
        {
            Id = transaction.Id,
            Kind = transaction.Kind,
            Description = transaction.Description,
            Category = transaction.Category,
            Date = transaction.Date.ToString("O", CultureInfo.InvariantCulture),
            Amount = transaction.Amount
        };

        transaction.WriteData(record.Data);

        return record;
    }
}
