using UnityEngine;

public class AnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Collider wandCollider;
    DealDamage _dealDamage;
    private void Awake()
    {
        _dealDamage = gameObject.GetComponentInChildren<DealDamage>();
    }
    public void EnableWandCollider()
    {
        wandCollider.enabled = true;
    }
    public void DisableWandCollider()
    {
        wandCollider.enabled = false;
    }
    public void callShoot()
    {
        _dealDamage.Shoot();
    }
}
