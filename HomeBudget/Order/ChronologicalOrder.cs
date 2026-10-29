using HomeBudget.Transactions;

namespace HomeBudget.Order;

/// <summary>Спершу найдавніші операції.</summary>
public class ChronologicalOrder : ITransactionOrder
{
    public string Name => "за датою";

    // OrderBy стійкий: операції з однаковою датою лишаються в порядку журналу.
    public List<Transaction> Order(List<Transaction> transactions) =>
        transactions.OrderBy(t => t.Date).ToList();
}
