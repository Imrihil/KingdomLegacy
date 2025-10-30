namespace KingdomLegacy.Domain.Logic;
internal class ReshuffleAction(Game game) : RecordedActionBase(game)
{
    public override State TargetState => State.Deck;
    public override bool Allowed => game.Deck.Count == 0;
    public override bool Disabled => false;
    public override string Text => "♺";
    private Card[] _cards = [];
    protected override bool ExecuteInternal()
    {
        _cards = game._deck
            .Concat(game._discovered)
            .Concat(game._hand)
            .Concat(game._inPlay)
            .Concat(game._discarded)
            .OrderBy(card => card.Id)
            .ToArray();

        game._deck = new Queue<Card>(_cards.OrderBy(_ => Random.Shared.Next()));

        foreach (var card in game._deck)
            card.State = State.Deck;

        if (game._deck.TryPeek(out var nextCard))
            nextCard.State = State.DeckTop;

        game._discovered.Clear();
        game._hand.Clear();
        game._inPlay.Clear();
        game._discarded.Clear();

        Description = $"Reshuffeled {string.Join(", ", _cards.Select(card => card.Id))}.";

        return true;
    }
}
