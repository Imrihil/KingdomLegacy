namespace KingdomLegacy.Domain.Logic;
internal class UndoAction(Game game) : IAction
{
    public State TargetState => default;
    public bool Allowed => true;
    public bool Disabled => ReversibleAction == null;
    public string Text => "↶";
    public string? Description => null;

    private IReversibleAction? ReversibleAction =>
        game.Actions._history.Count > 0 && game.Actions._history[^1] is IReversibleAction action
        ? action : null;

    private IReversibleAction? _revertedAction;

    public void Execute()
    {
        if (ReversibleAction == null)
            return;

        _revertedAction = ReversibleAction;
        _revertedAction.Undo();
        game.Actions._history.Remove(_revertedAction);
    }
}
