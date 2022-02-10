using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{
    
    public GameObject mainMenuScreen;
    public GameObject optionsScreen;
    public GameObject scoreBoardScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowOptions()
    {
        optionsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void HideOptions()
    {
        optionsScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }

    public void ShowScoreBoard()
    {
        scoreBoardScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }
    public void HideScoreBoard()
    {
        scoreBoardScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }
}
