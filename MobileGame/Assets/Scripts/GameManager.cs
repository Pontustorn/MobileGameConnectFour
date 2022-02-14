using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public static MatchInfo matchInfo;

    public Button joinGameButton;

    public Transform publicGamesListHolder;
    public Button buttonPrefab;
    private MenuHandler menuHandler;

    // Start is called before the first frame update
    void Start()
    {
        matchInfo = new MatchInfo();
        menuHandler = GameObject.Find("MenuHandler").GetComponent<MenuHandler>();

    }
    public void CreateGame()
    {
        matchInfo.players ??= new List<UserInfo>();

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

        SaveManager.Instance.SaveData("games/" + gameid, jsonString);
    }
    public void JoinGame(MatchInfo MatchInfo)
    {
        
        matchInfo.displayName = MatchInfo.displayName;
        matchInfo.matchid = MatchInfo.matchid;

        UserInfo userInfo = PlayerData.data;

        matchInfo.players.Add(userInfo);
        matchInfo.full = true;
        matchInfo.playerTurn = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        matchInfo.player2 = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        PlayerData.data.activeGames ??= new List<string>();

        PlayerData.data.activeGames.Add(matchInfo.matchid);

        string jsonString = JsonUtility.ToJson(matchInfo);

        PlayerData.SaveData();

        SaveManager.Instance.SaveData("games/" + matchInfo.matchid, jsonString);


        menuHandler.LoadScene("GameScene");
    }
    public void ListGames()
    {
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
}
