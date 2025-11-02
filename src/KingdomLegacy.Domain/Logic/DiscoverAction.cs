namespace KingdomLegacy.Domain.Logic;
internal class DiscoverAction(Game game, int count = 1) : ReversibleActionBase(game)
{
    public override State[] SourceStates => [State.Box];
    public override State TargetState => State.Discovered;
    public override bool Allowed => true;
    public override bool Disabled => game.BoxCount < count;
    public override string Text => $"+{count}";
    private List<Card> _cards = [];
    protected override bool ExecuteInternal()
    {
        while (count-- > 0 && game._box.Count > 0)
        {
            var card = game._box.First();
            if (!game._box.Remove(card))
            {
                UndoInternal();
                return false;
            }

            card.State = State.Discovered;
            game._discovered.Add(card);

            _cards.Add(card);
        }

        Description = $"Discovered {string.Join(", ", _cards.Select(card => card.Id))}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        foreach (var card in ((IEnumerable<Card>)_cards).Reverse())
        {
            if (!game._discovered.Remove(card))
                return false;

            card.State = State.Box;
            game._box.Insert(0, card);
        }

        return true;
    }
}