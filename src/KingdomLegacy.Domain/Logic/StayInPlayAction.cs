namespace KingdomLegacy.Domain.Logic;
internal class StayInPlayAction(Game game, Card card)
    : ReversibleCardActionBase(game)
{
    public override string Name => "Stay in play";
    public override State[] SourceStates => [State.Played];
    public override State TargetState => State.StayInPlay;
    public override string Text => "∞";
    protected override Card Card { get; } = card;
    protected override bool ExecuteInternal()
    {
        Description = $"Stayed in play {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
