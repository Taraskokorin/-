using HomeBudget.Transactions;

namespace HomeBudget.Desktop.ViewModels;

/// <summary>Спосіб назвати рахунок для операції.</summary>
public abstract class AccountInput : ObservableBase
{
    // Рахунок рядком – саме такий приймає Transaction.BalanceEffect.
    public abstract string Given { get; }

    // Чи є що проводити: від цього залежить кнопка «Провести».
    public abstract bool IsReady { get; }

    public static AccountInput For(Transaction transaction,
                                   IReadOnlyList<string> allAccounts) => transaction switch
    {
        // У переказу два власні рахунки – обирають один із них.
        Transfer transfer => new AccountChoiceInput(transfer.Prompt, transfer.Accounts),
        Expense or Income => new AccountChoiceInput(transaction.Prompt, allAccounts),
        // Різновид зі складника, про який оболонка нічого не знає, – поле введення.
        _ => new AccountTextInput(transaction.Prompt)
    };
}
