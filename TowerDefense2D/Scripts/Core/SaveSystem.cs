using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    public int highScore;
    public int currentLevel;
    public int totalMoney;
    public bool[] unlockedTowers;
    public float[] audioSettings; // [master, music, sfx]
    public bool[] gameSettings; // Various boolean settings
    
    public GameSaveData()
    {
        highScore = 0;
        currentLevel = 1;
        totalMoney = 0;
        unlockedTowers = new bool[5] { true, false, false, false, false };
        audioSettings = new float[3] { 1f, 0.7f, 0.8f };
        gameSettings = new bool[3] { true, true, false }; // [showFPS, hapticFeedback, tutorialCompleted]
    }
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    
    private GameSaveData currentSaveData;
    private string saveFilePath;
    
    public delegate void SaveEvent();
    public static event SaveEvent OnGameSaved;
    public static event SaveEvent OnGameLoaded;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSaveSystem()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        LoadGame();
    }
    
    public void SaveGame()
    {
        try
        {
            // Update save data with current game state
            UpdateSaveData();
            
            string jsonData = JsonUtility.ToJson(currentSaveData, true);
            File.WriteAllText(saveFilePath, jsonData);
            
            Debug.Log("Game saved successfully!");
            OnGameSaved?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }
    
    public void LoadGame()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string jsonData = File.ReadAllText(saveFilePath);
                currentSaveData = JsonUtility.FromJson<GameSaveData>(jsonData);
                
                Debug.Log("Game loaded successfully!");
                ApplySaveData();
                OnGameLoaded?.Invoke();
            }
            else
            {
                Debug.Log("No save file found, creating new save data.");
                currentSaveData = new GameSaveData();
                SaveGame();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            currentSaveData = new GameSaveData();
        }
    }
    
    private void UpdateSaveData()
    {
        if (GameManager.Instance != null)
        {
            // Update high score
            if (GameManager.Instance.score > currentSaveData.highScore)
            {
                currentSaveData.highScore = GameManager.Instance.score;
            }
            
            // Update current level (wave)
            currentSaveData.currentLevel = Mathf.Max(currentSaveData.currentLevel, GameManager.Instance.currentWave);
        }
        
        // Update audio settings
        if (AudioManager.Instance != null)
        {
            currentSaveData.audioSettings[0] = AudioManager.Instance.masterVolume;
            currentSaveData.audioSettings[1] = AudioManager.Instance.musicVolume;
            currentSaveData.audioSettings[2] = AudioManager.Instance.sfxVolume;
        }
        
        // Update tower unlocks based on level progression
        UpdateTowerUnlocks();
    }
    
    private void ApplySaveData()
    {
        // Apply audio settings
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(currentSaveData.audioSettings[0]);
            AudioManager.Instance.SetMusicVolume(currentSaveData.audioSettings[1]);
            AudioManager.Instance.SetSFXVolume(currentSaveData.audioSettings[2]);
        }
        
        // Apply other settings as needed
    }
    
    private void UpdateTowerUnlocks()
    {
        // Unlock towers based on level progression
        int level = currentSaveData.currentLevel;
        
        if (level >= 3) currentSaveData.unlockedTowers[1] = true; // Sniper
        if (level >= 5) currentSaveData.unlockedTowers[2] = true; // Splash
        if (level >= 7) currentSaveData.unlockedTowers[3] = true; // Slow
        if (level >= 10) currentSaveData.unlockedTowers[4] = true; // Freeze
    }
    
    public void DeleteSave()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("Save file deleted.");
            }
            
            currentSaveData = new GameSaveData();
            SaveGame();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to delete save: {e.Message}");
        }
    }
    
    public void SetHighScore(int score)
    {
        if (score > currentSaveData.highScore)
        {
            currentSaveData.highScore = score;
            SaveGame();
        }
    }
    
    public int GetHighScore()
    {
        return currentSaveData.highScore;
    }
    
    public bool IsTowerUnlocked(int towerIndex)
    {
        if (towerIndex >= 0 && towerIndex < currentSaveData.unlockedTowers.Length)
        {
            return currentSaveData.unlockedTowers[towerIndex];
        }
        return false;
    }
    
    public void UnlockTower(int towerIndex)
    {
        if (towerIndex >= 0 && towerIndex < currentSaveData.unlockedTowers.Length)
        {
            currentSaveData.unlockedTowers[towerIndex] = true;
            SaveGame();
        }
    }
    
    public int GetCurrentLevel()
    {
        return currentSaveData.currentLevel;
    }
    
    public void SetCurrentLevel(int level)
    {
        currentSaveData.currentLevel = level;
        SaveGame();
    }
    
    public GameSaveData GetSaveData()
    {
        return currentSaveData;
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGame();
        }
    }
    
    private void OnDestroy()
    {
        SaveGame();
    }
}