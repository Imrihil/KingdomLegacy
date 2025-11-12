namespace KingdomLegacy.Domain;

public static class Expansions
{
    public static ExpansionType[] All = {
        ExpansionType.FeudalKingdom,
        ExpansionType.RiddingTheWoods,
        ExpansionType.Merchants,
        ExpansionType.FeudalKingdomFoundations,
        ExpansionType.FK_Adventures,
        ExpansionType.DistantLands,
        ExpansionType.Ambitions
    };

    public static int CardsCount(this ExpansionType type) => type switch
    {
        ExpansionType.None => 0,
        ExpansionType.FeudalKingdom => 140,
        ExpansionType.RiddingTheWoods => 30,
        ExpansionType.Merchants => 26,
        ExpansionType.FeudalKingdomFoundations => 157,
        ExpansionType.FK_Adventures => 29,
        ExpansionType.DistantLands => 160,
        ExpansionType.Ambitions => 30,
        _ => throw new NotImplementedException(),
    };

    public static string Name(this ExpansionType type) => type switch
    {
        ExpansionType.None => "N/A",
        ExpansionType.FeudalKingdom => "Feudal Kingdom",
        ExpansionType.RiddingTheWoods => "Ridding the Woods",
        ExpansionType.Merchants => "Merchants",
        ExpansionType.FeudalKingdomFoundations => "Feudal Kingdom Foundations",
        ExpansionType.FK_Adventures => "Feudal Kingdom Adventures",
        ExpansionType.DistantLands => "Distant Lands",
        ExpansionType.Ambitions => "Ambitions",
        _ => throw new NotImplementedException(),
    };

    public static List<Card> Load(this ExpansionType type) =>
        Enumerable.Range(0, type.CardsCount())
        .Select(i => new Card { Id = i, Expansion = type, Orientation = Orientation.L1, State = State.Box })
        .ToList();
}

public enum ExpansionType
{
    None,
    FeudalKingdom,
    RiddingTheWoods,
    Merchants,
    FeudalKingdomFoundations,
    FK_Adventures,
    DistantLands,
    Ambitions
}