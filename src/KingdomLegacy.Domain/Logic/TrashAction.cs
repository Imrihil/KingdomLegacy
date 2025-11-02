namespace KingdomLegacy.Domain.Logic;
internal class TrashAction(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Discovered, State.Hand, State.InPlay, State.Discarded, State.Permanent];
    public override State TargetState => State.Removed;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "x";

    protected override bool ExecuteInternal()
    {
        Description = $"Trashed {card.Id}.";

        return game.ChangeState(card, TargetState);
    }
}
