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