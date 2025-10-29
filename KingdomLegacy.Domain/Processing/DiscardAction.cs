namespace KingdomLegacy.Domain.Processing;
internal class DiscardAction(Game game, Card card) : IAction
{
    public State TargetState => State.Discarded;
    public bool Allowed => card.State == State.Discovered || card.State == State.Hand || card.State == State.InPlay;
    public bool Disabled => false;
    public string Text => "✓";
    public bool Execute()
    {
        if (!game._discovered.Remove(card) && !game._hand.Remove(card) && !game._inPlay.Remove(card))
            return false;

        card.State = State.Discarded;
        game._discarded.Add(card);

        return true;
    }
}
