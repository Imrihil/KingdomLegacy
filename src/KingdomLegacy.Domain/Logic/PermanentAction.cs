namespace KingdomLegacy.Domain.Logic;
internal class PermanentAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State TargetState => State.Permanent;
    public override bool Allowed => card.State == State.Discovered || card.State == State.InPlay;
    public override bool Disabled => false;
    public override string Text => "∞";

    private List<Card>? _sourceList;
    protected override bool ExecuteInternal()
    {
        if (game._discovered.Remove(card))
            _sourceList = game._discovered;
        else if (game._inPlay.Remove(card))
            _sourceList = game._inPlay;

        card.State = State.Permanent;
        game._permanent.Add(card);

        Description = $"Added to permanent {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        if (_sourceList == null || !game._permanent.Remove(card))
            return false;

        card.State = _sourceList == game._discovered
            ? State.Discovered : State.InPlay;
        _sourceList.Add(card);

        return true;
    }
}
