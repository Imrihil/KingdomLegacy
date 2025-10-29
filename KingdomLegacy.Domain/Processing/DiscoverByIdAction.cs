namespace KingdomLegacy.Domain.Processing;
internal class DiscoverByIdAction(Game game, int id) : IAction
{
    public State TargetState => State.Discovered;

    public bool Allowed => true;

    public bool Disabled => game._box.Any(card => card.Id == id);

    public string Text => "+";

    public bool Execute()
    {
        var card = game._box.First(card => card.Id == id);
        if (!game._box.Remove(card))
            return false;

        card.State = State.Discovered;
        game._discovered.Add(card);

        return true;
    }
}
