using HomeBudget.Transactions;

namespace HomeBudget.Storage;

/// <summary>Звідки беруться операції: файл чи пам’ять – ядро не знає.</summary>
public interface ITransactionStorage
{
    List<Transaction> Load();
}
