using Blazored.LocalStorage;

namespace KingdomLegacy.Web.Configuration;

internal class SavedGameData(ILocalStorageService storage)
{
    public string? Data { get; private set; }

    public async Task<string?> Update(string? savedData = null)
    {
        if (savedData != null)
        {
            Data = savedData;
            return Data;
        }

        savedData = await storage.GetItemAsStringAsync("save");
        Data = savedData?.Trim('"').Replace("\\n", "\n").Replace("\\t", "\t");

        return Data;
    }
}
