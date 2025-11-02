namespace KingdomLegacy.Domain.Logic;
internal class TrashAction(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    private State[] _sourceStates = States.All.Where(state => state != State.Removed).ToArray();
    public override State[] SourceStates => _sourceStates;
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
