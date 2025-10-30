namespace KingdomLegacy.Domain.Logic;
internal class DiscardAction(Game game, Card card) : RecordedActionBase(game)
{
    public override State TargetState => State.Discarded;
    public override bool Allowed => card.State == State.Discovered || card.State == State.Hand || card.State == State.InPlay;
    public override bool Disabled => false;
    public override string Text => "✓";
    protected override bool ExecuteInternal()
    {
        if (!game._discovered.Remove(card) && !game._hand.Remove(card) && !game._inPlay.Remove(card))
            return false;

        card.State = State.Discarded;
        game._discarded.Add(card);

        return true;
    }
}
