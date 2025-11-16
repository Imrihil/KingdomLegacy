using System.Reflection;

namespace KingdomLegacy.Domain.Logic;
public class Actions(Game game)
{
    internal List<IAction> _history = [];
    public IReadOnlyCollection<IAction> History => _history.AsReadOnly();
    //internal List<IAction> _undoHistory = [];

    public void Discover(int? count = null) =>
        new DiscoverAction(game, count).Execute();

    public void EndDiscover(IStorage storage) =>
        new EndDiscoverAction(game, storage).Execute();

    public void Reshuffle() =>
        new ReshuffleAction(game).Execute();

    public void Discard(Card card) =>
        new DiscardAction(game, card).Execute();

    public void Draw4() =>
        new Draw4Action(game).Execute();

    public void Flip(Card card) =>
        new FlipAction(game, card).Execute();

    public void Rotate(Card card) =>
        new RotateAction(game, card).Execute();

    public void Reset(Card card) =>
        new ResetAction(game, card).Execute();

    private static readonly Type[] _allActionTypes = Assembly.GetAssembly(typeof(IAction))?
        .GetTypes().Where(type => type.IsAssignableTo(typeof(IAction)) && !type.IsAbstract).ToArray() ?? [];

    private static readonly Dictionary<State, Type[]> _stateActionTypes = States.All.ToDictionary(
        state => state,
        StateActionTypes);

    private static Type[] StateActionTypes(State state) => _allActionTypes
        .Select(type => GetExampleAction(type, state))
        .OfType<IAction>()
        .Where(action => action.SourceStates.Contains(state))
        .OrderBy(action => action.TargetState.Order())
        .ThenBy(action => action.Order)
        .Select(action => action.GetType())
        .ToArray();

    private static IAction? GetExampleAction(Type type, State state)
    {
        try
        {
            return GetAction(type, new Game(), new Card() { State = state });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    private static readonly IStorage NullStorage = new NullStorage();
    private static IAction? GetAction(Type type, Game game, Card card, IStorage? storage = null)
    {
        var constructorParameters = type.GetConstructors().FirstOrDefault()?.GetParameters() ?? [];
        var args = new List<object>();
        if (constructorParameters.Any(param => param.ParameterType == typeof(Game)))
            args.Add(game);

        if (constructorParameters.Any(param => param.ParameterType == typeof(Card)))
            args.Add(card);

        if (constructorParameters.Any(param => param.ParameterType == typeof(int)))
            args.Add(1);

        if (constructorParameters.Any(param => param.ParameterType == typeof(Resources)))
            args.Add(new Resources(game));

        if (constructorParameters.Any(param => param.ParameterType == typeof(IStorage)))
            args.Add(storage ?? NullStorage);

        return Activator.CreateInstance(type, args.ToArray()) as IAction;
    }

    public IAction[] GetCardActions(Card card) =>
        GetAvailableActions(_stateActionTypes[card.State].Select(type => GetAction(type, game, card)).OfType<IAction>());

    public IAction[] GetDeckActions() => GetAvailableActions([new ReshuffleAction(game)]);

    public IAction[] GetMainActions(IStorage storage) => GetAvailableActions(
        [new UndoAction(game), new EndDiscoverAction(game, storage), new EndTurnAction(game, storage), new EndRoundAction(game, storage)]);

    public IAction GetDiscover() => 
        new DiscoverAction(game);

    public IAction GetDiscoverById() => 
        new DiscoverByIdAction(game);

    private IAction[] GetAvailableActions(IEnumerable<IAction> actions) => actions
        .Where(action => action.Allowed)
        .ToArray();

    internal void Clear() => _history.Clear();
}
