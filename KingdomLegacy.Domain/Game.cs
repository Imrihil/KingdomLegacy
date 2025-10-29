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
    public bool IsInitialized { get; private set; }
    public int Points { get; set; }

    private Queue<Card> _box = new();

    private List<Card> _discovered = [];
    public IReadOnlyCollection<Card> Discovered => _discovered.AsReadOnly();

    public Card? DeckTop => _deck.Count > 0 ? _deck.Peek() : null;
    public IReadOnlyCollection<Card> Deck => _deck.ToArray();
    private Queue<Card> _deck = new();

    public IReadOnlyCollection<Card> Hand => _hand.AsReadOnly();
    private List<Card> _hand = [];

    public IReadOnlyCollection<Card> InPlay => _inPlay.AsReadOnly();
    private List<Card> _inPlay = [];

    public Card? DiscardedLast => _discarded.Count > 0 ? _discarded[^1] : null;
    public IReadOnlyCollection<Card> Discarded => _discarded.AsReadOnly();
    private List<Card> _discarded = [];

    public Card? TrashedLast => _trash.Count > 0 ? _trash.Peek() : null;
    public IReadOnlyCollection<Card> Trashed => _trash.ToArray();
    private Stack<Card> _trash = [];

    private IEnumerable<Card> All => _box
        .Concat(_deck)
        .Concat(_discovered)
        .Concat(_hand)
        .Concat(_inPlay)
        .Concat(_discarded)
        .Concat(_trash.Reverse());

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
        }

        ReshuffleDeck();
        IsInitialized = true;
        Notify(this);
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
        _box = new Queue<Card>(expansion.Cards);
        _discovered = [];
        _deck = new();
        _hand = [];
        _inPlay = [];
        _discarded = [];
        _trash = [];

        TakeFromBox();

        IsInitialized = true;

        Notify(this);
    }

    public void TakeFromBox(int i = 1)
    {
        while (i-- > 0 && _box.Count > 0)
        {
            var card = _box.Dequeue();
            card.State = State.Discovered;
            _discovered.Add(card);
        }

        Notify(this);
    }

    public bool BoxContainsId(int id) =>
        _box.Any(card => card.Id == id);

    public void TakeFromBoxById(int id)
    {
        var tempQueue = new Queue<Card>();
        Card? foundCard = null;
        while (_box.Count > 0)
        {
            var card = _box.Dequeue();
            if (card.Id == id && foundCard == null)
            {
                foundCard = card;
                card.State = State.Discovered;
                _discovered.Add(card);
            }
            else
            {
                tempQueue.Enqueue(card);
            }
        }
        _box = tempQueue;

        Notify(this);
    }

    public void Reshuffle()
    {
        _deck = new Queue<Card>(_deck
            .Concat(_discovered)
            .Concat(_hand)
            .Concat(_inPlay)
            .Concat(_discarded)
            .OrderBy(_ => Random.Shared.Next()));

        foreach (var card in _deck)
            card.State = State.Deck;

        if (_deck.TryPeek(out var nextCard))
            nextCard.State = State.DeckTop;

        _discovered.Clear();
        _hand.Clear();
        _inPlay.Clear();
        _discarded.Clear();

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

    public void AddToHand(Card card)
    {
        if (_discovered.Remove(card) || _hand.Remove(card) || _inPlay.Remove(card))
        {
            card.State = State.Hand;
            _hand.Add(card);
        }

        Notify(this);
    }

    public void Discard(Card card)
    {
        if (_discovered.Remove(card) || _hand.Remove(card) || _inPlay.Remove(card))
        {
            card.State = State.Discarded;
            _discarded.Add(card);
        }

        Notify(this);
    }

    public void UndoDiscard(Card? card = null)
    {
        if (card != null)
        {
            if (_discarded.Remove(card))
            {
                card.State = State.Hand;
                _hand.Add(card);
            }
        }
        else if (_discarded.Count > 0)
        {
            UndoDiscard(_discarded[^1]);
        }

        Notify(this);
    }

    public void Trash(Card card)
    {
        if (_discovered.Remove(card) || _hand.Remove(card) || _inPlay.Remove(card))
        {
            card.State = State.Removed;
            _trash.Push(card);
        }

        Notify(this);
    }

    public void UndoTrash()
    {
        if (_trash.Count > 0)
        {
            var card = _trash.Pop();
            card.State = State.Hand;
            _hand.Add(card);
        }

        Notify(this);
    }

    public void Draw(int i = 1)
    {
        while (i-- > 0 && _deck.Count > 0)
        {
            var card = _deck.Dequeue();
            card.State = State.Hand;
            _hand.Add(card);
            if (_deck.TryPeek(out var nextCard))
                nextCard.State = State.DeckTop;
        }

        Notify(this);
    }

    public void Play(Card card)
    {
        if (_hand.Remove(card))
        {
            card.State = State.InPlay;
            _inPlay.Add(card);
        }

        Notify(this);
    }

    public void EndDiscover()
    {
        if (_deck.Count + _inPlay.Count + _hand.Count == 0)
        {
            Reshuffle();
            Draw(4);

            Notify(this);
        }
        else
        {
            foreach (var card in _discovered.ToArray())
                Discard(card);

            Notify(this);
        }
    }

    public void EndTurn()
    {
        foreach (var card in _hand.ToArray())
            Discard(card);

        Draw(4);

        Notify(this);
    }

    public void EndRound()
    {
        foreach (var card in _hand.Concat(_inPlay).ToArray())
            Discard(card);

        TakeFromBox(2);

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
}
