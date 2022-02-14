using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuHandler : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject gameUI;

    bool pause = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
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
}
