using UnityEngine;

public class BatAnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private EnemyBullet bulletPrefab;
    public float bulletSpeed = 10f;
    public float bulletDamage=0;
    [SerializeField] private Animator animator;
    public GameObject player;
    private void Awake()
    {
        GameObject[] playerObject = GameObject.FindGameObjectsWithTag("Player");
        player=playerObject[0];
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
    public void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.Euler(0,0,90));
        //var heading = player.transform.position + new Vector3(0,10,0) - transform.position;
        //var distance = heading.magnitude;
        //var direction = heading / distance;
        if (bulletDamage > 0)
        {
            bullet.SetShootDamage(bulletDamage);
        }
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        
    }
}
