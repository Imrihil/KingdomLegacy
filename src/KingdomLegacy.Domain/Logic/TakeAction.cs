namespace KingdomLegacy.Domain.Logic;
internal class TakeAction(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Discovered, State.Blocked, State.Discarded, State.Removed];
    public override State TargetState => State.Hand;
    public override bool Allowed => card.State != State.Removed || game.TrashedLast == card;
    public override bool Disabled => false;
    public override string Text => card.State == State.Discarded || card.State == State.Removed ? "↺" : "✓";

    protected override bool ExecuteInternal()
    {
        Description = $"Took into hand: {card.Id}.";

        return game.ChangeState(card, TargetState);
    }
}
