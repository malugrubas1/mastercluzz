using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs (assign both)")]
    public GameObject enemyPrefabA;
    public GameObject enemyPrefabB;

    [Header("Difficulty / Wave Size")]
    public int baseEnemies = 6;
    public float enemiesPerWave = 2.0f;
    public int variance = 3;
    [Range(0f, 1f)] public float prefabBChance = 0.5f;

    [Header("Spawn Settings")]
    public List<Transform> spawnPoints = new List<Transform>();
    public bool randomizeSpawnPoint = true;
    public int waveDuration = 12;

    [Header("Between Waves")]
    public float breakDuration = 5f;   // <-- 5s break between waves
    public Text currentWaveText;

    [Header("References")]
    public LogicManager logicManager;

    [Header("Debug Info")]
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    // internals
    public int currWave = 1;
    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    private float waveTimer;
    private float spawnTimer;
    private float spawnInterval;
    private int nextSpawnIndex;

    private enum SpawnerState { Spawning, WaitingClear, Break }
    private SpawnerState state = SpawnerState.Spawning;
    private float breakTimer;

    void Start()
    {
        if (!logicManager)
        {
            var logic = GameObject.FindGameObjectWithTag("Logic");
            if (logic) logicManager = logic.GetComponent<LogicManager>();
        }

        if (!enemyPrefabA || !enemyPrefabB)
        {
            Debug.LogError("[WaveSpawner] Assign both enemyPrefabA and enemyPrefabB.");
            enabled = false; return;
        }
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("[WaveSpawner] No spawn points set. Drag Transforms into 'spawnPoints'.");
            enabled = false; return;
        }
        if (currWave < 1) currWave = 1;

        GenerateWave();
    }

    void FixedUpdate()
    {
        // UI hint (optional): show “Break” countdown
        if (currentWaveText)
        {
            if (state == SpawnerState.Break)
                currentWaveText.text = $"Wave {currWave} — Break: {Mathf.CeilToInt(breakTimer)}s";
            else
                currentWaveText.text = $"Wave {currWave}";
        }

        // Handle break
        if (state == SpawnerState.Break)
        {
            breakTimer -= Time.fixedDeltaTime;
            if (breakTimer <= 0f)
            {
                currWave++;
                GenerateWave();
            }
            return; // nothing else during break
        }

        // Normal spawning cadence
        if (state == SpawnerState.Spawning)
        {
            if (spawnTimer <= 0f)
            {
                if (enemiesToSpawn.Count > 0)
                {
                    SpawnEnemy();
                    spawnTimer = spawnInterval;
                }
                else
                {
                    state = SpawnerState.WaitingClear; // spawned everything; wait for clears/time
                }
            }
            else
            {
                spawnTimer -= Time.fixedDeltaTime;
            }
        }

        // wave timer counts down regardless (pacing)
        if (waveTimer > 0f) waveTimer -= Time.fixedDeltaTime;

        // When timer finished AND no enemies alive → end wave → start break
        if (waveTimer <= 0f && spawnedEnemies.Count == 0 && state != SpawnerState.Break)
        {
            if (logicManager) logicManager.EndWave();
            state = SpawnerState.Break;
            breakTimer = breakDuration;   // <-- start 5s break
        }
    }

    private void GenerateWave()
    {
        // build random mix for this wave
        int extra = (variance > 0) ? Random.Range(0, variance + 1) : 0;
        int totalThisWave = Mathf.Max(1, Mathf.RoundToInt(baseEnemies + enemiesPerWave * (currWave - 1)) + extra);

        enemiesToSpawn.Clear();
        spawnedEnemies.Clear();

        for (int i = 0; i < totalThisWave; i++)
        {
            bool pickB = Random.value < prefabBChance;
            enemiesToSpawn.Add(pickB ? enemyPrefabB : enemyPrefabA);
        }

        // shuffle
        for (int i = enemiesToSpawn.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (enemiesToSpawn[i], enemiesToSpawn[j]) = (enemiesToSpawn[j], enemiesToSpawn[i]);
        }

        spawnInterval = Mathf.Max(0.05f, (float)waveDuration / Mathf.Max(1, enemiesToSpawn.Count));
        waveTimer = waveDuration;
        spawnTimer = 0f;
        state = SpawnerState.Spawning;

        Debug.Log($"[WaveSpawner] Wave {currWave}: total={totalThisWave}, points={spawnPoints.Count}, interval={spawnInterval:0.00}s");
    }

    private void SpawnEnemy()
    {
        Transform point = randomizeSpawnPoint
            ? spawnPoints[Random.Range(0, spawnPoints.Count)]
            : spawnPoints[(nextSpawnIndex++) % spawnPoints.Count];

        var prefab = enemiesToSpawn[0];
        enemiesToSpawn.RemoveAt(0);

        var enemy = Instantiate(prefab, point.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }

    // called by enemies when they die/despawn
    public void EnterNameHere(GameObject enemyInstance)
    {
        if (!enemyInstance) return;
        spawnedEnemies.Remove(enemyInstance);
    }

    // legacy fallback
    public void EnterNameHere()
    {
        if (spawnedEnemies.Count > 0) spawnedEnemies.RemoveAt(0);
    }
}
