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
            PlayerHealthSystem player = other.GetComponent<PlayerHealthSystem>();
            player.TakeDamage(shootDamage, transform.forward);
        }
        Destroy(gameObject);
    }
}
