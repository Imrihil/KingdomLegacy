namespace KingdomLegacy.Domain;
public record Sticker(StickerType Type)
{
    public int X { get; set; }
    public int Y { get; set; }

    public void Set(Card card, int offsetX, int offsetY)
    {
        var details = Type.GetDetails();
        var stickerWidth = details?.Width ?? 0;
        var stickerHeight = details?.Height ?? 0;
        X = (card.Orientation.IsFlipped() ? -1 : 1) * Math.Min(Math.Max(0, offsetX - stickerWidth / 2), Card.Width - stickerWidth);
        Y = (card.Orientation.IsFlipped() ? -1 : 1) * Math.Min(Math.Max(0, offsetY - stickerHeight / 2), Card.Height - stickerHeight);
    }

    public bool IsRotated => Math.Abs(Y) > Card.Height / 2;
    public bool IsFlipped => X < 0 || Y < 0;
}

public enum StickerType
{
    Cross,
    Coin = 1,
    Wood = 2,
    Stone = 3,
    Steel = 4,
    Sword = 5,
    Good = 6,
    StaysInPlay = 7,
    Points2 = 8,
    Points5 = 10,
    Points = 16,
    Knight = 11,
    //StartsInPlay = 17,
    //PlaceBottomDeck = 18,
}

public record StickerTypeDetails(StickerType Type, int Width, int Height, string Name)
{
    public static readonly StickerTypeDetails Cross = new(StickerType.Cross, 52, 52, "x");
    public static readonly StickerTypeDetails Coin = new(StickerType.Coin, 52, 54, "coin");
    public static readonly StickerTypeDetails Wood = new(StickerType.Wood, 45, 53, "wood");
    public static readonly StickerTypeDetails Stone = new(StickerType.Stone, 48, 47, "stone");
    public static readonly StickerTypeDetails Steel = new(StickerType.Steel, 51, 42, "steel");
    public static readonly StickerTypeDetails Sword = new(StickerType.Sword, 50, 53, "sword");
    public static readonly StickerTypeDetails Good = new(StickerType.Good, 51, 44, "good");
    public static readonly StickerTypeDetails StaysInPlay = new(StickerType.StaysInPlay, 147, 59, "staysinplay");
    public static readonly StickerTypeDetails Points2 = new(StickerType.Points2, 44, 64, "points2");
    public static readonly StickerTypeDetails Points5 = new(StickerType.Points5, 44, 64, "points5");
    public static readonly StickerTypeDetails Points = new(StickerType.Points, 44, 64, "points");
    public static readonly StickerTypeDetails Knight = new(StickerType.Knight, 100, 22, "knight");
    //public static readonly StickerTypeDetails StartsInPlay = new(StickerType.StartsInPlay, 52, 52, "startsinplay");
    //public static readonly StickerTypeDetails PlaceBottomDeck = new(StickerType.PlaceBottomDeck, 52, 52, "placebottomdeck");
}

public static class Stickers
{
    public static readonly StickerType[] AllTypes = [
        StickerType.Cross,
        StickerType.Coin,
        StickerType.Wood,
        StickerType.Stone,
        StickerType.Steel,
        StickerType.Sword,
        StickerType.Good,
        StickerType.StaysInPlay,
        StickerType.Points2,
        StickerType.Points5,
        StickerType.Points,
        StickerType.Knight,
        //StickerType.StartsInPlay,
        //StickerType.PlaceBottomDeck
        ];

    public static readonly StickerTypeDetails[] AllDetails = AllTypes
        .Select(type => type.GetDetails())
        .OfType<StickerTypeDetails>()
        .ToArray();

    public static StickerTypeDetails? GetDetails(this StickerType type) => type switch
    {
        StickerType.Cross => StickerTypeDetails.Cross,
        StickerType.Coin => StickerTypeDetails.Coin,
        StickerType.Wood => StickerTypeDetails.Wood,
        StickerType.Stone => StickerTypeDetails.Stone,
        StickerType.Steel => StickerTypeDetails.Steel,
        StickerType.Sword => StickerTypeDetails.Sword,
        StickerType.Good => StickerTypeDetails.Good,
        StickerType.StaysInPlay => StickerTypeDetails.StaysInPlay,
        StickerType.Points2 => StickerTypeDetails.Points2,
        StickerType.Points5 => StickerTypeDetails.Points5,
        StickerType.Points => StickerTypeDetails.Points,
        StickerType.Knight => StickerTypeDetails.Knight,
        //StickerType.StartsInPlay => StickerTypeDetails.StartsInPlay,
        //StickerType.PlaceBottomDeck => StickerTypeDetails.PlaceBottomDeck,
        _ => null,
    };
}