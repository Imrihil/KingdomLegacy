namespace KingdomLegacy.Domain.Logic;
internal class DiscardAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State[] SourceStates => [State.Discovered, State.Hand, State.InPlay];
    public override State TargetState => State.Discarded;
    public override bool Allowed => true;
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

    protected override bool UndoInternal()
    {
        if (_sourceList == null || !game._discarded.Remove(card))
            return false;

        card.State =
            _sourceList == game._discovered ? State.Discovered
            : _sourceList == game._hand ? State.Hand
            : State.InPlay;
        _sourceList.Add(card);

        return true;
    }
}
