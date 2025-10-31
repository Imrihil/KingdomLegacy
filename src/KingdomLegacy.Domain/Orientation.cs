namespace KingdomLegacy.Domain;
public enum Orientation
{
    L1 = 1,
    L2 = 2,
    L3 = -2,
    L4 = -1
}

public static class OrientationExtensions
{
    public static Orientation Rotate(this Orientation orientation) => orientation switch
    {
        Orientation.L1 => Orientation.L2,
        Orientation.L2 => Orientation.L1,
        Orientation.L3 => Orientation.L4,
        Orientation.L4 => Orientation.L3,
        _ => throw new ArgumentOutOfRangeException(nameof(orientation), "Invalid orientation value")
    };

    public static Orientation Flip(this Orientation orientation) => orientation switch
    {
        Orientation.L1 => Orientation.L4,
        Orientation.L2 => Orientation.L3,
        Orientation.L3 => Orientation.L2,
        Orientation.L4 => Orientation.L1,
        _ => throw new ArgumentOutOfRangeException(nameof(orientation), "Invalid orientation value")
    };

    public static bool IsRotated(this Orientation orientation) => orientation == Orientation.L2 || orientation == Orientation.L3;

    public static bool IsFlipped(this Orientation orientation) => orientation == Orientation.L3 || orientation == Orientation.L4;
}