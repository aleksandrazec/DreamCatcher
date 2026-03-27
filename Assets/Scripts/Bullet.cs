using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 3;
    [SerializeField] private float shootDamage=10f;


    private void Awake()
    {
        Destroy(gameObject, life);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.GetComponent<HealthSystem>();
            enemy.TakeDamage(shootDamage);
        }
        Destroy(gameObject);
    }
}
