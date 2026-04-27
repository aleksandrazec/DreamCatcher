using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private HealthBar healthBar;

    private float health;
    public bool isDead;
    private void Awake()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        isDead = false;
    }
    public void TakeDamage(float damage, Vector3 knockbackDirection)
    {
        if (!isDead)
        {
            health -= damage;
            healthBar.SetHealth(health);
            Debug.Log("Enemy health: " + health);
            if (health <= 0)
            {
                isDead = true;
                enemyAI.Die();
                healthBar.Hide();
            }
            else
            {
                enemyAI.TakeDamage(knockbackDirection);
            }
        }
    }
}
