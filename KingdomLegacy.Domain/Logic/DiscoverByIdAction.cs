namespace KingdomLegacy.Domain.Logic;
internal class DiscoverByIdAction(Game game, int id) : ReversibleActionBase(game)
{
    public override State TargetState => State.Discovered;
    public override bool Allowed => true;
    public override bool Disabled => !game._box.Any(card => card.Id == id);
    public override string Text => "+";
    private Card? _card;
    protected override bool ExecuteInternal()
    {
        _card = game._box.First(card => card.Id == id);
        if (!game._box.Remove(_card))
            return false;

        _card.State = State.Discovered;
        game._discovered.Add(_card);

        Description = $"Discovered {_card}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        if (_card == null)
            return false;

        if (!game._discovered.Remove(_card))
            return false;

        _card.State = State.Box;
        var index = game._box.FindIndex(card => card.Id > _card.Id);

        if (index < 0)
            game._box.Add(_card);
        else
            game._box.Insert(index, _card);

        return true;
    }
}
