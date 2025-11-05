namespace KingdomLegacy.Domain.Logic;
internal class PermanentAction(Game game, Card card)
    : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Discovered, State.InPlay];
    public override State TargetState => State.Permanent;
    public override string Text => "∞";

    protected override bool ExecuteInternal()
    {
        Description = $"Added to permanent {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
