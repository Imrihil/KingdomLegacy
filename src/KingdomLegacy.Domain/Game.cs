using KingdomLegacy.Domain.Logic;
using System.Text;

namespace KingdomLegacy.Domain;
public class Game : Observable<Game>
{
    public readonly Actions Actions;
    public Game()
    {
        Actions = new(this);
    }

    public string KingdomName { get; set; } = "";
    public bool IsInitialized { get; internal set; }
    public int Points { get; set; }

    private List<Card> _box = new();
    public int BoxCount => _box.Count;
    internal Card? BoxNext => _box.Count > 0 ? _box[0] : null;
    internal Card? BoxById(int id) => 
        _box.FirstOrDefault(card => card.Id == id);

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

    internal IEnumerable<Card> All => _box
        .Concat(Deck)
        .Concat(_discovered)
        .Concat(_permanent)
        .Concat(_inPlay)
        .Concat(_hand)
        .Concat(_blocked)
        .Concat(_discarded)
        .Concat(_trash);

    internal List<Card> List(State state) => state switch
    {
        State.Box => _box,
        State.Discovered => _discovered,
        State.Deck => _deck,
        State.DeckTop => _deck,
        State.Hand => _hand,
        State.InPlay => _inPlay,
        State.Discarded => _discarded,
        State.Removed => _trash,
        State.Permanent => _permanent,
        State.Blocked => _blocked,
        _ => throw new NotImplementedException(),
    };

    internal bool ChangeState(Card card, State state)
    {
        if (!List(card.State).Remove(card))
            return false;

        card.State = state;
        if (States.AllReverted.Contains(state))
            List(state).Insert(0, card);
        else
            List(state).Add(card);

        if (DeckTop is Card topCard && topCard.State != State.DeckTop)
            topCard.State = State.DeckTop;

        return true;
    }

    public void Load(string data) =>
        Load(data.Split(Environment.NewLine));

    public void Load(IEnumerable<string> lines)
    {
        var readPoints = true;
        string? expansion = null;
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(KingdomName))
            {
                KingdomName = line.Trim();
                continue;
            }

            if (readPoints)
            {
                if (int.TryParse(line, out var points))
                {
                    Points = points;
                    readPoints = false;
                }
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
                expansion = null;
            else if (expansion == null)
                expansion = line.Trim();
            else
                AddToCollection(GetCard(expansion, line));
        }

        DeckReshuffleExceptTop();
        IsInitialized = true;

        Notify();
    }

    private static Card GetCard(string expansion, string text)
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

    private void AddToCollection(Card card)
    {
        switch (card.State)
        {
            case State.Box:
                _box.Add(card);
                break;
            case State.Discovered:
                _discovered.Add(card);
                break;
            case State.Deck:
                _deck.Add(card);
                break;
            case State.DeckTop:
                _deck.Add(card);
                break;
            case State.Hand:
                _hand.Add(card);
                break;
            case State.InPlay:
                _inPlay.Add(card);
                break;
            case State.Discarded:
                _discarded.Add(card);
                break;
            case State.Removed:
                _trash.Add(card);
                break;
            case State.Permanent:
                _permanent.Add(card);
                break;
            case State.Blocked:
                _blocked.Add(card);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public string Save()
    {
        var sb = new StringBuilder(KingdomName).AppendLine();
        sb.AppendLine(Points.ToString());
        sb.Append(string.Join($"{Environment.NewLine}{Environment.NewLine}",
            All.GroupBy(card => card.Expansion)
            .Select(SaveExpansion)));

        return sb.ToString();
    }

    private string SaveExpansion(IGrouping<string, Card> expansion)
    {
        var sb = new StringBuilder(expansion.Key).AppendLine();
        foreach (var card in expansion)
        {
            sb.Append($"{card.Id}\t{(int)card.Orientation}\t{(int)card.State}");
            foreach (var sticker in card.Stickers)
                sb.Append($"\t{(int)sticker.Type};{sticker.X};{sticker.Y}");

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public void Initialize(string? name, Expansion expansion)
    {
        if (name == null)
            return;

        name = name.ReplaceLineEndings();
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
            name = name.Replace(invalidChar.ToString(), "");

        if (string.IsNullOrWhiteSpace(name))
            return;

        Clear();
        KingdomName = name;
        _box = expansion.Cards.ToList();

        IsInitialized = true;

        Actions.Discover(1);
    }

    public void Clear()
    {
        KingdomName = "";
        _box = [];
        _discovered = [];
        _deck = new();
        _permanent = [];
        _inPlay = [];
        _hand = [];
        _blocked = [];
        _discarded = [];
        _trash = [];

        IsInitialized = false;

        Notify();
    }

    public void Insert(Card card1, Card? card2)
    {
        if (card1 == card2)
            return;

        if (card1.State != card2?.State)
            return;

        var list = List(card1.State);
        var index1 = list.IndexOf(card1);
        if (list.Remove(card2))
            list.Insert(index1, card2);

        Notify();
    }

    internal void Notify() => Notify(this);
}
