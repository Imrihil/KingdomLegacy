namespace KingdomLegacy.Domain.Logic;
internal class PlayAction(Game game, Card card) : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Hand];
    public override State TargetState => State.InPlay;
    public override bool Allowed => true;
    public override bool Disabled => false;
    public override string Text => "∞";
    protected override bool ExecuteInternal()
    {
        Description = $"Played {card.Id}.";

        return game.ChangeState(card, TargetState);
    }
}
