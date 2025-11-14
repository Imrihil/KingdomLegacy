namespace KingdomLegacy.Domain.Logic;
internal class EndRoundAction(Game game, IStorage storage) : IAction
{
    public string Name => ShouldDiscover() ? $"Discard all and discover {game.Config.DiscoverCount}" : "Reshuffle all";
    public State[] SourceStates => [];
    public State TargetState => State.Played;
    public int Order => 0;
    public bool Allowed => game.Discovered.Count == 0 && game.DeckCount == 0;
    public bool Disabled => false;
    public string Text => "End round";
    public string Description => "Round finished.";
    public void Execute()
    {
        game.Resources.Reset();
        if (ShouldDiscover())
        {
            foreach (var card in game.Played.Concat(game.StayInPlay).Concat(game.Blocked).ToArray())
                game.Actions.Discard(card);
            game.Actions.Discover();
        }
        else
        {
            game.Actions.Reshuffle();
        }

        storage.SaveGame(game);
    }

    private bool ShouldDiscover() => game.Config.DiscoverCount > 0;
}
