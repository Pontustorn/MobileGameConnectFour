using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public GameObject loginScreen;
    public GameObject registerScreen;
    public GameObject mainMenuScreen;
    public GameObject optionsScreen;
    public GameObject scoreBoardScreen;
    public GameObject gameScreen;
    public GameObject loadingScreen;
    public GameObject loadingOverlay;
    public GameObject menuGameBoard;
    public GameObject pauseScreen;
    public GameObject gameUI;
    public GameObject pimpMyChip;

    public GameObject testChip;

    bool pause = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ClearScreen()
    {
        loginScreen.SetActive(false);
        loadingScreen.SetActive(false);
        gameScreen.SetActive(false);
        mainMenuScreen.SetActive(false);
        registerScreen.SetActive(false);
        scoreBoardScreen.SetActive(false);
        optionsScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }
    public void Login()
    {
        ClearScreen();
        loginScreen.SetActive(true);
    }

    public void Register()
    {
        ClearScreen();
        registerScreen.SetActive(true);
    }

    public void MainMenu()
    {
        ClearScreen();
        mainMenuScreen.SetActive(true);
    }

    public void Options()
    {
        ClearScreen();
        optionsScreen.SetActive(true);
    }

    public void ScoreBoard()
    {
        ClearScreen();
        scoreBoardScreen.SetActive(true);
    }

    public void ActivateLoadingScreen()
    {
        ClearScreen();
        loadingScreen.SetActive(true);
    }

    public void ActivateGameScreen()
    {
        ClearScreen();
        menuGameBoard.SetActive(false);
        gameScreen.SetActive(true);
        gameUI.SetActive(true);
    }

    public void DeactivateGameScreen()
    {
        ClearScreen();
        menuGameBoard.SetActive(true);
        gameScreen.SetActive(false);
        gameUI.SetActive(false);
        mainMenuScreen.SetActive(true);
    }


    public void ActivateLoadingGameUI()
    {
        loadingOverlay.SetActive(true);
    }

    public void DeactivateLoadingGameUI()
    {
        loadingOverlay.SetActive(false);
    }

    public void ActivatePauseScreen()
    {
        switch (pause)
        {
            case true:
                    pauseScreen.SetActive(true);
                pause = false;
                break;
            default:
                pauseScreen.SetActive(false);
                pause = true;
                break;
        }
        
    }

    public void ActivatePimpMyChipScreen()
    {
        ClearScreen();
        menuGameBoard.SetActive(false);
        pimpMyChip.SetActive(true);
        testChip.SetActive(true);
    }

    public void DeactivatePimpMyChipScreen()
    {
        ClearScreen();
        menuGameBoard.SetActive(true);
        optionsScreen.SetActive(true);
        pimpMyChip.SetActive(false);
        testChip.SetActive(false);
    }

}
