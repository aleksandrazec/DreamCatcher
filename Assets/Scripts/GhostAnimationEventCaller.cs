using UnityEngine;

public class GhostAnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Collider hitCollider;
    [SerializeField] private Animator animator;

    private void EnableCollider()
    {
        hitCollider.enabled = true;
    }

    private void DisableCollider()
    {
        hitCollider.enabled = false;
    }
    private void EndAttack()
    {
        animator.SetBool("isAttacking", false);
    }
    private void EndDamage()
    {
        animator.SetBool("isDamaged", false);
    }
    private void EndDead()
    {
        animator.SetBool("isDead", false);
    }
}
