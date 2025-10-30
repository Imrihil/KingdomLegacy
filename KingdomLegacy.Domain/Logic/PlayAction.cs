namespace KingdomLegacy.Domain.Logic;
internal class PlayAction(Game game, Card card) : RecordedActionBase(game)
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
}
