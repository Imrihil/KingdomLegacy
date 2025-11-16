namespace KingdomLegacy.Domain.Logic;
internal class ResetAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override string Name => "Reset orientation";
    public override State[] SourceStates => States.AllNotRemoved;
    public override State TargetState => card.State;
    public override int Order => 2;
    public override bool Allowed => true;
    public override bool Disabled => card.Orientation == Orientation.L1;
    public override string Text => "↻";

    private Orientation _sourceOrientation;
    protected override bool ExecuteInternal()
    {
        _sourceOrientation = card.Orientation;
        card.Reset();

        Description = $"Reset orientation of {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        card.Orientation = _sourceOrientation;

        return true;
    }
}
