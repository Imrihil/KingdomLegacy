using KingdomLegacy.Domain.Logic;
using System.Text;

namespace KingdomLegacy.Domain;
public class Game
{
    public Actions Actions { get; private set; }
    public Resources Resources { get; private set; }
    public GameConfig Config { get; } = new();
    public ExpansionType Expansion { get; set; }

    private Action? _notify;
    public Game(Action? notify = null)
    {
        Actions = new(this);
        Resources = new(this);
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

    private List<Card> _played = [];
    public IReadOnlyCollection<Card> Played => _played.AsReadOnly();

    private List<Card> _stayInPlay = [];
    public IReadOnlyCollection<Card> StayInPlay => _stayInPlay.AsReadOnly();

    private List<Card> _blocked = [];
    public IReadOnlyCollection<Card> Blocked => _blocked.AsReadOnly();

    private List<Card> _discarded = [];
    public Card? DiscardedLast => _discarded.Count > 0 ? _discarded[^1] : null;
    public IReadOnlyCollection<Card> Discarded => _discarded.AsReadOnly();

    private List<Card> _destroyed = [];
    public Card? DestroyedLast => _destroyed.Count > 0 ? _destroyed[0] : null;
    public IReadOnlyCollection<Card> Destroyed => _destroyed.ToArray();

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
        .Concat(_stayInPlay)
        .Concat(_played)
        .Concat(_blocked)
        .Concat(_discarded)
        .Concat(_destroyed)
        .Concat(_purged);

    internal List<Card> List(State state, ExpansionType expansion) => state switch
    {
        State.Box => _box.TryGetValue(expansion, out var cards) ? cards : [],
        State.Discovered => _discovered,
        State.Deck => _deck,
        State.DeckTop => _deck,
        State.Played => _played,
        State.StayInPlay => _stayInPlay,
        State.Discarded => _discarded,
        State.Destroyed => _destroyed,
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
        if ((state == State.Deck || state == State.DeckTop) && DeckTop is Card oldTopCard)
            oldTopCard.State = State.Deck;

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
        Resources = new(this);
        return data.Contains($"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}")
            ? LoadV1(data.Split(Environment.NewLine))
            : LoadV2(data.Split(Environment.NewLine));
    }

    public bool LoadV1(IEnumerable<string> lines)
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

    public bool LoadV2(IEnumerable<string> lines)
    {
        var readPoints = true;
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

            if (!string.IsNullOrWhiteSpace(line))
                AddToCollection(GetCard(null, line));
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

    private static Card GetCard(ExpansionType? expansion, string text)
    {
        if (expansion == null)
        {
            var parts = text.Split('\t');
            if (parts.Length < 4)
                throw new FormatException("Invalid card data format.");

            return new Card
            {
                Id = int.Parse(parts[1]),
                Expansion = (ExpansionType)int.Parse(parts[0]),
                Orientation = (Orientation)int.Parse(parts[2]),
                State = (State)int.Parse(parts[3]),
                Stickers = parts[4..].Select(GetSticker).ToList()
            };
        }
        else
        {
            var parts = text.Split('\t');
            if (parts.Length < 3)
                throw new FormatException("Invalid card data format.");

            return new Card
            {
                Id = int.Parse(parts[0]),
                Expansion = expansion.Value,
                Orientation = (Orientation)int.Parse(parts[1]),
                State = (State)int.Parse(parts[2]),
                Stickers = parts[3..].Select(GetSticker).ToList()
            };
        }
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
        foreach (var card in All)
        {
            sb.Append($"{card.Expansion:d}\t{card.Id}\t{(int)card.Orientation}\t{(int)card.State}");

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
        Resources = new(this);
        KingdomName = name;
        _box = Domain.Expansions.All.ToDictionary(
            expansion => expansion,
            expansion => expansion.Load());
        _discovered = [];
        _deck = new();
        _permanent = [];
        _stayInPlay = [];
        _played = [];
        _blocked = [];
        _discarded = [];
        _destroyed = [];
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
