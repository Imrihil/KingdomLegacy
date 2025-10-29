namespace KingdomLegacy.Domain.Processing;
internal class EndTurnAction(Game game) : IAction
{
    public State TargetState => State.Hand;
    public bool Allowed => game._discovered.Count > 0;
    public bool Disabled => false;
    public string Text => "End turn";
    public bool Execute()
    {
        foreach (var card in game._hand.ToArray())
            game.Actions.Discard(card);

        game.Actions.Draw(4);

        return true;
    }
}
