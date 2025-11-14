namespace KingdomLegacy.Domain.Logic;
internal class EndTurnAction(Game game, IStorage storage) : IAction
{
    public string Name => "Discard played & draw 4";
    public State[] SourceStates => [];
    public State TargetState => State.Played;
    public int Order => 0;
    public bool Allowed => game.Discovered.Count == 0 && game.Deck.Count > 0;
    public bool Disabled => false;
    public string Text => "End turn";
    public string Description => "Turn finished.";
    public void Execute()
    {
        foreach (var card in game.Played.ToArray())
            game.Actions.Discard(card);

        game.Actions.Draw4();

        storage.SaveGame(game);
    }
}
