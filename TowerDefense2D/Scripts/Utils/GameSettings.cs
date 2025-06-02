using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Tower Defense/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Game Balance")]
    public int startingLives = 20;
    public int startingMoney = 100;
    public float baseGameSpeed = 1f;
    
    [Header("Tower Settings")]
    public TowerData[] towerTypes;
    
    [Header("Wave Settings")]
    public float baseWaveDelay = 5f;
    public float difficultyScaling = 1.1f;
    
    [Header("UI Settings")]
    public bool showFPS = false;
    public bool enableHapticFeedback = true;
    
    [Header("Audio Settings")]
    public float defaultMasterVolume = 1f;
    public float defaultMusicVolume = 0.7f;
    public float defaultSFXVolume = 0.8f;
    
    [Header("Performance")]
    public int maxEnemiesOnScreen = 50;
    public int maxProjectilesOnScreen = 100;
    public bool useObjectPooling = true;
    
    [Header("Mobile Settings")]
    public float touchSensitivity = 1f;
    public float doubleTapTime = 0.3f;
    public float longPressTime = 0.8f;
    public bool enableCameraZoom = true;
    public float minCameraZoom = 3f;
    public float maxCameraZoom = 8f;
}

[System.Serializable]
public class TowerData
{
    public string towerName;
    public TowerType towerType;
    public GameObject prefab;
    public Sprite icon;
    public int baseCost;
    public string description;
    public bool unlockedByDefault = true;
    public int unlockLevel = 1;
}