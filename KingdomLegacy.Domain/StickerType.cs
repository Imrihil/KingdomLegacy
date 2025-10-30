namespace KingdomLegacy.Domain;
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
    StartsInPlay = 17,
    PlaceBottomDeck = 18,
}

public record StickerTypeDetails(StickerType StickerType, int Width, int Height, string Name)
{
    public static readonly StickerTypeDetails Cross = new(StickerType.Cross, 52, 52, "x");
    public static readonly StickerTypeDetails Coin = new(StickerType.Coin, 52, 52, "coin");
    public static readonly StickerTypeDetails Wood = new(StickerType.Wood, 52, 52, "wood");
    public static readonly StickerTypeDetails Stone = new(StickerType.Stone, 52, 52, "stone");
    public static readonly StickerTypeDetails Steel = new(StickerType.Steel, 52, 52, "steel");
    public static readonly StickerTypeDetails Sword = new(StickerType.Sword, 52, 52, "sword");
    public static readonly StickerTypeDetails Good = new(StickerType.Good, 52, 52, "good");
    public static readonly StickerTypeDetails StaysInPlay = new(StickerType.StaysInPlay, 52, 52, "staysinplay");
    public static readonly StickerTypeDetails Points2 = new(StickerType.Points2, 52, 52, "points2");
    public static readonly StickerTypeDetails Points5 = new(StickerType.Points5, 52, 52, "points5");
    public static readonly StickerTypeDetails Points = new(StickerType.Points, 52, 52, "points");
    public static readonly StickerTypeDetails Knight = new(StickerType.Knight, 52, 52, "knight");
    //public static readonly StickerTypeDetails StartsInPlay = new(StickerType.StartsInPlay, 52, 52, "startsinplay");
    //public static readonly StickerTypeDetails PlaceBottomDeck = new(StickerType.PlaceBottomDeck, 52, 52, "placebottomdeck");
}

public static class StickerTypeExtensions
{
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