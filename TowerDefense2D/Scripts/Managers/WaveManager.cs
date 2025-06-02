using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public string waveName;
    public List<EnemySpawnInfo> enemies;
    public float timeBetweenSpawns = 1f;
    public float timeBeforeNextWave = 5f;
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnDelay = 0f;
}

public class WaveManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    public List<Wave> waves;
    public Transform spawnPoint;
    public bool autoStartWaves = true;
    public float timeBetweenWaves = 3f;
    
    [Header("Current State")]
    public int currentWaveIndex = 0;
    public bool waveInProgress = false;
    public bool allWavesComplete = false;
    
    private int enemiesAlive = 0;
    private Coroutine currentWaveCoroutine;
    
    public delegate void WaveEvent(int waveNumber);
    public static event WaveEvent OnWaveStart;
    public static event WaveEvent OnWaveComplete;
    public static event WaveEvent OnAllWavesComplete;
    
    private void Start()
    {
        if (autoStartWaves)
        {
            StartNextWave();
        }
    }
    
    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += OnEnemyDestroyed;
        Enemy.OnEnemyReachedEnd += OnEnemyReachedEnd;
    }
    
    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= OnEnemyDestroyed;
        Enemy.OnEnemyReachedEnd -= OnEnemyReachedEnd;
    }
    
    public void StartNextWave()
    {
        if (waveInProgress || allWavesComplete) return;
        
        if (currentWaveIndex < waves.Count)
        {
            currentWaveCoroutine = StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
        else
        {
            allWavesComplete = true;
            OnAllWavesComplete?.Invoke(currentWaveIndex);
        }
    }
    
    private IEnumerator SpawnWave(Wave wave)
    {
        waveInProgress = true;
        OnWaveStart?.Invoke(currentWaveIndex + 1);
        
        foreach (EnemySpawnInfo enemyInfo in wave.enemies)
        {
            yield return new WaitForSeconds(enemyInfo.spawnDelay);
            
            for (int i = 0; i < enemyInfo.count; i++)
            {
                SpawnEnemy(enemyInfo.enemyPrefab);
                yield return new WaitForSeconds(wave.timeBetweenSpawns);
            }
        }
        
        // Wait for all enemies to be defeated or reach the end
        yield return new WaitUntil(() => enemiesAlive <= 0);
        
        CompleteWave();
    }
    
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemiesAlive++;
        }
    }
    
    private void CompleteWave()
    {
        waveInProgress = false;
        OnWaveComplete?.Invoke(currentWaveIndex + 1);
        GameManager.Instance.CompleteWave();
        
        currentWaveIndex++;
        
        if (autoStartWaves && currentWaveIndex < waves.Count)
        {
            StartCoroutine(WaitForNextWave());
        }
    }
    
    private IEnumerator WaitForNextWave()
    {
        if (currentWaveIndex > 0 && currentWaveIndex <= waves.Count)
        {
            yield return new WaitForSeconds(waves[currentWaveIndex - 1].timeBeforeNextWave);
        }
        else
        {
            yield return new WaitForSeconds(timeBetweenWaves);
        }
        
        StartNextWave();
    }
    
    private void OnEnemyDestroyed()
    {
        enemiesAlive--;
    }
    
    private void OnEnemyReachedEnd()
    {
        enemiesAlive--;
    }
    
    public void ForceStartNextWave()
    {
        if (currentWaveCoroutine != null)
        {
            StopCoroutine(currentWaveCoroutine);
        }
        
        // Kill all remaining enemies in current wave
        Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in remainingEnemies)
        {
            enemy.Die(false); // Die without giving rewards
        }
        
        enemiesAlive = 0;
        StartNextWave();
    }
    
    public float GetTimeUntilNextWave()
    {
        if (waveInProgress) return 0f;
        
        if (currentWaveIndex > 0 && currentWaveIndex <= waves.Count)
        {
            return waves[currentWaveIndex - 1].timeBeforeNextWave;
        }
        
        return timeBetweenWaves;
    }
}