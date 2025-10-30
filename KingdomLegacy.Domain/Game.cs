using KingdomLegacy.Domain.Logic;
using System.Text;

namespace KingdomLegacy.Domain;
public class Game : Observable<Game>
{
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
    public IReadOnlyCollection<Card> Deck => _deck.Count > 0 ? new Card[] { _deck.Peek() }.Concat(_deck.Skip(1).OrderBy(card => card.Id)).ToList().AsReadOnly() : [];
    internal Queue<Card> _deck = new();

    public IReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    internal List<Card> _hand = [];

    public IReadOnlyCollection<Card> InPlay => _inPlay.AsReadOnly();
    internal List<Card> _inPlay = [];

    public Card? DiscardedLast => _discarded.Count > 0 ? _discarded[^1] : null;
    public IReadOnlyCollection<Card> Discarded => ((IEnumerable<Card>)_discarded).Reverse().ToList().AsReadOnly();
    internal List<Card> _discarded = [];

    public Card? TrashedLast => _trash.Count > 0 ? _trash.Peek() : null;
    public IReadOnlyCollection<Card> Trashed => _trash.ToArray();
    internal Stack<Card> _trash = [];

    internal IEnumerable<Card> All => _box
        .Concat(Deck)
        .Concat(Discovered)
        .Concat(Hand)
        .Concat(InPlay)
        .Concat(Discarded)
        .Concat(Trashed.Reverse());

    public readonly Actions Actions;

    public Game()
    {
        Actions = new(this);
    }

    public void Load(string data) =>
        Load(data.Split(Environment.NewLine));

    public void Load(IEnumerable<string> lines)
    {
        var firstLine = true;
        string? expansion = null;
        foreach (var line in lines)
        {
            if (firstLine)
            {
                if (!string.IsNullOrEmpty(line) && int.TryParse(line, out var points))
                {
                    Points = points;
                    continue;
                }
                firstLine = false;
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

    private static Card GetCard(string expansion, string line)
    {
        var parts = line.Split('\t');
        if (parts.Length != 3)
            throw new FormatException("Invalid card data format.");

        var id = int.Parse(parts[0]);
        var orientation = (Orientation)int.Parse(parts[1]);
        var state = (State)int.Parse(parts[2]);
        return new Card
        {
            Id = id,
            Expansion = expansion,
            Orientation = orientation,
            State = state
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public string Save()
    {
        var sb = new StringBuilder(Points.ToString()).AppendLine();
        sb.Append(string.Join($"{Environment.NewLine}{Environment.NewLine}",
            All.GroupBy(card => card.Expansion)
            .Select(SaveExpansion)));

        return sb.ToString();
    }


    private string SaveExpansion(IGrouping<string, Card> expansion)
    {
        var sb = new StringBuilder(expansion.Key).AppendLine();
        foreach (var card in expansion)
            sb.AppendLine($"{card.Id}\t{(int)card.Orientation}\t{(int)card.State}");

        return sb.ToString();
    }

    public void Initialize(Expansion expansion)
    {
        _box = expansion.Cards.ToList();
        _discovered = [];
        _deck = new();
        _hand = [];
        _inPlay = [];
        _discarded = [];
        _trash = [];

        Actions.Discover(1);

        IsInitialized = true;

        Notify(this);
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
