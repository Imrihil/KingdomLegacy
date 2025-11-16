namespace KingdomLegacy.Domain.Logic;
internal class ResetAndDiscard(Game game, Card card) : IAction
{
    public string Name => "Reset orientation & discard";
    public State[] SourceStates => [State.Played, State.StayInPlay, State.Blocked];
    public State TargetState => State.Discarded;
    public int Order => 2;
    public bool Allowed => true;
    public bool Disabled => false;
    public string Text => "↻";
    public string? Description => null;
    public void Execute()
    {
        game.Actions.Reset(card);
        game.Actions.Discard(card);
    }
}
