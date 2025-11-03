namespace KingdomLegacy.Domain.Logic;
internal class OrientationResetAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State[] SourceStates => States.AllNotRemoved;
    public override State TargetState => card.State;
    public override int Order => 0;
    public override bool Allowed => true;
    public override bool Disabled => card.Orientation == Orientation.L1;
    public override string Text => "↻";

    private Orientation _sourceOrientation;
    protected override bool ExecuteInternal()
    {
        _sourceOrientation = card.Orientation;
        card.Reset();

        Description = $"Reset rotation of {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        card.Orientation = _sourceOrientation;

        return true;
    }
}
