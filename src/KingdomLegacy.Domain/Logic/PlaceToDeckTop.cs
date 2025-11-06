namespace KingdomLegacy.Domain.Logic;
internal class PlaceToDeckTop(Game game, Card card)
    : ReversibleCardActionBase(game)
{
    public override State[] SourceStates => [State.Discarded];
    public override State TargetState => State.DeckTop;
    public override int Order => 7;
    public override string Text => "⊡";
    protected override Card Card { get; } = card;
    protected override bool ExecuteInternal()
    {
        Description = $"Placed on deck top {Card.Id}.";

        var deckTop = Game.DeckTop;
        if (!Game.ChangeState(Card, TargetState))
            return false;

        if (deckTop != null)
            deckTop.State = State.Deck;

        return true;
    }
}
