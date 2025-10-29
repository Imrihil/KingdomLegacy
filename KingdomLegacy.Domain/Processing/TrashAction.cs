namespace KingdomLegacy.Domain.Processing;
internal class TrashAction(Game game, Card card) : IAction
{
    public State TargetState => State.Removed;
    public bool Allowed => card.State == State.Discovered || card.State == State.Hand || card.State == State.InPlay;
    public bool Disabled => false;
    public string Text => "x";
    public bool Execute()
    {
        if (!game._discovered.Remove(card) && !game._hand.Remove(card) && !game._inPlay.Remove(card))
            return false;

        card.State = State.Removed;
        game._trash.Push(card);

        return true;
    }
}
