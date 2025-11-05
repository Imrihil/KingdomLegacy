namespace KingdomLegacy.Domain.Logic;
internal class DiscoverByIdAction(Game game, int id)
    : ReversibleCardActionBase(game, game.BoxById(id))
{
    public override State[] SourceStates => [State.Box];
    public override State TargetState => State.Discovered;
    public override string Text => "+";
    protected override bool ExecuteInternal()
    {
        Description = $"Discovered {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
