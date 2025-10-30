namespace KingdomLegacy.Domain.Logic;
internal class RotateDownAction(Game game, Card card) : RecordedActionBase(game)
{
    public override State TargetState => card.State;
    public override bool Allowed => card.State == State.Discovered;
    public override bool Disabled => false;
    public override string Text => "⇓";
    protected override bool ExecuteInternal()
    {
        card.RotateDown();

        Description = $"Rotated down: {card.Id}.";

        return true;
    }
}
