using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private EnemyAI enemyAI;

    public bool isDead;
    private void Awake()
    {
        isDead = false;
    }
    public void TakeDamage(float damage, Vector3 knockbackDirection)
    {
        if (!isDead)
        {
            health -= damage;
            Debug.Log("Enemy health: " + health);
            if (health <= 0)
            {
                isDead = true;
                enemyAI.Die();
            }
            else
            {
                enemyAI.TakeDamage(knockbackDirection);
            }
        }
    }
}
