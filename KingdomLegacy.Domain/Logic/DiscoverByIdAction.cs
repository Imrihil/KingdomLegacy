namespace KingdomLegacy.Domain.Logic;
internal class DiscoverByIdAction(Game game, int id) : RecordedActionBase(game)
{
    public override State TargetState => State.Discovered;

    public override bool Allowed => true;

    public override bool Disabled => game._box.Any(card => card.Id == id);

    public override string Text => "+";

    protected override bool ExecuteInternal()
    {
        var card = game._box.First(card => card.Id == id);
        if (!game._box.Remove(card))
            return false;

        card.State = State.Discovered;
        game._discovered.Add(card);

        return true;
    }
}
