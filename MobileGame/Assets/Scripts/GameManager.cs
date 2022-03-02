using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using TMPro;
using Firebase.Database;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public static MatchInfo matchInfo;

    public Button joinGameButton;

    public Transform publicGamesListHolder;
    public Button buttonPrefab;
    private MenuHandler menuHandler;

    public TMP_Text infoText;
    public TMP_Text listText;

    public GameObject foundGameWindow;
    public Button foundGameButton;

    // Start is called before the first frame update
    void Start()
    {
        matchInfo = new MatchInfo();
        matchInfo.players = null;
        menuHandler = GameObject.Find("MenuHandler").GetComponent<MenuHandler>();

    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (publicGamesListHolder != null)
        {
            if (publicGamesListHolder.childCount == 0)
            {
                infoText.text = "If you created a game, were still looking for an opponent";
            }

            else
            {
                infoText.text = "";
            }
        }
    }
    public void CreateGame()
    {
        
        if (PlayerData.data.activeGames.Count < 5)
        {
            SaveManager.Instance.LoadData("users/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId, UpdateActiveGames);
            matchInfo.players ??= new List<UserInfo>();
            matchInfo.chipPlacements ??= new List<int>();
            string gameid = SaveManager.Instance.GetKey("games/");
            matchInfo.matchid = gameid;
            matchInfo.displayName = PlayerData.data.username;
            matchInfo.players.Add(PlayerData.data);
            matchInfo.full = false;
            matchInfo.playerTurn = "";
            matchInfo.player1 = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            PlayerData.data.activeGames ??= new List<string>();

            PlayerData.data.activeGames.Add(gameid);

            string jsonString = JsonUtility.ToJson(matchInfo);

            PlayerData.SaveData();

            //Subscribe("games/" + gameid);

            SaveManager.Instance.SaveData("games/" + gameid, jsonString);

            

            foundGameWindow.SetActive(false);
        }

        else
        {
            Debug.Log("Please Finish Your Games Noob");
        }
    }
    public void JoinGame(MatchInfo MatchInfo)
    {
        if (PlayerData.data.activeGames.Count < 5)
        {
            PlayerData.data.activeGames ??= new List<string>();

            PlayerData.data.activeGames.Add(MatchInfo.matchid);

            MatchInfo.players.Add(PlayerData.data);
            MatchInfo.full = true;
            MatchInfo.playerTurn = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            MatchInfo.player2 = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            string jsonString = JsonUtility.ToJson(MatchInfo);

            PlayerData.SaveData();

            SaveManager.Instance.SaveData("games/" + MatchInfo.matchid, jsonString);

            matchInfo = MatchInfo;
            menuHandler.LoadScene("GameScene");
        }

        else
        {
            Debug.Log("Please Finish Your Games Noob");
        }
    }
    public void ListGames()
    {
        listText.text = "Currently listing new games";

        Debug.Log("Listing Games");

        foreach (Transform child in publicGamesListHolder)
            GameObject.Destroy(child.gameObject);

        SaveManager.Instance.LoadDataMultiple("games/", ShowGames);
    }
    public void ShowGames(string json)
    {
        var matchInfo = JsonUtility.FromJson<MatchInfo>(json);

        if (PlayerData.data.activeGames.Contains(matchInfo.matchid) || matchInfo.full == true)
        {
            //Don't list our own games or full games.
            return;
        }

        var newButton = Instantiate(buttonPrefab, publicGamesListHolder).GetComponent<Button>();
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = matchInfo.displayName + "'s game";
        newButton.onClick.AddListener(() => JoinGame(matchInfo));
    }
    public void ListMyGames()
    {
        listText.text = "Currently listing ongoing games";

        Debug.Log("Listing Games");

        foreach (Transform child in publicGamesListHolder)
            GameObject.Destroy(child.gameObject);

        SaveManager.Instance.LoadDataMultiple("games/", ShowMyGames);
    }
    public void ShowMyGames(string json)
    {
        var matchInfo = JsonUtility.FromJson<MatchInfo>(json);

        if (PlayerData.data.activeGames.Contains(matchInfo.matchid) && matchInfo.full == true)
        {
            var newButton = Instantiate(buttonPrefab, publicGamesListHolder).GetComponent<Button>();
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = matchInfo.displayName + "'s game";
            newButton.onClick.AddListener(() => JoinMyGame(matchInfo));
        }
    }
    public void JoinMyGame(MatchInfo newMatchInfo)
    {
        matchInfo = newMatchInfo;

        menuHandler.LoadScene("GameScene");
    }

    //public void Subscribe(string id)
    //{
    //    FirebaseDatabase.DefaultInstance.GetReference(id).ValueChanged += HandleValueChanged;
    //}

    //void HandleValueChanged(object sender, ValueChangedEventArgs args)
    //{
    //    if (args.DatabaseError != null)
    //    {
    //        Debug.LogError(args.DatabaseError.Message);
    //        return;
    //    }
    //    //update our game info
    //    MatchInfo updatedGame = JsonUtility.FromJson<MatchInfo>(args.Snapshot.GetRawJsonValue());

    //    if (updatedGame.player2 != "")
    //    {
    //        ToggleJoinWindow(updatedGame);
    //    }
    //    //run the game with the new information
    //}

    //public void ToggleJoinWindow(MatchInfo matchInfo)
    //{
    //    foundGameWindow.SetActive(true);
    //    foundGameButton.onClick.AddListener(() => JoinMyGame(matchInfo));
    //}

    void UpdateActiveGames(string json)
    {
        PlayerData.data = JsonUtility.FromJson<UserInfo>(json);
    }
}
