using UnityEngine;

public class PlayerAnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Collider wandCollider;
    PlayerDealDamage _dealDamage;
    private Animator _anim;
    private void Awake()
    {
        _dealDamage = gameObject.GetComponentInChildren<PlayerDealDamage>();
        _anim = GetComponent<Animator>();
    }
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
}
