namespace KingdomLegacy.Domain.Processing;
internal class TakeAction(Game game, Card card) : IAction
{
    public State TargetState => State.Hand;
    public bool Allowed => card.State == State.Discovered
        || card.State == State.InPlay
        || card.State == State.Discarded
        || card.State == State.Removed && game._trash.TryPeek(out var topCard) && topCard == card;
    public bool Disabled => false;
    public string Text => "✓";

    public bool Execute()
    {
        if (!game._discovered.Remove(card) && !game._inPlay.Remove(card) && !game._discarded.Remove(card) && (!game._trash.TryPop(out var poppedCard) || poppedCard != card))
            return false;

        card.State = State.Hand;
        game._hand.Add(card);

        return true;
    }
}
