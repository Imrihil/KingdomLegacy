namespace KingdomLegacy.Domain.Logic;
internal abstract class RecordedActionBase(Game game) : IAction
{
    public abstract State[] SourceStates { get; }
    public abstract State TargetState { get; }
    public abstract bool Allowed { get; }
    public abstract bool Disabled { get; }
    public abstract string Text { get; }
    public string? Description { get; protected set; }
    public void Execute()
    {
        if (!ExecuteInternal())
            return;

        game.Actions._history.Add(this);
        game.Notify();
    }

    protected abstract bool ExecuteInternal();
}

internal abstract class ReversibleActionBase(Game game) : RecordedActionBase(game), IReversibleAction
{
    public void Undo()
    {
        if (!UndoInternal())
            return;

        game.Actions._history.Remove(this);
        game.Notify();
    }

    protected abstract bool UndoInternal();
}