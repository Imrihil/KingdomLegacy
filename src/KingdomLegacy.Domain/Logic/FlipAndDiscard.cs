namespace KingdomLegacy.Domain.Logic;
internal class FlipAndDiscard(Game game, Card card) : IAction
{
    public State[] SourceStates => [State.Played, State.StayInPlay];
    public State TargetState => State.Discarded;
    public int Order => 3;
    public bool Allowed => card.State == State.Played || card.State == State.StayInPlay;
    public bool Disabled => false;
    public string Text => "⇒";
    public string? Description => null;
    public void Execute()
    {
        game.Actions.Flip(card);
        game.Actions.Discard(card);
    }
}
