using UnityEngine;

public class WormAnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Animator animator;
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
