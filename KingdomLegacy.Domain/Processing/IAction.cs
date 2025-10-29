namespace KingdomLegacy.Domain.Processing;
public interface IAction
{
    State TargetState { get; }
    bool Allowed { get; }
    bool Disabled { get; }
    string Text { get; }
    bool Execute();
    // TODO: Undo
    //void Undo();
}