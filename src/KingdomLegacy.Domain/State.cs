namespace KingdomLegacy.Domain;
public enum State
{
    Box,
    Discovered,
    Deck,
    DeckTop,
    Played,
    StayInPlay,
    Discarded,
    Destroyed,
    Permanent,
    Blocked,
    Purged
}

public static class States
{
    public static readonly State[] All = Enum.GetValues<State>().ToArray();
    public static readonly State[] AllNotRemoved = All.Where(state => state != State.Destroyed && state != State.Purged).ToArray();

    public static readonly HashSet<State> AllReverted = [State.Box, State.Deck, State.DeckTop, State.Played, State.Blocked, State.Discarded, State.Destroyed, State.Purged];

    public static int Order(this State state) => state switch
    {
        State.Box => 0,
        State.Discovered => 1,
        State.Deck => 2,
        State.DeckTop => 3,
        State.Permanent => 4,
        State.StayInPlay => 5,
        State.Played => 6,
        State.Blocked => 7,
        State.Discarded => 8,
        State.Destroyed => 9,
        State.Purged => 10,
        _ => int.MaxValue,
    };
}