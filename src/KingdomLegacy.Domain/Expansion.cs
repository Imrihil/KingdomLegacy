namespace KingdomLegacy.Domain;

public class Expansions
{
    public static readonly Expansion FeudalKingdom = new("FeudalKingdom", 140);
}

public class Expansion(string name, int count)
{
    public string Name { get; } = name;
    public IReadOnlyCollection<Card> Cards => Load(count);

    private List<Card> Load(int count) =>
        Enumerable.Range(0, count)
        .Select(i => new Card { Id = i, Expansion = Name, Orientation = Orientation.L1, State = State.Box })
        .ToList();
}
