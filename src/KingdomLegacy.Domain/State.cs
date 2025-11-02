namespace KingdomLegacy.Domain;
public enum State
{
    Box,
    Discovered,
    Deck,
    DeckTop,
    Hand,
    InPlay,
    Discarded,
    Removed,
    Permanent,
    Blocked
}

public static class States
{
    public static readonly State[] All = Enum.GetValues<State>().ToArray();
}