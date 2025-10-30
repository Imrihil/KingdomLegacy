namespace KingdomLegacy.Domain.Logic;
internal class DiscardAction(Game game, Card card) : RecordedActionBase(game), IReversibleAction
{
    public override State TargetState => State.Discarded;
    public override bool Allowed => card.State == State.Discovered || card.State == State.Hand || card.State == State.InPlay;
    public override bool Disabled => false;
    public override string Text => "✓";

    private List<Card>? _sourceList;

    protected override bool ExecuteInternal()
    {
        if (game._discovered.Remove(card))
            _sourceList = game._discovered;
        else if (game._hand.Remove(card))
            _sourceList = game._hand;
        else if (game._inPlay.Remove(card))
            _sourceList = game._inPlay;
        else
            return false;

        card.State = State.Discarded;
        game._discarded.Add(card);

        Description = $"Discarded {card.Id}.";

        return true;
    }

    public void Undo()
    {
        if (_sourceList == null || !game._discarded.Remove(card))
            return;

        card.State =
            _sourceList == game._discovered ? State.Discovered
            : _sourceList == game._hand ? State.Hand
            : State.InPlay;
        _sourceList.Add(card);
    }
}
