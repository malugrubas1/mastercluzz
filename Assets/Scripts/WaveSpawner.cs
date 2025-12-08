using System.Collections;
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

    [Header("Wave Timing")]
    public int waveDuration = 12;

    [Header("Break UI (optional)")]
    public GameObject breakPanel;
    public Text breakLabel;
    public Button startWaveButton;

    [Header("Wave Label (optional)")]
    public Text currentWaveText;

    [Header("Wave End UI (optional)")]
    public GameObject waveEndedRedImage;
    public GameObject waveEndedBlackImage;
    public float waveEndedBlinkDuration = 3f;
    public float waveEndedBlinkInterval = 0.15f;

    [Header("References")]
    public LogicManager logicManager;

    [Header("Debug Info (read-only at runtime)")]
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    public int currWave = 0;
    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    private float waveTimer;
    private float spawnTimer;
    private float spawnInterval;
    private int nextSpawnIndex;

    private enum SpawnerState { Break, Spawning, WaitingClear }
    private SpawnerState state = SpawnerState.Break;

    [Header("Input (optional)")]
    public bool allowKeyboardStart = true;
    public KeyCode startKey = KeyCode.E;

    public bool IsInBreak => state == SpawnerState.Break;

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
            enabled = false;
            return;
        }
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("[WaveSpawner] No spawn points set. Drag Transforms into 'spawnPoints'.");
            enabled = false;
            return;
        }

        if (waveEndedRedImage != null) waveEndedRedImage.SetActive(false);
        if (waveEndedBlackImage != null) waveEndedBlackImage.SetActive(false);

        EnterBreak();
    }

    void Update()
    {
        if (state == SpawnerState.Break && allowKeyboardStart && Input.GetKeyDown(startKey))
            StartNextWave();
    }

    void FixedUpdate()
    {
        if (currentWaveText != null)
            currentWaveText.text = currWave.ToString();

        if (state == SpawnerState.Break) return;

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
                    state = SpawnerState.WaitingClear;
                }
            }
            else
            {
                spawnTimer -= Time.fixedDeltaTime;
            }
        }

        if (waveTimer > 0f) waveTimer -= Time.fixedDeltaTime;

        if (waveTimer <= 0f && spawnedEnemies.Count == 0 && state != SpawnerState.Break)
        {
            if (waveEndedRedImage != null || waveEndedBlackImage != null)
            {
                StopAllCoroutines();
                StartCoroutine(WaveEndedBlinkRoutine());
            }

            if (logicManager) logicManager.EndWave();
            EnterBreak();
        }
    }

    public void StartNextWave()
    {
        if (!IsInBreak) return;
        currWave++;
        GenerateWave();
        ExitBreak();
    }

    private void EnterBreak()
    {
        state = SpawnerState.Break;
        enemiesToSpawn.Clear();
        spawnedEnemies.Clear();
        spawnTimer = 0f;
        waveTimer = 0f;

        if (breakPanel) breakPanel.SetActive(true);
        if (breakLabel) breakLabel.text = $"Wave {Mathf.Max(1, currWave + 1)} — press Start";
        if (startWaveButton) startWaveButton.interactable = true;
    }

    private void ExitBreak()
    {
        state = SpawnerState.Spawning;
        if (breakPanel) breakPanel.SetActive(false);
    }

    private void GenerateWave()
    {
        int extra = (variance > 0) ? Random.Range(0, variance + 1) : 0;
        int totalThisWave = Mathf.Max(1, Mathf.RoundToInt(baseEnemies + enemiesPerWave * (currWave - 1)) + extra);

        enemiesToSpawn.Clear();
        spawnedEnemies.Clear();

        for (int i = 0; i < totalThisWave; i++)
        {
            bool pickB = Random.value < prefabBChance;
            enemiesToSpawn.Add(pickB ? enemyPrefabB : enemyPrefabA);
        }

        for (int i = enemiesToSpawn.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (enemiesToSpawn[i], enemiesToSpawn[j]) = (enemiesToSpawn[j], enemiesToSpawn[i]);
        }

        spawnInterval = Mathf.Max(0.05f, (float)waveDuration / Mathf.Max(1, enemiesToSpawn.Count));
        waveTimer = waveDuration;
        spawnTimer = 0f;

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

    public void EnterNameHere(GameObject enemyInstance)
    {
        if (!enemyInstance) return;
        spawnedEnemies.Remove(enemyInstance);
    }

    public void EnterNameHere()
    {
        if (spawnedEnemies.Count > 0) spawnedEnemies.RemoveAt(0);
    }

    IEnumerator WaveEndedBlinkRoutine()
    {
        float t = 0f;
        bool redOn = false;

        if (waveEndedRedImage != null) waveEndedRedImage.SetActive(false);
        if (waveEndedBlackImage != null) waveEndedBlackImage.SetActive(false);

        while (t < waveEndedBlinkDuration)
        {
            redOn = !redOn;

            if (waveEndedRedImage != null)
                waveEndedRedImage.SetActive(redOn);

            if (waveEndedBlackImage != null)
                waveEndedBlackImage.SetActive(!redOn);

            yield return new WaitForSeconds(waveEndedBlinkInterval);
            t += waveEndedBlinkInterval;
        }

        if (waveEndedRedImage != null) waveEndedRedImage.SetActive(false);
        if (waveEndedBlackImage != null) waveEndedBlackImage.SetActive(false);
    }
}
