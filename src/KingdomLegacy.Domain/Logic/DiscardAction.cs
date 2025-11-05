namespace KingdomLegacy.Domain.Logic;
internal class DiscardAction(Game game, Card card)
    : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Deck, State.DeckTop, State.Discovered, State.Hand, State.InPlay, State.Blocked];
    public override State TargetState => State.Discarded;
    public override int Order => 20;
    public override string Text => "✓";
    protected override bool ExecuteInternal()
    {
        Description = $"Discarded {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
