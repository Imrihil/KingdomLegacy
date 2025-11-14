namespace KingdomLegacy.Domain.Logic;
internal class ReshuffleAction(Game game) : RecordedActionBase(game)
{
    public override string Name => "Reshuffle";
    public override State[] SourceStates => [];
    public override State TargetState => State.Deck;
    public override bool Allowed => Game.Deck.Count == 0;
    public override bool Disabled => false;
    public override string Text => "♺";
    private Card[] _cards = [];
    protected override bool ExecuteInternal()
    {
        _cards = Game.Deck
            .Concat(Game.Discovered)
            .Concat(Game.Played)
            .Concat(Game.StayInPlay)
            .Concat(Game.Discarded)
            .OrderBy(card => card.Id)
            .ToArray();

        foreach (var card in _cards)
            Game.ChangeState(card, TargetState);

        Game.DeckReshuffle();

        Description = $"Reshuffeled {string.Join(", ", _cards.Select(card => card.Id))}.";

        return true;
    }
}
