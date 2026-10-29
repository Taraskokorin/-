using HomeBudget.Order;
using HomeBudget.Storage;
using HomeBudget.Transactions;

namespace HomeBudget.Ledger;

/// <summary>
/// Ядро обліку. Не знає ані про консоль, ані про файли:
/// звідки брати операції й у якому порядку – вирішують ті, кого йому передали.
/// </summary>
public class LedgerSession
{
    private readonly ITransactionStorage storage;
    private readonly ITransactionOrder order;
    private readonly List<Transaction> needsAttention = new();

    private BalanceSummary summary = new(0, 0, 0, 0);

    public LedgerSession(ITransactionStorage storage, ITransactionOrder order)
    {
        this.storage = storage;
        this.order = order;
    }

    public string OrderName => order.Name;

    /// <summary>Операції в порядку, який задала стратегія.</summary>
    public List<Transaction> Transactions() => order.Order(storage.Load());

    /// <summary>Проводить операцію за названим рахунком і повертає вплив на баланс.</summary>
    public decimal Apply(Transaction transaction, string account)
    {
        var effect = transaction.BalanceEffect(account);

        summary += effect;

        // Рахунок не той – операцію не проведено, її варто переглянути.
        if (effect == 0)
            needsAttention.Add(transaction);

        return effect;
    }

    public IReadOnlyList<Transaction> NeedsAttention => needsAttention;

    public BalanceSummary Summary => summary;
}
