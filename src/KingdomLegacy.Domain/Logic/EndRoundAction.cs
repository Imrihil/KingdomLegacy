namespace KingdomLegacy.Domain.Logic;
internal class EndRoundAction(Game game, Resources resources, IStorage storage) : IAction
{
    public State[] SourceStates => [];
    public State TargetState => State.Hand;
    public int Order => 0;
    public bool Allowed => game.Discovered.Count == 0 && game.DeckCount == 0;
    public bool Disabled => false;
    public string Text => "End round";
    public string Description => "Round finished.";
    public void Execute()
    {
        foreach (var card in game.Hand.Concat(game.InPlay).Concat(game.Blocked).ToArray())
            game.Actions.Discard(card);

        game.Actions.Discover(2);

        resources.Reset();

        storage.SaveGame(game);
    }
}
