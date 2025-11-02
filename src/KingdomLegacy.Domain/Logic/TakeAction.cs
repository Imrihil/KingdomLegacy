namespace KingdomLegacy.Domain.Logic;
internal class TakeAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State[] SourceStates => [State.Discovered, State.InPlay, State.Discarded, State.Removed];
    public override State TargetState => State.Hand;
    public override bool Allowed => card.State != State.Removed || game._trash.Count > 0 && game._trash[0] == card;
    public override bool Disabled => false;
    public override string Text => card.State == State.Discarded || card.State == State.Removed ? "↺" : "✓";

    private List<Card>? _sourceList;

    protected override bool ExecuteInternal()
    {
        if (game._discovered.Remove(card))
            _sourceList = game._discovered;
        else if (game._inPlay.Remove(card))
            _sourceList = game._inPlay;
        else if (game._discarded.Remove(card))
            _sourceList = game._discarded;
        else if (game._trash.Remove(card))
            _sourceList = game._trash;
        else
            return false;

        card.State = State.Hand;
        game._hand.Add(card);

        Description = $"Took into hand: {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        if (_sourceList == null || !game._hand.Remove(card))
            return false;

        card.State =
            _sourceList == game._discovered ? State.Discovered
            : _sourceList == game._hand ? State.Hand
            : _sourceList == game._inPlay ? State.InPlay
            : State.Removed;

        if (_sourceList == game._trash)
            _sourceList.Insert(0, card);
        else
            _sourceList?.Add(card);

        return true;
    }
}
