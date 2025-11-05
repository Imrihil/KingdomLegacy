namespace KingdomLegacy.Domain.Logic;
internal class PurgeAction(Game game, Card card)
    : ReversibleCardActionBase(game, card)
{
    private State[] _sourceStates = [State.Permanent, State.Hand];
    public override State[] SourceStates => _sourceStates;
    public override State TargetState => State.Purged;
    public override int Order => 100;
    public override string Text => "★";

    protected override bool ExecuteInternal()
    {
        Description = $"Purged {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
