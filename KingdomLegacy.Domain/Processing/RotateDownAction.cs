namespace KingdomLegacy.Domain.Processing;
internal class RotateDownAction(Game game, Card card) : IAction
{
    public State TargetState => card.State;
    public bool Allowed => card.State == State.Discovered || card.State == State.Hand || card.State == State.InPlay;
    public bool Disabled => false;
    public string Text => "⇓";
    public bool Execute()
    {
        card.RotateDown();

        return true;
    }
}
