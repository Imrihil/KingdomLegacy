namespace KingdomLegacy.Domain.Logic;
internal class RotateRightAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State TargetState => card.State;
    public override bool Allowed => card.State == State.Discovered;
    public override bool Disabled => false;
    public override string Text => "⇒";
    protected override bool ExecuteInternal()
    {
        card.RotateRight();

        Description = $"Rotated right: {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        card.RotateRight();

        return true;
    }
}
