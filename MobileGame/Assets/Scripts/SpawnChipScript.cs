using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChipScript : MonoBehaviour
{
    public GameObject chip;
    public GameObject chip2;
    public bool playerOneTurn = true;
    public int placement = 0;
    public GameObject gameBoard;
    public GameController gc;
    public MenuHandler mh;
    int heightOfBoard = 6;
    int lengthOfBoard = 7;
    int chipCounter = 0;
    public GameObject[] spawnLoc;
    bool switchChip;
    int[,] boardState;
    MatchInfo matchInfo;
    MatchInfo newMatchInfo;
    public GameObject feedbackController;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();


        chipCounter = 0;

        boardState = new int[lengthOfBoard, heightOfBoard];

        matchInfo = GameManager.matchInfo;

        Subscribe("games/" + matchInfo.matchid);

        StartCoroutine(LoadChips());

        if (matchInfo.player1 == Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId)
        {
            switchChip = false;
        }

        else
        {
            switchChip = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Physics.gravity = new Vector3(0, -75.0F, 0);

        if (newMatchInfo != null)
        {
            if (newMatchInfo.chipPlacements.Count > matchInfo.chipPlacements.Count)
            {
                int latestChip = newMatchInfo.chipPlacements.Count - 1;
                SpawnChipOnChange(newMatchInfo.chipPlacements[latestChip]);
                matchInfo = newMatchInfo;
            }
        }
    }


    public void SpawnChipAndSave(int column)
    {
        if (matchInfo.playerTurn == Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId)
        {
            feedbackController.GetComponent<PlayerFeedBackScript>().randomTextFeedback();

            matchInfo.chipPlacements ??= new List<int>();

            matchInfo.chipPlacements.Add(column);

            if (matchInfo.player1 == matchInfo.playerTurn)
            {
                chipCounter++;
                matchInfo.playerTurn = matchInfo.player2;
                UpdateBoardState(column, 1);

            }
            else
            {
                chipCounter++;
                matchInfo.playerTurn = matchInfo.player1;
                UpdateBoardState(column, 2);
            }

            

            Instantiate(chip, spawnLoc[column].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));

            if(DidIWin(1))
            {
                if (matchInfo.player1 == Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId)
                {
                    PlayerData.data.wins = PlayerData.data.wins + 1;
                    PlayerData.data.chipsPlaced = PlayerData.data.chipsPlaced + chipCounter;
                    PlayerData.SaveData();
                    matchInfo.playerTurn = "PlayerOneWon";
                    StartCoroutine(feedbackController.GetComponent<PlayerFeedBackScript>().PlayerWin(0));
                }
                
            }

            if (DidIWin(2))
            {
                if (matchInfo.player2 == Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId)
                {
                    PlayerData.data.wins = PlayerData.data.wins + 1;
                    PlayerData.data.chipsPlaced = PlayerData.data.chipsPlaced + chipCounter;
                    PlayerData.SaveData();
                    matchInfo.playerTurn = "PlayerTwoWon";
                    StartCoroutine(feedbackController.GetComponent<PlayerFeedBackScript>().PlayerWin(1));
                }
                
            }

            string jsonString2 = JsonUtility.ToJson(matchInfo);

            SaveManager.Instance.SaveData("games/" + matchInfo.matchid, jsonString2);
        }
        
    }



    public IEnumerator LoadChips()
    {
        if (matchInfo.chipPlacements != null)
        {
            foreach (int chip3 in matchInfo.chipPlacements)
            {
                yield return new WaitForSeconds(0.40f);
                switch (switchChip)
                {
                    case true:
                        chipCounter++;
                        Instantiate(chip, spawnLoc[chip3].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));
                        UpdateBoardState(chip3, 1);
                        switchChip = false;
                        break;
                    default:
                        Instantiate(chip2, spawnLoc[chip3].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));
                        UpdateBoardState(chip3, 2);
                        switchChip = true;
                        break;
                }
            }
        }

    }

    public void SelectColumn(int column)
    {
        SpawnChipAndSave(column);
    }

    public void SpawnChipOnChange(int column)
    {
        Instantiate(chip2, spawnLoc[column].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));

        if(matchInfo.player1 == Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId)
        {
            UpdateBoardState(column, 2);
        }

        else
        {
            UpdateBoardState(column, 1);
        }

        if (DidIWin(1))
        {
            StartCoroutine(feedbackController.GetComponent<PlayerFeedBackScript>().PlayerWin(0));
        }

        if (DidIWin(2))
        {
            StartCoroutine(feedbackController.GetComponent<PlayerFeedBackScript>().PlayerWin(1));
        }
    }

        bool UpdateBoardState(int column, int playerNumber)
        {
            for (int row = 0; row < heightOfBoard; row++)
            {
                if (boardState[column, row] == 0)
                {
                    if (playerNumber == 1)
                    {
                        boardState[column, row] = 1;
                    }
                    else
                    {
                        boardState[column, row] = 2;
                }
                    return true;
                }
            }
            return false;
        }

        bool DidIWin(int playerNum)
        {
            //Horizontal
            for (int x = 0; x < lengthOfBoard - 3; x++)
            {
                for (int y = 0; y < heightOfBoard; y++)
                {
                    if (boardState[x, y] == playerNum && boardState[x + 1, y] == playerNum && boardState[x + 2, y] == playerNum && boardState[x + 3, y] == playerNum)
                    {
                        return true;
                    }
                }
            }

            //Vertical
            for (int x = 0; x < lengthOfBoard - 3; x++)
            {
                for (int y = 0; y < heightOfBoard - 3; y++)
                {
                    if (boardState[x, y] == playerNum && boardState[x + 1, y + 1] == playerNum && boardState[x + 2, y + 2] == playerNum && boardState[x + 3, y + 3] == playerNum)
                    {
                        return true;
                    }
                }
            }

            //Diogonally
            for (int x = 0; x < lengthOfBoard; x++)
            {
                for (int y = 0; y < heightOfBoard - 3; y++)
                {
                    if (boardState[x, y] == playerNum && boardState[x, y + 1] == playerNum && boardState[x, y + 2] == playerNum && boardState[x, y + 3] == playerNum)
                    {
                        return true;
                    }
                }
            }

            //Diogonally
            for (int x = 0; x < lengthOfBoard - 3; x++)
            {
                for (int y = 0; y < heightOfBoard - 3; y++)
                {
                    if (boardState[x, y + 3] == playerNum && boardState[x + 1, y + 2] == playerNum && boardState[x + 2, y + 1] == playerNum && boardState[x + 3, y] == playerNum)
                    {
                        return true;
                    }
                }
            }
            return false;

        }

    public void Subscribe(string id)
    {
        FirebaseDatabase.DefaultInstance.GetReference(id).ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        //update our game info
        MatchInfo updatedGame = JsonUtility.FromJson<MatchInfo>(args.Snapshot.GetRawJsonValue());

        newMatchInfo = updatedGame;

        if (matchInfo.playerTurn == "PlayerOneWon")
        {
            if(matchInfo.player2 == Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId)
            {
                StartCoroutine(feedbackController.GetComponent<PlayerFeedBackScript>().PlayerWin(0));
                PlayerData.data.losses = PlayerData.data.losses + 1;
                PlayerData.data.chipsPlaced = PlayerData.data.chipsPlaced + chipCounter;
                PlayerData.SaveData();
            }
        }

        if (matchInfo.playerTurn == "PlayerTwoWon")
        {
            if (matchInfo.player1 == Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId)
            {
                StartCoroutine(feedbackController.GetComponent<PlayerFeedBackScript>().PlayerWin(1));
                PlayerData.data.losses = PlayerData.data.losses + 1;
                PlayerData.data.chipsPlaced = PlayerData.data.chipsPlaced + chipCounter;
                PlayerData.SaveData();
            }
        }
        //run the game with the new information
    }
}




