namespace KingdomLegacy.Domain.Logic;
public class Actions(Game game)
{
    internal List<IAction> _history = [];
    public IReadOnlyCollection<IAction> History => _history.AsReadOnly();
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

    public void EndTurn(Resources resources) =>
        new EndTurnAction(game, resources).Execute();

    public void EndRound(Resources resources) =>
        new EndRoundAction(game, resources).Execute();

    public void RotateRight(Card card) =>
        new RotateRightAction(game, card).Execute();

    public void RotateDown(Card card) =>
        new RotateDownAction(game, card).Execute();

    public void RorateReset(Card card) =>
        new RorateResetAction(game, card).Execute();

    // TODO: Undo
    // public void Undo()

    // TODO: Redo
    // public void Redo()

    public IAction[] GetCardActions(Card card) => GetAvailableActions(card.State switch
    {
        State.Box => [],
        State.Discovered => [new RorateResetAction(game, card), new RotateRightAction(game, card), new RotateDownAction(game, card), new TakeAction(game, card), new DiscardAction(game, card), new TrashAction(game, card)],
        State.Deck => [],
        State.DeckTop => [new DrawAction(game, 1), new DrawAction(game, 2), new DrawAction(game, 4)],
        State.Hand => [new PlayAction(game, card), new RotateRightAndDiscard(game, card), new RotateDownAndDiscardAction(game, card), new DiscardAction(game, card), new TrashAction(game, card)],
        State.InPlay => [new RotateRightAndDiscard(game, card), new RotateDownAndDiscardAction(game, card), new DiscardAction(game, card), new TrashAction(game, card)],
        State.Discarded => [new TakeAction(game, card)],
        State.Removed => [new TakeAction(game, card)],
        _ => []
    });

    public IAction[] GetBoxActions() => GetAvailableActions(
        [new DiscoverAction(game, 1), new DiscoverAction(game, 2), new DiscoverAction(game, 5), new DiscoverAction(game, 10)]);

    public IAction[] GetBoxSpecialActions(int id) => GetAvailableActions(
        [new DiscoverByIdAction(game, id)]);

    public IAction[] GetDeckActions() => GetAvailableActions([new ReshuffleAction(game)]);

    public IAction[] GetMainActions(Resources resources) => GetAvailableActions(
        [new EndDiscoverAction(game), new EndTurnAction(game, resources), new EndRoundAction(game, resources)]);

    private IAction[] GetAvailableActions(IEnumerable<IAction> actions) =>
        actions
        .Where(action => action.Allowed)
        .ToArray();
}
