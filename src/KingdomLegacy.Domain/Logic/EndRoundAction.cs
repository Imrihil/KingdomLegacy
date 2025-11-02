namespace KingdomLegacy.Domain.Logic;
internal class EndRoundAction(Game game, Resources resources) : IAction
{
    public State[] SourceStates => [];
    public State TargetState => State.Hand;
    public bool Allowed => game._discovered.Count == 0 && game._deck.Count == 0;
    public bool Disabled => false;
    public string Text => "End round";
    public string Description => "Round finished.";
    public void Execute()
    {
        foreach (var card in game._hand.Concat(game._inPlay).ToArray())
            game.Actions.Discard(card);

        game.Actions.Discover(2);

        resources.Reset();
    }
}
