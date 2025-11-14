namespace KingdomLegacy.Domain.Logic;
internal class TrashAction(Game game, Card card) 
    : ReversibleCardActionBase(game)
{
    public override State[] SourceStates => States.AllNotRemoved;
    public override State TargetState => State.Destroyed;
    public override int Order => 100;
    public override string Text => "x";
    protected override Card Card { get; } = card;
    protected override bool ExecuteInternal()
    {
        Description = $"Trashed {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
