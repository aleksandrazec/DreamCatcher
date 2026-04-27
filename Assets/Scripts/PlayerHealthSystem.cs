using System.Collections;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private PlayerController controller;
    [SerializeField] private float damageCooldown=0f;
    [SerializeField] private HealthBar healthBar;

    private float health;
    private bool canBeDamaged;
    public bool isDead;
    private void Awake()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        canBeDamaged = true;
        isDead = false;
    }
    public void TakeDamage(float damage, Vector3 knockbackDirection)
    {
        if (canBeDamaged)
        {
            health -= damage;
            healthBar.SetHealth(health);
            Debug.Log("Player health: "+health);
            if (health <= 0)
            {
                controller.Die(knockbackDirection);
                 canBeDamaged = false;
                isDead = true;
            }
            else
            {
                controller.TakeDamage(knockbackDirection);
                StartCoroutine(DamageCooldown());
            }
        }
    }
    private IEnumerator DamageCooldown()
    {
        canBeDamaged = false;
        yield return new WaitForSeconds(damageCooldown);
        canBeDamaged = true;
    }
}
