namespace KingdomLegacy.Domain.Logic;
internal class TrashAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State[] SourceStates => [State.Discovered, State.Hand, State.InPlay, State.Discarded, State.Permanent];
    public override State TargetState => State.Removed;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "x";

    private List<Card>? _sourceList;
    protected override bool ExecuteInternal()
    {
        if (game._discovered.Remove(card))
            _sourceList = game._discovered;
        else if (game._hand.Remove(card))
            _sourceList = game._hand;
        else if (game._inPlay.Remove(card))
            _sourceList = game._inPlay;
        else if (game._discarded.Remove(card))
            _sourceList = game._discarded;
        else if (game._permanent.Remove(card))
            _sourceList = game._permanent;
        else
            return false;

        card.State = State.Removed;
        game._trash.Insert(0, card);

        Description = $"Trashed {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        if (_sourceList == null || game._trash.Count == 0 || game._trash[0] != card)
            return false;

        game._trash.Remove(card);

        card.State =
            _sourceList == game._discovered ? State.Discovered
            : _sourceList == game._hand ? State.Hand
            : _sourceList == game._inPlay ? State.InPlay
            : _sourceList == game._discarded ? State.Discarded
            : State.Permanent;
        _sourceList.Add(card);

        return true;
    }
}
