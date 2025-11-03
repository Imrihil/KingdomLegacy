namespace KingdomLegacy.Domain.Logic;
internal class PlaceToDeckTop(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Discarded];
    public override State TargetState => State.DeckTop;
    public override int Order => 7;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "⊡";
    protected override bool ExecuteInternal()
    {
        Description = $"Placed on deck top {card.Id}.";

        var deckTop = game.DeckTop;
        if (!game.ChangeState(card, TargetState))
            return false;

        if (deckTop != null)
            deckTop.State = State.Deck;

        return true;
    }
}
