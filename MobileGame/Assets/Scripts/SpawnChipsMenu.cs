using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChipsMenu : MonoBehaviour
{
    public GameObject chipPrefab;
    public GameObject chip2Prefab;
    public int maxChips = 100;
    public int frames = 0;

    private LinkedList<GameObject> _freeChips = new LinkedList<GameObject>();

    public GameObject[] spawnPoints;

    bool shouldSpawnYellow = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        frames++;
        if (frames % 2 == 0)
        {
            GameObject newChip = CreateChip();
            newChip.GetComponent<Rigidbody>().velocity = Vector3.down * 20;
            StartCoroutine(DestroyHelper(newChip, 6));
        }

    }

    private void OnDisable()
    {
        foreach (var chip in _freeChips)
            Destroy(chip);
        _freeChips.Clear();
    }

    public GameObject CreateChip()
    {
        if (_freeChips.Count == 0)
            return Instantiate(chipPrefab, spawnPoints[Random.Range(0, 6)].transform.position, transform.rotation * Quaternion.Euler(0f, 90, 90f), transform);

        var freeBulletNode = _freeChips.First;
        var bullet = freeBulletNode.Value;
        _freeChips.Remove(freeBulletNode);
        bullet.transform.position = spawnPoints[Random.Range(0, 6)].transform.position;
        bullet.transform.parent = transform;
        bullet.SetActive(true);
        return bullet;
    }

    public void DestroyChip(GameObject chip)
    {
        // Only keep 100 bullets so that we don't eat up to much memory
        if (_freeChips.Count == maxChips)
        {
            Destroy(chip);
            return;
        }

        // Remove it from the scene and deactivate it
        chip.transform.parent = null;
        chip.SetActive(false);
        _freeChips.AddFirst(chip);
    }

    IEnumerator DestroyHelper(GameObject chip, float time)
    {
        yield return new WaitForSeconds(time);
        DestroyChip(chip);
    }
}
