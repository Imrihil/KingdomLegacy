namespace KingdomLegacy.Domain.Logic;
internal class Draw1Action(Game game) : DrawAction(game, 1);
internal class Draw2Action(Game game) : DrawAction(game, 2);
internal class Draw4Action(Game game) : DrawAction(game, 4);
internal abstract class DrawAction(Game game, int count) : RecordedActionBase(game)
{
    public override State[] SourceStates => [State.DeckTop];
    public override State TargetState => State.Hand;
    public override bool Allowed => game.DeckCount >= count;
    public override bool Disabled => false;
    public override string Text => $"+{count}";
    private List<Card> _cards = [];
    protected override bool ExecuteInternal()
    {
        var i = count;
        while (i-- > 0 && game.DeckTop is Card card)
            if (game.ChangeState(card, TargetState))
                _cards.Add(card);

        Description = $"Drew {string.Join(", ", _cards.Select(card => card.Id))}.";

        return true;
    }
}
