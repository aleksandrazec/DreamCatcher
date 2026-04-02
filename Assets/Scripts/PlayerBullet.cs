using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float life = 3;
    [SerializeField] private float shootDamage;

    private void Awake()
    {
        Destroy(gameObject, life);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealthSystem enemy = other.GetComponent<EnemyHealthSystem>();
            enemy.TakeDamage(shootDamage, transform.forward);
        }
        Destroy(gameObject);
    }
}
