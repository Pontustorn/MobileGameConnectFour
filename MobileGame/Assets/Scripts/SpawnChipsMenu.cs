using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChipsMenu : MonoBehaviour
{
    public GameObject chip;
    public GameObject chip2;


    public GameObject[] spawnPoints;

    bool shouldSpawnYellow = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (shouldSpawnYellow)
        {
            case true:

                var spawnedChip = Instantiate(chip, spawnPoints[Random.Range(0, 6)].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));
                Destroy(spawnedChip, 5);
                shouldSpawnYellow = false;
                break;
            default:
                var spawnedChip2 = Instantiate(chip2, spawnPoints[Random.Range(0, 6)].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f));
                Destroy(spawnedChip2, 5);
                shouldSpawnYellow = true;
                break;
        }
        
    }
}
