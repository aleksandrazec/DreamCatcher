using UnityEngine;

public class GhostAttack : MonoBehaviour
{
    [SerializeField] private float attackDamage=10;
    [SerializeField] private Collider attackCollider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Attacked");
            HealthSystem player = other.GetComponent<HealthSystem>();
            player.TakeDamage(attackDamage);
        }
        attackCollider.enabled = false;
    }
}
