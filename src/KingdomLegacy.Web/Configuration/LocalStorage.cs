using Blazored.LocalStorage;
using KingdomLegacy.Domain;
using System.Text.RegularExpressions;

namespace KingdomLegacy.Web.Configuration;
public class LocalStorage(ILocalStorageService service) : IStorage
{
    private const string GameKey = "save";
    public const string CookiesAcceptedKey = "cookies-consent";
    public const string GameOwnerKey = "game-owner";

    public string? GameData { get; private set; }

    public async Task<T?> Load<T>(string key) =>
        await service.GetItemAsync<T>(key);

    public async Task<string?> LoadGame()
    {
        var data = await service.GetItemAsStringAsync(GameKey);
        return data == null 
            ? null 
            : (GameData = Regex.Unescape(data.Trim('"')));
    }

    public async Task Save<T>(string key, T value) =>
        await service.SetItemAsync(key, value);

    public async Task<string> SaveGame(Game value)
    {
        var data = GameData = value.Save();
        await service.SetItemAsync(GameKey, data);
        return data;
    }
}
