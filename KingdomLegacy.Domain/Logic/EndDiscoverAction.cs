namespace KingdomLegacy.Domain.Logic;
internal class EndDiscoverAction(Game game) : RecordedActionBase(game)
{
    public override State TargetState => State.Hand;
    public override bool Allowed => game._discovered.Count > 0;
    public override bool Disabled => false;
    public override string Text => "End discover";
    protected override bool ExecuteInternal()
    {
        game.Actions.Reshuffle();
        game.Actions.Draw(4);

        return true;
    }
}
