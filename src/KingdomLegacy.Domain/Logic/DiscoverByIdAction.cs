namespace KingdomLegacy.Domain.Logic;
internal class DiscoverByIdAction(Game game, int id) 
    : ReversibleCardActionBase(game,
        game._box.FirstOrDefault(card => card.Id == id))
{
    public override State[] SourceStates => [State.Box];
    public override State TargetState => State.Discovered;
    public override bool Allowed => true;
    public override bool Disabled => !game._box.Any(card => card.Id == id);
    public override string Text => "+";
    protected override bool ExecuteInternal()
    {
        Description = $"Discovered {Card.Id}.";

        return game.ChangeState(Card, State.Discovered);
    }
}
