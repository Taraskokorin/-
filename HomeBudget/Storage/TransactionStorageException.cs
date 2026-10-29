namespace HomeBudget.Storage;

/// <summary>Помилка читання чи запису журналу, зрозуміла людині.</summary>
public class TransactionStorageException : Exception
{
    public TransactionStorageException(string message, Exception inner)
        : base(message, inner) { }
}
