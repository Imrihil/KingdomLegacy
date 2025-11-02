namespace KingdomLegacy.Domain.Logic;
internal class DiscardAction(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Discovered, State.Hand, State.InPlay];
    public override State TargetState => State.Discarded;
    public override int Order => 10;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "✓";
    protected override bool ExecuteInternal()
    {
        Description = $"Discarded {card.Id}.";

        return game.ChangeState(card, TargetState);
    }
}
