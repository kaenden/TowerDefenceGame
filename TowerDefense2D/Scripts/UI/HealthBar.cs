using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Slider healthSlider;
    public Image fillImage;
    public Image backgroundImage;
    public bool followTarget = true;
    public Vector3 offset = Vector3.up;
    public bool hideWhenFull = true;
    public bool scaleWithDistance = true;
    
    [Header("Colors")]
    public Color fullHealthColor = Color.green;
    public Color halfHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    public float lowHealthThreshold = 0.3f;
    public float halfHealthThreshold = 0.6f;
    
    [Header("Animation")]
    public bool animateChanges = true;
    public float animationSpeed = 5f;
    
    private Transform target;
    private Camera mainCamera;
    private Canvas canvas;
    private float maxHealth;
    private float currentHealth;
    private float targetHealthValue;
    private bool isVisible = true;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        canvas = GetComponentInParent<Canvas>();
        target = transform.parent;
        
        if (healthSlider == null)
        {
            healthSlider = GetComponent<Slider>();
        }
        
        if (fillImage == null && healthSlider != null)
        {
            fillImage = healthSlider.fillRect.GetComponent<Image>();
        }
    }
    
    private void Start()
    {
        if (hideWhenFull)
        {
            SetVisible(false);
        }
    }
    
    private void Update()
    {
        if (followTarget && target != null)
        {
            UpdatePosition();
        }
        
        if (animateChanges && healthSlider != null)
        {
            AnimateHealthBar();
        }
        
        if (scaleWithDistance)
        {
            UpdateScale();
        }
    }
    
    private void UpdatePosition()
    {
        if (mainCamera == null || canvas == null) return;
        
        Vector3 worldPosition = target.position + offset;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        
        // Check if the target is behind the camera
        if (screenPosition.z < 0)
        {
            SetVisible(false);
            return;
        }
        
        // Convert screen position to canvas position
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out canvasPosition
        );
        
        transform.localPosition = canvasPosition;
        
        if (!isVisible && currentHealth < maxHealth)
        {
            SetVisible(true);
        }
    }
    
    private void UpdateScale()
    {
        if (mainCamera == null || target == null) return;
        
        float distance = Vector3.Distance(mainCamera.transform.position, target.position);
        float scale = Mathf.Clamp(1f / (distance * 0.1f), 0.5f, 2f);
        transform.localScale = Vector3.one * scale;
    }
    
    private void AnimateHealthBar()
    {
        if (healthSlider.value != targetHealthValue)
        {
            healthSlider.value = Mathf.MoveTowards(
                healthSlider.value,
                targetHealthValue,
                animationSpeed * Time.deltaTime
            );
        }
    }
    
    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;
        targetHealthValue = 1f;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
        }
        
        UpdateHealthColor();
        
        if (hideWhenFull)
        {
            SetVisible(false);
        }
    }
    
    public void SetHealth(float health)
    {
        currentHealth = health;
        targetHealthValue = currentHealth / maxHealth;
        
        if (!animateChanges && healthSlider != null)
        {
            healthSlider.value = targetHealthValue;
        }
        
        UpdateHealthColor();
        
        if (hideWhenFull)
        {
            SetVisible(currentHealth < maxHealth);
        }
    }
    
    private void UpdateHealthColor()
    {
        if (fillImage == null) return;
        
        float healthPercentage = currentHealth / maxHealth;
        
        Color targetColor;
        if (healthPercentage <= lowHealthThreshold)
        {
            targetColor = lowHealthColor;
        }
        else if (healthPercentage <= halfHealthThreshold)
        {
            float t = (healthPercentage - lowHealthThreshold) / (halfHealthThreshold - lowHealthThreshold);
            targetColor = Color.Lerp(lowHealthColor, halfHealthColor, t);
        }
        else
        {
            float t = (healthPercentage - halfHealthThreshold) / (1f - halfHealthThreshold);
            targetColor = Color.Lerp(halfHealthColor, fullHealthColor, t);
        }
        
        fillImage.color = targetColor;
    }
    
    public void SetVisible(bool visible)
    {
        isVisible = visible;
        gameObject.SetActive(visible);
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    public void Flash(Color flashColor, float duration = 0.2f)
    {
        if (fillImage != null)
        {
            StartCoroutine(FlashCoroutine(flashColor, duration));
        }
    }
    
    private System.Collections.IEnumerator FlashCoroutine(Color flashColor, float duration)
    {
        Color originalColor = fillImage.color;
        fillImage.color = flashColor;
        
        yield return new WaitForSeconds(duration);
        
        fillImage.color = originalColor;
    }
    
    public void Shake(float intensity = 1f, float duration = 0.3f)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }
    
    private System.Collections.IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            Vector3 randomOffset = Random.insideUnitCircle * intensity;
            transform.localPosition = originalPosition + randomOffset;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalPosition;
    }
}