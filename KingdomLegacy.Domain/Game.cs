using System.Text;

namespace KingdomLegacy.Domain;
public class Game : Observable<Game>
{
    public int BoxCount => _box.Count;
    public int DeckCount => _deck.Count;
    public int HandCount => _hand.Count;
    public int InPlayCount => _inPlay.Count;
    public int DiscardedCount => _discarded.Count;
    public int TrashCount => _trash.Count;
    public bool IsInitialized { get; private set; }

    public Card? DeckTop => _deck.Count > 0 ? _deck.Peek() : null;
    public Card? DiscardedLast => _discarded.Count > 0 ? _discarded.Peek() : null;
    public Card? TrashedLast => _trash.Count > 0 ? _trash.Peek() : null;

    private Queue<Card> _box = new();
    private Queue<Card> _deck = new();
    private List<Card> _hand = [];
    public IReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    private List<Card> _inPlay = [];
    public IReadOnlyCollection<Card> InPlay => _inPlay.AsReadOnly();
    private Stack<Card> _discarded = [];
    private Stack<Card> _trash = [];
    private IEnumerable<Card> All => _box
        .Concat(_deck)
        .Concat(_hand)
        .Concat(_inPlay)
        .Concat(_discarded)
        .Concat(_trash);

    public void Load(string data) =>
        Load(data.Split(Environment.NewLine));

    public void Load(IEnumerable<string> lines)
    {
        string? expansion = null;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                expansion = null;
            else if (expansion == null)
                expansion = line.Trim();
            else
            {
                var parts = line.Split('\t');
                if (parts.Length != 3)
                    throw new FormatException("Invalid card data format.");
                var id = int.Parse(parts[0]);
                var orientation = (Orientation)int.Parse(parts[1]);
                var state = (State)int.Parse(parts[2]);
                var card = new Card
                {
                    Id = id,
                    Expansion = expansion,
                    Orientation = orientation,
                    State = state
                };
                switch (state)
                {
                    case State.Box:
                        _box.Enqueue(card);
                        break;
                    case State.Deck:
                        _deck.Enqueue(card);
                        break;
                    case State.Hand:
                        _hand.Add(card);
                        break;
                    case State.InPlay:
                        _inPlay.Add(card);
                        break;
                    case State.Discarded:
                        _discarded.Push(card);
                        break;
                    case State.Removed:
                        _trash.Push(card);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        IsInitialized = true;
        Notify(this);
    }

    public string Save() =>
        string.Join($"{Environment.NewLine}{Environment.NewLine}",
            All.GroupBy(card => card.Expansion)
            .Select(SaveExpansion));


    private string SaveExpansion(IGrouping<string, Card> expansion)
    {
        var sb = new StringBuilder(expansion.Key).AppendLine();
        foreach (var card in expansion.OrderBy(card => card.Id))
            sb.AppendLine($"{card.Id}\t{(int)card.Orientation}\t{(int)card.State}");

        return sb.ToString();
    }

    public void Initialize(Expansion expansion)
    {
        _box = new Queue<Card>(expansion.Cards);
        _deck = new();
        _hand = [];
        _inPlay = [];
        _discarded = [];
        _trash = [];

        TakeFromBox();

        Reshuffle();

        IsInitialized = true;
        Notify(this);
    }

    public void TakeFromBox(int i = 1)
    {
        while (i-- > 0 && _box.Count > 0)
        {
            var card = _box.Dequeue();
            card.State = State.Discarded;
            _discarded.Push(card);
        }
    }

    public void Reshuffle()
    {
        _deck = new Queue<Card>(_deck
            .Concat(_hand)
            .Concat(_inPlay)
            .Concat(_discarded)
            .OrderBy(_ => Random.Shared.Next()));

        foreach (var card in _deck)
            card.State = State.Deck;

        _hand.Clear();
        _inPlay.Clear();
        _discarded.Clear();
    }

    public void Discard(Card card)
    {
        if (_hand.Remove(card) || _inPlay.Remove(card))
        {
            card.State = State.Discarded;
            _discarded.Push(card);
        }
    }

    public void UndoDiscard()
    {
        if (_discarded.Count > 0)
        {
            var card = _discarded.Pop();
            card.State = State.Hand;
            _hand.Add(card);
        }
    }

    public void UndoTrash()
    {
        if (_trash.Count > 0)
        {
            var card = _trash.Pop();
            card.State = State.Hand;
            _hand.Add(card);
        }
    }

    public void Trash(Card card)
    {
        if (_hand.Remove(card) || _inPlay.Remove(card))
        {
            card.State = State.Removed;
            _trash.Push(card);
        }
    }

    public void Draw(int i = 1)
    {
        while (i-- > 0 && _deck.Count > 0)
        {
            var card = _deck.Dequeue();
            card.State = State.Hand;
            _hand.Add(card);
        }
    }

    public void Play(Card card)
    {
        if (_hand.Remove(card))
        {
            card.State = State.InPlay;
            _inPlay.Add(card);
        }
    }
}
