using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TowerType
{
    Basic,
    Sniper,
    Splash,
    Slow,
    Freeze
}

public enum TargetingMode
{
    First,      // First enemy in path
    Last,       // Last enemy in path
    Closest,    // Closest enemy
    Strongest,  // Enemy with most health
    Weakest     // Enemy with least health
}

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    public TowerType towerType;
    public float damage = 25f;
    public float range = 3f;
    public float fireRate = 1f;
    public int cost = 50;
    
    [Header("Upgrade Stats")]
    public int upgradeLevel = 1;
    public int maxUpgradeLevel = 3;
    public float damageUpgradeMultiplier = 1.5f;
    public float rangeUpgradeMultiplier = 1.2f;
    public float fireRateUpgradeMultiplier = 1.3f;
    public int upgradeCostMultiplier = 2;
    
    [Header("Targeting")]
    public TargetingMode targetingMode = TargetingMode.First;
    public LayerMask enemyLayer = 1;
    
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    
    [Header("Visual")]
    public GameObject rangeIndicator;
    public SpriteRenderer towerSprite;
    
    private float lastFireTime;
    private Enemy currentTarget;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private CircleCollider2D rangeCollider;
    
    public bool IsSelected { get; private set; }
    
    private void Awake()
    {
        SetupRangeCollider();
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }
    
    private void Start()
    {
        UpdateTowerStats();
        ShowRangeIndicator(false);
    }
    
    private void Update()
    {
        if (GameManager.Instance.gameOver || GameManager.Instance.gamePaused) return;
        
        UpdateTarget();
        
        if (currentTarget != null && CanFire())
        {
            Fire();
        }
    }
    
    private void SetupRangeCollider()
    {
        rangeCollider = GetComponent<CircleCollider2D>();
        if (rangeCollider == null)
        {
            rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        rangeCollider.isTrigger = true;
        rangeCollider.radius = range;
    }
    
    private void UpdateTarget()
    {
        // Remove dead or out-of-range enemies
        enemiesInRange.RemoveAll(enemy => enemy == null || enemy.IsDead || 
            Vector3.Distance(transform.position, enemy.transform.position) > range);
        
        if (enemiesInRange.Count == 0)
        {
            currentTarget = null;
            return;
        }
        
        currentTarget = GetBestTarget();
    }
    
    private Enemy GetBestTarget()
    {
        if (enemiesInRange.Count == 0) return null;
        
        switch (targetingMode)
        {
            case TargetingMode.First:
                return GetFirstEnemy();
            case TargetingMode.Last:
                return GetLastEnemy();
            case TargetingMode.Closest:
                return GetClosestEnemy();
            case TargetingMode.Strongest:
                return GetStrongestEnemy();
            case TargetingMode.Weakest:
                return GetWeakestEnemy();
            default:
                return enemiesInRange[0];
        }
    }
    
    private Enemy GetFirstEnemy()
    {
        Enemy firstEnemy = null;
        float shortestDistance = float.MaxValue;
        
        foreach (Enemy enemy in enemiesInRange)
        {
            Pathfinding pathfinding = enemy.GetComponent<Pathfinding>();
            if (pathfinding != null)
            {
                float distanceToEnd = pathfinding.GetDistanceToEnd();
                if (distanceToEnd < shortestDistance)
                {
                    shortestDistance = distanceToEnd;
                    firstEnemy = enemy;
                }
            }
        }
        
        return firstEnemy ?? enemiesInRange[0];
    }
    
    private Enemy GetLastEnemy()
    {
        Enemy lastEnemy = null;
        float longestDistance = 0f;
        
        foreach (Enemy enemy in enemiesInRange)
        {
            Pathfinding pathfinding = enemy.GetComponent<Pathfinding>();
            if (pathfinding != null)
            {
                float distanceToEnd = pathfinding.GetDistanceToEnd();
                if (distanceToEnd > longestDistance)
                {
                    longestDistance = distanceToEnd;
                    lastEnemy = enemy;
                }
            }
        }
        
        return lastEnemy ?? enemiesInRange[0];
    }
    
    private Enemy GetClosestEnemy()
    {
        Enemy closestEnemy = null;
        float shortestDistance = float.MaxValue;
        
        foreach (Enemy enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy;
            }
        }
        
        return closestEnemy;
    }
    
    private Enemy GetStrongestEnemy()
    {
        Enemy strongestEnemy = null;
        float highestHealth = 0f;
        
        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy.GetHealthPercentage() > highestHealth)
            {
                highestHealth = enemy.GetHealthPercentage();
                strongestEnemy = enemy;
            }
        }
        
        return strongestEnemy ?? enemiesInRange[0];
    }
    
    private Enemy GetWeakestEnemy()
    {
        Enemy weakestEnemy = null;
        float lowestHealth = float.MaxValue;
        
        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy.GetHealthPercentage() < lowestHealth)
            {
                lowestHealth = enemy.GetHealthPercentage();
                weakestEnemy = enemy;
            }
        }
        
        return weakestEnemy ?? enemiesInRange[0];
    }
    
    private bool CanFire()
    {
        return Time.time >= lastFireTime + (1f / fireRate);
    }
    
    private void Fire()
    {
        lastFireTime = Time.time;
        
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            
            if (projectileScript != null)
            {
                projectileScript.Initialize(currentTarget, damage, towerType);
            }
        }
        else
        {
            // Direct damage if no projectile
            currentTarget.TakeDamage(damage);
        }
        
        // Rotate tower to face target
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    public bool CanUpgrade()
    {
        return upgradeLevel < maxUpgradeLevel && 
               GameManager.Instance.currentMoney >= GetUpgradeCost();
    }
    
    public int GetUpgradeCost()
    {
        return cost * upgradeCostMultiplier * upgradeLevel;
    }
    
    public void Upgrade()
    {
        if (!CanUpgrade()) return;
        
        int upgradeCost = GetUpgradeCost();
        if (GameManager.Instance.SpendMoney(upgradeCost))
        {
            upgradeLevel++;
            UpdateTowerStats();
        }
    }
    
    private void UpdateTowerStats()
    {
        float levelMultiplier = upgradeLevel - 1;
        
        damage = damage * Mathf.Pow(damageUpgradeMultiplier, levelMultiplier);
        range = range * Mathf.Pow(rangeUpgradeMultiplier, levelMultiplier);
        fireRate = fireRate * Mathf.Pow(fireRateUpgradeMultiplier, levelMultiplier);
        
        if (rangeCollider != null)
        {
            rangeCollider.radius = range;
        }
        
        UpdateRangeIndicator();
    }
    
    public void Select()
    {
        IsSelected = true;
        ShowRangeIndicator(true);
    }
    
    public void Deselect()
    {
        IsSelected = false;
        ShowRangeIndicator(false);
    }
    
    private void ShowRangeIndicator(bool show)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(show);
            UpdateRangeIndicator();
        }
    }
    
    private void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * range * 2f;
        }
    }
    
    public int GetSellValue()
    {
        int totalInvestment = cost;
        for (int i = 1; i < upgradeLevel; i++)
        {
            totalInvestment += cost * upgradeCostMultiplier * i;
        }
        return Mathf.RoundToInt(totalInvestment * 0.7f); // 70% return
    }
    
    public void Sell()
    {
        GameManager.Instance.AddMoney(GetSellValue());
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.IsDead)
        {
            enemiesInRange.Add(enemy);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}