namespace KingdomLegacy.Domain.Processing;
internal class EndRoundAction(Game game) : IAction
{
    public State TargetState => State.Hand;
    public bool Allowed => game._discovered.Count == 0 && game._deck.Count == 0;
    public bool Disabled => false;
    public string Text => "End round";
    public bool Execute()
    {
        foreach (var card in game._hand.Concat(game._inPlay).ToArray())
            game.Actions.Discard(card);

        game.Actions.Discover(2);

        return true;
    }
}
