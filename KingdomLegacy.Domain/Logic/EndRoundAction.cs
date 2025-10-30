namespace KingdomLegacy.Domain.Logic;
internal class EndRoundAction(Game game) : RecordedActionBase(game)
{
    public override State TargetState => State.Hand;
    public override bool Allowed => game._discovered.Count == 0 && game._deck.Count == 0;
    public override bool Disabled => false;
    public override string Text => "End round";
    protected override bool ExecuteInternal()
    {
        foreach (var card in game._hand.Concat(game._inPlay).ToArray())
            game.Actions.Discard(card);

        game.Actions.Discover(2);

        return true;
    }
}
