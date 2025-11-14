namespace KingdomLegacy.Domain.Logic;
internal class PlayAction(Game game, Card card)
    : ReversibleCardActionBase(game)
{
    public override State[] SourceStates => [State.Played];
    public override State TargetState => State.StayInPlay;
    public override string Text => "∞";
    protected override Card Card { get; } = card;
    protected override bool ExecuteInternal()
    {
        Description = $"Played {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
