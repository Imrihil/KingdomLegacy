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

        Game.Actions._history.Add(this);
        Game.Notify();
    }

    protected Game Game { get; } = game;
    protected abstract bool ExecuteInternal();
}

internal abstract class ReversibleCardActionBase(Game game) : RecordedActionBase(game), IReversibleAction
{
    protected State SourceState { get; set; }
    protected int SourceIndex { get; set; }
    public override bool Allowed => true;
    public override bool Disabled => Card == null;
    protected abstract Card? Card { get; }

    public new void Execute()
    {
        if (!Allowed || Disabled || Card == null)
            return;

        SourceState = Card.State;
        SourceIndex = Game.List(SourceState, Card.Expansion).IndexOf(Card);

        if (!ExecuteInternal())
            return;

        Game.Actions._history.Add(this);
        Game.Notify();
    }

    public void Undo()
    {
        if (!UndoInternal())
            return;

        Game.Actions._history.Remove(this);
        Game.Notify();
    }

    protected virtual bool UndoInternal()
    {
        if (Card != null && Game.List(TargetState, Card.Expansion).Remove(Card))
        {
            Game.List(SourceState, Card.Expansion).Insert(SourceIndex, Card);
            Card.State = SourceState;
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

        Game.Actions._history.Add(this);
        Game.Notify();
    }

    public void Undo()
    {
        if (!UndoInternal())
            return;

        Game.Actions._history.Remove(this);
        Game.Notify();
    }

    protected abstract bool UndoInternal();
}