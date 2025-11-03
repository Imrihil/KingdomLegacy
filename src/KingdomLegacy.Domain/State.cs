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

    public static readonly HashSet<State> AllReverted = [State.Box, State.Hand, State.Blocked, State.Discarded, State.Removed];

    public static int Order(this State state) => state switch
    {
        State.Box => 0,
        State.Discovered => 1,
        State.Deck => 2,
        State.DeckTop => 3,
        State.Permanent => 4,
        State.InPlay => 5,
        State.Hand => 6,
        State.Blocked => 7,
        State.Discarded => 8,
        State.Removed => 9,
        _ => int.MaxValue,
    };
}