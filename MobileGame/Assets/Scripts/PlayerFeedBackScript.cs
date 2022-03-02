using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerFeedBackScript : MonoBehaviour
{
    public TMP_Text[] infoText;
    public TMP_Text[] winText;

    public GameObject confetti;

    public GameObject[] spawnPos;

    public AudioSource childrenYay;
    public AudioSource chipSound;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void randomTextFeedback()
    {
        int randomNmbr = Random.Range(0, infoText.Length);

        StartCoroutine(Feedback(randomNmbr));
    }

    public IEnumerator Feedback(int textnmbr)
    {
        chipSound.Play();
        infoText[textnmbr].enabled = true;
        infoText[textnmbr].GetComponent<Animation>().Play();
        yield return new WaitForSeconds(1);

        childrenYay.Play();

        var confettiGO = Instantiate(confetti, spawnPos[0].transform.position, Quaternion.identity);
        var confettiGO2 = Instantiate(confetti, spawnPos[1].transform.position, Quaternion.identity);
        var confettiGO3 = Instantiate(confetti, spawnPos[2].transform.position, Quaternion.identity);
        var confettiGO4 = Instantiate(confetti, spawnPos[3].transform.position, Quaternion.identity);
        var confettiGO5 = Instantiate(confetti, spawnPos[4].transform.position, Quaternion.identity);
        var confettiGO6 = Instantiate(confetti, spawnPos[5].transform.position, Quaternion.identity);
        var confettiGO7 = Instantiate(confetti, spawnPos[6].transform.position, Quaternion.identity);
        var confettiGO8 = Instantiate(confetti, spawnPos[7].transform.position, Quaternion.identity);
        var confettiGO9 = Instantiate(confetti, spawnPos[8].transform.position, Quaternion.identity);
        var confettiGO10 = Instantiate(confetti, spawnPos[9].transform.position, Quaternion.identity);

        Destroy(confettiGO, 1);
        Destroy(confettiGO2, 1);
        Destroy(confettiGO3, 1);
        Destroy(confettiGO4, 1);
        Destroy(confettiGO5, 1);
        Destroy(confettiGO6, 1);
        Destroy(confettiGO7, 1);
        Destroy(confettiGO8, 1);
        Destroy(confettiGO9, 1);
        Destroy(confettiGO10, 1);
    }

    public IEnumerator PlayerWin(int nmbr)
    {
        childrenYay.Play();

        winText[nmbr].enabled = true;
        winText[nmbr].GetComponent<Animation>().Play();

        yield return new WaitForSeconds(1);

        var confettiGO = Instantiate(confetti, spawnPos[0].transform.position, Quaternion.identity);
        var confettiGO2 = Instantiate(confetti, spawnPos[1].transform.position, Quaternion.identity);
        var confettiGO3 = Instantiate(confetti, spawnPos[2].transform.position, Quaternion.identity);
        var confettiGO4 = Instantiate(confetti, spawnPos[3].transform.position, Quaternion.identity);
        var confettiGO5 = Instantiate(confetti, spawnPos[4].transform.position, Quaternion.identity);
        var confettiGO6 = Instantiate(confetti, spawnPos[5].transform.position, Quaternion.identity);
        var confettiGO7 = Instantiate(confetti, spawnPos[6].transform.position, Quaternion.identity);
        var confettiGO8 = Instantiate(confetti, spawnPos[7].transform.position, Quaternion.identity);
        var confettiGO9 = Instantiate(confetti, spawnPos[8].transform.position, Quaternion.identity);
        var confettiGO10 = Instantiate(confetti, spawnPos[9].transform.position, Quaternion.identity);

    }

}
