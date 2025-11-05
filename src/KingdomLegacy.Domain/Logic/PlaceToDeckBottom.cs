namespace KingdomLegacy.Domain.Logic;
internal class PlaceToDeckBottom(Game game, Card card)
    : ReversibleCardActionBase(game, card)
{
    public override State[] SourceStates => [State.Discarded];
    public override State TargetState => State.DeckTop;
    public override int Order => 8;
    public override string Text => "⊠";
    protected override bool ExecuteInternal()
    {
        Description = $"Placed on deck bottom {Card.Id}.";

        return Game.ChangeState(Card, TargetState, true);
    }
}
