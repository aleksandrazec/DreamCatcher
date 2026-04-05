using UnityEngine;

public class PlayerAnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Collider attackCollider;
    [SerializeField] private PlayerShoot playerShoot;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerAttack playerAttack;
    public void EnableWandCollider()
    {
        attackCollider.enabled = true;
    }
    public void DisableWandCollider()
    {
        attackCollider.enabled = false;
        playerAttack.AttackEnded();
    }
    public void endHit()
    {
        anim.SetBool("isHitting", false);
    }
    public void callShoot()
    {
        playerShoot.Shoot();
    }
    public void endShoot()
    {
        anim.SetBool("isShooting", false);
    }
    public void endDash()
    {
        anim.SetBool("isDashing", false);
    }
    public void endDamage()
    {
        anim.SetBool("isDamaged", false);
    }
}
