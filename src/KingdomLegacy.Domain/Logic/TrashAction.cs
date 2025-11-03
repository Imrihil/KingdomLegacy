namespace KingdomLegacy.Domain.Logic;
internal class TrashAction(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => States.AllNotRemoved;
    public override State TargetState => State.Removed;
    public override int Order => 100;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "x";

    protected override bool ExecuteInternal()
    {
        Description = $"Trashed {card.Id}.";

        return game.ChangeState(card, TargetState);
    }
}
