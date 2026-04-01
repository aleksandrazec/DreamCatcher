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
            PlayerHealthSystem player = other.GetComponent<PlayerHealthSystem>();
            player.TakeDamage(attackDamage);
        }
        attackCollider.enabled = false;
    }
}
