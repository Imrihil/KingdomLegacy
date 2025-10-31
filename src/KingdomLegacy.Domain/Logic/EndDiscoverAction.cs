namespace KingdomLegacy.Domain.Logic;
internal class EndDiscoverAction(Game game) : IAction
{
    public State TargetState => State.Hand;
    public bool Allowed => game._discovered.Count > 0;
    public bool Disabled => false;
    public string Text => "End discover";
    public string Description => "Discovering finished.";
    public void Execute()
    {
        if(game._deck.Count > 0 || game._hand.Count > 0 || game._inPlay.Count > 0) {
            foreach(var card in game._discovered) {
                game.Actions.Discard(card);
            }
        } else {
            game.Actions.Reshuffle();
            game.Actions.Draw(4);
        }
    }
}
