namespace KingdomLegacy.Domain.Logic;
internal class DiscoverAction(Game game, int count) : RecordedActionBase(game)
{
    public override State TargetState => State.Discovered;
    public override bool Allowed => true;
    public override bool Disabled => game.BoxCount >= count;
    public override string Text => $"+{count}";

    protected override bool ExecuteInternal()
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