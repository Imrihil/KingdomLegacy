namespace KingdomLegacy.Domain.Logic;
internal class PermanentAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State TargetState => State.Permanent;
    public override bool Allowed => card.State == State.Discovered;
    public override bool Disabled => false;
    public override string Text => "∞";

    protected override bool ExecuteInternal()
    {
        if (!game._discovered.Remove(card))
            return false;

        card.State = State.Permanent;
        game._permanent.Add(card);

        Description = $"Added to permanent {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        if (!game._permanent.Remove(card))
            return false;

        card.State = State.Discovered;
        game._discovered.Add(card);

        return true;
    }
}
