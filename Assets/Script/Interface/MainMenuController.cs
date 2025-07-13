using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    #region variables

    [Header("Player Section")]
    [SerializeField] private TMP_InputField NameInputField;
    private const string PlayerNameKey = "PlayerName";

    [Header("Multiplayer Section")]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private TMP_InputField roomIdInputField;

    [Header("Layout Setting")]
    //[SerializeField] private GameObject Multiplayerlayout;
    [SerializeField] private GameObject Lobbylayout;

    private LobbyLogic lobbyLogic;
    public static MainMenuController Instance { get; private set; }


    #endregion


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lobbyLogic = FindAnyObjectByType<LobbyLogic>();
        //Player Name
        NameInputField.onValueChanged.AddListener(OnNameChanged);
        //Multiplayer buttons
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowSection(MenuState state)
    {
        //Multiplayerlayout.SetActive(state == MenuState.Multiplayer);
        Lobbylayout.SetActive(state == MenuState.Lobby);
    }

    private void onDestroy()
    {
        createRoomButton.onClick.RemoveAllListeners();
        joinRoomButton.onClick.RemoveAllListeners();
        NameInputField.onValueChanged.RemoveAllListeners();      
    }

    private async void CreateRoom()
    {    
        //Random a room ID
        string roomId = $"{Random.Range(100000, 999999)}";
        var result = await lobbyLogic.CreateRoom(roomId);
        if (result.Ok)
        {
            ShowSection(MenuState.Lobby);
        }
        else
        {
            //multiplayerMainStatusText.text = $"Failed to create room: {result.ErrorMessage}";
        }
    }

    private async void JoinRoom()
    {
        string roomId = roomIdInputField.text.Trim();
        if (string.IsNullOrEmpty(roomId))
        {
            //error message display here
            return;
        }

        var result = await lobbyLogic.JoinRoom(roomId);
        if (result.Ok)
        {
            ShowSection(MenuState.Lobby);
        }
        else
        { 
            //multiplayerMainStatusText.text = $"Failed to join room: {result.ErrorMessage}";
        }
    }

    private void OnNameChanged(string newName)
    {
        PlayerPrefs.SetString(PlayerNameKey, newName);
        PlayerPrefs.Save();//Optional, ensures it's written immediately
        Debug.Log("Player name saved:" + newName);
    }



    

    public enum MenuState
    {
        Multiplayer,
        Lobby
    }
}
