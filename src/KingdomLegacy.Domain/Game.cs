using KingdomLegacy.Domain.Logic;
using System.Text;

namespace KingdomLegacy.Domain;
public class Game
{
    public Actions Actions { get; private set; }
    public GameConfig Config { get; } = new();
    public ExpansionType Expansion { get; set; }

    private Action? _notify;
    public Game(Action? notify = null)
    {
        Actions = new(this);
        _notify = notify;
    }

    public string KingdomName { get; set; } = "";
    public bool IsInitialized { get; internal set; }
    public int Points { get; set; }

    private Dictionary<ExpansionType, List<Card>> _box = new();
    public int BoxCount => _box.TryGetValue(Expansion, out var cards) ? cards.Count : 0;
    internal Card? BoxNext => _box.TryGetValue(Expansion, out var cards) && cards.Count > 0 ? cards[0] : null;
    internal Card? BoxById(int id) =>
        _box.TryGetValue(Expansion, out var cards) ? cards.FirstOrDefault(card => card.Id == id) : null;
    public IReadOnlyCollection<ExpansionType> Expansions => _box.Keys;

    private List<Card> _discovered = [];
    public IReadOnlyCollection<Card> Discovered => _discovered.AsReadOnly();

    private List<Card> _deck = new();
    public int DeckCount => _deck.Count;
    public Card? DeckTop => _deck.Count > 0 ? _deck[0] : null;
    public IReadOnlyCollection<Card> Deck => _deck.Count > 0
        ? new Card[] { _deck[0] }
            .Concat(_deck.Skip(1).OrderBy(card => card.Id))
            .ToList()
            .AsReadOnly()
        : [];
    internal void DeckReshuffle()
    {
        _deck = _deck.OrderBy(_ => Random.Shared.Next()).ToList();

        foreach (var card in _deck)
            card.State = State.Deck;

        if (DeckTop is Card topCard)
            topCard.State = State.DeckTop;
    }
    private void DeckReshuffleExceptTop()
    {
        var top = _deck.FirstOrDefault(card => card.State == State.DeckTop);
        var newDeck = new List<Card>();
        if (top != null)
            newDeck.Add(top);

        newDeck.AddRange(_deck
            .Where(card => card.State != State.DeckTop)
            .OrderBy(_ => Random.Shared.Next()));

        _deck = newDeck;
    }

    private List<Card> _hand = [];
    public IReadOnlyCollection<Card> Hand => _hand.AsReadOnly();

    private List<Card> _inPlay = [];
    public IReadOnlyCollection<Card> InPlay => _inPlay.AsReadOnly();

    private List<Card> _blocked = [];
    public IReadOnlyCollection<Card> Blocked => _blocked.AsReadOnly();

    private List<Card> _discarded = [];
    public Card? DiscardedLast => _discarded.Count > 0 ? _discarded[^1] : null;
    public IReadOnlyCollection<Card> Discarded => _discarded.AsReadOnly();

    private List<Card> _trash = [];
    public Card? TrashedLast => _trash.Count > 0 ? _trash[0] : null;
    public IReadOnlyCollection<Card> Trashed => _trash.ToArray();

    private List<Card> _permanent = [];
    public IReadOnlyCollection<Card> Permanent => _permanent.AsReadOnly();

    private List<Card> _purged = [];
    public Card? PurgedLast => _purged.Count > 0 ? _purged[0] : null;
    public IReadOnlyCollection<Card> Purged => _purged.ToArray();

    internal IEnumerable<Card> All => _box
        .Aggregate((IEnumerable<Card>)[], (cards, source) => cards.Concat(source.Value))
        .Concat(Deck)
        .Concat(_discovered)
        .Concat(_permanent)
        .Concat(_inPlay)
        .Concat(_hand)
        .Concat(_blocked)
        .Concat(_discarded)
        .Concat(_trash)
        .Concat(_purged);

    internal List<Card> List(State state, ExpansionType expansion) => state switch
    {
        State.Box => _box.TryGetValue(expansion, out var cards) ? cards : [],
        State.Discovered => _discovered,
        State.Deck => _deck,
        State.DeckTop => _deck,
        State.Played => _hand,
        State.StayInPlay => _inPlay,
        State.Discarded => _discarded,
        State.Destroyed => _trash,
        State.Permanent => _permanent,
        State.Blocked => _blocked,
        State.Purged => _purged,
        _ => throw new NotImplementedException(),
    };

    internal bool ChangeState(Card card, State state, bool placeOnBottom = false)
    {
        if (!List(card.State, card.Expansion).Remove(card))
            return false;

        var cards = List(state, card.Expansion);
        card.State = state;
        var isReverted = States.AllReverted.Contains(state);
        if (isReverted && !placeOnBottom || !isReverted && placeOnBottom)
            cards.Insert(0, card);
        else
            cards.Add(card);

        if (DeckTop is Card topCard && topCard.State != State.DeckTop)
            topCard.State = State.DeckTop;

        if (state == State.Box)
            cards.Sort();

        return true;
    }

