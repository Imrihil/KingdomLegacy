namespace KingdomLegacy.Domain;
public class Resources(Game game) : Observable<Resources>
{
    private int _coins;
    public int Coins
    {
        get => _coins;
        set
        {
            if (value < 0)
                return;
            _coins = value;
            Notify(this);
        }
    }

    private int _wood;
    public int Wood
    {
        get => _wood;
        set
        {
            if (value < 0)
                return;
            _wood = value;
            Notify(this);
        }
    }

    private int _stones;
    public int Stones
    {
        get => _stones;
        set
        {
            if (value < 0)
                return;
            _stones = value;
            Notify(this);
        }
    }

    private int _steel;
    public int Steel
    {
        get => _steel;
        set
        {
            if (value < 0)
                return;
            _steel = value;
            Notify(this);
        }
    }

    private int _goods;
    public int Goods
    {
        get => _goods;
        set
        {
            if (value < 0)
                return;
            _goods = value;
            Notify(this);
        }
    }

    private int _swords;
    public int Swords
    {
        get => _swords;
        set
        {
            if (value < 0)
                return;
            _swords = value;
            Notify(this);
        }
    }

    private int _points = game.Points;
    public int Points
    {
        get => _points;
        set
        {
            if (value < 0)
                return;
            _points = value;
            game.Points = value;
            Notify(this);
        }
    }

    public void Reset()
    {
        _coins = 0;
        _wood = 0;
        _stones = 0;
        _steel = 0;
        _goods = 0;
        _swords = 0;
        Notify(this);
    }
}
