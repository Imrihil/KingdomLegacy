namespace KingdomLegacy.Domain.Logic;
internal class EndDiscoverAction(Game game, IStorage storage) : IAction
{
    public string Name => IsDuringRound() ? "Discard all discovered" : "Reshuffle all & draw 4";
    public State[] SourceStates => [];
    public State TargetState => State.Played;
    public int Order => 0;
    public bool Allowed => game.Discovered.Count > 0;
    public bool Disabled => false;
    public string Text => "End discover";
    public string Description => "Discovering finished.";
    public void Execute()
    {
        if (IsDuringRound())
        {
            foreach (var card in game.Discovered.ToArray())
                game.Actions.Discard(card);
        }
        else
        {
            game.Actions.Reshuffle();
            game.Actions.Draw4();
        }

        storage.SaveGame(game);
    }

    private bool IsDuringRound() => game.DeckCount > 0 || game.Played.Count > 0 || game.StayInPlay.Count > 0;
}
