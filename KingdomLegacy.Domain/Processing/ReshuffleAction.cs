namespace KingdomLegacy.Domain.Processing;
internal class ReshuffleAction(Game game) : IAction
{
    public State TargetState => State.Deck;
    public bool Allowed => game.Deck.Count == 0;
    public bool Disabled => false;
    public string Text => "♺";
    public bool Execute()
    {
        game._deck = new Queue<Card>(game._deck
            .Concat(game._discovered)
            .Concat(game._hand)
            .Concat(game._inPlay)
            .Concat(game._discarded)
            .OrderBy(_ => Random.Shared.Next()));

        foreach (var card in game._deck)
            card.State = State.Deck;

        if (game._deck.TryPeek(out var nextCard))
            nextCard.State = State.DeckTop;

        game._discovered.Clear();
        game._hand.Clear();
        game._inPlay.Clear();
        game._discarded.Clear();

        return true;
    }
}
