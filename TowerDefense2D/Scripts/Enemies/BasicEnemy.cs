using UnityEngine;

public class BasicEnemy : Enemy
{
    [Header("Basic Enemy Settings")]
    public float baseHealth = 100f;
    public float baseSpeed = 2f;
    public int baseMoney = 10;
    public int baseScore = 50;
    
    protected override void Start()
    {
        // Set base stats
        maxHealth = baseHealth;
        moveSpeed = baseSpeed;
        moneyReward = baseMoney;
        scoreReward = baseScore;
        
        base.Start();
    }
}

public class FastEnemy : Enemy
{
    [Header("Fast Enemy Settings")]
    public float baseHealth = 60f;
    public float baseSpeed = 4f;
    public int baseMoney = 15;
    public int baseScore = 75;
    
    protected override void Start()
    {
        maxHealth = baseHealth;
        moveSpeed = baseSpeed;
        moneyReward = baseMoney;
        scoreReward = baseScore;
        
        base.Start();
    }
}

public class TankEnemy : Enemy
{
    [Header("Tank Enemy Settings")]
    public float baseHealth = 300f;
    public float baseSpeed = 1f;
    public int baseMoney = 30;
    public int baseScore = 150;
    
    [Header("Tank Abilities")]
    public bool hasArmor = true;
    public float armorReduction = 0.5f;
    
    protected override void Start()
    {
        maxHealth = baseHealth;
        moveSpeed = baseSpeed;
        moneyReward = baseMoney;
        scoreReward = baseScore;
        
        base.Start();
    }
    
    public override void TakeDamage(float damage)
    {
        if (hasArmor)
        {
            damage *= (1f - armorReduction);
        }
        
        base.TakeDamage(damage);
    }
}

public class FlyingEnemy : Enemy
{
    [Header("Flying Enemy Settings")]
    public float baseHealth = 80f;
    public float baseSpeed = 3f;
    public int baseMoney = 20;
    public int baseScore = 100;
    
    [Header("Flying Abilities")]
    public bool immuneToSlowTowers = true;
    
    protected override void Start()
    {
        maxHealth = baseHealth;
        moveSpeed = baseSpeed;
        moneyReward = baseMoney;
        scoreReward = baseScore;
        
        base.Start();
    }
    
    public override void ApplySlow(float duration, float slowAmount = 0.5f)
    {
        if (immuneToSlowTowers) return;
        
        base.ApplySlow(duration, slowAmount);
    }
}

public class RegeneratingEnemy : Enemy
{
    [Header("Regenerating Enemy Settings")]
    public float baseHealth = 150f;
    public float baseSpeed = 1.5f;
    public int baseMoney = 25;
    public int baseScore = 125;
    
    [Header("Regeneration")]
    public float regenRate = 5f; // HP per second
    public float regenDelay = 2f; // Delay after taking damage
    
    private float lastDamageTime;
    private bool isRegenerating;
    
    protected override void Start()
    {
        maxHealth = baseHealth;
        moveSpeed = baseSpeed;
        moneyReward = baseMoney;
        scoreReward = baseScore;
        
        base.Start();
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (Time.time - lastDamageTime > regenDelay && currentHealth < maxHealth && !IsDead)
        {
            Regenerate();
        }
    }
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        lastDamageTime = Time.time;
        isRegenerating = false;
    }
    
    private void Regenerate()
    {
        if (!isRegenerating)
        {
            isRegenerating = true;
        }
        
        currentHealth += regenRate * Time.deltaTime;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }
}