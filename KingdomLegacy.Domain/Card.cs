using System.Text.Json.Serialization;

namespace KingdomLegacy.Domain;
public record Card : IComparable<Card>
{
    public int Id { get; set; }
    public string Expansion { get; set; } = string.Empty;
    public Orientation Orientation { get; set; }
    public State State { get; set; }

    [JsonIgnore]
    public string Path => $"{Expansion}/{Expansion}_{Id}_{(Orientation > 0 ? "A" : "B")}.jpg";

    public void RotationReset() =>
        Orientation = Orientation.L1;

    public void RotateRight() =>
        Orientation = Orientation.RotateRight();

    public void RotateDown() =>
        Orientation = Orientation.RotateDown();

    public int CompareTo(Card? other) => Id.CompareTo(other?.Id);
}
