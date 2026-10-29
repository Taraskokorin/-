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

## Збірки для трьох систем

Зібрати все однією командою: `./publish.sh` (Linux, macOS, Git Bash) або `publish.ps1` (PowerShell). Результат з'явиться в каталозі `publish/`.

| RID | Для кого | Що запускати |
|---|---|---|
| `win-x64` | Windows 10/11, 64-розрядний | `publish\win-x64udget.exe` |
| `osx-arm64` | macOS на процесорах Apple (M1 і новіші) | `publish/osx-arm64/budget` |
| `linux-x64` | Linux, 64-розрядний | `publish/linux-x64/budget` |

Збірки самодостатні: .NET на цільовій машині не потрібен. Що може завадити запуску:
- **Linux і macOS:** після копіювання чи розпакування zip потрібно знову виконати `chmod +x budget`.
- **macOS:** файл, завантажений з мережі, потрапляє в карантин; зняти його можна командою `xattr -d com.apple.quarantine budget`.
- **Linux:** потрібна бібліотека ICU (`libicu`); у мінімальних образах її немає.
- **Windows:** політика PowerShell `Restricted` не дає запустити `publish.ps1`. Я збирав через `bash publish.sh`.

`budget --info` показує систему, RID і каталог даних. Журнал зберігається в каталозі даних користувача: `%APPDATA%\HomeBudget` у Windows і `~/.config/HomeBudget` у Linux та macOS.

Автор: Кокорін Тарас, група 33 Бд-СОінф.
