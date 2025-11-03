namespace KingdomLegacy.Domain.Logic;
internal abstract class RecordedActionBase(Game game) : IAction
{
    public abstract State[] SourceStates { get; }
    public abstract State TargetState { get; }
    public virtual int Order => 0;
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

internal abstract class ReversibleCardActionBase(Game game, Card? card) : RecordedActionBase(game), IReversibleAction
{
    protected State SourceState { get; set; }
    protected int SourceIndex { get; set; }
    protected Card Card { get; } = card;

    public new void Execute()
    {
        if (card == null)
            return;

        SourceState = card.State;
        SourceIndex = game.List(SourceState).IndexOf(card);

        if (!ExecuteInternal())
            return;

        game.Actions._history.Add(this);
        game.Notify();
    }

    public void Undo()
    {
        if (!UndoInternal())
            return;

        game.Actions._history.Remove(this);
        game.Notify();
    }

    protected virtual bool UndoInternal()
    {
        if (card != null && game.List(TargetState).Remove(card))
        {
            game.List(SourceState).Insert(SourceIndex, card);
            card.State = SourceState;
            return true;
        }

        return false;
    }
}

internal abstract class ReversibleActionBase(Game game) : RecordedActionBase(game), IReversibleAction
{
    public new void Execute()
    {
        if (!ExecuteInternal())
            return;

        game.Actions._history.Add(this);
        game.Notify();
    }

    public void Undo()
    {
        if (!UndoInternal())
            return;

        game.Actions._history.Remove(this);
        game.Notify();
    }

    protected abstract bool UndoInternal();
}