namespace KingdomLegacy.Domain.Logic;
internal class RotateRightAction(Game game, Card card) : RecordedActionBase(game)
{
    public override State TargetState => card.State;
    public override bool Allowed => card.State == State.Discovered || card.State == State.Hand || card.State == State.InPlay;
    public override bool Disabled => false;
    public override string Text => "⇒";
    protected override bool ExecuteInternal()
    {
        card.RotateRight();

        return true;
    }
}
