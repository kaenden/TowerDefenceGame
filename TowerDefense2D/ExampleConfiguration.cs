using UnityEngine;
using System.Collections.Generic;

// This file shows example configurations for your tower defense game
// Copy these settings to your actual GameObjects in the Unity Editor

public class ExampleConfiguration : MonoBehaviour
{
    [Header("Example Game Manager Settings")]
    public int exampleStartingLives = 20;
    public int exampleStartingMoney = 100;
    public float exampleGameSpeed = 1f;
    
    [Header("Example Tower Stats")]
    [System.Serializable]
    public class ExampleTowerStats
    {
        public string towerName;
        public float damage;
        public float range;
        public float fireRate;
        public int cost;
    }
    
    public ExampleTowerStats[] exampleTowers = new ExampleTowerStats[]
    {
        new ExampleTowerStats { towerName = "Basic", damage = 25f, range = 3f, fireRate = 1f, cost = 50 },
        new ExampleTowerStats { towerName = "Sniper", damage = 75f, range = 6f, fireRate = 0.5f, cost = 100 },
        new ExampleTowerStats { towerName = "Splash", damage = 40f, range = 2.5f, fireRate = 0.8f, cost = 80 },
        new ExampleTowerStats { towerName = "Slow", damage = 15f, range = 3.5f, fireRate = 1.2f, cost = 60 },
        new ExampleTowerStats { towerName = "Freeze", damage = 20f, range = 3f, fireRate = 0.7f, cost = 90 }
    };
    
    [Header("Example Enemy Stats")]
    [System.Serializable]
    public class ExampleEnemyStats
    {
        public string enemyName;
        public float health;
        public float speed;
        public int moneyReward;
        public int scoreReward;
    }
    
    public ExampleEnemyStats[] exampleEnemies = new ExampleEnemyStats[]
    {
        new ExampleEnemyStats { enemyName = "Basic", health = 100f, speed = 2f, moneyReward = 10, scoreReward = 50 },
        new ExampleEnemyStats { enemyName = "Fast", health = 60f, speed = 4f, moneyReward = 15, scoreReward = 75 },
        new ExampleEnemyStats { enemyName = "Tank", health = 300f, speed = 1f, moneyReward = 30, scoreReward = 150 },
        new ExampleEnemyStats { enemyName = "Flying", health = 80f, speed = 3f, moneyReward = 20, scoreReward = 100 },
        new ExampleEnemyStats { enemyName = "Regenerating", health = 150f, speed = 1.5f, moneyReward = 25, scoreReward = 125 }
    };
    
    [Header("Example Wave Configuration")]
    public void ConfigureExampleWaves()
    {
        // This is example code showing how to set up waves
        // You would implement this in your WaveManager
        
        /*
        Wave wave1 = new Wave
        {
            waveName = "Wave 1",
            enemies = new List<EnemySpawnInfo>
            {
                new EnemySpawnInfo { enemyPrefab = basicEnemyPrefab, count = 10, spawnDelay = 0f }
            },
            timeBetweenSpawns = 1f,
            timeBeforeNextWave = 5f
        };
        
        Wave wave2 = new Wave
        {
            waveName = "Wave 2",
            enemies = new List<EnemySpawnInfo>
            {
                new EnemySpawnInfo { enemyPrefab = basicEnemyPrefab, count = 8, spawnDelay = 0f },
                new EnemySpawnInfo { enemyPrefab = fastEnemyPrefab, count = 5, spawnDelay = 2f }
            },
            timeBetweenSpawns = 0.8f,
            timeBeforeNextWave = 5f
        };
        
        Wave wave3 = new Wave
        {
            waveName = "Wave 3 - Tank Wave",
            enemies = new List<EnemySpawnInfo>
            {
                new EnemySpawnInfo { enemyPrefab = tankEnemyPrefab, count = 3, spawnDelay = 0f },
                new EnemySpawnInfo { enemyPrefab = basicEnemyPrefab, count = 15, spawnDelay = 1f }
            },
            timeBetweenSpawns = 0.6f,
            timeBeforeNextWave = 8f
        };
        */
    }
    
    [Header("Example Waypoint Positions")]
    public Vector3[] exampleWaypoints = new Vector3[]
    {
        new Vector3(-8f, 0f, 0f),    // Start point (left side)
        new Vector3(-4f, 2f, 0f),    // First turn
        new Vector3(0f, 2f, 0f),     // Middle high
        new Vector3(4f, -2f, 0f),    // Second turn
        new Vector3(8f, -2f, 0f),    // End point (right side)
    };
    
    [Header("Example UI Layout")]
    [System.Serializable]
    public class ExampleUILayout
    {
        public string elementName;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
    }
    
    public ExampleUILayout[] exampleUIElements = new ExampleUILayout[]
    {
        new ExampleUILayout 
        { 
            elementName = "Lives Text", 
            anchorMin = new Vector2(0f, 1f), 
            anchorMax = new Vector2(0f, 1f),
            anchoredPosition = new Vector2(100f, -30f),
            sizeDelta = new Vector2(200f, 50f)
        },
        new ExampleUILayout 
        { 
            elementName = "Money Text", 
            anchorMin = new Vector2(1f, 1f), 
            anchorMax = new Vector2(1f, 1f),
            anchoredPosition = new Vector2(-100f, -30f),
            sizeDelta = new Vector2(200f, 50f)
        },
        new ExampleUILayout 
        { 
            elementName = "Tower Selection Panel", 
            anchorMin = new Vector2(0f, 0f), 
            anchorMax = new Vector2(1f, 0f),
            anchoredPosition = new Vector2(0f, 100f),
            sizeDelta = new Vector2(0f, 150f)
        }
    };
    
    [Header("Example Audio Clips")]
    public string[] exampleSoundNames = new string[]
    {
        "TowerPlace",
        "TowerShoot",
        "EnemyHit",
        "EnemyDeath",
        "WaveStart",
        "WaveComplete",
        "GameOver",
        "ButtonClick",
        "MoneyEarn",
        "TowerUpgrade"
    };
    
    [Header("Example Performance Settings")]
    public int maxEnemiesOnScreen = 50;
    public int maxProjectilesOnScreen = 100;
    public float targetFrameRate = 60f;
    public bool enableObjectPooling = true;
    public bool enableParticleEffects = true;
    
    // Example method to show how to balance difficulty
    public float CalculateEnemyHealthForWave(int waveNumber, float baseHealth)
    {
        // Increase enemy health by 15% each wave
        return baseHealth * Mathf.Pow(1.15f, waveNumber - 1);
    }
    
    public int CalculateEnemyCountForWave(int waveNumber, int baseCount)
    {
        // Increase enemy count more gradually
        return baseCount + (waveNumber - 1) * 2;
    }
    
    public int CalculateWaveReward(int waveNumber)
    {
        // Give more money for completing later waves
        return 50 + (waveNumber * 10);
    }
    
    // Example method for tower upgrade costs
    public int CalculateTowerUpgradeCost(int baseCost, int currentLevel)
    {
        return baseCost * currentLevel * 2;
    }
    
    // Example method for tower stat scaling
    public float CalculateUpgradedStat(float baseStat, int level, float multiplier = 1.5f)
    {
        return baseStat * Mathf.Pow(multiplier, level - 1);
    }
}