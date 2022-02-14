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
    public MenuHandler mh;
    int heightOfBoard = 6;
    int lengthOfBoard = 7;
    public GameObject[] spawnLoc;
    bool switchChip;
    int[,] boardState;
    MatchInfo matchInfo;
    // Start is called before the first frame update
    void Start()
    {
        

        boardState = new int[lengthOfBoard, heightOfBoard];

        matchInfo = GameManager.matchInfo;

        Subscribe("games/" + matchInfo.matchid);

        StartCoroutine(LoadChips());

        if(matchInfo.player1 == Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId)
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
    }


    public void SpawnChipAndSave(int column)
    {
        if (matchInfo.playerTurn == Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId)
        {
            if (matchInfo.player1 == matchInfo.playerTurn)
            {
                matchInfo.chipPlacements ??= new List<int>();

                matchInfo.chipPlacements.Add(column);
                matchInfo.playerTurn = matchInfo.player2;

                UpdateBoardState(column);

                string jsonString2 = JsonUtility.ToJson(matchInfo);

                SaveManager.Instance.SaveData("games/" + matchInfo.matchid, jsonString2);

                Instantiate(chip, spawnLoc[column].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));
            }

            else
            {
                matchInfo.chipPlacements ??= new List<int>();

                matchInfo.chipPlacements.Add(column);
                matchInfo.playerTurn = matchInfo.player1;

                string jsonString2 = JsonUtility.ToJson(matchInfo);

                UpdateBoardState(column);

                SaveManager.Instance.SaveData("games/" + matchInfo.matchid, jsonString2);

                Instantiate(chip, spawnLoc[column].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));
            }
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

                        UpdateBoardState(chip3);
                        Instantiate(chip, spawnLoc[chip3].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));
                        switchChip = false;
                        break;
                    default:
                        UpdateBoardState(chip3);
                        Instantiate(chip2, spawnLoc[chip3].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));
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
    }




        bool UpdateBoardState(int column)
        {
            for (int row = 0; row < heightOfBoard; row++)
            {
                if (boardState[column, row] == 0)
                {
                    if (playerOneTurn)
                    {
                        boardState[column, row] = 1;
                        playerOneTurn = false;
                    }
                    else
                    {
                        boardState[column, row] = 2;
                        playerOneTurn = true;
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
            for (int x = 0; x < lengthOfBoard; x++)
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
            for (int x = 0; x < lengthOfBoard - 3; x++)
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

        // Do something with the data in args.Snapshot
        Debug.Log("Value has changed: " + args.Snapshot.GetRawJsonValue());

        //update our game info
        MatchInfo updatedGame = JsonUtility.FromJson<MatchInfo>(args.Snapshot.GetRawJsonValue());


        //run the game with the new information
        Debug.Log(updatedGame.chipPlacements);

        int listLength = updatedGame.chipPlacements.Count - 1;

        SpawnChipOnChange(updatedGame.chipPlacements[listLength]);
    }
}




