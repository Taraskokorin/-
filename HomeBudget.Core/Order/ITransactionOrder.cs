using HomeBudget.Transactions;

namespace HomeBudget.Order;

/// <summary>У якому порядку перебирати операції.</summary>
public interface ITransactionOrder
{
    string Name { get; }
    List<Transaction> Order(List<Transaction> transactions);
}
