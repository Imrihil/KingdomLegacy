using KingdomLegacy.Domain;

namespace KingdomLegacy.Web.Configuration;

public class AppConfig : Observable<AppConfig>
{
    private bool _discardRotated = false;
    public bool DiscardRotated
    {
        get => _discardRotated;
        set
        {
            _discardRotated = value;
            Notify(this);
        }
    }
}
