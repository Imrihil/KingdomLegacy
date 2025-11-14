namespace KingdomLegacy.Domain.Logic;
internal class PlaceToDeckBottom(Game game, Card card)
    : ReversibleCardActionBase(game)
{
    public override string Name => "Place on a deck bottom";
    public override State[] SourceStates => [State.StayInPlay, State.Played, State.Discarded];
    public override State TargetState => State.DeckTop;
    public override int Order => 8;
    public override string Text => "⊠";
    protected override Card Card { get; } = card;
    protected override bool ExecuteInternal()
    {
        Description = $"Placed on a deck bottom {Card.Id}.";

        return Game.ChangeState(Card, TargetState, true);
    }
}
