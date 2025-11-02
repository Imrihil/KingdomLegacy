namespace KingdomLegacy.Domain.Logic;
internal class RotateAndDiscardAction(Game game, Card card) : IAction
{
    public State[] SourceStates => [State.Hand, State.InPlay];
    public State TargetState => State.Discarded;
    public int Order => 4;
    public bool Allowed => card.State == State.Hand || card.State == State.InPlay;
    public bool Disabled => false;
    public string Text => "⇓";
    public string? Description => null;
    public void Execute()
    {
        game.Actions.Rotate(card);
        game.Actions.Discard(card);
    }
}
