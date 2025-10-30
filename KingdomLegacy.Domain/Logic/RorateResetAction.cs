namespace KingdomLegacy.Domain.Logic;
internal class RorateResetAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State TargetState => card.State;
    public override bool Allowed => card.State == State.Discovered || card.State == State.Hand || card.State == State.InPlay;
    public override bool Disabled => card.Orientation == Orientation.L1;
    public override string Text => "↻";

    private Orientation _sourceOrientation;
    protected override bool ExecuteInternal()
    {
        _sourceOrientation = card.Orientation;
        card.RotationReset();

        Description = $"Reset rotation of {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        card.Orientation = _sourceOrientation;

        return true;
    }
}
