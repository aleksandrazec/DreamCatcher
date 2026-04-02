using System.Collections;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private PlayerController controller;
    [SerializeField] private float damageCooldown=0f;
    
    
    private bool canBeDamaged;
    public bool isDead;
    private void Awake()
    {
        canBeDamaged = true;
        isDead = false;
    }
    public void TakeDamage(float damage, Vector3 knockbackDirection)
    {
        if (canBeDamaged)
        {
            health -= damage;
            Debug.Log(health);
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
