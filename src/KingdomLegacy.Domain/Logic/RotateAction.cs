namespace KingdomLegacy.Domain.Logic;
internal class RotateAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override string Name => "Rotate";
    public override State[] SourceStates => States.AllNotRemoved;
    public override State TargetState => card.State;
    public override int Order => 2;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "⇓";
    protected override bool ExecuteInternal()
    {
        card.Rotate();

        Description = $"Rotated: {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        card.Rotate();
        return true;
    }
}
