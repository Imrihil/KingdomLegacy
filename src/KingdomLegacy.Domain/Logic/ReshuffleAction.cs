namespace KingdomLegacy.Domain.Logic;
internal class ReshuffleAction(Game game) : RecordedActionBase(game)
{
    public override State[] SourceStates => [];
    public override State TargetState => State.Deck;
    public override bool Allowed => game.Deck.Count == 0;
    public override bool Disabled => false;
    public override string Text => "♺";
    private Card[] _cards = [];
    protected override bool ExecuteInternal()
    {
        _cards = game.Deck
            .Concat(game.Discovered)
            .Concat(game.Hand)
            .Concat(game.InPlay)
            .Concat(game.Discarded)
            .OrderBy(card => card.Id)
            .ToArray();

        foreach (var card in _cards)
            game.ChangeState(card, TargetState);

        game.DeckReshuffle();

        Description = $"Reshuffeled {string.Join(", ", _cards.Select(card => card.Id))}.";

        return true;
    }
}
