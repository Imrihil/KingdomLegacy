namespace KingdomLegacy.Domain.Logic;
internal class PutToTheBoxAction(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Hand, State.InPlay, State.Discarded];
    public override State TargetState => State.Box;
    public override int Order => 0;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "⬓";

    private Orientation _sourceOrientation;
    protected override bool ExecuteInternal()
    {
        Description = $"Put back to the box {card.Id}.";

        if (!game.ChangeState(card, TargetState))
            return false;

        _sourceOrientation = card.Orientation;
        card.Reset();

        return true;
    }

    protected override bool UndoInternal()
    {
        if (!base.UndoInternal())
            return false;

        card.Orientation = _sourceOrientation;

        return true;
    }
}
