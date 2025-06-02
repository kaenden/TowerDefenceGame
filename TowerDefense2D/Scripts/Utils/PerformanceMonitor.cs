using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerformanceMonitor : MonoBehaviour
{
    [Header("Display Settings")]
    public bool showFPS = true;
    public bool showMemory = true;
    public bool showDrawCalls = false;
    public TextMeshProUGUI displayText;
    
    [Header("Performance Thresholds")]
    public float lowFPSThreshold = 30f;
    public float criticalFPSThreshold = 15f;
    
    [Header("Update Settings")]
    public float updateInterval = 0.5f;
    
    private float deltaTime = 0f;
    private float fps = 0f;
    private float memoryUsage = 0f;
    private int drawCalls = 0;
    private float timer = 0f;
    
    public delegate void PerformanceEvent(float fps);
    public static event PerformanceEvent OnLowFPS;
    public static event PerformanceEvent OnCriticalFPS;
    
    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        timer += Time.unscaledDeltaTime;
        
        if (timer >= updateInterval)
        {
            UpdatePerformanceMetrics();
            UpdateDisplay();
            CheckPerformanceThresholds();
            timer = 0f;
        }
    }
    
    private void UpdatePerformanceMetrics()
    {
        fps = 1.0f / deltaTime;
        
        if (showMemory)
        {
            memoryUsage = System.GC.GetTotalMemory(false) / (1024f * 1024f); // MB
        }
        
        if (showDrawCalls)
        {
            // Note: This is an approximation, actual draw calls would need profiler
            drawCalls = FindObjectsOfType<Renderer>().Length;
        }
    }
    
    private void UpdateDisplay()
    {
        if (displayText == null) return;
        
        string displayString = "";
        
        if (showFPS)
        {
            Color fpsColor = GetFPSColor(fps);
            displayString += $"<color=#{ColorUtility.ToHtmlStringRGB(fpsColor)}>FPS: {fps:F1}</color>\n";
        }
        
        if (showMemory)
        {
            displayString += $"Memory: {memoryUsage:F1} MB\n";
        }
        
        if (showDrawCalls)
        {
            displayString += $"Draw Calls: {drawCalls}\n";
        }
        
        // Add additional info
        displayString += $"Enemies: {FindObjectsOfType<Enemy>().Length}\n";
        displayString += $"Projectiles: {FindObjectsOfType<Projectile>().Length}";
        
        displayText.text = displayString;
    }
    
    private Color GetFPSColor(float currentFPS)
    {
        if (currentFPS >= lowFPSThreshold)
        {
            return Color.green;
        }
        else if (currentFPS >= criticalFPSThreshold)
        {
            return Color.yellow;
        }
        else
        {
            return Color.red;
        }
    }
    
    private void CheckPerformanceThresholds()
    {
        if (fps <= criticalFPSThreshold)
        {
            OnCriticalFPS?.Invoke(fps);
        }
        else if (fps <= lowFPSThreshold)
        {
            OnLowFPS?.Invoke(fps);
        }
    }
    
    public void SetDisplayEnabled(bool enabled)
    {
        if (displayText != null)
        {
            displayText.gameObject.SetActive(enabled);
        }
    }
    
    public float GetCurrentFPS()
    {
        return fps;
    }
    
    public float GetMemoryUsage()
    {
        return memoryUsage;
    }
    
    public bool IsPerformanceGood()
    {
        return fps >= lowFPSThreshold;
    }
    
    // Performance optimization suggestions
    public void OptimizePerformance()
    {
        if (fps < criticalFPSThreshold)
        {
            // Reduce quality settings
            QualitySettings.SetQualityLevel(0);
            
            // Disable non-essential effects
            ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
            foreach (ParticleSystem ps in particles)
            {
                ps.gameObject.SetActive(false);
            }
            
            // Reduce update frequencies
            Time.fixedDeltaTime = 0.04f; // 25 FPS physics
        }
        else if (fps < lowFPSThreshold)
        {
            // Moderate optimizations
            QualitySettings.SetQualityLevel(1);
            
            // Reduce particle counts
            ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
            foreach (ParticleSystem ps in particles)
            {
                var main = ps.main;
                main.maxParticles = Mathf.Max(1, main.maxParticles / 2);
            }
        }
    }
}