    public bool Load(string data)
    {
        IsInitialized = false;
        _box = Domain.Expansions.All.ToDictionary(expansion => expansion, _ => new List<Card>());
        Actions = new(this);
        return Load(data.Split(Environment.NewLine));
    }

    public bool Load(IEnumerable<string> lines)
    {
        var readPoints = true;
        ExpansionType? expansion = null;
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(KingdomName))
            {
                KingdomName = line.Trim();
                continue;
            }

            if (readPoints)
            {
                var parts = line.Split('\t');
                if (parts.Length > 0 && int.TryParse(parts[0], out var points))
                {
                    Points = points;
                    readPoints = false;
                }
                Expansion = parts.Length > 1 && int.TryParse(parts[1].Trim(), out var value) ? (ExpansionType)value : ExpansionType.FeudalKingdom;
                Config.DiscoverCount = parts.Length > 2 && int.TryParse(parts[2], out var discoverCount) ? discoverCount : 2;
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
                expansion = null;
            else if (expansion == null)
                expansion = (int.TryParse(line.Trim(), out var value) ? (ExpansionType?)value : null)
                    ?? (Enum.TryParse<ExpansionType>(line.Trim(), out var enumValue) ? enumValue : null);
            else
                AddToCollection(GetCard(expansion.Value, line));
        }

        var all = All.ToList();
        foreach (var missingCard in Domain.Expansions.All
            .SelectMany(expansion => expansion.Load())
            .Where(card => !all.Contains(card)))
            _box[missingCard.Expansion].Add(missingCard);

        foreach (var (_, cards) in _box)
            cards.Sort();

        DeckReshuffleExceptTop();
        IsInitialized = true;

        Notify();

        return true;
    }

    private static Card GetCard(ExpansionType expansion, string text)
    {
        var parts = text.Split('\t');
        if (parts.Length < 3)
            throw new FormatException("Invalid card data format.");

        return new Card
        {
            Id = int.Parse(parts[0]),
            Expansion = expansion,
            Orientation = (Orientation)int.Parse(parts[1]),
            State = (State)int.Parse(parts[2]),
            Stickers = parts[3..].Select(GetSticker).ToList()
        };
    }

    private static Sticker GetSticker(string text)
    {
        var parts = text.Split(';');
        if (parts.Length != 3)
            throw new FormatException("Invalid card data format.");

        return new Sticker((StickerType)int.Parse(parts[0]))
        {
            X = int.Parse(parts[1]),
            Y = int.Parse(parts[2])
        };
    }

    private void AddToCollection(Card card) =>
        List(card.State, card.Expansion).Add(card);

    public string Save()
    {
        var sb = new StringBuilder(KingdomName).AppendLine();
        sb.AppendLine($"{Points.ToString()}\t{Expansion}\t{Config.DiscoverCount}");
        sb.Append(string.Join($"{Environment.NewLine}{Environment.NewLine}",
            All.GroupBy(card => card.Expansion)
            .Select(SaveExpansion)));

        return sb.ToString();
    }

    private string SaveExpansion(IGrouping<ExpansionType, Card> expansion)
    {
        var sb = new StringBuilder($"{expansion.Key:d}").AppendLine();
        foreach (var card in expansion)
        {
            sb.Append($"{card.Id}\t{(int)card.Orientation}\t{(int)card.State}");

            foreach (var sticker in card.Stickers)
                sb.Append($"\t{(int)sticker.Type};{sticker.X};{sticker.Y}");

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public bool Initialize(string? name)
    {
        if (name == null)
            return false;

        name = name.ReplaceLineEndings();
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
            name = name.Replace(invalidChar.ToString(), "");

        if (string.IsNullOrWhiteSpace(name))
            return false;

        IsInitialized = false;
        Expansion = ExpansionType.FeudalKingdom;
        Actions = new(this);
        KingdomName = name;
        _box = Domain.Expansions.All.ToDictionary(
            expansion => expansion,
            expansion => expansion.Load());
        _discovered = [];
        _deck = new();
        _permanent = [];
        _inPlay = [];
        _hand = [];
        _blocked = [];
        _discarded = [];
        _trash = [];
        _purged = [];

        Actions.Discover(1);

        return IsInitialized = true;
    }

    public void Insert(Card card1, Card? card2)
    {
        if (card1 == card2)
            return;

        if (card1.State != card2?.State)
            return;

        var list = List(card1.State, card1.Expansion);
        var index1 = list.IndexOf(card1);
        if (list.Remove(card2))
            list.Insert(index1, card2);

        Notify();
    }

    internal void Notify() => _notify?.Invoke();

    public class GameConfig
    {
        public int DiscoverCount { get; set; } = 2;
        public int DiscoverId { get; set; } = 0;
    }
}
