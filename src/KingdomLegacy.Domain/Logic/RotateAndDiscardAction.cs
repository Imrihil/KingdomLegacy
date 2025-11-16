namespace KingdomLegacy.Domain.Logic;
internal class RotateAndDiscardAction(Game game, Card card) : IAction
{
    public string Name => "Rotate & discard";
    public State[] SourceStates => [State.Played, State.StayInPlay, State.Blocked];
    public State TargetState => State.Discarded;
    public int Order => 4;
    public bool Allowed => true;
    public bool Disabled => false;
    public string Text => "⇓";
    public string? Description => null;
    public void Execute()
    {
        game.Actions.Rotate(card);
        game.Actions.Discard(card);
    }
}
