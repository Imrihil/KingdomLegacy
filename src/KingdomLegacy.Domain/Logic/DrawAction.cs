namespace KingdomLegacy.Domain.Logic;
internal class DrawAction(Game game, int count) : RecordedActionBase(game)
{
    public override State TargetState => State.Hand;
    public override bool Allowed => game.DeckCount >= count;
    public override bool Disabled => false;
    public override string Text => $"+{count}";
    private List<Card> _cards = [];
    protected override bool ExecuteInternal()
    {
        var i = count;
        while (i-- > 0 && game._deck.Count > 0)
        {
            var card = game._deck.Dequeue();
            card.State = State.Hand;
            game._hand.Add(card);
            if (game._deck.TryPeek(out var nextCard))
                nextCard.State = State.DeckTop;

            _cards.Add(card);
        }

        _cards.Sort();
        Description = $"Drew {string.Join(", ", _cards.Select(card => card.Id))}.";

        return true;
    }
}
