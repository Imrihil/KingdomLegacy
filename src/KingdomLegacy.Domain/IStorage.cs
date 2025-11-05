namespace KingdomLegacy.Domain;
public interface IStorage
{
    string? GameData { get; }
    Task LoadGame(Game game);
    Task<string> SaveGame(Game value);
    Task<T?> Load<T>(string key);
    Task Save<T>(string key, T value);
}

public class NullStorage : IStorage
{
    public string? GameData => null;

    public Task<T?> Load<T>(string key) => Task.FromResult(default(T));
    public Task LoadGame(Game game) => Task.CompletedTask;
    public Task Save<T>(string key, T value) => Task.CompletedTask;
    public Task<string> SaveGame(Game value) => Task.FromResult(string.Empty);
}