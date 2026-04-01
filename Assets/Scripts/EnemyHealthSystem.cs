using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    [SerializeField] private float health;
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(health);
    }
}
