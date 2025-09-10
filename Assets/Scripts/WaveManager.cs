using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs_lv1;
    public GameObject[] enemyPrefabs_lv2;
    public GameObject bossPrefab_lv1;
    public GameObject bossPrefab_lv2;
    public Transform[] spawnPoints;
    int waveLevel = 1;
    int waveNumber = 0;
    int enemiesPerWave = 5;
    private GameObject[][] enemyPrefabsGroups;
    public float waveCooldown = 10f;
    bool waveInProgress = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyPrefabsGroups = new GameObject[2][];
        enemyPrefabsGroups[0] = enemyPrefabs_lv1;
        enemyPrefabsGroups[1] = enemyPrefabs_lv2;

    }

    // Update is called once per frame
    void Update()
    {
        if (!waveInProgress)
        {
            StartCoroutine(GenerateWave(waveLevel));
        }
    }

    IEnumerator GenerateWave(int level)
    {
        waveNumber++;
        waveInProgress = true;
        Debug.Log($"Starting Wave {waveNumber} at Level {level}");
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject enemyPrefab = enemyPrefabsGroups[level-1][Random.Range(0, enemyPrefabsGroups[level-1].Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        yield return new WaitForSeconds(waveCooldown);
        waveInProgress = false;
    }




}
