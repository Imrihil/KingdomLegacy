using System.Reflection;

namespace KingdomLegacy.Domain.Logic;
public class Actions(Game game)
{
    internal List<IAction> _history = [];
    public IReadOnlyCollection<IAction> History => _history.AsReadOnly();
    //internal List<IAction> _undoHistory = [];

    public void Discover(int count) =>
        new DiscoverAction(game, count).Execute();

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

    // TODO: Redo
    // public void Redo()

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

    private static IAction? GetAction(Type type, Game game, Card card)
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

        return Activator.CreateInstance(type, args.ToArray()) as IAction;
    }

    public IAction[] GetCardActions(Card card) =>
        GetAvailableActions(_stateActionTypes[card.State].Select(type => GetAction(type, game, card)).OfType<IAction>());

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
