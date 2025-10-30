namespace KingdomLegacy.Domain.Logic;
internal class TakeAction(Game game, Card card) : RecordedActionBase(game)
{
    public override State TargetState => State.Hand;
    public override bool Allowed => card.State == State.Discovered
        || card.State == State.InPlay
        || card.State == State.Discarded
        || card.State == State.Removed && game._trash.TryPeek(out var topCard) && topCard == card;
    public override bool Disabled => false;
    public override string Text => card.State == State.Discarded || card.State == State.Removed ? "↺" : "✓";

    protected override bool ExecuteInternal()
    {
        if (!game._discovered.Remove(card)
            && !game._inPlay.Remove(card)
            && !game._discarded.Remove(card)
            && !game._trash.TryPop(out var _))
            return false;

        card.State = State.Hand;
        game._hand.Add(card);

        Description = $"Took into hand: {card.Id}.";

        return true;
    }
}
