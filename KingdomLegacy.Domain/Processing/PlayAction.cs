namespace KingdomLegacy.Domain.Processing;
internal class PlayAction(Game game, Card card) : IAction
{
    public State TargetState => State.InPlay;
    public bool Allowed => card.State == State.Hand;
    public bool Disabled => false;
    public string Text => "∞";
    public bool Execute()
    {
        if (!game._hand.Remove(card))
            return false;

        card.State = State.InPlay;
        game._inPlay.Add(card);

        return true;
    }
}
