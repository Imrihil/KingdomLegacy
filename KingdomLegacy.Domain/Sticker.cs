namespace KingdomLegacy.Domain;

public record Sticker(StickerType Type, int X, int Y)
{
    public bool IsRotated => Math.Abs(Y) > Card.Height / 2;
}