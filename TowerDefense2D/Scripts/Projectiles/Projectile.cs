using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed = 10f;
    public float damage = 25f;
    public float lifetime = 5f;
    
    [Header("Effects")]
    public GameObject hitEffect;
    public GameObject trailEffect;
    
    [Header("Special Properties")]
    public bool piercing = false;
    public int maxPierceTargets = 3;
    public bool explosive = false;
    public float explosionRadius = 1f;
    public bool slowing = false;
    public float slowDuration = 2f;
    public float slowAmount = 0.5f;
    public bool freezing = false;
    public float freezeDuration = 1f;
    
    private Enemy target;
    private TowerType towerType;
    private Vector3 targetPosition;
    private bool hasTarget;
    private int pierceCount = 0;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;
    }
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
        
        if (trailEffect != null)
        {
            Instantiate(trailEffect, transform);
        }
    }
    
    private void Update()
    {
        MoveProjectile();
    }
    
    public void Initialize(Enemy targetEnemy, float projectileDamage, TowerType type)
    {
        target = targetEnemy;
        damage = projectileDamage;
        towerType = type;
        hasTarget = target != null;
        
        if (hasTarget)
        {
            targetPosition = target.transform.position;
        }
        
        SetProjectileProperties();
    }
    
    private void SetProjectileProperties()
    {
        switch (towerType)
        {
            case TowerType.Basic:
                // Standard projectile
                break;
            case TowerType.Sniper:
                speed *= 2f;
                break;
            case TowerType.Splash:
                explosive = true;
                explosionRadius = 1.5f;
                break;
            case TowerType.Slow:
                slowing = true;
                slowDuration = 3f;
                slowAmount = 0.3f;
                break;
            case TowerType.Freeze:
                freezing = true;
                freezeDuration = 2f;
                break;
        }
    }
    
    private void MoveProjectile()
    {
        if (!hasTarget)
        {
            // Move in straight line
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            return;
        }
        
        if (target != null && !target.IsDead)
        {
            // Update target position for homing
            targetPosition = target.transform.position;
        }
        
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // Rotate to face target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // Move towards target
        rb.velocity = direction * speed;
        
        // Check if close enough to target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            HitTarget();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.IsDead)
        {
            HitEnemy(enemy);
        }
    }
    
    private void HitTarget()
    {
        if (target != null && !target.IsDead)
        {
            HitEnemy(target);
        }
        else
        {
            CreateHitEffect();
            DestroyProjectile();
        }
    }
    
    private void HitEnemy(Enemy enemy)
    {
        // Apply damage
        enemy.TakeDamage(damage);
        
        // Apply special effects
        ApplySpecialEffects(enemy);
        
        // Handle explosion
        if (explosive)
        {
            Explode();
        }
        
        // Handle piercing
        if (piercing && pierceCount < maxPierceTargets)
        {
            pierceCount++;
            CreateHitEffect();
            
            if (pierceCount >= maxPierceTargets)
            {
                DestroyProjectile();
            }
        }
        else
        {
            CreateHitEffect();
            DestroyProjectile();
        }
    }
    
    private void ApplySpecialEffects(Enemy enemy)
    {
        if (slowing)
        {
            enemy.ApplySlow(slowDuration, slowAmount);
        }
        
        if (freezing)
        {
            enemy.ApplyFreeze(freezeDuration);
        }
    }
    
    private void Explode()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (Collider2D collider in enemiesInRange)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                // Reduced damage for explosion
                enemy.TakeDamage(damage * 0.7f);
                ApplySpecialEffects(enemy);
            }
        }
        
        // Create explosion effect
        if (hitEffect != null)
        {
            GameObject explosion = Instantiate(hitEffect, transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * explosionRadius;
        }
    }
    
    private void CreateHitEffect()
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
    }
    
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (explosive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}