namespace KingdomLegacy.Domain.Logic;
public class Actions(Game game)
{
    internal List<IAction> _history = [];
    //internal List<IAction> _undoHistory = [];

    public void Discover(int count) =>
        new DiscoverAction(game, count).Execute();

    public void DiscoverById(int id) =>
        new DiscoverByIdAction(game, id).Execute();

    public void Reshuffle() =>
        new ReshuffleAction(game).Execute();

    public void Take(Card card) =>
        new TakeAction(game, card).Execute();

    public void Play(Card card) =>
        new PlayAction(game, card).Execute();

    public void Discard(Card card) =>
        new DiscardAction(game, card).Execute();

    public void Trash(Card card) =>
        new TrashAction(game, card).Execute();

    public void Draw(int count) =>
        new DrawAction(game, count).Execute();

    public void EndDiscover() =>
        new EndDiscoverAction(game).Execute();

    public void EndTurn() =>
        new EndTurnAction(game).Execute();

    public void EndRound() =>
        new EndRoundAction(game).Execute();

    // TODO: Undo
    // public void Undo()

    // TODO: Redo
    // public void Redo()

    public IAction[] GetAvailableActions(Card card) =>
        GetActions(card)
        .Where(action => action.Allowed)
        .ToArray();

    public IAction[] GetActions(Card card) => card.State switch
    {
        State.Box => [],
        State.Discovered => [new RorateResetAction(game, card), new RotateRightAction(game, card), new RotateDownAction(game, card), new TakeAction(game, card), new DiscardAction(game, card), new TrashAction(game, card)],
        State.Deck => [],
        State.DeckTop => [new DrawAction(game, 1), new DrawAction(game, 2), new DrawAction(game, 4)],
        State.Hand => [new PlayAction(game, card), new RotateRightAction(game, card), new RotateDownAction(game, card), new DiscardAction(game, card), new TrashAction(game, card)],
        State.InPlay => [new RotateRightAction(game, card), new RotateDownAction(game, card), new DiscardAction(game, card), new TrashAction(game, card)],
        State.Discarded => [new TakeAction(game, card)],
        State.Removed => [new TakeAction(game, card)],
        _ => []
    };
}
