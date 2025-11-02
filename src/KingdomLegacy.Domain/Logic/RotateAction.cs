namespace KingdomLegacy.Domain.Logic;
internal class RotateAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State[] SourceStates => States.All;
    public override State TargetState => card.State;
    public override int Order => 2;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "⇓";
    protected override bool ExecuteInternal()
    {
        card.Rotate();

        Description = $"Rotated down: {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        card.Rotate();
        return true;
    }
}
