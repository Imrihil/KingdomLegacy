namespace KingdomLegacy.Domain.Logic;
internal class Draw1Action(Game game) : DrawAction(game, 1);
internal class Draw2Action(Game game) : DrawAction(game, 2);
internal class Draw4Action(Game game) : DrawAction(game, 4);
internal class Draw8Action(Game game) : DrawAction(game, 8);
internal abstract class DrawAction(Game game, int count) : RecordedActionBase(game)
{
    public override string Name => "Play";
    public override State[] SourceStates => [State.DeckTop];
    public override State TargetState => State.Played;
    public override bool Allowed => Game.DeckCount >= count;
    public override bool Disabled => false;
    public override string Text => $"+{count}";
    private List<Card> _cards = [];
    protected override bool ExecuteInternal()
    {
        var i = count;
        while (i-- > 0 && Game.DeckTop is Card card)
            if (Game.ChangeState(card, TargetState))
                _cards.Add(card);

        Game.Resources.Reset();

        Description = $"Played {string.Join(", ", _cards.Select(card => card.Id))}.";

        return true;
    }
}
