# Kokorin.HomeBudget.Core

Ядро домашньої бухгалтерії на .NET 10.

- `Transaction` і три різновиди: `Expense` (витрата), `Income` (надходження), `Transfer` (переказ між рахунками).
- Спільна дія `BalanceEffect(account)` повертає вплив операції на баланс рахунку.
- `JsonTransactionStorage` асинхронно читає й записує журнал у файл JSON.
- `LedgerSession` — ядро обліку. Порядок перебору задає `ITransactionOrder`, а підсумок зберігається в `BalanceSummary`.
