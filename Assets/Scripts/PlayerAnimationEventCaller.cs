using UnityEngine;

public class PlayerAnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Collider wandCollider;
    [SerializeField] private PlayerDealDamage _dealDamage;
    [SerializeField] private  Animator _anim;
    public void EnableWandCollider()
    {
        wandCollider.enabled = true;
    }
    public void DisableWandCollider()
    {
        wandCollider.enabled = false;
    }
    public void endHit()
    {
        _anim.SetBool("_isHitting", false);
    }
    public void callShoot()
    {
        _dealDamage.Shoot();
    }
    public void endShoot()
    {
        _anim.SetBool("_isShooting", false);
    }
    public void endDash()
    {
        _anim.SetBool("_isDashing", false);
    }
}
