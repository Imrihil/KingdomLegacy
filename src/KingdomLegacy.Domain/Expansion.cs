namespace KingdomLegacy.Domain;

public class Expansions
{
    public static readonly Expansion FeudalKingdom = new("FeudalKingdom", 140);
}

public class Expansion(string name, int count)
{
    public List<Card> Cards { get; } = Load(name, count);

    private static List<Card> Load(string name, int count) =>
        Enumerable.Range(0, count)
        .Select(i => new Card { Id = i, Expansion = name, Orientation = Orientation.L1, State = State.Box })
        .ToList();
}
