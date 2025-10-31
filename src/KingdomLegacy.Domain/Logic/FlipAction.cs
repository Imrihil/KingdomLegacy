namespace KingdomLegacy.Domain.Logic;
internal class FlipAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State TargetState => card.State;
    public override bool Allowed => card.State == State.Discovered || card.State == State.Permanent;
    public override bool Disabled => false;
    public override string Text => "⇒";
    protected override bool ExecuteInternal()
    {
        card.Flip();

        Description = $"Rotated right: {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        card.Flip();

        return true;
    }
}
