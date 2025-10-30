namespace KingdomLegacy.Domain.Logic;
internal class EndTurnAction(Game game) : RecordedActionBase(game)
{
    public override State TargetState => State.Hand;
    public override bool Allowed => game._discovered.Count == 0 && game._deck.Count > 0;
    public override bool Disabled => false;
    public override string Text => "End turn";
    protected override bool ExecuteInternal()
    {
        foreach (var card in game._hand.ToArray())
            game.Actions.Discard(card);

        game.Actions.Draw(4);

        return true;
    }
}
