namespace KingdomLegacy.Domain.Logic;
internal class PutToTheBoxAction(Game game, Card card)
    : ReversibleCardActionBase(game)
{
    public override string Name => "Put back to the box";
    public override State[] SourceStates => [State.Played, State.StayInPlay, State.Discarded];
    public override State TargetState => State.Box;
    public override int Order => 0;
    public override string Text => "⬓";
    protected override Card Card { get; } = card;

    private Orientation _sourceOrientation;
    protected override bool ExecuteInternal()
    {
        Description = $"Put back to the box {Card.Id}.";

        if (!Game.ChangeState(Card, TargetState))
            return false;

        _sourceOrientation = Card.Orientation;
        Card.Reset();

        return true;
    }

    protected override bool UndoInternal()
    {
        if (!base.UndoInternal())
            return false;

        Card.Orientation = _sourceOrientation;

        return true;
    }
}
