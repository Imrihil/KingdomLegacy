namespace KingdomLegacy.Domain.Logic;
internal class BlockAction(Game game, Card card)
    : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Hand, State.InPlay];
    public override State TargetState => State.Blocked;
    public override int Order => 10;
    public override string Text => "⦸";
    protected override bool ExecuteInternal()
    {
        Description = $"Blocked {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
