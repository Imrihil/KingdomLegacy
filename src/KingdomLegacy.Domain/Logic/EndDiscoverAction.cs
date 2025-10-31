namespace KingdomLegacy.Domain.Logic;
internal class EndDiscoverAction(Game game) : IAction
{
    public State TargetState => State.Hand;
    public bool Allowed => game._discovered.Count > 0;
    public bool Disabled => false;
    public string Text => "End discover";
    public string Description => "Discovering finished.";
    public void Execute()
    {
        game.Actions.Reshuffle();
        game.Actions.Draw(4);
    }
}
