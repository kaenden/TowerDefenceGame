using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI scoreText;
    public Button pauseButton;
    public Button speedButton;
    public Slider healthSlider;
    
    [Header("Tower Selection")]
    public GameObject towerSelectionPanel;
    public Button[] towerButtons;
    public GameObject[] towerPrefabs;
    
    [Header("Tower Info Panel")]
    public GameObject towerInfoPanel;
    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI towerStatsText;
    public Button upgradeButton;
    public Button sellButton;
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI sellValueText;
    
    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Pause Menu")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button pauseRestartButton;
    public Button pauseMainMenuButton;
    
    [Header("Wave Info")]
    public GameObject waveInfoPanel;
    public TextMeshProUGUI nextWaveText;
    public Button startWaveButton;
    public Slider waveProgressSlider;
    
    private TowerPlacement towerPlacement;
    private Tower selectedTower;
    private float[] gameSpeedOptions = { 1f, 1.5f, 2f };
    private int currentSpeedIndex = 0;
    
    private void Awake()
    {
        towerPlacement = FindObjectOfType<TowerPlacement>();
    }
    
    private void Start()
    {
        SetupUI();
        UpdateHUD();
    }
    
    private void OnEnable()
    {
        GameManager.OnLivesChanged += UpdateLives;
        GameManager.OnMoneyChanged += UpdateMoney;
        GameManager.OnGameOver += ShowGameOver;
        GameManager.OnWaveComplete += OnWaveComplete;
        
        TowerPlacement.OnTowerSelected += OnTowerSelected;
        TowerPlacement.OnTowerDeselected += OnTowerDeselected;
        
        WaveManager.OnWaveStart += OnWaveStart;
        WaveManager.OnWaveComplete += OnWaveComplete;
    }
    
    private void OnDisable()
    {
        GameManager.OnLivesChanged -= UpdateLives;
        GameManager.OnMoneyChanged -= UpdateMoney;
        GameManager.OnGameOver -= ShowGameOver;
        GameManager.OnWaveComplete -= OnWaveComplete;
        
        TowerPlacement.OnTowerSelected -= OnTowerSelected;
        TowerPlacement.OnTowerDeselected -= OnTowerDeselected;
        
        WaveManager.OnWaveStart -= OnWaveStart;
        WaveManager.OnWaveComplete -= OnWaveComplete;
    }
    
    private void SetupUI()
    {
        // Setup tower buttons
        for (int i = 0; i < towerButtons.Length && i < towerPrefabs.Length; i++)
        {
            int index = i; // Capture for closure
            towerButtons[i].onClick.AddListener(() => SelectTowerToBuild(index));
            
            // Update button text with tower cost
            Tower towerComponent = towerPrefabs[i].GetComponent<Tower>();
            if (towerComponent != null)
            {
                TextMeshProUGUI buttonText = towerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = $"{towerComponent.towerType}\n${towerComponent.cost}";
                }
            }
        }
        
        // Setup other buttons
        pauseButton?.onClick.AddListener(TogglePause);
        speedButton?.onClick.AddListener(ChangeGameSpeed);
        upgradeButton?.onClick.AddListener(UpgradeTower);
        sellButton?.onClick.AddListener(SellTower);
        startWaveButton?.onClick.AddListener(StartNextWave);
        
        restartButton?.onClick.AddListener(RestartGame);
        mainMenuButton?.onClick.AddListener(GoToMainMenu);
        resumeButton?.onClick.AddListener(TogglePause);
        pauseRestartButton?.onClick.AddListener(RestartGame);
        pauseMainMenuButton?.onClick.AddListener(GoToMainMenu);
        
        // Hide panels initially
        towerInfoPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        pausePanel?.SetActive(false);
    }
    
    private void UpdateHUD()
    {
        UpdateLives();
        UpdateMoney();
        UpdateWave();
        UpdateScore();
    }
    
    private void UpdateLives()
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {GameManager.Instance.currentLives}";
        }
        
        if (healthSlider != null)
        {
            healthSlider.value = (float)GameManager.Instance.currentLives / GameManager.Instance.startingLives;
        }
    }
    
    private void UpdateMoney()
    {
        if (moneyText != null)
        {
            moneyText.text = $"${GameManager.Instance.currentMoney}";
        }
        
        // Update tower button interactability
        for (int i = 0; i < towerButtons.Length && i < towerPrefabs.Length; i++)
        {
            Tower towerComponent = towerPrefabs[i].GetComponent<Tower>();
            if (towerComponent != null)
            {
                towerButtons[i].interactable = GameManager.Instance.currentMoney >= towerComponent.cost;
            }
        }
        
        // Update upgrade button
        if (selectedTower != null && upgradeButton != null)
        {
            upgradeButton.interactable = selectedTower.CanUpgrade();
        }
    }
    
    private void UpdateWave()
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {GameManager.Instance.currentWave}";
        }
    }
    
    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {GameManager.Instance.score}";
        }
    }
    
    private void SelectTowerToBuild(int towerIndex)
    {
        if (towerIndex >= 0 && towerIndex < towerPrefabs.Length)
        {
            towerPlacement.StartPlacingTower(towerPrefabs[towerIndex]);
        }
    }
    
    private void OnTowerSelected(Tower tower)
    {
        selectedTower = tower;
        ShowTowerInfo(tower);
    }
    
    private void OnTowerDeselected(Tower tower)
    {
        selectedTower = null;
        HideTowerInfo();
    }
    
    private void ShowTowerInfo(Tower tower)
    {
        if (towerInfoPanel == null) return;
        
        towerInfoPanel.SetActive(true);
        
        if (towerNameText != null)
        {
            towerNameText.text = $"{tower.towerType} Tower (Level {tower.upgradeLevel})";
        }
        
        if (towerStatsText != null)
        {
            towerStatsText.text = $"Damage: {tower.damage:F1}\n" +
                                 $"Range: {tower.range:F1}\n" +
                                 $"Fire Rate: {tower.fireRate:F1}";
        }
        
        if (upgradeCostText != null)
        {
            if (tower.CanUpgrade())
            {
                upgradeCostText.text = $"Upgrade: ${tower.GetUpgradeCost()}";
            }
            else
            {
                upgradeCostText.text = "Max Level";
            }
        }
        
        if (sellValueText != null)
        {
            sellValueText.text = $"Sell: ${tower.GetSellValue()}";
        }
        
        if (upgradeButton != null)
        {
            upgradeButton.interactable = tower.CanUpgrade();
        }
    }
    
    private void HideTowerInfo()
    {
        towerInfoPanel?.SetActive(false);
    }
    
    private void UpgradeTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Upgrade();
            ShowTowerInfo(selectedTower); // Refresh info
        }
    }
    
    private void SellTower()
    {
        if (selectedTower != null)
        {
            Vector3 towerPosition = selectedTower.transform.position;
            selectedTower.Sell();
            towerPlacement.RemoveTowerAt(towerPosition);
            HideTowerInfo();
        }
    }
    
    private void TogglePause()
    {
        GameManager.Instance.PauseGame();
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(GameManager.Instance.gamePaused);
        }
    }
    
    private void ChangeGameSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % gameSpeedOptions.Length;
        GameManager.Instance.SetGameSpeed(gameSpeedOptions[currentSpeedIndex]);
        
        if (speedButton != null)
        {
            TextMeshProUGUI buttonText = speedButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"{gameSpeedOptions[currentSpeedIndex]}x";
            }
        }
    }
    
    private void StartNextWave()
    {
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.StartNextWave();
        }
    }
    
    private void OnWaveStart(int waveNumber)
    {
        UpdateWave();
        
        if (startWaveButton != null)
        {
            startWaveButton.interactable = false;
        }
    }
    
    private void OnWaveComplete(int waveNumber)
    {
        UpdateScore();
        
        if (startWaveButton != null)
        {
            startWaveButton.interactable = true;
        }
        
        if (nextWaveText != null)
        {
            nextWaveText.text = $"Next Wave: {waveNumber + 1}";
        }
    }
    
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {GameManager.Instance.score}";
        }
    }
    
    private void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
    
    private void GoToMainMenu()
    {
        // Implement main menu navigation
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    private void Update()
    {
        UpdateScore(); // Update score continuously
        
        // Update wave progress if available
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null && waveProgressSlider != null)
        {
            // This would need implementation in WaveManager to track progress
            // waveProgressSlider.value = waveManager.GetWaveProgress();
        }
    }
}