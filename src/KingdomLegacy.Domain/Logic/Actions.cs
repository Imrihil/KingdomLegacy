using System.Reflection;

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

    public void Draw4() =>
        new Draw4Action(game).Execute();

    public void EndDiscover() =>
        new EndDiscoverAction(game).Execute();

    public void EndTurn(Resources resources) =>
        new EndTurnAction(game, resources).Execute();

    public void EndRound(Resources resources) =>
        new EndRoundAction(game, resources).Execute();

    public void Flip(Card card) =>
        new FlipAction(game, card).Execute();

    public void Rotate(Card card) =>
        new RotateAction(game, card).Execute();

    public void rientationReset(Card card) =>
        new OrientationResetAction(game, card).Execute();

    public void Undo() =>
        new UndoAction(game).Execute();

    // TODO: Redo
    // public void Redo()

    private static readonly Type[] _allActionTypes = Assembly.GetAssembly(typeof(IAction))?.GetTypes().Where(type => type.IsAssignableTo(typeof(IAction)) && !type.IsAbstract).ToArray() ?? [];
    private static readonly IAction[] _allActions = _allActionTypes.Select(GetExampleAction).OfType<IAction>().ToArray();
    private static IAction? GetExampleAction(Type type)
    {
        try
        {
            return GetAction(type);
        }
        catch
        {
            return null;
        }
    }

    private static IAction? GetAction(Type type, Game? game = null, Card? card = null) => Activator.CreateInstance(type, game ?? new Game(), card ?? new Card()) as IAction;
    private static readonly Dictionary<State, Type[]> _stateActionTypes = States.All.ToDictionary(
        state => state,
        state => _allActions.Where(action => action.SourceStates.Contains(state)).Select(action => action.GetType()).ToArray());

    public IAction[] GetCardActions(Card card) => GetAvailableActions(_stateActionTypes[card.State].Select(type => GetAction(type, game, card)).OfType<IAction>());

    public IAction[] GetBoxActions() => GetAvailableActions(
        [new DiscoverAction(game, 1), new DiscoverAction(game, 2), new DiscoverAction(game, 5), new DiscoverAction(game, 10)]);

    public IAction[] GetBoxSpecialActions(int id) => GetAvailableActions(
        [new DiscoverByIdAction(game, id)]);

    public IAction[] GetDeckActions() => GetAvailableActions([new ReshuffleAction(game)]);

    public IAction[] GetMainActions(Resources resources) => GetAvailableActions(
        [new UndoAction(game), new EndDiscoverAction(game), new EndTurnAction(game, resources), new EndRoundAction(game, resources)]);

    private IAction[] GetAvailableActions(IEnumerable<IAction> actions) => actions
        .Where(action => action.Allowed)
        .ToArray();
}
