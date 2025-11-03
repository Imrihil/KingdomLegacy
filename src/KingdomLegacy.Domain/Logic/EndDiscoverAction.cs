namespace KingdomLegacy.Domain.Logic;
internal class EndDiscoverAction(Game game) : IAction
{
    public State[] SourceStates => [];
    public State TargetState => State.Hand;
    public int Order => 0;
    public bool Allowed => game.Discovered.Count > 0;
    public bool Disabled => false;
    public string Text => "End discover";
    public string Description => "Discovering finished.";
    public void Execute()
    {
        if(game.DeckCount > 0 || game.Hand.Count > 0 || game.InPlay.Count > 0) {
            foreach(var card in game.Discovered.ToArray()) {
                game.Actions.Discard(card);
            }
        } else {
            game.Actions.Reshuffle();
            game.Actions.Draw4();
        }
    }
}
