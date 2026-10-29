# HomeBudget — Домашня бухгалтерія

**Тема проєкту:** «Домашня бухгалтерія» (тема 11 з додатка А).

Консольний застосунок на .NET 10 для обліку особистих фінансів: витрати, надходження й перекази між рахунками, а наприкінці — підсумковий баланс.

## Карта назв

| Роль у кістяку | У прикладі | Домашня бухгалтерія |
|---|---|---|
| Основний тип | `Card` | `Transaction` |
| Різновид 1 | `OpenAnswerCard` | `Expense` (витрата) |
| Різновид 2 | `ChoiceCard` | `Income` (надходження) |
| Різновид 3 | `NumericCard` | `Transfer` (переказ між рахунками) |
| Спільна дія | `Check(answer)` | `BalanceEffect(account)` — вплив на баланс рахунку |
| Набір записів | `List<Card> deck` | `List<Transaction> transactions` |
| Сховище | `ICardStorage` | `ITransactionStorage` |
| Порядок перебору | `IReviewStrategy` | `ITransactionOrder` |
| Підсумок | `SessionSummary` | `BalanceSummary` |

## Будова

| Каталог | Що це |
|---|---|
| `HomeBudget.Core/` | бібліотека-ядро: операції, сховище JSON, порядок перебору, ядро обліку |
| `HomeBudget.App/` | консольна оболонка на Spectre.Console (`budget`) |
| `tools/BudgetReport/` | окремий застосунок, що підключає ядро пакетом NuGet |

## Як запустити

Потрібен .NET SDK 10.

```
dotnet run --project HomeBudget.App
```

Автор: Кокорін Тарас, група 33 Бд-СОінф.
