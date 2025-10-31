namespace KingdomLegacy.Domain.Logic;
internal class PlayAction(Game game, Card card) : ReversibleActionBase(game)
{
    public override State TargetState => State.InPlay;
    public override bool Allowed => card.State == State.Hand;
    public override bool Disabled => false;
    public override string Text => "∞";
    protected override bool ExecuteInternal()
    {
        if (!game._hand.Remove(card))
            return false;

        card.State = State.InPlay;
        game._inPlay.Add(card);

        Description = $"Played {card.Id}.";

        return true;
    }

    protected override bool UndoInternal()
    {
        if (!game._inPlay.Remove(card))
            return false;

        card.State = State.Hand;
        game._hand.Add(card);

        return true;
    }
}
