using BlazorBootstrap;
using KingdomLegacy.Domain;

namespace KingdomLegacy.Web.Configuration;

public static class ColorExtensions
{
    public static ButtonColor GetButtonColor(this State state) => state switch
    {
        State.Box => ButtonColor.Dark,
        State.Discovered => ButtonColor.Info,
        State.Deck or State.DeckTop => ButtonColor.Primary,
        State.InPlay => ButtonColor.Secondary,
        State.Hand => ButtonColor.Success,
        State.Blocked => ButtonColor.Dark,
        State.Discarded => ButtonColor.Warning,
        State.Removed => ButtonColor.Danger,
        State.Permanent => ButtonColor.Light,
        State.Purged => ButtonColor.Danger,
        _ => ButtonColor.None,
    };
}
