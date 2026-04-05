using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float life = 3;
    [SerializeField] private float shootDamage;
    public LayerMask targetLayers;
    private void Awake()
    {
        Destroy(gameObject, life);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (targetLayers == (targetLayers | (1 << other.gameObject.layer)))
        {
            EnemyHealthSystem enemy = other.GetComponentInParent<EnemyHealthSystem>();
            enemy.TakeDamage(shootDamage, transform.forward);
        }
        Destroy(gameObject);
    }
}
