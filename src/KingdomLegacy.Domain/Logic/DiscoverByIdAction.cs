namespace KingdomLegacy.Domain.Logic;
internal class DiscoverByIdAction(Game game)
    : ReversibleCardActionBase(game)
{
    public override State[] SourceStates => [State.Box];
    public override State TargetState => State.Discovered;
    public override string Text => Card != null ? $"+[{Card?.Id}]" : "⦸";
    private Card? _card;
    protected override Card? Card => _card ?? Game.BoxById(Game.Config.DiscoverId);
    protected override bool ExecuteInternal()
    {
        if (Card == null)
            return false;

        _card = Card;

        Description = $"Discovered {Card.Id}.";

        return Game.ChangeState(Card, TargetState);
    }
}
