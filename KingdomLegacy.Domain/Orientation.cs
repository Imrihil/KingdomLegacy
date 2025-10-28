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
    public static Orientation RotateDown(this Orientation orientation)
    {
        return orientation switch
        {
            Orientation.L1 => Orientation.L2,
            Orientation.L2 => Orientation.L1,
            Orientation.L3 => Orientation.L4,
            Orientation.L4 => Orientation.L3,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), "Invalid orientation value")
        };
    }

    public static Orientation RotateRight(this Orientation orientation)
    {
        return orientation switch
        {
            Orientation.L1 => Orientation.L4,
            Orientation.L2 => Orientation.L3,
            Orientation.L3 => Orientation.L2,
            Orientation.L4 => Orientation.L1,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), "Invalid orientation value")
        };
    }
}