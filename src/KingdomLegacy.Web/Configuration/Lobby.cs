using KingdomLegacy.Domain;
using System.Diagnostics.CodeAnalysis;

namespace KingdomLegacy.Web.Configuration;

public class Lobby : Observable<Lobby>
{
    public Game Game { get; private set; }

    private IStorage _storage;
    private LobbyStage _stage;
    public LobbyStage Stage
    {
        get => _stage;
        private set
        {
            _stage = value;
            Notify(this);
        }
    }

    public Lobby(IStorage storage)
    {
        _storage = storage;
        CreateNewGame();
    }

    [MemberNotNull(nameof(Game))]
    private Game CreateNewGame()
    {
        Game = new Game(() => Notify(this));
        return Game;
    }

    public async Task Initialize()
    {
        CreateNewGame();
        Stage = LobbyStage.AcceptCookies;
        if (!await TryCookiesAccepted())
            return;

        if (!await TryConfirmGameOwner())
            return;

        await TryContinueGame();
    }

    public async Task CookiesAccepted()
    {
        await _storage.Save(LocalStorage.CookiesAcceptedKey, true);
        ConfirmGameOwner();
    }

    private void ConfirmGameOwner()
    {
        if (!IsTransitionAllowed(LobbyStage.ConfirmGameOwner))
            return;

        Stage = LobbyStage.ConfirmGameOwner;
    }

    public async Task GameOwnerConfirmed()
    {
        await _storage.Save(LocalStorage.GameOwnerKey, true);
        ChooseGame();
    }

    public void ChooseGame()
    {
        if (!IsTransitionAllowed(LobbyStage.ChooseGame))
            return;

        Stage = LobbyStage.ChooseGame;
    }

    public void LoadGame()
    {
        if (!IsTransitionAllowed(LobbyStage.LoadGame))
            return;

        Stage = LobbyStage.LoadGame;
    }

    public void NewGame()
    {
        if (!IsTransitionAllowed(LobbyStage.NewGame))
            return;

        Stage = LobbyStage.NewGame;
    }

    public void ContinueGame()
    {
        if (Game.IsInitialized && !IsTransitionAllowed(LobbyStage.Play))
            return;

        Stage = LobbyStage.Play;
    }

    public void PlayLoaded(string data)
    {
        if (!IsTransitionAllowed(LobbyStage.Play))
            return;

        CreateNewGame();
        if (Game.Load(data))
            Stage = LobbyStage.Play;
    }

    public void PlayNew(string name)
    {
        if (!IsTransitionAllowed(LobbyStage.Play))
            return;

        CreateNewGame();
        if (Game.Initialize(name))
            Stage = LobbyStage.Play;
    }

    private bool IsTransitionAllowed(LobbyStage nextStage) =>
        _allowedTransitions[Stage].Contains(nextStage);

    private async Task<bool> TryCookiesAccepted()
    {
        if (await _storage.Load<bool>(LocalStorage.CookiesAcceptedKey))
        {
            ConfirmGameOwner();
            return true;
        }

        return false;
    }

    private async Task<bool> TryConfirmGameOwner()
    {
        if (await _storage.Load<bool>(LocalStorage.GameOwnerKey))
        {
            ChooseGame();
            return true;
        }

        return false;
    }

    private async Task<bool> TryContinueGame()
    {
        var data = await _storage.LoadGame();
        if (!string.IsNullOrEmpty(data) && Game.Load(data))
        {
            ContinueGame();
            return true;
        }

        return false;
    }

    private Dictionary<LobbyStage, LobbyStage[]> _allowedTransitions = new()
    {
        [LobbyStage.AcceptCookies] = [LobbyStage.ConfirmGameOwner],
        [LobbyStage.ConfirmGameOwner] = [LobbyStage.ChooseGame],
        [LobbyStage.ChooseGame] = [LobbyStage.LoadGame, LobbyStage.NewGame, LobbyStage.Play],
        [LobbyStage.LoadGame] = [LobbyStage.ChooseGame, LobbyStage.Play],
        [LobbyStage.NewGame] = [LobbyStage.ChooseGame, LobbyStage.Play],
        [LobbyStage.Play] = [LobbyStage.ChooseGame]
    };
}

public enum LobbyStage
{
    AcceptCookies,
    ConfirmGameOwner,
    ChooseGame,
    LoadGame,
    NewGame,
    Play
}
