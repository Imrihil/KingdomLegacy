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
        State.StayInPlay => ButtonColor.Secondary,
        State.Played => ButtonColor.Success,
        State.Blocked => ButtonColor.Dark,
        State.Discarded => ButtonColor.Warning,
        State.Destroyed => ButtonColor.Danger,
        State.Permanent => ButtonColor.Light,
        State.Purged => ButtonColor.Danger,
        _ => ButtonColor.None,
    };
}
