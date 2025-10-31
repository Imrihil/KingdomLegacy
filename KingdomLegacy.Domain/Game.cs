using KingdomLegacy.Domain.Logic;
using System.Text;

namespace KingdomLegacy.Domain;
public class Game : Observable<Game>
{
    public string KingdomName { get; set; } = "";
    public int BoxCount => _box.Count;
    public int DiscoveredCount => _discovered.Count;
    public int DeckCount => _deck.Count;
    public int HandCount => _hand.Count;
    public int InPlayCount => _inPlay.Count;
    public int DiscardedCount => _discarded.Count;
    public int TrashCount => _trash.Count;
    public bool IsInitialized { get; internal set; }
    public int Points { get; set; }

    internal List<Card> _box = new();

    internal List<Card> _discovered = [];
    public IReadOnlyCollection<Card> Discovered => _discovered.AsReadOnly();

    public Card? DeckTop => _deck.Count > 0 ? _deck.Peek() : null;
    public IReadOnlyCollection<Card> Deck => _deck.Count > 0
        ? new Card[] { _deck.Peek() }
            .Concat(_deck.Skip(1).OrderBy(card => card.Id))
            .ToList()
            .AsReadOnly()
        : [];
    internal Queue<Card> _deck = new();

    public IReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    internal List<Card> _hand = [];

    public IReadOnlyCollection<Card> InPlay => _inPlay.AsReadOnly();
    internal List<Card> _inPlay = [];

    public Card? DiscardedLast => _discarded.Count > 0 ? _discarded[^1] : null;
    public IReadOnlyCollection<Card> Discarded =>
        ((IEnumerable<Card>)_discarded).Reverse().ToList().AsReadOnly();
    internal List<Card> _discarded = [];

    public Card? TrashedLast => _trash.Count > 0 ? _trash.Peek() : null;
    public IReadOnlyCollection<Card> Trashed => _trash.ToArray();
    internal Stack<Card> _trash = [];

    public IReadOnlyCollection<Card> Permanent => _permanent.AsReadOnly();
    internal List<Card> _permanent = [];

    internal IEnumerable<Card> All => _box
        .Concat(Deck)
        .Concat(Discovered)
        .Concat(Hand)
        .Concat(InPlay)
        .Concat(Discarded.Reverse())
        .Concat(Trashed.Reverse())
        .Concat(Permanent);

    public readonly Actions Actions;

    public Game()
    {
        Actions = new(this);
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

        ReshuffleDeck();
        IsInitialized = true;

        Notify(this);
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
                _deck.Enqueue(card);
                break;
            case State.DeckTop:
                _deck.Enqueue(card);
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
                _trash.Push(card);
                break;
            case State.Permanent:
                _permanent.Add(card);
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

        KingdomName = name;
        _box = expansion.Cards.ToList();
        _discovered = [];
        _deck = new();
        _hand = [];
        _inPlay = [];
        _discarded = [];
        _trash = [];
        _permanent = [];

        IsInitialized = true;

        Actions.Discover(1);
    }

    private void ReshuffleDeck()
    {
        var top = _deck.FirstOrDefault(card => card.State == State.DeckTop);
        var newDeck = new Queue<Card>();

        if (top != null)
            newDeck.Enqueue(top);

        foreach (var card in _deck.Where(card => card.State != State.DeckTop).OrderBy(_ => Random.Shared.Next()))
            newDeck.Enqueue(card);

        _deck = newDeck;
    }

    public void Insert(Card card1, Card? card2)
    {
        if (card1 == card2)
            return;

        if (card1.State != card2?.State)
            return;

        var list = card1.State switch
        {
            State.Discovered => _discovered,
            State.Hand => _hand,
            State.InPlay => _inPlay,
            State.Permanent => _permanent,
            _ => null
        };

        if (list == null)
            return;

        var index1 = list.IndexOf(card1);
        if (list.Remove(card2))
            list.Insert(index1, card2);

        Notify(this);
    }

    public void Swap(Card card1, Card? card2)
    {
        if (card1 == card2)
            return;

        if (card1.State != card2?.State)
            return;

        var list = card1.State switch
        {
            State.Discovered => _discovered,
            State.Hand => _hand,
            State.InPlay => _inPlay,
            State.Permanent => _permanent,
            _ => null
        };

        if (list == null)
            return;

        var index1 = list.IndexOf(card1);
        var index2 = list.IndexOf(card2);
        if (index1 >= 0 && index2 >= 0)
        {
            list[index1] = card2;
            list[index2] = card1;
        }

        Notify(this);
    }

    internal void Notify() => Notify(this);
}
