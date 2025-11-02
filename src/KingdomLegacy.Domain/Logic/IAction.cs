namespace KingdomLegacy.Domain.Logic;
public interface IAction
{
    State[] SourceStates { get; }
    State TargetState { get; }
    bool Allowed { get; }
    bool Disabled { get; }
    string Text { get; }
    string? Description { get; }
    void Execute();
}

public interface IReversibleAction : IAction
{
    void Undo();
}