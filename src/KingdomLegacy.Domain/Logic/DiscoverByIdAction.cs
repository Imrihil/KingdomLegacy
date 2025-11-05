namespace KingdomLegacy.Domain.Logic;
internal class DiscoverByIdAction(Game game, int id) 
    : ReversibleCardActionBase(game,
        game.BoxById(id))
{
    public override State[] SourceStates => [State.Box];
    public override State TargetState => State.Discovered;
    public override bool Allowed => true;
    public override bool Disabled => game.BoxById(id) == null;
    public override string Text => "+";
    protected override bool ExecuteInternal()
    {
        if (Card == null)
            return false;

        Description = $"Discovered {Card.Id}.";

        return game.ChangeState(Card, State.Discovered);
    }
}
