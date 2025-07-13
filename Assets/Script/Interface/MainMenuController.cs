using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    #region Singleton

    public static MainMenuController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lobbyLogic = FindAnyObjectByType<LobbyLogic>();
        RegisterUIEvents();
    }

    #endregion

    #region Serialized Fields

    [Header("Player Section")]
    [SerializeField] private TMP_InputField nameInputField;
    private const string PlayerNameKey = "PlayerName";

    [Header("Multiplayer Section")]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private TMP_InputField roomIdInputField;

    [Header("Lobby Section")]
    [SerializeField] private Button startGameInLobbyButton;
    [SerializeField] private TMP_Text startGameInLobbyButtonText;

    [Header("Layout Setting")]
    [SerializeField] private GameObject multiplayerLayout;
    [SerializeField] private GameObject lobbyLayout;

    #endregion

    #region Private Fields

    private LobbyLogic lobbyLogic;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        ShowSection(MenuState.Multiplayer);
    }

    private void OnDestroy()
    {
        UnregisterUIEvents();
    }

    #endregion

    #region UI Events

    private void RegisterUIEvents()
    {
        nameInputField.onValueChanged.AddListener(OnNameChanged);
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
        startGameInLobbyButton.onClick.AddListener(StartGameOrReadyInLobby);
    }

    private void UnregisterUIEvents()
    {
        nameInputField.onValueChanged.RemoveAllListeners();
        createRoomButton.onClick.RemoveAllListeners();
        joinRoomButton.onClick.RemoveAllListeners();
        startGameInLobbyButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region UI Navigation

    public void ShowSection(MenuState state)
    {
        multiplayerLayout.SetActive(state == MenuState.Multiplayer);
        lobbyLayout.SetActive(state == MenuState.Lobby);

        if (state == MenuState.Lobby)
        {
            bool isHost = LobbyManager.Instance != null &&
                          LobbyManager.Instance.Runner != null &&
                          LobbyManager.Instance.Runner.IsServer;

            UpdateLobbyStartButtonText(isHost);
        }
    }

    #endregion

    #region Player Name

    private void OnNameChanged(string newName)
    {
        PlayerPrefs.SetString(PlayerNameKey, newName);
        PlayerPrefs.Save();
        Debug.Log("Player name saved: " + newName);
    }

    #endregion

    #region Multiplayer Logic

    private async void CreateRoom()
    {
        string roomId = $"{Random.Range(100000, 999999)}";
        Debug.Log($"Creating room with ID: {roomId}"); // 🔹 Log the room number

        var result = await lobbyLogic.CreateRoom(roomId);

        if (result.Ok)
        {
            ShowSection(MenuState.Lobby);
            Debug.Log($"Room {roomId} created successfully."); // 🔹 Optional success log
        }
        else
        {
            Debug.LogError($"Failed to create room {roomId}: {result.ErrorMessage}");
            // Optionally show error in UI
            // multiplayerMainStatusText.text = $"Failed to create room: {result.ErrorMessage}";
        }
    }

    private async void JoinRoom()
    {
        string roomId = roomIdInputField.text.Trim();
        Debug.Log($"Attempting to join room with ID: {roomId}"); // 🔹 Log the input

        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogWarning("JoinRoom failed: Room ID input is empty."); // 🔹 Warn if empty
            return;
        }

        var result = await lobbyLogic.JoinRoom(roomId);

        if (result.Ok)
        {
            Debug.Log($"Successfully joined room: {roomId}"); // 🔹 Confirm success
            ShowSection(MenuState.Lobby);
        }
        else
        {
            Debug.LogError($"Failed to join room {roomId}: {result.ErrorMessage}"); // 🔹 Show error
            // Optionally show error in UI
            // multiplayerMainStatusText.text = $"Failed to join room: {result.ErrorMessage}";
        }
    }

    #endregion

    #region Lobby Logic

    private void StartGameOrReadyInLobby()
    {
        LobbyManager.Instance?.OnStartGameOrReadyClicked();
    }

    public void UpdateLobbyStartButtonText(bool isHost)
    {
        startGameInLobbyButtonText.text = isHost ? "START" : "READY";
        // Optional: add state toggle for ready/not ready
        // startGameInLobbyButtonText.text = isReady ? "READY" : "NOT READY";
    }

    #endregion

    public async void HandleLeftRoom(bool isKicked = false)
    {
        await lobbyLogic.LeaveRoom();
        ShowSection(MenuState.Multiplayer);
        // Optional: Show kick notification
    }
}

public enum MenuState { Multiplayer, Lobby }