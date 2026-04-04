using UnityEngine;

public class BatAnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider batCollider;
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
    public void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.Euler(0,0,90));
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        bullet.GetComponent<EnemyBullet>().SetParentCollider(batCollider);
    }
}
