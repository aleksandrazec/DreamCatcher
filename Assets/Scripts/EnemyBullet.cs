using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float life = 3;
    [SerializeField] private float shootDamage;

    private void Awake()
    {
        Destroy(gameObject, life);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem player = other.GetComponent<HealthSystem>();
            player.TakeDamage(shootDamage);
        }
        Destroy(gameObject);
    }
}
