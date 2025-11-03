namespace KingdomLegacy.Domain.Logic;
internal class TakeAction(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Discovered, State.Blocked, State.Discarded, State.Removed, State.Purged];
    public override State TargetState => State.Hand;
    public override bool Allowed => card.State != State.Removed && card.State != State.Purged
        || game.TrashedLast == card
        || game.PurgedLast == card;

    public override bool Disabled => false;
    public override string Text => card.State == State.Discarded || card.State == State.Removed || card.State == State.Purged ? "↺" : "✓";

    protected override bool ExecuteInternal()
    {
        Description = $"Took into hand: {card.Id}.";

        return game.ChangeState(card, TargetState);
    }
}
