namespace KingdomLegacy.Domain;
public record Resources(Game game)
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
        }
    }

    public int Points
    {
        get => game.Points;
        set
        {
            if (value < 0)
                return;

            game.Points = value;
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
    }
}
