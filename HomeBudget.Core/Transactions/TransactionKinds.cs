using System.Collections.Concurrent;
using System.Reflection;

namespace HomeBudget.Transactions;

public static class TransactionKinds
{
    // Пошук атрибута повільний, а відповідь для типу незмінна:
    // кожен тип перевіряємо один раз.
    private static readonly ConcurrentDictionary<Type, TransactionKindAttribute>
        Known = new();

    public static TransactionKindAttribute Describe(Type type) =>
        Known.GetOrAdd(type, Read);

    private static TransactionKindAttribute Read(Type type)
    {
        var attribute =
            type.GetCustomAttribute<TransactionKindAttribute>(inherit: false);

        if (attribute is null)
            throw new InvalidOperationException(
                $"До класу {type.Name} не додано атрибут [TransactionKind].");

        return attribute;
    }

    public static string Of(Type type) => Describe(type).Kind;
}
