namespace KingdomLegacy.Domain.Logic;
internal class EndTurnAction(Game game, Resources resources) : IAction
{
    public State[] SourceStates => [];
    public State TargetState => State.Hand;
    public int Order => 0;
    public bool Allowed => game.Discovered.Count == 0 && game.Deck.Count > 0;
    public bool Disabled => false;
    public string Text => "End turn";
    public string Description => "Turn finished.";
    public void Execute()
    {
        foreach (var card in game.Hand.ToArray())
            game.Actions.Discard(card);

        game.Actions.Draw4();

        resources.Reset();
    }
}
