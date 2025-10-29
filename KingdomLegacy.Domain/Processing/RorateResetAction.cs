namespace KingdomLegacy.Domain.Processing;
internal class RorateResetAction(Game game, Card card) : IAction
{
    public State TargetState => card.State;
    public bool Allowed => card.State == State.Discovered || card.State == State.Hand || card.State == State.InPlay;
    public bool Disabled => card.Orientation == Orientation.L1;
    public string Text => "↻";
    public bool Execute()
    {
        card.RotationReset();

        return true;
    }
}
