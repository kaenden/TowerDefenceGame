using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public int startingLives = 20;
    public int startingMoney = 100;
    public float gameSpeed = 1f;
    
    [Header("Game State")]
    public int currentLives;
    public int currentMoney;
    public int currentWave = 0;
    public int score = 0;
    public bool gameOver = false;
    public bool gamePaused = false;
    
    [Header("References")]
    public WaveManager waveManager;
    public UIManager uiManager;
    
    public delegate void OnGameStateChanged();
    public static event OnGameStateChanged OnLivesChanged;
    public static event OnGameStateChanged OnMoneyChanged;
    public static event OnGameStateChanged OnGameOver;
    public static event OnGameStateChanged OnWaveComplete;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        currentLives = startingLives;
        currentMoney = startingMoney;
        currentWave = 0;
        score = 0;
        gameOver = false;
        gamePaused = false;
        
        Time.timeScale = gameSpeed;
        
        OnLivesChanged?.Invoke();
        OnMoneyChanged?.Invoke();
    }
    
    public void LoseLife(int amount = 1)
    {
        if (gameOver) return;
        
        currentLives -= amount;
        OnLivesChanged?.Invoke();
        
        if (currentLives <= 0)
        {
            GameOver();
        }
    }
    
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke();
    }
    
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke();
            return true;
        }
        return false;
    }
    
    public void AddScore(int points)
    {
        score += points;
    }
    
    public void CompleteWave()
    {
        currentWave++;
        AddMoney(50 + (currentWave * 10)); // Bonus money for completing wave
        OnWaveComplete?.Invoke();
    }
    
    public void GameOver()
    {
        gameOver = true;
        Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }
    
    public void PauseGame()
    {
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : gameSpeed;
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void SetGameSpeed(float speed)
    {
        gameSpeed = speed;
        if (!gamePaused && !gameOver)
        {
            Time.timeScale = gameSpeed;
        }
    }
}