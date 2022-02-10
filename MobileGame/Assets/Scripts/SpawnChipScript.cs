using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChipScript : MonoBehaviour
{
    public GameObject chip;
    public GameObject chip2;
    public bool playerOneTurn = true;
    private GameController gc;
    private bool someoneWon;
    public int placement = 0;
    public GameObject gameBoard;
    bool gameStarted = true;

    bool playerColor;

    public FirebaseManager fb;
    public MenuHandler mh;

    int heightOfBoard = 6;
    int lengthOfBoard = 7;

    public GameObject[] spawnLoc;

    bool switchChip;

    int[,] boardState;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        fb = GameObject.Find("FirebaseManager").GetComponent<FirebaseManager>();
        mh = GameObject.Find("MenuHandler").GetComponent<MenuHandler>();
        boardState = new int[lengthOfBoard, heightOfBoard];
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameBoard.activeSelf && gameStarted)
        {
            mh.ActivateLoadingGameUI();
            //fb.setupListener();
            StartCoroutine(fb.WhichPlayerAmI((myReturnValue) =>
            {
                playerColor = myReturnValue;
                switchChip = playerColor;
                Debug.Log(switchChip);
            }));
            getChipsGame();
            gameStarted = false;
            
        }

        if(!gameStarted)
        {
            mh.DeactivateLoadingGameUI();
        }

        Physics.gravity = new Vector3(0, -75.0F, 0);
    }


    public void SelectColumn(int column)
    {
        SpawnChip(column);
    }

    public void SpawnChipOnChange(int column)
    {
        Instantiate(chip, spawnLoc[column].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));
    }

    public void SpawnChip(int column)
    {
            StartCoroutine(fb.CheckWhosTurnItIs((myReturnValue) =>
            {
                if (myReturnValue)
                {
                    if (UpdateBoardState(column))
                    {
                        
                        Instantiate(chip, spawnLoc[column].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));

                        playerOneTurn = false;

                        StartCoroutine(fb.ReturnLatestChip((myReturnValue) =>
                        {
                            StartCoroutine(fb.playerPlayedChip(column.ToString(), myReturnValue.ToString()));
                        }));

                        if (DidIWin(1))
                        {
                            if (!someoneWon)
                            {
                                gc.playerWon();
                                Debug.Log("Player One Won");
                                someoneWon = true;
                            }
                        }
                    }
                }
            }));

        }

    public void getChipsGame()
    {
        List<int> returnList = new List<int>();

        StartCoroutine(fb.LoadChips((myReturnValue) =>
        {
            foreach (var chip3 in myReturnValue)
            {

                returnList.Add(chip3);
                
                
            }
            StartCoroutine(loadChipsGame(returnList));
        }));
    }

    public IEnumerator loadChipsGame(List<int> returnList)
    {
        foreach (int chip3 in returnList)
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


        bool UpdateBoardState(int column)
        {
            for (int row = 0; row < heightOfBoard; row++)
            {
                if (boardState[column, row] == 0)
                {
                    if (playerOneTurn)
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
    }


