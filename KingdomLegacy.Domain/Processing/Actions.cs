namespace KingdomLegacy.Domain.Processing;
public class Actions(Game game)
{
    private List<IAction> _history = [];
    //private List<IAction> _undoHistory = [];

    public void Discover(int count) =>
        Execute(new DiscoverAction(game, count));

    public void DiscoverById(int id) =>
        Execute(new DiscoverByIdAction(game, id));

    public void Reshuffle() =>
        Execute(new ReshuffleAction(game));

    public void Take(Card card) =>
        Execute(new TakeAction(game, card));

    public void Play(Card card) =>
        Execute(new PlayAction(game, card));

    public void Discard(Card card) =>
        Execute(new DiscardAction(game, card));

    public void Trash(Card card) =>
        Execute(new TrashAction(game, card));

    public void Draw(int count) =>
        Execute(new DrawAction(game, count));

    public void Execute(IAction action)
    {
        if (!action.Execute())
            return;

        _history.Add(action);
        game.Notify();
    }

    // TODO: Undo
    // public void Undo()

    // TODO: Redo
    // public void Redo()
}
