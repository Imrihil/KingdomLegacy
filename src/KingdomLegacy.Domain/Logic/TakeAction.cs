namespace KingdomLegacy.Domain.Logic;
internal class TakeAction(Game game, Card card) 
    : ReversibleCardActionBase(game)
{
    public override string Name => "Play";
    public override State[] SourceStates => [State.Discovered, State.Blocked, State.Discarded, State.Destroyed, State.Purged];
    public override State TargetState => State.Played;
    public override bool Allowed => Card.State != State.Destroyed && Card.State != State.Purged
        || Game.DestroyedLast == Card
        || Game.PurgedLast == Card;

    public override string Text => Card.State == State.Discarded || Card.State == State.Destroyed || Card.State == State.Purged ? "↺" : "✓";
    protected override Card Card { get; } = card;
    protected override bool ExecuteInternal()
    {
        Description = $"Played: {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
