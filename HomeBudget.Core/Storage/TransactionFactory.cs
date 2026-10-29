using System.Globalization;
using HomeBudget.Transactions;

namespace HomeBudget.Storage;

internal class TransactionFactory
{
    private readonly TransactionCatalog catalog;

    public TransactionFactory(TransactionCatalog catalog) => this.catalog = catalog;

    // перестав бути static: тепер він користується полем catalog
    public Transaction? FromRecord(TransactionRecord record)
    {
        // Замість switch – запит до каталогу.
        var transaction = catalog.Create(record.Kind);

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
