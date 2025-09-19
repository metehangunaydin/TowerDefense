using UnityEngine;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs_lv1;
    public GameObject[] enemyPrefabs_lv2;
    public GameObject[] enemyPrefabs_lv3;
    public GameObject bossPrefab_lv1;
    public GameObject bossPrefab_lv2;
    public Transform[] spawnPoints;
    public int waveLevel = 1;
    int waveNumber = 0;
    public int enemiesPerWave = 5;
    private GameObject[][] enemyPrefabsGroups;
    public float waveCooldown = 10f;
    bool waveInProgress = false;
    public TextMeshProUGUI waveInfoText;
    public int levelUpEveryNWaves = 2;
    public Material redMaterial;
    public Material yellowMaterial;
    public Material greenMaterial;
    private GameObject enemiesParent;
    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyPrefabsGroups = new GameObject[3][];
        enemyPrefabsGroups[0] = enemyPrefabs_lv1;
        enemyPrefabsGroups[1] = enemyPrefabs_lv2;
        enemyPrefabsGroups[2] = enemyPrefabs_lv3;
        UpdateWaveInfoText();
        enemiesParent = new GameObject("Enemies");
        player = FindObjectOfType<Player>();

    }

    // Update is called once per frame
    void Update()
    {
        if (player.currentState == Player.State.Dead) return;
        if (!waveInProgress && player != null)
        {
            StartCoroutine(GenerateWave(waveLevel));
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(GenerateWave(waveLevel));
        }
        UpdateWaveInfoText();
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
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemy.GetComponent<Enemy>().difficulty = (Enemy.EnemyDifficulty)(waveLevel-1);
            enemy.transform.Find("EnemyHealthBar/HealthBar").GetComponent<MeshRenderer>().material = level == 1 ? greenMaterial : level == 2 ? yellowMaterial : redMaterial;
            enemy.transform.parent = enemiesParent.transform;
            yield return new WaitForSeconds(0.15f);
        }
        if(waveNumber % levelUpEveryNWaves == 0 && waveLevel < 3)
        {
            waveLevel++;
            Debug.Log("Wave Level Up! New Wave Level: " + waveLevel);
        }
        //UpdateWaveInfoText();
        yield return new WaitForSeconds(waveCooldown);
        waveInProgress = false;

    }


    void UpdateWaveInfoText()
    {
        waveInfoText.text = $"Next Wave Difficulty: {(Enemy.EnemyDifficulty)(waveLevel-1)} \n Wave Number: {waveNumber}";
    }

    public void Restart()
    {
        StopAllCoroutines();
        waveLevel = 1;
        waveNumber = 0;
        waveInProgress = false;
        UpdateWaveInfoText();
        Destroy(enemiesParent);
        enemiesParent = new GameObject("Enemies");
    }

}
