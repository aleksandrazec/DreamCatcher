using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private PlayerController controller;
    [SerializeField] private float damageCooldown=0f;
    [SerializeField] private HealthBar healthBar;
    public TMP_Text text;

    private float health;
    private bool canBeDamaged;
    public bool isDead;
    private void Awake()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        canBeDamaged = true;
        isDead = false;
        text.text = health + "/" + maxHealth;
    }
    public void TakeDamage(float damage, Vector3 knockbackDirection)
    {
        if (canBeDamaged)
        {
            health -= damage;
            text.text = health + "/" + maxHealth;
            healthBar.SetHealth(health);
            Debug.Log("Player health: "+health);
            if (health <= 0)
            {
                text.text = 0 + "/" + maxHealth;
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
