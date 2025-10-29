namespace KingdomLegacy.Domain.Processing;
internal class DiscoverAction(Game game, int count) : IAction
{
    public State TargetState => State.Discovered;

    public bool Allowed => true;

    public bool Disabled => game.BoxCount >= count;

    public string Text => $"+{count}";

    public bool Execute()
    {
        while (count-- > 0 && game._box.Count > 0)
        {
            var card = game._box.First();
            if (!game._box.Remove(card))
                return false;

            card.State = State.Discovered;
            game._discovered.Add(card);
        }

        return true;
    }
}
