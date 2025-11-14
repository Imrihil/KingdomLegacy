namespace KingdomLegacy.Domain.Logic;
internal class PermanentAction(Game game, Card card)
    : ReversibleCardActionBase(game)
{
    public override string Name => "Move to permanent";
    public override State[] SourceStates => [State.Discovered, State.StayInPlay];
    public override State TargetState => State.Permanent;
    public override string Text => "∞";
    protected override Card Card { get; } = card;
    protected override bool ExecuteInternal()
    {
        Description = $"Moved to permanent {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
