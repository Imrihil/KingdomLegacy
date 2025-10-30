﻿using System.Text.Json.Serialization;

namespace KingdomLegacy.Domain;
public record Card : IComparable<Card>
{
    public const int Width = 373;
    public const int Height = 520;

    public int Id { get; set; }
    public string Expansion { get; set; } = string.Empty;
    public Orientation Orientation { get; set; }
    public State State { get; set; }
    public List<Sticker> Stickers { get; set; } = [];

    [JsonIgnore]
    public string Path => $"{Expansion}/{Expansion}_{Id}_{(Orientation > 0 ? "A" : "B")}.jpg";

    public void Reset() =>
        Orientation = Orientation.L1;

    public void Flip() =>
        Orientation = Orientation.Flip();

    public void Rotate() =>
        Orientation = Orientation.Rotate();

    public int CompareTo(Card? other) => Id.CompareTo(other?.Id);

    public Sticker AddSticker(StickerType type, int offsetX, int offsetY)
    {
        var sticker = new Sticker(type);
        sticker.Set(this, offsetX, offsetY);
        Stickers.Add(sticker);

        return sticker;
    }

    public void RemoveSticker(Sticker sticker) =>
        Stickers.Remove(sticker);
}
