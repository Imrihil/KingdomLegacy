namespace KingdomLegacy.Domain.Logic;
internal class DiscoverAction(Game game, int? count = null) : ReversibleActionBase(game)
{
    private int Count => count ?? Game.Config.DiscoverCount;
    public override State[] SourceStates => [State.Box];
    public override State TargetState => State.Discovered;
    public override bool Allowed => true;
    public override bool Disabled => Game.BoxCount < Count;
    public override string Text => $"+{Count}";
    private List<Card> _cards = [];
    protected override bool ExecuteInternal()
    {
        var i = Count;
        while (i-- > 0 && Game.BoxNext is Card card)
            if (Game.ChangeState(card, TargetState))
                _cards.Add(card);

        Description = $"Discovered {string.Join(", ", _cards.Select(card => card.Id))}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        foreach (var card in ((IEnumerable<Card>)_cards).Reverse())
            if (!Game.ChangeState(card, State.Box))
                return false;

        return true;
    }
}