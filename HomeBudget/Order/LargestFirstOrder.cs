using HomeBudget.Transactions;

namespace HomeBudget.Order;

/// <summary>Спершу найбільші суми – те, що найсильніше змінює баланс.</summary>
public class LargestFirstOrder : ITransactionOrder
{
    public string Name => "спершу найбільші";

    public List<Transaction> Order(List<Transaction> transactions) =>
        transactions.OrderByDescending(t => t.Amount).ToList();
}
