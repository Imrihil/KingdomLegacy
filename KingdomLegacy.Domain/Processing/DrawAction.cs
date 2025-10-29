namespace KingdomLegacy.Domain.Processing;
internal class DrawAction(Game game, int count) : IAction
{
    public State TargetState => State.Hand;
    public bool Allowed => game.DeckCount >= count;
    public bool Disabled => false;
    public string Text => $"+{count}";

    public bool Execute()
    {
        var i = count;
        while (i-- > 0 && game._deck.Count > 0)
        {
            var card = game._deck.Dequeue();
            card.State = State.Hand;
            game._hand.Add(card);
            if (game._deck.TryPeek(out var nextCard))
                nextCard.State = State.DeckTop;
        }

        return true;
    }
}
