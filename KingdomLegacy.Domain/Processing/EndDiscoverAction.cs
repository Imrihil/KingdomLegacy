namespace KingdomLegacy.Domain.Processing;
internal class EndDiscoverAction(Game game) : IAction
{
    public State TargetState => State.Hand;
    public bool Allowed => game._discovered.Count > 0;
    public bool Disabled => false;
    public string Text => "End discover";
    public bool Execute()
    {
        game.Actions.Reshuffle();
        game.Actions.Draw(4);

        return true;
    }
}
