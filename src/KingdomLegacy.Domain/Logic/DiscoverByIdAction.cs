namespace KingdomLegacy.Domain.Logic;
internal class DiscoverByIdAction(Game game)
    : ReversibleCardActionBase(game)
{
    public override State[] SourceStates => [State.Box];
    public override State TargetState => State.Discovered;
    public override string Text => $"+[{Card?.Id}]";
    protected override Card? Card => Game.BoxById(Game.Config.DiscoverId);
    protected override bool ExecuteInternal()
    {
        if (Card == null)
            return false;

        Description = $"Discovered {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
