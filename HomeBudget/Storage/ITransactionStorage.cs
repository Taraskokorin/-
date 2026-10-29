using HomeBudget.Transactions;

namespace HomeBudget.Storage;

/// <summary>Звідки беруться операції: файл чи пам’ять – ядро не знає.</summary>
public interface ITransactionStorage
{
    Task<List<Transaction>> LoadAsync();
    Task SaveAsync(List<Transaction> transactions);
}
