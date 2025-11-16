namespace KingdomLegacy.Domain.Logic;
internal class FlipAndDiscard(Game game, Card card) : IAction
{
    public string Name => "Flip & discard";
    public State[] SourceStates => [State.Played, State.StayInPlay, State.Blocked];
    public State TargetState => State.Discarded;
    public int Order => 3;
    public bool Allowed => true;
    public bool Disabled => false;
    public string Text => "⇒";
    public string? Description => null;
    public void Execute()
    {
        game.Actions.Flip(card);
        game.Actions.Discard(card);
    }
}
