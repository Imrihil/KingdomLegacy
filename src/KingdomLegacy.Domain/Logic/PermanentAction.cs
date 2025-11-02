namespace KingdomLegacy.Domain.Logic;
internal class PermanentAction(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Discovered, State.InPlay];
    public override State TargetState => State.Permanent;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "∞";

    protected override bool ExecuteInternal()
    {
        Description = $"Added to permanent {card.Id}.";

        return game.ChangeState(card, TargetState);
    }
}
