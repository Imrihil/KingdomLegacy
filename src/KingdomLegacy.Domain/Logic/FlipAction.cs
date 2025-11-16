namespace KingdomLegacy.Domain.Logic;
internal class FlipAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override string Name => "Flip";
    public override State[] SourceStates => States.AllNotRemoved;
    public override State TargetState => card.State;
    public override int Order => 3;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "⇒";
    protected override bool ExecuteInternal()
    {
        card.Flip();

        Description = $"Flipped: {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        card.Flip();

        return true;
    }
}
