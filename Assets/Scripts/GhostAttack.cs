using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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
            player.TakeDamage(attackDamage, transform.forward);
        }
        attackCollider.enabled = false;
    }
}
