using System.Text.Encodings.Web;
using System.Text.Json;
using HomeBudget.Transactions;

namespace HomeBudget.Storage;

public class JsonTransactionStorage : ITransactionStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        // Без цього кирилиця у файлі перетвориться на АБ…
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly string path;
    private readonly TransactionFactory factory;

    /// <summary>Сховище з різновидами, які знає ядро.</summary>
    public JsonTransactionStorage(string path) : this(path, TransactionCatalog.Default) { }

    /// <summary>Сховище з власним каталогом різновидів.</summary>
    public JsonTransactionStorage(string path, TransactionCatalog catalog)
    {
        this.path = path;
        factory = new TransactionFactory(catalog);
    }

    public string Path => path;

    public async Task<List<Transaction>> LoadAsync()
    {
        if (!File.Exists(path))
            return new List<Transaction>();

        string json;

        try
        {
            json = await File.ReadAllTextAsync(path);
        }
        catch (IOException ex)
        {
            throw new TransactionStorageException(
                $"Не вдалося прочитати файл {path}.", ex);
        }

        List<TransactionRecord>? records;

        try
        {
            records = JsonSerializer.Deserialize<List<TransactionRecord>>(json, Options);
        }
        catch (JsonException ex)
        {
            throw new TransactionStorageException(
                $"Файл {path} пошкоджено: це не коректний JSON.", ex);
        }

        var transactions = new List<Transaction>();

        foreach (var record in records ?? new List<TransactionRecord>())
        {
            var transaction = factory.FromRecord(record);

            // Запис невідомого різновиду пропускають, решту читають.
            if (transaction is not null)
                transactions.Add(transaction);
        }

        return transactions;
    }

    public async Task SaveAsync(List<Transaction> transactions)
    {
        var records = new List<TransactionRecord>();

        foreach (var transaction in transactions)
            records.Add(TransactionFactory.ToRecord(transaction));

        var json = JsonSerializer.Serialize(records, Options);

        try
        {
            // System.IO.Path повністю: у класі є власна властивість Path.
            var folder = System.IO.Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(folder))
                Directory.CreateDirectory(folder);

            await File.WriteAllTextAsync(path, json);
        }
        catch (Exception ex)
            when (ex is IOException or UnauthorizedAccessException)
        {
            throw new TransactionStorageException(
                $"Не вдалося записати файл {path}.", ex);
        }
    }
}
