using UnityEngine;

public class EnemyBullet : MonoBehaviour
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
            PlayerHealthSystem player = other.GetComponentInParent<PlayerHealthSystem>();
            player.TakeDamage(shootDamage, transform.forward);
        }
        Destroy(gameObject);
    }
}
