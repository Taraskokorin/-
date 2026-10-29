using System.Reflection;

namespace HomeBudget.Transactions;

public record TransactionKindEntry(string Kind, string Title, Type Type);

public class TransactionCatalog
{
    private readonly Dictionary<string, TransactionKindEntry> byKind =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Каталог із різновидами самого ядра. Готовий одразу.</summary>
    public static TransactionCatalog Default { get; } = CreateDefault();

    private static TransactionCatalog CreateDefault()
    {
        var catalog = new TransactionCatalog();
        catalog.Add(typeof(Transaction).Assembly);
        return catalog;
    }

    public IReadOnlyCollection<TransactionKindEntry> Kinds => byKind.Values;

    /// <summary>Переглядає збірку й додає всі типи операцій із неї.</summary>
    public int Add(Assembly assembly)
    {
        var added = 0;

        foreach (var type in TypesOf(assembly))
        {
            if (type.IsAbstract || !typeof(Transaction).IsAssignableFrom(type))
                continue;

            var attribute =
                type.GetCustomAttribute<TransactionKindAttribute>(inherit: false);

            if (attribute is null)
                continue;

            // Каталог створює операції конструктором без параметрів.
            if (type.GetConstructor(Type.EmptyTypes) is null)
                throw new InvalidOperationException(
                    $"Класу {type.FullName} потрібен відкритий " +
                    "конструктор без параметрів.");

            byKind[attribute.Kind] =
                new TransactionKindEntry(attribute.Kind, attribute.Title, type);
            added++;
        }

        return added;
    }

    public Transaction? Create(string kind) =>
        byKind.TryGetValue(kind, out var entry)
            ? (Transaction)Activator.CreateInstance(entry.Type)!
            : null;

    private static IEnumerable<Type> TypesOf(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Частину типів завантажити не вдалося – беремо ті, що вдалося.
            return ex.Types.Where(t => t is not null)!;
        }
    }
}
