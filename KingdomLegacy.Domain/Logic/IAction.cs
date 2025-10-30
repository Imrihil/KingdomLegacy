namespace KingdomLegacy.Domain.Logic;
public interface IAction
{
    State TargetState { get; }
    bool Allowed { get; }
    bool Disabled { get; }
    string Text { get; }
    void Execute();
    // TODO: Undo
    //void Undo();
}