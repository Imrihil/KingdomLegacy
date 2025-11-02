namespace KingdomLegacy.Domain.Logic;
internal class EndDiscoverAction(Game game) : IAction
{
    public State[] SourceStates => [];
    public State TargetState => State.Hand;
    public int Order => 0;
    public bool Allowed => game._discovered.Count > 0;
    public bool Disabled => false;
    public string Text => "End discover";
    public string Description => "Discovering finished.";
    public void Execute()
    {
        if(game._deck.Count > 0 || game._hand.Count > 0 || game._inPlay.Count > 0) {
            foreach(var card in game._discovered.ToArray()) {
                game.Actions.Discard(card);
            }
        } else {
            game.Actions.Reshuffle();
            game.Actions.Draw4();
        }
    }
}
