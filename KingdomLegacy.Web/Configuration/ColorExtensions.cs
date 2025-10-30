using BlazorBootstrap;
using KingdomLegacy.Domain;

namespace KingdomLegacy.Web.Configuration;

public static class ColorExtensions
{
    public static ButtonColor GetButtonColor(this State state)
    {
        return state switch
        {
            State.Box => ButtonColor.Dark,
            State.Discovered => ButtonColor.Info,
            State.Deck or State.DeckTop => ButtonColor.Primary,
            State.Hand => ButtonColor.Success,
            State.InPlay => ButtonColor.Secondary,
            State.Discarded => ButtonColor.Warning,
            State.Removed => ButtonColor.Danger,
            State.Permanent => ButtonColor.Light,
            _ => ButtonColor.Secondary,
        };
    }
}
