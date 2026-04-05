using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackDamage=10;
    [SerializeField] private Collider attackCollider;
    public LayerMask targetLayers;

    private void OnTriggerEnter(Collider other)
    {
        if (targetLayers == (targetLayers | (1 << other.gameObject.layer)))
        {
            PlayerHealthSystem player = other.GetComponentInParent<PlayerHealthSystem>();
            player.TakeDamage(attackDamage, transform.forward);
        }
        attackCollider.enabled = false;
    }
}
