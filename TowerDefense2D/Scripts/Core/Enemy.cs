using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public int moneyReward = 10;
    public int scoreReward = 50;
    
    [Header("Status Effects")]
    public bool isSlowed = false;
    public bool isFrozen = false;
    public float slowMultiplier = 0.5f;
    
    [Header("Visual")]
    public GameObject deathEffect;
    public HealthBar healthBar;
    
    protected float currentHealth;
    private float currentMoveSpeed;
    private Pathfinding pathfinding;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    public delegate void EnemyEvent();
    public static event EnemyEvent OnEnemyDestroyed;
    public static event EnemyEvent OnEnemyReachedEnd;
    
    public bool IsDead { get; private set; }
    
    private void Awake()
    {
        pathfinding = GetComponent<Pathfinding>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        currentMoveSpeed = moveSpeed;
        
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        
        if (pathfinding != null)
        {
            pathfinding.SetMoveSpeed(currentMoveSpeed);
        }
    }
    
    protected virtual void Update()
    {
        // Override in derived classes for special behaviors
    }
    
    public virtual void TakeDamage(float damage)
    {
        if (IsDead) return;
        
        currentHealth -= damage;
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        // Flash red when taking damage
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Die(bool giveRewards = true)
    {
        if (IsDead) return;
        
        IsDead = true;
        
        if (giveRewards)
        {
            GameManager.Instance.AddMoney(moneyReward);
            GameManager.Instance.AddScore(scoreReward);
        }
        
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        OnEnemyDestroyed?.Invoke();
        Destroy(gameObject);
    }
    
    public void ReachEnd()
    {
        if (IsDead) return;
        
        GameManager.Instance.LoseLife();
        OnEnemyReachedEnd?.Invoke();
        Destroy(gameObject);
    }
    
    public virtual void ApplySlow(float duration, float slowAmount = 0.5f)
    {
        if (IsDead || isFrozen) return;
        
        StartCoroutine(SlowEffect(duration, slowAmount));
    }
    
    public void ApplyFreeze(float duration)
    {
        if (IsDead) return;
        
        StartCoroutine(FreezeEffect(duration));
    }
    
    private IEnumerator SlowEffect(float duration, float slowAmount)
    {
        if (isSlowed) yield break;
        
        isSlowed = true;
        slowMultiplier = slowAmount;
        UpdateMoveSpeed();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.blue;
        }
        
        yield return new WaitForSeconds(duration);
        
        isSlowed = false;
        slowMultiplier = 1f;
        UpdateMoveSpeed();
        
        if (spriteRenderer != null && !isFrozen)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    private IEnumerator FreezeEffect(float duration)
    {
        isFrozen = true;
        UpdateMoveSpeed();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.cyan;
        }
        
        yield return new WaitForSeconds(duration);
        
        isFrozen = false;
        UpdateMoveSpeed();
        
        if (spriteRenderer != null && !isSlowed)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    private void UpdateMoveSpeed()
    {
        if (isFrozen)
        {
            currentMoveSpeed = 0f;
        }
        else if (isSlowed)
        {
            currentMoveSpeed = moveSpeed * slowMultiplier;
        }
        else
        {
            currentMoveSpeed = moveSpeed;
        }
        
        if (pathfinding != null)
        {
            pathfinding.SetMoveSpeed(currentMoveSpeed);
        }
    }
    
    private IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            
            if (!isSlowed && !isFrozen)
            {
                spriteRenderer.color = originalColor;
            }
            else if (isSlowed)
            {
                spriteRenderer.color = Color.blue;
            }
            else if (isFrozen)
            {
                spriteRenderer.color = Color.cyan;
            }
        }
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}