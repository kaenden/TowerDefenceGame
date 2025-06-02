using UnityEngine;
using UnityEngine.UI;

public class CleanGameManager : MonoBehaviour
{
    [Header("Game State")]
    public int lives = 20;
    public int money = 100;
    public int score = 0;
    public int currentWave = 1;
    
    [Header("UI References (Optional - assign in Inspector)")]
    public Text livesText;
    public Text moneyText;
    public Text scoreText;
    public Text waveText;
    
    [Header("Game Status")]
    public bool gameStarted = false;
    public bool gameOver = false;
    public bool gamePaused = false;
    
    // Singleton pattern
    public static CleanGameManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton setup
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
    
    void Start()
    {
        Debug.Log("CleanGameManager started successfully!");
        UpdateUI();
    }
    
    void Update()
    {
        // Check for game over
        if (lives <= 0 && !gameOver)
        {
            GameOver();
        }
    }
    
    public void StartGame()
    {
        gameStarted = true;
        gameOver = false;
        Debug.Log("Game Started!");
    }
    
    public void LoseLife()
    {
        if (gameOver) return;
        
        lives--;
        Debug.Log($"Life lost! Remaining: {lives}");
        UpdateUI();
        
        if (lives <= 0)
        {
            GameOver();
        }
    }
    
    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"Money added: {amount}. Total: {money}");
        UpdateUI();
    }
    
    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            Debug.Log($"Money spent: {amount}. Remaining: {money}");
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("Not enough money!");
            return false;
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score added: {points}. Total: {score}");
        UpdateUI();
    }
    
    public void NextWave()
    {
        currentWave++;
        Debug.Log($"Wave {currentWave} started!");
        UpdateUI();
    }
    
    public void GameOver()
    {
        gameOver = true;
        Debug.Log("Game Over!");
        // You can add game over UI here
    }
    
    public void PauseGame()
    {
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : 1f;
        Debug.Log(gamePaused ? "Game Paused" : "Game Resumed");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void UpdateUI()
    {
        // Update UI elements if they're assigned
        if (livesText != null) livesText.text = "Lives: " + lives;
        if (moneyText != null) moneyText.text = "Money: $" + money;
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (waveText != null) waveText.text = "Wave: " + currentWave;
    }
    
    // Public getters for other scripts
    public int GetLives() { return lives; }
    public int GetMoney() { return money; }
    public int GetScore() { return score; }
    public int GetCurrentWave() { return currentWave; }
    public bool IsGameOver() { return gameOver; }
    public bool IsGameStarted() { return gameStarted; }
}