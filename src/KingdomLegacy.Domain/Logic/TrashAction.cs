namespace KingdomLegacy.Domain.Logic;
internal class TrashAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State TargetState => State.Removed;
    public override bool Allowed => card.State == State.Discovered || card.State == State.Hand || card.State == State.InPlay || card.State == State.Discarded || card.State == State.Permanent;
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
        game._trash.Push(card);

        Description = $"Trashed {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        if (_sourceList == null || !game._trash.TryPop(out var poppedCard))
            return false;

        if (poppedCard != card)
        {
            game._trash.Push(poppedCard);
            return false;
        }

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
